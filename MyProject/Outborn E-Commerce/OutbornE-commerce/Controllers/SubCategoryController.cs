using Mapster;
using Microsoft.AspNetCore.Mvc;
using OutbornE_commerce.BAL.Dto;

using OutbornE_commerce.FilesManager;
using OutbornE_commerce.DAL.Enums;
using OutbornE_commerce.DAL.Models;
using OutbornE_commerce.BAL.Dto.Categories;
using OutbornE_commerce.BAL.Repositories.SubCategoryRepo;
using OutbornE_commerce.BAL.Repositories.CategorySubCategory;
using OutbornE_commerce.BAL.Dto.TypeDto;
using OutbornE_commerce.BAL.Repositories.SubCategoryTypeRepo;

namespace OutbornE_commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubCategoriesController : ControllerBase
    {
        private readonly ISubCategoryRepository _subCategoryRepository;
        private readonly IFilesManager _filesManager;

        public ISubcategoryTypeRepository SubcategoryTypeRepository { get; }
        public ICategorySubCategoryBridgeRepository CategorySubCategoryBridgeRepository { get; }

        public SubCategoriesController(ISubcategoryTypeRepository subcategoryTypeRepository, ICategorySubCategoryBridgeRepository categorySubCategoryBridgeRepository, ISubCategoryRepository subCategoryRepository, IFilesManager filesManager)
        {
            SubcategoryTypeRepository = subcategoryTypeRepository;
            CategorySubCategoryBridgeRepository = categorySubCategoryBridgeRepository;
            _subCategoryRepository = subCategoryRepository;
            _filesManager = filesManager;
        }

        [HttpGet("GetTypesByCategoryAndSubCategory/{categoryId}/{subCategoryId}")]
        public async Task<IActionResult> GetTypesByCategoryAndSubCategory(Guid categoryId, Guid subCategoryId)
        {
            string[] includes = { "SubCategory.SubCategoryTypes.type" };

            var categorySubCategoryBridge = await CategorySubCategoryBridgeRepository.FindByCondition(
                x => x.CategoryId == categoryId && x.SubCategoryId == subCategoryId,
                includes
            );

            if (categorySubCategoryBridge == null || !categorySubCategoryBridge.Any())
            {
                return NotFound(new Response<string>
                {
                    Data = null,
                    IsError = true,
                    Message = "Category or Subcategory not found",
                    MessageAr = "الفئة أو الفئة الفرعية غير موجودة",
                    Status = (int)StatusCodeEnum.NotFound
                });
            }

            var typesDto = categorySubCategoryBridge
                .SelectMany(cs => cs.SubCategory.SubCategoryTypes)
                .Select(st => st.type.Adapt<GetAllTypessDto>())
                .ToList();

            return Ok(new Response<List<GetAllTypessDto>>
            {
                Data = typesDto,
                IsError = false,
                Message = "Types retrieved successfully",
                MessageAr = "تم جلب الأنواع بنجاح",
                Status = (int)StatusCodeEnum.Ok
            });
        }

        [HttpGet("GetAllTypesForSpecificSubCategory/{id}")]
        public async Task<IActionResult> GetAllTypesForSpecificSubCategory(Guid id)
        {
            string[] includes = { "type" };

            var subcategoryTypes = await SubcategoryTypeRepository.FindByCondition(
                x => x.SubCategoryId == id,
                includes
            );

            if (subcategoryTypes == null || !subcategoryTypes.Any())
            {
                return NotFound(new Response<string>
                {
                    Data = null,
                    IsError = true,
                    Message = "Subcategory not found",
                    MessageAr = "هذه الفئة الفرعية غير موجودة",
                    Status = (int)StatusCodeEnum.NotFound
                });
            }

            var typesDto = subcategoryTypes
                .Select(st => st.type.Adapt<GetAllTypessDto>())
                .ToList();

            return Ok(new Response<List<GetAllTypessDto>>
            {
                Data = typesDto,
                IsError = false,
                Message = "Types retrieved successfully",
                MessageAr = "تم جلب الأنواع بنجاح",
                Status = (int)StatusCodeEnum.Ok
            });
        }

        [HttpGet("GetAllsubsWithoutpagination")]
        public async Task<IActionResult> GetAllsubsWithoutpagination()
        {
            var subs = await _subCategoryRepository.FindAllAsync(null);

            var subsDto = subs.Adapt<List<getAllSubCategoriesDto>>();

            if (subsDto == null || !subsDto.Any())
            {
                return Ok(new Response<List<getAllSubCategoriesDto>>()
                {
                    Data = subsDto,
                    IsError = true,
                    Message = "Subcategories not found",
                    MessageAr = "الفئات الفرعيه غير موجوده",
                    Status = (int)StatusCodeEnum.NotFound
                });
            }

            return Ok(new Response<List<getAllSubCategoriesDto>>()
            {
                Data = subsDto,
                IsError = false,
                Message = "Subcategories fetched successfully",
                MessageAr = "تم ارجاع الفئات الفرعيه بنجاح",

                Status = (int)StatusCodeEnum.Ok
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSubCategories(int pageNumber = 1, int pageSize = 10, string? searchTerm = null)
        {
            var subCategories = new PagainationModel<IEnumerable<SubCategory>>();
            string[] includes = { "CategorySubCategories" };

            if (string.IsNullOrEmpty(searchTerm))
            {
                subCategories = await _subCategoryRepository.FindAllAsyncByPagination(null, pageNumber, pageSize, includes);
            }
            else
            {
                subCategories = await _subCategoryRepository.FindAllAsyncByPagination(
                    s => s.NameAr.Contains(searchTerm) || s.NameEn.Contains(searchTerm),
                    pageNumber, pageSize, includes
                );
            }

            var data = subCategories.Data.Adapt<List<SubCategoryDto>>();

            return Ok(new PaginationResponse<List<SubCategoryDto>>
            {
                Data = data,
                IsError = false,
                Status = (int)StatusCodeEnum.Ok,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = subCategories.TotalCount
            });
        }

        [HttpGet("GetAllSubCategoriesByTypes")]
        public async Task<IActionResult> GetAllSubCategoriesByTypes(int pageNumber = 1, int pageSize = 10, string? searchTerm = null)
        {
            var subCategories = new PagainationModel<IEnumerable<SubCategory>>();
            string[] includes = { "SubCategoryTypes", "SubCategoryTypes.type" };

            // Fetch data with pagination and search
            if (string.IsNullOrEmpty(searchTerm))
            {
                subCategories = await _subCategoryRepository.FindAllAsyncByPagination(null, pageNumber, pageSize, includes);
            }
            else
            {
                subCategories = await _subCategoryRepository.FindAllAsyncByPagination(
                    s => s.NameAr.Contains(searchTerm) || s.NameEn.Contains(searchTerm),
                    pageNumber, pageSize, includes
                );
            }

            var data = subCategories.Data.Adapt<List<AllSubCategoryWithTypes>>();

            return Ok(new PaginationResponse<List<AllSubCategoryWithTypes>>
            {
                Data = data,
                IsError = false,
                Status = (int)StatusCodeEnum.Ok,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = subCategories.TotalCount
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSubCategoryById(Guid id)
        {
            string[] includes = { "CategorySubCategories", "CategorySubCategories.Category" };

            var subCategory = await _subCategoryRepository.Find(s => s.Id == id, false, includes);

            if (subCategory == null)
            {
                return NotFound(new Response<string>
                {
                    Data = null,
                    IsError = true,
                    Message = "SubCategory not found",
                    MessageAr = "هذا التصنيف غير موجود",
                    Status = (int)StatusCodeEnum.NotFound
                });
            }

            var data = subCategory.Adapt<AllSubsWithCategoryDto>();

            return Ok(new Response<AllSubsWithCategoryDto>
            {
                Data = data,
                IsError = false,
                Message = "Retrieved successfully",
                MessageAr = "تم الحصول على التصنيف بنجاح",
                Status = (int)StatusCodeEnum.Ok
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateSubCategory([FromForm] SubCategoryDto model, CancellationToken cancellationToken)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.NameEn) || string.IsNullOrWhiteSpace(model.NameAr))
            {
                return BadRequest(new Response<string>
                {
                    Data = null,
                    IsError = true,
                    Message = "Invalid input data",
                    MessageAr = "بيانات الإدخال غير صالحة",
                    Status = (int)StatusCodeEnum.BadRequest
                });
            }

            if (model.CategoriesId == null || !model.CategoriesId.Any())
            {
                return BadRequest(new Response<string>
                {
                    Data = null,
                    IsError = true,
                    Message = "At least one CategoryId is required",
                    MessageAr = "مطلوب على الأقل معرف فئة واحدة",
                    Status = (int)StatusCodeEnum.BadRequest
                });
            }

            try
            {
                //var existingByNameEn = await _subCategoryRepository.Find(s => (s.NameEn == model.NameEn|| s.NameAr == model.NameAr ), false);
                //if (existingByNameEn != null)
                //{
                //    return Conflict(new Response<string>
                //    {
                //        Data = null,
                //        IsError = true,
                //        Message = "The name already exists",
                //        MessageAr = "الاسم موجود بالفعل",
                //        Status = (int)StatusCodeEnum.BadRequest
                //    });
                //}

                var subCategory = model.Adapt<SubCategory>();
                subCategory.CreatedBy = "admin";
                subCategory.CreatedOn = DateTime.UtcNow;

                var result = await _subCategoryRepository.Create(subCategory);
                await _subCategoryRepository.SaveAsync(cancellationToken);

                var bridges = model.CategoriesId.Select(categoryId => new CategorySubCategoryBridge
                {
                    CategoryId = categoryId,
                    SubCategoryId = result.Id,
                    CreatedOn = DateTime.UtcNow,
                    CreatedBy = "admin"
                }).ToList();

                await CategorySubCategoryBridgeRepository.CreateRange(bridges);
                await CategorySubCategoryBridgeRepository.SaveAsync(cancellationToken);

                return Ok(new Response<Guid>
                {
                    Data = result.Id,
                    IsError = false,
                    Message = "Created successfully",
                    MessageAr = "تم الإضافة بنجاح",
                    Status = (int)StatusCodeEnum.Ok
                });
            }
            catch (Exception ex)
            {
                return StatusCode((int)StatusCodeEnum.ServerError, new Response<string>
                {
                    Data = null,
                    IsError = true,
                    Message = "An error occurred while creating the subcategory",
                    MessageAr = "حدث خطأ أثناء إنشاء الفئة الفرعية",
                    Status = (int)StatusCodeEnum.ServerError
                });
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateSubCategory([FromForm] SubCategoryDto model, CancellationToken cancellationToken)
        {
            if (model == null || model.Id == null || string.IsNullOrWhiteSpace(model.NameEn) || string.IsNullOrWhiteSpace(model.NameAr))
            {
                return BadRequest(new Response<string>
                {
                    Data = null,
                    IsError = true,
                    Message = "Invalid input data",
                    MessageAr = "بيانات الإدخال غير صالحة",
                    Status = (int)StatusCodeEnum.BadRequest
                });
            }

            try
            {
                //var existingByNameEn = await _subCategoryRepository.Find(s => (s.NameEn == model.NameEn || s.NameAr == model.NameAr) && s.Id != model.Id, false);
                //if (existingByNameEn != null)
                //{
                //    return Conflict(new Response<string>
                //    {
                //        Data = null,
                //        IsError = true,
                //        Message = "The name already exists",
                //        MessageAr = "الاسم موجود بالفعل",
                //        Status = (int)StatusCodeEnum.BadRequest
                //    });
                //}

                var existingSubCategory = await _subCategoryRepository.Find(s => s.Id == model.Id, false);
                if (existingSubCategory == null)
                {
                    return NotFound(new Response<string>
                    {
                        Data = null,
                        IsError = true,
                        Message = "SubCategory not found",
                        MessageAr = "هذا التصنيف غير موجود",
                        Status = (int)StatusCodeEnum.NotFound
                    });
                }

                var updatedSubCategory = model.Adapt(existingSubCategory);
                updatedSubCategory.UpdatedBy = "admin";
                updatedSubCategory.UpdatedOn = DateTime.UtcNow;

                _subCategoryRepository.Update(updatedSubCategory);

                if (model.CategoriesId != null && model.CategoriesId.Any())
                {
                    var existingBridges = await CategorySubCategoryBridgeRepository.FindByCondition(b => b.SubCategoryId == existingSubCategory.Id);
                    foreach (var bridge in existingBridges)
                    {
                        CategorySubCategoryBridgeRepository.Delete(bridge);
                    }

                    var newBridges = model.CategoriesId.Select(categoryId => new CategorySubCategoryBridge
                    {
                        CategoryId = categoryId,
                        SubCategoryId = existingSubCategory.Id,
                        CreatedOn = DateTime.UtcNow,
                        CreatedBy = "admin"
                    }).ToList();

                    foreach (var bridge in newBridges)
                    {
                        await CategorySubCategoryBridgeRepository.Create(bridge);
                    }
                }

                await _subCategoryRepository.SaveAsync(cancellationToken);
                await CategorySubCategoryBridgeRepository.SaveAsync(cancellationToken);

                return Ok(new Response<Guid>
                {
                    Data = existingSubCategory.Id,
                    IsError = false,
                    Message = "Updated successfully",
                    MessageAr = "تم التعديل بنجاح",
                    Status = (int)StatusCodeEnum.Ok
                });
            }
            catch (Exception ex)
            {
                return StatusCode((int)StatusCodeEnum.ServerError, new Response<string>
                {
                    Data = null,
                    IsError = true,
                    Message = "An error occurred while updating the subcategory",
                    MessageAr = "حدث خطأ أثناء تعديل الفئة الفرعية",
                    Status = (int)StatusCodeEnum.ServerError
                });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSubCategory(Guid id, CancellationToken cancellationToken)
        {
            var includes = new List<string>
    {
        "CategorySubCategories",
        "SubCategoryTypes",
    };
            string[] includesArray = includes.ToArray();

            var subCategory = await _subCategoryRepository.Find(
                s => s.Id == id,
                false,
                includesArray
            );

            if (subCategory == null)
            {
                return NotFound(new Response<string>
                {
                    Data = null,
                    IsError = true,
                    Message = "SubCategory not found",
                    MessageAr = "هذا التصنيف غير موجود",
                    Status = (int)StatusCodeEnum.NotFound
                });
            }

            var hasSubCategoryTypes = subCategory.SubCategoryTypes != null && subCategory.SubCategoryTypes.Any();

            if (hasSubCategoryTypes)
            {
                return Ok(new Response<Guid>
                {
                    Data = id,
                    Message = "The SubCategory cannot be deleted because it is connected to other data.",
                    MessageAr = "لا يمكن حذف التصنيف لأنه مرتبط ببيانات أخرى.",
                    IsError = true,
                    Status = (int)StatusCodeEnum.BadRequest
                });
            }

            subCategory.IsDeleted = true;
            _subCategoryRepository.Update(subCategory);
            await _subCategoryRepository.SaveAsync(cancellationToken);

            return Ok(new Response<Guid>
            {
                Data = id,
                IsError = false,
                Message = "Deleted successfully",
                MessageAr = "تم الحذف بنجاح",
                Status = (int)StatusCodeEnum.Ok
            });
        }
    }
}