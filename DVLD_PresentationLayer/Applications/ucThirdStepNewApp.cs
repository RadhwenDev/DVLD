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

namespace DVLD_PresentationLayer.Applications
{
    public partial class ucThirdStepNewApp : UserControl
    {
        public delegate void DataBackEventHandler(object sender, int PersonID);
        public event DataBackEventHandler DataBack;
        public int SelectedPersonID { get; private set; } = -1;
        public int SelectedApplicationTypeID { get; private set; } = -1;
        public int SelectedLicenseClassID { get; private set; } = -1;

        public event EventHandler OnStepThirdCompleted;
        public event EventHandler OnBackButtonClicked;

        public ucThirdStepNewApp()
        {
            InitializeComponent();
        }

        public ucThirdStepNewApp(int personID, int applicationTypeID, int licenseClassID) : this()
        {
            SelectedPersonID = personID;
            SelectedApplicationTypeID = applicationTypeID;
            SelectedLicenseClassID = licenseClassID;
        }
        decimal fees = 0;
        private void ucThirdStepNewApp_Load(object sender, EventArgs e)
        {
            DataTable _dtName = clsPerson.GetFullNameByID(SelectedPersonID);
            DataTable _dtAppType = clsApplicant.getApplicationTypesTitle_Fees(SelectedApplicationTypeID);
            lblApplicant.Text = _dtName.Rows[0]["FullName"].ToString();
            lblService.Text = _dtAppType.Rows[0]["ApplicationTypeTitle"].ToString();
            if (SelectedLicenseClassID == -1)
                label7.Visible = false;
            else
            {
                label7.Visible = true;
                DataTable _dtLicenseClass = clsLicenseClass.GetLicenseClassesNameByID(SelectedLicenseClassID);
                lblLicenseClass.Text = _dtLicenseClass.Rows[0]["ClassName"].ToString();
            }
            fees = Convert.ToDecimal(_dtAppType.Rows[0]["ApplicationFees"]);
            lblFees.Text = $"${fees:N2}";
            lblDate.Text = DateTime.Now.ToString("MMMM dd, yyyy");
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            OnBackButtonClicked?.Invoke(this, e);
        }

        private void btnContinue_Click(object sender, EventArgs e)
        {
            byte ApplicationStatus = 1;
            if (SelectedApplicationTypeID != 1)
            {
                ApplicationStatus = 3;
            }
            int ApplicationID = clsApplicant.AddNewApplication(SelectedPersonID, DateTime.Now, SelectedApplicationTypeID, ApplicationStatus, DateTime.Now, fees, clsCurrentUser._UserID);
            if (ApplicationID != -1)
            {
                MessageBox.Show($"Application saved successfully with ID = {ApplicationID}!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                if (SelectedApplicationTypeID == 1 || SelectedApplicationTypeID == 7)
                {
                    int LocalDrivingLicenseAppID = clsLocalDrivingLicenseApplications.AddNewLocalDrivingLicenseApplications(ApplicationID, SelectedLicenseClassID);
                    if(LocalDrivingLicenseAppID != -1)
                    {
                        MessageBox.Show($"Local Driving License Applications saved successfully with ID = {LocalDrivingLicenseAppID}!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Failed to save Local Driving License Application data.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                else if (SelectedApplicationTypeID == 6)
                {
                    clsInternationalLicense internationalLicense = new clsInternationalLicense();

                    // تعبئة البيانات المطلوبة
                    internationalLicense.ApplicationID = ApplicationID;
                    clsDriver driver = clsDriver.FindByPersonID(SelectedPersonID);
                    if (driver == null)
                    {
                        MessageBox.Show("Failed to find this Driver record.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    int activeLocalLicenseID = clsLicenses.GetLicenseIDByApplicationID(driver.DriverID);

                    if (activeLocalLicenseID == -1)
                    {
                        MessageBox.Show("No active local license found for this person to issue an international license.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    internationalLicense.DriverID = driver.DriverID; // تأكد من الحصول على DriverID المطلوب
                    internationalLicense.IssuedUsingLocalLicenseID = activeLocalLicenseID; // ID الرخصة المحلية المستند عليها
                    internationalLicense.IssueDate = DateTime.Now;
                    internationalLicense.ExpirationDate = DateTime.Now.AddYears(1); // عادة مدة الرخصة الدولية سنة
                    internationalLicense.IsActive = true;
                    internationalLicense.CreatedByUserID = clsCurrentUser._UserID;

                    if (internationalLicense.Save())
                    {
                        MessageBox.Show($"International License saved successfully with ID = {internationalLicense.InternationalLicenseID}!",
                                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Failed to save International License data.",
                                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

                    // 🌟 إرسال الـ ID عبر الـ DataBack المخصص للإرسال الخارجي
                    DataBack?.Invoke(this, ApplicationID);

                // 🌟 إطلاق حدث اكتمال الخطوة الثالثة ليتحرك الـ Wizard
                OnStepThirdCompleted?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                MessageBox.Show("Failed to save Application data.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
