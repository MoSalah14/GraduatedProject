using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Dto
{
    public class UpdateCurrenciesResponse
    {
        public Dictionary<string,decimal> Rates { get; set; }
    }
}
