using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Dto
{
    public class RegisterModel
    {
        [Required, StringLength(150)]
        public string FirstName { get; set; }
        [Required, StringLength(150)]
        public string LastName { get; set; }

        [Required, StringLength(128)]
        public string Email { get; set; }
        [Required, StringLength(256)]
        public string Password { get; set; }
        [Required, StringLength(150)]
        public string PhoneNumber { get; set; }
        [Required, StringLength(150)]
        public string Address { get; set; }

        [Required, StringLength(150)]
        public string City { get; set; }
    }
}
