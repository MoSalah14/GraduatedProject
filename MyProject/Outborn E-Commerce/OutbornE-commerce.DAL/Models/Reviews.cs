using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.DAL.Models
{
    public class Reviews
    {
        public Guid ReviewId { get; set; }
        public int? Rating { get; set; }  // Rating between 1 to 5
        public string? Comment { get; set; }
        public DateTime ReviewDate { get; set; } = DateTime.Now;
        public string UserId { get; set; }
        public virtual User User { get; set; }

        public Guid ProductId { get; set; }
        public virtual Product Product { get; set; }
    }
}
