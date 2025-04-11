using OutbornE_commerce.BAL.Dto;
using OutbornE_commerce.BAL.Dto.Profile;
using OutbornE_commerce.BAL.Repositories.BaseRepositories;
using OutbornE_commerce.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Repositories.UserRepo
{
    public interface IUserReposatry : IBaseRepository<User>
    {
        //Task<List<string>> GetUsersEmailAsync();
        Task<string> GetSpecificUserEmailAsync(string userId);

        Task<PagainationModel<List<StaffUserDto>>> GetPaginatedStaffUsersAsync(string? searchTerm, int pageNumber, int pageSize);

        Task<PagainationModel<IEnumerable<UserDisplayDto>>> GetUsersForDisplayAsync(string? IteemSearch, int pageNumber, int pageSize);

        Task<bool> GetSpecificUserAsync(string userId, Guid CurrencyID, CancellationToken cancellationToken);
    }
}