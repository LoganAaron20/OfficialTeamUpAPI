namespace TeamUpAPI.Models.Requests
{
    public class UserSettingsRequest
    {
        public int UserId { get; set; }
        public string Language { get; set; }
        public string TimeZone { get; set; }
        public string ThemePreference { get; set; }
        public bool EnableTwoFactorAuthentication { get; set; }
        public bool AllowEmailNotification { get; set; }
        public PrivacySetting ProfilePrivacy { get; set; }
        public string DisplayName { get; set; }
        public string Bio { get; set; }
        public string SocialMediaLinks { get; set; }
    }
}
