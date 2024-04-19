using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace GreenStem.ClassModules
{
    public class clsHash
    {
        public static string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
        public static bool VerifyPassword(string enteredPassword, string storedHash)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                // Hash the entered password
                byte[] enteredBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(enteredPassword));
                // Convert the entered hash to a string
                StringBuilder enteredBuilder = new StringBuilder();
                for (int i = 0; i < enteredBytes.Length; i++)
                {
                    enteredBuilder.Append(enteredBytes[i].ToString("x2"));
                }
                // Compare the entered hash string with the stored hash
                return enteredBuilder.ToString().Equals(storedHash);
            }
        }
    }
}
