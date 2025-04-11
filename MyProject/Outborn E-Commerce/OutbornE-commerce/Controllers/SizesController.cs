using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OutbornE_commerce.BAL.Dto.Colors;
using OutbornE_commerce.BAL.Dto.ProductColors;
using OutbornE_commerce.BAL.Dto.Sizes;
using OutbornE_commerce.BAL.Repositories.Sizes;
using OutbornE_commerce.DAL.Models;
using System.Net.Sockets;

namespace OutbornE_commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SizesController : ControllerBase
    {
        private readonly ISizeRepository _sizeRepository;

        public SizesController(ISizeRepository sizeRepository)
        {
            _sizeRepository = sizeRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSizes(int pageNumber = 1, int pageSize = 5, string? searchTerm = null)
        {
            var items = new PagainationModel<IEnumerable<Size>>();

            if (string.IsNullOrEmpty(searchTerm))
                items = await _sizeRepository.FindAllAsyncByPagination(null, pageNumber, pageSize);
            else
                items = await _sizeRepository
                                    .FindAllAsyncByPagination(b => (b.Name.Contains(searchTerm))
                                                               , pageNumber, pageSize);

            var data = items.Data.Adapt<List<SizeDto>>();

            return Ok(new PaginationResponse<List<SizeDto>>
            {
                Data = data,
                IsError = false,
                Status = (int)StatusCodeEnum.Ok,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = items.TotalCount
            });
        }

        [HttpGet("Clothing")]
        public async Task<IActionResult> GetAllClothingSizes()
        {
            var clothingSizes = await _sizeRepository.FindByCondition(t => t.Type == 0, null);
            var sizeEntites = clothingSizes.Adapt<List<SizeDto>>();
            return Ok(new Response<List<SizeDto>>
            {
                Data = sizeEntites,
                IsError = false,
                Status = (int)StatusCodeEnum.Ok
            });
        }

        [HttpGet("Shoes")]
        public async Task<IActionResult> GetAllShoesSizes()
        {
            var shoesSizes = await _sizeRepository.FindByCondition(t => (int)t.Type == 1, null);
            var sizeEntites = shoesSizes.Adapt<List<SizeDto>>();
            return Ok(new Response<List<SizeDto>>
            {
                Data = sizeEntites,
                IsError = false,
                Status = (int)StatusCodeEnum.Ok
            });
        }

        [HttpGet("Accessorize")]
        public async Task<IActionResult> GetAllAccessorizeSizes()
        {
            var shoesSizes = await _sizeRepository.FindByCondition(t => (int)t.Type == 2, null);
            var sizeEntites = shoesSizes.Adapt<List<SizeDto>>();
            return Ok(new Response<List<SizeDto>>
            {
                Data = sizeEntites,
                IsError = false,
                Status = (int)StatusCodeEnum.Ok
            });
        }

        [HttpGet("{Id}")]
        public async Task<IActionResult> GetSizeById(Guid Id)
        {
            var size = await _sizeRepository.Find(c => c.Id == Id, false);
            if (size == null)
                return Ok(new Response<string>
                {
                    Data = null,
                    IsError = true,
                    Status = (int)StatusCodeEnum.Ok,
                    MessageAr = "لم يتم العثور علي هذا المقاس",
                    Message = $"Size  doesn't exist"
                });

            var sizeEntity = size.Adapt<SizeDto>();
            return Ok(new Response<SizeDto>
            {
                Data = sizeEntity,
                IsError = false,
                Status = (int)StatusCodeEnum.Ok
            });
        }

        [HttpPost("CreateSize")]
        public async Task<IActionResult> CreateSize([FromBody] SizeForCreationDto model, CancellationToken cancellationToken)
        {
            var size = model.Adapt<Size>();
            size.CreatedBy = "admin";
            var result = await _sizeRepository.Create(size);
            await _sizeRepository.SaveAsync(cancellationToken);
            return Ok(new Response<Guid>()
            {
                Data = size.Id,
                IsError = false,
                Message = "Created successfully",
                MessageAr = "تم الاضافه بنجاح",
                Status = (int)StatusCodeEnum.Ok
            });
        }

        [HttpPut("UpdateSize")]
        public async Task<IActionResult> UpdateSize([FromBody] SizeDto model, CancellationToken cancellationToken)
        {
            var size = await _sizeRepository.Find(s => s.Id == model.Id, false);
            if (size == null)
                return Ok(new { message = $"Size with Id: {model.Id} doesn't exist in the database" });
            size = model.Adapt<Size>();
            size.UpdatedBy = "admin";
            size.CreatedBy = "admin";
            _sizeRepository.Update(size);
            await _sizeRepository.SaveAsync(cancellationToken);
            return Ok(new Response<Guid>()
            {
                Data = size.Id,
                IsError = false,
                Message = "Updated successfully",
                MessageAr = "تم التعديل بنجاح",
                Status = (int)StatusCodeEnum.Ok
            });
        }

        [HttpDelete("{Id}")]
        public async Task<IActionResult> DeleteSize(Guid Id, CancellationToken cancellationToken)
        {
            try
            {
                string[] includes = new string[]
                       {
                           "productSizes",
                       };
                var size = await _sizeRepository.Find(c => c.Id == Id, false, includes);

                if (size == null)
                {
                    return Ok(new { message = $"Size with Id: {Id} doesn't exist in the database" });
                }

                var isSizeUsed = size.productSizes.Any();

                if (isSizeUsed)
                {
                    return Ok(new Response<Guid>
                    {
                        Data = Id,
                        IsError = true,
                        Status = (int)StatusCodeEnum.BadRequest,
                        Message = "The size cannot be deleted because it is associated with products.",
                        MessageAr = "لا يمكن حذف الحجم لأنه مرتبط بمنتجات.",
                    });
                }

                size.IsDeleted = true;
                _sizeRepository.Update(size);

                await _sizeRepository.SaveAsync(cancellationToken);

                return Ok(new Response<Guid>
                {
                    Data = Id,
                    IsError = false,
                    Status = (int)StatusCodeEnum.Ok,
                    Message = "Deleted Successfully",
                    MessageAr = "تم الحذف بنجاح",
                });
            }
            catch (Exception ex)
            {
                return StatusCode((int)StatusCodeEnum.ServerError, new Response<string>
                {
                    Data = null,
                    IsError = true,
                    Status = (int)StatusCodeEnum.ServerError,
                    Message = "An error occurred while deleting the size",
                    MessageAr = "حدث خطأ أثناء حذف الحجم",
                });
            }
        }

    }
}