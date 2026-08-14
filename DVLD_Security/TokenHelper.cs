using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DVLD_Security
{
    public class TokenHelper
    {
        public static string GenerateToken()
        {
            // 1. إنشاء مصفوفة بايتات بالحجم المطلوب (32 بايت)
            byte[] randomNumber = new byte[32];

            // 2. تعبئة المصفوفة بأرقام عشوائية آمنة
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
            }

            // 3. تحويل البايتات إلى Hex String
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < randomNumber.Length; i++)
            {
                builder.Append(randomNumber[i].ToString("x2"));
            }

            return builder.ToString();
        }

        // باش نعملو function خاصة بال ResetCode لانها باش ترجعلنا 6 ارقام
        public static string GenerateResetCode()
        {
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                byte[] bytes = new byte[4];
                rng.GetBytes(bytes);

                uint number = BitConverter.ToUInt32(bytes, 0);

                return (number % 1000000).ToString("D6");
            }
        }
    }
}
