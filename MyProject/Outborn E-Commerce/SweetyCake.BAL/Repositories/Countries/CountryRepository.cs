using OutbornE_commerce.BAL.Repositories.BaseRepositories;
using OutbornE_commerce.DAL.Data;
using OutbornE_commerce.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Repositories.Countries
{
    public class CountryRepository : BaseRepository<Country>, ICountryRepository
    {
        public CountryRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<bool> IsUAECountry(Guid countryId)
        {
            var isCountry = await Find(e => e.Id == countryId);

            if (isCountry == null || isCountry.Id.ToString() != "b393e0e8-ec9c-467a-b894-9923b3743580")
                return false;
            else return true;
        }
    }
}