using OutbornE_commerce.BAL.Dto.Categories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Dto.ProductCategories
{
    public class ProductCategoryDto
    {
        // public Guid Id { get; set; }
        // public Guid CategoryId { get; set; }
        // public CategoryDto? Category { get; set; }
        public Guid? Id { get; set; }

        public string NameEn { get; set; }
        public string NameAr { get; set; }

        //public Guid? SuperCategoryID { get; set; }
        //  public string? DescriptionEn { get; set; }
        //  public string? DescriptionAr { get; set; }
        // public string? ImageUrl { get; set; }
    }
}