using Microsoft.AspNetCore.Identity.UI.Services;
using OutbornE_commerce.BAL.Dto;
using OutbornE_commerce.BAL.Dto.ContactUs;
using OutbornE_commerce.BAL.Dto.Newsletters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.EmailServices
{
    public interface IEmailSenderCustom : IEmailSender
    {
        Task SendEmailContactUsAsync(ContactUsForCreationDto contact);
        Task SendEmailToListAsync(SendMultipleEmailsDto sendMultipleEmailsDto);
        Task SendConfirmationEmailToUserAsync(string userEmail);
    }
}
