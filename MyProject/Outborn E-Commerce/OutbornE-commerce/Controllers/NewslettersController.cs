using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using OutbornE_commerce.BAL.Dto.Newsletters;
using OutbornE_commerce.BAL.EmailServices;
using OutbornE_commerce.BAL.Repositories.Newsletters;
using OutbornE_commerce.BAL.Repositories.NewsletterSubscribers;
using OutbornE_commerce.BAL.Repositories.SMTP_Server;
using OutbornE_commerce.DAL.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace OutbornE_commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewslettersController : ControllerBase
    {
        private readonly INewsletterRepository _newsletterRepository;
        private readonly INewsletterSubscriberRepository _newsletterSubscriberRepository;
        private readonly ISMTPRepository _SMTPRepository;
        private readonly IEmailSenderCustom _emailSender;

        public NewslettersController(
            INewsletterRepository newsletterRepository, INewsletterSubscriberRepository _newsletterSubscriberRepository,
            ISMTPRepository sMTPRepository,
            IEmailSenderCustom emailSender)
        {
            _newsletterRepository = newsletterRepository;
            _SMTPRepository = sMTPRepository;
            _emailSender = emailSender;
            this._newsletterSubscriberRepository = _newsletterSubscriberRepository;

        }
        [HttpGet]
        public async Task<IActionResult> GetAllNewsletters(int pageNumber = 1,int pageSize = 10,string? searchTerm= null)
        {
            var items = new PagainationModel<IEnumerable<Newsletter>>();

            if (string.IsNullOrEmpty(searchTerm))
                items = await _newsletterRepository.FindAllAsyncByPagination(null, pageNumber, pageSize);
            else
                items = await _newsletterRepository
                                    .FindAllAsyncByPagination(b => (b.Subject.Contains(searchTerm)
                                                               || b.Body.Contains(searchTerm))
                                                               , pageNumber, pageSize);
            var data = items.Data.Adapt<List<NewsletterDto>>();

            return Ok(new PaginationResponse<List<NewsletterDto>>
            {
                Data = data,
                IsError = false,
                Status = (int)StatusCodeEnum.Ok,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = items.TotalCount
            });
        }
        [HttpPost]
        public async Task<IActionResult> CreateNewsletter([FromBody] NewsletterDto model, CancellationToken cancellationToken)
        {
            try
            {
                var newsLetter = model.Adapt<Newsletter>();
                newsLetter.CreatedBy = "admin";
                newsLetter.CreatedOn = DateTime.Now;

                await _newsletterRepository.Create(newsLetter);
                await _newsletterRepository.SaveAsync(cancellationToken);

                try
                {
                    var sendMultipleEmailsDto = new SendMultipleEmailsDto
                    {
                        Subject=model.Subject,
                        Body=model.Body,
                        Emails=model.Emails
                    };
                    await _emailSender.SendEmailToListAsync(sendMultipleEmailsDto);
                }
                catch (Exception emailEx)
                {
                    return BadRequest(new Response<string>
                    {
                        Data = null,
                        IsError = true,
                        Status = (int)StatusCodeEnum.ServerError,
                        Message = "Newsletter was saved, but there was an error sending the emails.",
                        MessageAr= "تم حفظ النشرة البريدية، ولكن حدث خطأ أثناء إرسال الرسائل الإلكترونية.",
                    });
                }

                return Ok(new Response<Guid>
                {
                    Data = newsLetter.Id,
                    IsError = false,
                    Message = "Created successfully",
                    MessageAr = "تم الاضافه بنجاح",
                    Status = (int)StatusCodeEnum.Ok
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating newsletter: {ex.Message}");

                return BadRequest(new Response<string>
                {
                    Data = null,
                    IsError = true,
                    Status = (int)StatusCodeEnum.ServerError,
                    Message = "An error occurred while creating the newsletter.",
                    MessageAr= "حدث خطأ أثناء إنشاء النشرة البريدية"
                });
            }
        }

    
    [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var newsletter = await _newsletterRepository.Find(n => n.Id == id);

            if (newsletter == null)
            {
                return NotFound(new Response<NewsletterDto>
                {
                    Data = null,
                    IsError = true,
                    Status = (int)StatusCodeEnum.NotFound,
                    Message = "Newsletter not found.",
                    MessageAr = "النشرة البريدية غير موجودة."
                });
            }

            var data = newsletter.Adapt<NewsletterDto>();
            return Ok(new Response<NewsletterDto>
            {
                Data = data,
                IsError = false,
                Status = (int)StatusCodeEnum.Ok,
                Message = "Newsletter retrieved successfully.",
                MessageAr = "تم استرجاع النشرة البريدية بنجاح."
            });
        }


    }
}
