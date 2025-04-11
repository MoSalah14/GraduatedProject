using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OutbornE_commerce.BAL.Dto.discount;
using OutbornE_commerce.BAL.Dto.DiscountCagegoryDto;
using OutbornE_commerce.BAL.Dto.ProductColors;
using OutbornE_commerce.BAL.Repositories.DiscountCategoryRepo;
using OutbornE_commerce.BAL.Repositories.DiscountRepository;
using OutbornE_commerce.BAL.Repositories.ProductSizes;
using System.Collections.Immutable;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace OutbornE_commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiscountCategoryController : ControllerBase
    {
        private readonly IDiscountCategoryRepository _discountCategoryRepository;
        private readonly IProductSizeRepository _productSize;


        public DiscountCategoryController(IDiscountCategoryRepository discountCategoryRepository, IProductSizeRepository productSize)
        {
            _discountCategoryRepository = discountCategoryRepository;
            _productSize = productSize;
        }




        [HttpGet("GetAllDiscountCategory")]
        public async Task<IActionResult> GetAllDiscountCategory(int pageNumber = 1, int pageSize = 10)
        {
            string[] includes = new string[] { "Category", "Discount" };

            var paginatedResult = await _discountCategoryRepository.FindAllAsyncByPagination(null,  pageNumber, pageSize, includes);

            if (paginatedResult.Data == null || !paginatedResult.Data.Any())
            {
                return NotFound(new PaginationResponse<List<GetAllDiscountCategory>>
                {
                    Data = null,
                    IsError = true,
                    Status = (int)StatusCodeEnum.NotFound,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalCount = 0
                });
            }

            var allDiscountCategories = paginatedResult.Data.Select(discountCategory => new GetAllDiscountCategory
            {
                StartDate = discountCategory.StartDate,
                EndDate = discountCategory.EndDate,
                DiscountPercentage = discountCategory.Discount.Percentage,
                Id = discountCategory.Id,
                IsActive = discountCategory.IsActive,
                CategoryNameArabic = discountCategory.Category.NameAr,
                CategoryNameEndlish = discountCategory.Category.NameEn,
                DiscountId = discountCategory.DiscountId,
                CategoryId = discountCategory.CategoryId
            }).ToList();

            return Ok(new PaginationResponse<List<GetAllDiscountCategory>>
            {
                Data = allDiscountCategories,
                IsError = false,
                Status = (int)StatusCodeEnum.Ok,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = paginatedResult.TotalCount
            });
        }


        [HttpGet("GetCategoryDiscountById/{id}")]
        public async Task<IActionResult> GetCategoryDiscountById(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest(new Response<string>
                {
                    Data = null,
                    IsError = true,
                    Message = "Invalid ID.",
                    MessageAr = "معرف غير صالح.",
                    Status = (int)StatusCodeEnum.BadRequest
                });

            string[] includes = new string[] { "Category", "Discount" };
            var item = await _discountCategoryRepository.Find(d => d.Id == id, false, includes);

            if (item == null)
                return NotFound(new Response<string>
                {
                    Data = null,
                    IsError = true,
                    Message = "Category discount not found.",
                    MessageAr = "لم يتم العثور على خصم الفئة.",
                    Status = (int)StatusCodeEnum.NotFound
                });

            var discountCategoryDetails = new GetAllDiscountCategory
            {
                StartDate = item.StartDate,
                EndDate = item.EndDate,
                DiscountPercentage = item.Discount.Percentage,
                Id = item.Id,
                IsActive = item.IsActive,
                CategoryNameArabic = item.Category.NameAr,
                CategoryNameEndlish = item.Category.NameEn,
                DiscountId = item.Discount.Id,
                CategoryId = item.Category.Id
            };

            return Ok(new Response<GetAllDiscountCategory>
            {
                Data = discountCategoryDetails,
                IsError = false,
                Message = "Fetched successfully.",
                MessageAr = "تم الجلب بنجاح.",
                Status = (int)StatusCodeEnum.Ok
            });
        }



        [HttpPost("CreateDiscountCategory")]
        public async Task<IActionResult> CreateDiscountCategory([FromForm] CreateDiscountCategory model, CancellationToken cancellationToken)
        {
            try
            {
                if (model == null)
                    return BadRequest(new { Message = "Invalid model." });

                var discountCategory = model.Adapt<DiscountCategory>();
                discountCategory.CreatedBy = "admin";

                var result = await _discountCategoryRepository.Create(discountCategory);
                await _discountCategoryRepository.SaveAsync(cancellationToken);

                bool isDiscountApplied = await _productSize.ApplyDiscountToCategory(result.Id);

                if (!isDiscountApplied)
                {
                     _discountCategoryRepository.Delete(result);
                    return BadRequest(new Response<string>
                    {
                        Data = null,
                        IsError = true,
                        Message = "Failed to apply discount.",
                        MessageAr = "فشل في تطبيق الخصم.",
                        Status = (int)StatusCodeEnum.BadRequest
                    });
                }


                return Ok(new Response<object>
                {
                    Data = result.Id,
                    IsError = false,
                    Message = "Discount applied and category created successfully.",
                    MessageAr = "تم إنشاء فئة الخصم وتطبيق الخصم بنجاح.",
                    Status = (int)StatusCodeEnum.Ok
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Response<string>
                {
                    Data = ex.Message,
                    IsError = true,
                    Message = "An error occurred while creating the DiscountCategory.",
                    MessageAr = "حدث خطأ أثناء إنشاء فئة الخصم.",
                    Status = (int)StatusCodeEnum.ServerError
                });
            }
        }


        [HttpPut("UpdateDiscountCategory")]
        public async Task<IActionResult> UpdateDiscountCategory([FromForm] CreateDiscountCategory model, CancellationToken cancellationToken)
        {
            try
            {
                if (model == null)
                    return BadRequest(new { Message = "Invalid model." });

                var discount = await _discountCategoryRepository.Find(d => d.Id == model.Id, false);
                if (discount == null)
                    return NotFound(new { Message = "DiscountCategory not found." });

                bool isChanged = discount.DiscountId != model.DiscountId || discount.CategoryId != model.CategoryId;

                discount = model.Adapt(discount);
                discount.UpdatedBy = "admin";
                discount.UpdatedOn = DateTime.Now;

                if (isChanged)
                {
                    if (discount.IsActive)
                    {
                        bool isDiscountApplied = await _productSize.ApplyDiscountToCategory(discount.Id);

                        if (!isDiscountApplied)
                        {
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
                    else
                    {
                        return BadRequest(new Response<string>
                        {
                            Data = null,
                            IsError = true,
                            Message = "Discount is not active.",
                            MessageAr = "الخصم غير مفعل.",
                            Status = (int)StatusCodeEnum.BadRequest
                        });
                    }
                }

                _discountCategoryRepository.Update(discount);
                await _discountCategoryRepository.SaveAsync(cancellationToken);

                return Ok(new Response<Guid>
                {
                    Data = discount.Id,
                    IsError = false,
                    Message = "DiscountCategory updated successfully.",
                    MessageAr = "تم تحديث فئة الخصم بنجاح.",
                    Status = (int)StatusCodeEnum.Ok
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Response<string>
                {
                    Data = ex.Message,
                    IsError = true,
                    Message = "An error occurred while updating the DiscountCategory.",
                    MessageAr = "حدث خطأ أثناء تحديث فئة الخصم.",
                    Status = (int)StatusCodeEnum.ServerError
                });
            }
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDiscountCategory(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                if (id == Guid.Empty)
                    return BadRequest(new { Message = "Invalid ID." });

                var discount = await _discountCategoryRepository.Find(d => d.Id == id, true);
                if (discount == null)
                    return NotFound(new Response<Discount>
                    {
                        Data = null,
                        IsError = true,
                        Message = "No discount to Show.",
                        MessageAr="لا يوجد خصم للعرض",
                        Status = (int)StatusCodeEnum.Ok
                    });

                _discountCategoryRepository.Delete(discount);
                await _discountCategoryRepository.SaveAsync(cancellationToken);

                return Ok(new Response<Guid>
                {
                    Data = discount.Id,
                    IsError = false,
                    Message = "DiscountCategory deleted successfully.",
                    Status = (int)StatusCodeEnum.Ok
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Response<string>
                {
                    Data = ex.Message,
                    IsError = true,
                    Message = "An error occurred while deleting the DiscountCategory.",
                    Status = (int)StatusCodeEnum.ServerError
                });
            }
        }
        [HttpGet("SearchByDate")]
        public async Task<IActionResult> SearchDiscountsByDate(DateTime? startDate, DateTime? endDate, int pageNumber = 1, int pageSize = 10)
        {
            var result = await _discountCategoryRepository.SearchDiscountsByDate(startDate, endDate, pageNumber, pageSize);

            if (result.Data == null || !result.Data.Any())
            {
                return NotFound(new Response<string>
                {
                    Data = null,
                    IsError = true,
                    Message = "No data found.",
                    MessageAr = "لم يتم العثور على بيانات.",
                    Status = (int)StatusCodeEnum.NotFound
                });
            }

            return Ok(new Response<PaginationResponse<List<GetAllDiscountCategory>>>
            {
                Data = result,
                IsError = false,
                Message = "Fetched successfully.",
                MessageAr = "تم الجلب بنجاح.",
                Status = (int)StatusCodeEnum.Ok
            });
        }

    }
}

