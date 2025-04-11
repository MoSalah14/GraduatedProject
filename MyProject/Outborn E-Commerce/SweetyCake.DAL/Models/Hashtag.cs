using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.DAL.Models
{
    public class Hashtag : BaseEntity
    {
        public required string NameEn {  get; set; }
        public required string NameAr { get; set; }
    }
}
