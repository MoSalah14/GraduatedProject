using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OutbornE_commerce.BAL.Dto.Copun;
using OutbornE_commerce.BAL.Repositories.BaseRepositories;
using OutbornE_commerce.BAL.Services.CouponsService;
using OutbornE_commerce.DAL.Models;

namespace OutbornE_commerce.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CouponController : ControllerBase
    {
        private readonly CouponService _couponService;
        private readonly IBaseRepository<Coupons> baseRepository;

        public CouponController(CouponService couponService, IBaseRepository<Coupons> baseRepository)
        {
            _couponService = couponService;
            this.baseRepository = baseRepository;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllCoupon(int pageNumber = 1, int pageSize = 10, string? searchTerm = null)
        {
            var items = new PagainationModel<IEnumerable<Coupons>>();

            if (string.IsNullOrEmpty(searchTerm))
                items = await baseRepository.FindAllAsyncByPagination(null, pageNumber, pageSize);
            else
                items = await baseRepository
                                    .FindAllAsyncByPagination(b => (b.Code.Contains(searchTerm)
                                                               || b.DiscountType.ToString().Contains(searchTerm)
                                                               || b.Status.ToString().Contains(searchTerm)
                                                               || b.StartDate.ToString().Contains(searchTerm))
                                                               , pageNumber, pageSize);

            return Ok(items);
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddCoupon([FromBody] CreateCouponDto couponDto, CancellationToken cancellationToken)
        {
            try
            {
                var createdCoupon = await _couponService.AddCouponAsync(couponDto, cancellationToken);
                return Ok(new Response<string>
                {
                    Data = createdCoupon.Code,
                    IsError = false,
                    Message = "Addedd Successfully",
                    MessageAr = "تم الاضافة بنجاح",
                    Status = (int)StatusCodeEnum.Ok,
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // Read
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCoupon(Guid id, CancellationToken cancellationToken)
        {
            var coupon = await baseRepository.Find(e => e.Id == id);
            if (coupon == null)
                return NotFound(new Response<Guid>
                {
                    Data = id,
                    IsError = true,
                    Message = "Coupon not found",
                    MessageAr = "لم يتم العثور علي الكوبون",
                    Status = (int)StatusCodeEnum.NotFound,
                });

            var x = coupon.Adapt<GetCouponDto>();
            return Ok(new Response<GetCouponDto>
            {
                Data = x,
                IsError = false,
                Message = "Fetched Successfully",
                MessageAr = "تم بنجاح",
                Status = (int)StatusCodeEnum.Ok,
            });
        }

        // Update
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateCoupon(Guid id, [FromBody] CreateCouponDto couponDto, CancellationToken cancellationToken)
        {
            try
            {
                var GetCoupon = await baseRepository.Find(e => e.Id == id);
                if (GetCoupon == null)
                    return NotFound(new Response<Guid>
                    {
                        Data = id,
                        IsError = true,
                        Message = "Coupon not found",
                        MessageAr = "لم يتم العثور علي الكوبون",
                        Status = (int)StatusCodeEnum.NotFound,
                    });

                // will apply properties of `couponDto` to `existingCoupon
                couponDto.Adapt(GetCoupon);
                GetCoupon.UpdatedBy = "Admin";
                GetCoupon.UpdatedOn = DateTime.Now;

                baseRepository.Update(GetCoupon);
                await baseRepository.SaveAsync(cancellationToken);
                return Ok(new Response<string>
                {
                    Data = GetCoupon.Code,
                    IsError = false,
                    Message = "Updated Successfully",
                    MessageAr = "تم التحديث بنجاح",
                    Status = (int)StatusCodeEnum.Ok,
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // Delete
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteCoupon(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                var GetCoupon = await baseRepository.Find(e => e.Id == id);
                if (GetCoupon is null)
                {
                    return Ok(new Response<CityDto>
                    {
                        Data = null,
                        IsError = true,
                        Message = "Not Found The Coupon",
                        MessageAr = "لم يتم العثور علي الكوبون",
                        Status = (int)StatusCodeEnum.NotFound
                    });
                }

                GetCoupon.IsDeleted = true;
                baseRepository.Update(GetCoupon);
                await baseRepository.SaveAsync(cancellationToken);
                return Ok(new Response<string>
                {
                    Data = id.ToString(),
                    IsError = false,
                    Message = "Deleted Successfully",
                    MessageAr = "تم الحذف بنجاح",
                    Status = (int)StatusCodeEnum.Ok,
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}