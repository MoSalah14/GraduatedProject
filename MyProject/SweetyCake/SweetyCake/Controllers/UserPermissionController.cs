using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OutbornE_commerce.BAL.Dto.PermissionDTO;
using OutbornE_commerce.BAL.Dto.UserPermissionsDTO;
using OutbornE_commerce.BAL.Repositories.UserPermissionRepo;
using OutbornE_commerce.DAL.Models;
using System.Linq.Expressions;

namespace OutbornE_commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserPermissionController : ControllerBase
    {
        private readonly IUserPermissionRepo _userPermissionRepository;

        public UserPermissionController(IUserPermissionRepo userPermissionRepository)
        {
            _userPermissionRepository = userPermissionRepository;
        }

        [HttpGet("GetAllUserPermissions")]
        public async Task<IActionResult> GetAllUserPermissions(int pageNumber = 1, int pageSize = 10, string? userId = null)
        {
            // Define the filter expression: If `userId` is provided, filter by it; otherwise, retrieve all
            Expression<Func<UserPermission, bool>> predicate = up => string.IsNullOrEmpty(userId) || up.UserId == userId;

            string[] includes = new string[] { "User", "Permission" };

            var items = await _userPermissionRepository.FindAllAsyncByPagination(predicate, pageNumber, pageSize, includes);

            var groupedPermissions = items.Data
                .GroupBy(up => new { up.UserId, UserName = up.User.FullName })
                .Select(g => new GetUserPermissionDto
                {
                    UserId = g.Key.UserId,
                    UserName = g.Key.UserName,
                    Permissions = g.Select(up => new PermissionDetailDto
                    {
                        PermissionId = up.PermissionId,
                        PermissionName = up.Permission.Permission.ToString() 
                    }).ToList()
                }).ToList();

            // Return paginated response
            return Ok(new PaginationResponse<List<GetUserPermissionDto>>
            {
                Data = groupedPermissions,
                IsError = false,
                Status = (int)StatusCodeEnum.Ok,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = items.TotalCount
            });
        }


        [HttpGet("GetUserWithPermissions/{UserId}")]

        public async Task<IActionResult> GetUserWithPermissions(string UserId)
        {
            // Retrieve all UserPermission records for the given UserId
            var userPermissions = await _userPermissionRepository.FindByCondition(up => up.UserId == UserId, includes: new[] { "User", "Permission" });

            if (userPermissions == null || !userPermissions.Any())
                return NotFound(new { Message = "User or permissions not found." });

            // Map the user and permissions to the DTO structure
            var firstUserPermission = userPermissions.First();
            var result = new GetUserPermissionDto
            {
                UserId = firstUserPermission.UserId,
                UserName = firstUserPermission.User.FullName,
                Permissions = userPermissions.Select(up => new PermissionDetailDto
                {
                    PermissionId = up.Permission.Id,
                    PermissionName = up.Permission.Permission.ToString() 
                }).ToList()
            };
        
         return Ok(new Response<GetUserPermissionDto>
    {
        Data = result,
        IsError = false,
        Message = "User and permissions retrieved successfully.",
        Status = (int) StatusCodeEnum.Ok
    });
}



              [HttpPost("AssignPermission")]
        public async Task<IActionResult> AssignPermission([FromBody] AssignPermissionDTO request)
        {
            try
            {
              
                await _userPermissionRepository.AssignPermissionsToUser(request.Email, request.PermissionId);
                return Ok(new Response<string>
                {
                    Data = null,
                    IsError = false,
                    Message = "Created successfully",
                    MessageAr = "تم الاضافه بنجاح",
                    Status = (int)StatusCodeEnum.Ok
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { Error = ex.Message });
            }
        }

        //[HttpPut("UpdatePermissions")]
        //public async Task<IActionResult> UpdateUserPermissions([FromBody] UpdateUserPermissionDto request)
        //{
        //    try
        //    {
        //        await _userPermissionRepository.UpdatePermissionsForUser(request.Email, request.PermissionIds);
        //        return Ok(new { Message = "Permissions updated successfully" });
        //    }
        //    catch (ArgumentException ex)
        //    {
        //        return BadRequest(new { Error = ex.Message });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new { Error = "An error occurred while updating permissions." });
        //    }
        //}


        [HttpDelete("DeleteUserPermission")]
        public async Task<IActionResult> DeleteUserPermission(string UserId, CancellationToken cancellationToken)
        {

            var userPermissions = await _userPermissionRepository.FindByCondition(up => up.UserId == UserId, includes: new[] { "User", "Permission" });
            if (userPermissions == null || !userPermissions.Any())
            {
                return NotFound(new Response<string>
                {
                    Data = null,
                    IsError = true,
                    Message = "User permissions not found.",
                    MessageAr = "لم يتم العثور عن تصريح",
                    Status = (int)StatusCodeEnum.NotFound
                });

            }
            
            foreach (var userPermission in userPermissions)
            {
                _userPermissionRepository.Delete(userPermission);
            }

            await _userPermissionRepository.SaveAsync(cancellationToken);

            return Ok(new Response<string>
            {
              
                IsError = false,
                Message = "User permissions deleted successfully.",
                MessageAr = "تم  الحذف بنجاح",
                Status = (int)StatusCodeEnum.Ok
            });
        }
    }
}
    

