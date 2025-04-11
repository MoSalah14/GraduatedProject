using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.DAL.Models
{
    public class Ticket : BaseEntity
    {
        public string UserId { get; set; }
        public User? user { get; set; }
        public string Description { get; set; }
        public TicketStatus Status { get; set; }

    }
    public enum TicketStatus
    {
        Pending,
        WorkingOn,
        Closed,
        Sloved
    }

}
