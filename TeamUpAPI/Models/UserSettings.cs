using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace TeamUpAPI.Models
{
    public class UserSettings
    {
        [Key]
        public int UserID { get; set; }

        public string Language { get; set; } = "EN-US";
        public string TimeZone { get; set; } = "EST";
        public string ThemePreference { get; set; } = "";
        public bool EnableTwoFactorAuthentication { get; set; } = false;
        public bool AllowEmailNotification { get; set; } = false;
        public PrivacySetting ProfilePrivacy { get; set; }
        public string DisplayName { get; set; } = "";
        public string Bio { get; set; } = "";
        public string SocialMediaLinks { get; set; } = "";
        
        public UserProfile UserProfile { get; set; }
    }

    public enum PrivacySetting
    {
        Public,
        FriendsOnly,
        Private
    }

    public class AccessibilitySettings
    {
        [Key]
        public int ID { get; set; }
        public int UserID { get; set; }
        public string TextSize { get; set; } = "D";
        public string Contrast { get; set; } = "D";
        public UserProfile UserProfile { get; set; }
    }

    public class ContentPreferences
    {
        [Key]
        public int ID { get; set; }
        public int UserID { get; set; }
        public bool ShowImages { get; set; } = true;
        public bool ShowVideos { get; set; } = true;
        public UserProfile UserProfile { get; set; }
    }
}
