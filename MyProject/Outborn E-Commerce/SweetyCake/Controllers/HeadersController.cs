using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OutbornE_commerce.BAL.Dto.Currencies;
using OutbornE_commerce.BAL.Dto.Headers;
using OutbornE_commerce.BAL.Repositories.Headers;
using OutbornE_commerce.DAL.Models;
using OutbornE_commerce.FilesManager;

namespace OutbornE_commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HeadersController : ControllerBase
    {
        private readonly IHeaderRepository _headerRepository;
        private readonly IFilesManager _filesManager;

        public HeadersController(IHeaderRepository headerRepository, IFilesManager filesManager)
        {
            _headerRepository = headerRepository;
            _filesManager = filesManager;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllHeaders(int pageNumber = 1, int pageSize = 10,string? searchTerm=null)
        {
            var items = new PagainationModel<IEnumerable<Header>>();

            if (string.IsNullOrEmpty(searchTerm))
                items = await _headerRepository.FindAllAsyncByPagination(null, pageNumber, pageSize);
            else
                items = await _headerRepository
                                    .FindAllAsyncByPagination(b => (b.Title1En.Contains(searchTerm)
                                                               || b.Title1En.Contains(searchTerm)
                                                               || b.Title2Ar.Contains(searchTerm)
                                                               || b.Title1En.Contains(searchTerm))
                                                               , pageNumber, pageSize);

            var data = items.Data.Adapt<List<HeaderDto>>();

            return Ok(new PaginationResponse<List<HeaderDto>>
            {
                Data = data,
                IsError = false,
                Status = (int)StatusCodeEnum.Ok,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = items.TotalCount
            });
        }
        [HttpGet("all")]
        public async Task<IActionResult> GetAllHeadersInHome()
        {
              var  items = await _headerRepository.FindAllAsync(null);


            var data = items.Adapt<List<HeaderDto>>();

            return Ok(new Response<List<HeaderDto>>
            {
                Data = data,
                IsError = false,
                Status = (int)StatusCodeEnum.Ok,
            });
        }
        [HttpGet("{Id}")]
        public async Task<IActionResult>GetHeaderById(Guid Id)
        {
            var header = await _headerRepository.Find(i => i.Id == Id, false);
            var headerEntity = header.Adapt<HeaderDto>();
            //if (headerEntity == null)
            //    return Ok(new { message =$"Header with Id{header!.Id} doesn't exist in the database"});
            return Ok(new Response<HeaderDto>
            {
                Data = headerEntity,
                IsError = false,
                Message = "",
                Status = (int)(StatusCodeEnum.Ok)
            });
        }
        [HttpPost]
        public async Task<IActionResult> CreateHeader([FromForm] HeaderForCreationDto model , CancellationToken cancellationToken)
        {
            var header = model.Adapt<Header>();
            header.CreatedBy = "admin";
            if(model.Image != null)
            {
                var fileModel = await _filesManager.UploadFile(model.Image, "Headers");
                header.ImageUrl = fileModel!.Url;
            }
            var result = await _headerRepository.Create(header);
            await _headerRepository.SaveAsync(cancellationToken);
            return Ok(new Response<Guid>
            {
                Data = header.Id,
                IsError = false,
                Message = "Success",
                MessageAr = "تم بنجاح",
                Status = (int)(StatusCodeEnum.Ok)
            });
        }
        [HttpPut]
        public async Task<IActionResult> UpdateHeader([FromForm] HeaderDto model, CancellationToken cancellationToken)
        {
            var header = await _headerRepository.Find(h => h.Id == model.Id, false);
            var OldImage = header.ImageUrl;
            header = model.Adapt<Header>();
            header.CreatedBy = "admin";
            if (model.Image != null)
            {
                var fileModel = await _filesManager.UploadFile(model.Image, "Headers",OldImage);
                header.ImageUrl = fileModel!.Url;
            }
           _headerRepository.Update(header);
            await _headerRepository.SaveAsync(cancellationToken);
            return Ok(new Response<Guid>
            {
                Data = header.Id,
                IsError = false,
                Message = "Updated  Successfully",
                MessageAr = "تم التعديل بنجاح",
                Status = (int)(StatusCodeEnum.Ok)
            });
        }
        [HttpDelete("{Id}")]
        public async Task<IActionResult> DeleteHeader(Guid Id , CancellationToken cancellationToken)
        {
            var header = await _headerRepository.Find(h => h.Id==Id, false);
            if(header == null)
                return BadRequest(new { message = $"Header with Id{header!.Id} doesn't exist in the database" });
            _headerRepository.Delete(header);
            await _headerRepository.SaveAsync(cancellationToken);
            return Ok(new Response<Guid> { Data = header.Id, IsError = false,
                Message = "Deleted Successfully",
                MessageAr = "تم  الحذف بنجاح",
                Status = (int)(StatusCodeEnum.Ok) });
        }
    }
}
