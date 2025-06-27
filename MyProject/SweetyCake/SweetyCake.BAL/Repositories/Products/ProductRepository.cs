using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OutbornE_commerce.BAL.Dto;
using OutbornE_commerce.BAL.Dto.Products;
using OutbornE_commerce.BAL.Extentions;
using OutbornE_commerce.BAL.Repositories.BaseRepositories;
using OutbornE_commerce.DAL.Data;
using OutbornE_commerce.DAL.Enums;
using OutbornE_commerce.DAL.Models;

namespace OutbornE_commerce.BAL.Repositories.Products
{
    public class ProductRepository : BaseRepository<Product>, IProductRepository
    {
        public ProductRepository(ApplicationDbContext context) : base(context)
        {
        }


        public IQueryable<GetAllProductForUserDtoWithCategory> GetAllProductInHomePage(string? searchTerm, int pageNumber, int pageSize, string? userId, SortingCriteria? sortingCriteria = null, Guid? CategoryId = null)
        {
            // Start the query

            var ProductQuery = _context.Products.AsQueryable();

            // Apply filtering before projection
            if (!string.IsNullOrEmpty(searchTerm))
            {
                var lowerSearchTerm = searchTerm.ToLower();
                ProductQuery = ProductQuery.Where(p =>
                    p.NameEn.ToLower().Contains(lowerSearchTerm) ||
                    p.NameAr.ToLower().Contains(lowerSearchTerm) ||
                    p.Price.ToString().Contains(lowerSearchTerm)
                );
            }

            if (CategoryId.HasValue)
            {
                ProductQuery = ProductQuery.Where(p => p.CategoryId == CategoryId.Value);
            }

            if (sortingCriteria != null)
            {
                ProductQuery = ProductQuery.ApplySorting(sortingCriteria);
            }

            var result = ProductQuery.Include(e => e.Category).Include(e => e.WishLists).Select(p => new GetAllProductForUserDtoWithCategory
            {
                Id = p.Id,
                NameEn = p.NameEn,
                NameAr = p.NameAr,
                MainImageUrl = p.MainImageUrl,
                CreatedOn = p.CreatedOn,
                Price = p.Price,
                DiscountPrice = p.DiscountPrice,
                RatingAverage = (int)(p.Reviews.Average(r => r.Rating) ?? 0),
                CategoryID = p.CategoryId,
                CategoryNameEn = p.Category.NameEn,
                CategoryNameAr = p.Category.NameAr,
                QuantityInStock = p.QuantityInStock,
                IsLiked = userId != null && p.WishLists.Any(w => w.UserId == userId)
            });


            return result;
        }

        
        public async Task<PaginationResponse<List<GetAllProductForUserDtoWithCategory>>> GetProductsByCategoryAsync(Guid CategoryId, int pageNumber, int pageSize, string? userId, SortingCriteria? sortingCriteria = null)
        {
            var query = _context.Products
                .Where(e => e.CategoryId == CategoryId).Include(e => e.Category).Select(p => new GetAllProductForUserDtoWithCategory
                {
                    Id = p.Id,
                    NameEn = p.NameEn,
                    MainImageUrl = p.MainImageUrl,
                    Price = p.Price,
                    CategoryID = p.CategoryId,
                    CategoryNameEn = p.Category.NameEn,
                    CategoryNameAr = p.Category.NameAr,
                    IsLiked = userId != null && p.WishLists.Any(w => w.UserId == userId)
                });

            if (sortingCriteria is not null)
            {
                query = query.ApplySorting(sortingCriteria);
            }

            var totalCount = await query.CountAsync();
            var products = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginationResponse<List<GetAllProductForUserDtoWithCategory>>
            {
                Data = products,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<List<GetAllProductForUserDto>> GetFlashSaleProductsAsync(string? userId, int flashsaleNumber)
        {
            var productQuery = await _context.Products
                .Where(e => e.DiscountPrice > 0)
                .OrderByDescending(e => e.DiscountPrice)
                .Take(flashsaleNumber)
                .Select(p => new GetAllProductForUserDto
                {
                    Id = p.Id,
                    NameEn = p.NameEn,
                    NameAr = p.NameAr,
                    MainImageUrl = p.MainImageUrl,
                    CreatedOn = p.CreatedOn,
                    Price = p.Price,
                    DiscountPrice = p.DiscountPrice,
                    RatingAverage = (int)(p.Reviews.Average(r => r.Rating) ?? 0),
                    QuantityInStock = p.QuantityInStock,
                    IsLiked = userId != null && p.WishLists.Any(w => w.UserId == userId)
                })
                .ToListAsync();

            return productQuery;
        }

        public async Task<List<GetAllProductForUserDto>> GetNewArrivaleProductsAsync(string? userId)
        {
            var productQuery = await _context.Products
                .OrderByDescending(e => e.CreatedOn)
                .Take(4)
                .Select(p => new GetAllProductForUserDto
                {
                    Id = p.Id,
                    NameEn = p.NameEn,
                    NameAr = p.NameAr,
                    MainImageUrl = p.MainImageUrl,
                    CreatedOn = p.CreatedOn,
                    Price = p.Price,
                    DiscountPrice = p.DiscountPrice,
                    RatingAverage = (int)(p.Reviews.Average(r => r.Rating) ?? 0),
                    IsLiked = userId != null && p.WishLists.Any(w => w.UserId == userId),
                })
                .ToListAsync();

            return productQuery;
        }

    }
}