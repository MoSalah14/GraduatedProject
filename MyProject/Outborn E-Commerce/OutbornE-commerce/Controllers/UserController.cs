using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OutbornE_commerce.BAL.Dto.OrderDto;
using OutbornE_commerce.BAL.Dto.Profile;
using OutbornE_commerce.BAL.Repositories.UserRepo;
using OutbornE_commerce.Extensions;

namespace OutbornE_commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<User> userManager;
        private readonly IUserReposatry _userReposatry;

        public UserController(RoleManager<IdentityRole> roleManager, UserManager<User> userManager, IUserReposatry userReposatry)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
            _userReposatry = userReposatry;
        }

        [HttpGet("GetUsersForDisplay")]
        public async Task<IActionResult> GetUsersForDisplay(string? IteemSearch, int pageNumber = 1, int pageSize = 10)
        {
            var users = await _userReposatry.GetUsersForDisplayAsync(IteemSearch, pageNumber, pageSize);
            return Ok(users);
        }

        [HttpGet("staff-users")]
        public async Task<IActionResult> GetStaffUsersAsync(string? searchTerm, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var staffUsersPagination = await _userReposatry.GetPaginatedStaffUsersAsync(searchTerm, pageNumber, pageSize);

                return Ok(new Response<PagainationModel<List<StaffUserDto>>>
                {
                    Data = staffUsersPagination,
                    IsError = false,
                    Status = (int)StatusCodeEnum.Ok,
                    Message = "Users retrieved successfully.",
                    MessageAr = "تم استرجاع المستخدمين بنجاح.",
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Response<string>
                {
                    Data = null,
                    IsError = true,
                    Status = (int)StatusCodeEnum.ServerError,
                    Message = "An error occurred while fetching staff users.",
                    MessageAr = "حدث خطأ أثناء جلب المستخدمين.",
                });
            }
        }

        //[HttpGet("emailsUser")]
        //public async Task<IActionResult> GetUserEmails()
        //{
        //    try
        //    {
        //        var emails = await _userReposatry.GetUsersEmailAsync();

        //        if (emails == null || !emails.Any())
        //        {
        //            return NotFound(new Response<string>
        //            {
        //                Data = null,
        //                IsError = true,
        //                Status = (int)StatusCodeEnum.NotFound,
        //                Message = "No user emails found.",
        //                MessageAr = "لم يتم العثور عن مستخدم",
        //            });
        //        }

        //        return Ok(new Response<List<string>>
        //        {
        //            Data = emails,
        //            IsError = false,
        //            Status = (int)StatusCodeEnum.Ok,
        //            Message = "User emails retrieved successfully.",
        //             MessageAr = "تم الارجاع بنجاح",
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new Response<string>
        //        {
        //            Data = null,
        //            IsError = true,
        //            Status = (int)StatusCodeEnum.ServerError,
        //            Message = "An error occurred while retrieving user emails."
        //        });
        //    }
        //}

        [HttpGet("usernames")]
        public IActionResult GetAllUsernames()
        {
            var usernames = userManager.Users
                .Select(u => u.UserName)
                .ToList();

            return Ok(usernames);
        }

        [HttpGet("emailsUser")]
        public IActionResult GetUserEmails()
        {
            var emails = userManager.Users
                .Select(u => u.Email)
                .ToList();

            return Ok(emails);
        }

        [HttpGet("emailsUserStuff")]
        public async Task<IActionResult> emailsUserStuff()
        {
            var users = await userManager.Users.ToListAsync();

            var staffUsers = new List<string>();

            foreach (var user in users)
            {
                var roles = await userManager.GetRolesAsync(user);
                if (roles.Contains("Stuff"))
                {
                    staffUsers.Add(user.Email);
                }
            }

            return Ok(staffUsers);
        }

        [HttpDelete("DeleteUser")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound();

            // Soft delete the user
            user.IsDeleted = true;
            var result = await userManager.UpdateAsync(user);

            if (!result.Succeeded)
                return BadRequest(new Response<string>
                {
                    Data = userId,
                    IsError = true,
                    Message = "Removed failed",
                    MessageAr = "فشلت عملية المسح",
                    Status = (int)StatusCodeEnum.BadRequest
                });

            return Ok(new Response<string>
            {
                Data = userId,
                IsError = false,
                Message = " Removed successfully",
                MessageAr = "تم الازالة بنجاح",
                Status = (int)StatusCodeEnum.Ok
            });
        }

        [HttpPost("UpdateDefaultCurrenciesForUser")]
        [Authorize]
        public async Task<IActionResult> UpdateDefaultCurrenciesForUser(Guid CurrenciedId, CancellationToken cancellationToken)
        {
            var UserId = User.GetUserIdFromToken();

            var UpdateCurrencies = await _userReposatry.GetSpecificUserAsync(UserId, CurrenciedId, cancellationToken);
            if (UpdateCurrencies)
                return Ok();
            else
                return BadRequest();
        }
    }
}