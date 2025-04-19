using Microsoft.EntityFrameworkCore;
using OutbornE_commerce.DAL.Models;

namespace OutbornE_commerce.BAL.Extentions;

public static class SearchProductExtention
{

    public static IQueryable<Product> SearchByTerm(this IQueryable<Product> products, string? searchTerm)
    {
        if (!string.IsNullOrEmpty(searchTerm))
            products = products.Where(p => p.NameEn.Contains(searchTerm));

        return products;
    }
    public static IQueryable<Product> SearchByPrice(this IQueryable<Product> products, decimal? minPrice, decimal? maxPrice)
    {
        if ((minPrice.HasValue && minPrice.Value >= 0) || (maxPrice.HasValue && maxPrice.Value > 0))
        {

            if (minPrice.HasValue && minPrice.Value >= 0)
            {
                products = products.Where(p => p.Price > minPrice);
            }

            if (maxPrice.HasValue && maxPrice.Value > 0)
            {
                products = products.Where(p => p.Price < maxPrice);
            }
        }

        return products;
    }
}