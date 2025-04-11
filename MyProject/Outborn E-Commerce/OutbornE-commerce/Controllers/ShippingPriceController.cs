using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OutbornE_commerce.BAL.Repositories.Address;
using OutbornE_commerce.BAL.Repositories.BaseRepositories;
using OutbornE_commerce.BAL.Repositories.ShippingPriceRepo;
using OutbornE_commerce.DAL.Models;

namespace OutbornE_commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShippingPriceController : ControllerBase
    {
        private readonly IShippingPriceRepo repository;
        private readonly IAddressRepository addressRepository;

        public ShippingPriceController(IShippingPriceRepo repository, IAddressRepository addressRepository)
        {
            this.repository = repository;
            this.addressRepository = addressRepository;
        }

        [HttpGet("GetPriceByCountryId")]
        public async Task<IActionResult> GetPriceByCountryId(Guid countryId, double ProductWeight, bool IsExpress = false)
        {
            string UAE_COUNTRY_ID = "b393e0e8-ec9c-467a-b894-9923b3743580";
            double UAE_STANDARD_PRICE = 20.00;
            double UAE_EXPRESS_PRICE = 35.00;
            double MAX_WEIGHT = 10.00;

            if (countryId.ToString() == UAE_COUNTRY_ID)
                return Ok(IsExpress ? UAE_EXPRESS_PRICE : UAE_STANDARD_PRICE);

            double queryWeight = ProductWeight >= MAX_WEIGHT ? MAX_WEIGHT : ProductWeight;

            var GetShippingPrice = await repository.Find(c => c.CountryId == countryId && c.Weight == queryWeight);

            if (GetShippingPrice is null)
            {
                return NotFound(new Response<string>
                {
                    IsError = true,
                    Message = "Data Not Found",
                    MessageAr = "لم يتم العثور على بيانات",
                    Status = (int)StatusCodeEnum.NotFound
                });
            }
            return Ok(GetShippingPrice.Price);
        }

        [HttpGet("GetPriceByAddressId")]
        public async Task<IActionResult> GetPriceByAddressId(Guid AddressId, double ProductWeight, bool IsExpress = false)
        {
            string UAE_COUNTRY_ID = "b393e0e8-ec9c-467a-b894-9923b3743580";
            double UAE_STANDARD_PRICE = 20.00;
            double UAE_EXPRESS_PRICE = 35.00;
            double MAX_WEIGHT = 10.00;

            var GetUserAddress = await addressRepository.Find(e => e.Id == AddressId);
            if (GetUserAddress is not null)
            {
                if (GetUserAddress.CountryId.ToString() == UAE_COUNTRY_ID)
                    return Ok(IsExpress ? UAE_EXPRESS_PRICE : UAE_STANDARD_PRICE);

                double queryWeight = ProductWeight >= MAX_WEIGHT ? MAX_WEIGHT : ProductWeight;

                var GetShippingPrice = await repository.Find(c => c.CountryId == GetUserAddress.CountryId && c.Weight == queryWeight);

                if (GetShippingPrice is null)
                {
                    return NotFound(new Response<string>
                    {
                        IsError = true,
                        Message = "Data Not Found",
                        MessageAr = "لم يتم العثور على بيانات",
                        Status = (int)StatusCodeEnum.NotFound
                    });
                }
                return Ok(GetShippingPrice.Price);
            }
            return BadRequest(new Response<string>
            {
                IsError = true,
                Message = "Data Not Found",
                MessageAr = "لم يتم العثور على بيانات",
                Status = (int)StatusCodeEnum.NotFound
            });
        }
    }
}