
using System.ComponentModel.DataAnnotations;

namespace OutbornE_commerce.BAL.Dto.Products
{
    public class GetAllProductForUserDto
    {
        public Guid? Id { get; set; }
        [Display(Name = "English Name")]
        public string NameEn { get; set; }

        [Display(Name = "Arabic Name")]
        public string NameAr { get; set; }

        [Display(Name = "Main Image URL")]
        public string MainImageUrl { get; set; }

        [Display(Name = "Price")]
        public decimal Price { get; set; }

        [Display(Name = "Discount Price")]
        public decimal? DiscountPrice { get; set; }

        [Display(Name = "Average Rating")]
        public int RatingAverage { get; set; }

        [Display(Name = "Created On")]
        public DateTime? CreatedOn { get; set; }
    }

    public class GetAllProductForUserDtoWithCategory : GetAllProductForUserDto
    {
        public Guid? CategoryID { get; set; }
        public string CategoryNameEn { get; set; }
        public string CategoryNameAr { get; set; }
    }
}