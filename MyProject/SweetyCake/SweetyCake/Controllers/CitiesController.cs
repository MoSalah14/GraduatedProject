using OutbornE_commerce.BAL.Dto.Categories;
using System.Threading;

namespace OutbornE_commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CitiesController : ControllerBase
    {
        private readonly ICityRepository _cityRepository;
        private readonly ICountryRepository _countryRepository;

        public CitiesController(ICityRepository cityRepository, ICountryRepository countryRepository)
        {
            _cityRepository = cityRepository;
            _countryRepository = countryRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCities(int pageNumber = 1, int pageSize = 10, string? searchTerm = null)
        {
            var items = new PagainationModel<IEnumerable<City>>();

            if (string.IsNullOrEmpty(searchTerm))
                items = await _cityRepository.FindAllAsyncByPagination(null, pageNumber, pageSize, new string[] { "Country" });
            else
                items = await _cityRepository
                                    .FindAllAsyncByPagination(b => (b.NameAr.Contains(searchTerm)
                                                               || b.NameEn.Contains(searchTerm)
                                                               || b.Country.NameEn.Contains(searchTerm)
                                                               || b.Country.NameAr.Contains(searchTerm))
                                                               , pageNumber, pageSize, new string[] { "Country" });

            var data = items.Data.Adapt<List<CityDto>>();

            return Ok(new PaginationResponse<List<CityDto>>
            {
                Data = data,
                IsError = false,
                Status = (int)StatusCodeEnum.Ok,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = items.TotalCount
            });
        }

        [HttpGet("{Id}")]
        public async Task<IActionResult> GetCityById(Guid Id)
        {
            var city = await _cityRepository.Find(c => c.Id == Id, false);
            if (city == null)
                return Ok(new Response<CountryDto>
                {
                    Data = null,
                    IsError = true,
                    Message = $"Country with Id:Id doesn't exist in the database",
                    Status = (int)StatusCodeEnum.NotFound,
                });
            var countryEntity = city.Adapt<CityDto>();
            return Ok(new Response<CityDto>
            {
                Data = countryEntity,
                IsError = false,
                Message = "",
                Status = (int)StatusCodeEnum.Ok,
            });
        }

        [HttpGet("byCountryId")]
        public async Task<IActionResult> GetAllCitiesForCountry(Guid countryId)
        {
            var cities = await _cityRepository.FindByCondition(c => c.CountryId == countryId, null); // null for the includes !!!
            var cityEntites = cities.Adapt<List<CityDto>>();
            return Ok(new Response<List<CityDto>>
            {
                Data = cityEntites,
                IsError = false,
                Message = "",
                Status = (int)StatusCodeEnum.Ok
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateCity([FromBody] CityForCreationDto model, CancellationToken cancellationToken)
        {
            var country = await _countryRepository.Find(c => c.Id == model.CountryId, false);
            if (country == null)
                return Ok(new Response<CountryDto>
                {
                    Data = null,
                    IsError = true,
                    Message = $"Contry with Id: {model.CountryId} doesn't exist in the database",
                    Status = (int)StatusCodeEnum.NotFound
                });
            var city = model.Adapt<City>();
            city.CreatedBy = "admin";
            var result = await _cityRepository.Create(city);
            await _cityRepository.SaveAsync(cancellationToken);
            return Ok(new Response<Guid>
            {
                Data = city.Id,
                IsError = false,
                Message = "Success",
                MessageAr = "تم بنجاح",
                Status = (int)StatusCodeEnum.Ok
            });
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCity([FromBody] CityDto model, CancellationToken cancellationToken)
        {
            var country = await _countryRepository.Find(c => c.Id == model.CountryId, false);
            if (country == null)
                return Ok(new Response<CountryDto>
                {
                    Data = null,
                    IsError = true,
                    Message = $"Contry with Id: {model.CountryId} doesn't exist in the database",
                    Status = (int)StatusCodeEnum.NotFound
                });
            var city = await _cityRepository.Find(c => c.Id == model.Id, false);
            if (city == null)
            {
                return Ok(new Response<CityDto>
                {
                    Data = null,
                    IsError = true,
                    Message = $"City with Id: {model.Id} doesn't exist in the database",
                    Status = (int)StatusCodeEnum.NotFound
                });
            }
            city = model.Adapt<City>();
            city.CreatedBy = "admin";
            _cityRepository.Update(city);
            await _cityRepository.SaveAsync(cancellationToken);
            return Ok(new Response<Guid>
            {
                Data = city.Id,
                IsError = false,
                Message = "Success",
                MessageAr = "تم بنجاح",
                Status = (int)StatusCodeEnum.Ok
            });
        }

        [HttpDelete("{Id}")]
        public async Task<IActionResult> DeleteCity(Guid Id, CancellationToken cancellationToken)
        {
            var city = await _cityRepository.Find(c => c.Id == Id, false);
            if (city == null)
                return Ok(new Response<CityDto>
                {
                    Data = null,
                    IsError = true,
                    Message = $"City with Id: {Id} doesn't exist in the database",
                    Status = (int)StatusCodeEnum.NotFound
                });
            _cityRepository.Delete(city);
            await _cityRepository.SaveAsync(cancellationToken);
            return Ok(new Response<Guid>
            {
                Data = city.Id,
                IsError = false,
                Status = (int)StatusCodeEnum.Ok,
                Message = "Success",
                MessageAr = "تم الحذف  بنجاح",
            });
        }
    }
}