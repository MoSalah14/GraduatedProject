using OutbornE_commerce.BAL.Repositories.BaseRepositories;
using OutbornE_commerce.DAL.Data;
using OutbornE_commerce.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Repositories.DiscountRepository
{
    public class DiscountRepository : BaseRepository<Discount>, IDiscountRepository
    {
        public DiscountRepository(ApplicationDbContext context) : base(context)
        {
        }
        //public IQueryable<Discount> GetDiscountsByDateRange(DateTime? startDate, DateTime? endDate)
        //{
        //    var query =  _context.Discounts.AsQueryable();

        //    if (startDate.HasValue)
        //    {
        //        query = query.Where(d => d.StartDate >= startDate.Value);
        //    }

        //    if (endDate.HasValue)
        //    {
        //        query = query.Where(d => d.EndDate <= endDate.Value);
        //    }

        //    return query;
        //}

    }
}
