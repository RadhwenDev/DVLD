using DVLD_BusinessLayer;
using DVLD_PresentationLayer.Global;
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
using static System.Net.Mime.MediaTypeNames;

namespace DVLD_PresentationLayer.Applications
{
    public partial class ucShowApplicationDetails : UserControl
    {
        int _ApplicationID = -1;
        public ucShowApplicationDetails(int ApplicationID)
        {
            InitializeComponent();
            _ApplicationID = ApplicationID;
        }

        private void ucShowApplicationDetails_Load(object sender, EventArgs e)
        {
            DataTable dt = clsApplicant.getAllDetailsForShowButton(_ApplicationID);
            if (dt == null || dt.Rows.Count == 0) return;

            string AppID = dt.Rows[0]["ApplicationID"].ToString().Trim();
            lblAppID.Text = "Application #" + AppID;
            lblServiceType.Text = dt.Rows[0]["ApplicationTypeTitle"].ToString();

            string statusName = dt.Rows[0]["StatusName"].ToString();
            switch (statusName)
            {
                case "New":
                    statusBadge.Text = "New";
                    statusBadge.FillColor = Color.FromArgb(239, 246, 255);
                    statusBadge.ForeColor = Color.FromArgb(40, 90, 231);
                    statusBadge.BorderColor = Color.FromArgb(40, 90, 231);
                    break;
                case "Completed":
                    statusBadge.Text = "Completed";
                    statusBadge.FillColor = Color.FromArgb(232, 245, 233);
                    statusBadge.ForeColor = Color.FromArgb(56, 142, 60);
                    statusBadge.BorderColor = Color.FromArgb(56, 142, 60);
                    break;
                case "Cancelled":
                    statusBadge.Text = "Cancelled";
                    statusBadge.FillColor = Color.FromArgb(254, 234, 234);
                    statusBadge.ForeColor = Color.FromArgb(183, 28, 28);
                    statusBadge.BorderColor = Color.FromArgb(183, 28, 28);
                    break;
            }

            lblAppID2.Text = "#" + AppID;

            // 📅 تظبيط التواريخ بالإنجليزية مجبرين (رغم أن الويندوز فرنسي)
            if (DateTime.TryParse(dt.Rows[0]["ApplicationDate"].ToString(), out DateTime appDate))
                lblApplicationDate.Text = appDate.ToString("MMM dd, yyyy", CultureInfo.InvariantCulture);
            else
                lblApplicationDate.Text = dt.Rows[0]["ApplicationDate"].ToString();

            if (DateTime.TryParse(dt.Rows[0]["LastStatusDate"].ToString(), out DateTime statusDate))
                lblLastStatusDate.Text = statusDate.ToString("MMM dd, yyyy", CultureInfo.InvariantCulture);
            else
                lblLastStatusDate.Text = dt.Rows[0]["LastStatusDate"].ToString();

            lblServiceType2.Text = lblServiceType.Text;

            // 💰 تظبيط الفلوس بالإنجليزية لمنع ظهور الفاصلة الفرنسية (,)
            if (decimal.TryParse(dt.Rows[0]["PaidFees"].ToString(), out decimal paidFees))
                lblFeesPaid.Text = "$" + paidFees.ToString("N2", CultureInfo.InvariantCulture);
            else
                lblFeesPaid.Text = "$" + dt.Rows[0]["PaidFees"].ToString();

            // 🔍 فحص ما إذا كانت الخدمة مرتبطة بفئة رخصة (مثل طلب جديد) أو خدمة عامة (مثل تعويض ضائع)
            string className = dt.Rows[0]["ClassName"].ToString();

            if (string.IsNullOrEmpty(className))
            {
                // 🌟 إخفاء البانل السفلي والـ Card الصغير لأن الخدمة لا تحتاجهم
                pnlLicenseClassDetails.Visible = false; // تأكد من اسم البانل السفلي عندك في التصميم
                lblLicenseClass.Visible = false;

                // إذا كنت واضع كرت الـ LICENSE CLASS داخل كادر أو Panel صغير، قم بإخفائه هنا:
                // pnlLicenseClassCard.Visible = false; 
            }
            else
            {
                // إظهارهم وتعبئة البيانات بشكل عادي إذا توفرت الفئة
                pnlLicenseClassDetails.Visible = true;
                lblLicenseClass.Visible = true;
                // pnlLicenseClassCard.Visible = true;

                lblLicenseClass.Text = className;
                lblClassName.Text = className;
                lblMinAge.Text = dt.Rows[0]["MinimumAllowedAge"].ToString() + " years";
                lblValidityPeriod.Text = dt.Rows[0]["DefaultValidityLength"].ToString() + " years";

                if (decimal.TryParse(dt.Rows[0]["ClassFees"].ToString(), out decimal classFees))
                    lblLicenseFees.Text = "$" + classFees.ToString("N2", CultureInfo.InvariantCulture);
                else
                    lblLicenseFees.Text = "$" + dt.Rows[0]["ClassFees"].ToString();

                lblDescription.Text = dt.Rows[0]["ClassDescription"].ToString();
            }

            // 👤 بيانات الشخص المتقدم
            lblName.Text = dt.Rows[0]["FullName"].ToString();
            lblGendor.Text = dt.Rows[0]["GenderName"].ToString();
            lblNationalID.Text = dt.Rows[0]["NationalNo"].ToString();

            if (DateTime.TryParse(dt.Rows[0]["DateOfBirth"].ToString(), out DateTime birthDate))
                lblBirthDay.Text = birthDate.ToString("MMM dd, yyyy", CultureInfo.InvariantCulture);
            else
                lblBirthDay.Text = dt.Rows[0]["DateOfBirth"].ToString();

            lblPhone.Text = dt.Rows[0]["Phone"].ToString();
            lblAddress.Text = dt.Rows[0]["Address"].ToString();
            string imagePath = dt.Rows[0]["ImagePath"].ToString();

            if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
            {
                try
                {
                    using (var stream = new FileStream(imagePath, FileMode.Open, FileAccess.Read))
                    {
                        pbImagePerson.Image = System.Drawing.Image.FromStream(stream);
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

        private void btnBack_Click(object sender, EventArgs e)
        {
            Control parentPanel = this.Parent;

            if (parentPanel != null)
            {
                // 2. ننظف الـ Panel تماماً وننحي الـ Show Details الحالي
                parentPanel.Controls.Clear();

                // 3. نصنع نسخة جديدة من الـ ucApplications ونعرضها
                ucApplications ucApps = new ucApplications();
                ucApps.Dock = DockStyle.Fill; // باش تاخذ الحجم الكامل للـ Panel تلقائياً

                parentPanel.Controls.Add(ucApps);
            }
        }

    }
}