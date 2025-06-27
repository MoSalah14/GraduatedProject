using Mapster;
using OutbornE_commerce.BAL.Dto.Address;
using OutbornE_commerce.BAL.Repositories.BaseRepositories;
using OutbornE_commerce.DAL.Data;
using OutbornE_commerce.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Repositories
{
    public class AddressRepository : BaseRepository<Address>, IAddressRepository
    {
        public AddressRepository(ApplicationDbContext context) : base(context)
        {
        }


        public async Task<Address> CreateAddress(Address addressForCreation, string userId, CancellationToken cancellationToken)
        {
            var result = await Create(addressForCreation);

            await SaveAsync(cancellationToken);
            return result;
        }
    }
}