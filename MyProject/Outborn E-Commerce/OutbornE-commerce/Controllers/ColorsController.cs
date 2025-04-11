using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OutbornE_commerce.BAL.Dto;
using OutbornE_commerce.BAL.Dto.Colors;
using OutbornE_commerce.BAL.Dto.ProductColors;
using OutbornE_commerce.BAL.Repositories.Colors;
using OutbornE_commerce.DAL.Models;
using System.Reflection.PortableExecutable;

namespace OutbornE_commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ColorsController : ControllerBase
    {
        private readonly IColorRepository _colorRepository;
        public ColorsController(IColorRepository colorRepository)
        {
            _colorRepository = colorRepository;
        }
        [HttpGet("StaticColor")]
        public IActionResult GetStaticColor()
        {
            return Ok(new ColorDto()
            {
                Hexadecimal = "#CCC",
                NameAr = "رمادي",
                NameEn = "Grey"
            });
        }
        [HttpGet]
        public async Task<IActionResult> GetAllColors()
        {
            var colors = await _colorRepository.FindAllAsync(null, false);
            var colorEntites = colors.Adapt<List<ColorDto>>();
            return Ok(new { data = colorEntites, message = "" });
        }
        [HttpGet("paginated")]
        public async Task<IActionResult> GetAllColorsPaginated(int pageNumber = 1, int pageSize = 10, string? searchTerm = null)
        {
            var items = new PagainationModel<IEnumerable<Color>>();

            if (string.IsNullOrEmpty(searchTerm))
                items = await _colorRepository.FindAllAsyncByPagination(null, pageNumber, pageSize);
            else
                items = await _colorRepository
                                    .FindAllAsyncByPagination(b => (b.NameAr.Contains(searchTerm)
                                                               || b.NameEn.Contains(searchTerm))
                                                               , pageNumber, pageSize);

            var data = items.Data.Adapt<List<ColorDto>>();

            return Ok(new PaginationResponse<List<ColorDto>>
            {
                Data = data,
                IsError = false,
                Status = (int)StatusCodeEnum.Ok,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = items.TotalCount
            });
        }
        [HttpGet("{Id}")]
        public async Task<IActionResult> GetColorById(Guid Id)
        {
            var color = await _colorRepository.Find(c => c.Id == Id, false);
            if (color == null)
                return Ok(new Response<ColorDto>
                {
                    Data = null,
                    Status = (int)StatusCodeEnum.NotFound,
                    IsError = true,
                    Message = $"Color with Id: {Id} doesn't exist in the database"
                }); ;
            var colorEntity = color.Adapt<ColorDto>();
            return Ok(new Response<ColorDto>
            {
                Data = colorEntity,
                Status = (int)StatusCodeEnum.Ok, // Success  
                IsError = false,
                Message = ""
            });
        }
        [HttpPost]
        public async Task<IActionResult> CreateColor([FromBody] ColorForCreationDto model, CancellationToken cancellationToken)
        {
            var color = model.Adapt<Color>();
            color.CreatedBy = "admin";
            var result = await _colorRepository.Create(color);
            await _colorRepository.SaveAsync(cancellationToken);
            return Ok(new Response<Guid>
            {
                Data = result.Id,
                Status = (int)StatusCodeEnum.Ok,
                IsError = false,
                Message = "created successfully",
                MessageAr = "تم الاضافه بنجاح",
            });
        }
        [HttpPut]
        public async Task<IActionResult> UpdateColor([FromBody] ColorDto model, CancellationToken cancellationToken)
        {
            var color = await _colorRepository.Find(c => c.Id == model.Id, false);
            if (color == null)
                return Ok(new { message = $"Color with Id: doesn't exist in the database" });
            color = model.Adapt<Color>();
            color.UpdatedBy = "admin";
            color.CreatedBy = "admin";
            _colorRepository.Update(color);
            await _colorRepository.SaveAsync(cancellationToken);
            return Ok(new Response<Guid>
            {
                Data = color.Id,
                Status = (int)StatusCodeEnum.Ok,
                IsError = false,
                Message = "updated successfully",
                MessageAr = "تم التعديل بنجاح ",
            });
        }
        [HttpDelete("{Id}")]
        public async Task<IActionResult> DeleteColor(Guid Id, CancellationToken cancellationToken)
        {
            try
            {
                string[] includes = new string[]
                   {
                       "ProductColor"
                   };
                var color = await _colorRepository.Find(c => c.Id == Id, false, includes);

                if (color == null)
                {
                    return Ok(new { message = $"Color with Id: {Id} doesn't exist in the database" });
                }

                var isColorUsed = color.ProductColor.Any();

                if (isColorUsed)
                {
                    return Ok(new Response<Guid>
                    {
                        Data = color.Id,
                        Status = (int)StatusCodeEnum.BadRequest,
                        IsError = true,
                        Message = "The color cannot be deleted because it is associated with products.",
                        MessageAr = "لا يمكن حذف اللون لأنه مرتبط بمنتجات.",
                    });
                }

                color.IsDeleted = true;
                _colorRepository.Update(color);

                await _colorRepository.SaveAsync(cancellationToken);

                return Ok(new Response<Guid>
                {
                    Data = color.Id,
                    Status = (int)StatusCodeEnum.Ok,
                    IsError = false,
                    Message = "Deleted successfully",
                    MessageAr = "تم الحذف بنجاح",
                });
            }
            catch (Exception ex)
            {
                return StatusCode((int)StatusCodeEnum.ServerError, new Response<string>
                {
                    Data = null,
                    Status = (int)StatusCodeEnum.ServerError,
                    IsError = true,
                    Message = "An error occurred while deleting the color",
                    MessageAr = "حدث خطأ أثناء حذف اللون",
                });
            }

        }
    }
}
