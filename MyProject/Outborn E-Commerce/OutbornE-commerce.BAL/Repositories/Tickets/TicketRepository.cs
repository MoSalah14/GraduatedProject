using Microsoft.AspNetCore.Http.HttpResults;
using OutbornE_commerce.BAL.Dto.Tickets;
using OutbornE_commerce.BAL.Repositories.BaseRepositories;
using OutbornE_commerce.DAL.Data;
using OutbornE_commerce.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Repositories.Tickets
{
    public class TicketRepository : BaseRepository<Ticket>, ITicketRepository
    {
        public TicketRepository(ApplicationDbContext context) : base(context)
        {
        }

        public TicketDto MapToDto(Ticket ticket)
        {
            return new TicketDto
            {
                Id = ticket.Id,
                Description = ticket.Description,
                UserId = ticket.UserId,
                Email = ticket.user?.Email ?? string.Empty,
                Name = ticket.user?.FullName ?? string.Empty,
                Status = ticket.Status.ToString(),
                CreatedOn = ticket.CreatedOn
            };
        }
    }
}