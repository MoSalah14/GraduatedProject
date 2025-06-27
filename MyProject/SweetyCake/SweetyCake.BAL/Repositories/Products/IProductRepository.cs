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

        Task<PaginationResponse<List<GetAllProductForUserDtoWithCategory>>> GetProductsByCategoryAsync(Guid CategoryId, int pageNumber, int pageSize, string? userId, SortingCriteria? sortingCriteria);


        IQueryable<GetAllProductForUserDtoWithCategory> GetAllProductInHomePage(string? searchTerm, int pageNumber, int pageSize, string? userId, SortingCriteria? sortingCriteria = null, Guid? CategoryId = null);
        Task<List<GetAllProductForUserDto>> GetFlashSaleProductsAsync(string? userId, int flashsaleNumber);
        Task<List<GetAllProductForUserDto>> GetNewArrivaleProductsAsync(string? userId);

    }
}