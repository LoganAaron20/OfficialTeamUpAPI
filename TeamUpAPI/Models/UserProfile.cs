using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using TeamUpAPI.Services;

namespace TeamUpAPI.Models
{
    // One-to-One relationship with User table
    public class UserProfile
    {

        [Key]
        public int ID { get; set; }

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }

        [MaxLength(100)]
        public string DisplayName { get; set; }

        [Required]
        [EmailAddress]
        public string EmailAddress { get; set; }

        [Required]
        public string Password { get; set; }

        public UserSettings UserSettings { get; set; }
        public AccessibilitySettings Accessibility { get; set; }
        public ContentPreferences ContentPreferences { get; set; }

        public byte[] ProfileImage { get; set; } = GetDefaultProfileImageBytes();
        public bool IsAdmin { get; set; } = false;
        public bool IsVerified { get; set; } = false;

        public void SetProfileImage(byte[] imageData)
        {
            ProfileImage = imageData;
        }

        public void HashAndSetPassword(string password)
        {
            this.Password = PasswordHasher.HashPassword(password);
        }

        private static byte[] GetDefaultProfileImageBytes()
        {
            string defaultImagePath = "Images/download.jfif"; // Adjust the path as needed
            byte[] defaultImageBytes;

            try
            {
                // Assuming the image is located at "TeamUpAPI/Images/download.jfif"
                string fullPath = Path.Combine(Directory.GetCurrentDirectory(), defaultImagePath);

                // Read the image bytes from the file
                defaultImageBytes = File.ReadAllBytes(fullPath);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error loading default profile image: {ex.Message}");
            }

            return defaultImageBytes;
        }
    }
}
