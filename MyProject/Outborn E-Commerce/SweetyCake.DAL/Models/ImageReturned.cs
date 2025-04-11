using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.DAL.Models
{
    public class ImageReturned
    {
        public Guid ID { get; set; }
        public string ImageUrl { get; set; }
        public Guid ReturnItemReasonId { get; set; }
        public ReturnItemReason? ReturnItemReason { get; set; }
    }
}