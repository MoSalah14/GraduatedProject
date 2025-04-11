using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.DAL.Enums
{
    public enum ParentTitleEnglish
    {
        CUSTOMERCARE = 1,
        INFORMATION = 2,

        [Display(Name = "MY ACCOUNT")]
        MyAccount = 3,

        CURRENCY = 4,
    }

    public enum ParentTitleArabic
    {
        [Display(Name = "خدمة العملاء")]
        الزبائن = 1,

        معلومات = 2,
        حسابي = 3,
        العملات = 4,
    }
}