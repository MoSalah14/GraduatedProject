using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OutbornE_commerce.BAL.Dto.Profile;
using OutbornE_commerce.Extensions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace OutbornE_commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly UserManager<User> _userManager;

        public ProfileController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            string userId = User.GetUserIdFromToken();
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest(new Response<ProfileDto>
                {
                    Data = null,
                    IsError = true,
                    Status = (int)StatusCodeEnum.BadRequest,
                    Message = "User ID is invalid.",
                    MessageAr = "برجاء تسجيل الدخول اولا"
                });
            }

            var user = await _userManager.Users
                .Include(e => e.Addresses)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return NotFound(new Response<ProfileDto>
                {
                    Data = null,
                    IsError = true,
                    Status = (int)StatusCodeEnum.NotFound,
                    Message = "User not found.",
                    MessageAr = "لم يتم العثور علي اسم المستخدم"
                });
            }

            var data = user.Adapt<ProfileDto>();

            return Ok(new Response<ProfileDto>
            {
                Data = data,
                IsError = false,
                Status = (int)StatusCodeEnum.Ok
            });
        }

        [HttpPut]
        public async Task<IActionResult> EditProfile([FromBody] ProfileUpdateDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new Response<string>
                {
                    Data = null,
                    IsError = true,
                    Status = (int)StatusCodeEnum.BadRequest,
                    Message = "Invalid profile data.",
                    MessageAr = "لم يتم العثور عن بروفيل",
                });
            }
            string userId = User.GetUserIdFromToken();

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound(new Response<string>
                {
                    Data = null,
                    IsError = true,
                    Status = (int)StatusCodeEnum.NotFound,
                    Message = "fail",
                    MessageAr = "لم يتم العثور عن مستخدم",
                });
            }

            user.FullName = model.FullName;
            user.PhoneNumberConfirmed = false;
            user.PhoneNumber = model.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return BadRequest(new Response<string>
                {
                    Data = null,
                    IsError = true,
                    Status = (int)StatusCodeEnum.BadRequest,
                    Message = "Failed to update profile",
                    MessageAr = "فشل التعديل",
                });
            }

            // Step 5: Return a successful response
            return Ok(new Response<string>
            {
                Data = user.Id,
                IsError = false,
                Status = (int)StatusCodeEnum.Ok,
                Message = "Profile updated successfully.",
                MessageAr = "تم التعديل بنجاح",
            });
        }
    }
}