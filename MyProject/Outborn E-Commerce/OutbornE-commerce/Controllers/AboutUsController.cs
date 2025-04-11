using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OutbornE_commerce.BAL.Dto.AboutUs;
using OutbornE_commerce.BAL.Repositories.AboutUs;
using OutbornE_commerce.FilesManager;

namespace OutbornE_commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AboutUsController : ControllerBase
    {
        private readonly IAboutUsRepository _aboutUsRepository;
        private readonly IFilesManager _filesManager;

        public AboutUsController(IAboutUsRepository aboutUsRepository, IFilesManager filesManager)
        {
            _aboutUsRepository = aboutUsRepository;
            _filesManager = filesManager;
        }

        [HttpGet("AllWithPaginate")]
        public async Task<IActionResult> GetAllAboutUs(int pageNumber = 1, int pageSize = 10, string? searchTerm = null)
        {
            var items = string.IsNullOrEmpty(searchTerm)
                ? await _aboutUsRepository.FindAllAsyncByPagination(null, pageNumber, pageSize)
                : await _aboutUsRepository.FindAllAsyncByPagination(a =>
                    a.Description1Ar.Contains(searchTerm) ||
                    a.Description1En.Contains(searchTerm) ||
                    a.TitleAr.Contains(searchTerm) ||
                    a.TitleEn.Contains(searchTerm), pageNumber, pageSize);

            if (!items.Data.Any())
                return NotFound(new { Message = "No AboutUs records found." });

            var data = items.Data.Adapt<List<AboutUsDto>>();
            return Ok(new PaginationResponse<List<AboutUsDto>>
            {
                Data = data,
                IsError = false,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Status = (int)StatusCodeEnum.Ok,
                TotalCount = items.TotalCount
            });
        }

        [HttpGet("All")]
        public async Task<IActionResult> GetAllAboutsUs()
        {
            var items = await _aboutUsRepository.FindAllAsync(null);
            var itemEntities = items.Adapt<List<AboutUsDto>>();

            if (!itemEntities.Any())
                return NotFound(new { Message = "No AboutUs records found." });

            return Ok(new Response<List<AboutUsDto>>
            {
                Data = itemEntities,
                IsError = false,
                Message = "",
                Status = (int)StatusCodeEnum.Ok
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateAboutUs([FromForm] AboutUsForCreationDto model, CancellationToken cancellationToken)
        {
            try
            {


                if (model == null)
                    return BadRequest(new { Message = "Invalid model." });

                var item = model.Adapt<AboutUs>();
                item.CreatedBy = "admin";

                if (model.Image != null)
                {
                    var fileModel = await _filesManager.UploadFile(model.Image, "AboutUs");
                    item.ImageUrl = fileModel?.Url;
                }

                var result = await _aboutUsRepository.Create(item);
                await _aboutUsRepository.SaveAsync(cancellationToken);

                return Ok(new Response<Guid>
                {
                    Data = result.Id,
                    IsError = false,
                    Message = "AboutUs entry created successfully.",
                    MessageAr = "تمت عملية الاضافة بنجاح",
                    Status = (int)StatusCodeEnum.Ok
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Response<string>
                {
                    Data = ex.Message,
                    IsError = true,
                    Message = "An error occurred while processing your request.",
                    MessageAr = "حدث خطأ اثناء الاضافة",
                    Status = (int)StatusCodeEnum.ServerError
                });
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAboutUs([FromForm] AboutUsDto model, CancellationToken cancellationToken)
        {
            if (model == null)
                return BadRequest(new { Message = "Invalid model." });

            var item = await _aboutUsRepository.Find(a => a.Id == model.Id, false);
            if (item == null)
                return NotFound(new { Message = "AboutUs entry not found." });

            item = model.Adapt<AboutUs>();
            item.CreatedBy = "admin";

            if (model.Image != null)
            {
                var fileModel = await _filesManager.UploadFile(model.Image, "AboutUs");
                item.ImageUrl = fileModel?.Url;
            }

            _aboutUsRepository.Update(item);
            await _aboutUsRepository.SaveAsync(cancellationToken);

            return Ok(new Response<Guid>
            {
                Data = item.Id,
                IsError = false,
                Message = "AboutUs entry updated successfully.",
                MessageAr ="تم التعديل بنجاح",
                Status = (int)StatusCodeEnum.Ok
            });
        }

        [HttpGet("{Id}")]
        public async Task<IActionResult> GetAboutUsById(Guid Id)
        {
            if (Id == Guid.Empty)
                return BadRequest(new { Message = "Invalid ID." });

            var item = await _aboutUsRepository.Find(a => a.Id == Id);
            if (item == null)
                return NotFound(new { Message = "AboutUs entry not found." });

            var itemEntity = item.Adapt<AboutUsDto>();
            return Ok(new Response<AboutUsDto>
            {
                Data = itemEntity,
                IsError = false,
                Message = "",
                Status = (int)StatusCodeEnum.Ok
            });
        }
        [HttpDelete("{Id}")]
        public async Task<IActionResult> DeleteAboutUs(Guid Id, CancellationToken cancellationToken)
        {

            try
            {
                if (Id == Guid.Empty)
                    return BadRequest(new { Message = "Invalid ID." });

                var item = await _aboutUsRepository.Find(a => a.Id == Id, true);
                if (item == null)
                    return NotFound(new { Message = "AboutUs entry not found." });

                _aboutUsRepository.Delete(item);
                await _aboutUsRepository.SaveAsync(cancellationToken);

                return Ok(new Response<Guid>
                {
                    Data = item.Id,
                    IsError = false,
                    Message = "AboutUs entry deleted successfully.",
                    MessageAr = "تم المسح بنجاح",
                    Status = (int)StatusCodeEnum.Ok
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Response<string>
                {
                    Data = ex.Message,
                    IsError = true,
                    Message = "An error occurred while processing your request.",
                    MessageAr = "حدث خطأ اثناء المسح",
                    Status = (int)StatusCodeEnum.ServerError
                });
            }
        }

    }
}
