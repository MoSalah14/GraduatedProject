using OutbornE_commerce.BAL.Repositories.BaseRepositories;
using OutbornE_commerce.DAL.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Repositories.ContactUs
{
    public class ContactUsRepository : BaseRepository<DAL.Models.ContactUs> , IContactUsRepository
    {
        public ContactUsRepository(ApplicationDbContext context):base(context)
        {
            
        }
    }
}
