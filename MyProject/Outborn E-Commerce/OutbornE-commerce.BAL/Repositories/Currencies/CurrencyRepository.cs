using Microsoft.Extensions.Configuration;
using OutbornE_commerce.BAL.Repositories.BaseRepositories;
using OutbornE_commerce.DAL.Data;
using OutbornE_commerce.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Repositories.Currencies
{
    public class CurrencyRepository : BaseRepository<Currency>, ICurrencyRepository
    {
        private readonly IConfiguration configuration;

        public CurrencyRepository(ApplicationDbContext context, IConfiguration _configuration) : base(context)
        {
            configuration = _configuration;
        }

        public async Task<decimal> GetAEDValue()
        {
            var AEDValue = await Find(e => e.Sign == "AED");
            if (AEDValue is null)
                throw new Exception();

            return AEDValue.Price;
        }

        public async Task<decimal> GetTargetValue(string CurrencySign)
        {
            var CurrencySignValue = await Find(e => e.Sign == CurrencySign);
            if (CurrencySignValue is null)
                throw new Exception();

            return CurrencySignValue.Price;
        }

        public async Task UpdateNewCurrencies(Dictionary<string, decimal> NewCurrencies)
        {
            var requiredCurrencies = configuration.GetSection("Currencies:RequiredCurrencies").Get<List<string>>();

            foreach (var code in requiredCurrencies)
            {
                if (NewCurrencies.ContainsKey(code))
                {
                    decimal newRate = NewCurrencies[code];

                    var GetOldCurrency = await Find(e => e.NameEn == code, true);

                    if (GetOldCurrency != null)
                    {
                        GetOldCurrency.Price = newRate;
                        GetOldCurrency.UpdatedOn = DateTime.Now;
                    }
                    else
                    {
                        var AddNewCuurency = new Currency
                        {
                            Id = Guid.NewGuid(),
                            NameEn = code,
                            NameAr = code,
                            Price = newRate,
                            CreatedBy = "Admin",
                            IsDeafult = code == "AED" ? true : false,
                            CreatedOn = DateTime.Now,
                            Sign = code,
                            IsDeleted = false,
                        };
                        await _context.AddAsync(AddNewCuurency);
                    }
                }
            }
            await _context.SaveChangesAsync();
        }
    }
}