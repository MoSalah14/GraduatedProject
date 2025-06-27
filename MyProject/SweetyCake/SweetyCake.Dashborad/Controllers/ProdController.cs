using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OutbornE_commerce.BAL.Dto.Products;
using OutbornE_commerce.BAL.Dto;
using OutbornE_commerce.BAL.Extentions;
using OutbornE_commerce.BAL.Repositories.ProductImageRepo;
using OutbornE_commerce.BAL.Repositories.Products;
using OutbornE_commerce.DAL.Enums;
using OutbornE_commerce.FilesManager;
using OutbornE_commerce.BAL.Dto.Categories;
using Mapster;
using OutbornE_commerce.DAL.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using OutbornE_commerce.BAL.Repositories.Categories;

namespace SweetyCake.Dashborad.Controllers
{
    public class ProdController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly IFilesManager _filesManager;
        private readonly IProductImageRepositry ProductImageRepositry;
        private readonly ICategoryRepository _categoryRepository;

        public ProdController(IProductImageRepositry productImageRepositry,
            IProductRepository productRepository, IFilesManager filesManager, ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _filesManager = filesManager;
            ProductImageRepositry = productImageRepositry;
            _categoryRepository = categoryRepository;
        }

        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 100000, string? searchTerm = null, [FromQuery] SortingCriteria? sortingCriteria = null)
        {
            try
            {
                var ProductsResponse = _productRepository.GetAllProductInHomePage(searchTerm, pageNumber, pageSize, null, sortingCriteria);

                int TotalProductCount = await ProductsResponse.CountAsync();

                var ProductsWithPagination = await ProductsResponse.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

                if (ProductsWithPagination == null)
                {
                    return BadRequest();
                }
                return View(ProductsWithPagination);

            }
            catch (Exception ex)
            {
                return BadRequest();
            }

        }

        public async Task<IActionResult> Details(Guid id)
        {
            string[] includes = new string[] { "Category", "Reviews.User", "ProductImage" };

            var product = await _productRepository.Find(i => i.Id == id, false, includes);
            if (product == null)
                return BadRequest();


            var data = product.Adapt<ProductDto>();

            data.ProductImage = product.ProductImage
                .Select(img => img.ImageUrl)
                .ToList();

            data.CategoryNameEn = product.Category.NameEn;
            data.CategoryNameAr = product.Category.NameAr;

            return View(data);

        }

        public async Task<IActionResult> Create()
        {
            var categories = await _categoryRepository.FindAllAsync(null);
            if (categories != null)
            {
                ViewBag.CategoryEn = new SelectList(categories, "Id", "NameEn");
                ViewBag.CategoryAr = new SelectList(categories, "Id", "NameAr");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] ProductForCreateDto model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();

            }

            if (model != null && model.ProductImages.Count > 10)
            {
                return BadRequest();

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


                return RedirectToAction("Index");

            }
            catch (Exception ex)
            {
                await _productRepository.RollbackTransactionAsync();

                return BadRequest();

            }
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var categories = await _categoryRepository.FindAllAsync(null);
            if (categories != null)
            {
                ViewBag.CategoryEn = new SelectList(categories, "Id", "NameEn");
                ViewBag.CategoryAr = new SelectList(categories, "Id", "NameAr");
            }

            string[] includes = new string[] { "Category", "Reviews.User", "ProductImage" };

            var product = await _productRepository.Find(i => i.Id == id, false, includes);
            if (product == null)
                return NotFound();

            // Map product to ProductForUpdateDto
            var productDto = new ProductForUpdateDto
            {
                Id = product.Id,
                NameEn = product.NameEn,
                NameAr = product.NameAr,
                AboutEn = product.AboutEn,
                AboutAr = product.AboutAr,
                MaterialEn = product.MaterialEn,
                MaterialAr = product.MaterialAr,
                QuantityInStock = product.QuantityInStock,
                Price = product.Price,
                DiscountPrice = product.DiscountPrice,
                IsPreOrder = product.IsPreOrder,
                IsActive = product.IsActive,
                CategoryId = product.CategoryId,
                MainImagesUrl = product.MainImageUrl,
                ImagesUrl = product.ProductImage?.Select(img => img.ImageUrl).ToList()
            };

            // Pass image data to ViewBag
            ViewBag.CurrentMainImage = productDto.MainImagesUrl;
            ViewBag.CurrentImages = productDto.ImagesUrl;

            return View(productDto);
        }




        [HttpPost]
        public async Task<IActionResult> Edit([FromForm] ProductForUpdateDto model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            try
            {
                await _productRepository.BeginTransactionAsync();

                string[] includes = { "Category", "Reviews", "ProductImage" };

                var existingProduct = await _productRepository.Find(x => x.Id == model.Id, true, includes);
                if (existingProduct == null)
                {
                    return NotFound();

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

                return RedirectToAction("Index");

            }
            catch (Exception ex)
            {
                await _productRepository.RollbackTransactionAsync();

                return BadRequest();

            }
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            var Product = await _productRepository.Find(c => c.Id == id, true);
            var ProductDto = Product.Adapt<ProductDto>();

            if (ProductDto == null)
                return NotFound();


            return View(ProductDto);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                string[] includes = new string[] { "Category", "Reviews.User", "ProductImage" };

                var product = await _productRepository.Find(i => i.Id == id, false, includes);
                if (product == null)
                    return BadRequest();

                product.IsActive = false;
                product.IsDeleted = true;
                _productRepository.Delete(product);

                await _productRepository.SaveAsync(cancellationToken);

                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                return BadRequest();

            }
        }

    }
}
