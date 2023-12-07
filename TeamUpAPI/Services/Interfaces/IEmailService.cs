namespace TeamUpAPI.Services.Interfaces
{
    public interface IEmailService
    {
        public Task SendEmailAsync(string toEmail, string subject, string body);
        public string GenerateWelcomeBody(string userName);
    }
}
