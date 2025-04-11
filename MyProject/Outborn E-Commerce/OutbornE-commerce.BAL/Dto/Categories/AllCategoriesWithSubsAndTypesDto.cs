using OutbornE_commerce.BAL.Dto.TypeDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Dto.Categories
{
    public class AllCategoriesWithSubsAndTypesDto
    {
        public Guid? Id { get; set; }
        public string NameEn { get; set; }
        public string NameAr { get; set; }
        public List<getAllSubCategoriesDto>? SubCategories { get; set; }
    }
}