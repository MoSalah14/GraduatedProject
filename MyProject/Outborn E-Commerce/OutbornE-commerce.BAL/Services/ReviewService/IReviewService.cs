using OutbornE_commerce.BAL.Dto;
using OutbornE_commerce.BAL.Dto.Products;
using OutbornE_commerce.BAL.Dto.Review;
using OutbornE_commerce.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Services.ReviewService
{
    public interface IReviewService
    {
        Task<Response<Reviews>> AddReviewAsync(UserReviewDto userReviewDto, string userID, CancellationToken cancellationToken);

        //Task<Response<IEnumerable<ProductReviewCount>>> GetAllProductReviewAsync(int pageNumber, int PageSize, string Search);
    }
}
