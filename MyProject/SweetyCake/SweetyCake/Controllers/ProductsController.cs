using OutbornE_commerce.BAL.Dto.Products;
using OutbornE_commerce.BAL.Repositories.Products;
using OutbornE_commerce.FilesManager;
using Microsoft.EntityFrameworkCore;
using OutbornE_commerce.BAL.Repositories.ProductImageRepo;
using OutbornE_commerce.BAL.Extentions;
using OutbornE_commerce.DAL.Models;

namespace OutbornE_commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly IFilesManager _filesManager;
        private readonly IProductImageRepositry ProductImageRepositry;

        public ProductsController(IProductImageRepositry productImageRepositry, IProductRepository productRepository, IFilesManager filesManager)
        {
            _productRepository = productRepository;
            _filesManager = filesManager;
            ProductImageRepositry = productImageRepositry;
        }


        [HttpGet("GetAllProductsForUser")]
        public async Task<IActionResult> GetAllProductsForUser(int pageNumber = 1, int pageSize = 10, string? searchTerm = null, [FromQuery] SortingCriteria? sortingCriteria = null, Guid? CategoryId = null)
        {
            try
            {
                var ProductsResponse = _productRepository.GetAllProductInHomePage(searchTerm, pageNumber, pageSize, sortingCriteria, CategoryId);

                int TotalProductCount = await ProductsResponse.CountAsync();

                var ProductsWithPagination = await ProductsResponse.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

                return Ok(new PaginationResponse<List<GetAllProductForUserDtoWithCategory>>
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
            string[] includes = new string[] { "Category", "Reviews.User", "ProductImage" };

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

            data.ProductImage = product.ProductImage
                .Select(img => img.ImageUrl)
                .ToList();

            data.CategoryNameEn = product.Category.NameEn;
            data.CategoryNameAr = product.Category.NameAr;

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
            if (!ModelState.IsValid)
            {
                return BadRequest(new Response<Guid>
                {
                    Message = "All fields are required",
                    IsError = true,
                    MessageAr = "جميع الحقول مطلوبة",
                    Status = (int)StatusCodeEnum.BadRequest
                });
            }

            if (model != null && model.ProductImages.Count > 10)
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


                var uploadedFile = await _filesManager.UploadFile(model.MainImagePhoto, "Products");
                if (uploadedFile is not null)
                {
                    product.MainImageUrl = uploadedFile.Url;
                }

                foreach (var image in model.ProductImages)
                {
                    var fileModel = await _filesManager.UploadFile(image, "Products");
                    if (fileModel != null)
                    {
                        var productColorImage = new ProductImage
                        {
                            ImageUrl = fileModel.Url,
                            ProductId = product.Id,
                        };
                        product.ProductImage.Add(productColorImage);
                    }
                }

                await _productRepository.Create(product);

                await _productRepository.SaveAsync(cancellationToken);
                await _productRepository.CommitTransactionAsync();


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
        public async Task<IActionResult> UpdateProduct([FromForm] ProductForUpdateDto model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
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

                string[] includes = { "Category", "Reviews", "ProductImage" };

                var existingProduct = await _productRepository.Find(x => x.Id == model.Id, true, includes);
                if (existingProduct == null)
                {
                    return NotFound(new Response<Guid>
                    {
                        Message = "Product not found",
                        MessageAr = "المنتج غير موجود",
                        IsError = true,
                        Status = (int)StatusCodeEnum.NotFound
                    });
                }

                existingProduct = model.Adapt(existingProduct);

                #region Handle Photos


                // 1. Handle Product Images (Gallery)
                var existingImages = existingProduct.ProductImage.ToList();

                // If user sent no new files and no URLs => delete all old images
                bool hasNoNewImages = model.ProductImages == null || !model.ProductImages.Any();
                bool hasNoImageUrls = model.ImagesUrl == null || !model.ImagesUrl.Any();

                if (hasNoNewImages && hasNoImageUrls)
                {
                    foreach (var image in existingImages)
                    {
                        _filesManager.DeleteFile(image.ImageUrl);
                        ProductImageRepositry.Delete(image);
                    }
                }
                else
                {
                    // Delete any image that's not in the submitted URLs
                    foreach (var image in existingImages)
                    {
                        if (model.ImagesUrl == null || !model.ImagesUrl.Contains(image.ImageUrl))
                        {
                            _filesManager.DeleteFile(image.ImageUrl);
                            ProductImageRepositry.Delete(image);
                        }
                    }

                    // Add new uploaded images (if any)
                    if (model.ProductImages != null && model.ProductImages.Any())
                    {
                        foreach (var file in model.ProductImages)
                        {
                            var uploaded = await _filesManager.UploadFile(file, "Products");
                            if (uploaded != null)
                            {
                                var productImage = new ProductImage
                                {
                                    ImageUrl = uploaded.Url,
                                    ProductId = existingProduct.Id
                                };

                                existingProduct.ProductImage.Add(productImage);
                            }
                        }
                    }
                }

                // 2. Handle Main Image
                if (model.MainImagePhoto != null)
                {
                    // Upload the new main image
                    var fileModel = await _filesManager.UploadFile(model.MainImagePhoto, "Products");
                    if (fileModel != null)
                    {
                        // Delete old main image
                        if (!string.IsNullOrEmpty(existingProduct.MainImageUrl))
                        {
                            _filesManager.DeleteFile(existingProduct.MainImageUrl);
                        }

                        // Set new main image URL
                        existingProduct.MainImageUrl = fileModel.Url;
                    }
                }
                else if (string.IsNullOrEmpty(model.MainImagesUrl))
                {
                    // No main image was submitted at all => delete existing one
                    if (!string.IsNullOrEmpty(existingProduct.MainImageUrl))
                    {
                        _filesManager.DeleteFile(existingProduct.MainImageUrl);
                        existingProduct.MainImageUrl = null;
                    }
                }
                else
                {
                    // MainImagesUrl was submitted => keep it as is
                    existingProduct.MainImageUrl = model.MainImagesUrl;
                }
                #endregion

                // Update product in the repository
                _productRepository.Update(existingProduct);
                await _productRepository.SaveAsync(cancellationToken);
                await _productRepository.CommitTransactionAsync();

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

            var data = products.Data.Adapt<List<GetAllProductForUserDtoWithCategory>>();

            return Ok(new PaginationResponse<List<GetAllProductForUserDtoWithCategory>>
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
                string[] includes = { "Category", "PreOrderDetails", "Reviews", "ProductImage" };


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
            catch (Exception)
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

        [HttpGet("flashsale")]
        public async Task<IActionResult> GetFlashSaleProducts(int flashsaleNumber)
        {
            var products = await _productRepository.GetFlashSaleProductsAsync(flashsaleNumber);


            var data = products.Adapt<List<GetAllProductForUserDto>>();

            return Ok(new PaginationResponse<List<GetAllProductForUserDto>>()
            {
                PageSize = 0,
                TotalCount = flashsaleNumber,
                PageNumber = 1,
                Data = data,
                IsError = false,
                Status = (int)StatusCodeEnum.Ok
            });
        }


        [HttpGet("GetProductsByCategory/{CategoryId}")]
        public async Task<IActionResult> GetProductsByCategory(Guid CategoryId, int pageNumber = 1, int pageSize = 10, [FromQuery] SortingCriteria? sortingCriteria = null)
        {
            if (CategoryId == Guid.Empty)
                return BadRequest("Invalid category ID.");

            var response = await _productRepository.GetProductsByCategoryAsync(CategoryId, pageNumber, pageSize, sortingCriteria);

            if (response is null)
            {
                return NotFound(new Response<Guid>
                {
                    Data = CategoryId,
                    IsError = true,
                    Message = "Not Found This Category",
                    MessageAr = " لم يتم العثور عليه",
                    Status = (int)StatusCodeEnum.NotFound
                });
            }
            return Ok(response);
        }


        [HttpGet("newArrivale")]
        public async Task<IActionResult> GetNewArrivaleProducts()
        {
            var products = await _productRepository.GetNewArrivaleProductsAsync();


            var data = products.Adapt<List<GetAllProductForUserDto>>();

            return Ok(new PaginationResponse<List<GetAllProductForUserDto>>()
            {
                PageSize = 0,
                TotalCount = 4,
                PageNumber = 1,
                Data = data,
                IsError = false,
                Status = (int)StatusCodeEnum.Ok
            });
        }
    }
}