using OutbornE_commerce.DAL.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Dto.FooterDto
{
    public class CreateFooterDto
    {
        public Guid? Id { get; set; }

        public string TitleEnglish { get; set; }
        public string TitleArabic { get; set; }
        public ParentTitleEnglish ParentTitleEn { get; set; }
        public ParentTitleArabic ParentTitleAr { get; set; }

        public string LinkPage { get; set; }
        public bool IsActive { get; set; } = true;

    }
}
