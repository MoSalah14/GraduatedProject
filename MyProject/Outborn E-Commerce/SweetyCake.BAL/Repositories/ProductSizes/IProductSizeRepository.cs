using OutbornE_commerce.BAL.Repositories.BaseRepositories;
using OutbornE_commerce.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Repositories.ProductSizes
{
    public interface IProductSizeRepository : IBaseRepository<ProductSize>
    {
        //Task ApplyDiscountToProductSizes(Guid categoryId);
        Task<bool> ApplyDiscountToCategory(Guid discountCategoryId);
        Task UpdateProductQuantities(Order order, CancellationToken cancellationToken);
        Task<bool> ApplyDiscountToProductSize(Guid productSizeDiscountId);
        Task<bool> ResetDiscountsByCategoryId(Guid discountCategoryId);
    }
}
