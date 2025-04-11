using Microsoft.AspNetCore.Mvc;
using OutbornE_commerce.BAL.Dto.AboutUs;
using OutbornE_commerce.BAL.Repositories.BaseRepositories;

namespace OutbornE_commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CityAreaController : ControllerBase
    {
        private readonly IBaseRepository<CityAreas> repository;

        public CityAreaController(IBaseRepository<CityAreas> repository)
        {
            this.repository = repository;
        }

        [HttpGet("AllWithPaginate")]
        public async Task<IActionResult> GetAllCityAreas(int pageNumber = 1, int pageSize = 10, string? searchTerm = null)
        {
            var items = string.IsNullOrEmpty(searchTerm)
                ? await repository.FindAllAsyncByPagination(null, pageNumber, pageSize)
                : await repository.FindAllAsyncByPagination(a =>
                    a.NameEn.Contains(searchTerm) ||
                    a.NameAr.Contains(searchTerm), pageNumber, pageSize);

            if (!items.Data.Any())
                return NotFound(new { Message = "No AboutUs records found." });

            var data = items.Data.Adapt<List<CityAreaDto>>();
            return Ok(new PaginationResponse<List<CityAreaDto>>
            {
                Data = data,
                IsError = false,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Status = (int)StatusCodeEnum.Ok,
                TotalCount = items.TotalCount
            });
        }

        [HttpGet("All")]
        public async Task<IActionResult> GetAllCityAreas()
        {
            var items = await repository.FindAllAsync(null);
            var itemEntities = items.Adapt<List<CityAreaDto>>();

            if (!itemEntities.Any())
                return NotFound(new { Message = "No CityAreas records found." });

            return Ok(new Response<List<CityAreaDto>>
            {
                Data = itemEntities,
                IsError = false,
                Message = "",
                Status = (int)StatusCodeEnum.Ok
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateCityAreas(CityAreaDto model, CancellationToken cancellationToken)
        {
            try
            {
                if (model == null)
                    return BadRequest(new { Message = "Invalid model." });

                var item = model.Adapt<CityAreas>();

                var result = await repository.Create(item);
                await repository.SaveAsync(cancellationToken);

                return Ok(new Response<Guid>
                {
                    Data = result.Id,
                    IsError = false,
                    Message = "AboutUs entry created successfully.",
                    MessageAr = "تمت عملية الاضافة بنجاح",
                    Status = (int)StatusCodeEnum.Ok
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Response<string>
                {
                    Data = ex.Message,
                    IsError = true,
                    Message = "An error occurred while processing your request.",
                    MessageAr = "حدث خطأ اثناء الاضافة",
                    Status = (int)StatusCodeEnum.ServerError
                });
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCityAreas(CityAreaDto model, CancellationToken cancellationToken)
        {
            if (model == null)
                return BadRequest(new { Message = "Invalid model." });

            var item = await repository.Find(a => a.Id == model.Id, false);
            if (item == null)
                return NotFound(new { Message = "AboutUs entry not found." });

            item = model.Adapt<CityAreas>();
            item.UpdatedBy = "admin";
            item.UpdatedOn = DateTime.Now;

            repository.Update(item);
            await repository.SaveAsync(cancellationToken);

            return Ok(new Response<Guid>
            {
                Data = item.Id,
                IsError = false,
                Message = "AboutUs entry updated successfully.",
                MessageAr = "تم التعديل بنجاح",
                Status = (int)StatusCodeEnum.Ok
            });
        }

        [HttpGet("{Id}")]
        public async Task<IActionResult> GetCityAreasById(Guid Id)
        {
            if (Id == Guid.Empty)
                return BadRequest(new { Message = "Invalid ID." });

            var item = await repository.Find(a => a.Id == Id);
            if (item == null)
                return NotFound(new { Message = "AboutUs entry not found." });

            var itemEntity = item.Adapt<CityAreaDto>();
            return Ok(new Response<CityAreaDto>
            {
                Data = itemEntity,
                IsError = false,
                Message = "",
                Status = (int)StatusCodeEnum.Ok
            });
        }

        [HttpDelete("{Id}")]
        public async Task<IActionResult> DeleteCityAreas(Guid Id, CancellationToken cancellationToken)
        {
            try
            {
                if (Id == Guid.Empty)
                    return BadRequest(new { Message = "Invalid ID." });

                var item = await repository.Find(a => a.Id == Id, true);
                if (item == null)
                    return NotFound(new { Message = "CityAreas entry not found." });

                repository.Delete(item);
                await repository.SaveAsync(cancellationToken);

                return Ok(new Response<Guid>
                {
                    Data = item.Id,
                    IsError = false,
                    Message = "Areas deleted successfully.",
                    MessageAr = "تم المسح بنجاح",
                    Status = (int)StatusCodeEnum.Ok
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Response<string>
                {
                    Data = ex.Message,
                    IsError = true,
                    Message = "An error occurred while processing your request.",
                    MessageAr = "حدث خطأ اثناء المسح",
                    Status = (int)StatusCodeEnum.ServerError
                });
            }
        }

        [HttpGet("byCityId")]
        public async Task<IActionResult> GetAllAreaForCity(Guid CityId)
        {
            var cities = await repository.FindByCondition(c => c.CityId == CityId, null); // null for the includes !!!
            var cityEntites = cities.Adapt<List<CityAreaDto>>();
            return Ok(new Response<List<CityAreaDto>>
            {
                Data = cityEntites,
                IsError = false,
                Message = "",
                Status = (int)StatusCodeEnum.Ok
            });
        }
    }
}