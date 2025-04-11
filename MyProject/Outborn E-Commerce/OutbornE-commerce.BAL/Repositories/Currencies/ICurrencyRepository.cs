using OutbornE_commerce.BAL.Repositories.BaseRepositories;
using OutbornE_commerce.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Repositories.Currencies
{
    public interface ICurrencyRepository : IBaseRepository<Currency>
    {
        Task UpdateNewCurrencies(Dictionary<string, decimal> NewCurrencies);

        Task<decimal> GetTargetValue(string CurrencySign);

        Task<decimal> GetAEDValue();
    }
}