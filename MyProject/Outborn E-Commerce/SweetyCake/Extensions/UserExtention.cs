using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OutbornE_commerce.BAL.Dto.Currencies;
using OutbornE_commerce.DAL.Data;
using System.Security.Claims;

namespace OutbornE_commerce.Extensions
{
    public static class UserExtensions
    {
        public static string GetUserIdFromToken(this ClaimsPrincipal user) => user.FindFirstValue("uid")!;
        public static string GetUserFullName(this ClaimsPrincipal user) => user.FindFirstValue("userName")!;

    }
}
