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
            int ApplicationID = clsApplicant.AddNewApplication(SelectedPersonID, DateTime.Now, SelectedApplicationTypeID, 1, DateTime.Now, fees, /*clsCurrentUser._UserID*/ 1);
            if (ApplicationID != -1)
            {
                MessageBox.Show($"Application saved successfully with ID = {ApplicationID}!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                if (SelectedApplicationTypeID == 1 || SelectedApplicationTypeID == 8)
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
