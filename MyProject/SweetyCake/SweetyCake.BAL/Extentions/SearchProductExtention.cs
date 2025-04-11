using Microsoft.EntityFrameworkCore;
using OutbornE_commerce.DAL.Models;

namespace OutbornE_commerce.BAL.Extentions;

public static class SearchProductExtention
{
    public static IQueryable<Product> SearchByBrand(this IQueryable<Product> products, List<Guid>? brandsIds)
    {
        if (brandsIds != null && brandsIds.Any())
            products = products.Where(p => brandsIds.Contains(p.BrandId));
        return products;
    }

    public static IQueryable<Product> SearchByType(this IQueryable<Product> products, List<Guid> TypeIds)
    {
        if (TypeIds != null && TypeIds.Any())
            products = products.Where(p => TypeIds.Contains(p.ProductTypeId.Value));
        return products;
    }

    public static IQueryable<Product> SearchByTerm(this IQueryable<Product> products, string? searchTerm)
    {
        if (!string.IsNullOrEmpty(searchTerm))
            products = products.Where(p => p.NameEn.Contains(searchTerm) || p.NameAr.Contains(searchTerm));

        return products;
    }

    //public static IQueryable<Product> SearchByCategories(this IQueryable<Product> products, List<Guid>? categoriesIds)
    //{
    //    if (categoriesIds != null && categoriesIds.Any())
    //    {
    //        products = products.Include(p => p.ProductCategories)
    //                    .Where(p => p.ProductCategories!.Any(c => categoriesIds.Contains(c.CategoryId)));
    //    }
    //    return products;
    //}

    public static IQueryable<Product> SearchBySizes(this IQueryable<Product> products, List<Guid>? sizesIds)
    {
        if (sizesIds != null && sizesIds.Any())
        {
            products = products.Where(p => p.ProductColors!.Any(pc => pc.ProductSizes
                                   .Any(ps => sizesIds.Contains(ps.SizeId))));
        }

        return products;
    }

    public static IQueryable<Product> SearchByColors(this IQueryable<Product> products, List<Guid>? colorsIds)
    {
        if (colorsIds != null && colorsIds.Any())
            products = products.Where(p => p.ProductColors!.Any(c => colorsIds.Contains(c.ColorId)));

        return products;
    }

    public static IQueryable<Product> SearchByPrice(this IQueryable<Product> products, decimal? minPrice, decimal? maxPrice)
    {
        if ((minPrice.HasValue && minPrice.Value >= 0) || (maxPrice.HasValue && maxPrice.Value > 0))
        {
            //products = products.Include(p => p.ProductColors)
            //                   .ThenInclude(pc => pc.ProductSizes);

            if (minPrice.HasValue && minPrice.Value >= 0)
            {
                products = products.Where(p => p.ProductColors.Any(pc => pc.ProductSizes
                                                    .Any(ps => ps.Price >= minPrice.Value)));
            }

            if (maxPrice.HasValue && maxPrice.Value > 0)
            {
                products = products.Where(p => p.ProductColors
                                                .Any(pc => pc.ProductSizes
                                                    .Any(ps => ps.Price <= maxPrice.Value)));
            }
        }

        return products;
    }
}