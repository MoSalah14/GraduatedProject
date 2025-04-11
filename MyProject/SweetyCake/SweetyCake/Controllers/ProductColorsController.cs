using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OutbornE_commerce.BAL.Dto.Currencies;
using OutbornE_commerce.BAL.Dto.ProductColors;
using OutbornE_commerce.BAL.Repositories.ProductColors;
using OutbornE_commerce.DAL.Models;
using OutbornE_commerce.FilesManager;
using System.Threading;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace OutbornE_commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductColorsController : ControllerBase
    {
        private readonly IProductColorRepository _productColorRepository;
        private readonly IFilesManager _filesManager;

        public ProductColorsController(
            IProductColorRepository productColorRepository,
            IFilesManager filesManager)
        {
            _productColorRepository = productColorRepository;
            _filesManager = filesManager;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllProductColors(Guid productId, int pageNumber = 1, int pageSize = 10)
        {
            var items = await _productColorRepository.FindAllAsyncByPagination(b => b.ProductId == productId, pageNumber, pageSize, new string[] { "Color", "ProductImages" });

            var data = items.Data.Adapt<List<ProductColorDto>>();

            return Ok(new PaginationResponse<List<ProductColorDto>>
            {
                Data = data,
                IsError = false,
                Status = (int)StatusCodeEnum.Ok,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = items.TotalCount
            });
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductColorById(Guid id)
        {
            var productColor = await _productColorRepository.Find(p => p.Id == id, true, new string[] { "Color", "ProductImages" });
            var data = productColor.Adapt<ProductColorDto>();
            return Ok(new Response<ProductColorDto>
            {
                Data = data,
                IsError = false,
                Message = "getting successfully",
                Status = (int)StatusCodeEnum.Ok
            });
        }
        /*  [HttpPost]
         public async Task<IActionResult> CreateProductColor([FromForm] ProductColorForCreateDto model,CancellationToken cancellationToken)
            {
                var productColor = model.Adapt<ProductDetails>();
                productColor.CreatedBy = "admin";
                productColor.ProductImages = new List<ProductImage>();
                if(model.Images != null)
                {
                   List<FileModel> images = await _filesManager.UploadMultipleFile(model.Images,"Products");
                    if (images == null)
                    {
                        return BadRequest(new Response<Guid>
                        {
                            IsError = true,
                            Message = "Invalid Images",
                            MessageAr = "برجاء ادخال الصور بطريقة صحيحة",
                            Status = (int)StatusCodeEnum.BadRequest
                        });
                    }

                    foreach (var image in images)
                    {
                        var prodImage = new ProductImage
                        {
                            CreatedBy = "admin",
                            ImageUrl = image.Url,
                            CreatedOn = DateTime.Now,
                        };
                        productColor.ProductImages.Add(prodImage);
                    }
                }

                var result =  await _productColorRepository.Create(productColor);
                await _productColorRepository.SaveAsync(cancellationToken);

                return Ok(new Response<Guid>
                {
                    Data = result.Id,
                    IsError = false,
                    Message = "",
                    Status = (int)StatusCodeEnum.Ok
                });
            }
            [HttpPut]
            public async Task<IActionResult> UpdateProductColor([FromForm]ProductColorDto model,CancellationToken cancellationToken)
            {
                var productColor = await _productColorRepository.Find(p=>p.Id == model.Id);
                 productColor = model.Adapt<ProductDetails>();
                //if (model.Images != null)
                //{
                //    List<FileModel> images = await _filesManager.UploadMultipleFile(model.Images, "Products");
                //    foreach (var image in images)
                //    {
                //        var prodImage = new ProductImage
                //        {
                //            CreatedBy = "admin",
                //            ImageUrl = image.Url,
                //            CreatedOn = DateTime.Now,
                //        };
                //        if (productColor.ProductImages is null)
                //            productColor.ProductImages = new List<ProductImage>();
                //        productColor.ProductImages.Add(prodImage);
                //    }
                //}
                //await _productImageRepository.DeleteRange(p=>p.ProductColorId == model.Id);
                productColor.UpdatedBy = "admin";
                productColor.CreatedBy = "admin";
                productColor.UpdatedOn = DateTime.Now;
                _productColorRepository.Update(productColor);
                await _productColorRepository.SaveAsync(cancellationToken);
                return Ok(new Response<Guid>
                {
                    Data = productColor.Id,
                    IsError = false,
                    Message = "",
                    Status = (int)StatusCodeEnum.Ok
                });
            }
            [HttpPost("UploadProductImage")]
            public async Task<IActionResult> UploadProductImage([FromQuery]Guid productColorId,[FromForm] FileToUploadDtoAPi file,CancellationToken cancellationToken)
            {
               var fileModel = await _filesManager.UploadFile(file.File, "Products");
                var image = new ProductImage
                {
                    ImageUrl = fileModel.Url,
                    CreatedBy = "admin",
                    ProductColorId = productColorId,    

                };
               var result= await _productImageRepository.Create(image);
                await _productColorRepository.SaveAsync(cancellationToken);
                return Ok(new Response<Guid>
                {
                    Data = result.Id,
                    IsError = false,
                    Message = "",
                    Status = (int)StatusCodeEnum.Ok
                });
            }
            [HttpDelete("DeleteProductImage/{id}")]
            public async Task<IActionResult> DeleteProductImage(Guid id, CancellationToken cancellationToken)
            {
                var image = await _productImageRepository.Find(c=>c.Id == id);
                 _productImageRepository.Delete(image);
                await _productImageRepository.SaveAsync(cancellationToken);
                return Ok(new Response<Guid>
                {
                    Data = id,
                    IsError = false,
                    Message = "",
                    Status = (int)StatusCodeEnum.Ok
                });
            }
            [HttpPut("SetIsDefault")]
            public async Task<IActionResult> SetIsDefault([FromQuery]Guid id, CancellationToken cancellationToken)
            {
                var oldDefaultColor = await _productColorRepository.Find(p=>p.IsDefault);
                if(oldDefaultColor != null)
                {
                    if (oldDefaultColor.Id == id)
                    {
                        return Ok(new Response<ProductDetails>
                        {
                            Data = null,
                            IsError = true,
                            Message = "This Already Default",
                            MessageAr = "هذا بالفعل الافتراضى",
                            Status = (int)StatusCodeEnum.BadRequest
                        });
                    }
                    oldDefaultColor.IsDefault = false;

                    _productColorRepository.Update(oldDefaultColor);
                    await _productColorRepository.SaveAsync(cancellationToken);


                }

                var newDefaultColor = await _productColorRepository.Find(p => p.Id == id);
                newDefaultColor!.IsDefault = true;

                _productColorRepository.Update(newDefaultColor);
                await  _productColorRepository.SaveAsync(cancellationToken);

                return Ok(new Response<dynamic> 
                {
                    Data = new { Id = id, IsDefault = newDefaultColor.IsDefault },
                    IsError = false,
                    Message = "",
                    Status = (int)StatusCodeEnum.Ok
                });
            }*/
    }
}
