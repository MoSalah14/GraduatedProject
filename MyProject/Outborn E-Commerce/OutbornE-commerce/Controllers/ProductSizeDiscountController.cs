using Microsoft.AspNetCore.Mvc;
using OutbornE_commerce.BAL.Dto.ProductSizeDiscountDto;
using OutbornE_commerce.BAL.Repositories.ProductSizeDiscountRepo;
using OutbornE_commerce.BAL.Repositories.ProductSizes;
using OutbornE_commerce.DAL.Models;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace OutbornE_commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductSizeDiscountController : ControllerBase
    {
        private readonly IproductSizeDiscountRepository _productSizeDiscountRepository;
        private readonly IProductSizeRepository _productSizeRepository;

        public ProductSizeDiscountController(IproductSizeDiscountRepository productSizeDiscountRepository, IProductSizeRepository productSizeRepository)
        {
            _productSizeDiscountRepository = productSizeDiscountRepository;
            _productSizeRepository = productSizeRepository;
        }
        [HttpGet("GetAllProductSizeDiscounts")]
        public async Task<IActionResult> GetAllProductSizeDiscounts(int pageNumber = 1, int pageSize = 10)
        {
            string[] includes = new string[] { "Discount", "ProductSize.ProductColor.Product" };

            var paginatedResult = await _productSizeDiscountRepository
                .FindAllAsyncByPagination(null, pageNumber, pageSize, includes);

            if (paginatedResult.Data == null || !paginatedResult.Data.Any())
            {
                return NotFound(new PaginationResponse<List<GetProductSizeDiscountDto>>
                {
                    Data = null,
                    IsError = true,                   
                    Status = (int)StatusCodeEnum.NotFound,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalCount = 0
                });
            }

            // Map to DTO
            var productSizeDiscountDtos = paginatedResult.Data.Select(psd => new GetProductSizeDiscountDto
            {
                Id = psd.Id,
                StartDate = psd.StartDate,
                EndDate = psd.EndDate,
                IsActive = psd.IsActive,
                DiscountPercentage = psd.Discount?.Percentage ?? 0,
                DiscountId = psd.DiscountId,
                ProductNameArabic = psd.ProductSize.ProductColor.Product.NameAr,
                ProductNameEndlish = psd.ProductSize.ProductColor.Product.NameEn,
                ProductSizeId = psd.ProductSizeId
            }).ToList();

            // Return response with pagination metadata
            return Ok(new PaginationResponse<List<GetProductSizeDiscountDto>>
            {
                Data = productSizeDiscountDtos,
                IsError = false,
                Status = (int)StatusCodeEnum.Ok,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = paginatedResult.TotalCount
            });
        }


        [HttpGet("GetProductSizeDiscountById/{id}")]
        public async Task<IActionResult> GetProductSizeDiscountById(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest(new Response<string>
                {
                    Data = null,
                    IsError = true,
                    Message = "Invalid ID.",
                    MessageAr = "معرف غير صالح.",
                    Status = (int)StatusCodeEnum.BadRequest
                });
            }

            var productSizeDiscount = await _productSizeDiscountRepository.Find(
                psd => psd.Id == id,
                false,
                includes: new string[] { "Discount", "ProductSize.ProductColor.Product" });

            if (productSizeDiscount == null)
            {
                return NotFound(new Response<string>
                {
                    Data = null,
                    IsError = true,
                    Message = "Product Size Discount not found.",
                    MessageAr = "لم يتم العثور على خصم لحجم المنتج.",
                    Status = (int)StatusCodeEnum.NotFound
                });
            }

            var productSizeDiscountDto = new GetProductSizeDiscountDto
            {
                Id = productSizeDiscount.Id,
                StartDate = productSizeDiscount.StartDate,
                EndDate = productSizeDiscount.EndDate,
                IsActive = productSizeDiscount.IsActive,
                ProductNameArabic = productSizeDiscount.ProductSize.ProductColor.Product.NameAr,
                ProductNameEndlish = productSizeDiscount.ProductSize.ProductColor.Product.NameEn,
                DiscountPercentage = productSizeDiscount.Discount?.Percentage ?? 0,
                DiscountId = productSizeDiscount.DiscountId,
                ProductSizeId = productSizeDiscount.ProductSizeId
            };

            return Ok(new Response<GetProductSizeDiscountDto>
            {
                Data = productSizeDiscountDto,
                IsError = false,
                Message = "Product Size Discount fetched successfully.",
                MessageAr = "تم جلب خصم حجم المنتج بنجاح.",
                Status = (int)StatusCodeEnum.Ok
            });
        }

        [HttpPost("CreateProductSizeDiscount")]
        public async Task<IActionResult> CreateProductSizeDiscount(CreateProductSizeDiscountDto model, CancellationToken cancellationToken)
        {
            try
            {
                if (model == null)
                    return BadRequest(new Response<object> { Data = null, IsError = true, Message = "No Data to show", Status = (int)StatusCodeEnum.BadRequest });

                var productSizeDiscount = model.Adapt<ProductSizeDiscount>();
                productSizeDiscount.CreatedBy = "admin";
                var result = await _productSizeDiscountRepository.Create(productSizeDiscount);
                await _productSizeDiscountRepository.SaveAsync(cancellationToken);

                if (productSizeDiscount.IsActive)
                {
                    bool isDiscountApplied = await _productSizeRepository.ApplyDiscountToProductSize(result.Id);

                    if (!isDiscountApplied)
                    {
                        _productSizeDiscountRepository.Delete(productSizeDiscount);
                        return BadRequest(new Response<string>
                        {
                            Data = null,
                            IsError = true,
                            Message = "Failed to apply discount.",
                            MessageAr = "فشل في تطبيق الخصم.",
                            Status = (int)StatusCodeEnum.BadRequest
                        });
                    }
                }

               

                return Ok(new Response<Guid>
                {
                    Data = result.Id,
                    IsError = false,
                    Message = "Discount applied and Product Size Discount created successfully.",
                    MessageAr = "تم إنشاء الخصم على حجم المنتج وتطبيق الخصم بنجاح.",
                    Status = (int)StatusCodeEnum.Ok
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Response<string>
                {
                    Data = ex.Message,
                    IsError = true,
                    Message = "An error occurred while creating the Product Size Discount.",
                    MessageAr = "حدث خطأ أثناء إنشاء خصم حجم المنتج.",
                    Status = (int)StatusCodeEnum.ServerError
                });
            }
        }


        [HttpPut("UpdateProductSizeDiscount")]
        public async Task<IActionResult> UpdateProductSizeDiscount([FromBody] CreateProductSizeDiscountDto model, CancellationToken cancellationToken)
        {
            try
            {
                if (model == null)
                    return BadRequest(new Response<object>
                    {
                        Data = null,
                        IsError = true,
                        Message = "No Data to show",
                        Status = (int)StatusCodeEnum.BadRequest
                    });

                var productSizeDiscount = await _productSizeDiscountRepository.Find(psd => psd.Id == model.Id, false);
                if (productSizeDiscount == null)
                    return BadRequest(new Response<object>
                    {
                        Data = null,
                        IsError = true,
                        Message = "No Data to show",
                        Status = (int)StatusCodeEnum.BadRequest
                    });

                bool isDiscountChanged = productSizeDiscount.DiscountId != model.DiscountId || productSizeDiscount.ProductSizeId != model.ProductSizeId;

                var updatedDiscount = model.Adapt(productSizeDiscount);

                if (isDiscountChanged)
                {
                    bool isDiscountApplied = updatedDiscount.IsActive &&
                                             await _productSizeRepository.ApplyDiscountToProductSize(updatedDiscount.Id);

                    if (!isDiscountApplied)
                    {
                        return BadRequest(new Response<string>
                        {
                            Data = null,
                            IsError = true,
                            Message = updatedDiscount.IsActive ? "Failed to apply discount." : "Discount is not active.",
                            MessageAr = updatedDiscount.IsActive ? "فشل في تطبيق الخصم." : "الخصم غير مفعل.",
                            Status = (int)StatusCodeEnum.BadRequest
                        });
                    }
                }

                updatedDiscount.UpdatedBy = "admin";
                updatedDiscount.UpdatedOn = DateTime.Now;

                _productSizeDiscountRepository.Update(updatedDiscount);
                await _productSizeDiscountRepository.SaveAsync(cancellationToken);

                return Ok(new Response<Guid>
                {
                    Data = updatedDiscount.Id,
                    IsError = false,
                    Message = "Discount applied and updated successfully.",
                    MessageAr = "تم تعديل الخصم وتطبيقه بنجاح.",
                    Status = (int)StatusCodeEnum.Ok
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Response<string>
                {
                    Data = ex.Message,
                    IsError = true,
                    Message = "An error occurred while updating the Product Size Discount.",
                    MessageAr = "حدث خطأ أثناء تحديث خصم حجم المنتج.",
                    Status = (int)StatusCodeEnum.ServerError
                });
            }
        }


        [HttpDelete("DeleteProductSizeDiscount/{id}")]
        public async Task<IActionResult> DeleteProductSizeDiscount(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                if (id == Guid.Empty)
                    return BadRequest(new Response<object>
                    {
                        Data = null,
                        IsError = true,
                        Message = "No Data to show",
                        Status = (int)StatusCodeEnum.BadRequest
                    });

                var productSizeDiscount = await _productSizeDiscountRepository.Find(psd => psd.Id == id, false);
                if (productSizeDiscount == null)
                    return BadRequest(new Response<object>
                    {
                        Data = null,
                        IsError = true,
                        Message = "No Data to show",
                        Status = (int)StatusCodeEnum.BadRequest
                    });


                _productSizeDiscountRepository.Delete(productSizeDiscount);
                await _productSizeDiscountRepository.SaveAsync(cancellationToken);

                return Ok(new Response<Guid>
                {
                    Data = productSizeDiscount.Id,
                    IsError = false,
                    Message = "Product Size Discount deleted successfully.",
                    MessageAr = "تم الحذف بنجاح",
                    Status = (int)StatusCodeEnum.Ok
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Response<string>
                {
                    Data = ex.Message,
                    IsError = true,
                    Message = "An error occurred while deleting the Product Size Discount.",
                    Status = (int)StatusCodeEnum.ServerError
                });
            }
        }
        [HttpGet("SearchByDate")]
        public async Task<IActionResult> SearchDiscountsByDate(DateTime? startDate, DateTime? endDate, int pageNumber = 1, int pageSize = 10)
        {

            var query = await _productSizeDiscountRepository.GetDiscountsByDateRange(startDate, endDate, pageNumber, pageSize);





            if (query == null)
                return BadRequest(new Response<object>
                {
                    Data = query,
                    IsError = true,
                    Message = "No Data to show",
                    Status = (int)StatusCodeEnum.BadRequest
                });


            return Ok(new Response<object>
            {

                Data = query,
                IsError = false,
                Message = "Get Successfully",
                Status = (int)StatusCodeEnum.Ok
            });
        }
    }
}
