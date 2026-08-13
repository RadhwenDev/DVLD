using DVLD_BusinessLayer;
using DVLD_PresentationLayer.Applications;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using static DVLD_PresentationLayer.Users.ucAddUpdateUser;

namespace DVLD_PresentationLayer.Users
{
    public partial class ucShowDetailsUser : UserControl
    {
        private int _UserID = -1;

        public ucShowDetailsUser(int UserID)
        {
            InitializeComponent();
            _UserID = UserID;
        }

        // الألوان المعتمدة لحالات الصلاحيات
        Color ColorGranted = Color.FromArgb(56, 142, 60);
        Color ColorDenied = Color.FromArgb(158, 158, 158);
        Color ColorTextGranted = Color.FromArgb(33, 33, 33);
        Color ColorTextDenied = Color.FromArgb(189, 189, 189);

        // القيم المحدثة للـ Enum الخاص بالصلاحيات (Bitwise Power of 2)
        const int pManagePeople = 1;
        const int pManageUsers = 2;
        const int pManageApplications = 4;
        const int pNewApplications = 8;
        const int pManageApplicationTypes = 16;
        const int pManageTestTypes = 32;
        const int pManageLicenseClasses = 64;
        const int pManageDetainedLicenses = 128;
        const int pManageInternationalApp = 256;
        const int pAuditLogs = 512;

        private void UpdateLabelPermission(Label label, Label lblText, bool isGranted, bool isSuperUser)
        {
            if (isGranted || isSuperUser)
            {
                label.Text = "✓";
                label.ForeColor = isSuperUser ? Color.FromArgb(148, 139, 216) : ColorGranted;
                lblText.ForeColor = ColorTextGranted;
                label.Font = new Font(label.Font, FontStyle.Bold);
                lblText.Font = new Font(lblText.Font, FontStyle.Bold);
            }
            else
            {
                label.Text = "✕";
                label.ForeColor = ColorDenied;
                lblText.ForeColor = ColorTextDenied;
                label.Font = new Font(label.Font, FontStyle.Regular);
                lblText.Font = new Font(lblText.Font, FontStyle.Regular);
            }
        }

        private void UpdatePermissionVisuals(int permissions, bool isSuperUser,
                Label l15, Label l11, Label l16, Label l12, Label l17, Label l13,
                Label l18, Label l14, Label l19, Label l22, Label l20, Label l23,
                Label l21, Label l24, Label l25, Label l28, Label l26, Label l29,
                Label l27, Label l30)
        {
            // دالة مساعدة محليّة لفحص وإبراز كل صلاحية على حدة
            void Check(Label lblSym, Label lblText, int permissionVal)
            {
                bool isGranted = (permissions & permissionVal) == permissionVal;
                UpdateLabelPermission(lblSym, lblText, isGranted, isSuperUser);
            }

            Check(l15, l11, pManagePeople);            
            Check(l19, l22, pManageUsers);             
            Check(l25, l12, pManageApplications);      
            Check(l16, l13, pNewApplications);         
            Check(l20, l30, pManageApplicationTypes);  
            Check(l26, l29, pManageTestTypes);         
            Check(l17, l23, pManageLicenseClasses);    
            Check(l21, l24, pManageDetainedLicenses);  
            Check(l27, l28, pManageInternationalApp);  
            Check(l18, l14, pAuditLogs);               
        }

        private void ucShowDetailsUser_Load(object sender, EventArgs e)
        {
            DataTable dt = clsUsers.getAllDetailsForShowButton(_UserID);

            if (dt == null || dt.Rows.Count == 0) return;

            DataRow row = dt.Rows[0];

            lblFullName.Text = row["FullName"].ToString();
            string PersonID = row["PersonID"].ToString();
            string UserName = row["UserName"].ToString();
            lblPersonID_UserName.Text = $"PersonID: {PersonID} • Username: {UserName}";

            lblNationalNo.Text = row["NationalNo"].ToString();

            if (DateTime.TryParse(row["DateOfBirth"].ToString(), out DateTime birthDate))
                lblDoB.Text = birthDate.ToString("MMM dd, yyyy", CultureInfo.InvariantCulture);
            else
                lblDoB.Text = row["DateOfBirth"].ToString();

            lblGender.Text = row["GenderName"].ToString();
            lblNationality.Text = row["CountryName"].ToString();
            lblPhone.Text = row["Phone"].ToString();
            lblMail.Text = row["Email"].ToString();
            lblAddress.Text = row["Address"].ToString();

            int permissions = Convert.ToInt32(row["Permissions"]);
            bool isSuperUser = (permissions == -1);

            // تحديث حالة الأيقونات والنصوص لكل صلاحية
            UpdatePermissionVisuals(permissions, isSuperUser,
                    label15, label11, label16, label12, label17, label13,
                    label18, label14, label19, label22, label20, label23,
                    label21, label24, label25, label28, label26, label29, label27, label30);

            // إدراج حالة الـ Badge الخاصة بالصلاحيات (Full Permissions = 1023 كـ Sum للـ Flags من 1 إلى 512)
            if (isSuperUser)
            {
                permissionsBadge.Text = "Global access (super user)";
                permissionsBadge.ForeColor = Color.FromArgb(148, 139, 216);
                permissionsBadge.FillColor = Color.FromArgb(29, 22, 73);
                permissionsBadge.BorderColor = Color.FromArgb(45, 45, 90);
            }
            else if (permissions == 1023)
            {
                permissionsBadge.Text = "Global access (full permissions)";
                permissionsBadge.ForeColor = Color.FromArgb(91, 144, 206);
                permissionsBadge.FillColor = Color.FromArgb(3, 32, 66);
                permissionsBadge.BorderColor = Color.FromArgb(20, 50, 80);
            }
            else
            {
                permissionsBadge.Text = "Custom access";
                permissionsBadge.ForeColor = Color.FromArgb(143, 143, 135);
                permissionsBadge.FillColor = Color.FromArgb(26, 26, 25);
                permissionsBadge.BorderColor = Color.FromArgb(26, 26, 25);
            }

            // تحديد حالة المستخدم (Active / Inactive)
            string statusName = row["Status"].ToString();
            if (statusName == "Active")
            {
                statusBadge.Text = "Active";
                statusBadge.FillColor = Color.FromArgb(232, 245, 233);
                statusBadge.ForeColor = Color.FromArgb(56, 142, 60);
                statusBadge.BorderColor = Color.FromArgb(56, 142, 60);
            }
            else
            {
                statusBadge.Text = "Inactive";
                statusBadge.FillColor = Color.FromArgb(254, 234, 234);
                statusBadge.ForeColor = Color.FromArgb(183, 28, 28);
                statusBadge.BorderColor = Color.FromArgb(183, 28, 28);
            }

            // تحميل صورة المستخدم بأمان
            string imagePath = row["ImagePath"].ToString();
            if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
            {
                try
                {
                    using (var stream = new FileStream(imagePath, FileMode.Open, FileAccess.Read))
                    {
                        pbImageUser.Image = System.Drawing.Image.FromStream(stream);
                    }
                }
                catch
                {
                    // يمكن وضع صورة افتراضية هنا في حال فشل تحميل الملف
                }
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.FindForm()?.Close();
        }
    }
}