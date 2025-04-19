
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using OutbornE_commerce.BAL.AuthServices;
using OutbornE_commerce.Extensions;
using System.Security.Claims;
using System.Text;
using OutbornE_commerce.BAL.External_Logins;
using OutbornE_commerce.BAL.EmailServices;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.CodeAnalysis.Options;
using Microsoft.Extensions.Options;
using OutbornE_commerce.BAL.Extentions;
using System.Text.RegularExpressions;
using PreMailer.Net;

namespace OutbornE_commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IAuthService _authService;
        private readonly SignInManager<User> signInManager;
        private readonly IConfiguration Configuration;
        private readonly IEmailSenderCustom emailSender;
        private readonly FrontBaseUrlSettings FrontBaseUrl;
        private readonly IHostEnvironment Environment;
        private readonly IWebHostEnvironment webHostEnvironment;

        public AuthController(IWebHostEnvironment env, UserManager<User> userManager, IAuthService authService,
             SignInManager<User> signInManager,
            IConfiguration _configuration, IEmailSenderCustom emailSender,
            IOptions<FrontBaseUrlSettings> option, IHostEnvironment _env)
        {
            Environment = _env;
            _userManager = userManager;
            _authService = authService;
            this.signInManager = signInManager;
            Configuration = _configuration;
            this.emailSender = emailSender;
            FrontBaseUrl = option.Value;
            webHostEnvironment = env;
        }

        [HttpPost("registerUser")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterModel userForRegistration)
        {
            var checkByEmail = await _userManager.FindByEmailAsync(userForRegistration.Email);
            if (checkByEmail != null)
            {
                return Ok(new AuthResponseModel
                {
                    MessageEn = "Email already Exist",
                    MessageAr = "هذا البريد موجود من قبل",
                    IsError = true,
                    Email = userForRegistration.Email,
                });
            }

            var user = userForRegistration.Adapt<User>();
            user.UserName = userForRegistration.Email;
            user.FullName = $"{userForRegistration.FirstName} {userForRegistration.LastName}";
            try
            {

                var result = await _userManager.CreateAsync(user, userForRegistration.Password!);
                if (!result.Succeeded)
                {
                    return Ok(new AuthResponseModel
                    {
                        MessageEn = result.Errors.FirstOrDefault().Description,
                        MessageAr = "حدث خطأ",
                        IsError = true,
                        Email = userForRegistration.Email,
                    });
                }

                var GeneratedToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                var confirmationLink = Url.Action("ConfirmEmail", "Auth", new { userId = user.Id, GeneratedToken }, Request.Scheme);

                await emailSender.SendEmailAsync(user.Email, "Confirm Email", confirmationLink);

                return Ok(new AuthResponseModel
                {
                    MessageEn = "SuccesFully Added",
                    MessageAr = "تم الاضافة بنجاح",
                    IsError = false,
                    Email = userForRegistration.Email,
                    Id = user.Id,
                });
            }
            catch (Exception ex)
            {
                return Ok(new AuthResponseModel
                {
                    IsError = true,
                    MessageEn = ex.Message,
                });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Authenticate([FromBody] UserForLoginDto user)
        {
            if (user == null)
                return Ok("Email Or Password Incorrect");
            try
            {
                var validate = await _authService.ValidateUser(user);
                return Ok(validate);
            }
            catch (Exception)
            {
                return (BadRequest(new
                {
                    MessageAr = "حدث خطأ برجاء المحاولة مرة اخري",
                    MessageEn = "Error Please Try Again"
                }));
            }
        }

        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string userId, string GeneratedToken)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(GeneratedToken))
                return BadRequest("User ID or token is invalid.");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound("User not found.");

            var result = await _userManager.ConfirmEmailAsync(user, GeneratedToken);
            if (result.Succeeded)
            {

                await emailSender.SendEmailAsync(user.Email, "Welcome", "Welcome to Sweety’s Community");

                return Ok("Email confirmed successfully.");

            }

            return BadRequest("Email confirmation failed.");
        }

        [HttpPost("ChangePassword")]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ResetPasswordModelDto model)
        {
            string userId = User.GetUserIdFromToken();
            var user = await _userManager.FindByIdAsync(userId);

            if (user is null || !await _userManager.CheckPasswordAsync(user, model.OldPassword))
            {
                return Ok(new Response<string>
                {
                    Data = null,
                    IsError = true,
                    Message = "The Old Password Incorrect",
                    MessageAr = "كلمة المرور القديمة خاطئة",
                    Status = (int)StatusCodeEnum.BadRequest
                });
            }

            var hashPassword = _userManager.PasswordHasher.HashPassword(user, model.NewPassword);
            user.PasswordHash = hashPassword;
            var res = await _userManager.UpdateAsync(user);
            if (res.Succeeded)
            {
                return Ok(new Response<string>
                {
                    Message = "Success Reset Password ",
                    MessageAr = "تم تغيير كلمة السر بنجاح",
                    Data = user.Id,
                    IsError = false,
                    Status = (int)StatusCodeEnum.Ok
                });
            }
            else
            {
                return Ok(new Response<string>
                {
                    Data = null,
                    IsError = true,
                    Message = "Something went wrong",
                    MessageAr = "حدث خطأ",
                    Status = (int)StatusCodeEnum.ServerError
                });
            }
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return BadRequest("Invalid request.");

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            // var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            var BaseUrl = Environment.IsDevelopment() ? FrontBaseUrl.Local : FrontBaseUrl.Production;

            var callbackUrl = $"{BaseUrl}auth/reset-pass?token={token}&email={Uri.EscapeDataString(user.Email)}";

            await emailSender.SendEmailAsync(email, "Reset Password", $"Reset your password by <a href='{callbackUrl}'>clicking here</a>.");

            return Ok("Password reset link sent.");
        }

        [HttpPost("resetpassword")]
        public async Task<IActionResult> ResetPassword([FromQuery] string token, [FromQuery] string email, [FromBody] ResetPasswordModel model)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
                return BadRequest("Invalid password reset request.");

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return NotFound("Invalid Email Address");

            //var decodedTokenBytes = WebEncoders.Base64UrlDecode(token);
            //var decodedToken = Encoding.UTF8.GetString(decodedTokenBytes);

            var result = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);

            if (result.Succeeded)
            {
                return Ok(new Response<string>
                {
                    Data = "Password has been reset successfully.",
                    IsError = false,
                    Status = (int)StatusCodeEnum.Ok
                });
            }

            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return BadRequest(new Response<string>
            {
                Data = errors,
                IsError = true,
                Status = (int)StatusCodeEnum.BadRequest
            });
        }

        [HttpGet("signin-google")]
        public async Task<IActionResult> SignInGoogle([FromQuery] string code)
        {
            try
            {
                var ValidationToken = await _authService.LoginWithGoogleAsync(code);

                if (ValidationToken.IsSuccessLogin)
                {
                    return Ok(new Response<ExternalLoginResult>
                    {
                        Data = ValidationToken,
                        IsError = false,
                        Status = (int)StatusCodeEnum.Ok,
                        MessageAr = "تم تسجيل الدخول بنجاح"
                    });
                }
                else
                {
                    return Ok(new Response<ExternalLoginResult>
                    {
                        Data = ValidationToken,
                        IsError = true,
                        Status = (int)StatusCodeEnum.Ok,
                        MessageAr = "فشل تسجيل الدخول "
                    });
                }
            }
            catch
            {
                return Unauthorized(new Response<ExternalLoginResult>
                {
                    Data = null,
                    IsError = true,
                    Status = (int)StatusCodeEnum.Unauthorized,
                    Message = "Login Failed With Google",
                    MessageAr = " فشل تسجيل الدخول"
                });
            }
        }

        [HttpGet("signin-facebook")]
        public async Task<IActionResult> SignInFacebook([FromQuery] string code)
        {
            try
            {
                var ValidToken = await _authService.LoginWithFaceBookAsync(code);
                if (ValidToken.IsSuccessLogin)
                {
                    return Ok(new Response<ExternalLoginResult>
                    {
                        Data = ValidToken,
                        IsError = false,
                        Status = (int)StatusCodeEnum.Ok,
                        MessageAr = "تم تسجيل الدخول بنجاح"
                    });
                }
                else
                {
                    return Ok(new Response<ExternalLoginResult>
                    {
                        Data = ValidToken,
                        IsError = true,
                        Status = (int)StatusCodeEnum.Ok,
                        MessageAr = "فشل تسجيل الدخول "
                    });
                }
            }
            catch (Exception)
            {
                return Unauthorized(new Response<ExternalLoginResult>
                {
                    Data = null,
                    IsError = true,
                    Status = (int)StatusCodeEnum.Unauthorized,
                    Message = "Login Failed With Facebook",
                    MessageAr = " فشل تسجيل الدخول"
                });
            }
        }
    }
}