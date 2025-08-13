using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Security.Cryptography;
using System.Text;

namespace BTEDiploma.Helper
{
    public static class SecurityHelper
    {
        public const int SaltSize = 16; // 128-bit

        public static void CreatePasswordHash(string password, out byte[] salt, out byte[] hash)
        {
            if (password == null) throw new ArgumentNullException(nameof(password));

            // Allocate salt buffer
            salt = new byte[SaltSize];  // SaltSize is an int (e.g., 16)

            // Fill with cryptographic random bytes
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            // Encode password as Unicode to match SQL NVARCHAR
            byte[] pwdBytes = Encoding.Unicode.GetBytes(password);

            // salt + password
            byte[] combined = new byte[salt.Length + pwdBytes.Length];
            Buffer.BlockCopy(salt, 0, combined, 0, salt.Length);
            Buffer.BlockCopy(pwdBytes, 0, combined, salt.Length, pwdBytes.Length);

            using (var sha = SHA256.Create())
            {
                hash = sha.ComputeHash(combined); // 32 bytes
            }
        }

    }
}
