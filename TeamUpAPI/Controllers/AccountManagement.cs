using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using TeamUpAPI.Models;
using TeamUpAPI.Models.Requests;
using TeamUpAPI.Services;
using TeamUpAPI.Services.Interfaces;
namespace TeamUpAPI.Controllers
{
    public class AccountManagement : ControllerBase
    {
        private readonly TeamUpContext DB;
        private readonly ILogger<AccountManagement> Logger;
        private readonly IEmailService _emailService;

        public AccountManagement(TeamUpContext context, ILogger<AccountManagement> logger, IEmailService emailService)
        {
            this.DB = context;
            this.Logger = logger;
            this._emailService = emailService;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.UserName) || string.IsNullOrEmpty(request.Password))
            {
                Logger.LogInformation("A bad login request was just received.");
                return BadRequest("Invalid login request");
            }

            UserProfile loggedInUser = await LoginUser(request.UserName, request.Password);

            if (loggedInUser == null)
            {
                return NoContent();
            }

            return Ok(loggedInUser);

        }

        private async Task<UserProfile> LoginUser(string username, string password)
        {
            try
            {
                // Find the user
                var user = await (from x in DB.Users where x.DisplayName == username || x.EmailAddress == username select x).FirstOrDefaultAsync();

                if (user == null)
                {
                    Logger.LogWarning($"A query was made on a user that does not exist: {username}");
                    return null;
                }

                // We have found the user now. Now we need to verify their password in order to log them in
                if (!VerifyPassword(password, user))
                {
                    Logger.LogInformation($"A bad login attempt was made by: {username}");
                    return null;
                }

                // Password matches
                return user;
            }
            catch (Exception ex)
            {
                Logger.LogError($"There was an exception thrown will logging in user: {username}. Error: {ex.Message}");
                return null;
            }
        }

        private bool VerifyPassword(string password, UserProfile user)
        {
            return PasswordHasher.VerifyPassword(user.Password, password) ? true : false;
        }
    }
}
