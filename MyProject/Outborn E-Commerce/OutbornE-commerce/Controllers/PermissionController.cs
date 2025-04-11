using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OutbornE_commerce.BAL.Dto;
using OutbornE_commerce.BAL.Dto.PermissionDTO;
using OutbornE_commerce.BAL.Dto.UserPermissionsDTO;
using OutbornE_commerce.BAL.Repositories.PermissionRepo;
using System.Linq.Expressions;
using System.Threading;

namespace OutbornE_commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IpermissionRepository _permissionRepository;

        public PermissionController(UserManager<User> userManager, IpermissionRepository permissionRepository)
        {
            _userManager = userManager;
            _permissionRepository = permissionRepository;

        }
       



        [HttpPost("AddPermission")]
        public async Task<IActionResult> AddPermission([FromBody] AddPermissionRequest request)
        {
            if (!Enum.IsDefined(typeof(Permission), request.Permission)&& !ModelState.IsValid)
            {
                return BadRequest(new Response<string>
                {
                    Data = null,
                    IsError = true,
                    Message = "Invalid permission value.",
                    MessageAr = "قيمه الصلاحية غير صالحة.",
                    Status = (int)StatusCodeEnum.BadRequest
                });
            }

            try
            {
                await _permissionRepository.AddPermission(request);

                return Ok(new Response<string>
                {
                    Data=request.Permission.ToString(),
                    IsError = false,
                    Message = "Created successfully.",
                    MessageAr = "تم الإضافة بنجاح.",
                    Status = (int)StatusCodeEnum.Ok
                });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new Response<string>
                {
                    Data = null,
                    IsError = true,
                    Message = ex.Message,
                    MessageAr = "هناك تعارض في البيانات.",
                    Status = (int)StatusCodeEnum.BadRequest
                });
            }
            catch (Exception ex)
            {
                return StatusCode((int)StatusCodeEnum.ServerError, new Response<string>
                {
                    Data = null,
                    IsError = true,
                    Message = "An unexpected error occurred.",
                    MessageAr = "حدث خطأ غير متوقع.",
                    Status = (int)StatusCodeEnum.ServerError
                });
            }
        }


        [HttpGet("GetPermissionById/{id}")]
        public async Task<IActionResult> GetPermissionById(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest(new { Message = "Invalid ID." });

            var item = await _permissionRepository.Find(p => p.Id == id);
            GetAllPermissionDto permissionDto = new GetAllPermissionDto();
            
            if (item == null)
                return NotFound(new { Message = "Permission not found." });
            permissionDto.permission=item.Permission.ToString();
            permissionDto.Id=item.Id;

             return Ok(new Response<GetAllPermissionDto>
            {
                Data = permissionDto,
                IsError = false,
                Message = "",
                Status = (int)StatusCodeEnum.Ok
            });
        }


        [HttpPut("UpdatePermission")]
        public async Task<IActionResult> UpdatePermission(UpdatePermissionDto model, CancellationToken cancellationToken)
        {
            
            if (!Enum.IsDefined(typeof(Permission), model.Permission))
            {
                return BadRequest(new Response<string>
                {
                    Data = null,
                    IsError = true,
                    Message = "Invalid permission value.",
                    Status = (int)StatusCodeEnum.BadRequest
                });
            }

            var permission = await _permissionRepository.Find(p => p.Id == model.Id, false);

            if (permission == null)
            {
                return NotFound(new Response<string>
                {
                    Data = null,
                    IsError = true,
                    Message = "Permission not found.",
                    MessageAr = "لم يتم العثور عن تصريح",
                    Status = (int)StatusCodeEnum.NotFound
                });
            }

            permission = model.Adapt(permission);
            permission.UpdatedBy = "admin";
            permission.UpdatedOn = DateTime.UtcNow;

            _permissionRepository.Update(permission);
            await _permissionRepository.SaveAsync(cancellationToken);

            return Ok(new Response<Guid>
            {
                Data = permission.Id,
                IsError = false,
                Message = "Permission updated successfully.",
                MessageAr = "تم التعديل بنجاح",
                Status = (int)StatusCodeEnum.Ok
            });
        }



        [HttpDelete("DeletePermission/{id}")]
        public async Task<IActionResult> DeletePermission(Guid id,CancellationToken cancellationToken)
        {
            var permission = await _permissionRepository.Find(p => p.Id == id, false);
            if (permission == null)
            {
                return NotFound(new Response<string>
                {
                    Data = null,
                    IsError = true,
                    Message = "Permission not found.",
                    MessageAr = "لم يتم العثور عن تصريح",
                    Status = (int)StatusCodeEnum.NotFound
                });
            }

            _permissionRepository.Delete(permission);
            await _permissionRepository.SaveAsync(cancellationToken);

            return Ok(new Response<Guid>
            {
                Data = id,
                IsError = false,
                Message = "Permission deleted successfully.",
                MessageAr = "تم حذف الإذن بنجاح.",

                Status = (int)StatusCodeEnum.Ok
            });
        }

        [HttpGet("GetAllPermissions")]
        public async Task<IActionResult> GetAllPermissions(int pageNumber = 1, int pageSize = 10, Permission? permissionFilter = null)
        {
            // Build the predicate
            Expression<Func<PermissionEntity, bool>> predicate = p => !permissionFilter.HasValue || p.Permission == permissionFilter;

            // Retrieve paginated data
            var items = await _permissionRepository.FindAllAsyncByPagination(predicate, pageNumber, pageSize);

            // Group by TypePermission
            var groupedData = items.Data
                .GroupBy(p => p.TypePermission)
                .Select(group => new BaseAllPermissionsDto
                {
                    TypePermission = group.Key.ToString(), // Convert TypePermission to string if needed
                    getAllPermissionDtos = group.Select(item => new GetAllPermissionDto
                    {
                        Id = item.Id,
                        permission = item.Permission.ToString()
                    }).ToList()
                })
                .ToList();

            // Prepare the response
            return Ok(new PaginationResponse<List<BaseAllPermissionsDto>>
            {
                Data = groupedData,
                IsError = false,
                Status = (int)StatusCodeEnum.Ok,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = items.TotalCount
            });
        }

    }


}


