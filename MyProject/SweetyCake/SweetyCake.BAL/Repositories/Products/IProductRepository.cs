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

        Task<PaginationResponse<List<GetAllProductForUserDtoًWithCategory>>> GetProductsByCategoryAsync(Guid CategoryId, int pageNumber, int pageSize, SortingCriteria? sortingCriteria);

        Task<PagainationModel<List<Product>>> SearchProducts(SearchModelDto model, SortingCriteria? sortingCriteria = null);

        Task<List<ProductNameIdModel>> GetProductNameAndIdByPaginationAsync(string searchTerm, int pageNumber, int pageSize);

        IQueryable<GetAllProductForUserDto> GetAllProductInHomePage(string searchTerm, int pageNumber, int pageSize, SortingCriteria? sortingCriteria = null);
        Task<List<GetAllProductForUserDto>> GetFlashSaleProductsAsync(int flashsaleNumber);
        Task<List<GetAllProductForUserDto>> GetNewArrivaleProductsAsync();

    }
}