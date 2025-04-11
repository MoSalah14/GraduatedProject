using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OutbornE_commerce.BAL.Dto.ContactUs;
using OutbornE_commerce.BAL.EmailServices;
using OutbornE_commerce.BAL.Repositories.ContactUs;

namespace OutbornE_commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactUsController : ControllerBase
    {
        private readonly IContactUsRepository _contactUsRepository;
        private readonly IEmailSenderCustom _EmailSender;

        public ContactUsController(IContactUsRepository contactUsRepository, IEmailSenderCustom emailSender)
        {
            _contactUsRepository = contactUsRepository;
            _EmailSender = emailSender;
        }

        [HttpGet("All")]
        public async Task<IActionResult> GetAllContactUs(int pageNumber = 1, int pageSize = 10, string? searchTerm = null)
        {
            var items = new PagainationModel<IEnumerable<ContactUs>>();
            if (string.IsNullOrEmpty(searchTerm))
                items = await _contactUsRepository.FindAllAsyncByPagination(null, pageNumber, pageSize);
            else
                items = await _contactUsRepository.FindAllAsyncByPagination(c => (c.OrderNo.Contains(searchTerm)
                                                                                                           || c.Email.Contains(searchTerm)
                                                                                                           || c.FirstName.Contains(searchTerm)
                                                                                                           || c.LastName.Contains(searchTerm)
                                                                                                           || c.PhoneNo.Contains(searchTerm)
                                                                                                           || c.Subject.Contains(searchTerm)
                                                                                                           || c.Body.Contains(searchTerm)
                                                                                                           || c.InquireTypeId.ToString().Contains(searchTerm)
                                                                                                           ), pageNumber, pageSize);
            var data = items.Data.Adapt<List<ContactUsDto>>();
            return Ok(new PaginationResponse<List<ContactUsDto>>
            {
                Data = data,
                IsError = false,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Status = (int)StatusCodeEnum.Ok,
                TotalCount = items.TotalCount,
            });
        }

        //[HttpGet]
        //public async Task<IActionResult> GetAllContactUs()
        //{
        //    var items = await _contactUsRepository.FindAllAsync(null);
        //    var itemEntities = items.Adapt<List<ContactUsDto>>();
        //    return Ok(new Response<List<ContactUsDto>>
        //    {
        //        Data = itemEntities,
        //        IsError = false,
        //        Message = "",
        //        Status = (int)StatusCodeEnum.Ok
        //    });
        //}

        [HttpGet("{Id}")]
        public async Task<IActionResult> GetContactUsById(Guid Id)
        {
            var item = await _contactUsRepository.Find(c => c.Id == Id);
            if (item is null)
                return Ok(new Response<ContactUsDto>
                {
                    Data = null,
                    IsError = true,
                    Message = "",
                    Status = (int)StatusCodeEnum.NotFound
                });
            var itemEntity = item.Adapt<ContactUsDto>();
            return Ok(new Response<ContactUsDto>
            {
                Data = itemEntity,
                IsError = false,
                Message = "",
                Status = (int)StatusCodeEnum.Ok
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateContactUs([FromBody] ContactUsForCreationDto model, CancellationToken cancellationToken)
        {
            var item = model.Adapt<ContactUs>();
            item.CreatedBy = "User";
            var result = await _contactUsRepository.Create(item);
            await _EmailSender.SendEmailContactUsAsync(model);
            await _contactUsRepository.SaveAsync(cancellationToken);
            return Ok(new Response<Guid>
            {
                Data = result.Id,
                IsError = false,
                Message = "Send Succeffully",
                MessageAr = " تم الارسال بنجاح",
                Status = (int)StatusCodeEnum.Ok
            });
        }

        [HttpPut]
        public async Task<IActionResult> UpdateContactUs([FromBody] ContactUsDto model, CancellationToken cancellationToken)
        {
            var item = await _contactUsRepository.Find(c => c.Id == model.Id);
            if (item is null)
                return Ok(new Response<ContactUsDto>
                {
                    Data = null,
                    IsError = true,
                    Message = "",
                    Status = (int)StatusCodeEnum.NotFound
                });
            item = model.Adapt<ContactUs>();
            item.CreatedBy = "user";
            _contactUsRepository.Update(item);
            await _contactUsRepository.SaveAsync(cancellationToken);
            return Ok(new Response<Guid>
            {
                Data = item.Id,
                IsError = false,
                Message = "Success",
                MessageAr = "تم التعديل  بنجاح ",
                Status = (int)StatusCodeEnum.Ok
            });
        }

        [HttpDelete("{Id}")]
        public async Task<IActionResult> DeleteContactUs(Guid Id, CancellationToken cancellationToken)
        {
            var item = await _contactUsRepository.Find(c => c.Id == Id);
            if (item is null)
                return Ok(new Response<ContactUsDto>
                {
                    Data = null,
                    IsError = true,
                    Message = "",
                    Status = (int)StatusCodeEnum.NotFound
                });
            _contactUsRepository.Delete(item);
            await _contactUsRepository.SaveAsync(cancellationToken);
            return Ok(new Response<Guid>
            {
                Data = item.Id,
                IsError = false,
                Message = " Deleted Successfully",
                MessageAr = "تم الحذف  بنجاح",
                Status = (int)StatusCodeEnum.Ok
            });
        }

    }
}
