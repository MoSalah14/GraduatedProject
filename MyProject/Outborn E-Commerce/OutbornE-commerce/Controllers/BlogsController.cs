using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Core.Types;
using OutbornE_commerce.BAL.Dto.BlogCategory;
using OutbornE_commerce.BAL.Dto.Blogs;
using OutbornE_commerce.BAL.Repositories.BaseRepositories;
using OutbornE_commerce.DAL.Data;
using OutbornE_commerce.DAL.Models;
using OutbornE_commerce.FilesManager;

namespace OutbornE_commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogsController : ControllerBase
    {
        private readonly IBaseRepository<Blogs> repository;
        private readonly IFilesManager _filesManager;

        public BlogsController(IBaseRepository<Blogs> _repository, IFilesManager filesManager)
        {
            repository = _repository;
            _filesManager = filesManager;
        }

        // GET: api/Blogs
        [HttpGet]
        [HttpGet]
        [HttpGet]
        public async Task<IActionResult> GetAllBlogs(int pageNumber = 1, int pageSize = 10, string? searchTerm = null)
        {
            var items = new PagainationModel<IEnumerable<Blogs>>();

            if (string.IsNullOrEmpty(searchTerm))
            {
                items = await repository.FindAllAsyncByPagination(null, pageNumber, pageSize);
            }
            else
            {
                items = await repository.FindAllAsyncByPagination(b =>
                    b.TitleEn.Contains(searchTerm) ||
                    b.TitleAr.Contains(searchTerm) ||
                    b.ContentEn.Contains(searchTerm) ||
                    b.ShortDescriptionAr.Contains(searchTerm)||
                    b.ShortDescriptionEn.Contains(searchTerm)||
                    b.ContentAr.Contains(searchTerm),
                    pageNumber, pageSize);
            }

            if (items == null) return NotFound();

            var blogs = items.Data.Adapt<List<BlogsDto>>();

            return Ok(new PaginationResponse<List<BlogsDto>>
            {
                Data = blogs,
                IsError = false,
                Status = (int)StatusCodeEnum.Ok,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = items.TotalCount
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBlogById(Guid id)
        {
            var blog = await repository.Find(e => e.Id == id);
            if (blog == null) return NotFound(new Response<Guid>
            {
                Data = id,
                IsError = true,
                Message = "No Found This Blog",
                MessageAr = "لم يتم العثور علي المدونة",
                Status = (int)StatusCodeEnum.NotFound
            });

            var Blogs = blog.Adapt<BlogsDto>();

            return Ok(new Response<BlogsDto>
            {
                Data = Blogs,
                IsError = false,
                Message = "Success",
                MessageAr = "تم بنجاح",
                Status = (int)StatusCodeEnum.Ok
            });
        }

        [HttpPut]
        public async Task<IActionResult> UpdateBlogs([FromForm] BlogsCreateDto blogs, CancellationToken cancellationToken)
        {
            // Check if the blog exists
            var existingBlog = await repository.Find(e => e.Id == blogs.Id);
            if (existingBlog == null)
            {
                return NotFound(new Response<Guid>
                {
                    IsError = true,
                    Message = "Blog not found",
                    MessageAr = "لم يتم العثور على المدونة",
                    Status = (int)StatusCodeEnum.NotFound
                });
            }

            try
            {
                // Get the old image URLs before adapting
                var old1ImageUrl = existingBlog.Image1Url;
                var old2ImageUrl = existingBlog.Image2Url;

                blogs.Adapt(existingBlog);

                existingBlog.UpdatedBy = "Admin";
                existingBlog.UpdatedOn = DateTime.Now;

                if (blogs.Image1Url != null)
                {
                    // Re-upload new image and update the URL
                    var newImage1Url = await _filesManager.UploadFile(blogs.Image1Url, "Blogs");
                    existingBlog.Image1Url = newImage1Url.Url;
                }
                else
                    existingBlog.Image1Url = old1ImageUrl;

                if (blogs.Image2Url != null)
                {
                    var newImage2Url = await _filesManager.UploadFile(blogs.Image2Url, "Blogs");
                    existingBlog.Image2Url = newImage2Url.Url;
                }
                else
                    existingBlog.Image2Url = old2ImageUrl;

                repository.Update(existingBlog);
                await repository.SaveAsync(cancellationToken);

                return Ok(new Response<Guid>
                {
                    Data = existingBlog.Id,
                    IsError = false,
                    Message = "Blog updated successfully",
                    MessageAr = "تم تحديث المدونة بنجاح",
                    Status = (int)StatusCodeEnum.Ok
                });
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Conflict(new Response<Guid>
                {
                    Data = existingBlog.Id,
                    IsError = true,
                    Message = "Concurrency error: Blog was updated by someone else",
                    MessageAr = "خطأ في التزامن: تم تحديث المدونة بواسطة شخص آخر",
                    Status = (int)StatusCodeEnum.ServerError
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Response<Guid>
                {
                    IsError = true,
                    Message = "Internal server error occurred while updating the blog",
                    MessageAr = "حدث خطأ داخلي أثناء تحديث المدونة",
                    Status = (int)StatusCodeEnum.ServerError
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> PostBlogs([FromForm] BlogsCreateDto blogs, CancellationToken cancellationToken)
        {
            if (blogs.Image1Url == null || blogs.Image2Url == null)
            {
                return BadRequest(new Response<string>
                {
                    IsError = true,
                    Message = "Both images are required",
                    MessageAr = "الصور مطلوبة",
                    Status = (int)StatusCodeEnum.BadRequest
                });
            }

            try
            {
                var image1Result = await _filesManager.UploadFile(blogs.Image1Url, "Blogs");
                var image2Result = await _filesManager.UploadFile(blogs.Image2Url, "Blogs");

                if (image1Result == null || image2Result == null)
                {
                    return BadRequest(new Response<string>
                    {
                        IsError = true,
                        Message = "Failed to upload one or both images",
                        MessageAr = "فشل في رفع صورة أو كلا الصورتين",
                        Status = (int)StatusCodeEnum.BadRequest
                    });
                }

                var blog = blogs.Adapt<Blogs>();
                blog.CreatedBy = "Admin";
                blog.Image1Url = image1Result.Url;
                blog.Image2Url = image2Result.Url;

                await repository.Create(blog);
                await repository.SaveAsync(cancellationToken);

                return Ok(new Response<Guid>
                {
                    Data = blog.Id,
                    IsError = false,
                    Message = "Blog Created successfully",
                    MessageAr = "تم إنشاء المدونة بنجاح",
                    Status = (int)StatusCodeEnum.Ok
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Response<string>
                {
                    IsError = true,
                    Message = "Internal server error occurred while creating the blog",
                    MessageAr = "حدث خطأ داخلي أثناء إنشاء المدونة",
                    Status = (int)StatusCodeEnum.ServerError
                });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBlogs(Guid id, CancellationToken cancellationToken)
        {
            var blogs = await repository.Find(e => e.Id == id);
            if (blogs == null)
                return NotFound(new Response<Guid>
                {
                    Data = id,
                    IsError = true,
                    Message = "Failed",
                    MessageAr = "فشل التحديث",
                    Status = (int)StatusCodeEnum.NotFound
                });

            repository.Delete(blogs);
            await repository.SaveAsync(cancellationToken);

            return Ok(new Response<Guid>
            {
                Data = id,
                IsError = false,
                Message = "Deleted Successfully",
                MessageAr = "تم المسح بنجاح",
                Status = (int)StatusCodeEnum.Ok
            });
        }
    }
}