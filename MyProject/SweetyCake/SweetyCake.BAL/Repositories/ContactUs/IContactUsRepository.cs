using Microsoft.AspNetCore.Mvc;
using OutbornE_commerce.BAL.Dto.ContactUs;
using OutbornE_commerce.BAL.Repositories.BaseRepositories;
using OutbornE_commerce.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Repositories.ContactUs
{
    public interface IContactUsRepository : IBaseRepository<DAL.Models.ContactUs>
    {
        Task<bool> CreateContact(ContactUsForCreationDto dto, string UserUd);
    }
}
