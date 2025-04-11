using OutbornE_commerce.DAL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.DAL.Models
{
    public class Size : BaseEntity
    {
        public string Name { get; set; } // Clothing {S,M,L,Xl,XXL , etc ..} , Shoes {32 , 33 , 34 ,35 , ... 46}
        public TypeEnum Type { get; set; }
        public List<ProductSize> productSizes { get; set; } = new List<ProductSize>();

    }

}
