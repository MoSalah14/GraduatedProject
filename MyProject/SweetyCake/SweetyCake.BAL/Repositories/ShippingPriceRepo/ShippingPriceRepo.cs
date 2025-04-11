using Microsoft.EntityFrameworkCore;
using OutbornE_commerce.BAL.Repositories.BaseRepositories;
using OutbornE_commerce.DAL.Data;
using OutbornE_commerce.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Repositories.ShippingPriceRepo
{
    public class ShippingPriceRepo : BaseRepository<ShippingPrice>, IShippingPriceRepo
    {
        public ShippingPriceRepo(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<decimal> GetShippingPriceBasedOnWeightAndCountryId(Guid? CountryId, double OrderWeight)
        {
            var GetShppingPrice = await _context.ShippingPrices.Where(e => e.CountryId == CountryId && e.Weight == OrderWeight)
                .Select(e => e.Price).FirstOrDefaultAsync();
            if (GetShppingPrice == 0)
            {
                throw new KeyNotFoundException("No shipping price found for Country");
            }

            return GetShppingPrice;
        }
    }
}