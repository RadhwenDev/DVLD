using DVLD_PresentationLayer.Login;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DVLD_PresentationLayer
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
           // frmLogin loginForm = new frmLogin();
            Application.Run(new frmMain());

         /*   // 2. فتحها كـ Dialog (لتجميد البرنامج حتى يضع بياناته)
            if (loginForm.ShowDialog() == DialogResult.OK)
            {
                // 3. إذا نجح الـ Login، افتح الشاشة الرئيسية للمشروع
                Application.Run(new frmMain()); // تأكد من كتابة اسم الفورم الرئيسي لديك بشكل صحيح
            }
            else
            {
                // إذا أغلق شاشة الـ Login أو ضغط Cancel، يغلق التطبيق بالكامل فوراً
                Application.Exit();
            }*/
        }
    }
}
