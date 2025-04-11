using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Dto.Categories
{
    public class GetAllCategorieswithSubsDto
    {
        public Guid? Id { get; set; }
        public string NameEn { get; set; }
        public string NameAr { get; set; }
        public List<getAllSubCategoriesDto> subCategories { set; get; }
    }
}
