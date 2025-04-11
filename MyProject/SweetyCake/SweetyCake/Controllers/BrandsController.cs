using Mapster;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OutbornE_commerce.BAL.Dto;
using OutbornE_commerce.BAL.Dto.Brands;
using OutbornE_commerce.BAL.Dto.Categories;
using OutbornE_commerce.BAL.EmailServices;
using OutbornE_commerce.BAL.Repositories.Brands;
using OutbornE_commerce.BAL.Repositories.Categories;
using OutbornE_commerce.DAL.Enums;
using OutbornE_commerce.DAL.Models;
using OutbornE_commerce.FilesManager;
using System.Drawing.Drawing2D;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace OutbornE_commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandsController : ControllerBase
    {
        private readonly IBrandRepository _brandRepository;
        private readonly IFilesManager _filesManager;

        public IHostEnvironment Environment { get; }
        public IWebHostEnvironment webHostEnvironment { get; }
        public IEmailSenderCustom _emailSender { get; }
        private readonly UserManager<User> _userManager;

        public BrandsController(UserManager<User> userManager,IHostEnvironment _env,IWebHostEnvironment env,IEmailSenderCustom emailSender,IFilesManager filesManager, IBrandRepository brandRepository)
        {
            Environment = _env;
            webHostEnvironment = env;
            _emailSender = emailSender;
            _filesManager = filesManager;
            _brandRepository = brandRepository;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBrands(int pageNumber = 1, int pageSize = 10, string? searchTerm = null)
        {
            try
            {
                var brands = new PagainationModel<IEnumerable<Brand>>();

                if (string.IsNullOrEmpty(searchTerm))
                {
                    brands = await _brandRepository.FindAllAsyncByPagination(null, pageNumber, pageSize);
                }
                else
                {
                    brands = await _brandRepository.FindAllAsyncByPagination(
                        b => (b.NameAr != null && b.NameAr.Contains(searchTerm)) ||
                             (b.NameEn != null && b.NameEn.Contains(searchTerm)) ||
                             (b.DescriptionAr != null && b.DescriptionAr.Contains(searchTerm)) ||
                             (b.DescriptionEn != null && b.DescriptionEn.Contains(searchTerm)),
                        pageNumber,
                        pageSize
                    );
                }

                var data = brands.Data?.Adapt<List<BrandDto>>() ?? new List<BrandDto>();

                return Ok(new PaginationResponse<List<BrandDto>>
                {
                    Data = data,
                    IsError = false,
                    Status = (int)StatusCodeEnum.Ok,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalCount = brands.TotalCount
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAllBrands: {ex.Message}");

                // You can return a custom error response without unsupported properties like TargetSite.
                return StatusCode(500, new
                {
                    Message = "An error occurred while processing your request.",
                    Details = ex.Message // You can also include a more detailed message if needed
                });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBrandById(Guid id)
        {
            var brand = await _brandRepository.Find(c => c.Id == id, false);
            if (brand == null)
            {
                return Ok(new Response<BrandDto>
                {
                    Data = null,
                    IsError = true,
                    Message = "Not Found",
                    MessageAr = "لا يوجد بيانات بخصوص هذا البراند",
                    Status = (int)StatusCodeEnum.NotFound
                });
            }
            var data = brand.Adapt<BrandDto>();
            return Ok(new Response<BrandDto>
            {
                Data = data,
                IsError = false,
                Message = "Get Brands successfully",
                MessageAr = "الحصول على العلامات التجارية بنجاح",
                Status = (int)StatusCodeEnum.Ok
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateBrand([FromForm] BrandDto model, CancellationToken cancellationToken)
        {
            var brand = model.Adapt<Brand>();
            brand.CreatedBy = "admin";
            if (model.ImageHome != null)
            {
                var fileModel = await _filesManager.UploadFile(model.ImageHome, "Brands");
                brand.ImageUrl = fileModel!.Url;
            }
            if (model.ImageBaner != null)
            {
                var fileModel = await _filesManager.UploadFile(model.ImageBaner, "Brands");
                brand.ImageBanerUrl = fileModel!.Url;
            }
            if (model.BrandSizeChart != null)
            {
                var fileModel = await _filesManager.UploadFile(model.BrandSizeChart, "Brands");
                brand.BrandSizeChartUrl = fileModel!.Url;
            }
            var result = await _brandRepository.Create(brand);

            var lastBrand = await _brandRepository.GetLastBrandAsync();
            brand.BrandNumber = lastBrand?.BrandNumber + 1 ?? 1;

            await _brandRepository.SaveAsync(cancellationToken);


            var templatePath = Path.Combine(webHostEnvironment.WebRootPath, "Templates", "NewBrand.html");

            if (!System.IO.File.Exists(templatePath))
            {
                return NotFound("Email template not found.");
            }

            var emailContent = System.IO.File.ReadAllText(templatePath);

            // Load CSS
            //var cssPath = Path.Combine(webHostEnvironment.WebRootPath, "Css", "style.css");
            //if (!System.IO.File.Exists(cssPath))
            //{
            //    return NotFound("CSS style file not found.");
            //}
            if (!string.IsNullOrEmpty(emailContent))
            {
                emailContent = emailContent.Replace("{{Link}}", "https://mp3quran.net/ar/yasser");
            }
            var usersInRole = await _userManager.GetUsersInRoleAsync("User");

            var emails = usersInRole.Where(u => !u.IsDeleted).Select(x=>x.Email).ToList();



            await _emailSender.SendEmailToListAsync(new SendMultipleEmailsDto
            {
                Subject="Hello",
                Body=emailContent,
                Emails= emails

            });
            return Ok(new Response<Guid>
            {
                Data = result.Id,
                IsError = false,
                Message = "Created successfully",
                MessageAr = "تم الاضافه بنجاح",
                Status = (int)StatusCodeEnum.Ok
            });
        }

        [HttpPut]
        public async Task<IActionResult> UpdateBrand([FromForm] BrandDto model, CancellationToken cancellationToken)
        {
            var brand = await _brandRepository.Find(c => c.Id == model.Id, false);
            if (brand == null)
            {
                return NotFound(new Response<string>
                {
                    Data = null,
                    IsError = true,
                    Message = "Brand not found",
                    MessageAr = "هذه العلامه التجاريه غير موجوده",
                    Status = (int)StatusCodeEnum.NotFound
                });
            }

            var config = new TypeAdapterConfig();
            config.Default.IgnoreNullValues(true);

            model.Adapt(brand, config);

            if (model.ImageHome != null)
            {
                var fileModel = await _filesManager.UploadFile(model.ImageHome, "Brands", brand.ImageUrl);
                brand.ImageUrl = fileModel!.Url;
            }

            if (model.ImageBaner != null)
            {
                var fileModel = await _filesManager.UploadFile(model.ImageBaner, "Brands", brand.ImageBanerUrl);
                brand.ImageBanerUrl = fileModel!.Url;
            }
            if (model.BrandSizeChart != null)
            {
                var fileModel = await _filesManager.UploadFile(model.ImageBaner, "Brands", brand.BrandSizeChartUrl);
                brand.BrandSizeChartUrl = fileModel!.Url;
            }
            brand.UpdatedBy = "admin";
            brand.UpdatedOn = DateTime.UtcNow;

            _brandRepository.Update(brand);
            await _brandRepository.SaveAsync(cancellationToken);

            return Ok(new Response<Guid>
            {
                Data = brand.Id,
                IsError = false,
                Message = "Updated successfully",
                MessageAr = "تم التعديلات بنجاح",
                Status = (int)StatusCodeEnum.Ok
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBrand(Guid id, CancellationToken cancellationToken)
        {
            var brand = await _brandRepository.Find(c => c.Id == id, false, new string[] { "Products", "SubBrands" });

            if (brand == null)
            {
                return NotFound(new Response<string>
                {
                    Data = null,
                    IsError = true,
                    Message = "Brand not found",
                    MessageAr = "هذه العلامه التجاريه غير موجوده",
                    Status = (int)StatusCodeEnum.NotFound
                });
            }

            bool HasRelatedData = brand.Products.Any(e => e.BrandId == brand.Id) ||
                                  brand.SubBrands.Any(e => e.ParentBrandId == brand.Id);

            if (HasRelatedData)
            {
                return BadRequest(new Response<string>
                {
                    Data = null,
                    IsError = true,
                    Message = "Cannot delete brand with related data",
                    MessageAr = "لا يمكن حذف العلامة التجارية لوجود بيانات مرتبطة",
                    Status = (int)StatusCodeEnum.BadRequest
                });
            }

            if (!string.IsNullOrEmpty(brand.ImageUrl))
            {
                bool fileDeleted = _filesManager.DeleteFile(brand.ImageUrl);

                if (!fileDeleted)
                {
                    return BadRequest(new Response<string>
                    {
                        Data = null,
                        IsError = true,
                        Message = "Failed to delete associated file",
                        MessageAr = "فشلت عملية المسح",
                        Status = (int)StatusCodeEnum.BadRequest
                    });
                }
            }
            if (!string.IsNullOrEmpty(brand.ImageBanerUrl))
            {
                bool fileDeleted = _filesManager.DeleteFile(brand.ImageBanerUrl);

                if (!fileDeleted)
                {
                    return BadRequest(new Response<string>
                    {
                        Data = null,
                        IsError = true,
                        Message = "Failed to delete associated file",
                        MessageAr = " فشلت عملية المسح",
                        Status = (int)StatusCodeEnum.BadRequest
                    });
                }
            }
            if (!string.IsNullOrEmpty(brand.BrandSizeChartUrl))
            {
                bool fileDeleted = _filesManager.DeleteFile(brand.BrandSizeChartUrl);

                if (!fileDeleted)
                {
                    return BadRequest(new Response<string>
                    {
                        Data = null,
                        IsError = true,
                        Message = "Failed to delete associated file",
                        MessageAr = " فشلت عملية المسح",
                        Status = (int)StatusCodeEnum.BadRequest
                    });
                }
            }
            _brandRepository.Delete(brand);
            await _brandRepository.SaveAsync(cancellationToken);
            return Ok(new Response<Guid>
            {
                Data = id,
                IsError = false,
                Message = "Brand and associated file deleted successfully",
                MessageAr = "تم المسح بنجاح",
                Status = (int)StatusCodeEnum.Ok
            });
        }

        [HttpGet("featuredInHome")]
        public async Task<IActionResult> GetAllBrandsFeaturedInHome()
        {
            var brands = await _brandRepository.FindByCondition(b => b.IsFeatured);
            var data = brands.Adapt<List<BrandDto>>();

            return Ok(new Response<List<BrandDto>>
            {
                Data = data,
                IsError = false,
                Message = "Get Brand successfully",
                Status = (int)StatusCodeEnum.Ok
            });
        }

        [HttpGet("subBrands/{brandId}")]
        public async Task<IActionResult> GetAllSubBrands(Guid brandId, int pageNumber, int pageSize, string? searchTerm = null)
        {
            var brands = new PagainationModel<IEnumerable<Brand>>();
            if (searchTerm == null)
                brands = await _brandRepository.FindAllAsyncByPagination(null, pageNumber, pageSize);
            else
                brands = await _brandRepository
                                  .FindAllAsyncByPagination(b => b.ParentBrandId == brandId && (b.NameAr.Contains(searchTerm)
                                                             || b.NameEn.Contains(searchTerm)
                                                             || b.DescriptionAr.Contains(searchTerm)
                                                             || b.DescriptionEn.Contains(searchTerm))
                                 , pageNumber, pageSize);

            var data = brands.Data.Adapt<List<SubBrandDto>>();

            return Ok(new PaginationResponse<List<SubBrandDto>>
            {
                Data = data,
                IsError = false,
                Status = (int)StatusCodeEnum.Ok,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = brands.TotalCount
            });
        }

        [HttpGet("subBrandById/{id}")]
        public async Task<IActionResult> GetSubBrandById(Guid id)
        {
            var sub = await _brandRepository.Find(s => s.Id == id, false);
            if (sub == null)
            {
                return NotFound();
            }
            var data = sub.Adapt<SubBrandDto>();

            return Ok(new Response<SubBrandDto>
            {
                Data = data,
                IsError = false,
                Message = "Geting Operation successfully",
                Status = (int)StatusCodeEnum.Ok
            });
        }

        [HttpPost("subBrand")]
        public async Task<IActionResult> CreateSubBrand([FromBody] SubBrandDto model, CancellationToken cancellationToken)
        {
            var subbrand = model.Adapt<Brand>();
            subbrand.CreatedBy = "admin";
            if (model.Image != null)
            {
                var fileModel = await _filesManager.UploadFile(model.Image, "Brands");
                subbrand.ImageUrl = fileModel!.Url;
            }

            var result = await _brandRepository.Create(subbrand);
            await _brandRepository.SaveAsync(cancellationToken);

            return Ok(new Response<Guid>
            {
                Data = result.Id,
                IsError = false,
                Message = $"Created",
                MessageAr = "تم الاضافه بنجاح ",
                Status = (int)StatusCodeEnum.Ok
            });
        }

        [HttpPut("subBrand")]
        public async Task<IActionResult> UpdateSubBrand([FromForm] SubBrandDto model, CancellationToken cancellationToken)
        {
            var brand = await _brandRepository.Find(c => c.Id == model.Id, false);
            var OldImage = brand.ImageUrl;
            brand = model.Adapt<Brand>();
            brand.CreatedBy = "admin";

            if (model.Image != null)
            {
                var fileModel = await _filesManager.UploadFile(model.Image, "Brands", OldImage);
                brand.ImageUrl = fileModel!.Url;
            }

            _brandRepository.Update(brand);
            await _brandRepository.SaveAsync(cancellationToken);

            return Ok(new Response<Guid>
            {
                Data = brand.Id,
                IsError = false,
                Message = $"successfuly Updated",
                MessageAr = "تم التعديل بنجاح",
                Status = (int)StatusCodeEnum.Ok
            });
        }
    }
}