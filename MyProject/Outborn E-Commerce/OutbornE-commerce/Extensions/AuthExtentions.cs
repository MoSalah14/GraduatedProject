using OutbornE_commerce.BAL.Dto;
using OutbornE_commerce.DAL.Enums;
using OutbornE_commerce.DAL.Models;

namespace OutbornE_commerce.Extensions
{
    public static class AuthExtentions
    {
        public static AuthResponseModel GetAuthResponse(this User user, List<string> roles , string messageEn ,string messageAr ,int statusCode,string? token)
        {
            if (statusCode != (int)StatusCodeEnum.Ok)
            {
                return new AuthResponseModel()
                {
                    StatusCode = statusCode,
                    Id = null,
                    IsError = true,
                    Token = null,
                    MessageEn = messageEn,
                    MessageAr = messageAr,
                    Permissions = null,
                    Roles = null,
                    RefreshToken = null,
                    Email = null,
                };
            }

            return new AuthResponseModel()
            {
                Id = user.Id,
                Roles = roles,
                Token = token,
                IsError = false,
                RefreshToken = "beta",
                MessageEn = messageEn,
                MessageAr = messageAr,
                StatusCode = statusCode,
                Permissions = null,
                Email = user.Email
            };
        }
    }
}
