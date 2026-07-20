using DVLD_BusinessLayer;
using DVLD_PresentationLayer.Global;
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
            clsUsers rememberedUser = clsUsers.TryLoginWithRememberMe();
            bool isLoginSuccessful = false;
            if (rememberedUser != null)
            {
                // إذا وجدنا مستخدماً محفوظاً، نحفظ بياناته في الـ Global مباشرة
                clsCurrentUser.CurrentUser = rememberedUser;
                clsCurrentPerson.CurrentPerson = clsPerson.Find(rememberedUser.PersonID);
                isLoginSuccessful = true;
            }
            else
            {
                // إذا لم يوجد توكن صالح، نفتح شاشة الـ Login
                frmLogin loginForm = new frmLogin();
                if (loginForm.ShowDialog() == DialogResult.OK)
                {
                    isLoginSuccessful = true;
                }
            }
            //Application.Run(new frmMain());

            // 2. فتحها كـ Dialog (لتجميد البرنامج حتى يضع بياناته)
            if (isLoginSuccessful)
            {
                Application.Run(new frmMain()); // تأكد من اسم الفورم الرئيسي لديك
            }
            else
            {
                Application.Exit();
            }
        }
    }
}
