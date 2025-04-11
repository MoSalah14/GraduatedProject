using OutbornE_commerce.BAL.Repositories.BaseRepositories;
using OutbornE_commerce.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Repositories.Sizes
{
    public interface ISizeRepository : IBaseRepository<Size>
    {
       // Task<List<Size>> GetSizesBasedType(int type);
    }
}
