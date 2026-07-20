using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace DVLD_Security
{
    public class DPAPIHelper
    {
        public static string Encrypt(string token)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(token);

            byte[] encrypted = ProtectedData.Protect(bytes, null, DataProtectionScope.CurrentUser);

            return Convert.ToBase64String(encrypted);
        }

        public static string Decrypt(string encryptedToken)
        {
            byte[] bytes =
                Convert.FromBase64String(encryptedToken);

            byte[] decrypted =
                ProtectedData.Unprotect(bytes, null, DataProtectionScope.CurrentUser);

            return Encoding.UTF8.GetString(decrypted);
        }
    }
}
