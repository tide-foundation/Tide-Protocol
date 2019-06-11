using System;
using System.Security.Cryptography;
using System.Text;

namespace Tide.Library.Classes.Encryption
{
    public static class Hasher
    {
        public static string Hash(string data)
        {
            using (SHA256 sha = SHA256.Create())
            {
                var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(data));
                return Convert.ToBase64String(hash);
            }
        }

        public static (string, string) DoubleHash(string data)
        {
            using (SHA256 sha = SHA256.Create())
            {
                var hash1 = sha.ComputeHash(Encoding.UTF8.GetBytes(data));
                var hash2 = sha.ComputeHash(hash1);
                return (Convert.ToBase64String(hash1), Convert.ToBase64String(hash2));
            }
        }
    }
}
