using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using OutbornE_commerce.BAL.Dto;

using OutbornE_commerce.BAL.Repositories.PermissionRepo;

using OutbornE_commerce.BAL.Dto.Eternal_Logins;
using OutbornE_commerce.BAL.External_Logins;

using OutbornE_commerce.DAL.Enums;
using OutbornE_commerce.DAL.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace OutbornE_commerce.BAL.AuthServices
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IpermissionRepository _permissionRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ExternalLoginAuth externalLogins;
        private readonly HttpClient httpClient;
        private User? _user;
        private string GoogleRedirectUrl;
        private string FaceBookRedirectUrl;
        private readonly string BaseUrl;

        public AuthService(UserManager<User> userManager, IConfiguration configuration,
            IHttpClientFactory httpClientFactory, IpermissionRepository permissionRepository,
            IHttpContextAccessor httpContextAccessor
            , IOptions<ExternalLoginAuth> ExternalLogins)
        {
            _permissionRepository = permissionRepository;
            _httpContextAccessor = httpContextAccessor;
            externalLogins = ExternalLogins.Value;
            _userManager = userManager;
            _configuration = configuration;
            httpClient = httpClientFactory.CreateClient();

            GoogleRedirectUrl = $"https://deemaabdo.com/auth";
            FaceBookRedirectUrl = $"https://deemaabdo.com/auth?isFacebook=facebook";
        }

        public async Task<AuthResponseModel?> ValidateUser(UserForLoginDto userForAuth)
        {
            _user = await _userManager.FindByEmailAsync(userForAuth.Email);
            AuthResponseModel AuthResponse = new AuthResponseModel();

            if (_user == null || !await _userManager.CheckPasswordAsync(_user, userForAuth.Password!))
            {
                return new AuthResponseModel
                {
                    Email = userForAuth.Email,
                    IsError = true,
                    MessageAr = "البريد الالكترونى او كلمة السر غير صحيحين",
                    MessageEn = "Email or Password not correct",
                    StatusCode = (int)StatusCodeEnum.BadRequest
                };
            }

            if (!_user.EmailConfirmed)
                return new AuthResponseModel
                {
                    Email = userForAuth.Email,
                    IsError = true,
                    MessageAr = "برجاء تأكيد البريد الالكترونى",
                    MessageEn = "Email Not Confirmed",
                    StatusCode = (int)StatusCodeEnum.Ok
                };

            var token = await CreateToken();
            var roles = await _userManager.GetRolesAsync(_user!);

            return new AuthResponseModel
            {
                Id = _user.Id,
                Email = userForAuth.Email,
                IsError = false,
                MessageAr = "تم تسجيل الدخول بنجاح ",
                MessageEn = "Login Successfully",
                StatusCode = (int)StatusCodeEnum.Ok,
                Token = token,
                Roles = roles.ToList(),
                AccountType = _user.AccountType
            };
        }

        public async Task<string> CreateToken()
        {
            var signingCredentials = GetSigningCredentials();
            var claims = await GetClaims();
            var tokenOptions = GenerateTokenOptions(signingCredentials, claims);
            return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        }

        private SigningCredentials GetSigningCredentials()
        {
            var key = Encoding.UTF8.GetBytes("HQDshfnnystWB3Ff4tKeQx3d0aIR2uoEurrknFhsyjA");
            var secret = new SymmetricSecurityKey(key);
            return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
        }

        private async Task<List<Claim>> GetClaims()
        {
            var claims = new List<Claim> {
                new Claim("userName", _user.FullName!),
                new Claim("uid", _user.Id!),
                new Claim("email", _user.Email!),
            };
            var roles = await _userManager.GetRolesAsync(_user);

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            var Permissions = await _permissionRepository.GetPermissionsUser(_user.Id);
            foreach (var permission in Permissions)
            {
                claims.Add(new Claim("permissions", permission.Permission.ToString()));
            }

            return claims;
        }

        private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
        {
            var jwtSettings = _configuration.GetSection("JWT");
            var tokenOptions = new JwtSecurityToken(
                issuer: jwtSettings.GetSection("Issuer").Value,
                audience: jwtSettings.GetSection("Audience").Value,
                claims: claims,
                expires: DateTime.Now.AddMonths(Convert.ToInt32(jwtSettings.GetSection("expires").Value)),
                signingCredentials: signingCredentials
                );
            return tokenOptions;
        }

        public async Task<ExternalLoginResult> LoginWithFaceBookAsync(string accessToken)
        {
            try
            {
                var tokenUrl = $"https://graph.facebook.com/v12.0/oauth/access_token?client_id=" +
                    $"{externalLogins.Facebook.ClientId}&redirect_uri={FaceBookRedirectUrl}&client_secret={externalLogins.Facebook.ClientSecret}&code={accessToken}";

                var response = await httpClient.GetStringAsync(tokenUrl);

                FacebookTokenResponse? tokenResponse = JsonConvert.DeserializeObject<FacebookTokenResponse>(response);

                var userInfo = await GetFacebookUserInfo(tokenResponse.AccessToken);
                if (userInfo == null)
                {
                    return new ExternalLoginResult
                    {
                        IsSuccessLogin = false,
                        Token = null,
                        ReturnedMessage = "Failed to Login with Google"
                    };
                }

                _user = await _userManager.FindByEmailAsync(userInfo.Email);
                if (_user == null)
                {
                    _user = new User
                    {
                        Id = Guid.NewGuid().ToString(),
                        Email = userInfo.Email,
                        UserName = userInfo.Email,
                        FullName = userInfo.FirstName + " " + userInfo.LastName,
                        ProfilePicture = userInfo.Picture.Url
                    };
                    var Result = await _userManager.CreateAsync(_user);

                    if (!Result.Succeeded)
                    {
                        return new ExternalLoginResult
                        {
                            IsSuccessLogin = false,
                            Token = null,
                            ReturnedMessage = "Failed to Create User"
                        };
                    }
                    await _userManager.AddToRoleAsync(_user, "User");
                }
                var token = await CreateToken();
                return new ExternalLoginResult
                {
                    IsSuccessLogin = true,
                    Token = token,
                    ReturnedMessage = "Successfully logged in with FaceBook."
                };
            }
            catch
            {
                return new ExternalLoginResult
                {
                    IsSuccessLogin = false,
                    Token = null,
                    ReturnedMessage = "Something Went Wrong"
                };
            }
        }

        public async Task<ExternalLoginResult> LoginWithGoogleAsync(string code)
        {
            try
            {
                var tokenData = await GetGoogleTokenAsync(code);
                if (tokenData == null)
                {
                    return new ExternalLoginResult
                    {
                        IsSuccessLogin = false,
                        Token = null,
                        ReturnedMessage = "Failed to Login with Google"
                    };
                }

                var accessToken = tokenData.UserToken;
                var payload = await GoogleJsonWebSignature.ValidateAsync(accessToken, new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[] { externalLogins.Google.ClientId }
                });

                _user = await _userManager.FindByEmailAsync(payload.Email);
                if (_user == null)
                {
                    _user = new User
                    {
                        Id = Guid.NewGuid().ToString(),
                        Email = payload.Email,
                        UserName = payload.Email,
                        FullName = payload.Name,
                        ProfilePicture = payload.Picture
                    };
                    var Result = await _userManager.CreateAsync(_user);
                    if (!Result.Succeeded)
                    {
                        return new ExternalLoginResult
                        {
                            IsSuccessLogin = false,
                            Token = null,
                            ReturnedMessage = "Failed to Create User"
                        };
                    }

                    await _userManager.AddToRoleAsync(_user, "User");
                }

                var token = await CreateToken();
                return new ExternalLoginResult
                {
                    IsSuccessLogin = true,
                    Token = token,
                    ReturnedMessage = "Successfully logged in with Google."
                };
            }
            catch
            {
                return new ExternalLoginResult
                {
                    IsSuccessLogin = false,
                    Token = null,
                    ReturnedMessage = "Something Went Wrong"
                };
            }
        }

        private async Task<GoogleTokenResponse> GetGoogleTokenAsync(string code)
        {
            var tokenRequest = new HttpRequestMessage(HttpMethod.Post, "https://oauth2.googleapis.com/token")
            {
                Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "code", code },
                { "client_id",  externalLogins.Google.ClientId},
                { "client_secret", externalLogins.Google.ClientSecret},
                { "redirect_uri", GoogleRedirectUrl},
                { "grant_type", "authorization_code" }
            })
            };

            var response = await httpClient.SendAsync(tokenRequest);
            if (!response.IsSuccessStatusCode)
                return null;

            var jsonResponse = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<GoogleTokenResponse>(jsonResponse);
        }

        private async Task<FacebookUserInfo?> GetFacebookUserInfo(string accessToken)
        {
            var response = await httpClient.GetStringAsync($"https://graph.facebook.com/me?fields=id,first_name,last_name,picture,email&access_token={accessToken}");
            FacebookUserInfo? UserInfo = JsonConvert.DeserializeObject<FacebookUserInfo>(response);
            if (UserInfo != null)
                return UserInfo;
            else
                return null;
        }
    }
}