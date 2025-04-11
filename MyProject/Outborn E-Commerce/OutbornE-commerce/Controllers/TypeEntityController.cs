using Mapster;
using Microsoft.AspNetCore.Mvc;
using OutbornE_commerce.BAL.Dto;
using OutbornE_commerce.FilesManager;
using OutbornE_commerce.DAL.Enums;
using OutbornE_commerce.DAL.Models;
using OutbornE_commerce.BAL.Repositories.TypeEntityRepo;
using OutbornE_commerce.BAL.Dto.TypeDto;
using OutbornE_commerce.BAL.Repositories.SubCategoryTypeRepo;
using OutbornE_commerce.BAL.Dto.Categories;
using OutbornE_commerce.BAL.Repositories.Products;

namespace OutbornE_commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TypesController : ControllerBase
    {
        private readonly ITypeEntityRepository _typeRepository;

        public IProductRepository ProductRepository { get; }
        public ISubcategoryTypeRepository SubcategoryTypeRepository { get; }

        public TypesController(IProductRepository productRepository, ISubcategoryTypeRepository subcategoryTypeRepository, ITypeEntityRepository typeRepository)
        {
            ProductRepository = productRepository;
            SubcategoryTypeRepository = subcategoryTypeRepository;
            _typeRepository = typeRepository;
        }

        [HttpGet("GetAllTypesWithoutpagination")]
        public async Task<IActionResult> GetAllTypesWithoutpagination()
        {
            var types = await _typeRepository.FindAllAsync(null);

            var typesDto = types.Adapt<List<GetAllTypessDto>>();

            if (typesDto == null || !typesDto.Any())
            {
                return Ok(new Response<List<GetAllTypessDto>>()
                {
                    Data = typesDto,
                    IsError = true,
                    Message = "Types not found",
                    MessageAr = "الانواع غير موجوده",
                    Status = (int)StatusCodeEnum.NotFound
                });
            }

            return Ok(new Response<List<GetAllTypessDto>>()
            {
                Data = typesDto,
                IsError = false,
                Message = "Types fetched successfully",
                MessageAr = "تم ارجاع الانواع بنجاح",

                Status = (int)StatusCodeEnum.Ok
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTypes(int pageNumber = 1, int pageSize = 10, string? searchTerm = null)
        {
            var types = new PagainationModel<IEnumerable<TypeEntity>>();
            string[] includes = { "SubCategoryTypes" };

            if (string.IsNullOrEmpty(searchTerm))
            {
                types = await _typeRepository.FindAllAsyncByPagination(null, pageNumber, pageSize, includes);
            }
            else
            {
                types = await _typeRepository.FindAllAsyncByPagination(
                    t => t.NameAr.Contains(searchTerm) || t.NameEn.Contains(searchTerm),
                    pageNumber, pageSize, includes
                );
            }

            var data = types.Data.Adapt<List<TypeDto>>();

            return Ok(new PaginationResponse<List<TypeDto>>
            {
                Data = data,
                IsError = false,
                Status = (int)StatusCodeEnum.Ok,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = types.TotalCount
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTypeById(Guid id)
        {
            var type = await _typeRepository.Find(t => t.Id == id, false, new string[] { "SubCategoryTypes", "SubCategoryTypes.SubCategory", "SubCategoryTypes.SubCategory.CategorySubCategories.Category" });

            if (type == null)
            {
                return NotFound(new Response<string>
                {
                    Data = null,
                    IsError = true,
                    Message = "Type not found",
                    MessageAr = "هذا النوع غير موجود",
                    Status = (int)StatusCodeEnum.NotFound
                });
            }

            var data = type.Adapt<ReturnTypeDto>();

            return Ok(new Response<ReturnTypeDto>
            {
                Data = data,
                IsError = false,
                Message = "Retrieved successfully",
                MessageAr = "تم الحصول على النوع بنجاح",
                Status = (int)StatusCodeEnum.Ok
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateType([FromForm] TypeDto model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new Response<string>
                {
                    Data = null,
                    IsError = true,
                    Message = "Invalid data provided",
                    MessageAr = "تم توفير بيانات غير صحيحة",
                    Status = (int)StatusCodeEnum.BadRequest
                });
            }

            //var existingTypeEn = await _typeRepository.Find(t => t.NameEn == model.NameEn || t.NameAr == model.NameAr);
            //if (existingTypeEn != null)
            //{
            //    return Conflict(new Response<string>
            //    {
            //        Data = model.NameEn,
            //        IsError = true,
            //        Message = "The name already exists",
            //        MessageAr = "الاسم موجود بالفعل",
            //        Status = (int)StatusCodeEnum.BadRequest
            //    });
            //}

            try
            {
                var type = model.Adapt<TypeEntity>();
                type.CreatedBy = "admin";
                type.CreatedOn = DateTime.Now;

                if (model.SubCategoriesIds != null && model.SubCategoriesIds.Any())
                {
                    var subCategoryTypes = new List<SubCategoryType>();
                    foreach (var subCategoryId in model.SubCategoriesIds)
                    {
                        subCategoryTypes.Add(new SubCategoryType
                        {
                            SubCategoryId = subCategoryId,
                            TypeId = type.Id,
                            CreatedBy = "admin",
                            CreatedOn = DateTime.Now
                        });
                    }
                    type.SubCategoryTypes = subCategoryTypes;
                }
                var result = await _typeRepository.Create(type);

                await _typeRepository.SaveAsync(cancellationToken);

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
                    Data = ex.Message,
                    IsError = true,
                    Message = "An error occurred while creating the type",
                    MessageAr = "حدث خطأ أثناء إنشاء النوع",
                    Status = (int)StatusCodeEnum.ServerError
                });
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateType([FromForm] TypeDto model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new Response<string>
                {
                    Data = null,
                    IsError = true,
                    Message = "Invalid data provided",
                    MessageAr = "تم توفير بيانات غير صحيحة",
                    Status = (int)StatusCodeEnum.BadRequest
                });
            }

            //var existingType = await _typeRepository.Find(t => (t.NameEn == model.NameEn || t.NameAr == model.NameAr) && t.Id != model.Id);
            //if (existingType != null)
            //{
            //    return Conflict(new Response<string>
            //    {
            //        Data = model.NameEn,
            //        IsError = true,
            //        Message = "The name already exists",
            //        MessageAr = "الاسم موجود بالفعل",
            //        Status = (int)StatusCodeEnum.BadRequest
            //    });
            //}

            var type = await _typeRepository.Find(t => t.Id == model.Id, false);

            if (type == null)
            {
                return NotFound(new Response<string>
                {
                    Data = null,
                    IsError = true,
                    Message = "Type not found",
                    MessageAr = "هذا النوع غير موجود",
                    Status = (int)StatusCodeEnum.NotFound
                });
            }
            if (type.SubCategoryTypes == null)
            {
                type.SubCategoryTypes = new List<SubCategoryType>();
            }

            if (model.SubCategoriesIds != null && model.SubCategoriesIds.Any())
            {
                await SubcategoryTypeRepository.DeleteRange(st => type.SubCategoryTypes.Contains(st));
                foreach (var subCategoryId in model.SubCategoriesIds)
                {
                    type.SubCategoryTypes.Add(new SubCategoryType
                    {
                        SubCategoryId = subCategoryId,
                        TypeId = type.Id,
                        CreatedOn = DateTime.Now,
                        CreatedBy = "admin"
                    });
                }
            }

            type = model.Adapt<TypeEntity>();

            type.UpdatedBy = "admin";
            type.UpdatedOn = DateTime.Now;

            try
            {
                _typeRepository.Update(type);
                await _typeRepository.SaveAsync(cancellationToken);

                return Ok(new Response<Guid>
                {
                    Data = type.Id,
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
                    Data = ex.Message,
                    IsError = true,
                    Message = "An error occurred while updating the type",
                    MessageAr = "حدث خطأ أثناء التعديل",
                    Status = (int)StatusCodeEnum.ServerError
                });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteType(Guid id, CancellationToken cancellationToken)
        {
            var type = await _typeRepository.Find(
                t => t.Id == id,
                trackChanges: false,
                includes: new[] { "Products", "SubCategoryTypes" }
            );

            if (type == null)
            {
                return NotFound(new Response<string>
                {
                    Data = null,
                    IsError = true,
                    Message = "Type not found",
                    MessageAr = "هذا النوع غير موجود",
                    Status = (int)StatusCodeEnum.NotFound
                });
            }

            if (type.Products?.Any() == true)
            {
                return BadRequest(new Response<Guid>
                {
                    Data = id,
                    IsError = true,
                    Message = "The type cannot be deleted because it is connected to products.",
                    MessageAr = "لا يمكن حذف النوع لأنه مرتبط بمنتجات.",
                    Status = (int)StatusCodeEnum.BadRequest
                });
            }

            if (type.SubCategoryTypes?.Any() == true)
            {
                foreach (var subCategoryType in type.SubCategoryTypes)
                {
                    subCategoryType.IsDeleted = true;
                    SubcategoryTypeRepository.Update(subCategoryType);
                }
            }

            type.IsDeleted = true;
            _typeRepository.Update(type);

            await _typeRepository.SaveAsync(cancellationToken);

            return Ok(new Response<Guid>
            {
                Data = id,
                IsError = false,
                Message = "Deleted successfully .",
                MessageAr = "تم الحذف بنجاح .",
                Status = (int)StatusCodeEnum.Ok
            });
        }
    }
}