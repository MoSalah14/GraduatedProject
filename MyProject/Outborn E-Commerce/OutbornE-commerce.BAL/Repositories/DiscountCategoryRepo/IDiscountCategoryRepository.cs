using OutbornE_commerce.BAL.Dto;
using OutbornE_commerce.BAL.Dto.DiscountCagegoryDto;
using OutbornE_commerce.BAL.Repositories.BaseRepositories;
using OutbornE_commerce.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Repositories.DiscountCategoryRepo
{
    public interface IDiscountCategoryRepository :IBaseRepository<DiscountCategory>

    {
        Task<PaginationResponse<List<GetAllDiscountCategory>>> SearchDiscountsByDate(DateTime? startDate, DateTime? endDate, int pageNumber, int pageSize);        //Task<decimal> GetDiscountByCategoryId(Guid CategoryId);
    }
}
