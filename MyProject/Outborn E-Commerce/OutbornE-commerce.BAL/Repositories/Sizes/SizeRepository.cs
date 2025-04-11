using Microsoft.EntityFrameworkCore;
using OutbornE_commerce.BAL.Repositories.BaseRepositories;
using OutbornE_commerce.DAL.Data;
using OutbornE_commerce.DAL.Enums;
using OutbornE_commerce.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Repositories.Sizes
{
    public class SizeRepository : BaseRepository<Size> , ISizeRepository
    {
        public SizeRepository(ApplicationDbContext context) : base(context) { }

        //public async Task<List<Size>> GetSizesBasedType(int type)
        //{
        //    return await _context.Sizes.Where(x => x.Type == (TypeEnum)type).ToListAsync();
        //}

    }
}
