using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OutbornE_commerce.BAL.Dto.FAQs;
using OutbornE_commerce.BAL.Repositories.FAQs;

namespace OutbornE_commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FAQsController : ControllerBase
    {
        private readonly IFAQRepository _faqRepository;

        public FAQsController(IFAQRepository faqRepository)
        {
            _faqRepository = faqRepository;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllFAQ(int pageNumber = 1 , int pagesize = 10 , string? searchTerm = null)
        {
            var items = new PagainationModel<IEnumerable<FAQ>>();
            if(string.IsNullOrEmpty(searchTerm))
                items = await _faqRepository.FindAllAsyncByPagination(null,pageNumber, pagesize);
            else
                items = await _faqRepository.FindAllAsyncByPagination(f =>(f.QuestionAr.Contains(searchTerm))
                                                                                                         || f.QuestionAr.Contains(searchTerm)
                                                                                                         || f.QuestionEn.Contains(searchTerm)
                                                                                                         || f.AnswerAr.Contains(searchTerm)
                                                                                                         || f.AnswerEn.Contains(searchTerm),
                                                                                                         pageNumber,pagesize);
            var data = items.Data.Adapt<List<FAQDto>>();
            return Ok(new PaginationResponse<List<FAQDto>>
            {
                Data = data,
                PageNumber = pageNumber,
                PageSize = pagesize,
                TotalCount = items.TotalCount,
                IsError = false,
                Status = (int)StatusCodeEnum.Ok

            });
        }
        [HttpGet("All")]
        public async Task<IActionResult> GetAllFAQ()
        {
            var faqs = await _faqRepository.FindAllAsync(null);
            var faqEntites = faqs.Adapt<List<FAQDto>>();
            return Ok(new Response<List<FAQDto>>
            {
                Data = faqEntites,
                IsError = false,
                Message = "",
                Status = (int)StatusCodeEnum.Ok
            });
        }
        [HttpGet("{Id}")]
        public async Task<IActionResult> GetFAQById(Guid Id)
        {
            var faq = await _faqRepository.Find(f => f.Id == Id);
            var faqEntity = faq.Adapt<FAQDto>();
            if (faq is null)
                return Ok(new Response<FAQDto>
                {
                    Data= null,
                    IsError = true,
                    Message = "",
                     Status = (int)StatusCodeEnum.NotFound
                });
            return Ok(new Response<FAQDto>
            {
                Data = faqEntity,
                IsError = false,
                Message = "",
                Status = (int)StatusCodeEnum.Ok
            });
        }
        [HttpPost]
        public async Task<IActionResult> CreateFAQ([FromBody] FAQForCreationDto model , CancellationToken cancellationToken)
        {
            var faq = model.Adapt<FAQ>();
            faq.CreatedBy = "admin";
            var result = await _faqRepository.Create(faq);
            await _faqRepository.SaveAsync(cancellationToken);
            return Ok(new Response<Guid>
            {
                Data = result.Id,
                IsError= false,
                Message = "Success",
                MessageAr = "تم بنجاح",
                Status = (int)StatusCodeEnum.Ok
            });
        }
        [HttpPut]
        public async Task<IActionResult> UpdateFAQ([FromBody] FAQDto model , CancellationToken cancellationToken)
        {
            var faq = await _faqRepository.Find(f => f.Id == model.Id);
            if (faq is null)
                return BadRequest(new Response<FAQDto>
                {
                    Data = null,
                    IsError = true,
                    Status = (int)StatusCodeEnum.NotFound,
                    Message = "خطا",
                    MessageAr = "لم يتم العثور",
                });
            faq = model.Adapt<FAQ>();
            _faqRepository.Update(faq);
            await _faqRepository.SaveAsync(cancellationToken);
            return Ok(new Response<Guid>
            {
                Data = faq.Id,
                IsError = false,
                Message = "Success",
                MessageAr = "تم بنجاح",
                Status = (int)StatusCodeEnum.Ok
            });
        }
        [HttpDelete("{Id}")]
        public async Task<IActionResult> DeleteFAQ(Guid Id , CancellationToken cancellationToken)
        {
            var faq = await _faqRepository.Find(f => f.Id == Id);
            if (faq is null)
                return Ok(new Response<FAQDto>
                {
                    Data = null,
                    IsError = true,
                    Message = "",
                    Status = (int)StatusCodeEnum.NotFound
                });
            _faqRepository.Delete(faq);
            await _faqRepository.SaveAsync(cancellationToken);
            return Ok(new Response<Guid>
            {
                Data = faq.Id,
                IsError = false,
                Message = "Deleted Successfully",
                Status = (int)StatusCodeEnum.Ok
            });
        }
    }
}
