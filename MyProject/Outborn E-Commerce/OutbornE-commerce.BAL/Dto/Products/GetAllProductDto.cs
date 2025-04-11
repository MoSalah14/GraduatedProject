using OutbornE_commerce.DAL.Enums;

public class GetAllProductDto
{
    public Guid Id { get; set; }
    public string NameEn { get; set; }
    public string NameAr { get; set; }
    public List<string>? ImageUrl { get; set; }
    public Guid? BrandID { get; set; }
    public string? BrandNameEn { get; set; }
    public string? BrandNameAr { get; set; }
    public ProductLabelEnum Label { get; set; }
    public decimal Price { get; set; }
    public Guid? ProductTypeID { get; set; }
    public string? ProductTypeNameEn { get; set; }
    public string? ProductTypeNameAr { get; set; }
    public Guid? SubCategoryID { get; set; }
    public string? SubCategoryNameEn { get; set; }
    public string? SubCategoryNameAr { get; set; }
    public Guid? CategoryID { get; set; }
    public string? CategoryNameEn { get; set; }
    public string? CategoryNameAr { get; set; }
}