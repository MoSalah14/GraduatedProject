using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using OutbornE_commerce.BAL.Dto.ContactUs;
using Azure.Core;
using OutbornE_commerce.BAL.Dto.Newsletters;
using OutbornE_commerce.BAL.Dto;

namespace OutbornE_commerce.BAL.EmailServices
{
    public class EmailSender : IEmailSenderCustom
    {
        private readonly EmailSettings _mailSettings;

        public EmailSender(
            IOptions<EmailSettings> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }

        public async Task SendEmailAsync(string email, string subject, string? htmlMessage)
        {
            var serverData = _mailSettings;
            MailMessage message =  new MailMessage()
            {
                From = new MailAddress(serverData.Email!, serverData.DisplayName),
                Body = $"{htmlMessage}",
                Subject = subject,
                IsBodyHtml = true,
            };

            message.To.Add(email);

            SmtpClient smtpClient = new(serverData.Host)
            {
                Port = 587,
                Credentials = new NetworkCredential(serverData.Email, serverData.Password),
                EnableSsl = true,
            };

            await smtpClient.SendMailAsync(message);

            smtpClient.Dispose();
        }

        public async Task SendEmailContactUsAsync(ContactUsForCreationDto contact)
        {
            var serverData = _mailSettings;
            MailMessage message = new()
            {
                From = new MailAddress(serverData.Email!, serverData.DisplayName),
                Body = $"Name: {contact.FirstName} {contact.LastName}\nEmail: {contact.Email}\n\nMessage:\n{contact.Body}",
                Subject = contact.Subject,
                IsBodyHtml = true
            };

            message.To.Add(serverData.Email);


            message.ReplyToList.Add(new MailAddress(contact.Email));
            SmtpClient smtpClient = new(serverData.Host)
            {
                Port = _mailSettings.Port,
                Credentials = new NetworkCredential(serverData.Email, serverData.Password),
                EnableSsl = true,
            };

            await smtpClient.SendMailAsync(message);

            smtpClient.Dispose();
        }

        public async Task SendConfirmationEmailToUserAsync(string userEmail)
        {
            var mailSettings = _mailSettings;

            using var message = new MailMessage
            {
                From = new MailAddress(mailSettings.Email!, mailSettings.DisplayName),
                Subject = "Thank you for Contacting us!",
                Body = "We have Received your message and will get back to you shortly.",
                IsBodyHtml = true
            };

            message.To.Add(userEmail);

            using var smtpClient = new SmtpClient(mailSettings.Host)
            {
                Port = mailSettings.Port,
                Credentials = new NetworkCredential(mailSettings.Email, mailSettings.Password),
                EnableSsl = true,
            };

            await smtpClient.SendMailAsync(message);
        }
     
    }
}
