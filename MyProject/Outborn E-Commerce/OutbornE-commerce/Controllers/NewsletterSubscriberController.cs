using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OutbornE_commerce.BAL.Dto.Newsletters;
using OutbornE_commerce.BAL.Repositories.NewsletterSubscribers;

namespace OutbornE_commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewsletterSubscriberController : ControllerBase
    {
        private readonly INewsletterSubscriberRepository newsletterSubscriberRepository;
       public NewsletterSubscriberController(INewsletterSubscriberRepository newsletterSubscriberRepository)
        {
            this.newsletterSubscriberRepository=newsletterSubscriberRepository;
        }
        [HttpGet("GetAllNewslettersSubscriber")]
        public async Task<IActionResult> GetAllNewslettersSubscriber(int pageNumber = 1, int pageSize = 10, string? searchTerm = null)
        {
            var items = new PagainationModel<IEnumerable<NewsletterSubscriber>>();

            if (string.IsNullOrEmpty(searchTerm))
                items = await newsletterSubscriberRepository.FindAllAsyncByPagination(null, pageNumber, pageSize);
            else
                items = await newsletterSubscriberRepository
                                    .FindAllAsyncByPagination(b => (b.Email.Contains(searchTerm)), pageNumber, pageSize);
            var data = items.Data.Adapt<List<NewsletterSubscriberDto>>();

            return Ok(new PaginationResponse<List<NewsletterSubscriberDto>>
            {
                Data = data,
                IsError = false,
                Status = (int)StatusCodeEnum.Ok,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = items.TotalCount
            });
        }

        [HttpPost("Createsubscribe")]
        public async Task<IActionResult> Createsubscribe(string Email, CancellationToken cancellationToken)
        {
            try
            {
                var newsletterSubscriber = new NewsletterSubscriber
                {
                    CreatedBy = "User",
                    Email = Email,
                    CreatedOn = DateTime.UtcNow,
                };

                await newsletterSubscriberRepository.Create(newsletterSubscriber);
                await newsletterSubscriberRepository.SaveAsync(cancellationToken);

                return Ok(new Response<string>
                {
                    Data = Email,
                    IsError = false,
                    Message = "Created successfully",
                    MessageAr = "تم الاضافه بنجاح",
                    Status = (int)StatusCodeEnum.Ok
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new Response<object>
                {
                    Data = null,
                    IsError = true,
                    Status = (int)StatusCodeEnum.BadRequest,
                    Message = "This email is already subscribed.",
                    MessageAr="الجميل مستخدم من قبل"
                });
            }
        }
        [HttpGet("GetNewslettersSubscriberById{email}")]
        public async Task<IActionResult> GetNewslettersSubscriberById(string email)
        {
            var newsletter = await newsletterSubscriberRepository.Find(n => n.Email == email);

            if (newsletter == null)
            {
                return NotFound(new Response<NewsletterSubscriberDto>
                {
                    Data = null,
                    IsError = true,
                    Status = (int)StatusCodeEnum.NotFound,
                    Message = "Newsletter subscriber not found.",
                    MessageAr = "لم يتم العثور على مشترك النشرة البريدية."
                });
            }

            var data = newsletter.Adapt<NewsletterSubscriberDto>();
            return Ok(new Response<NewsletterSubscriberDto>
            {
                Data = data,
                IsError = false,
                Status = (int)StatusCodeEnum.Ok,
                Message = "Newsletter subscriber retrieved successfully.",
                MessageAr = "تم استرجاع مشترك النشرة البريدية بنجاح."
            });
        }


        [HttpDelete("{email}")]
        public async Task<IActionResult> DeleteNewsletterSubscriber(string email, CancellationToken cancellationToken)
        {

            var Subscriber = await newsletterSubscriberRepository.Find(c => c.Email == email, false);

            if (Subscriber == null)
            {
                return NotFound(new Response<string>
                {
                    Data = null,
                    IsError = true,
                    Message = " newsletterSubscriber not found",
                    MessageAr = "لم يتم العثور ",
                    Status = (int)StatusCodeEnum.NotFound
                });
            }
           
            newsletterSubscriberRepository.Delete(Subscriber);
            await newsletterSubscriberRepository.SaveAsync(cancellationToken);
            return Ok(new Response<string>
            {
                Data =email ,
                IsError = false,
                Message = "Deleted Successfully",
                MessageAr = "تم الحذف بنجاح",
                Status = (int)StatusCodeEnum.Ok
            });
        }



    }
}
