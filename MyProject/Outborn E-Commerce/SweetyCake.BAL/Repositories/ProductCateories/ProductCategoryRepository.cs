using OutbornE_commerce.BAL.Dto;
using OutbornE_commerce.BAL.Dto.Products;
using OutbornE_commerce.BAL.Repositories.BaseRepositories;
using OutbornE_commerce.DAL.Data;
using OutbornE_commerce.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Repositories.ProductCateories
{
    public class ProductCategoryRepository : BaseRepository<ProductCategory>, IProductCategoryRepository
    {
        public ProductCategoryRepository(ApplicationDbContext context) : base(context)
        {
        }

    //    public async Task<PaginationResponse<List<GetAllProductDto>>> GetProductsByCategoryAsync(Guid categoryId, int pageNumber, int pageSize)
    //    {
    //        string[] includes = { "Category", "Product.Brand", "Product.ProductColors", "Product.ProductColors.ProductColorImages", "Product.ProductColors.ProductSizes" };

    //        var productCategories = await FindAllAsyncByPagination(pc => pc.CategoryId == categoryId, pageNumber, pageSize, includes);

    //        //int totalCount = productCategories.Count();

    //        //var pagedProductCategories = productCategories
    //        //    .Skip((pageNumber - 1) * pageSize)
    //        //    .Take(pageSize)
    //        //    .ToList();

    //        var productDtos = productCategories.Data
    //            .Select(pc => new GetAllProductDto
    //            {
    //                Id = pc.Product.Id,
    //                NameEn = pc.Product.NameEn,
    //                NameAr = pc.Product.NameAr,
    //                ImageUrl = pc.Product.ProductColors.SelectMany(e => e.ProductColorImages).FirstOrDefault(e => e.IsDefault == true)?.ImageUrl,
    //                BrandNameEn = pc.Product.Brand?.NameEn,
    //                BrandNameAr = pc.Product.Brand?.NameAr,
    //                Label = pc.Product.Label,
    //                SuperCategoryNameEn = pc.Category.NameEn,
    //                SuperCategoryNameAr = pc.Category.NameAr,

    //                CategoryNameEn = pc.Product.ProductCategories
    //                                .Select(c => c.Category!.NameEn).ToList(),
    //                CategoryNameAr = pc.Product.ProductCategories
    //                               .Select(c => c.Category!.NameAr).ToList(),
    //                Price = pc.Product.ProductColors
    //                    .SelectMany(pc => pc.ProductSizes)
    //                    .Select(ps => ps.Price)
    //                    .DefaultIfEmpty(0)
    //                    .Min()
    //            })
    //            .ToList();

    //        return new PaginationResponse<List<GetAllProductDto>>
    //        {
    //            TotalCount = productDtos.Count,
    //            PageNumber = pageNumber,
    //            PageSize = pageSize,
    //            IsError = false,
    //            Status = 200,
    //            Data = productDtos
    //        };
    //    }
   }
}