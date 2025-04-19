using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OutbornE_commerce.BAL.Dto;
using OutbornE_commerce.BAL.Dto.Colors;
using OutbornE_commerce.BAL.Dto.Products;
using OutbornE_commerce.BAL.Dto.ProductSizes;
using OutbornE_commerce.BAL.Dto.Sizes;
using OutbornE_commerce.BAL.Dto.WishList;
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


        public IQueryable<GetAllProductForUserDto> GetAllProductInHomePage(string searchTerm, int pageNumber, int pageSize, SortingCriteria? sortingCriteria = null)
        {
            // Start the query

            var ProductQuery = _context.Products.Select(p => new GetAllProductForUserDto
            {
                Id = p.Id,
                NameEn = p.NameEn,
                NameAr = p.NameAr,
                MainImageUrl = p.MainImageUrl,
                CreatedOn = p.CreatedOn,
                Price = p.Price,
                DiscountPrice = p.DiscountPrice,
                RatingAverage = (int)(p.Reviews.Average(r => r.Rating) ?? 0),
            });

            if (!string.IsNullOrEmpty(searchTerm))
            {
                var lowerSearchTerm = searchTerm.ToLower();
                ProductQuery = ProductQuery.Where(p =>
                    p.NameEn.ToLower().Contains(lowerSearchTerm) ||
                    p.Price.ToString().Contains(lowerSearchTerm)
                );
            }

            if (sortingCriteria != null)
            {
                ProductQuery = ProductQuery.ApplySorting(sortingCriteria);
            }
            return ProductQuery;
        }

        public async Task<List<ProductNameIdModel>> GetProductNameAndIdByPaginationAsync(string searchTerm, int pageNumber, int pageSize)
        {
            // Base query that selects only Id and NameEn fields
            var query = _context.Products.Include(e => e.Reviews).AsNoTracking()
                .Select(p => new ProductNameIdModel
                {
                    Id = p.Id,
                    ProductName = p.NameEn,
                    Review = p.Reviews
                });

            if (!string.IsNullOrEmpty(searchTerm))
                query = query.Where(p => p.ProductName.Contains(searchTerm));

            int totalCount = await query.CountAsync();

            var ProductsWithReview = await query.Skip((pageNumber - 1) * pageSize)
                                               .Take(pageSize)
                                               .ToListAsync();
            return ProductsWithReview;
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

        public async Task<PaginationResponse<List<GetAllProductForUserDtoًWithCategory>>> GetProductsByCategoryAsync(Guid CategoryId, int pageNumber, int pageSize, SortingCriteria? sortingCriteria = null)
        {
            var query = _context.Products.Include(e => e.Category).Select(p => new GetAllProductForUserDtoًWithCategory
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

            return new PaginationResponse<List<GetAllProductForUserDtoًWithCategory>>
            {
                Data = products,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }




    }
}