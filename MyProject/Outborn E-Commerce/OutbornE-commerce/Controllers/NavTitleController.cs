using Microsoft.AspNetCore.Mvc;
using OutbornE_commerce.BAL.Dto.NavTitle;
using OutbornE_commerce.BAL.Repositories.NavTitles;

namespace OutbornE_commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NavTitleController : ControllerBase
    {
        private readonly INavTitleRepo _navTitleRepo;
        public NavTitleController(INavTitleRepo navTitleRepo)
        {
            _navTitleRepo = navTitleRepo;
        }

        [HttpGet("All")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var items = await _navTitleRepo.FindAllAsync(null);
                return Ok(new Response<List<NavTitle>>
                {
                    Data = items.ToList(),
                    IsError = false,
                    Message = "",
                    Status = (int)StatusCodeEnum.Ok
                });
            }
            catch (Exception ex) 
            {
                return Ok(new Response<List<NavTitle>>
                {
                    Data = null,
                    IsError = true,
                    Message = ex.Message,
                    Status = (int)StatusCodeEnum.BadRequest
                });
            }
        }

        [HttpGet("{Id}")]
        public async Task<ActionResult<Response<NavTitleDto>>> GetById(Guid Id)
        {
            try
            {
                var navTitle = await _navTitleRepo.Find(a => a.Id == Id, false);
                var navTitleMapper = navTitle.Adapt<NavTitleDto>();
                if (navTitle is null)
                    return Ok(new Response<NavTitleDto>
                    {
                        Data = null,
                        IsError = true,
                        Status = (int)StatusCodeEnum.NotFound
                    });

                return Ok(new Response<NavTitleDto>
                {
                    Data = navTitleMapper,
                    IsError = false,
                    Status = (int)StatusCodeEnum.Ok,
                });
            }
            catch (Exception ex)
            {
                return Ok(new Response<NavTitleDto>
                {
                    Data = null,
                    IsError = true,
                    Message = ex.Message,
                    Status = (int)StatusCodeEnum.BadRequest
                });
            }
        }

        [HttpPost]
        public async Task<ActionResult<Response<Guid?>>> Create([FromForm] NavTitleDto model, CancellationToken cancellationToken)
        {
            try
            {
                var item = model.Adapt<NavTitle>();
                item.CreatedBy = "admin";

                var result = await _navTitleRepo.Create(item);
                await _navTitleRepo.SaveAsync(cancellationToken);
                return Ok(new Response<Guid?>
                {
                    Data = result.Id,
                    IsError = false,
                    Message = "Success",
                    MessageAr = "تم الاضافة بنجاح",
                    Status = (int)StatusCodeEnum.Ok
                });
            }
            catch (Exception ex) 
            {
                return Ok(new Response<Guid?>
                {
                    Data = null,
                    IsError = true,
                    Message = ex.Message,
                    Status = (int)StatusCodeEnum.BadRequest
                });
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromForm] NavTitleDto model, CancellationToken cancellationToken)
        {
            try
            {
                var item = await _navTitleRepo.Find(a => a.Id == model.Id, false);
                if (item is null)
                    return Ok(new Response<Guid?>
                    {
                        Data = model.Id,
                        IsError = true,
                        Message = "fail",
                        MessageAr = "لم يتم العثور عن الناف",
                        Status = (int)StatusCodeEnum.NotFound
                    });
                item = model.Adapt<NavTitle>();
                item.CreatedBy = "admin";

                _navTitleRepo.Update(item);
                await _navTitleRepo.SaveAsync(cancellationToken);
                return Ok(new Response<Guid>
                {
                    Data = item.Id,
                    IsError = false,
                    Message = "Updated Successfully",
                    MessageAr = "تم التحديث بنجاح",
                    Status = (int)StatusCodeEnum.Ok
                });
            }
            catch (Exception ex)
            {
                return Ok(new Response<Guid?>
                {
                    Data = model.Id,
                    IsError = true,
                    Message = ex.Message,
                    Status = (int)StatusCodeEnum.BadRequest
                });
            }
        }

        [HttpDelete("{Id}")]
        public async Task<IActionResult> Delete(Guid Id, CancellationToken cancellationToken)
        {
            try
            {
                var navTitle = await _navTitleRepo.Find(a => a.Id == Id, false);
                if (navTitle is null)
                    return Ok(new Response<Guid>
                    {
                        Data = Id,
                        IsError = true,
                        Message = "Not found",
                        MessageAr = "هذا العنصر غير موجود",
                        Status = (int)StatusCodeEnum.NotFound
                    });
                _navTitleRepo.Delete(navTitle!);
                await _navTitleRepo.SaveAsync(cancellationToken);

                return Ok(new Response<Guid>
                {
                    Data = Id,
                    IsError = false,
                    Message = "Deleted Successfully",
                    MessageAr = "تم  الحذف بنجاح",
                    Status = (int)StatusCodeEnum.Ok
                });
            }
            catch (Exception ex)
            {
                return Ok(new Response<Guid?>
                {
                    Data = Id,
                    IsError = true,
                    Message = ex.Message,
                    Status = (int)StatusCodeEnum.BadRequest
                });
            }
        }
    }
}
