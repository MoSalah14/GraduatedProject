using OutbornE_commerce.BAL.Dto.Products;
using OutbornE_commerce.BAL.Repositories.ProductCateories;
using OutbornE_commerce.BAL.Repositories.Products;
using OutbornE_commerce.BAL.Repositories.ProductSizes;
using OutbornE_commerce.FilesManager;
using Microsoft.EntityFrameworkCore;
using OutbornE_commerce.BAL.Repositories.UserReviews;
using OutbornE_commerce.BAL.Repositories.ProductColors;
using OutbornE_commerce.BAL.Repositories.Categories;
using OutbornE_commerce.BAL.Repositories.DiscountCategoryRepo;
using OutbornE_commerce.BAL.Repositories.CategorySubCategory;
using OutbornE_commerce.BAL.Repositories.SubCategoryTypeRepo;
using OutbornE_commerce.BAL.Repositories.ProductImageRepo;
using OutbornE_commerce.BAL.Repositories.WishList;
using OutbornE_commerce.BAL.Extentions;

namespace OutbornE_commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly IProductSizeRepository _productSizeRepository;
        private readonly IFilesManager _filesManager;
        private readonly IProductColorRepository productColorRepository;
        private readonly IProductImageRepositry ProductImageRepositry;

        public ProductsController(IProductImageRepositry productImageRepositry, IProductColorRepository productColorRepository,
                                  IProductRepository productRepository, IFilesManager filesManager,
                                  IProductSizeRepository productSizeRepository)
        {
            _productRepository = productRepository;
            _filesManager = filesManager;
            _productSizeRepository = productSizeRepository;
            ProductImageRepositry = productImageRepositry;
            this.productColorRepository = productColorRepository;
        }

        [HttpGet("GetAllProductsByTypeId/{id}")]
        public async Task<IActionResult> GetAllProductsByTypeId(Guid id, int pageNumber = 1, int pageSize = 10, [FromQuery] SortingCriteria? sortingCriteria = null)
        {
            // Call the service function
            var products = await _productRepository.GetProductsByTypeIdAsync(id, pageNumber, pageSize, sortingCriteria);

            if (products == null || !products.Any())
            {
                return NotFound(new Response<string>
                {
                    Data = null,
                    IsError = true,
                    Message = "No products found for the given type",
                    MessageAr = "لم يتم العثور على منتجات لهذا النوع",
                    Status = (int)StatusCodeEnum.NotFound
                });
            }

            return Ok(new Response<List<GetAllProductDto>>
            {
                Data = products,
                IsError = false,
                Message = "Products retrieved successfully",
                MessageAr = "تم جلب المنتجات بنجاح",
                Status = (int)StatusCodeEnum.Ok
            });
        }

        [HttpGet("GetAllProductsForUser")]
        public async Task<IActionResult> GetAllProductsForUser(int pageNumber = 1, int pageSize = 10, string? searchTerm = null, [FromQuery] SortingCriteria? sortingCriteria = null)
        {
            try
            {
                var ProductsResponse = _productRepository.GetAllProductInHomePage(searchTerm, pageNumber, pageSize, sortingCriteria);

                int TotalProductCount = await ProductsResponse.CountAsync();

                var ProductsWithPagination = await ProductsResponse.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

                return Ok(new PaginationResponse<List<GetAllProductForUserDto>>
                {
                    Data = ProductsWithPagination,
                    IsError = false,
                    Status = (int)StatusCodeEnum.Ok,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalCount = TotalProductCount
                });
            }
            catch (Exception ex)
            {
                return StatusCode((int)StatusCodeEnum.ServerError, new PaginationResponse<Exception>
                {
                    Data = ex,
                    IsError = true,
                    Status = (int)StatusCodeEnum.ServerError,
                });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(Guid id)
        {
            string[] includes = new string[] { "ProductColors.ProductSizes","Reviews", "Reviews.User", "ProductColors.Color", "ProductColors.ProductColorImages", "ProductColors.ProductSizes.Size", "Brand","PreOrderDetails",

            "ProductType.SubCategoryTypes.SubCategory.CategorySubCategories.Category"};

            var product = await _productRepository.Find(i => i.Id == id, false, includes);
            if (product == null)
            {
                return BadRequest(new Response<ProductDto>
                {
                    Data = null,
                    Message = "Invalid Product Id",
                    IsError = true,
                    Status = (int)StatusCodeEnum.NotFound
                });
            }

            var data = product.Adapt<ProductDto>();

            return Ok(new PaginationResponse<ProductDto>
            {
                Data = data,
                IsError = false,
                Status = (int)StatusCodeEnum.Ok,
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromForm] ProductForCreateDto model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid || model.ProductColors == null || !model.ProductColors.Any())
            {
                return BadRequest(new Response<Guid>
                {
                    Message = "All fields are required",
                    IsError = true,
                    MessageAr = "جميع الحقول مطلوبة",
                    Status = (int)StatusCodeEnum.BadRequest
                });
            }

            var firstProductColor = model.ProductColors.FirstOrDefault();
            if (firstProductColor != null && firstProductColor.ProductPhotos.Count > 10)
            {
                return BadRequest(new Response<Guid>
                {
                    Message = "Create_Photos cannot contain more than 10 items.",
                    MessageAr = "لا يمكن أن تحتوي الصور على أكثر من 10 عناصر.",
                    IsError = true,
                    Status = (int)StatusCodeEnum.BadRequest
                });
            }

            try
            {
                await _productRepository.BeginTransactionAsync();

                // Map product data
                var product = model.Adapt<Product>();
                product.CreatedBy = "admin";
                product.CreatedOn = DateTime.UtcNow;

                if (model.IsPreOrder && model.PreOrderDetails != null)
                {
                    var preOrderDetails = model.PreOrderDetails.Adapt<PreOrderDetails>();
                    preOrderDetails.ProductId = product.Id;
                    preOrderDetails.CreatedBy = "admin";
                    preOrderDetails.CreatedOn = DateTime.Now;
                    product.PreOrderDetails = preOrderDetails;
                }

                product.ProductColors = new List<ProductColor>();

                // Process product colors and sizes
                foreach (var detail in model.ProductColors)
                {
                    if (!detail.IsValid())
                    {
                        return BadRequest(new Response<Guid>
                        {
                            Message = "You cannot add more than 10 photos.",
                            IsError = true,
                            Status = (int)StatusCodeEnum.BadRequest
                        });
                    }

                    var productColor = detail.Adapt<ProductColor>();
                    productColor.CreatedOn = DateTime.Now;
                    productColor.CreatedBy = "admin";
                    productColor.ProductColorImages ??= new List<ProductColorImage>();

                    foreach (var photo in detail.ProductPhotos)
                    {
                        var fileModel = await _filesManager.UploadFile(photo.Photo, "Products");
                        if (fileModel != null)
                        {
                            var productColorImage = new ProductColorImage
                            {
                                ImageUrl = fileModel.Url,
                                IsDefault = photo.IsDefault,
                                CreatedOn = DateTime.UtcNow,
                            };

                            productColor.ProductColorImages.Add(productColorImage);
                        }
                    }

                    productColor.ProductSizes = new List<ProductSize>();
                    foreach (var productSize in detail.ProductSizes)
                    {
                        var newProductSize = productSize.Adapt<ProductSize>();

                        if (productSize.Quantity > 0 && model.IsPreOrder)
                        {
                            return BadRequest(new Response<string>
                            {
                                MessageAr = "لا يمكن تحديد الكميات للمنتجات المطلوبة مسبقًا",
                                Message = "Cannot specify quantities for pre-order products.",
                                IsError = true,
                                Status = (int)StatusCodeEnum.BadRequest
                            });
                        }
                        else if (productSize.Quantity == 0 && !model.IsPreOrder)
                        {
                            return BadRequest(new Response<Guid>
                            {
                                MessageAr = " الكميات يجب أن تكون أكثر من 0",
                                Message = "Quantities must be more than 0",
                                IsError = true,
                                Status = (int)StatusCodeEnum.BadRequest
                            });
                        }

                        newProductSize.CreatedOn = DateTime.Now;
                        newProductSize.CreatedBy = "admin";

                        productColor.ProductSizes.Add(newProductSize);
                    }

                    product.ProductColors.Add(productColor);
                }

                await _productRepository.Create(product);

                await _productRepository.SaveAsync(cancellationToken);
                await _productRepository.CommitTransactionAsync();

                await _productRepository.ApplyDiscountToProductSizes(product.Id, cancellationToken);

                return Ok(new Response<Guid>
                {
                    Data = product.Id,
                    IsError = false,
                    Status = (int)StatusCodeEnum.Ok,
                    Message = "Added Successfully",
                    MessageAr = "تم الاضافة بنجاح"
                });
            }
            catch (Exception ex)
            {
                await _productRepository.RollbackTransactionAsync();

                return BadRequest(new Response<Guid>
                {
                    Message = ex.InnerException?.Message ?? ex.Message,
                    IsError = true,
                    MessageAr = ex.Message,
                    Status = (int)StatusCodeEnum.BadRequest
                });
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProduct([FromForm] ProductForCreateDto model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid || model.ProductColors == null || !model.ProductColors.Any())
            {
                return BadRequest(new Response<Guid>
                {
                    Message = "All fields are required",
                    IsError = true,
                    MessageAr = "جميع الحقول مطلوبة",
                    Status = (int)StatusCodeEnum.BadRequest
                });
            }

            try
            {
                await _productRepository.BeginTransactionAsync();

                // Fetch the existing product along with related entities
                string[] includes = {
            "ProductType",
            "ProductColors",
            "ProductColors.Color",
            "ProductColors.ProductColorImages",
            "ProductColors.ProductSizes",
            "PreOrderDetails"
        };

                var existingProduct = await _productRepository.Find(x => x.Id == model.Id, true, includes);
                if (existingProduct == null)
                {
                    return NotFound(new Response<Guid>
                    {
                        Message = "Product not found",
                        IsError = true,
                        Status = (int)StatusCodeEnum.NotFound
                    });
                }

                existingProduct = model.Adapt(existingProduct);
                existingProduct.UpdatedBy = "admin";
                existingProduct.UpdatedOn = DateTime.UtcNow;

                // Handle PreOrderDetails
                if (model.IsPreOrder && model.PreOrderDetails != null)
                {
                    var preOrderDetails = model.PreOrderDetails.Adapt(existingProduct.PreOrderDetails ?? new PreOrderDetails());
                    preOrderDetails.ProductId = existingProduct.Id;
                    preOrderDetails.UpdatedBy = "admin";
                    preOrderDetails.UpdatedOn = DateTime.UtcNow;
                    existingProduct.PreOrderDetails = preOrderDetails;
                }
                else if (!model.IsPreOrder)
                {
                    existingProduct.PreOrderDetails = null;
                }

                // Clear existing product colors and related entities only if no old URLs are provided
                if (existingProduct.ProductColors != null)
                {
                    foreach (var color in existingProduct.ProductColors.ToList())
                    {
                        if (color.ProductColorImages != null)
                        {
                            foreach (var image in color.ProductColorImages.ToList())
                            {
                                if (model.ProductPhotosUrl == null || !model.ProductPhotosUrl.Any(img => img.ImageUrl == image.ImageUrl))
                                {
                                    _filesManager.DeleteFile(image.ImageUrl);
                                    ProductImageRepositry.Delete(image);
                                }
                            }
                        }

                        if (color.ProductSizes != null)
                        {
                            foreach (var size in color.ProductSizes.ToList())
                            {
                                _productSizeRepository.Delete(size);
                            }
                        }

                        productColorRepository.Delete(color);
                    }
                }

                // Add new product colors and handle images
                foreach (var colorDto in model.ProductColors)
                {
                    var productColor = colorDto.Adapt<ProductColor>();
                    productColor.ProductId = existingProduct.Id;
                    productColor.UpdatedOn = DateTime.UtcNow;
                    productColor.UpdatedBy = "admin";
                    productColor.ProductColorImages = new List<ProductColorImage>();

                    if (colorDto.ProductPhotos != null)
                    {
                        foreach (var photo in colorDto.ProductPhotos)
                        {
                            var fileModel = await _filesManager.UploadFile(photo.Photo, "Products");
                            if (fileModel != null)
                            {
                                var productColorImage = new ProductColorImage
                                {
                                    ImageUrl = fileModel.Url,
                                    IsDefault = photo.IsDefault,
                                    CreatedOn = DateTime.UtcNow
                                };
                                productColor.ProductColorImages.Add(productColorImage);
                            }
                        }
                    }
                    //// Add existing images from URLs if provided
                    if (model.ProductPhotosUrl != null)
                    {
                        foreach (var photoUrl in model.ProductPhotosUrl)
                        {
                            if (!productColor.ProductColorImages.Any(img => img.ImageUrl == photoUrl.ImageUrl))
                            {
                                productColor.ProductColorImages.Add(new ProductColorImage
                                {
                                    ImageUrl = photoUrl.ImageUrl,
                                    IsDefault = photoUrl.IsDefault,
                                    CreatedOn = DateTime.UtcNow
                                });
                            }
                        }
                    }

                    productColor.ProductSizes = new List<ProductSize>();
                    foreach (var sizeDto in colorDto.ProductSizes)
                    {
                        if (model.IsPreOrder && sizeDto.Quantity > 0)
                        {
                            return BadRequest(new Response<Guid>
                            {
                                MessageAr = "لا يمكن تحديد الكميات للمنتجات المطلوبة مسبقًا",
                                Message = "Cannot specify quantities for pre-order products.",
                                IsError = true,
                                Status = (int)StatusCodeEnum.BadRequest
                            });
                        }

                        var productSize = sizeDto.Adapt<ProductSize>();
                        productSize.ProductColorId = productColor.Id;
                        productSize.UpdatedOn = DateTime.UtcNow;
                        productSize.UpdatedBy = "admin";

                        productColor.ProductSizes.Add(productSize);
                    }

                    existingProduct.ProductColors.Add(productColor);
                }

                // Update product in the repository
                _productRepository.Update(existingProduct);
                await _productRepository.SaveAsync(cancellationToken);
                await _productRepository.CommitTransactionAsync();

                // Apply any necessary discount logic
                await _productRepository.ApplyDiscountToProductSizes(existingProduct.Id, cancellationToken);

                return Ok(new Response<Guid>
                {
                    Data = existingProduct.Id,
                    IsError = false,
                    Status = (int)StatusCodeEnum.Ok,
                    Message = "Product updated successfully.",
                    MessageAr = "تم تحديث المنتج بنجاح."
                });
            }
            catch (Exception ex)
            {
                await _productRepository.RollbackTransactionAsync();

                return BadRequest(new Response<Guid>
                {
                    Message = $"Error: {ex.Message}, Inner: {ex.InnerException?.Message}",
                    IsError = true,
                    MessageAr = "حدث خطأ أثناء تحديث المنتج.",
                    Status = (int)StatusCodeEnum.BadRequest
                });
            }
        }

        [HttpPost("search")]
        public async Task<IActionResult> SearchProducts([FromBody] SearchModelDto model, [FromQuery] SortingCriteria? sortingCriteria = null)
        {
            var products = await _productRepository.SearchProducts(model, sortingCriteria);

            var data = products.Data.Adapt<List<GetAllProductForUserDtoًWithCategory>>();

            return Ok(new PaginationResponse<List<GetAllProductForUserDtoًWithCategory>>
            {
                Data = data,
                IsError = false,
                Status = (int)StatusCodeEnum.Ok,
                PageNumber = model.PageNumber,
                PageSize = model.PageSize,
                TotalCount = products.TotalCount
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                string[] includes = new string[]
                {
            "ProductColors",
            "ProductColors.ProductSizes",
            "ProductColors.ProductSizes.OrderItems",
            "Reviews",
            "WishLists"
                };

                var product = await _productRepository.Find(p => p.Id == id, false, includes);

                if (product == null)
                {
                    return Ok(new Response<ProductDto>
                    {
                        Data = null,
                        Message = "Invalid Product Id",
                        MessageAr = "هذا المنتج غير موجود",
                        IsError = true,
                        Status = (int)StatusCodeEnum.NotFound
                    });
                }

                var hasOrderItems = product.ProductColors.Any(pc => pc.ProductSizes
                        .Any(ps => ps.OrderItems != null && ps.OrderItems.Any()));

                if (hasOrderItems)
                {
                    return Ok(new Response<Guid>
                    {
                        Data = id,
                        Message = "The product cannot be deleted because it is connected to existing orders.",
                        MessageAr = "لا يمكن حذف المنتج لأنه مرتبط بطلبات موجودة.",
                        IsError = true,
                        Status = (int)StatusCodeEnum.BadRequest
                    });
                }

                foreach (var productColor in product.ProductColors)
                {
                    productColor.IsDeleted = true;
                    foreach (var productSize in productColor.ProductSizes)
                    {
                        productSize.IsDeleted = true;
                    }
                }

                product.IsActive = false;
                product.IsDeleted = true;
                _productRepository.Update(product);

                await _productRepository.SaveAsync(cancellationToken);

                return Ok(new Response<Guid>
                {
                    Message = "Deleted Successfully",
                    MessageAr = "تم الحذف بنجاح",
                    Data = id,
                    IsError = false,
                    Status = (int)StatusCodeEnum.Ok
                });
            }
            catch (Exception ex)
            {
                return StatusCode((int)StatusCodeEnum.ServerError, new Response<string>
                {
                    Data = null,
                    Message = "An error occurred while deleting the product",
                    MessageAr = "حدث خطأ أثناء حذف المنتج",
                    IsError = true,
                    Status = (int)StatusCodeEnum.ServerError
                });
            }
        }

        [HttpGet("bestSeller")]
        public async Task<IActionResult> GetBestSellerProducts(string searchTerm = null, int pageNumber = 1, int pageSize = 10)
        {
            var products = _productRepository.GetAllBestSellerProduct(searchTerm, pageNumber, pageSize);
            int ProductCounts = await products.CountAsync();
            var ProductQuery = await products.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            var data = ProductQuery.Adapt<List<GetAllProductForUserDto>>();

            return Ok(new PaginationResponse<List<GetAllProductForUserDto>>()
            {
                PageSize = pageSize,
                TotalCount = ProductCounts,
                PageNumber = pageNumber,
                Data = data,
                IsError = false,
                Status = (int)StatusCodeEnum.Ok
            });
        }

        [HttpGet("GetProductsBySubCategoryId/{SubCategoryId}")]
        public async Task<IActionResult> GetProductsBySubCategory(Guid SubCategoryId, int pageNumber = 1, int pageSize = 10, [FromQuery] SortingCriteria? sortingCriteria = null)
        {
            if (SubCategoryId == Guid.Empty)
                return BadRequest("Invalid category ID.");

            var response = await _productRepository.GetProductsBySubCategoryAsync(SubCategoryId, pageNumber, pageSize, sortingCriteria);

            if (response is null)
            {
                return NotFound(new Response<Guid>
                {
                    Data = SubCategoryId,
                    IsError = true,
                    Message = "Not Found This Category",
                    MessageAr = " لم يتم العثور عليه",
                    Status = (int)StatusCodeEnum.NotFound
                });
            }

            return Ok(new PaginationResponse<List<GetAllProductDto>>()
            {
                Data = response.Data,
                IsError = false,
                Status = (int)StatusCodeEnum.Ok
            });
        }

        [HttpGet("GetProductsBySuperCategory/{SuperCategoryId}")]
        public async Task<IActionResult> GetProductsBySuperCategory(Guid SuperCategoryId, int pageNumber = 1, int pageSize = 10, [FromQuery] SortingCriteria? sortingCriteria = null)
        {
            if (SuperCategoryId == Guid.Empty)
                return BadRequest("Invalid category ID.");

            var response = await _productRepository.GetProductsByCategoryAsync(SuperCategoryId, pageNumber, pageSize, sortingCriteria);

            if (response is null)
            {
                return NotFound(new Response<Guid>
                {
                    Data = SuperCategoryId,
                    IsError = true,
                    Message = "Not Found This Category",
                    MessageAr = " لم يتم العثور عليه",
                    Status = (int)StatusCodeEnum.NotFound
                });
            }

            //return Ok(new PaginationResponse<List<GetAllProductForUserDto>>()
            //{
            //    Data = response.Data,
            //    IsError = false,
            //    Status = (int)StatusCodeEnum.Ok
            //});
            return Ok(response);
        }

        [HttpGet("Brand/{BrandId}")]
        public async Task<IActionResult> GetProductByBrandId(Guid BrandId, int pageNumber = 1, int pageSize = 10, [FromQuery] SortingCriteria? sortingCriteria = null)
        {
            if (BrandId == Guid.Empty)
            {
                return BadRequest("Invalid BrandId ID");
            }

            var response = await _productRepository.GetProductsByBrandIdAsync(BrandId, pageNumber, pageSize, sortingCriteria);

            return Ok(new PaginationResponse<List<GetAllProductDto>>()
            {
                Data = response.Data,
                IsError = false,
                Status = (int)StatusCodeEnum.Ok
            });
        }

        [HttpGet("peopleAlsoBought")]
        public async Task<IActionResult> GetPeopleAlsoBoughtProducts(int pageNumber = 1, int pageSize = 10)
        {
            string[] includes = new string[] { "ProductColors.ProductSizes", "ProductColors.ProductColorImages" };

            var products = await _productRepository.FindAllAsyncByPagination(p => p.IsPeopleAlseBought, pageNumber, pageSize, includes);

            var data = products.Data.Select(src => new ProductCardDto
            {
                ImageUrl = src.ProductColors.SelectMany(e => e.ProductColorImages).FirstOrDefault(e => e.IsDefault)?.ImageUrl,
                Id = src.Id,
                NameAr = src.NameAr,
                NameEn = src.NameEn,
                Price = src.ProductColors
                                .Where(pc => pc.ProductSizes.Any())
                                .SelectMany(pc => pc.ProductSizes)
                                .DefaultIfEmpty(new ProductSize { Price = 0 })
                                .Min(ps => ps.Price)
            }).ToList();

            return Ok(new Response<List<ProductCardDto>>()
            {
                Data = data,
                IsError = false,
                Status = (int)StatusCodeEnum.Ok
            });
        }
    }
}