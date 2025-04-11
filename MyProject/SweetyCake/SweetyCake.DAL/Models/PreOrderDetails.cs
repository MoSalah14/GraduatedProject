using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.DAL.Models
{
    public class PreOrderDetails : BaseEntity
    {
        public int PreparationTimeInDays { get; set; }   // Time in days required to prepare the order
        public Guid ProductId { get; set; }
        public Product Product { get; set; }
    }
}
