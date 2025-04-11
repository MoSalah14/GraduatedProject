using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OutbornE_commerce.BAL.Dto.Hashtags;
using OutbornE_commerce.BAL.Dto.Headers;
using OutbornE_commerce.BAL.Repositories.Hashtags;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace OutbornE_commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HashtagsController : ControllerBase
    {
        private readonly IHashtagRepository _hashtagRepository;

        public HashtagsController(IHashtagRepository hashtagRepository)
        {
            _hashtagRepository = hashtagRepository;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllHashtags()
        {
            var hashtags = await _hashtagRepository.FindAllAsync(null);
            var hashtagEntites = hashtags.Adapt<List<HashtagDto>>();
            return Ok(new Response<List<HashtagDto>>
            {
                Data = hashtagEntites,
                IsError = false,
                Status = (int)StatusCodeEnum.Ok,
            });
        }
        [HttpGet("{Id}")]
        public async Task<IActionResult> GetHashtagById(Guid Id)
        {
            var hashtag = await _hashtagRepository.Find(h => h.Id == Id);
            var hashtagEntity = hashtag.Adapt<HashtagDto>();
            if (hashtag is null)
                return Ok(new Response<HashtagDto>
                {
                    Data = null,
                    IsError = true,
                    Status = (int) StatusCodeEnum.NotFound,
                    Message = ""
                });
            return Ok(new Response<HashtagDto>
            {
                Data = hashtagEntity,
                IsError = false,
                Status = (int)StatusCodeEnum.Ok,
                Message = ""
            });
        }
        [HttpPost]
        public async Task<IActionResult> CreateHashtag([FromBody] HashtagForCreationDto model , CancellationToken cancellationToken)
        {
            var hastag = model.Adapt<Hashtag>();
            hastag.CreatedBy = "admin";
            var result = await _hashtagRepository.Create(hastag);
            await _hashtagRepository.SaveAsync(cancellationToken);
            return Ok(new Response<Guid>
            {
                Data = result.Id,
                IsError = false,
                Message = "Success",
                MessageAr = "تم بنجاح",
                Status = (int)StatusCodeEnum.Ok
            });
        }
        [HttpPut]
        public async Task<IActionResult> UpdateHashtag([FromBody] HashtagDto model , CancellationToken cancellationToken)
        {
            var hashtag = await _hashtagRepository.Find(h => h.Id == model.Id);
            hashtag = model.Adapt<Hashtag>();
            hashtag.CreatedBy = "admin";
            _hashtagRepository.Update(hashtag);
            await _hashtagRepository.SaveAsync(cancellationToken);
            return Ok(new Response<Guid>
            {
                Data= hashtag.Id,
                IsError=false,
                Message = "Success",
                MessageAr = "تم التعديل  بنجاح",
                Status = (int)StatusCodeEnum.Ok
            });
        }
        [HttpDelete("{Id}")]
        public async Task<IActionResult> DeleteHashtag(Guid Id , CancellationToken cancellationToken)
        {
            var hashtag = await _hashtagRepository.Find(h => h.Id == Id, true);
            if (hashtag is null)
                return Ok(new Response<Guid>
                {
                    Data = Id,
                    IsError = true,
                    Message = "",
                    Status = (int)StatusCodeEnum.NotFound
                });
            _hashtagRepository.Delete(hashtag);
            await _hashtagRepository.SaveAsync(cancellationToken);
            return Ok(new Response<Guid>
            {
                Data = hashtag.Id,
                IsError=false,
                Message = "Deleted Successfully",
                MessageAr = "تم  الحذف بنجاح",
                Status = (int)StatusCodeEnum.Ok
            });
        }
    }
}
