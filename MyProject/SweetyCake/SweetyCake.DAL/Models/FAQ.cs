using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.DAL.Models
{
    public class FAQ : BaseEntity
    {
        public string QuestionEn { get; set; }
        public string QuestionAr { get; set; }
        public string AnswerEn { get; set; }
        public string AnswerAr { get; set; }
    }
}
