using Microsoft.EntityFrameworkCore;
using OutbornE_commerce.BAL.Dto;
using OutbornE_commerce.BAL.Dto.ProductSizeDiscountDto;
using OutbornE_commerce.BAL.Repositories.BaseRepositories;
using OutbornE_commerce.BAL.Repositories.ProductSizes;
using OutbornE_commerce.DAL.Data;
using OutbornE_commerce.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Repositories.ProductSizeDiscountRepo
{
    public class productSizeDiscountRepository : BaseRepository<ProductSizeDiscount>, IproductSizeDiscountRepository
    {
        public productSizeDiscountRepository(ApplicationDbContext context) : base(context)
        {
        }
        public async Task<PaginationResponse<List<GetProductSizeDiscountDto>>> GetDiscountsByDateRange(  DateTime? startDate,  DateTime? endDate, int pageNumber,int pageSize)
        {
            var query = _context.ProductSizeDiscounts
                .Include(psd => psd.Discount)
                .Include(psd => psd.ProductSize.ProductColor.Product)
                .AsQueryable();

            if (startDate.HasValue)
            {
                query = query.Where(psd => psd.StartDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(psd => psd.EndDate <= endDate.Value);
            }

            var totalRecords = await query.CountAsync();

            var paginatedData = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = paginatedData.Select(psd => new GetProductSizeDiscountDto
            {
                Id = psd.Id,
                StartDate = psd.StartDate,
                EndDate = psd.EndDate,
                IsActive = psd.IsActive,
                ProductNameArabic = psd.ProductSize.ProductColor.Product.NameAr,
                ProductNameEndlish = psd.ProductSize.ProductColor.Product.NameEn,
                DiscountPercentage = psd.Discount?.Percentage ?? 0,
                DiscountId = psd.DiscountId,
                ProductSizeId = psd.ProductSizeId
            }).ToList();

            return new PaginationResponse<List<GetProductSizeDiscountDto>>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalRecords,
                Data = result
            };
        }

    }
}
