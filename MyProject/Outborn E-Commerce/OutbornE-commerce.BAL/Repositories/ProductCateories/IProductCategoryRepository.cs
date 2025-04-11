using OutbornE_commerce.BAL.Dto;
using OutbornE_commerce.BAL.Dto.Products;
using OutbornE_commerce.BAL.Repositories.BaseRepositories;
using OutbornE_commerce.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Repositories.ProductCateories
{
    public interface IProductCategoryRepository : IBaseRepository<ProductCategory>
    {
        //Task<PaginationResponse<List<GetAllProductDto>>> GetProductsByCategoryAsync(Guid categoryId, int pageNumber, int pageSize);
    }
}
