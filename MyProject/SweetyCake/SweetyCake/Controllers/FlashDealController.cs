using Mapster;
using Microsoft.AspNetCore.Mvc;
using OutbornE_commerce.BAL.Dto.FlashDealDTO;
using OutbornE_commerce.BAL.Repositories.FlashDealRepo;
using OutbornE_commerce.FilesManager;

namespace OutbornE_commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlashDealController : ControllerBase
    {
        private readonly IFlashDealRepo _flashSaleRepository;
        private readonly IFilesManager _filesManager;

        public FlashDealController(IFilesManager filesManager, IFlashDealRepo flashSaleRepository)
        {
            _filesManager = filesManager;
            _flashSaleRepository = flashSaleRepository;
        }

        [HttpGet("GetAllFlashSales")]
        public async Task<IActionResult> GetAllFlashSales(int pageNumber = 1, int pageSize = 10, string? searchTerm = null)
        {
            var items = new PagainationModel<IEnumerable<FlashDeal>>();

            if (string.IsNullOrEmpty(searchTerm))
            {
                items = await _flashSaleRepository.FindAllAsyncByPagination(null, pageNumber, pageSize);
            }
            else
            {
                items = await _flashSaleRepository
                                     .FindAllAsyncByPagination(fs => (fs.TitleEn.Contains(searchTerm)
                                                                   || fs.TitleAr.Contains(searchTerm)
                                                                   || fs.DescriptionEn.Contains(searchTerm)
                                                                   || fs.DescriptionAr.Contains(searchTerm)),
                                                               pageNumber, pageSize);
            }

            var data = items.Data.Adapt<List<GetAllFlashDeal>>();

            return Ok(new PaginationResponse<List<GetAllFlashDeal>>
            {
                Data = data,
                IsError = false,
                Status = (int)StatusCodeEnum.Ok,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = items.TotalCount
            });
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetFlashDealById(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest(new { Message = "Invalid ID." });

            var item = await _flashSaleRepository.Find(fd => fd.Id == id);
            if (item == null)
                return NotFound(new { Message = "Flash Deal not found." });

            var itemEntity = item.Adapt<GetAllFlashDeal>();
            return Ok(new Response<GetAllFlashDeal>
            {
                Data = itemEntity,
                IsError = false,
                Message = "",
                Status = (int)StatusCodeEnum.Ok
            });
        }


        [HttpGet("random-active")]
        public async Task<IActionResult> GetRandomActiveFlashDeal()
        {
            try
            {
                var flashDeal = await _flashSaleRepository.GetRandomActiveFlashDealAsync();

                if (flashDeal == null)
                {
                    return NotFound(new Response<string>
                    {
                        Data = null,
                        IsError = true,
                        Message = "No active flash deals found.",
                        Status = (int)StatusCodeEnum.NotFound
                    });
                }

                var flashDealDto = flashDeal.Adapt<GetAllFlashDeal>();

                return Ok(new Response<GetAllFlashDeal>
                {
                    Data = flashDealDto,
                    IsError = false,
                    Message = "Random active flash deal retrieved successfully.",
                    Status = (int)StatusCodeEnum.Ok
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Response<string>
                {
                    Data = ex.Message,
                    IsError = true,
                    Message = "An error occurred while retrieving a random active flash deal.",
                    Status = (int)StatusCodeEnum.ServerError
                });
            }
        }



        [HttpPost("CreateFlashSale")]
        public async Task<IActionResult> CreateFlashSale([FromForm] CreateFlashDealDto model, CancellationToken cancellationToken)
        {
            var flashSale = model.Adapt<FlashDeal>();
            flashSale.CreatedBy = "admin";
            flashSale.CreatedOn = DateTime.UtcNow;

            if (model.Image != null)
            {
                var fileModel = await _filesManager.UploadFile(model.Image, "FlashSales");
                flashSale.ImageUrl = fileModel!.Url;
            }

            var result = await _flashSaleRepository.Create(flashSale);
            await _flashSaleRepository.SaveAsync(cancellationToken);

            return Ok(new Response<Guid>
            {
                Data = result.Id,
                IsError = false,
                Message = "Flash Sale created successfully.",
                MessageAr= "تم إنشاء البيع السريع بنجاح",
                Status = (int)StatusCodeEnum.Ok
            });
        }

        [HttpPut("UpdateFlashSale")]
        public async Task<IActionResult> UpdateFlashSale([FromForm] UpdateFlashDeal model, CancellationToken cancellationToken)
        {
            var flashSale = await _flashSaleRepository.Find(f => f.Id == model.Id, false);
            if (flashSale == null)
            {
                return NotFound(new Response<string>
                {
                    Data = null,
                    IsError = true,
                    Message = "Flash Sale not found.",
                    MessageAr = "حدث خطأ لا يوجد اعلان",
                    Status = (int)StatusCodeEnum.NotFound
                });
            }

            var oldImageUrl = flashSale.ImageUrl;
            flashSale = model.Adapt(flashSale);
            flashSale.UpdatedBy = "admin";
            flashSale.UpdatedOn = DateTime.Now;
            
            

            if (model.Image != null)
            {
                var fileModel = await _filesManager.UploadFile(model.Image, "FlashSales", oldImageUrl);
                flashSale.ImageUrl = fileModel!.Url;
            }

            _flashSaleRepository.Update(flashSale);
            await _flashSaleRepository.SaveAsync(cancellationToken);

            return Ok(new Response<Guid>
            {
                Data = flashSale.Id,
                IsError = false,
                Message = "Flash Sale updated successfully.",
                MessageAr="تم التعديل بنجاح",
                Status = (int)StatusCodeEnum.Ok
            });
        }

        [HttpDelete("DeleteFlashSale/{id}")]
        public async Task<IActionResult> DeleteFlashSale(Guid id, CancellationToken cancellationToken)
        {
            var flashSale = await _flashSaleRepository.Find(f => f.Id == id, false);

            if (flashSale == null)
            {
                return NotFound(new Response<string>
                {
                    Data = null,
                    IsError = true,
                    Message = "Flash Sale not found.",
                    MessageAr = "تم الحذف بنجاح",
                    Status = (int)StatusCodeEnum.NotFound
                });
            }

            if (!string.IsNullOrEmpty(flashSale.ImageUrl))
            {
                bool fileDeleted = _filesManager.DeleteFile(flashSale.ImageUrl);
                if (!fileDeleted)
                {
                    return BadRequest(new Response<string>
                    {
                        Data = null,
                        IsError = true,
                        Message = "Failed to delete associated image.",
                        MessageAr = "حدث خطاء اثناء الحذف",
                        Status = (int)StatusCodeEnum.BadRequest
                    });
                }
            }

            _flashSaleRepository.Delete(flashSale);
            await _flashSaleRepository.SaveAsync(cancellationToken);

            return Ok(new Response<Guid>
            {
                Data = id,
                IsError = false,
                Message = "Flash Sale and associated image deleted successfully.",
                MessageAr = "تم الحذف بنجاح",
                Status = (int)StatusCodeEnum.Ok
            });
        }
    }
}
