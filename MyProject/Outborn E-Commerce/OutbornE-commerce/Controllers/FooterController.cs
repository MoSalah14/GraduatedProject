using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OutbornE_commerce.BAL.Dto.FooterDto;
using OutbornE_commerce.BAL.Dto.Sizes;
using OutbornE_commerce.BAL.Repositories.FooterRepo;

namespace OutbornE_commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FooterController : ControllerBase
    {
        private readonly IFooterRepository footerRepository;

        public FooterController(IFooterRepository footerRepository)
        {
            this.footerRepository = footerRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTotalFooter(int pageNumber = 1, int pageSize = 10, string? searchTerm = null)
        {
            var footers = new PagainationModel<IEnumerable<Footer>>();

            if (string.IsNullOrEmpty(searchTerm))
            {
                footers = await footerRepository.FindAllAsyncByPagination(null, pageNumber, pageSize);
            }
            else
            {
                footers = await footerRepository.FindAllAsyncByPagination(
                    f => f.TitleArabic.Contains(searchTerm) ||
                         f.TitleEnglish.Contains(searchTerm),
                    pageNumber,
                    pageSize);
            }

            var footerEntities = footers.Data.Adapt<List<BaseFooterDto>>();

            return Ok(new PaginationResponse<List<BaseFooterDto>>
            {
                Data = footerEntities,
                IsError = false,
                Status = (int)StatusCodeEnum.Ok,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = footers.TotalCount
            });
        }

        [HttpGet("AllActiveFooter")]
        public async Task<IActionResult> AllActiveFooter()
        {
            var footer = await footerRepository.FindByCondition(x => x.IsActive, null);

            if (footer == null || !footer.Any())
            {
                return BadRequest(new Response<object>
                {
                    Data = null,
                    Message = "No Footers found",
                    IsError = true,
                    Status = (int)StatusCodeEnum.NotFound
                });
            }

            var footerEntities = footer.Adapt<List<BaseFooterDto>>();

            return Ok(new Response<object>
            {
                Data = new
                {
                    CustomerCare = footerEntities.Where(e => e.ParentTitleEn == ParentTitleEnglish.CUSTOMERCARE.ToString()).ToList(),
                    Currency = footerEntities.Where(e => e.ParentTitleEn == ParentTitleEnglish.CURRENCY.ToString()).ToList(),
                    MyAccount = footerEntities.Where(e => e.ParentTitleEn == ParentTitleEnglish.MyAccount.ToString()).ToList(),
                    Information = footerEntities.Where(e => e.ParentTitleEn == ParentTitleEnglish.INFORMATION.ToString()).ToList(),
                },
                Message = "Footers retrieved successfully",
                IsError = false,
                Status = (int)StatusCodeEnum.Ok
            });
        }

        [HttpGet("GetfooterById/{Id}")]
        public async Task<IActionResult> GetfooterById(Guid Id)
        {
            var footer = await footerRepository.Find(c => c.Id == Id, false);
            if (footer == null)
                return Ok(new { message = $"Footer with Id: {footer!.Id} doesn't exist in the database" });
            var footerEntity = footer.Adapt<BaseFooterDto>();
            return Ok(new Response<BaseFooterDto>
            {
                Data = footerEntity,
                IsError = false,
                Status = (int)StatusCodeEnum.Ok
            });
        }

        [HttpPost("CreateFooter")]
        public async Task<IActionResult> CreateFooter([FromBody] CreateFooterDto model, CancellationToken cancellationToken)
        {
            var footer = model.Adapt<Footer>();
            footer.CreatedBy = "admin";
            var result = await footerRepository.Create(footer);
            await footerRepository.SaveAsync(cancellationToken);
            return Ok(new Response<Guid>()
            {
                Data = footer.Id,
                IsError = false,
                Message = "Created successfully",
                MessageAr = "تم الاضافه بنجاح",
                Status = (int)StatusCodeEnum.Ok
            });
        }

        [HttpPut("UpdateFooter")]
        public async Task<IActionResult> UpdateFooter([FromBody] CreateFooterDto model, CancellationToken cancellationToken)
        {
            var footer = await footerRepository.Find(s => s.Id == model.Id, false);
            if (footer == null)
                return Ok(new { message = $"footer with Id: {model.Id} doesn't exist in the database" });
            footer = model.Adapt<Footer>();
            footer.UpdatedBy = "admin";
            footer.UpdatedOn = DateTime.Now;
            footerRepository.Update(footer);
            await footerRepository.SaveAsync(cancellationToken);
            return Ok(new Response<Guid>()
            {
                Data = footer.Id,
                IsError = false,
                Message = "Updated successfully",
                MessageAr = "تم التعديل بنجاح",
                Status = (int)StatusCodeEnum.Ok
            });
        }

        [HttpPut("UpdateStatusFooter/{Id}")]
        public async Task<IActionResult> UpdateStatusFooter(Guid Id, CancellationToken cancellationToken)
        {
            var footer = await footerRepository.Find(s => s.Id == Id, false);
            if (footer == null)
                return Ok(new { message = $"footer with Id: {Id} doesn't exist in the database" });
            footer.IsActive = !footer.IsActive;
            footer.UpdatedBy = "admin";
            footer.UpdatedOn = DateTime.Now;
            footerRepository.Update(footer);
            await footerRepository.SaveAsync(cancellationToken);
            return Ok(new Response<Guid>()
            {
                Data = footer.Id,
                IsError = false,
                Message = "Updated successfully",
                MessageAr = "تم التعديل بنجاح",
                Status = (int)StatusCodeEnum.Ok
            });
        }

        [HttpDelete("{Id}")]
        public async Task<IActionResult> DeleteSize(Guid Id, CancellationToken cancellationToken)
        {
            var footer = await footerRepository.Find(c => c.Id == Id, false);
            if (footer == null)
                return Ok(new { message = $"footer with Id: {Id} doesn't exist in the database" });
            footerRepository.Delete(footer);
            await footerRepository.SaveAsync(cancellationToken);
            return Ok(new Response<Guid>()
            {
                Data = Id,
                IsError = false,
                Status = (int)StatusCodeEnum.Ok,
                Message = "Deleted Successfully",
                MessageAr = "تم الحذف بنجاح",
            });
        }
    }
}