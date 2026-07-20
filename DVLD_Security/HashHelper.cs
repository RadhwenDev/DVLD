using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DVLD_Security
{
    public static class HashHelper
    {
        public static string ComputeSHA256(string text)
        {
            using (SHA256 sha = System.Security.Cryptography.SHA256.Create())
            {
                byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(text));

                // تحويل البايتات إلى String بصيغة Hex يدوياً
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    // "x2" تعني تحويل الرقم إلى Hex مكون من خانتين
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
