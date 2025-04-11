using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OutbornE_commerce.BAL.Dto;
using OutbornE_commerce.BAL.Dto.Categories;
using OutbornE_commerce.BAL.Dto.Countries;
using OutbornE_commerce.BAL.Dto.Currencies;
using OutbornE_commerce.BAL.Repositories.Countries;
using OutbornE_commerce.DAL.Models;

namespace OutbornE_commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountriesController : ControllerBase
    {
        private readonly ICountryRepository _countryRepository;

        public CountriesController(ICountryRepository countryRepository)
        {
            _countryRepository = countryRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCountriesPaginated(int pageNumber = 1, int pageSize = 10, string? searchTerm = null)
        {
            var items = new PagainationModel<IEnumerable<Country>>();

            if (string.IsNullOrEmpty(searchTerm))
                items = await _countryRepository.FindAllAsyncByPagination(null, pageNumber, pageSize);
            else
                items = await _countryRepository
                                    .FindAllAsyncByPagination(b => (b.NameAr.Contains(searchTerm)
                                                               || b.NameEn.Contains(searchTerm)
                                                               || b.CountryCode.Contains(searchTerm))
                                                               , pageNumber, pageSize, null);

            var data = items.Data.Adapt<List<CountryDto>>();

            return Ok(new PaginationResponse<List<CountryDto>>
            {
                Data = data.OrderBy(e => e.NameEn).ToList(),
                IsError = false,
                Status = (int)StatusCodeEnum.Ok,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = items.TotalCount
            });
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllCountries()
        {
            var items = await _countryRepository.FindAllAsync(null);

            var data = items.Adapt<List<CountryDto>>();

            return Ok(new Response<List<CountryDto>>
            {
                Data = data.OrderBy(e => e.NameEn).ToList(),
                IsError = false,
                Status = (int)StatusCodeEnum.Ok,
            });
        }

        [HttpGet("{Id}")]
        public async Task<IActionResult> GetCountryById(Guid Id)
        {
            var country = await _countryRepository.Find(c => c.Id == Id, false);
            if (country == null)
                return Ok(new Response<CountryDto>
                {
                    Data = null,
                    IsError = true,
                    Message = $"Country with Id: {country!.Id} doesn't exist in the database",
                    Status = (int)StatusCodeEnum.NotFound,
                });
            var countryEntity = country.Adapt<CountryDto>();
            return Ok(new Response<CountryDto>
            {
                Data = countryEntity,
                IsError = false,
                Message = "",
                Status = (int)StatusCodeEnum.Ok,
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateCountry([FromBody] CountryForCreationDto model, CancellationToken cancellationToken)
        {
            var country = model.Adapt<Country>();
            country.CreatedBy = "admin";
            var result = await _countryRepository.Create(country);
            await _countryRepository.SaveAsync(cancellationToken);
            return Ok(new Response<Guid>
            {
                Data = result.Id,
                IsError = false,
                Message = "Success",
                MessageAr = "تم بنجاح",
                Status = (int)StatusCodeEnum.Ok,
            });
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCountry([FromBody] CountryDto model, CancellationToken cancellationToken)
        {
            var country = await _countryRepository.Find(c => c.Id == model.Id, false);
            if (country == null)
                return Ok(new Response<CountryDto>
                {
                    Data = null,
                    IsError = true,
                    Message = $"Country with Id: {country!.Id} doesn't exist in the database",
                    Status = (int)StatusCodeEnum.NotFound,
                });
            country = model.Adapt<Country>();
            country.CreatedBy = "admin";
            _countryRepository.Update(country);
            await _countryRepository.SaveAsync(cancellationToken);
            return Ok(new Response<Guid>
            {
                Data = country.Id,
                IsError = false,
                Message = "Success",
                MessageAr = "تم بنجاح",
                Status = (int)StatusCodeEnum.Ok,
            });
        }

        [HttpDelete("{Id}")]
        public async Task<IActionResult> DeleteCountry(Guid Id, CancellationToken cancellationToken)
        {
            var country = await _countryRepository.Find(c => c.Id == Id, false);
            if (country == null)
                return Ok(new Response<CountryDto>
                {
                    Data = null,
                    IsError = true,
                    Message = $"Country with Id: {country!.Id} doesn't exist in the database",
                    Status = ((int)StatusCodeEnum.NotFound),
                });
            _countryRepository.Delete(country);
            await _countryRepository.SaveAsync(cancellationToken);
            return Ok(new Response<CountryDto>
            {
                Data = null,
                IsError = false,
                Message = "Success",
                MessageAr = "تم الحذف بنجاح",
                Status = (int)StatusCodeEnum.Ok,
            });
        }
    }
}