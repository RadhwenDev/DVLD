using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLD_Security
{
    public static class RememberMeManager
    {
        private static readonly string FolderPath =
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "DVLD");

        private static readonly string FilePath =
            Path.Combine(FolderPath, "remember.dat");

        // حفظ الـ Token
        public static void SaveToken(string token)
        {
            if (!Directory.Exists(FolderPath))
                Directory.CreateDirectory(FolderPath);

            string encryptedToken = DPAPIHelper.Encrypt(token);

            File.WriteAllText(FilePath, encryptedToken);
        }

        // قراءة الـ Token
        public static bool TryLoadToken(out string token)
        {
            token = "";

            if (!File.Exists(FilePath))
                return false;

            try
            {
                string encryptedToken = File.ReadAllText(FilePath);

                token = DPAPIHelper.Decrypt(encryptedToken);

                return true;
            }
            catch
            {
                return false;
            }
        }

        // حذف الملف
        public static void DeleteToken()
        {
            if (File.Exists(FilePath))
                File.Delete(FilePath);
        }

        // هل الملف موجود؟
        public static bool HasToken()
        {
            return File.Exists(FilePath);
        }
    }
}
