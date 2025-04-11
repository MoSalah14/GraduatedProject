using OutbornE_commerce.BAL.Dto.Products;
using OutbornE_commerce.BAL.Dto;
using OutbornE_commerce.BAL.Repositories.BaseRepositories;
using OutbornE_commerce.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OutbornE_commerce.BAL.Dto.Review;
using OutbornE_commerce.DAL.Enums;
using Microsoft.AspNetCore.Mvc;
using OutbornE_commerce.BAL.Dto.WishList;
using OutbornE_commerce.BAL.Extentions;

namespace OutbornE_commerce.BAL.Repositories.Products
{
    public interface IProductRepository : IBaseRepository<Product>
    {
        Task<List<GetAllProductDto>> GetProductsByTypeIdAsync(Guid typeId, int pageNumber, int pageSize, SortingCriteria? sortingCriteria);

        Task<PaginationResponse<List<GetAllProductForUserDtoًWithCategory>>> GetProductsByCategoryAsync(Guid CategoryId, int pageNumber, int pageSize, SortingCriteria? sortingCriteria);

        Task ApplyDiscountToProductSizes(Guid productId, CancellationToken cancellationToken);

        Task<List<WishListReportDto>> GetWishListProductAsync(CancellationToken cancellationToken);

        Task<PagainationModel<List<Product>>> SearchProducts(SearchModelDto model, SortingCriteria? sortingCriteria = null);

        Task<List<ProductNameIdModel>> GetProductNameAndIdByPaginationAsync(string searchTerm, int pageNumber, int pageSize);

        Task<PaginationResponse<List<GetAllProductDto>>> GetProductsByBrandIdAsync(Guid BrandId, int pageNumber, int pageSize, SortingCriteria? sortingCriteria = null);

        Task<PaginationResponse<List<GetAllProductDto>>> GetProductsBySubCategoryAsync(Guid subCategoryId, int pageNumber, int pageSize, SortingCriteria? sortingCriteria = null);

        IQueryable<GetAllProductForUserDto> GetAllBestSellerProduct(string searchTerm, int pageNumber, int pageSize);

        IQueryable<GetAllProductForUserDto> GetAllProductInHomePage(string searchTerm, int pageNumber, int pageSize, SortingCriteria? sortingCriteria = null);

        Task<WishListReportDto> GetWishListByProduct(Guid productId);

        //Task<List<ProductReport>> GetProductsByCategory(Guid categoryId);

        Task<List<SalesReportDto>> GetSalesReportAsync(SalesReportSearch searchCriteria);
    }
}