using OutbornE_commerce.BAL.Dto;
using OutbornE_commerce.BAL.Dto.Categories;
using OutbornE_commerce.BAL.Repositories.BaseRepositories;
using OutbornE_commerce.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Repositories.Categories
{
    public interface ICategoryRepository : IBaseRepository<Category>
    {
        Task<PagainationModel<IEnumerable<GetAllCategorieswithSubsDto>>> GetAllCategoriesWithSubCategoriesAsync(
                int pageNumber, int pageSize, string? searchTerm, CancellationToken cancellationToken);    }
}
