using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using TeamUpAPI.Models;
using TeamUpAPI.Models.Requests;
using TeamUpAPI.Services;
using TeamUpAPI.Services.Interfaces;

namespace TeamUpAPI.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly TeamUpContext DB;
        private readonly ILogger<UsersController> Logger;
        private readonly IEmailService _emailService;

        public UsersController(TeamUpContext context, ILogger<UsersController> logger, IEmailService emailService)
        {
            this.DB = context;
            this.Logger = logger;
            this._emailService = emailService;
        }

        [HttpGet("{userID}")]
        public async Task<IActionResult> GetUserById(int userId)
        {
            try
            {
                var user = await DB.Users.FindAsync(userId);

                if (user == null)
                {
                    Logger.LogInformation($"User with ID {userId} not found.");
                    return NotFound($"User with ID {userId} not found.");
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                Logger.LogError($"An error occurred while retrieving user with ID {userId}: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPost]
        [Route("CreateUser")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest model)
        {
            try
            {
                if (model == null || string.IsNullOrEmpty(model.DisplayName))
                {
                    Logger.LogError("Invalid UserName.");
                    return BadRequest("Invalid UserName.");
                }

                if (model == null || string.IsNullOrEmpty(model.FirstName))
                {
                    Logger.LogError("Invalid First Name.");
                    return BadRequest("Invalid First Name.");
                }

                if (model == null || string.IsNullOrEmpty(model.LastName))
                {
                    Logger.LogError("Invalid Last Name.");
                    return BadRequest("Invalid Last Name.");
                }

                // Check if the username is already taken
                if (!CheckUsernameAvailability(model.DisplayName))
                {
                    return Conflict($"Username '{model.DisplayName}' is already taken.");
                }

                // Hash password
                string hashedPassword = PasswordHasher.HashPassword(model.Password);

                // Create new user profile
                var newUser = new UserProfile
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    DisplayName = model.DisplayName,
                    Password = hashedPassword,
                    EmailAddress = model.Email,
                };



                DB.Users.Add(newUser);

                await DB.SaveChangesAsync();

                var createdUser = await DB.Users.SingleAsync(u => u.DisplayName == newUser.DisplayName);

                if (!await CreateNewUserSettings(createdUser))
                {
                    return StatusCode(500, "Failure on user settings creation.");
                }

                Logger.LogInformation($"User '{model.DisplayName}' created successfully.");

                _emailService.SendEmailAsync(newUser.EmailAddress, "Welcome to TeamUp!", _emailService.GenerateWelcomeBody(newUser.DisplayName));

                return Ok(newUser);

            }
            catch (Exception ex)
            {
                Logger.LogError($"An error occurred while creating user: {ex.Message} ");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPost]
        [Route("RemoveUser/{userId}")]
        public async Task<IActionResult>RemoveUser(int userId)
        {
            try
            {
                var user = await DB.Users.FindAsync(userId);

                if (user == null)
                {
                    Logger.LogWarning($"Could not find user with ID: {userId}");
                    return NotFound($"Could not find user with ID: {userId}");
                }

                DB.Users.Remove(user);
                await DB.SaveChangesAsync();

                return Ok($"User deleted.");
            }
            catch (Exception ex)
            {
                Logger.LogError($"An error occurred while removing user: {userId}. {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPost]
        [Route("UpdateSettings")]
        public async Task<IActionResult> UpdateUserSettings([FromBody] UserSettingsRequest request)
        {
            try
            {
                var foundUserSetting = await DB.Settings.FindAsync(request.UserId);

                if (foundUserSetting == null)
                {
                    Logger.LogWarning($"Hmmm... It seems as if user: {request.UserId} is missing settings.");
                    return NotFound($"Could not find settings for user: {request.UserId}");
                }

                // Update the settings
                foundUserSetting.Language = request.Language;
                foundUserSetting.TimeZone = request.TimeZone;
                foundUserSetting.ThemePreference = request.ThemePreference;
                foundUserSetting.EnableTwoFactorAuthentication = request.EnableTwoFactorAuthentication;
                foundUserSetting.AllowEmailNotification = request.AllowEmailNotification;
                foundUserSetting.ProfilePrivacy = request.ProfilePrivacy;
                foundUserSetting.DisplayName = request.DisplayName;
                foundUserSetting.Bio = request.Bio;
                foundUserSetting.SocialMediaLinks = request.SocialMediaLinks;

                await DB.SaveChangesAsync();

                Logger.LogInformation($"Settings updated successfully for user: {request.UserId}");

                return Ok("Settings updated successfully");

            }
            catch (Exception ex)
            {
                Logger.LogError($"An error occurred while updating the settings for user: {request.UserId} ###{ex.Message}###");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPost]
        [Route("UpdateProfilePicture")]
        public async Task<IActionResult> UpdateProfilePicture([FromBody] UpdateProfilePictureRequest request)
        {
            try
            {
                // Validate the request
                if (request == null || request.ProfilePicture == null || request.ProfilePicture.Length == 0)
                {
                    Logger.LogWarning($"Invalid request for updating user: {request.UserId}'s profile picture");
                    return BadRequest($"Invalid request for updating user: {request.UserId}'s profile picture");
                }

                var user = await DB.Users.FindAsync(request.UserId);

                if (user == null)
                {
                    Logger.LogWarning($"Could not find user with ID: {request.UserId}");
                    return NotFound($"Could not find user with ID: {request.UserId}");
                }

                using (var stream = new MemoryStream())
                {
                    await request.ProfilePicture.CopyToAsync(stream);
                    user.SetProfileImage(stream.ToArray());
                }

                await DB.SaveChangesAsync();

                Logger.LogInformation($"Profile picture updated successfully for user: {request.UserId}");

                return Ok("Profile picture updated successfully");
            }
            catch (Exception ex)
            {
                Logger.LogError($"An error occurred while updating the profile image for user: {request.UserId}. Error: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        private bool CheckUsernameAvailability(string username)
        {
            if (DB.Users.Any(u => u.DisplayName == username))
            {
                Logger.LogWarning($"Username '{username}' is already taken.");
                return false;
            }
            return true;
        }

        private async Task<bool> CreateNewUserSettings(UserProfile newUser)
        {
            try
            {
                var newUserSettings = new UserSettings
                {
                    UserID = newUser.ID,
                    UserProfile = newUser,
                };

                var newUserAccessibilitySettings = new AccessibilitySettings
                {
                    UserID = newUser.ID,
                    UserProfile = newUser,
                };

                var newUserContentPreferences = new ContentPreferences
                {
                    UserID = newUser.ID,
                    UserProfile = newUser,
                };

                DB.Settings.Add(newUserSettings);
                DB.AccessibilitySettings.Add(newUserAccessibilitySettings);
                DB.ContentPreferences.Add(newUserContentPreferences);

                await DB.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error creating settings for user: {newUser.ID} ###{ex.Message}###");
                return false;
            }
        }
    }
}
