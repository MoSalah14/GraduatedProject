using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Dto.Products
{
    public class SearchModelDto
    {
        private int _pageNumber = 1;
        private int _pageSize = 20;

        public string? SearchTerm { get; set; }
        public List<Guid>? BrandsIds { get; set; }
        public List<Guid>? CategoriesIds { get; set; }
        public List<Guid>? SizesIds { get; set; }
        public List<Guid>? ColorsIds { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public List<Guid>? TypeIds { get; set; }

        public int PageNumber
        {
            get => _pageNumber;
            set => _pageNumber = value > 0 ? value : 1;
        }

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > 0 ? value : 20;
        }
    }
}