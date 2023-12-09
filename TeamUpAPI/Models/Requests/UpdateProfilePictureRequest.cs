namespace TeamUpAPI.Models.Requests
{
    public class UpdateProfilePictureRequest
    {
        public int UserId { get; set; }
        public IFormFile ProfilePicture { get; set; }
    }
}
