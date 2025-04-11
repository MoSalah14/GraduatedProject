using OutbornE_commerce.BAL.Dto;
using OutbornE_commerce.BAL.Dto.ProductSizeDiscountDto;
using OutbornE_commerce.BAL.Repositories.BaseRepositories;
using OutbornE_commerce.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Repositories.ProductSizeDiscountRepo
{
    public interface IproductSizeDiscountRepository : IBaseRepository<ProductSizeDiscount>
    {
        Task<PaginationResponse<List<GetProductSizeDiscountDto>>> GetDiscountsByDateRange(DateTime? startDate, DateTime? endDate, int pageNumber, int pageSize);
    }
}
