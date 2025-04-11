using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OutbornE_commerce.BAL.Dto.HomeSections;
using OutbornE_commerce.BAL.Repositories.HomeSections;
using OutbornE_commerce.DAL.Models;
using OutbornE_commerce.FilesManager;
using System.Reflection.PortableExecutable;
using static System.Collections.Specialized.BitVector32;

namespace OutbornE_commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeSectionsController : ControllerBase
    {
        private readonly IHomeSectionRepository _homeSectionRepository;
        private readonly IFilesManager _filesManager;

        public HomeSectionsController(IHomeSectionRepository homeSectionRepository, IFilesManager filesManager)
        {
            _homeSectionRepository = homeSectionRepository;
            _filesManager = filesManager;
        }
        [HttpGet]
        public async Task<IActionResult> GetHomeSection()
        {
            var homeSection = (await _homeSectionRepository.FindAllAsync(null, false));
            var homeEntity = homeSection.Adapt<List<HomeSectionDto>>();
            return Ok(new Response<List<HomeSectionDto>>
            {
                Data = homeEntity,
                IsError = false,
                Status = (int)StatusCodeEnum.Ok
            });
        }
        [HttpPost]
        public async Task<IActionResult> CreateHomeSection([FromForm] HomeSectionForCreationDto model , CancellationToken cancellationToken)
        {
            var homeSection = await _homeSectionRepository.FindAllAsync(null, false);
            if (homeSection.Any())
                return Ok(new { message = "You have already inserted a home section" });
            var section = model.Adapt<HomeSection>();
            section.CreatedBy = "admin";
            if (model.Image != null)
            {
                var fileModel = await _filesManager.UploadFile(model.Image, "HomeSections");
                section.ImageUrl = fileModel!.Url;
            }
            var result = await _homeSectionRepository.Create(section);
            await _homeSectionRepository.SaveAsync(cancellationToken);
            return Ok(new Response<Guid>
            {
                Data = result.Id,
                IsError = false,
                Status = (int)StatusCodeEnum.Ok,
                Message = "Success",
                MessageAr = "تم بنجاح",
            });
        }
        [HttpPut]
        public async Task<IActionResult> UpdateHomeSection([FromForm] HomeSectionDto model , CancellationToken cancellationToken)
        {
            var homeSection = await _homeSectionRepository.Find(s => s.Id == model.Id, false);

            if (homeSection == null)
                return Ok(new {message = $"Home Section with Id : {homeSection!.Id} doesn't exist in the database"});
            var OldImage = homeSection.ImageUrl;
            homeSection = model.Adapt<HomeSection>();
            homeSection.CreatedBy = "admin";
            if (model.Image != null)
            {
                var fileModel = await _filesManager.UploadFile(model.Image, "HomeSections",OldImage);
                homeSection.ImageUrl = fileModel!.Url;
            }
            _homeSectionRepository.Update(homeSection);
            await _homeSectionRepository.SaveAsync(cancellationToken);
            return Ok(new Response<Guid>
            {
                Data = model.Id,
                IsError = false,
                Status = (int)StatusCodeEnum.Ok,
                Message = "Success",
                MessageAr = "تم التعديل بنجاح",
            });
        }
        [HttpDelete("{Id}")]
        public async Task<IActionResult> DeleteHomeSection(Guid Id, CancellationToken cancellationToken)
        {
            var homeSection = await _homeSectionRepository.Find(h => h.Id == Id, false);
            if (homeSection == null)
                return Ok(new { message = $"Home Section with Id: {homeSection!.Id} doesn't exist in the database" });
            _homeSectionRepository.Delete(homeSection);
            await _homeSectionRepository.SaveAsync(cancellationToken);
            return Ok(new Response<Guid>
            {
                Data = Id,
                IsError = false,
                Status = (int)StatusCodeEnum.Ok,
                Message = "Deleted Successfully",
                MessageAr = "تم  الحذف بنجاح",
            });
        }
    }
}
