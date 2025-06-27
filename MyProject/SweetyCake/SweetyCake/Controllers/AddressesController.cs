using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OutbornE_commerce.BAL.Dto.Address;
using OutbornE_commerce.BAL.Repositories;
using OutbornE_commerce.Extensions;
using System.Threading;

namespace OutbornE_commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AddressesController : ControllerBase
    {
        private readonly IAddressRepository _addressRepository;

        public AddressesController(IAddressRepository addressRepository)
        {
            _addressRepository = addressRepository;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllAddresses()
        {
            var addresses = await _addressRepository.FindAllAsync(null);
            var addressEntities = addresses.Adapt<List<AddressDto>>();

            //var response = new Response<List<AddressDto>>
            //{
            //    Data = addressEntities,
            //    IsError = false,
            //    Message = "",
            //    MessageAr = "",
            //    Status = 200
            //};
            //return Ok(response);
            return Ok(new Response<List<AddressDto>>
            {
                Data = addressEntities,
                IsError = false,
                Message = "",
                MessageAr = "",
                Status = 200
            });
        }


        
        [HttpGet("GetAllAddressForUser")]
        public async Task<IActionResult> GetAllAddressesForSpecifecUser()
        {
            var UserID = User.GetUserIdFromToken();
            var addresses = await _addressRepository.FindByCondition(us => us.UserId == UserID);
            var addressEntities = addresses.Adapt<List<AddressDto>>();
            return Ok(new Response<List<AddressDto>>
            {
                Data = addressEntities,
                IsError = false,
                Message = "",
                MessageAr = "",
                Status = 200
            });
        }

        [HttpGet("{Id}")]
        public async Task<IActionResult> GetAddressById(Guid Id)
        {
            var address = await _addressRepository.Find(a => a.Id == Id);
            var addressEntity = address.Adapt<AddressDto>();
            if (address is null)
                return NotFound(new Response<AddressDto>
                {
                    Message = "Address Not Found",
                    MessageAr = "العنوان غير موجود",
                    IsError = true,
                    Status = (int)StatusCodeEnum.NotFound
                });
            return Ok(new Response<AddressDto>
            {
                Data = addressEntity,
                IsError = false,
                Status = (int)StatusCodeEnum.Ok,
            });
        }

        [HttpDelete("{Id}")]
        public async Task<IActionResult> DeleteAddress(Guid Id, CancellationToken cancellationToken)
        {
            var address = await _addressRepository.Find(a => a.Id == Id, true);
            if (address is null)
                return NotFound(new Response<AddressDto>
                {
                    Message = "Address Not Found",
                    MessageAr = "العنوان غير موجود",
                    IsError = true,
                    Status = (int)StatusCodeEnum.NotFound
                });

            address.IsDeleted = true;
            await _addressRepository.SaveAsync(cancellationToken);

            return Ok(new Response<Guid>
            {
                Data = Id,
                IsError = false,
                Message = "deleted successfully",
                MessageAr = "تم الحذف بنجاح",
                Status = (int)StatusCodeEnum.Ok
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateAddress([FromBody] AddressForCreationDto model, CancellationToken cancellationToken)
        {
            var UserID = User.GetUserIdFromToken();

            var address = model.Adapt<Address>();
            address.CreatedBy = "admin";
            address.UserId = UserID;
            var result = await _addressRepository.Create(address);
            await _addressRepository.SaveAsync(cancellationToken);
            return Ok(new Response<Guid>
            {
                Data = result.Id,
                IsError = false,
                Message = "Created successfully",
                MessageAr = "تم الاضافه بنجاح",
                Status = (int)StatusCodeEnum.Ok
            });
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAddress([FromBody] AddressDto model, CancellationToken cancellationToken)
        {
            var UserID = User.GetUserIdFromToken();

            var address = await _addressRepository.Find(a => a.Id == model.Id && a.UserId == UserID);

            if (address == null)
            {
                return NotFound(new Response<string>
                {
                    Data = null,
                    IsError = true,
                    Message = "Address not found",
                    MessageAr = "العنوان غير موجود",
                    Status = (int)StatusCodeEnum.NotFound
                });
            }

            model.Adapt(address);
            address.UserId = UserID;
            address.UpdatedBy = "admin";
            address.UpdatedOn = DateTime.Now;

            _addressRepository.Update(address);
            await _addressRepository.SaveAsync(cancellationToken);

            return Ok(new Response<Guid>
            {
                Data = address.Id,
                IsError = false,
                Message = "Updated successfully",
                MessageAr = "تم التعديل بنجاح",
                Status = (int)StatusCodeEnum.Ok
            });
        }
    }
}