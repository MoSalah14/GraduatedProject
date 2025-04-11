using OutbornE_commerce.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Dto.Review
{
    public class ReviewDataDto
    {
        public string? Comment { get; set; }
        public int Rating { get; set; }

        public string User { get; set; }
        public DateTime ReviewDate { get; set; }
    }
}
