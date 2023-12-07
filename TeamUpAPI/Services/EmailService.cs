using System.Net;
using System.Net.Mail;
using TeamUpAPI.Services.Interfaces;

namespace TeamUpAPI.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var smtpServer = _configuration["EmailSettings:SmtpServer"];
            var port = int.Parse(_configuration["EmailSettings:Port"]);
            var userName = _configuration["EmailSettings:UserName"];
            var password = _configuration["EmailSettings:Password"];
            var senderEmail = _configuration["EmailSettings:SenderEmail"];
            var senderName = _configuration["EmailSettings:SenderName"];

            using (var client = new SmtpClient(smtpServer, port))
            {
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(userName, password);
                client.EnableSsl = true;

                var message = new MailMessage
                {
                    From = new MailAddress(senderEmail, senderName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true,
                };

                message.To.Add(toEmail);

                await client.SendMailAsync(message);
            }
        }

        public string GenerateWelcomeBody(string userName)
        {
            string welcomeBody = $@"
            <html>
            <head>
                <title>Welcome to Your Application</title>
            </head>
            <body>
                <h2>Dear {userName},</h2>
                <p>Welcome to Your Application! We are excited to have you on board.</p>
                <p>Feel free to explore our features and reach out if you have any questions.</p>
                <p>Best regards,<br>Your Application Team</p>
            </body>
            </html>
            ";

            return welcomeBody;
        }
    }
}
