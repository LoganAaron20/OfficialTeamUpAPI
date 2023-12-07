//using System;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Identity;
//using TeamUpAPI.Models;
//using TeamUpAPI.Models.ViewModels;

//namespace TeamUpAPI.Services
//{

//    public interface IUserService
//    {
//        Task<bool> RegisterUser(UserRegistrationViewModel model);
//    }

//    public class UserService : IUserService
//    {
//        private readonly MovieContext DB;
//        private readonly UserManager<User> UserManager;

//        public UserService(MovieContext db, UserManager<User> userManager)
//        {
//            DB = db;
//            UserManager = userManager;
//        }

//        public async Task<bool> RegisterUser(UserRegistrationViewModel model)
//        {
//            try
//            {
//                User user = new User
//                {

//                };

//                UserProfile userProfile = new UserProfile
//                {

//                };

//                user.UserProfile = userProfile;

//                var result = await UserManager.CreateAsync(user);
//            }
//        }
//    }
//}
