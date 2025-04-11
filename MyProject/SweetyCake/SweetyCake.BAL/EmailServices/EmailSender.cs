using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using OutbornE_commerce.BAL.Repositories.SMTP_Server;
using OutbornE_commerce.BAL.Dto.ContactUs;
using Azure.Core;
using OutbornE_commerce.BAL.Dto.Newsletters;
using OutbornE_commerce.BAL.Dto;

namespace OutbornE_commerce.BAL.EmailServices
{
    public class EmailSender : IEmailSenderCustom
    {
        private readonly EmailSettings _mailSettings;
        private readonly ISMTPRepository _sMTPRepository;

        public EmailSender(
            IOptions<EmailSettings> mailSettings, ISMTPRepository sMTPRepository)
        {
            _mailSettings = mailSettings.Value;
            _sMTPRepository = sMTPRepository;
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
        public async Task SendEmailToListAsync(SendMultipleEmailsDto sendMultipleEmailsDto)
        {
            try
            {
                var serverData = (await _sMTPRepository.FindAllAsync(null)).FirstOrDefault();
                if (serverData == null)
                {
                    throw new InvalidOperationException("SMTP server data not found.");
                }

                using (var message = new MailMessage())
                {
                    message.From = new MailAddress(serverData.Email!, serverData.DisplayName);
                    message.Subject = sendMultipleEmailsDto.Subject;
                    message.Body = sendMultipleEmailsDto.Body; 
                    message.IsBodyHtml = true;

                    foreach (var email in sendMultipleEmailsDto.Emails)
                    {
                        message.To.Add(email);
                    }

                    using (var smtpClient = new SmtpClient(serverData.Host))
                    {
                        smtpClient.Port = 587;
                        smtpClient.Credentials = new NetworkCredential(serverData.Email, serverData.Password);
                        smtpClient.EnableSsl = true;

                        await smtpClient.SendMailAsync(message);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
            }
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
