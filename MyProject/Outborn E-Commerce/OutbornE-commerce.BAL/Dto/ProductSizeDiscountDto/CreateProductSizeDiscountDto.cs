using OutbornE_commerce.DAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Dto.ProductSizeDiscountDto
{
    public class CreateProductSizeDiscountDto : IValidatableObject
    {
        public Guid? Id { get; set; }
        [Required(ErrorMessage = "StartDate is required.")]
        public DateTime StartDate { get; set; }
        [Required(ErrorMessage = "EndDate is required.")]
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; } = false;

        public Guid DiscountId { get; set; }
        public Guid ProductSizeId { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (EndDate < StartDate)
            {
                yield return new ValidationResult("EndDate must be later than StartDate.", new[] { "EndDate" });
            }
        }

    }
}
