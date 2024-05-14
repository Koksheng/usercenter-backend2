using System.Security.Cryptography;
using System.Text;

namespace usercenter.Application.Common
{
    public class EncryptionService
    {
        // Hashes the password using HMAC-SHA256 with a secret key
        public static string HashPasswordWithKey(string password, string key)
        {
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key)))
            {
                byte[] hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashBytes);
            }
        }

        /** Example to call VerifyPassword()
         * 
         *  string hashedPassword = "YourStoredHashedPassword"; // Retrieve hashed password from storage
            string inputPassword = "UserInputPassword"; // User's input password

            bool isPasswordCorrect = EncryptionService.VerifyPassword(inputPassword, hashedPassword, "YourSecretKey");
         */

        // Checks if the input password matches the hashed password
        public static bool VerifyPassword(string inputPassword, string hashedPassword, string key)
        {
            string hashedInputPassword = HashPasswordWithKey(inputPassword, key);
            return hashedInputPassword == hashedPassword;
        }

    }
}
