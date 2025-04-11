using OutbornE_commerce.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Dto.Tickets
{
    public class UpdateStatusTicket
    {
        public Guid Id { get; set; }
        public TicketStatus Status { get; set; }

    }
}
