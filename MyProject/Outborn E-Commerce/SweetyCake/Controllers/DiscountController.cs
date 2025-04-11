using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OutbornE_commerce.BAL.Dto.discount;
using OutbornE_commerce.BAL.Repositories.DiscountRepository;
using OutbornE_commerce.FilesManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OutbornE_commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiscountController : ControllerBase
    {
        private readonly IDiscountRepository _discountRepository;
       

        public DiscountController(IDiscountRepository discountRepository)
        {
            _discountRepository = discountRepository;
            
        }




        [HttpGet("GetAllDiscounts")]
        public async Task<IActionResult> GetAllDiscounts(int pageNumber = 1, int pageSize = 10)
        {
            var discounts = await _discountRepository.FindAllAsyncByPagination(null, pageNumber, pageSize);

            var data = discounts.Data.Adapt<List<GetAllDiscount>>();

            if (!data.Any())
            {
                return NotFound(new PaginationResponse<List<GetAllDiscount>>
                {
                    Data = null,
                    IsError = true,
                    Status = (int)StatusCodeEnum.NotFound,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalCount = 0
                });
            }

            return Ok(new PaginationResponse<List<GetAllDiscount>>
            {
                Data = data,
                IsError = false,
                Status = (int)StatusCodeEnum.Ok,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = discounts.TotalCount
            });
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetDiscountById(Guid id)
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

            var item = await _discountRepository.Find(d => d.Id == id);
            if (item == null)
                return NotFound(new Response<string>
                {
                    Data = null,
                    IsError = true,
                    Message = "Discount not found.",
                    MessageAr = "لم يتم العثور على الخصم.",
                    Status = (int)StatusCodeEnum.NotFound
                });

            var itemEntity = item.Adapt<GetAllDiscount>();
            return Ok(new Response<GetAllDiscount>
            {
                Data = itemEntity,
                IsError = false,
                Message = "Fetched successfully",
                Status = (int)StatusCodeEnum.Ok
            });
        }


        [HttpPost]
        public async Task<IActionResult> CreateDiscount([FromForm] CreateDiscountDto model, CancellationToken cancellationToken)
        {
            try
            {
                var validationErrors = model.Validate();
                if (validationErrors.Any())
                {
                    return BadRequest(new
                    {
                        Message = "Validation failed.",
                        Errors = validationErrors
                    });
                }

                var discount = model.Adapt<Discount>();
                discount.CreatedBy = "admin";

                var result = await _discountRepository.Create(discount);
                await _discountRepository.SaveAsync(cancellationToken);

                return Ok(new Response<Guid>
                {
                    Data = result.Id,
                    IsError = false,
                    Message = "Discount created successfully.",
                    MessageAr = "تم إنشاء الخصم بنجاح.",
                    Status = (int)StatusCodeEnum.Ok
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred.", Error = ex.Message });
            }
        }



        [HttpPut]
        public async Task<IActionResult> UpdateDiscount([FromForm] UpdateDiscount model, CancellationToken cancellationToken)
        {
            if (model == null)
                return BadRequest(new { Message = "Invalid model." });

            var discount = await _discountRepository.Find(d => d.Id == model.Id, false);
            if (discount == null)
                return NotFound(new { Message = "Discount not found." });

            discount = model.Adapt(discount);
            discount.UpdatedBy = "admin";
            discount.UpdatedOn = DateTime.Now;


            _discountRepository.Update(discount);
            await _discountRepository.SaveAsync(cancellationToken);

            return Ok(new Response<Guid>
            {
                Data = discount.Id,
                IsError = false,
                Message = "Discount updated successfully.",
                MessageAr= "تم تحديث الخصم بنجاح",
                Status = (int)StatusCodeEnum.Ok
            });
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDiscount(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                if (id == Guid.Empty)
                    return BadRequest(new { Message = "Invalid ID." });

                var discount = await _discountRepository.Find(d => d.Id == id, true);
                if (discount == null)
                    return NotFound(new { Message = "Discount not found." });

                _discountRepository.Delete(discount);
                await _discountRepository.SaveAsync(cancellationToken);

                return Ok(new Response<Guid>
                {
                    Data = discount.Id,
                    IsError = false,
                    Message = "Discount deleted successfully.",
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
                    Message = "An error occurred while deleting the discount.",
                    Status = (int)StatusCodeEnum.ServerError
                });
            }
        }
        //[HttpGet("SearchByDate")]
        //public async Task<IActionResult> SearchDiscountsByDate(DateTime? startDate, DateTime? endDate, int pageNumber = 1, int pageSize = 10)
        //{

        //    var query = _discountRepository.GetDiscountsByDateRange(startDate, endDate);


        //    var paginatedResults = await query.Skip((pageNumber - 1) * pageSize)
        //                                      .Take(pageSize)
        //                                      .ToListAsync();


        //    if (!paginatedResults.Any())
        //        return NotFound(new { Message = "No discounts found within the specified date range." });


        //    return Ok(new
        //    {
        //        Data = paginatedResults, 
        //        PageNumber = pageNumber,
        //        PageSize = pageSize,
        //        TotalCount = await query.CountAsync()
        //    });
        //}
    }
}

