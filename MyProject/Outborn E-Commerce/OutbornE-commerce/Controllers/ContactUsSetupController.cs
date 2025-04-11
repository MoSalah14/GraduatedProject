using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OutbornE_commerce.BAL.Dto.ContactUsSetups;
using OutbornE_commerce.BAL.Repositories.ContactUsSetups;

namespace OutbornE_commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactUsSetupController : ControllerBase
    {
        private readonly IContactUsSetupRepository _contactUsSetupRepository;

        public ContactUsSetupController(IContactUsSetupRepository contactUsSetupRepository)
        {
            _contactUsSetupRepository = contactUsSetupRepository;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllContactUsSetup()
        {
            var items = await _contactUsSetupRepository.FindAllAsync(null);
            var itemEntites = items.Adapt<List<ContactUsSetupDto>>();
            return Ok(new Response<List<ContactUsSetupDto>>
            {
                Data = itemEntites,
                IsError = false,
                Message = "",
                Status = (int)StatusCodeEnum.Ok
            });
        }
        [HttpGet("{Id}")]
        public async Task<IActionResult> GetContactUsSetupById(Guid Id)
        {

            var item = await _contactUsSetupRepository.Find(c => c.Id == Id);
            if (item is null)
                return Ok(new Response<ContactUsSetupDto>
                {
                    Data = null,
                    IsError = true,
                    Message = "",
                    Status = (int)StatusCodeEnum.NotFound
                });
            var itemEntity = item.Adapt<ContactUsSetupDto>();
            return Ok(new Response<ContactUsSetupDto>
            {
                Data = itemEntity,
                IsError = false,
                Message = "",
                Status = (int)StatusCodeEnum.Ok
            });
        }
        [HttpPost]
        public async Task<IActionResult> CreateContactUsSetup([FromBody] ContactUsSetupForCreationDto model, CancellationToken cancellationToken)
        {
            var item = model.Adapt<ContactUsSetup>();
            item.CreatedBy = "admin";
            var result = await _contactUsSetupRepository.Create(item);
            await _contactUsSetupRepository.SaveAsync(cancellationToken);
            return Ok(new Response<Guid>
            {
                Data = result.Id,
                IsError = false,
                Message = "created successfully",
                MessageAr="تم الاضافه بنجاح",
                Status = (int)StatusCodeEnum.Ok
            });
        }
        [HttpPut]
        public async Task<IActionResult> UpdateContactUsSetup([FromBody] ContactUsSetupDto model, CancellationToken cancellationToken)
        {
            var item = await _contactUsSetupRepository.Find(c => c.Id == model.Id);
            if (item is null)
                return Ok(new Response<ContactUsSetupDto>
                {
                    Data = null,
                    IsError = true,
                    Message = "Updated Filled",
                    MessageAr="فشل التعديل",
                    Status = (int)StatusCodeEnum.NotFound
                });
            item = model.Adapt<ContactUsSetup>();
            item.CreatedBy = "admin";
            _contactUsSetupRepository.Update(item);
            await _contactUsSetupRepository.SaveAsync(cancellationToken);
            return Ok(new Response<Guid>
            {
                Data = item.Id,
                IsError = false,
                Message = "Updated successfully",
                MessageAr = "تم التعديل بنجاح",
                Status = (int)StatusCodeEnum.Ok
            });
        }
        [HttpDelete("{Id}")]
        public async Task<IActionResult> DeleteContactUsSetup(Guid Id, CancellationToken cancellationToken)
        {
            var item = await _contactUsSetupRepository.Find(c => c.Id == Id, true);
            if (item is null)
                return Ok(new Response<ContactUsSetupDto>
                {
                    Data = null,
                    IsError = true,
                    Message = "",
                    Status = (int)StatusCodeEnum.NotFound
                });
            _contactUsSetupRepository.Delete(item);
            await _contactUsSetupRepository.SaveAsync(cancellationToken);
            return Ok(new Response<Guid>
            {
                Data = item.Id,
                IsError = false,
                Message = "deleted successfully",
                MessageAr = "تم الحذف بنجاح",
                Status = (int)StatusCodeEnum.Ok
            });
        }
    }
}
