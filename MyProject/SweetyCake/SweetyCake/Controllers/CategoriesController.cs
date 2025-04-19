
using Microsoft.EntityFrameworkCore;
using OutbornE_commerce.BAL.Dto.Categories;
using OutbornE_commerce.BAL.Repositories.Categories;

namespace OutbornE_commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoriesController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        [HttpGet("GetAllCategoriesWithoutpagination")]
        public async Task<IActionResult> GetAllCategoriesWithoutpagination()
        {
            var categories = await _categoryRepository.FindAllAsync(null);

            var categoryDtos = categories.Adapt<List<CategoryDto>>();

            if (categoryDtos == null || !categoryDtos.Any())
            {
                return Ok(new Response<List<CategoryDto>>()
                {
                    Data = categoryDtos,
                    IsError = true,
                    Message = "Category not found",
                    MessageAr = "الكتيجوري غير موجود",
                    Status = (int)StatusCodeEnum.NotFound
                });
            }

            return Ok(new Response<List<CategoryDto>>()
            {
                Data = categoryDtos,
                IsError = false,
                Message = "Categories fetched successfully",
                MessageAr = "تم ارجاع الفئات بنجاح",

                Status = (int)StatusCodeEnum.Ok
            });
        }

        [HttpGet("GetAllSubsAndTypesByCategoryId/{CategoryId}")]
        public async Task<IActionResult> GetAllSubsAndTypesByCategoryId(Guid CategoryId)
        {
            if (CategoryId == Guid.Empty)
            {
                return BadRequest(new Response<List<AllCategoriesWithSubsAndTypesDto>>()
                {
                    Data = null,
                    IsError = true,
                    Message = "Invalid Category ID",
                    MessageAr = "معرف الفئة غير صالح",
                    Status = (int)StatusCodeEnum.BadRequest
                });
            }

            string[] includes = { "CategorySubCategories", "CategorySubCategories.SubCategory", "CategorySubCategories.SubCategory.SubCategoryTypes.type" };

            var category = await _categoryRepository.FindByCondition(x => x.Id == CategoryId, includes);

            if (category == null || !category.Any())
            {
                return Ok(new Response<List<AllCategoriesWithSubsAndTypesDto>>()
                {
                    Data = null,
                    IsError = true,
                    Message = "Category not found",
                    MessageAr = "الفئة غير موجودة",
                    Status = (int)StatusCodeEnum.NotFound
                });
            }

            var categoryDtos = category.Adapt<List<AllCategoriesWithSubsAndTypesDto>>();

            return Ok(new Response<List<AllCategoriesWithSubsAndTypesDto>>()
            {
                Data = categoryDtos,
                IsError = false,
                Message = "Categories fetched successfully",
                MessageAr = "تم إرجاع الفئات بنجاح",
                Status = (int)StatusCodeEnum.Ok
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCateogries(int pageNumber = 1, int pageSize = 10, string? searchTerm = null)
        {
            var items = new PagainationModel<IEnumerable<Category>>();
            if (string.IsNullOrEmpty(searchTerm))
                items = await _categoryRepository.FindAllAsyncByPagination(null, pageNumber, pageSize);
            else
                items = await _categoryRepository
                                    .FindAllAsyncByPagination(b => (b.NameAr.Contains(searchTerm)
                                                               || b.NameEn.Contains(searchTerm))

                                   , pageNumber, pageSize);
            var data = items.Data.Adapt<List<CategoryDto>>();

            return Ok(new PaginationResponse<List<CategoryDto>>
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
        public async Task<IActionResult> GetCategoryById(Guid id)
        {
            var category = await _categoryRepository.Find(c => c.Id == id, true);
            if (category == null)
            {
                return Ok(new Response<CategoryDto>()
                {
                    Data = null,
                    IsError = true,
                    Message = $"Not Found",
                    MessageAr = $"هذا التصنيف غير موجود",
                    Status = (int)StatusCodeEnum.NotFound
                });
            }
            var data = category.Adapt<CategoryDto>();
            return Ok(new Response<CategoryDto>()
            {
                Data = data,
                IsError = false,
                Message = $"get Category successfully",
                Status = (int)StatusCodeEnum.Ok
            });
        }

       

        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromForm] CategoryDto model, CancellationToken cancellationToken)
        {
            try
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

                var category = model.Adapt<Category>();
                category.CreatedBy = "admin";

                await _categoryRepository.BeginTransactionAsync();

                var result = await _categoryRepository.Create(category);
                await _categoryRepository.SaveAsync(cancellationToken);

                await _categoryRepository.CommitTransactionAsync();

                return Ok(new Response<Guid>
                {
                    Data = result.Id,
                    IsError = false,
                    Message = "Added Successfully",
                    MessageAr = "تم الإضافة بنجاح",
                    Status = (int)StatusCodeEnum.Ok
                });
            }
            catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("IX_Category_NameEn") == true || ex.InnerException?.Message.Contains("IX_Category_NameAr") == true)
            {
                await _categoryRepository.RollbackTransactionAsync();

                return Conflict(new Response<string>
                {
                    Data = null,
                    IsError = true,
                    Message = "The name already exists",
                    MessageAr = "الاسم موجود بالفعل",
                    Status = (int)StatusCodeEnum.BadRequest
                });
            }
            catch (Exception ex)
            {
                await _categoryRepository.RollbackTransactionAsync();

                return StatusCode((int)StatusCodeEnum.ServerError, new Response<string>
                {
                    Data = ex.Message,
                    IsError = true,
                    Message = "Error while adding",
                    MessageAr = "حدث خطأ أثناء الإضافة",
                    Status = (int)StatusCodeEnum.ServerError
                });
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCategory([FromForm] CategoryDto model, CancellationToken cancellationToken)
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

            var category = await _categoryRepository.Find(c => c.Id == model.Id, false);
            if (category is null)
            {
                return NotFound(new Response<Guid?>
                {
                    Data = model.Id,
                    IsError = true,
                    Message = "This Category Not Found",
                    MessageAr = "لم يتم العثور على هذه الفئة",
                    Status = (int)StatusCodeEnum.NotFound
                });
            }

            try
            {
                var duplicateCategoryEn = await _categoryRepository.Find(c => (c.NameEn == model.NameEn || c.NameAr == model.NameAr) && c.Id != model.Id);
                if (duplicateCategoryEn != null)
                {
                    return Conflict(new Response<string>
                    {
                        Data = model.NameEn,
                        IsError = true,
                        Message = "The name already exists",
                        MessageAr = "الاسم موجود بالفعل",
                        Status = (int)StatusCodeEnum.BadRequest
                    });
                }

                category = model.Adapt(category);
                category.UpdatedBy = "admin";
                category.UpdatedOn = DateTime.UtcNow;

                await _categoryRepository.BeginTransactionAsync();

                _categoryRepository.Update(category);
                await _categoryRepository.SaveAsync(cancellationToken);

                await _categoryRepository.CommitTransactionAsync();

                return Ok(new Response<Guid>
                {
                    Data = category.Id,
                    IsError = false,
                    Message = "Updated Successfully",
                    MessageAr = "تم التحديث بنجاح",
                    Status = (int)StatusCodeEnum.Ok
                });
            }
            catch (Exception ex)
            {
                await _categoryRepository.RollbackTransactionAsync();

                return StatusCode((int)StatusCodeEnum.ServerError, new Response<string>
                {
                    Data = ex.InnerException?.Message ?? ex.Message,
                    IsError = true,
                    Message = "Error While Updating",
                    MessageAr = "حدث خطأ أثناء التحديث",
                    Status = (int)StatusCodeEnum.ServerError
                });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                var category = await _categoryRepository.Find(c => c.Id == id, false, new string[] { "DiscountCategories", "CategorySubCategories" });

                if (category == null)
                {
                    return NotFound(new Response<string>
                    {
                        Data = null,
                        IsError = true,
                        Message = "Category not found",
                        MessageAr = "الكتيجوري غير موجود",
                        Status = (int)StatusCodeEnum.NotFound
                    });
                }

                category.IsDeleted = true;
                _categoryRepository.Update(category);

                await _categoryRepository.SaveAsync(cancellationToken);

                return Ok(new Response<Guid>
                {
                    Data = category.Id,
                    IsError = false,
                    Message = "Category and associated data marked as deleted successfully",
                    MessageAr = "تم الحذف بنجاح ",
                    Status = (int)StatusCodeEnum.Ok
                });
            }
            catch (Exception ex)
            {
                return StatusCode((int)StatusCodeEnum.ServerError, new Response<string>
                {
                    Data = null,
                    Message = "An error occurred while deleting the category",
                    MessageAr = "حدث خطأ أثناء حذف الكتيجوري",
                    IsError = true,
                    Status = (int)StatusCodeEnum.ServerError
                });
            }
        }

        //[HttpGet("SuperCategories")]
        //public async Task<IActionResult> GetAllSuperCategoriesById()
        //{
        //    var category = await _categoryRepository.FindByCondition(c => c.SuperCategoryID == null);
        //    if (category == null)
        //    {
        //        return Ok(new Response<CategoryDto>()
        //        {
        //            Data = null,
        //            IsError = true,
        //            MessageAr = "لم يتم العثور علي اي بيانات",
        //            Message = "No Data Found",
        //            Status = (int)StatusCodeEnum.NotFound
        //        });
        //    }
        //    var data = category.Adapt<List<CategoryDto>>();
        //    return Ok(new Response<List<CategoryDto>>()
        //    {
        //        Data = data,
        //        IsError = false,
        //        Message = $"",
        //        Status = (int)StatusCodeEnum.Ok
        //    });
        //}

        //[HttpGet("GetAllSubCategories")]
        //public async Task<IActionResult> GetAllSubCategories(int pageNumber = 1, int pageSize = 10)
        //{
        //    // Validate pageNumber and pageSize
        //    if (pageNumber <= 0)
        //        return BadRequest(new { Message = "Page number must be greater than 0." });

        //    if (pageSize <= 0 || pageSize > 100)
        //        return BadRequest(new { Message = "Page size must be between 1 and 100." });

        //   var items = await _categoryRepository.FindAllAsyncByPagination(
        //        e => e.SuperCategoryID != null,
        //        pageNumber,
        //        pageSize
        //    );

        //    if (items == null || !items.Data.Any())
        //        return NotFound(new { Message = "No subcategories found." });

        //    var data = items.Data.Adapt<List<SubCategoryDto>>();

        //    return Ok(new PaginationResponse<List<SubCategoryDto>>
        //    {
        //        Data = data,
        //        IsError = false,
        //        Status = (int)StatusCodeEnum.Ok,
        //        PageNumber = pageNumber,
        //        PageSize = pageSize,
        //        TotalCount = items.TotalCount
        //    });
        //}

        //[HttpGet("GetCategoryBySuperId/{SuperCatigoryId}")]
        //public async Task<IActionResult> GetCategoryBySuperId(Guid SuperCatigoryId, int pageNumber = 1, int pageSize = 10)
        //{
        //    var subCategories = await _categoryRepository.FindAllAsyncByPagination(s => s.SuperCategoryID == SuperCatigoryId, pageNumber, pageSize);
        //    if (subCategories.Data.Count() == 0)
        //    {
        //        return NotFound(new
        //        {
        //            MessageAr = "لم يتم العثور علي اي بيانات",
        //            MessageEn = "No Data Found"
        //        });
        //    }
        //    var data = subCategories.Data.Adapt<List<SubCategoryDto>>();
        //    return Ok(new Response<List<SubCategoryDto>>()
        //    {
        //        Data = data,
        //        IsError = false,
        //        Message = $"",
        //        Status = (int)StatusCodeEnum.Ok,
        //    });
        //}
    }
}