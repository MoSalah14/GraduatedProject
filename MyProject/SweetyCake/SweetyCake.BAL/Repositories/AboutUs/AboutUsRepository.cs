using OutbornE_commerce.BAL.Repositories.BaseRepositories;
using OutbornE_commerce.DAL.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Repositories.AboutUs
{
    public class AboutUsRepository : BaseRepository<DAL.Models.AboutUs> , IAboutUsRepository
    {
        public AboutUsRepository(ApplicationDbContext context ):base(context)
        {
            
        }
    }
}
