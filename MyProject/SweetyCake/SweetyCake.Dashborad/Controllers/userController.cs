using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OutbornE_commerce.BAL.Dto.Profile;
using OutbornE_commerce.BAL.Dto;
using OutbornE_commerce.BAL.Repositories.UserRepo;
using OutbornE_commerce.DAL.Enums;
using OutbornE_commerce.DAL.Models;

namespace SweetyCake.Dashborad.Controllers
{
    public class userController : DashboardBaseController
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<User> userManager;
        private readonly IUserReposatry _userReposatry;

        public userController(RoleManager<IdentityRole> roleManager, UserManager<User> userManager, IUserReposatry userReposatry)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
            _userReposatry = userReposatry;
        }
        public async Task<IActionResult> Index(string? searchTerm, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var staffUsersPagination = await _userReposatry.GetPaginatedStaffUsersAsync(searchTerm, pageNumber, pageSize);

                return View(staffUsersPagination);

            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
    }
}
