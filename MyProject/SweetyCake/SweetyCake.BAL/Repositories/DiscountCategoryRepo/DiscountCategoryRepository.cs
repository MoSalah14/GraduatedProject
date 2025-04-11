using Microsoft.EntityFrameworkCore;
using OutbornE_commerce.BAL.Dto;
using OutbornE_commerce.BAL.Dto.DiscountCagegoryDto;
using OutbornE_commerce.BAL.Repositories.BaseRepositories;
using OutbornE_commerce.BAL.Repositories.Currencies;
using OutbornE_commerce.DAL.Data;
using OutbornE_commerce.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Repositories.DiscountCategoryRepo
{
    public class DiscountCategoryRepository : BaseRepository<DiscountCategory>, IDiscountCategoryRepository
    {
        public DiscountCategoryRepository(ApplicationDbContext context) : base(context)
        {
        }
        public async Task<PaginationResponse<List<GetAllDiscountCategory>>> SearchDiscountsByDate(DateTime? startDate, DateTime? endDate, int pageNumber, int pageSize)
        {
            var query = _context.DiscountCategories
                .Include(d => d.Category)
                .Include(d => d.Discount)
                .AsQueryable();

            if (startDate.HasValue)
            {
                query = query.Where(d => d.StartDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(d => d.EndDate <= endDate.Value);
            }

            var totalRecords = await query.CountAsync();

            var paginatedData = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Map the data to DTO
            var result = paginatedData.Select(item => new GetAllDiscountCategory
            {
                StartDate = item.StartDate,
                EndDate = item.EndDate,
                DiscountPercentage = item.Discount.Percentage,
                Id = item.Id,
                IsActive = item.IsActive,
                CategoryNameArabic = item.Category.NameAr,
                CategoryNameEndlish = item.Category.NameEn,
                DiscountId = item.Discount.Id,
                CategoryId = item.Category.Id
            }).ToList();

            return new PaginationResponse<List<GetAllDiscountCategory>>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalRecords,
                Data = result
            };
        }

        //public async Task<decimal> GetDiscountByCategoryId(Guid categoryId)
        //{
        //    if (categoryId == Guid.Empty) 
        //        return 0;

        //    // Query the database for active discounts related to the given category
        //    var discount = await _context.Categories
        //        .Where(c => c.Id == categoryId) // Filter by CategoryId
        //        .SelectMany(c => c.DiscountCategories) // Access related DiscountCategories
        //        .Where(dc => dc.IsActive) // Active discounts
        //        .Select(dc => dc.Discount.Percentage) // Get the percentage
        //        .FirstOrDefaultAsync(); // Get the first active discount or return 0

        //    return discount; // Return the discount percentage
    }

}

