using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OutbornE_commerce.BAL.Dto.BlogCategory;
using OutbornE_commerce.BAL.Dto.Blogs;
using OutbornE_commerce.BAL.Repositories.BaseRepositories;
using OutbornE_commerce.DAL.Data;
using OutbornE_commerce.DAL.Models;

namespace OutbornE_commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogCategoriesController : ControllerBase
    {
        private readonly IBaseRepository<BlogCategory> repository;

        public BlogCategoriesController(IBaseRepository<BlogCategory> _repository)
        {
            repository = _repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBlogs(int pageNumber = 1, int pageSize = 10, string? searchTerm = null)
        {
            var items = new PagainationModel<IEnumerable<BlogCategory>>();

            if (string.IsNullOrEmpty(searchTerm))
            {
                items = await repository.FindAllAsyncByPagination(null, pageNumber, pageSize);
            }
            else
            {
                items = await repository.FindAllAsyncByPagination(b =>
                    b.NameEn.Contains(searchTerm) ||
                    b.NameAr.Contains(searchTerm),
                    pageNumber, pageSize);
            }

            if (items == null) return NotFound();

            var blogs = items.Data.Adapt<List<BlogCategoryDto>>();

            return Ok(new PaginationResponse<List<BlogCategoryDto>>
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
            var blogCategory = await repository.Find(e => e.Id == id);
            if (blogCategory == null) return NotFound();

            var Blogs = blogCategory.Adapt<BlogCategoryDto>();

            return Ok(new Response<BlogCategoryDto>
            {
                Data = Blogs,
                IsError = false,
                Message = "Success",
                MessageAr = "تم بنجاح",
                Status = (int)StatusCodeEnum.Ok
            });
        }

        [HttpPut]
        public async Task<IActionResult> UpdateBlogCategory(BlogCategoryDto blogs, CancellationToken cancellationToken)
        {
            var GetBlogs = await repository.Find(e => e.Id == blogs.Id);
            if (GetBlogs is null)
                return BadRequest(new Response<Guid>
                {
                    IsError = true,
                    Message = "Category Not Found",
                    MessageAr = "فشل التحديث",
                    Status = (int)StatusCodeEnum.BadRequest
                });

            //var Blogs = blogs.Adapt<BlogCategory>();
            GetBlogs.NameAr = blogs.NameAr;
            GetBlogs.NameEn = blogs.NameEn;
            GetBlogs.CreatedBy = "Admin";
            GetBlogs.UpdatedBy = "Admin";
            GetBlogs.UpdatedOn = DateTime.Now;


            repository.Update(GetBlogs);
            // _context.Entry(blogs).State = EntityState.Modified;

            try
            {
                await repository.SaveAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest(new Response<Guid>
                {
                    Data = GetBlogs.Id,
                    IsError = true,
                    Message = "Failed",
                    MessageAr = "فشل التحديث",
                    Status = (int)StatusCodeEnum.BadRequest
                });
            }

            return Ok(new Response<Guid>
            {
                Data = GetBlogs.Id,
                IsError = false,
                Message = "Success",
                MessageAr = "تم بنجاح",
                Status = (int)StatusCodeEnum.Ok
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateBlogCategory(BlogCategoryDto blogs, CancellationToken cancellationToken)
        {
            try
            {
                var Blogs = blogs.Adapt<BlogCategory>();
                Blogs.CreatedBy = "Admin";

                await repository.Create(Blogs);
                await repository.SaveAsync(cancellationToken);

                return Ok(new Response<Guid>
                {
                    Data = Blogs.Id,
                    IsError = false,
                    Message = "Success",
                    MessageAr = "تم بنجاح",
                    Status = (int)StatusCodeEnum.Ok
                });

            }
            catch (Exception)
            {

                return BadRequest(new Response<Guid>
                {
                    IsError = true,
                    Message = "Failed Create",
                    MessageAr = "فشل الانشاء",
                    Status = (int)StatusCodeEnum.BadRequest
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
