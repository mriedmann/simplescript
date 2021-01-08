using System;
using System.Security.Cryptography;
using System.Text;

namespace exampleservice.CustomerService.Utils
{
    public static class Password
    {
        public static string ComputeHash(string password)
        {
            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException("password");

            //From: https://www.c-sharpcorner.com/article/compute-sha256-hash-in-c-sharp/
            // Create a SHA256   
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
