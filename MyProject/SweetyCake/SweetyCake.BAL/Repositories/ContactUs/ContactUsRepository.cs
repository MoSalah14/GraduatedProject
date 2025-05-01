using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OutbornE_commerce.BAL.Dto.ContactUs;
using OutbornE_commerce.BAL.Repositories.BaseRepositories;
using OutbornE_commerce.DAL.Data;

namespace OutbornE_commerce.BAL.Repositories.ContactUs
{
    public class ContactUsRepository : BaseRepository<DAL.Models.ContactUs>, IContactUsRepository
    {
        private readonly ApplicationDbContext _Context;

        public ContactUsRepository(ApplicationDbContext context) : base(context)
        {
            _Context = context;
        }

        public async Task<bool> CreateContact(ContactUsForCreationDto dto, string UserUd)
        {
            try
            {
                var RealUser = await _Context.Users.FindAsync(UserUd);

                if (RealUser is null)
                    return false;
                

                var contact = new DAL.Models.ContactUs
                {
                    Name = RealUser.FullName,
                    Email = dto.Email,
                    Subject = dto.Subject,
                    UserId = UserUd
                };

                _Context.ContactUs.Add(contact);
                await _Context.SaveChangesAsync();

                return true;

            }
            catch
            {
                return false;
            }
            
        }
    }
}
