using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using OutbornE_commerce.BAL.Dto;
using OutbornE_commerce.BAL.Dto.Review;
using OutbornE_commerce.BAL.Repositories.Products;
using OutbornE_commerce.BAL.Repositories.UserReviews;
using OutbornE_commerce.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Services.ReviewService
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewsRepository reviewsRepository;
        private readonly IProductRepository productRepository;
        private readonly UserManager<User> userManager;
        private readonly ILogger<ReviewService> _logger;

        public ReviewService(IReviewsRepository _reviewsRepository, IProductRepository _productRepository, UserManager<User> _userManager, ILogger<ReviewService> logger)
        {
            reviewsRepository = _reviewsRepository;
            productRepository = _productRepository;
            userManager = _userManager;
            _logger = logger;
        }

        public async Task<Response<Reviews>> AddReviewAsync(UserReviewDto userReviewDto, string userID, CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrEmpty(userID))
                {
                    return new Response<Reviews>
                    {
                        IsError = true,
                        Message = "Please login first before adding a comment",
                        MessageAr = "الرجاء تسجيل الدخول قبل إضافة تعليق",
                        Status = 401
                    };
                }

                // Check if the product exists
                var product = await productRepository.Find(p => p.Id == userReviewDto.ProductId);
                if (product == null)
                {
                    return new Response<Reviews>
                    {
                        IsError = true,
                        Message = "Invalid Product, Please Try Again Later",
                        MessageAr = "المنتج غير صالح، حاول مرة أخرى",
                        Status = 400
                    };
                }

                // Validate the user
                var validateUser = await userManager.FindByIdAsync(userID);
                if (validateUser == null)
                {
                    return new Response<Reviews>
                    {
                        IsError = true,
                        Message = "User not found, please register first",
                        MessageAr = "المستخدم غير موجود، الرجاء التسجيل أولاً",
                        Status = 404
                    };
                }

                // Map DTO to entity and save the review
                var userReview = userReviewDto.Adapt<Reviews>();
                userReview.UserId = userID;
                var createdReview = await reviewsRepository.Create(userReview);
                await reviewsRepository.SaveAsync(cancellationToken);

                return new Response<Reviews>
                {
                    IsError = false,
                    Message = "Added Successfully",
                    MessageAr = "تمت الإضافة بنجاح",
                    Status = 200
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding user review");

                return new Response<Reviews>
                {
                    IsError = true,
                    Message = ex.Message,
                    MessageAr = "حدث خطأ أثناء الإضافة",
                    Status = 500
                };
            }
        }

        //public async Task<Response<IEnumerable<ProductReviewCount>>> GetAllProductReviewAsync(int pageNumber ,int PageSize, string Search = null)
        //{
        //    var product = await productRepository.GetProductNameAndIdByPaginationAsync(Search, pageNumber, PageSize);

        //    var result = product.Select(p => new ProductReviewCount
        //    {
        //        ProductId = p.Id,
        //        ProductName = p.ProductName,
        //        ReviewsCount = p.Review.Count,
        //        // Calculate the average rating in memory
        //        RatingAverage = (int)Math.Round(p.Review.Where(r => r.Rating.HasValue && r.Rating > 0)
        //                                      .Select(r => r.Rating.Value)
        //                                      .DefaultIfEmpty(0)
        //                                      .Average())
        //    }).ToList();


        //    return new Response<IEnumerable<ProductReviewCount>>
        //    {
        //        Data = result,
        //        IsError = false,
        //        Message = "Success",
        //        MessageAr = "تم بنجاح",
        //        Status = 200
        //    };

        //}

    }
}
