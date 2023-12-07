using System;
using System.Security.Cryptography;
using System.Text;

namespace TeamUpAPI.Services
{
    public static class PasswordHasher
    {
        private const int SaltSize = 16; // 128 bits

        public static string HashPassword(string password)
        {
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[SaltSize]);

            // Create a hash using the salt and password
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256);
            byte[] hash = pbkdf2.GetBytes(20); // 160 bits

            // Combine salt and hash
            byte[] hashBytes = new byte[SaltSize + 20];
            Array.Copy(salt, 0, hashBytes, 0, SaltSize);
            Array.Copy(hash, 0, hashBytes, SaltSize, 20);

            // Convert to base64 for storage
            return Convert.ToBase64String(hashBytes);
        }

        public static bool VerifyPassword(string hashedPassword, string enteredPassword)
        {
            // Extract salt and hash from the stored password
            byte[] hashBytes = Convert.FromBase64String(hashedPassword);
            byte[] salt = new byte[SaltSize];
            Array.Copy(hashBytes, 0, salt, 0, SaltSize);

            // Compute the hash of the entered password using the stored salt
            var pbkdf2 = new Rfc2898DeriveBytes(enteredPassword, salt, 10000, HashAlgorithmName.SHA256);
            byte[] hash = pbkdf2.GetBytes(20);

            // Compare the computed hash with the stored hash
            for (int i = 0; i < 20; i++)
            {
                if (hashBytes[i + SaltSize] != hash[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}
