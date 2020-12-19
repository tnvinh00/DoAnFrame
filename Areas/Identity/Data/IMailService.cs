using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sky.Areas.Identity.Data
{
    interface IMailService
    {
        Task SendEmailAsync(string email, string subject, string content);
    }
    public class SendGridEmailService : IMailService
    {
        public readonly IConfiguration _configuration;
        public SendGridEmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task SendEmailAsync(string email, string subject, string content)
        {
            var apiKey = _configuration.GetSection("SendGridAPIKey").Value;
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("test@meow.com", "Meow Demo");
            var to = new EmailAddress(email);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, content, content);
            var response = await client.SendEmailAsync(msg);
            //_ = await client.SendEmailAsync(msg);
        }
    }
}
