using OutbornE_commerce.BAL.Dto.Categories;
using OutbornE_commerce.DAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Dto.discount
{
    public class CreateDiscountDto 
    {

        [Range(0, 99, ErrorMessage = "The discount percentage must be between 0 and 100.")]
        public decimal? Percentage { get; set; }
        public decimal? Number { get; set; }

        public List<string> Validate()
        {
            var errors = new List<string>();

            if (Percentage.HasValue && Number.HasValue)
            {
                errors.Add("Only one of Percentage or Number can be specified.");
            }

            if (!Percentage.HasValue && !Number.HasValue)
            {
                errors.Add("You must specify either a Percentage or a Number.");
            }

            return errors;
        }

    }
}
