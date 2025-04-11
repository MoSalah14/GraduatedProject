using OutbornE_commerce.BAL.Repositories.BaseRepositories;
using OutbornE_commerce.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Repositories.DiscountRepository
{
    public interface IDiscountRepository  :IBaseRepository<Discount>
    {
      //IQueryable<Discount> GetDiscountsByDateRange(DateTime? startDate, DateTime? endDate);
    }
}
