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

namespace DVLD_PresentationLayer.Licenses
{
    public partial class ucShowLicense : UserControl
    {
        int _ApplicationID = -1;
        public ucShowLicense(int ApplicationID)
        {
            InitializeComponent();
            _ApplicationID = ApplicationID;
        }

        private void ucShowLicense_Load(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            if (clsApplicant.IsReleaseApplication(_ApplicationID))
            {
                dt = clsLicenses.getShowLicenseRelease(_ApplicationID);
            }
            else if (clsApplicant.IsInternationalApplication(_ApplicationID))
            {
                dt = clsLicenses.getShowInternationalLicense(_ApplicationID);
            }
            else if (clsApplicant.IsRenewApplication(_ApplicationID))
            {
                dt = clsLicenses.getShowRenewLicense(_ApplicationID);
            }
            else
            {
                dt = clsLicenses.getShowLicense(_ApplicationID);

            }
            if (dt != null && dt.Rows.Count > 0)
            {
                lblClassName.Text = dt.Rows[0]["LicenseClass"].ToString();
                lblName.Text = dt.Rows[0]["FullName"].ToString();
                lblLicenseID.Text = dt.Rows[0]["LicenseID"].ToString();
                lblNationalNo.Text = dt.Rows[0]["NationalNo"].ToString();
                if (DateTime.TryParse(dt.Rows[0]["DateOfBirth"].ToString(), out DateTime birthDate))
                    lblDoB.Text = birthDate.ToString("MMM dd, yyyy", CultureInfo.InvariantCulture);
                else
                    lblDoB.Text = dt.Rows[0]["DateOfBirth"].ToString();
                lblGender.Text = dt.Rows[0]["Gender"].ToString();

                string isActive = dt.Rows[0]["IsActive"].ToString();

                if (isActive == "Yes")
                {
                    lblIsActive.Text = "Yes";
                    lblIsActive.ForeColor = Color.FromArgb(56, 142, 60);
                }
                else
                {
                    lblIsActive.Text = "No";
                    lblIsActive.ForeColor = Color.FromArgb(183, 28, 28);
                }
                lblDriverID.Text = dt.Rows[0]["DriverID"].ToString();
                if (DateTime.TryParse(dt.Rows[0]["IssueDate"].ToString(), out DateTime issueDate))
                    lblIssueDate.Text = issueDate.ToString("MMM dd, yyyy", CultureInfo.InvariantCulture);
                else
                    lblIssueDate.Text = dt.Rows[0]["IssueDate"].ToString();
                if (DateTime.TryParse(dt.Rows[0]["ExpirationDate"].ToString(), out DateTime expirationDate))
                    lblExpirationDate.Text = expirationDate.ToString("MMM dd, yyyy", CultureInfo.InvariantCulture);
                else
                    lblExpirationDate.Text = dt.Rows[0]["ExpirationDate"].ToString();
                lblIssueReason.Text = dt.Rows[0]["IssueReason"].ToString();
                string isDetained = dt.Rows[0]["IsDetained"].ToString();

                if (isDetained == "Yes")
                {
                    lblIsDetained.Text = "Yes";
                    lblIsDetained.ForeColor = Color.FromArgb(56, 142, 60);
                }
                else
                {
                    lblIsDetained.Text = "No";
                    lblIsDetained.ForeColor = Color.FromArgb(183, 28, 28);
                }
                if (dt.Rows[0]["Notes"] != DBNull.Value && !string.IsNullOrEmpty(dt.Rows[0]["Notes"].ToString()))
                {
                    lblNotes.Text = dt.Rows[0]["Notes"].ToString();
                }
                else
                {
                    lblNotes.Text = "No Notes";
                }
                string imagePath = dt.Rows[0]["ImagePath"].ToString();

                if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
                {
                    try
                    {
                        using (var stream = new FileStream(imagePath, FileMode.Open, FileAccess.Read))
                        {
                            pbImageDriver.Image = System.Drawing.Image.FromStream(stream);
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
