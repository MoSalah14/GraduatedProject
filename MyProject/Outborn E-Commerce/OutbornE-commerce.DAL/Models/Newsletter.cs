using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.DAL.Models
{
    public class Newsletter : BaseEntity
    {
        public string Subject {  get; set; }
        public string Body { get; set; }
    }
}
