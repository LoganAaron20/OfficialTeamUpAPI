namespace TeamUpAPI.Models
{
    // One-to-One relationship with UserProfile table
    public class User
    {
        public int ID { get; set; }
        public string UserName { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsVerified { get; set; }
        public bool IsActive { get; set; }
        public bool PasswordVerified { get; set; }

        // Navigation property to UserProfile
        public UserProfile UserProfile { get; set; }
    }
}
