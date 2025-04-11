using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OutbornE_commerce.BAL.Dto.Countries;
using OutbornE_commerce.BAL.Dto;
using OutbornE_commerce.BAL.Dto.Currencies;
using OutbornE_commerce.BAL.Repositories.Currencies;
using OutbornE_commerce.DAL.Models;
using OutbornE_commerce.BAL.Dto.Brands;
using OutbornE_commerce.BAL.Services;

namespace OutbornE_commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CurrenciesController : ControllerBase
    {
        private readonly ICurrencyRepository _currencyRepository;
        private readonly ICurrencyService CurrencyService;

        public CurrenciesController(ICurrencyRepository currencyRepository, ICurrencyService _CurrencyService)
        {
            _currencyRepository = currencyRepository;
            CurrencyService = _CurrencyService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllCurrencies(int pageNumber = 1, int pageSize = 10, string? searchTerm = null)
        {
            var items = new PagainationModel<IEnumerable<Currency>>();

            if (string.IsNullOrEmpty(searchTerm))
                items = await _currencyRepository.FindAllAsyncByPagination(null, pageNumber, pageSize);
            else
                items = await _currencyRepository
                                    .FindAllAsyncByPagination(b => (b.NameAr.Contains(searchTerm)
                                                               || b.NameEn.Contains(searchTerm)
                                                               || b.Price.ToString().Contains(searchTerm)
                                                               || b.Sign.Contains(searchTerm))
                                                               , pageNumber, pageSize);

            var data = items.Data.Adapt<List<CurrencyDto>>();

            return Ok(new PaginationResponse<List<CurrencyDto>>
            {
                Data = data,
                IsError = false,
                Status = (int)StatusCodeEnum.Ok,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = items.TotalCount
            });
        }
        [HttpGet("all")]
        public async Task<IActionResult> GetAllCurrenciesSelect()
        {
            var items = await _currencyRepository.FindAllAsync(null);

            var data = items.Adapt<List<CurrencyDto>>();

            return Ok(new Response<List<CurrencyDto>>
            {
                Data = data,
                IsError = false,
                Status = (int)StatusCodeEnum.Ok,
            });
        }
        [HttpGet("{Id}")]
        public async Task<IActionResult> GetCurrencyById(Guid Id)
        {
            var currency = await _currencyRepository.Find(c => c.Id == Id, true);
            if (currency is null)
                return Ok(new Response<CurrencyDto>
                {
                    Data = null,
                    IsError = true,
                    Message = $"Currency with Id: {Id} doesn't exist in the database",
                    Status = (int)(StatusCodeEnum.NotFound)
                });
            var currencyEntity = currency.Adapt<CurrencyDto>();
            return Ok(new Response<CurrencyDto>
            {
                Data = currencyEntity,
                IsError = false,
                Message = "",
                Status = (int)(StatusCodeEnum.Ok)
            });
        }
        [HttpPost]
        public async Task<IActionResult> CreateCurrency([FromBody] CurrencyDto model, CancellationToken cancellationToken)
        {
            var currency = model.Adapt<Currency>();
            currency.CreatedBy = "admin";
            var result = await _currencyRepository.Create(currency);
            await _currencyRepository.SaveAsync(cancellationToken);
            return Ok(new Response<Guid>
            {
                Data = result.Id,
                IsError = false,
                Message = "Success",
                MessageAr = "تم بنجاح",
                Status = (int)(StatusCodeEnum.Ok)
            });
        }
        [HttpPut]
        public async Task<IActionResult> UpdateCurrency([FromBody] CurrencyDto model, CancellationToken cancellationToken)
        {
            var currency = await _currencyRepository.Find(c => c.Id != model.Id, false);
            if (currency is null)
            {
                return Ok(new Response<CurrencyDto>
                {
                    Data = null,
                    IsError = false,
                    Message = "Success",
                    MessageAr = "تم بنجاح",
                    Status = (int)(StatusCodeEnum.NotFound)
                });
            }
            currency = model.Adapt<Currency>();
            currency.CreatedBy = "admin";
            _currencyRepository.Update(currency);
            await _currencyRepository.SaveAsync(cancellationToken);
            return Ok(new Response<Guid>
            {
                Data = currency.Id,
                IsError = false,
                Message = "",
                Status = (int)(StatusCodeEnum.NotFound)
            });
        }
        [HttpDelete("{Id}")]
        public async Task<IActionResult> DeleteCurrency(Guid Id, CancellationToken cancellationToken)
        {
            var currency = await _currencyRepository.Find(c => c.Id != Id, false);
            if (currency is null)
            {
                return Ok(new Response<CurrencyDto>
                {
                    Data = null,
                    IsError = true,
                    Message = $"Currency with Id: {Id} doesn't exist in the database",
                    Status = (int)StatusCodeEnum.NotFound
                });
            }
            _currencyRepository.Delete(currency);
            await _currencyRepository.SaveAsync(cancellationToken);
            return Ok(new Response<Guid>
            {
                Data = currency.Id,
                IsError = false,
                Message = " Deleted Successfully",
                MessageAr = "تم الحذف بنجاح",
                Status = (int)StatusCodeEnum.Ok
            });
        }

        [HttpGet("UpdateCurrencies")]
        public async Task<IActionResult> UpdateCurrencies()
        {
            var GetValue = await CurrencyService.UpdateCurrecies();

            await _currencyRepository.UpdateNewCurrencies(GetValue.Rates);
            return Ok();
        }

    }
}
