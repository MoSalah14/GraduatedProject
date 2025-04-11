using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using OutbornE_commerce.BAL.Dto;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Services
{
    public class CurrencyService : ICurrencyService
    {

        private readonly string? App_ID;
        private readonly string? CurrenciesUpdateUrl;
        private readonly IConfiguration configuration;
        private readonly HttpClient httpClient;

        public CurrencyService(IConfiguration _configuration, HttpClient _httpClient)
        {
            configuration = _configuration;
            httpClient = _httpClient;
            App_ID = configuration["Currencies:AppId"];
            CurrenciesUpdateUrl = configuration["Currencies:CurerencyUrl"];
        }

        public async Task<UpdateCurrenciesResponse> UpdateCurrecies()
        {
            var GetUpdatedCurrency = await httpClient.GetAsync($"{CurrenciesUpdateUrl}?app_id={App_ID}");
            GetUpdatedCurrency.EnsureSuccessStatusCode();

            var UpdatedCurrencyJson = await GetUpdatedCurrency.Content.ReadAsStringAsync();


            return JsonConvert.DeserializeObject<UpdateCurrenciesResponse>(UpdatedCurrencyJson);

        }
    }
}
