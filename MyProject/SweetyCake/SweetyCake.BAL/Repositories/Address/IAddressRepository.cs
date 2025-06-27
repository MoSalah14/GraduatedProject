using OutbornE_commerce.BAL.Dto.Address;
using OutbornE_commerce.BAL.Repositories.BaseRepositories;
using OutbornE_commerce.DAL.Models;

namespace OutbornE_commerce.BAL.Repositories
{
    public interface IAddressRepository : IBaseRepository<Address>
    {
        Task<Address> CreateAddress(Address addressForCreation, string userId, CancellationToken cancellationToken);
    }
}
