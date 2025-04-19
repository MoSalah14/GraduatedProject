using Microsoft.EntityFrameworkCore;
using OutbornE_commerce.DAL.Enums;
using OutbornE_commerce.DAL.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Extentions
{
    public static class SortingHelper
    {
        public static IQueryable<T> ApplySorting<T>(this IQueryable<T> query, SortingCriteria sortingCriteria)
        {
            // Check if T is Product
            if (typeof(T) == typeof(Product))
            {
                query = sortingCriteria.SortBy.ToLower() switch
                {
                    "newest" => sortingCriteria.IsAscending
                        ? query.OrderBy(p => ((Product)(object)p).CreatedOn)
                        : query.OrderByDescending(p => ((Product)(object)p).CreatedOn),

                    "price" => sortingCriteria.IsAscending
                        ? query.OrderBy(p => ((Product)(object)p).Price)
                        : query.OrderByDescending(p => ((Product)(object)p).Price),

                    //"bestseller" => sortingCriteria.IsAscending
                    //    ? query.OrderBy(p => ((Product)(object)p).Label)
                    //    : query.OrderByDescending(p => ((Product)(object)p).Label),

                    _ => sortingCriteria.IsAscending
                        ? query.OrderBy(p => ((Product)(object)p).NameEn)
                        : query.OrderByDescending(p => ((Product)(object)p).NameEn),
                };
            }
            else
            {
                query = sortingCriteria.SortBy.ToLower() switch
                {
                    "newest" => sortingCriteria.IsAscending
                        ? query.OrderBy(p => EF.Property<DateTime>(p, "CreatedOn"))
                        : query.OrderByDescending(p => EF.Property<DateTime>(p, "CreatedOn")),

                    "price" => sortingCriteria.IsAscending
                        ? query.OrderBy(p => EF.Property<decimal>(p, "Price"))
                        : query.OrderByDescending(p => EF.Property<decimal>(p, "Price")),

                    "bestseller" => sortingCriteria.IsAscending
                        ? query.OrderBy(p => EF.Property<int>(p, "Label"))
                        : query.OrderByDescending(p => EF.Property<int>(p, "Label")),

                    _ => sortingCriteria.IsAscending
                        ? query.OrderBy(p => EF.Property<string>(p, "NameEn"))
                        : query.OrderByDescending(p => EF.Property<string>(p, "NameEn")),
                };
            }

            return query;
        }
    }
}