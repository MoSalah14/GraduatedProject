using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Dto
{
    public class PaginationResponse<T>
    {
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public bool IsError { get; set; }
        public int Status { get; set; }
        public T Data { get; set; }
       
    }
}
