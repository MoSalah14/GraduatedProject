using OutbornE_commerce.DAL.Enums;

namespace OutbornE_commerce.BAL.Dto.Sizes
{
    public class SizeDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } // Clothing {S,M,L,Xl,XXL , etc ..} , Shoes {32 , 33 , 34 ,35 , ... 46}
        public TypeEnum Type { get; set; }

    }
}