using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OutbornE_commerce.BAL.Dto;
using OutbornE_commerce.BAL.Dto.ProductColors;
using OutbornE_commerce.BAL.Dto.Tickets;
using OutbornE_commerce.BAL.EmailServices;
using OutbornE_commerce.BAL.Repositories.SMTP_Server;
using OutbornE_commerce.BAL.Repositories.Tickets;
using OutbornE_commerce.BAL.Repositories.UserRepo;
using OutbornE_commerce.DAL.Models;
using OutbornE_commerce.Extensions;

namespace OutbornE_commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketsController : ControllerBase
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly IEmailSenderCustom _emailSender;
        private readonly IUserReposatry userReposatry;
        private readonly ISMTPRepository _SMTPRepository;
        private string[] includes = { "user" };

        public TicketsController(ISMTPRepository _SMTPRepository, ITicketRepository ticketRepository, IEmailSenderCustom _emailSender, IUserReposatry userReposatry)
        {
            _ticketRepository = ticketRepository;
            this._emailSender = _emailSender;
            this.userReposatry = userReposatry;
            this._SMTPRepository = _SMTPRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTickets(int pageNumber = 1, int pageSize = 10, string? searchTerm = null)
        {
            var items = new PagainationModel<IEnumerable<Ticket>>();

            if (string.IsNullOrEmpty(searchTerm))
            {
                items = await _ticketRepository.FindAllAsyncByPagination(null, pageNumber, pageSize, includes);
            }
            else
            {
                items = await _ticketRepository.FindAllAsyncByPagination(
                    b => b.Description.Contains(searchTerm),
                    pageNumber,
                    pageSize,
                    includes
                );
            }

            var data = items.Data.Select(_ticketRepository.MapToDto).ToList();

            return Ok(new PaginationResponse<List<TicketDto>>
            {
                Data = data,
                IsError = false,
                Status = (int)StatusCodeEnum.Ok,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = items.TotalCount
            });
        }

        [HttpGet("GetAllTicketsForSpesificUser"), Authorize]
        public async Task<IActionResult> GetAllTicketsForSpesificUser(int pageNumber = 1, int pageSize = 10, string? searchTerm = null)
        {
            var items = new PagainationModel<IEnumerable<Ticket>>();
            var userId = User.GetUserIdFromToken();
            if (userId == null)
            {
                return Unauthorized(new Response<string>
                {
                    Data = null,
                    IsError = true,
                    Message = "Please Login First",
                    MessageAr = "برجاء تسجيل الدخول اولا",
                    Status = (int)StatusCodeEnum.Unauthorized
                });
            }

            if (string.IsNullOrEmpty(searchTerm))
            {
                items = await _ticketRepository.FindAllAsyncByPagination(x => x.UserId == userId, pageNumber, pageSize, includes);
            }
            else
            {
                items = await _ticketRepository.FindAllAsyncByPagination(
                    b => b.Description.Contains(searchTerm),
                    pageNumber,
                    pageSize,
                    includes
                );
            }

            var data = items.Data.Select(_ticketRepository.MapToDto).ToList();

            return Ok(new PaginationResponse<List<TicketDto>>
            {
                Data = data,
                IsError = false,
                Status = (int)StatusCodeEnum.Ok,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = items.TotalCount
            });
        }

        [HttpGet("GetAllPendingTickets")]
        public async Task<IActionResult> GetAllPendingTickets(int pageNumber = 1, int pageSize = 10, string? searchTerm = null)
        {
            var items = new PagainationModel<IEnumerable<Ticket>>();

            if (string.IsNullOrEmpty(searchTerm))
                items = await _ticketRepository.FindAllAsyncByPagination(t => t.Status == TicketStatus.Pending, pageNumber, pageSize, includes);
            else
                items = await _ticketRepository
                                    .FindAllAsyncByPagination(b => (b.Description.Contains(searchTerm))
                                                               , pageNumber, pageSize, includes);

            var data = items.Data.Select(_ticketRepository.MapToDto).ToList();

            return Ok(new PaginationResponse<List<TicketDto>>
            {
                Data = data,
                IsError = false,
                Status = (int)StatusCodeEnum.Ok,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = items.TotalCount
            });
        }

        [HttpGet("GetAllSlovedTickets")]
        public async Task<IActionResult> GetAllSlovedTickets(int pageNumber = 1, int pageSize = 10, string? searchTerm = null)
        {
            var items = new PagainationModel<IEnumerable<Ticket>>();

            if (string.IsNullOrEmpty(searchTerm))
                items = await _ticketRepository.FindAllAsyncByPagination(t => t.Status == TicketStatus.Sloved, pageNumber, pageSize, includes);
            else
                items = await _ticketRepository
                                    .FindAllAsyncByPagination(b => (b.Description.Contains(searchTerm))
                                                               , pageNumber, pageSize, includes);

            var data = items.Data.Select(_ticketRepository.MapToDto).ToList();

            return Ok(new PaginationResponse<List<TicketDto>>
            {
                Data = data,
                IsError = false,
                Status = (int)StatusCodeEnum.Ok,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = items.TotalCount
            });
        }

        [HttpGet("GetAllClosedTickets")]
        public async Task<IActionResult> GetAllClosedTickets(int pageNumber = 1, int pageSize = 10, string? searchTerm = null)
        {
            var items = new PagainationModel<IEnumerable<Ticket>>();

            if (string.IsNullOrEmpty(searchTerm))
                items = await _ticketRepository.FindAllAsyncByPagination(t => t.Status == TicketStatus.Closed, pageNumber, pageSize, includes);
            else
                items = await _ticketRepository
                                    .FindAllAsyncByPagination(b => (b.Description.Contains(searchTerm))
                                                               , pageNumber, pageSize, includes);

            var data = items.Data.Select(_ticketRepository.MapToDto).ToList();

            return Ok(new PaginationResponse<List<TicketDto>>
            {
                Data = data,
                IsError = false,
                Status = (int)StatusCodeEnum.Ok,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = items.TotalCount
            });
        }

        [HttpGet("GetAllWorkingOnTickets")]
        public async Task<IActionResult> GetAllWorkingOnTickets(int pageNumber = 1, int pageSize = 10, string? searchTerm = null)
        {
            var items = new PagainationModel<IEnumerable<Ticket>>();

            if (string.IsNullOrEmpty(searchTerm))
                items = await _ticketRepository.FindAllAsyncByPagination(t => t.Status == TicketStatus.WorkingOn, pageNumber, pageSize, includes);
            else
                items = await _ticketRepository
                                    .FindAllAsyncByPagination(b => (b.Description.Contains(searchTerm))
                                                               , pageNumber, pageSize, includes);

            var data = items.Data.Select(_ticketRepository.MapToDto).ToList();

            return Ok(new PaginationResponse<List<TicketDto>>
            {
                Data = data,
                IsError = false,
                Status = (int)StatusCodeEnum.Ok,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = items.TotalCount
            });
        }

        [HttpGet("GetTicketById/{Id}")]
        public async Task<IActionResult> GetTicketById(Guid Id)
        {
            var ticket = await _ticketRepository.Find(t => t.Id == Id, false, includes);
            if (ticket == null)
                return Ok(new Response<TicketDto>
                {
                    Data = null,
                    IsError = true,
                    Status = (int)StatusCodeEnum.NotFound,
                    Message = $"Ticket with Id : {Id} doesn't exist in the database",
                    MessageAr = "لم يتم العثور علي هذا الحجز"
                });
            var ticketEntity = _ticketRepository.MapToDto(ticket);
            return Ok(new Response<TicketDto>
            {
                Data = ticketEntity,
                IsError = false,
                Status = (int)StatusCodeEnum.Ok,
                Message = ""
            });
        }

        [HttpPost, Authorize]
        public async Task<IActionResult> CreateTicket([FromBody] TicketForCreationDto model, CancellationToken cancellationToken)
        {
            await _ticketRepository.BeginTransactionAsync();
            try
            {
                var userId = User.GetUserIdFromToken();
                if (userId == null)
                {
                    return Unauthorized(new Response<string>
                    {
                        Data = null,
                        IsError = true,
                        Message = "Please Login First",
                        MessageAr = "برجاء تسجيل الدخول اولا",
                        Status = (int)StatusCodeEnum.Unauthorized
                    });
                }
                var ticket = new Ticket()
                {
                    UserId = userId,
                    Description = model.Description,
                    CreatedBy = "Admin",
                    CreatedOn = DateTime.Now
                };
                var result = await _ticketRepository.Create(ticket);
                await _ticketRepository.SaveAsync(cancellationToken);
                string EmailUser = await userReposatry.GetSpecificUserEmailAsync(userId);
                if (EmailUser != null)
                {
                    await _emailSender.SendEmailAsync(EmailUser, "Hello OutBorn Online Shopping!", model.Description);
                }
                await _ticketRepository.CommitTransactionAsync();

                return Ok(new Response<Guid>()
                {
                    Data = result.Id,
                    IsError = false,
                    Message = "Created successfully",
                    MessageAr = "تم الاضافه بنجاح",
                    Status = (int)StatusCodeEnum.Ok
                });
            }
            catch (Exception ex)
            {
                await _ticketRepository.RollbackTransactionAsync();

                return BadRequest(new Response<string>()
                {
                    Data = "An error occurred while creating the ticket.",
                    IsError = true,
                    Status = (int)StatusCodeEnum.ServerError,
                    Message = ex.Message
                });
            }
        }

        //[HttpPut("UpdateTicket/{Id}")]
        //public async Task<IActionResult> UpdateTicket( [FromBody] TicketDto model,  CancellationToken cancellationToken)
        //{
        //    var ticket = await _ticketRepository.Find(t => t.Id == model.Id, false);
        //    if(ticket == null)
        //        return Ok(new Response<TicketDto>
        //        {
        //            Data = null,
        //            IsError = true,
        //            Status = (int)StatusCodeEnum.NotFound,
        //            Message = $"Ticket with Id : {model.Id} doesn't exist in the database"
        //        });
        //    ticket = model.Adapt<Ticket>();
        //    ticket.UpdatedBy = "user";
        //    _ticketRepository.Update(ticket);
        //    await _ticketRepository.SaveAsync(cancellationToken);
        //    return Ok(new Response<Guid>()
        //    {
        //        Data = ticket.Id,
        //        IsError = false,
        //        Status = (int)StatusCodeEnum.Ok

        //    });
        //}

        [HttpPut("UpdateStatusTicket")]
        public async Task<IActionResult> UpdateStatusTicket([FromBody] UpdateStatusTicket model, CancellationToken cancellationToken)
        {
            var ticket = await _ticketRepository.Find(t => t.Id == model.Id, false);
            if (ticket == null)
                return Ok(new Response<TicketDto>
                {
                    Data = null,
                    IsError = true,
                    Status = (int)StatusCodeEnum.NotFound,
                    Message = $"Ticket with Id : {model.Id} doesn't exist in the database"
                });
            ticket.Status = model.Status;
            ticket.UpdatedBy = "Admin";
            _ticketRepository.Update(ticket);
            await _ticketRepository.SaveAsync(cancellationToken);
            return Ok(new Response<Guid>()
            {
                Data = ticket.Id,
                Message = "Updated successfully",
                MessageAr = "تم التعديل بنجاح",
                IsError = false,
                Status = (int)StatusCodeEnum.Ok
            });
        }

        [HttpDelete("Id")]
        public async Task<IActionResult> DeleteTicket(Guid Id, CancellationToken cancellationToken)
        {
            var ticket = await _ticketRepository.Find(t => t.Id == Id, false);
            if (ticket == null)
                return Ok(new Response<TicketDto>
                {
                    Data = null,
                    IsError = true,
                    Message = $"Ticket with Id : {Id} doesn't exist in the database",
                    MessageAr = "لم يتم العثور علي التذكرة",
                });
            _ticketRepository.Delete(ticket);
            await _ticketRepository.SaveAsync(cancellationToken);
            return Ok(new Response<Guid>()
            {
                Data = ticket.Id,
                IsError = false,
                Status = (int)StatusCodeEnum.Ok,
                Message = "Deleted Successfully",
                MessageAr = "تم الحذف بنجاح",
            });
        }
    }
}