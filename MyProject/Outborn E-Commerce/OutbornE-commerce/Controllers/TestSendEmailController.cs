using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OutbornE_commerce.BAL.Dto.OrderDto;
using OutbornE_commerce.BAL.Dto.OrderItemDto;
using OutbornE_commerce.BAL.EmailServices;
using OutbornE_commerce.BAL.Repositories.SMTP_Server;

namespace OutbornE_commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestSendEmailController : ControllerBase
    {

        private readonly IWebHostEnvironment _env;
        private readonly ISMTPRepository _SMTPRepository;
        private readonly IEmailSenderCustom _emailSender;
        public TestSendEmailController(ISMTPRepository sMTPRepository,IEmailSenderCustom emailSenderCustom,IWebHostEnvironment env)
        {
            _env = env;
            _emailSender = emailSenderCustom;
            _SMTPRepository = sMTPRepository;
        }
        [HttpGet]
        [HttpGet]
        public async Task<IActionResult> TestEmail()
        {
            // Path to the template
            var templatePath = Path.Combine(_env.WebRootPath, "Templates", "ConfirmEmail.html");

            // Read the template content
            if (!System.IO.File.Exists(templatePath))
            {
                return NotFound("Email template not found.");
            }

            var emailContent = System.IO.File.ReadAllText(templatePath);

           
            //emailContent = emailContent
            //    .Replace("{{OrderNumber}}", testOrder.OrderNumber)
        //        .Replace("{{CreatedOn}}", testOrder.CreatedOn.ToString("yyyy-MM-dd HH:mm:ss"))
        //        .Replace("{{TotalAmount}}", testOrder.TotalAmount?.ToString("C") ?? "N/A");

        //    var orderItemsHtml = string.Empty;
        //    foreach (var item in testOrder.OrderItems)
        //    {
        //        orderItemsHtml += $@"
        //<li>
        //    <strong>{item.ProductNameEn}</strong> (Size: {item.Size}, Color: {item.ColorNameEn})<br />
        //    Quantity: {item.Quantity}, Price: {item.ItemPrice:C}
        //</li>";
        //    }

            //emailContent = emailContent.Replace("{{OrderItems}}", orderItemsHtml);

            //await _emailSender.SendEmailAsync("marwasaeed373@gmail.com", "OutBorn - Order Pending", emailContent);

            return Ok(emailContent);
        }


    }
}
