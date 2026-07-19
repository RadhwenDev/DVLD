using DVLD_BusinessLayer;
using DVLD_PresentationLayer.Global;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DVLD_PresentationLayer.Login
{
    public partial class frmLogin : Form
    {
        public frmLogin()
        {
            InitializeComponent();
        }

        private void LoginScreen_Load(object sender, EventArgs e)
        {
            lblCopyRight.Text = $"© {DateTime.Now.Year} DVLD Administration. All rights reserved.";
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnSignIn_Click(object sender, EventArgs e)
        {
            // 1. نبحث عن المستخدم أولاً بالـ Username والـ Password فقط (بدون شرط IsActive في الـ Query)
            // نصيحة: يفضل أن ترجع دالة الـ Find المستخدم حتى لو كان غير نشط، لكي نتحقق من حالته هنا
            string hashedPassword = clsCryptoSettings.ComputeSha256Hash(txtPassword.Text.Trim());
            clsUsers user = clsUsers.Find(txtUserName.Text, hashedPassword);

            // 2. التحقق أولاً: هل المستخدم موجود في قاعدة البيانات؟
            if (user == null)
            {
                MessageBox.Show("Invalid Username/Password!", "Wrong Credentials", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; // نخرج من الدالة فوراً لحماية الكود
            }

            // 3. التحقق ثانياً: هل الحساب نشط (IsActive)؟
            if (!user.isActive) // أو user.IsActive حسب التسمية عندك
            {
                MessageBox.Show("Your account is deactivated. Please contact your administrator.", "Account Inactive", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // نخرج ولا نسمح له بالدخول
            }
            clsLoginLog.RegisterLogin(user.UserID);
            // 4. إذا تجاوز الشروط السابقة بأمان، نقوم بجلب بيانات الـ Person الآن بدون أي خوف من الـ Null
            clsPerson person = clsPerson.Find(user.PersonID);

            // 5. حفظ البيانات في الكلاسات العالمية (Global)
            clsCurrentUser.CurrentUser = user;
            clsCurrentPerson.CurrentPerson = person;

            // 6. نجاح الدخول وإغلاق الشاشة
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
