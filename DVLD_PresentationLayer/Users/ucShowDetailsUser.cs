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
        int _UserID = -1;
        public ucShowDetailsUser(int UserID)
        {
            InitializeComponent();
            _UserID = UserID;
        }
        Color ColorGranted = Color.FromArgb(56, 142, 60);
        Color ColorDenied = Color.FromArgb(158, 158, 158);
        Color ColorTextGranted = Color.FromArgb(33, 33, 33);
        Color ColorTextDenied = Color.FromArgb(189, 189, 189);

        // أمثلة لقيم الصلاحيات (تأكد أنها تطابق الـ Database عندك)
        const int pManagePeople = 1;
        const int pManageUsers = 2;
        const int pManageDrivers= 4;
        const int pViewApplications = 8;
        const int pProcessApplications = 16;
        const int pManageApplicationTests= 32;
        const int pEnterTestResults = 64;
        const int pManageDetainedLicenses = 128;
        const int pManageSettingsAndFees = 256;
        const int pAuditLogs = 512;
        private void UpdateLabelPermission(Label label, Label lblText, bool test, bool isSuperUser)
        {
            if (test)
            {
                label.Text = "✓";
                if (isSuperUser)
                    label.ForeColor = Color.FromArgb(148, 139, 216);
                else
                    label.ForeColor = ColorGranted;
                lblText.ForeColor = ColorTextGranted;
                label.Font = new Font(label.Font, FontStyle.Bold); // جعل الـ صح سميك
                lblText.Font = new Font(lblText.Font, FontStyle.Bold);
            }
            else
            {
                label.Text = "✕"; // أو يمكن مسح النص تماماً ""
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
            // دالة داخلية لتسهيل الفحص
            void Check(Label lblSym, Label lblText, int permissionVal)
            {
                bool isGranted = (permissions & permissionVal) == permissionVal;
                UpdateLabelPermission(lblSym, lblText, isGranted, isSuperUser);
            }

            // الآن نفحص كل صلاحية بناءً على قيمتها
            Check(l15, l11, pManagePeople);
            Check(l16, l12, pViewApplications);
            Check(l17, l13, pEnterTestResults);
            Check(l18, l14, pManageUsers);
            Check(l19, l22, pProcessApplications);
            Check(l20, l23, pManageDetainedLicenses);
            Check(l21, l24, pManageDrivers);
            Check(l25, l28, pManageApplicationTests);
            Check(l26, l29, pManageSettingsAndFees);
            Check(l27, l30, pAuditLogs);
        }

        private void ucShowDetailsUser_Load(object sender, EventArgs e)
        {
            DataTable dt = clsUsers.getAllDetailsForShowButton(_UserID);
            lblFullName.Text = dt.Rows[0]["FullName"].ToString();
            string PersonID = dt.Rows[0]["PersonID"].ToString();
            string UserName = dt.Rows[0]["UserName"].ToString();
            lblPersonID_UserName.Text = $"PersonID: {PersonID} • Username: {UserName}";

            lblNationalNo.Text = dt.Rows[0]["NationalNo"].ToString();
            if (DateTime.TryParse(dt.Rows[0]["DateOfBirth"].ToString(), out DateTime birthDate))
                lblDoB.Text = birthDate.ToString("MMM dd, yyyy", CultureInfo.InvariantCulture);
            else
                lblDoB.Text = dt.Rows[0]["DateOfBirth"].ToString();
            lblGender.Text = dt.Rows[0]["GenderName"].ToString();
            lblNationality.Text = dt.Rows[0]["CountryName"].ToString();
            lblPhone.Text = dt.Rows[0]["Phone"].ToString();
            lblMail.Text = dt.Rows[0]["Email"].ToString();
            lblAddress.Text = dt.Rows[0]["Address"].ToString();
            lblAddress.Text = dt.Rows[0]["Address"].ToString();

            int Permissions = int.Parse(dt.Rows[0]["Permissions"].ToString());

            bool isSuperUser = (Permissions == -1);
            UpdatePermissionVisuals(Permissions, isSuperUser,
                    label15, label11, label16, label12, label17, label13,
                    label18, label14, label19, label22, label20, label23,
                    label21, label24, label25, label28, label26, label29, label27, label30);

            if (isSuperUser)
            {
                permissionsBadge.Text = "Global access (super user)";
                permissionsBadge.ForeColor = Color.FromArgb(148, 139, 216);
                permissionsBadge.FillColor = Color.FromArgb(29, 22, 73);
                permissionsBadge.BorderColor = Color.FromArgb(45, 45, 90);
            }
            else if(Permissions == 1023)
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

                string statusName = dt.Rows[0]["Status"].ToString();
            if (statusName == "Active")
            {
                statusBadge.Text = "Active";
                statusBadge.FillColor = Color.FromArgb(232, 245, 233); // أخضر فاتح
                statusBadge.ForeColor = Color.FromArgb(56, 142, 60);
                statusBadge.BorderColor = Color.FromArgb(56, 142, 60);
            }
            else
            {
                statusBadge.Text = "Inactive";
                statusBadge.FillColor = Color.FromArgb(254, 234, 234); // أحمر فاتح
                statusBadge.ForeColor = Color.FromArgb(183, 28, 28);
                statusBadge.BorderColor = Color.FromArgb(183, 28, 28);
            }

            string imagePath = dt.Rows[0]["ImagePath"].ToString();

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
                }
            }
            else
            {
            }

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.FindForm()?.Close();
        }
    }
}
