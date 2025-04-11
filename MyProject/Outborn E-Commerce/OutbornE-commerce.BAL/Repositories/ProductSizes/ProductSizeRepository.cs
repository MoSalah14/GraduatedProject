using Microsoft.EntityFrameworkCore;
using OutbornE_commerce.BAL.Repositories.BaseRepositories;
using OutbornE_commerce.DAL.Data;
using OutbornE_commerce.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Repositories.ProductSizes
{
    public class ProductSizeRepository : BaseRepository<ProductSize>, IProductSizeRepository
    {
        public ProductSizeRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<bool> ApplyDiscountToCategory(Guid discountCategoryId)
        {
            try
            {
               
                
                var discountCategory = await _context.DiscountCategories
                    .Where(dc => dc.Id == discountCategoryId)
                    .Select(dc => new
                    {
                        dc.IsActive,
                        discountnumber=dc.Discount.Number,
                        DiscountPercentage = dc.Discount.Percentage,
                        ProductSizes = dc.Category.CategorySubCategories
                            .SelectMany(csc => csc.SubCategory.SubCategoryTypes)
                            .Select(csc => csc.type)
                            .SelectMany(sct => sct.Products)
                            .SelectMany(p => p.ProductColors)
                            .SelectMany(pc => pc.ProductSizes)
                            .ToList()  
                    })
                    .FirstOrDefaultAsync();

                if (discountCategory == null)
                    return false;

             
                foreach (var size in discountCategory.ProductSizes)
                {
                    decimal discountedPrice = size.Price - (size.Price * (discountCategory.DiscountPercentage / 100));
                    if (discountCategory.discountnumber> 0 && size.Price > discountCategory.discountnumber)
                    {
                        discountedPrice = size.Price - discountCategory.discountnumber;
                    }

                    size.DiscountedPrice = discountedPrice;

                    Update(size);
                }

              
                await _context.SaveChangesAsync();
                return true; 
            }
            catch (Exception ex)
            {
              
                return false; 
            }
        }
            public async Task UpdateProductQuantities(Order order, CancellationToken cancellationToken)
        {
            foreach (var orderItem in order.OrderItems)
            {
                var productSize = await Find(x => x.Id == orderItem.ProductSizeId);
                if (productSize != null)
                {
                    productSize.Quantity += orderItem.Quantity;
                    Update(productSize);
                    await SaveAsync(cancellationToken);
                }
            }
        }

        public async Task<bool> ApplyDiscountToProductSize(Guid productSizeDiscountId)
        {

            var productSizeDiscount = await _context.ProductSizeDiscounts
                .Include(psd => psd.Discount)
                .Include(psd => psd.ProductSize)
                .FirstOrDefaultAsync(psd => psd.Id == productSizeDiscountId);
            if (productSizeDiscount == null || !productSizeDiscount.IsActive)
            {
                return false;
            }

            var productSizes = await FindByCondition(ps => ps.ProductSizeDiscounts.Any(d => d.Id == productSizeDiscountId));

            if (!productSizes.Any())
            {
                return false;
            }

            foreach (var productSize in productSizes)
            {
                decimal discountedPrice = productSize.Price - (productSize.Price * (productSizeDiscount.Discount.Percentage / 100));
                if (productSizeDiscount.Discount.Number > 0 && productSize.Price > productSizeDiscount.Discount.Number)
                {
                    discountedPrice = productSize.Price - productSizeDiscount.Discount.Number;
                }

                productSize.DiscountedPrice = discountedPrice;

                if (productSize.ProductSizeDiscounts == null)
                {
                    productSize.ProductSizeDiscounts = new List<ProductSizeDiscount>();
                }
                productSize.ProductSizeDiscounts.Add(productSizeDiscount);

                _context.Update(productSize);
            }

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ResetDiscountsByCategoryId(Guid discountCategoryId)
        {
            try
            {
                var discountCategory = await _context.DiscountCategories
                    .Include(dc => dc.Category)
                        .ThenInclude(c => c.CategorySubCategories)
                        .ThenInclude(cs => cs.SubCategory)
                        .ThenInclude(sc => sc.SubCategoryTypes)
                        .ThenInclude(sct => sct.type)
                        .ThenInclude(t => t.Products)
                        .ThenInclude(p => p.ProductColors)
                        .ThenInclude(pc => pc.ProductSizes)
                    .FirstOrDefaultAsync(dc => dc.Id == discountCategoryId && dc.IsActive);

                if (discountCategory == null) return true;

                var productSizes = discountCategory.Category.CategorySubCategories
                    .SelectMany(cs => cs.SubCategory.SubCategoryTypes)
                    .SelectMany(sct => sct.type.Products)
                    .SelectMany(p => p.ProductColors)
                    .SelectMany(pc => pc.ProductSizes)
                    .ToList();

                foreach (var size in productSizes)
                {
                    size.DiscountedPrice = 0;
                    _context.ProductSizes.Update(size);
                }

                //discountCategory.IsActive = false;
                //_context.DiscountCategories.Update(discountCategory);

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

    }
}