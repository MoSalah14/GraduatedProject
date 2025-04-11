using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OutbornE_commerce.BAL.AuthServices;
using OutbornE_commerce.BAL.Dto.Review;
using OutbornE_commerce.BAL.Repositories.Products;
using OutbornE_commerce.BAL.Repositories.UserReviews;
using OutbornE_commerce.BAL.Services.ReviewService;
using OutbornE_commerce.DAL.Data;
using OutbornE_commerce.DAL.Models;
using OutbornE_commerce.Extensions;

namespace OutbornE_commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService reviewService;

        public ReviewsController(IReviewService _ReviewService)
        {
            reviewService = _ReviewService;
        }

        //[HttpGet("GetReportReview")]
        //public async Task<IActionResult> GetAllReviewsCount(string Search = null, int pageNumber = 1, int pageSize = 10)
        //{
        //    var Respons = await reviewService.GetAllProductReviewAsync(pageNumber, pageSize, Search);
        //    if (Respons.IsError)
        //        return StatusCode(Respons.Status, Respons);

        //    return Ok(Respons);
        //}

        //[HttpGet("GetProductReview")]
        //public async Task<IActionResult> GetAllReviewsCount(Guid ProductId, int pageNumber = 1, int pageSize = 10)
        //{
        //    var Respons = await reviewService.GetAllProductReviewAsync(pageNumber, pageSize);
        //    if (Respons.IsError)
        //        return StatusCode(Respons.Status, Respons);

        //    return Ok(Respons);
        //}

        [HttpPost("AddReview")]
        [Authorize]
        public async Task<IActionResult> AddUserReviews(UserReviewDto userReviewDto, CancellationToken cancellationToken)
        {
            // Check user ID from token
            var userId = User.GetUserIdFromToken();

            var response = await reviewService.AddReviewAsync(userReviewDto, userId ?? string.Empty, cancellationToken);

            if (response.IsError)
                return StatusCode(response.Status, response);

            return Ok(new Response<Guid>
            {
                Data = userReviewDto.ProductId,
                IsError = false,
                Message = "Created successfully",
                MessageAr = "تم الاضافه بنجاح",
                Status = (int)StatusCodeEnum.Ok
            });
        }
    }
}