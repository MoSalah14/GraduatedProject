using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OutbornE_commerce.BAL.Dto;
using OutbornE_commerce.BAL.Dto.SMTP_Server;
using OutbornE_commerce.BAL.Repositories.SMTP_Server;
using OutbornE_commerce.DAL.Models;

namespace OutbornE_commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SMTPController : ControllerBase
    {
        private readonly ISMTPRepository _sMTPRepository;
        public SMTPController(ISMTPRepository sMTPRepository)
        {
            _sMTPRepository = sMTPRepository;
        }
        [HttpGet]
        public async Task<IActionResult> GetSMTPSERVER()
        {
            var smtp = await _sMTPRepository.FindAllAsync(null, false);
            var smtpEntites = smtp.Adapt<List<SMTPDto>>();
            return Ok(new Response<List<SMTPDto>>()
            {
                Data = smtpEntites,
                IsError = false,
                Status = (int)StatusCodeEnum.Ok

            });
        }
        [HttpPost]
        public async Task<IActionResult> CreateSMTPServer([FromBody] SMTPForCreationDto model , CancellationToken cancellationToken)
        {
            var smtpServer = await _sMTPRepository.FindAllAsync(null, false);
            if(smtpServer.Any())
                return Ok(new Response<SMTPDto>{
                    Data = null,
                    IsError = true,
                    Message = "You have already inserted a SMTP Server before ",
                    Status = (int)StatusCodeEnum.NotFound
                    //Status = (StatusCode)2
            });;
            var smtp = model.Adapt<SMTPServer>();
            smtp.CreatedBy = "admin";
            var result = await _sMTPRepository.Create(smtp);
            await _sMTPRepository.SaveAsync(cancellationToken);
              return Ok(new Response<Guid>()
            {
                Data = smtp.Id,
                IsError = false,
                Status = (int)StatusCodeEnum.Ok

            });
        }
        [HttpPut]
        public async Task<IActionResult> UpdateSMTPServer([FromBody] SMTPDto model, CancellationToken cancellationToken)
        {
            var smtp = await _sMTPRepository.Find(s => s.Id == model.Id, false);
            if (smtp is null)
                return Ok(new Response<SMTPDto>
                {
                    Data = null,
                    IsError = true,
                    Message = $"SMTP Server with Id: {model.Id} doesn't exist in the database",
                    //Status = (StatusCode)2
                });
            smtp = model.Adapt<SMTPServer>();
            smtp.CreatedBy = "admin";
            _sMTPRepository.Update(smtp);
            await _sMTPRepository.SaveAsync(cancellationToken);
            return Ok(new Response<Guid>()
            {
                Data = smtp.Id,
                IsError = false,
                Status = (int)StatusCodeEnum.Ok

            });
        }
        [HttpDelete("Id")]
        public async Task<IActionResult> DeleteSMTPServer(Guid Id, CancellationToken cancellationToken)
        {
            var smtp = await _sMTPRepository.Find(s => s.Id == Id, false);
            if (smtp is null)
                return Ok(new Response<SMTPDto>
                {
                    Data = null,
                    IsError = true,
                    Message = $"SMTP Server with Id: {Id} doesn't exist in the database",
                    //Status = (StatusCode)2
                });
            _sMTPRepository.Delete(smtp);
            await _sMTPRepository.SaveAsync(cancellationToken) ;
            return Ok(new Response<Guid>()
            {
                Data = smtp.Id,
                IsError = false,
                Status = (int)StatusCodeEnum.Ok

            });
        }
    }
}
