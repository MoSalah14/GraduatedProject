﻿using OutbornE_commerce.BAL.Dto.Cart;
using OutbornE_commerce.BAL.Repositories.BaseRepositories;

namespace OutbornE_commerce.BAL.Repositories
{
    public interface IBagItemsRepo : IBaseRepository<DAL.Models.BagItem>
    {
        Task<DisplayCartItemDto> GetUserCartAsync(string userId);
        Task ClearCartAsync(string userId, CancellationToken cancellationToken);
    }
}
