using Microsoft.EntityFrameworkCore;
using OutbornE_commerce.BAL.Dto;
using OutbornE_commerce.BAL.Dto.Categories;
using OutbornE_commerce.BAL.Repositories.BaseRepositories;
using OutbornE_commerce.DAL.Data;
using OutbornE_commerce.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Repositories.Categories
{
    public class CategoryRepository : BaseRepository<Category> , ICategoryRepository
    {
        public CategoryRepository(ApplicationDbContext context) : base(context)
        {
        }
        public async Task<PagainationModel<IEnumerable<GetAllCategorieswithSubsDto>>> GetAllCategoriesWithSubCategoriesAsync(
        int pageNumber, int pageSize, string? searchTerm, CancellationToken cancellationToken)
        {
            var query = _context.Categories
                .Include(c => c.CategorySubCategories)
                .ThenInclude(cs => cs.SubCategory)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(c => c.NameEn.Contains(searchTerm) || c.NameAr.Contains(searchTerm));
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var categoriesWithSubCategories = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new GetAllCategorieswithSubsDto
                {
                    Id = c.Id,
                    NameEn = c.NameEn,
                    NameAr = c.NameAr,
                    subCategories = c.CategorySubCategories
                        .Select(cs => new getAllSubCategoriesDto
                        {
                            Id = cs.SubCategory.Id,
                            NameEn = cs.SubCategory.NameEn,
                            NameAr = cs.SubCategory.NameAr
                        })
                        .ToList()
                })
                .ToListAsync(cancellationToken);

            return new PagainationModel<IEnumerable<GetAllCategorieswithSubsDto>>
            {
                Data = categoriesWithSubCategories,
                TotalCount = totalCount
            };
        }


    }
}
