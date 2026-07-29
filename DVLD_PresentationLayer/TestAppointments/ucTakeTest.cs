using DVLD_BusinessLayer;
using DVLD_PresentationLayer.Global;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DVLD_PresentationLayer.TestAppointments
{
    public partial class ucTakeTest : UserControl
    {
        private int _TestAppointmentID = -1;
        private int _TestTypeID = -1;
        private clsTestAppointment _TestAppointment;
        public ucTakeTest(int testAppointmentID, int testTypeID)
        {
            InitializeComponent();
            this._TestAppointmentID = testAppointmentID;
            this._TestTypeID = testTypeID;
        }

        private void ucTakeTest_Load(object sender, EventArgs e)
        {
            SetTestTypeHeaderInfo();
            LoadAppointmentData();
        }
        private void SetTestTypeHeaderInfo()
        {
            ComponentResourceManager resources = new ComponentResourceManager(typeof(ucTakeTest));

            switch (_TestTypeID)
            {
                case 1:
                    lblTestType.Text = "Vision Test";
                    pbtypeTest.Image = (Image)resources.GetObject("visionAppointment");
                    break;
                case 2:
                    lblTestType.Text = "Written Test";
                    pbtypeTest.Image = (Image)resources.GetObject("Written");
                    break;
                case 3:
                    lblTestType.Text = "Street Test";
                    pbtypeTest.Image = (Image)resources.GetObject("Street");
                    break;
            }
        }

        private void LoadAppointmentData()
        {
            // 1. البحث عن الموعد بواسطة clsTestAppointment
            _TestAppointment = clsTestAppointment.Find(_TestAppointmentID);

            if (_TestAppointment == null)
            {
                MessageBox.Show($"No Appointment Found with ID ({_TestAppointmentID})", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnSave.Enabled = false;
                return;
            }

            // 2. تعبئة بيانات الموعد على عناصر الواجهة (تعديل الأسماء حسب التصميم لديك)
            lblLicenseID.Text = _TestAppointment.LocalDrivingLicenseApplicationID.ToString();
            lblDClass.Text = _TestAppointment.ClassName;
            lblName.Text = _TestAppointment.FullName;
            lblTrial.Text = _TestAppointment.TestTrialCount.ToString();
            lblDate.Text = _TestAppointment.AppointmentDate.ToString("MMM dd, yyyy", CultureInfo.InvariantCulture);
            lblFees.Text = _TestAppointment.PaidFees.ToString("N2", CultureInfo.InvariantCulture);
            UpdateResultSelectionUI();
            if (_TestAppointment.TestID != -1)
            {
                lblTestID.Text = _TestAppointment.TestID.ToString();
                lblTestID.ForeColor = Color.Black; 
            }
            else
            {
                lblTestID.Text = "Not Taken Yet";
                lblTestID.ForeColor = Color.Gray;
            }
        }
        private void UpdateResultSelectionUI()
        {
            if (rbPass.Checked)
            {
                // Pass Style (Green Accent)
                rbPass.FlatStyle = FlatStyle.Flat;
                rbPass.FlatAppearance.BorderSize = 0;
                rbPass.BackColor = Color.FromArgb(230, 246, 238); // خلفية خضراء فاتحة
                rbPass.ForeColor = Color.FromArgb(46, 125, 50);    // نص أخضر غامق

                // Reset Fail Style
                rbFail.FlatStyle = FlatStyle.Standard;
                rbFail.BackColor = Color.Transparent;
                rbFail.ForeColor = Color.Black;
            }
            else if (rbFail.Checked)
            {
                // Fail Style (Red Accent)
                rbFail.FlatStyle = FlatStyle.Flat;
                rbFail.FlatAppearance.BorderSize = 0;
                rbFail.BackColor = Color.FromArgb(253, 237, 237); // خلفية حمراء فاتحة
                rbFail.ForeColor = Color.FromArgb(211, 47, 47);    // نص أحمر غامق

                // Reset Pass Style
                rbPass.FlatStyle = FlatStyle.Standard;
                rbPass.BackColor = Color.Transparent;
                rbPass.ForeColor = Color.Black;
            }
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.FindForm()?.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to save the test result? You cannot edit it later.",
                                "Confirm Save", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                return;
            }

            // Create a new test object for recording the result
            clsTest test = new clsTest();
            test.TestAppointmentID = _TestAppointmentID;
            test.TestResult = rbPass.Checked; // true if Passed, false if Failed
            test.Notes = txtNotes.Text.Trim();
            test.CreatedByUserID = clsCurrentUser.CurrentUser?.UserID ?? 1; // Current system user

            if (test.Save())
            {
                MessageBox.Show("Test result saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                btnSave.Enabled = false;
                this.FindForm()?.Close();
            }
            else
            {
                MessageBox.Show("Failed to save the test result!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void rbPass_CheckedChanged(object sender, EventArgs e)
        {
            UpdateResultSelectionUI();
        }

        private void rbFail_CheckedChanged(object sender, EventArgs e)
        {
            UpdateResultSelectionUI();
        }
    }
}
