using OutbornE_commerce.BAL.Dto.Products;
using OutbornE_commerce.BAL.Dto;
using OutbornE_commerce.BAL.Repositories.BaseRepositories;
using OutbornE_commerce.DAL.Data;
using OutbornE_commerce.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace OutbornE_commerce.BAL.Repositories.Brands
{
    public class BrandRepository : BaseRepository<Brand>, IBrandRepository
    {
        public BrandRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Brand?> GetLastBrandAsync()
        {
            return await _context.Brands.OrderByDescending(b => b.BrandNumber).FirstOrDefaultAsync();
        }
    }
}