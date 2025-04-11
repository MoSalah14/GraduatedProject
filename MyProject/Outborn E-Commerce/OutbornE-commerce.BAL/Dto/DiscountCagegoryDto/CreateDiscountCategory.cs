using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Dto.DiscountCagegoryDto
{
    public class CreateDiscountCategory : IValidatableObject
    {
        public Guid? Id { get; set; }
        public Guid CategoryId { get; set; }
        public Guid DiscountId { get; set; }
        public bool IsActive { get; set; }

        [Required(ErrorMessage = "StartDate is required.")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "EndDate is required.")]
        public DateTime EndDate { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (EndDate < StartDate)
            {
                yield return new ValidationResult("EndDate must be later than StartDate.", new[] { "EndDate" });
            }
        }
    }
}
