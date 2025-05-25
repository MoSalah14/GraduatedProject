using System;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.DAL.Models
{
    public class User : IdentityUser
    {
        public string FullName { get; set; }
        public string? ProfilePicture { get; set; }
        public string? ProfilePictureName { get; set; }
        public int AccountType { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string? PhoneNumber { get; set; }
        public List<Address>? Addresses { get; set; }
        public bool IsDeleted { get; set; } = false;
        public virtual ICollection<Reviews> Users_Reviews { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<WishList> WishLists { get; set; }
        public virtual ICollection<ContactUs> ContactUs { get; set; }
    }
}