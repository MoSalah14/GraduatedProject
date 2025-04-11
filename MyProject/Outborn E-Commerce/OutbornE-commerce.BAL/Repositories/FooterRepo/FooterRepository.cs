using OutbornE_commerce.BAL.Repositories.BaseRepositories;
using OutbornE_commerce.DAL.Data;
using OutbornE_commerce.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Repositories.FooterRepo
{
    public class FooterRepository : BaseRepository<Footer>, IFooterRepository
    {
        public FooterRepository(ApplicationDbContext context) : base(context) { }
    }
}
