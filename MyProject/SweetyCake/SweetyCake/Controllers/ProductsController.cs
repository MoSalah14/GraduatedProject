using OutbornE_commerce.BAL.Dto.Products;
using OutbornE_commerce.BAL.Repositories.Products;
using OutbornE_commerce.FilesManager;
using Microsoft.EntityFrameworkCore;
using OutbornE_commerce.BAL.Repositories.ProductImageRepo;
using OutbornE_commerce.BAL.Extentions;

namespace OutbornE_commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly IFilesManager _filesManager;
        private readonly IProductImageRepositry ProductImageRepositry;

        public ProductsController(IProductImageRepositry productImageRepositry,
                                  IProductRepository productRepository, IFilesManager filesManager
                                  )
        {
            _productRepository = productRepository;
            _filesManager = filesManager;
            ProductImageRepositry = productImageRepositry;
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
            string[] includes = new string[] { "Category", "PreOrderDetails", "Reviews", "ProductImage" };

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

                // Process product colors and sizes
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
        public async Task<IActionResult> UpdateProduct([FromForm] ProductForCreateDto model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid || !model.ProductImages.Any())
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
                string[] includes = { "Category", "PreOrderDetails", "Reviews", "ProductImage" };

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

                // Clear existing photos
                foreach (var image in existingProduct.ProductImage.ToList())
                {
                    _filesManager.DeleteFile(image.ImageUrl);
                    ProductImageRepositry.Delete(image);
                }

                // Add new product colors and handle images
                foreach (var productImage in model.ProductImages)
                {
                    existingProduct.ProductImage = new List<ProductImage>();

                    var fileModel = await _filesManager.UploadFile(productImage, "Products");
                    if (fileModel != null)
                    {
                        var productColorImage = new ProductImage
                        {
                            ImageUrl = fileModel.Url,
                            ProductId = existingProduct.Id,
                        };
                        existingProduct.ProductImage.Add(productColorImage);
                    }
                }

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

        //[HttpGet("bestSeller")]
        //public async Task<IActionResult> GetBestSellerProducts(string searchTerm = null, int pageNumber = 1, int pageSize = 10)
        //{
        //    var products = _productRepository.GetAllBestSellerProduct(searchTerm, pageNumber, pageSize);
        //    int ProductCounts = await products.CountAsync();
        //    var ProductQuery = await products.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

        //    var data = ProductQuery.Adapt<List<GetAllProductForUserDto>>();

        //    return Ok(new PaginationResponse<List<GetAllProductForUserDto>>()
        //    {
        //        PageSize = pageSize,
        //        TotalCount = ProductCounts,
        //        PageNumber = pageNumber,
        //        Data = data,
        //        IsError = false,
        //        Status = (int)StatusCodeEnum.Ok
        //    });
        //}


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
            return Ok(response);
        }
    }
}