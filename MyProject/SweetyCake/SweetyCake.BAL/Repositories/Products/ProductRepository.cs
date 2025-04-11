using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OutbornE_commerce.BAL.Dto;
using OutbornE_commerce.BAL.Dto.Colors;
using OutbornE_commerce.BAL.Dto.Products;
using OutbornE_commerce.BAL.Dto.ProductSizes;
using OutbornE_commerce.BAL.Dto.Sizes;
using OutbornE_commerce.BAL.Dto.WishList;
using OutbornE_commerce.BAL.Extentions;
using OutbornE_commerce.BAL.Repositories.BaseRepositories;
using OutbornE_commerce.DAL.Data;
using OutbornE_commerce.DAL.Enums;
using OutbornE_commerce.DAL.Models;

namespace OutbornE_commerce.BAL.Repositories.Products
{
    public class ProductRepository : BaseRepository<Product>, IProductRepository
    {
        public ProductRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<List<WishListReportDto>> GetWishListProductAsync(CancellationToken cancellationToken)
        {
            var result = await _context.Products
                .Where(product => product.WishLists.Any())
                .Select(product => new WishListReportDto
                {
                    ProductName = product.NameEn,
                    WishCount = product.WishLists.Count
                })
                .ToListAsync(cancellationToken);

            return result;
        }

        public IQueryable<GetAllProductForUserDto> GetAllProductInHomePage(string searchTerm, int pageNumber, int pageSize, SortingCriteria? sortingCriteria = null)
        {
            // Start the query

            var ProductQuery = _context.Products.Select(p => new GetAllProductForUserDto
            {
                Id = p.Id,
                NameEn = p.NameEn,
                NameAr = p.NameAr,
                ImageUrl = p.ProductColors.SelectMany(e => e.ProductColorImages).OrderBy(e => e.IsDefault ? 0 : 1).Select(e => e.ImageUrl).Take(2).ToList(),
                BrandID = p.Brand!.Id,
                BrandNameEn = p.Brand!.NameEn,
                BrandNameAr = p.Brand.NameAr,
                //Label = p.Label,
                CreatedOn = p.CreatedOn,
                Label = p.Label == ProductLabelEnum.BestSeller ? p.Label : (ProductLabelEnum)3,
                Price = p.ProductColors.SelectMany(pc => pc.ProductSizes).Min(ps => ps.Price),
                DiscountPrice = p.ProductColors.SelectMany(pc => pc.ProductSizes)
                                       .Min(ps => ps.DiscountedPrice),

                ProductSizes = p.ProductColors
                        .SelectMany(pc => pc.ProductSizes)
                        .Select(ps => new SizeDto
                        {
                            Id = ps.Id,
                            Name = ps.Size.Name,
                        }).ToList(),

                ProductColors = p.ProductColors.Select(e => e.Color).Select(e => new ColorDto
                {
                    NameEn = e.NameEn,
                    NameAr = e.NameAr,
                    Id = e.Id,
                    Hexadecimal = e.Hexadecimal
                }).ToList(),
            });

            if (!string.IsNullOrEmpty(searchTerm))
            {
                var lowerSearchTerm = searchTerm.ToLower();
                ProductQuery = ProductQuery.Where(p =>
                    p.NameEn.ToLower().Contains(lowerSearchTerm) ||
                    p.NameAr.ToLower().Contains(lowerSearchTerm) ||
                    p.BrandNameAr.ToLower().Contains(lowerSearchTerm) ||
                    p.BrandNameEn.ToLower().Contains(lowerSearchTerm) ||
                    p.Price.ToString().Contains(lowerSearchTerm)
                );
            }

            if (sortingCriteria != null)
            {
                ProductQuery = ProductQuery.ApplySorting(sortingCriteria);
            }
            return ProductQuery;
        }

        public async Task<List<ProductNameIdModel>> GetProductNameAndIdByPaginationAsync(string searchTerm = null, int pageNumber = 1, int pageSize = 10)
        {
            // Base query that selects only Id and NameEn fields
            var query = _context.Products.Include(e => e.Reviews).AsNoTracking()
                .Select(p => new ProductNameIdModel
                {
                    Id = p.Id,
                    ProductName = p.NameEn,
                    Review = p.Reviews
                });

            if (!string.IsNullOrEmpty(searchTerm))
                query = query.Where(p => p.ProductName.Contains(searchTerm));

            int totalCount = await query.CountAsync();

            var ProductsWithReview = await query.Skip((pageNumber - 1) * pageSize)
                                           .Take(pageSize)
                                           .ToListAsync();
            return ProductsWithReview;
        }

        public async Task<PagainationModel<List<Product>>> SearchProducts(SearchModelDto model, SortingCriteria? sortingCriteria = null)
        {
            int totalCount = 0;
            var products = _context.Products
                                   .AsNoTracking()
                                   .Include(b => b.Brand)
                                   .Include(b => b.ProductType)
                                   .ThenInclude(b => b.SubCategoryTypes)
                                   .ThenInclude(b => b.SubCategory)
                                   .ThenInclude(b => b.CategorySubCategories)
                                   .ThenInclude(b => b.Category)
                                   .Include(e => e.ProductColors)
                                   .ThenInclude(e => e.Color)
                                   .Include(e => e.ProductColors)
                                   .ThenInclude(e => e.ProductSizes)
                                   .Include(e => e.ProductColors)
                                   .ThenInclude(e => e.ProductColorImages)
                                   .SearchByTerm(model.SearchTerm)
                                   .SearchByBrand(model.BrandsIds)
                                   .SearchByType(model.TypeIds)
                                   .SearchBySizes(model.SizesIds)
                                   .SearchByColors(model.ColorsIds)
                                   .SearchByPrice(model.MinPrice, model.MaxPrice);

            if (sortingCriteria is not null)
            {
                products = products.ApplySorting(sortingCriteria);
            }
            totalCount = products.Count();
            var data = await products.Skip((model.PageNumber - 1) * model.PageSize).Take(model.PageSize).ToListAsync();

            return new PagainationModel<List<Product>>()
            {
                TotalCount = totalCount,
                Data = data,
                PageNumber = model.PageNumber,
                PageSize = model.PageSize,
            };
        }

        public async Task<PaginationResponse<List<GetAllProductDto>>> GetProductsByBrandIdAsync(Guid brandId, int pageNumber, int pageSize, SortingCriteria? sortingCriteria = null)
        {
            var query = _context.Products
                .Include(p => p.Brand)
                .Include(p => p.ProductColors)
                    .ThenInclude(pc => pc.ProductColorImages)
                .Include(p => p.ProductColors)
                    .ThenInclude(pc => pc.ProductSizes)
                .Include(p => p.ProductType)
                    .ThenInclude(pt => pt.SubCategoryTypes)
                    .ThenInclude(st => st.SubCategory)
                    .ThenInclude(sc => sc.CategorySubCategories)
                    .ThenInclude(cs => cs.Category)
                .Where(p => p.BrandId == brandId);

            var totalCount = await query.CountAsync();

            if (sortingCriteria is not null)
            {
                query = query.ApplySorting(sortingCriteria);
            }

            var products = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var productDtos = products.Select(pc => new GetAllProductDto
            {
                Id = pc.Id,
                NameEn = pc.NameEn,
                NameAr = pc.NameAr,
                ImageUrl = pc.ProductColors
                             .SelectMany(e => e.ProductColorImages).Where(e => e.IsDefault == true).Select(e => e.ImageUrl).Take(2).ToList(),
                BrandNameEn = pc.Brand?.NameEn,
                BrandNameAr = pc.Brand?.NameAr,
                Label = pc.Label,

                Price = pc.ProductColors
                          .SelectMany(pc => pc.ProductSizes)
                          .Select(ps => ps.Price)
                          .DefaultIfEmpty(0)
                          .Min(),

                ProductTypeNameEn = pc.ProductType?.NameEn,
                ProductTypeNameAr = pc.ProductType?.NameAr,
                SubCategoryNameEn = pc.ProductType?.SubCategoryTypes
                                      .Select(st => st.SubCategory!.NameEn)
                                      .FirstOrDefault(),
                SubCategoryNameAr = pc.ProductType?.SubCategoryTypes
                                      .Select(st => st.SubCategory!.NameAr)
                                      .FirstOrDefault(),
                CategoryNameEn = pc.ProductType?.SubCategoryTypes
                                  .SelectMany(st => st.SubCategory!.CategorySubCategories)
                                  .Select(cs => cs.Category!.NameEn)
                                  .FirstOrDefault(),
                CategoryNameAr = pc.ProductType?.SubCategoryTypes
                                  .SelectMany(st => st.SubCategory!.CategorySubCategories)
                                  .Select(cs => cs.Category!.NameAr)
                                  .FirstOrDefault()
            }).ToList();

            return new PaginationResponse<List<GetAllProductDto>>
            {
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                IsError = false,
                Status = 200,
                Data = productDtos
            };
        }

        public IQueryable<GetAllProductForUserDto> GetAllBestSellerProduct(string searchTerm, int pageNumber, int pageSize)
        {
            // Start the query

            var ProductQuery = _context.Products.Where(e => e.Label == ProductLabelEnum.BestSeller).Select(p => new GetAllProductForUserDto
            {
                Id = p.Id,
                NameEn = p.NameEn,
                NameAr = p.NameAr,
                ImageUrl = p.ProductColors.SelectMany(e => e.ProductColorImages).OrderBy(e => e.IsDefault ? 0 : 1).Select(e => e.ImageUrl).Take(2).ToList(),
                BrandNameEn = p.Brand!.NameEn,
                BrandNameAr = p.Brand.NameAr,
                Label = p.Label,
                Price = p.ProductColors.SelectMany(pc => pc.ProductSizes)
                                       .Min(ps => ps.Price),
                ProductColors = p.ProductColors.Select(e => e.Color).Select(e => new ColorDto
                {
                    NameEn = e.NameEn,
                    NameAr = e.NameAr,
                    Id = e.Id,
                    Hexadecimal = e.Hexadecimal
                }).ToList(),
            });

            if (!string.IsNullOrEmpty(searchTerm))
            {
                var lowerSearchTerm = searchTerm.ToLower();
                ProductQuery = ProductQuery.Where(p =>
                    p.NameEn.ToLower().Contains(lowerSearchTerm) ||
                    p.NameAr.Contains(lowerSearchTerm) ||
                    p.Price.ToString().Contains(lowerSearchTerm)
                );
            }

            return ProductQuery;
        }

        public async Task<WishListReportDto> GetWishListByProduct(Guid productId)
        {
            var result = await _context.WishLists
                .Where(w => w.ProductId == productId)
                .GroupBy(w => new { w.ProductWishList.NameEn })
                .Select(g => new WishListReportDto
                {
                    ProductName = g.Key.NameEn,
                    WishCount = g.Count()
                })
                .FirstOrDefaultAsync();
            return result;
        }

        public async Task<PaginationResponse<List<GetAllProductDto>>> GetProductsBySubCategoryAsync(Guid subCategoryId, int pageNumber, int pageSize, SortingCriteria? sortingCriteria)
        {
            var query = _context.Products
      .Include(p => p.ProductType)
      .ThenInclude(pt => pt.SubCategoryTypes)
      .ThenInclude(sct => sct.SubCategory)
      .Include(p => p.ProductColors)
      .ThenInclude(pc => pc.ProductSizes)
      .Include(p => p.ProductColors)
      .ThenInclude(pc => pc.ProductColorImages)
      .Where(p => p.ProductType.SubCategoryTypes.Any(sct => sct.SubCategoryId == subCategoryId))
      .Select(p => new GetAllProductDto
      {
          Id = p.Id,
          NameEn = p.NameEn,
          NameAr = p.NameAr,
          ImageUrl = p.ProductColors
              .SelectMany(e => e.ProductColorImages)
              .Where(e => e.IsDefault)
              .Select(e => e.ImageUrl)
              .Take(2)
              .ToList(),
          BrandID = p.Brand.Id,
          BrandNameEn = p.Brand.NameEn,
          BrandNameAr = p.Brand.NameAr,
          Label = p.Label,
          Price = p.ProductColors
              .SelectMany(pc => pc.ProductSizes)
              .OrderBy(ps => ps.DiscountedPrice > 0 ? ps.DiscountedPrice : ps.Price)
              .Select(ps => ps.DiscountedPrice > 0 ? ps.DiscountedPrice : ps.Price)
              .FirstOrDefault(),
          ProductTypeID = p.ProductType.Id,
          ProductTypeNameEn = p.ProductType.NameEn,
          ProductTypeNameAr = p.ProductType.NameAr,
          SubCategoryID = p.ProductType.SubCategoryTypes
              .Where(sct => sct.SubCategoryId == subCategoryId)
              .Select(sct => sct.SubCategoryId)
              .FirstOrDefault(),
          SubCategoryNameEn = p.ProductType.SubCategoryTypes
              .Where(sct => sct.SubCategoryId == subCategoryId)
              .Select(sct => sct.SubCategory.NameEn)
              .FirstOrDefault(),
          SubCategoryNameAr = p.ProductType.SubCategoryTypes
              .Where(sct => sct.SubCategoryId == subCategoryId)
              .Select(sct => sct.SubCategory.NameAr)
              .FirstOrDefault(),
          CategoryID = p.ProductType.SubCategoryTypes
              .SelectMany(sct => sct.SubCategory.CategorySubCategories)
              .Select(csc => csc.CategoryId)
              .FirstOrDefault(),
          CategoryNameEn = p.ProductType.SubCategoryTypes
              .SelectMany(sct => sct.SubCategory.CategorySubCategories)
              .Select(csc => csc.Category.NameEn)
              .FirstOrDefault(),
          CategoryNameAr = p.ProductType.SubCategoryTypes
              .SelectMany(sct => sct.SubCategory.CategorySubCategories)
              .Select(csc => csc.Category.NameAr)
              .FirstOrDefault(),
      });

            if (sortingCriteria is not null)
            {
                query = query.ApplySorting(sortingCriteria);
            }

            var totalCount = await query.CountAsync();
            var products = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginationResponse<List<GetAllProductDto>>
            {
                Data = products,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        //public async Task<PaginationResponse<List<GetAllProductForUserDto>>> GetProductsBySubCategoryAsync(Guid subCategoryId, int pageNumber, int pageSize)
        //  {
        //      var query = _context.Products
        //          .Include(p => p.ProductType)
        //          .ThenInclude(pt => pt.SubCategoryTypes)
        //          .Include(p => p.ProductColors)
        //          .ThenInclude(pc => pc.ProductSizes)
        //          .Include(p => p.ProductColors)
        //          .ThenInclude(pc => pc.ProductColorImages)
        //          .Where(p => p.ProductType.SubCategoryTypes.Any(sct => sct.SubCategoryId == subCategoryId))
        //          .Select(p => new GetAllProductForUserDto
        //          {
        //              Id = p.Id,
        //              NameEn = p.NameEn,
        //              NameAr = p.NameAr,
        //              ImageUrl = p.ProductColors
        //                   .SelectMany(e => e.ProductColorImages).Where(e => e.IsDefault == true).Select(e => e.ImageUrl).Take(2).ToList(),
        //              BrandID = p.Brand.Id,
        //              BrandNameEn = p.Brand.NameEn,
        //              BrandNameAr = p.Brand.NameAr,
        //              Label = p.Label,
        //              ProductColors = p.ProductColors.Select(pc => new ColorDto
        //              {
        //                  Id = pc.Color.Id,
        //                  NameEn = pc.Color.NameEn,
        //                  NameAr = pc.Color.NameAr,
        //                  Hexadecimal = pc.Color.Hexadecimal,
        //              }).ToList(),
        //              ProductSizes = p.ProductColors
        //                  .SelectMany(pc => pc.ProductSizes)
        //                  .Select(ps => new SizeDto
        //                  {
        //                      Id = ps.Id,
        //                      Name = ps.Size.Name,
        //                  }).ToList(),
        //              Price = p.ProductColors
        //                  .SelectMany(pc => pc.ProductSizes)
        //                  .OrderBy(ps => ps.DiscountedPrice > 0 ? ps.DiscountedPrice : ps.Price)
        //                  .Select(ps => ps.DiscountedPrice > 0 ? ps.DiscountedPrice : ps.Price)
        //                  .FirstOrDefault(),
        //          });

        //      var totalCount = await query.CountAsync();
        //      var products = await query
        //          .Skip((pageNumber - 1) * pageSize)
        //          .Take(pageSize)
        //          .ToListAsync();

        //      return new PaginationResponse<List<GetAllProductForUserDto>>
        //      {
        //          Data = products,
        //          TotalCount = totalCount,
        //          PageNumber = pageNumber,
        //          PageSize = pageSize
        //      };
        //  }

        public async Task<PaginationResponse<List<GetAllProductForUserDtoًWithCategory>>> GetProductsByCategoryAsync(Guid CategoryId, int pageNumber, int pageSize, SortingCriteria? sortingCriteria = null)
        {
            var query = from p in _context.Products
                        join pt in _context.TypeEntities on p.ProductTypeId equals pt.Id
                        join sct in _context.SubCategoryTypes on pt.Id equals sct.TypeId
                        join sc in _context.SubCategories on sct.SubCategoryId equals sc.Id
                        join csc in _context.CategorySubCategoryBridges on sc.Id equals csc.SubCategoryId
                        join c in _context.Categories on csc.CategoryId equals c.Id
                        where csc.CategoryId == CategoryId
                        select new GetAllProductForUserDtoًWithCategory
                        {
                            Id = p.Id,
                            NameEn = p.NameEn,
                            NameAr = p.NameAr,
                            ImageUrl = p.ProductColors
                                         .SelectMany(e => e.ProductColorImages).Where(e => e.IsDefault == true).Select(e => e.ImageUrl).Take(2).ToList(),
                            BrandID = p.Brand.Id,
                            BrandNameEn = p.Brand.NameEn,
                            BrandNameAr = p.Brand.NameAr,
                            Label = p.Label,
                            ProductColors = p.ProductColors.Select(pc => new ColorDto
                            {
                                Id = pc.Color.Id,
                                Hexadecimal = pc.Color.Hexadecimal,
                                NameEn = pc.Color.NameEn,
                                NameAr = pc.Color.NameAr
                            }).ToList(),
                            ProductSizes = p.ProductColors
                                .SelectMany(pc => pc.ProductSizes)
                                .Select(ps => new SizeDto
                                {
                                    Id = ps.Id,
                                    Name = ps.Size.Name,
                                }).ToList(),
                            Price = p.ProductColors
                                .SelectMany(pc => pc.ProductSizes)
                                .OrderBy(ps => ps.DiscountedPrice > 0 ? ps.DiscountedPrice : ps.Price)
                                .Select(ps => ps.DiscountedPrice > 0 ? ps.DiscountedPrice : ps.Price)
                                .FirstOrDefault(),
                            // Select Category Name in both languages
                            CategoryID = c.Id,
                            CategoryNameEn = c.NameEn,
                            CategoryNameAr = c.NameAr
                        };

            if (sortingCriteria is not null)
            {
                query = query.ApplySorting(sortingCriteria);
            }

            var totalCount = await query.CountAsync();
            var products = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginationResponse<List<GetAllProductForUserDtoًWithCategory>>
            {
                Data = products,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<List<SalesReportDto>> GetSalesReportAsync(SalesReportSearch searchCriteria)
        {
            if (searchCriteria == null) throw new ArgumentNullException(nameof(searchCriteria));

            var query = _context.Products
           .Include(p => p.Brand)
           .Include(p => p.ProductColors)
           .ThenInclude(pc => pc.ProductSizes)
           .ThenInclude(ps => ps.OrderItems)
           .ThenInclude(oi => oi.Order)
                .Where(p => p.ProductColors.Any(pc => pc.ProductSizes.Any(ps =>
                        ps.OrderItems.Any(oi => oi.Order.CreatedOn >= searchCriteria.StartDate && oi.Order.CreatedOn <= searchCriteria.EndDate))))
       .Select(p => new SalesReportDto
       {
           StartDate = searchCriteria.StartDate,
           EndDate = searchCriteria.EndDate,
           Period = $"{(searchCriteria.EndDate - searchCriteria.StartDate).TotalDays} days",
           BrandNumber = p.Brand.BrandNumber,
           BrandName = p.Brand.NameEn,
           ProductSKU = p.SKU,
           ProductName = p.NameEn,
           Status = p.IsActive ? "Active" : "InActive",
           DateAdded = p.CreatedOn,
           RetailPrice = p.ProductColors
               .SelectMany(pc => pc.ProductSizes)
               .Select(ps => ps.Price)
               .FirstOrDefault(),
           PeriodStartInventory = p.ProductColors
               .SelectMany(pc => pc.ProductSizes)
               .Sum(ps => ps.Quantity),
           PeriodEndInventory = p.ProductColors
               .SelectMany(pc => pc.ProductSizes)
               .Sum(ps => ps.Quantity) -
               p.ProductColors.SelectMany(pc => pc.ProductSizes)
               .SelectMany(ps => ps.OrderItems
                   .Where(oi => oi.Order.CreatedOn >= searchCriteria.StartDate &&
                                oi.Order.CreatedOn <= searchCriteria.EndDate))
                                .Sum(oi => oi.Quantity),
           PeriodSalesCount = p.ProductColors
               .SelectMany(pc => pc.ProductSizes)
               .SelectMany(ps => ps.OrderItems
                   .Where(oi => oi.Order.CreatedOn >= searchCriteria.StartDate &&
                                oi.Order.CreatedOn <= searchCriteria.EndDate))
                                .Sum(oi => oi.Quantity),
           PeriodSalesAmount = p.ProductColors
               .SelectMany(pc => pc.ProductSizes)
               .SelectMany(ps => ps.OrderItems
                   .Where(oi => oi.Order.CreatedOn >= searchCriteria.StartDate &&
                                oi.Order.CreatedOn <= searchCriteria.EndDate))
                                .Sum(oi => oi.Quantity * oi.ItemPrice),
           TotalSalesCount = p.ProductColors
               .SelectMany(pc => pc.ProductSizes)
               .SelectMany(ps => ps.OrderItems)
               .Sum(oi => oi.Quantity),
           TotalSalesAmount = p.ProductColors
               .SelectMany(pc => pc.ProductSizes)
               .SelectMany(ps => ps.OrderItems)
               .Sum(oi => oi.Quantity * oi.ItemPrice),
           PeriodReturnsCount = p.ProductColors
               .SelectMany(pc => pc.ProductSizes)
               .SelectMany(ps => ps.OrderItems
                   .Where(oi => oi.Order.CreatedOn >= searchCriteria.StartDate &&
                                oi.Order.CreatedOn <= searchCriteria.EndDate &&
                                oi.Order.OrderStatus == OrderStatus.Returned))
                                .Sum(oi => oi.Quantity),
           PeriodReturnsAmount = p.ProductColors
               .SelectMany(pc => pc.ProductSizes)
               .SelectMany(ps => ps.OrderItems
                   .Where(oi => oi.Order.CreatedOn >= searchCriteria.StartDate &&
                                oi.Order.CreatedOn <= searchCriteria.EndDate &&
                                oi.Order.OrderStatus == OrderStatus.Returned))
                                .Sum(oi => oi.Quantity * oi.ItemPrice),
       });

            var reportData = await query.ToListAsync();
            return reportData;
        }

        public async Task<List<GetAllProductDto>> GetProductsByTypeIdAsync(Guid typeId, int pageNumber, int pageSize, SortingCriteria? sortingCriteria)
        {
            var products = _context.Products
                .Include(p => p.Brand)
                .Include(p => p.ProductColors)
                    .ThenInclude(pc => pc.ProductColorImages)
                .Include(p => p.ProductColors)
                    .ThenInclude(pc => pc.ProductSizes)
                .Include(p => p.ProductType)
                    .ThenInclude(pt => pt.SubCategoryTypes)
                    .ThenInclude(sct => sct.SubCategory)
                    .ThenInclude(sc => sc.CategorySubCategories)
                    .ThenInclude(csc => csc.Category)
                .Where(p => p.ProductTypeId == typeId);

            if (sortingCriteria is not null)
            {
                products = products.ApplySorting(sortingCriteria);
            }

            var totalCount = await products.CountAsync();
            var Returnproducts = await products
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            if (Returnproducts == null || !Returnproducts.Any())
            {
                return new List<GetAllProductDto>();
            }

            var productDtos = Returnproducts.Select(pc => new GetAllProductDto
            {
                Id = pc.Id,
                NameEn = pc.NameEn,
                NameAr = pc.NameAr,
                ImageUrl = pc.ProductColors
                             .SelectMany(e => e.ProductColorImages).Where(e => e.IsDefault == true).Select(e => e.ImageUrl).Take(2).ToList(),
                BrandID = pc.Brand?.Id,
                BrandNameEn = pc.Brand?.NameEn,
                BrandNameAr = pc.Brand?.NameAr,
                Label = pc.Label,

                Price = pc.ProductColors
                          .SelectMany(pc => pc.ProductSizes)
                          .Select(ps => ps.Price)
                          .DefaultIfEmpty(0)
                          .Min(),
                ProductTypeID = pc.ProductType?.Id,
                ProductTypeNameEn = pc.ProductType?.NameEn,
                ProductTypeNameAr = pc.ProductType?.NameAr,
                SubCategoryID = pc.ProductType.SubCategoryTypes.Select(st => st.SubCategory!.Id)
                                      .FirstOrDefault(),
                SubCategoryNameEn = pc.ProductType?.SubCategoryTypes
                                      .Select(st => st.SubCategory!.NameEn)
                                      .FirstOrDefault(),
                SubCategoryNameAr = pc.ProductType?.SubCategoryTypes
                                      .Select(st => st.SubCategory!.NameAr)
                                      .FirstOrDefault(),
                CategoryID = pc.ProductType?.SubCategoryTypes
                                  .SelectMany(st => st.SubCategory!.CategorySubCategories)
                                  .Select(cs => cs.Category!.Id)
                                  .FirstOrDefault(),
                CategoryNameEn = pc.ProductType?.SubCategoryTypes
                                  .SelectMany(st => st.SubCategory!.CategorySubCategories)
                                  .Select(cs => cs.Category!.NameEn)
                                  .FirstOrDefault(),
                CategoryNameAr = pc.ProductType?.SubCategoryTypes
                                  .SelectMany(st => st.SubCategory!.CategorySubCategories)
                                  .Select(cs => cs.Category!.NameAr)
                                  .FirstOrDefault()
            }).ToList();

            return productDtos;
        }

        public async Task ApplyDiscountToProductSizes(Guid productId, CancellationToken cancellationToken)
        {
            string[] includes = { "ProductColors", "ProductColors.ProductSizes" };
            var product = await Find(p => p.Id == productId, false, includes);
            if (product == null)
                throw new Exception("Product not found");

            var discount = await GetDiscountByProductId(product.Id);
            if (discount <= 0) return;

            foreach (var color in product.ProductColors)
            {
                foreach (var size in color.ProductSizes)
                {
                    size.DiscountedPrice = size.Price * (1 - discount / 100);
                }
            }

            await SaveAsync(cancellationToken);
        }

        private async Task<decimal> GetDiscountByProductId(Guid? productId)
        {
            if (productId == Guid.Empty)
                return 0;

            var discount = await _context.Products
                .Where(p => p.Id == productId)
                .Select(p => p.ProductType)
                .SelectMany(pt => pt.SubCategoryTypes)
                .Select(sct => sct.SubCategory)
                .SelectMany(sc => sc.CategorySubCategories)
                .Select(cs => cs.Category)
                .SelectMany(c => c.DiscountCategories)
                .Where(dc => dc.IsActive && dc.StartDate <= DateTime.Now && dc.EndDate >= DateTime.Now)
                .Select(dc => dc.Discount.Percentage)
                .FirstOrDefaultAsync();

            return discount;
        }
    }
}