﻿using OutbornE_commerce.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Dto.Tickets
{
    public class TicketDto
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public string UserId { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}