using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Extentions
{
    public class SortingCriteria
    {
        public string SortBy { get; set; } = "default"; // Default sorting property
        public bool IsAscending { get; set; } = true;  // Default order is ascending
    }
}