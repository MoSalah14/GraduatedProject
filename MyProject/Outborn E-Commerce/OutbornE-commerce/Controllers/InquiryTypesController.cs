using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OutbornE_commerce.BAL.Dto.InquiryTypes;
using OutbornE_commerce.BAL.Repositories.InquiryTypes;

namespace OutbornE_commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InquiryTypesController : ControllerBase
    {
        private readonly IInquiryTypeRepository _inquiryTypeRepository;

        public InquiryTypesController(IInquiryTypeRepository inquiryTypeRepository)
        {
            _inquiryTypeRepository = inquiryTypeRepository;
        }
        [HttpGet]
        public async Task<IActionResult>GetAllInquiryTypes(int pageNumber = 1 ,  int pageSize = 10 , string? searchTerm = null)
        {
            var items = new PagainationModel<IEnumerable<InquiryType>>();
            if(string.IsNullOrEmpty(searchTerm) )
                items = await _inquiryTypeRepository.FindAllAsyncByPagination(null,pageNumber, pageSize);
            else
                items = await _inquiryTypeRepository.FindAllAsyncByPagination(i => (i.NameEn.Contains(searchTerm)
                                                                                                             || i.NameAr.Contains(searchTerm))
                                                                                                             ,pageNumber, pageSize);
            var data = items.Data.Adapt<List<InquiryTypeDto>>();
            return Ok(new PaginationResponse<List<InquiryTypeDto>>
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
        public async Task<IActionResult> GetAllInquiryTypes()
        {
            var items = await _inquiryTypeRepository.FindAllAsync(null);
            var itemEntites = items.Adapt<List<InquiryTypeDto>>();
            return Ok(new Response<List<InquiryTypeDto>>
            {
                Data = itemEntites,
                IsError = false,
                Message = "",
                Status = (int)StatusCodeEnum.Ok

            });
        }
        [HttpGet("{Id}")]
        public async Task<IActionResult> GetInquiryTypeById(Guid Id)
        {
            var item = await _inquiryTypeRepository.Find(i => i.Id == Id);
            if (item is null)
                return Ok(new Response<InquiryTypeDto>
                {
                    Data = null,
                    IsError = true,
                    Message = "",
                    Status = (int)StatusCodeEnum.NotFound
                });
            var itemEntity = item.Adapt<InquiryTypeDto>();
            return Ok(new Response<InquiryTypeDto>
            {
                Data = itemEntity,
                IsError = false,
                Message = "",
                Status = (int)StatusCodeEnum.Ok
            });
        }
        [HttpPost]
        public async Task<IActionResult> CreateInquiryType([FromBody] InquiryTypeForCreationDto model, CancellationToken cancellationToken)
        {
            var item = model.Adapt<InquiryType>();
            item.CreatedBy = "admin";
            var result = await _inquiryTypeRepository.Create(item);
            await _inquiryTypeRepository.SaveAsync(cancellationToken);
            return Ok(new Response<Guid>
            {
                Data = result.Id,
                IsError = false,
                Message = "Success",
                MessageAr = "تم بنجاح",
                Status = (int)StatusCodeEnum.Ok
            });
        }
        [HttpPut]
        public async Task<IActionResult> UpdateInquiryType([FromBody] InquiryTypeDto model, CancellationToken cancellationToken)
        {
            var item = await _inquiryTypeRepository.Find(i => i.Id == model.Id);
            if (item is null)
                return BadRequest(new Response<InquiryTypeDto>
                {
                    Data = null,
                    IsError = true,
                    Message = "fail",
                    MessageAr = "لم يتم العثور عن استفسار",
                    Status = (int)StatusCodeEnum.NotFound
                });
            item = model.Adapt<InquiryType>();
            item.CreatedBy = "admin";
            _inquiryTypeRepository.Update(item);
            await _inquiryTypeRepository.SaveAsync(cancellationToken);
            return Ok(new Response<Guid>
            {
                Data = item.Id,
                IsError = false,
                Message = "updated Successfully",
                MessageAr = "تم التعديل بنجاح",
                Status = (int)StatusCodeEnum.Ok
            });
        }
        [HttpDelete("{Id}")]
        public async Task<IActionResult>DeleteInquiryType(Guid Id , CancellationToken cancellationToken)
        {
            var item = await _inquiryTypeRepository.Find(i => i.Id == Id);
            if (item is null)
                return Ok(new Response<InquiryTypeDto>
                {
                    Data = null,
                    IsError = true,
                    Message = "fail",
                    MessageAr = "لم يتم العثور عن استفسار",
                    Status = (int)StatusCodeEnum.NotFound
                });
            _inquiryTypeRepository.Delete(item);
            await _inquiryTypeRepository.SaveAsync(cancellationToken) ;
            return Ok(new Response<Guid>
            {
                Data = item.Id,
                IsError = false,
                Message = "Deleted Successfully",
                MessageAr = "تم  الحذف بنجاح",
                Status = (int)StatusCodeEnum.Ok
            });
        }
    }

}
