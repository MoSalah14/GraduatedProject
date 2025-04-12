using OutbornE_commerce.DAL.Models;
using System;
using OutbornE_commerce.BAL.Repositories.BaseRepositories;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OutbornE_commerce.DAL.Data;
using Microsoft.EntityFrameworkCore;
using OutbornE_commerce.BAL.Dto.Profile;
using Microsoft.AspNetCore.Mvc;
using OutbornE_commerce.BAL.Dto.OrderDto;
using OutbornE_commerce.BAL.Dto;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Identity;

namespace OutbornE_commerce.BAL.Repositories.UserRepo
{
    public class UserReposatry : BaseRepository<User>, IUserReposatry
    {
        public UserManager<User> UserManager { get; }

        public UserReposatry(ApplicationDbContext context, UserManager<User> userManager) : base(context)
        {
            UserManager = userManager;
        }

        //public async Task<List<string>> GetUsersEmailAsync()
        //{
        //    return  await _context.Users.Select(x=>x.Email).ToListAsync();
        //}
        public async Task<PagainationModel<List<StaffUserDto>>> GetPaginatedStaffUsersAsync(string? searchTerm, int pageNumber, int pageSize)
        {
            var users = await UserManager.GetUsersInRoleAsync("Stuff");

            if (!string.IsNullOrEmpty(searchTerm))
            {
                users = users.Where(user =>
                    (user.UserName != null && user.UserName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (user.Email != null && user.Email.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (user.PhoneNumber != null && user.PhoneNumber.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                ).ToList();
            }

            var totalCount = users.Count;

            var paginatedUsers = users
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(user => new StaffUserDto
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber
                })
                .ToList();

            return new PagainationModel<List<StaffUserDto>>
            {
                Data = paginatedUsers,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<PagainationModel<IEnumerable<UserDisplayDto>>> GetUsersForDisplayAsync(string? IteemSearch, int pageNumber, int pageSize)
        {
            var userRoleUsers = await UserManager.GetUsersInRoleAsync("User");
            var userIds = userRoleUsers.Select(u => u.Id).ToList();

            var query = _context.Users
                .Include(u => u.Wallet)
                .Where(u => userIds.Contains(u.Id));

            if (!string.IsNullOrEmpty(IteemSearch))
            {
                query = query.Where(u =>
                    (u.FullName != null && u.FullName.Contains(IteemSearch)) ||
                    (u.Email != null && u.Email.Contains(IteemSearch)) ||
                    (u.PhoneNumber != null && u.PhoneNumber.Contains(IteemSearch)));
            }

            var totalCount = await query.CountAsync();

            var paginatedUsers = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var userDtos = paginatedUsers.Select(u => new UserDisplayDto
            {
                UserID = u.Id,
                FullName = u.FullName,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                WalletBalance = u.Wallet != null ? u.Wallet.Balance : 0
            });

            return new PagainationModel<IEnumerable<UserDisplayDto>>
            {
                Data = userDtos,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<string> GetSpecificUserEmailAsync(string userId)
        {
            return await _context.Users
                .Where(x => x.Id == userId)
                .Select(x => x.Email)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> GetSpecificUserAsync(string userId, Guid CurrencyID, CancellationToken cancellationToken)
        {
            var User = await Find(e => e.Id == userId, true);

            if (User is null)
                return false;

            try
            {
                await SaveAsync(cancellationToken);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}