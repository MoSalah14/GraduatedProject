using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Dto.Colors
{
    public class ColorDto
    {
        public Guid Id { get; set; }
        public string Hexadecimal { get; set; }
        public string NameAr { get; set; }
        public string NameEn { get; set; }
        public bool IsMultiColor { get; set; } = false;
    }
}