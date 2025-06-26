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


        public IQueryable<GetAllProductForUserDtoWithCategory> GetAllProductInHomePage(string? searchTerm, int pageNumber, int pageSize, SortingCriteria? sortingCriteria = null, Guid? CategoryId = null)
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

            var result = ProductQuery.Include(e => e.Category).Select(p => new GetAllProductForUserDtoWithCategory
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
            });


            return result;
        }

        public async Task<PagainationModel<List<Product>>> SearchProducts(SearchModelDto model, SortingCriteria? sortingCriteria = null)
        {
            int totalCount = 0;
            var products = _context.Products
                                   .AsNoTracking()
                                   .Include(b => b.Category)
                                   .SearchByTerm(model.SearchTerm)
                                   .SearchByPrice(model.MinPrice, model.MaxPrice);

            if (sortingCriteria is not null)
            {
                products = products.ApplySorting(sortingCriteria);
            }
            totalCount = await products.CountAsync();
            var data = await products.Skip((model.PageNumber - 1) * model.PageSize).Take(model.PageSize).ToListAsync();

            return new PagainationModel<List<Product>>()
            {
                TotalCount = totalCount,
                Data = data,
                PageNumber = model.PageNumber,
                PageSize = model.PageSize,
            };
        }

        public async Task<PaginationResponse<List<GetAllProductForUserDtoWithCategory>>> GetProductsByCategoryAsync(Guid CategoryId, int pageNumber, int pageSize, SortingCriteria? sortingCriteria = null)
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
                    CategoryNameAr = p.Category.NameAr
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

        public async Task<List<GetAllProductForUserDto>> GetFlashSaleProductsAsync(int flashsaleNumber)
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
                    QuantityInStock = p.QuantityInStock
                })
                .ToListAsync();

            return productQuery;
        }

        public async Task<List<GetAllProductForUserDto>> GetNewArrivaleProductsAsync()
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
                })
                .ToListAsync();

            return productQuery;
        }

    }
}