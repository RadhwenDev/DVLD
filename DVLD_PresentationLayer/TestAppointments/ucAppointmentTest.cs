using DVLD_BusinessLayer;
using DVLD_PresentationLayer.Applications;
using DVLD_PresentationLayer.Global;
using Guna.UI2.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DVLD_PresentationLayer.TestAppointments
{
    public partial class ucAppointmentTest : UserControl
    {
        public delegate void DataBackEventHandler(object sender, int testAppointmentID);
        public event DataBackEventHandler DataBack;

        private int _LocalDrivingLicenseApplicationID = -1;
        private int _TestAppointmentID = -1;
        private clsTestAppointment _TestAppointment;

        public enum enMode { Add = 0, Edit = 1 }
        private enMode _Mode;

        // يمكنك تغيير هذا الحقل ليمثل نوع الاختبار الحركي (1 = Vision, 2 = Written, 3 = Street)
        private int _TestTypeID;

        public ucAppointmentTest(int LocalDrivingLicenseApplicationID, int Mode, int testAppointmentID = -1)
        {
            InitializeComponent();
            this._LocalDrivingLicenseApplicationID = LocalDrivingLicenseApplicationID;
            this._Mode = (enMode)Mode;
            this._TestAppointmentID = testAppointmentID;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.FindForm()?.Close();
        }

        private void ucAppointmentTest_Load(object sender, EventArgs e)
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ucAppointmentTest));
            byte passedTests = clsTestAppointment.GetPassedTestsCount(_LocalDrivingLicenseApplicationID);
            switch (passedTests)
            {
                case 0:
                    pbtypeTest.Image = (Image)resources.GetObject("visionAppointment");
                    lblTestType.Text = "Vision Test";
                    _TestTypeID = 1;
                    break;

                case 1:
                    pbtypeTest.Image = (Image)resources.GetObject("writtenAppointment");
                    lblTestType.Text = "Written Test";
                    _TestTypeID = 2;
                    break;

                case 2:
                    pbtypeTest.Image = (Image)resources.GetObject("streetAppointment");
                    lblTestType.Text = "Street Test";
                    _TestTypeID = 3;
                    break;
            }

            // إعداد عناصر التاريخ
            Date.FillColor = Color.FromArgb(248, 250, 252);
            Date.BorderColor = Color.FromArgb(213, 218, 223);
            Date.HoverState.BorderColor = Color.FromArgb(94, 148, 255);
            Date.HoverState.FillColor = Color.FromArgb(248, 250, 252);
            FillTimeSlots();

            if (_Mode == enMode.Add)
            {
                _TestAppointment = new clsTestAppointment();

                DataTable dt = clsTestAppointment.getDataAppintment(_LocalDrivingLicenseApplicationID, _TestTypeID);
                if (dt != null && dt.Rows.Count > 0)
                {
                    lblLicenseID.Text = dt.Rows[0]["D.L.App.ID"].ToString();
                    lblDClass.Text = dt.Rows[0]["D.Class"].ToString();
                    lblName.Text = dt.Rows[0]["Name"].ToString();
                    lblTrial.Text = dt.Rows[0]["Trial"].ToString();

                    if (decimal.TryParse(dt.Rows[0]["Fees"].ToString(), out decimal classFees))
                        lblFees.Text = classFees.ToString("N2", CultureInfo.InvariantCulture);
                    else
                        lblFees.Text = dt.Rows[0]["Fees"].ToString();


                    if (lblTrial.Text != "0")
                    {
                        groupBox1.Visible = true;
                        lblRAppFees.Text = dt.Rows[0]["R.App.Fees"].ToString();
                        lblTotalFees.Text = dt.Rows[0]["Total Fees"].ToString();
                    }
                    else
                    {
                        groupBox1.Visible = false;
                    }
                    lblRTestAppID.Text = "N/A";
                }

                Date.Value = DateTime.Now;
            }
            else // Edit Mode
            {
                _TestAppointment = clsTestAppointment.Find(_TestAppointmentID);
                if (_TestAppointment == null)
                {
                    MessageBox.Show("No Appointment with ID = " + _TestAppointmentID, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.FindForm()?.Close();
                    return;
                }

                lblLicenseID.Text = _TestAppointment.LocalDrivingLicenseApplicationID.ToString();
                lblDClass.Text = _TestAppointment.ClassName;
                lblName.Text = _TestAppointment.FullName;
                lblTrial.Text = _TestAppointment.TestTrialCount.ToString();
                lblFees.Text = _TestAppointment.PaidFees.ToString("N2", CultureInfo.InvariantCulture);
                Date.Value = _TestAppointment.AppointmentDate.Date;
                string savedTime = _TestAppointment.AppointmentDate.ToString("hh:mm tt", CultureInfo.InvariantCulture);
                if (cbTimeSlots.Items.Contains(savedTime))
                    cbTimeSlots.SelectedItem = savedTime;
                else
                    cbTimeSlots.Text = savedTime;
                groupBox1.Visible = false;
            }
        }

        private void FillTimeSlots()
        {
            cbTimeSlots.Items.Clear();

            // إضافة فترات العمل المتاحة للمواعيد
            cbTimeSlots.Items.Add("08:00 AM");
            cbTimeSlots.Items.Add("09:00 AM");
            cbTimeSlots.Items.Add("10:00 AM");
            cbTimeSlots.Items.Add("11:00 AM");
            cbTimeSlots.Items.Add("01:00 PM");
            cbTimeSlots.Items.Add("02:00 PM");
            cbTimeSlots.Items.Add("03:00 PM");

            // تحديد خيار افتراضي
            cbTimeSlots.SelectedIndex = 0;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            // 1. Validation First: Ensure a time slot is selected before processing
            if (cbTimeSlots.SelectedItem == null)
            {
                MessageBox.Show("Please select an appointment time slot!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. تحويل الوقت المختار في ComboBox إلى TimeSpan
            if (DateTime.TryParseExact(cbTimeSlots.SelectedItem.ToString(), "hh:mm tt",
                CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedTime))
            {
                _TestAppointment.AppointmentDate = Date.Value.Date.Add(parsedTime.TimeOfDay);
            }
            else
            {
                _TestAppointment.AppointmentDate = Date.Value;
            }

            _TestAppointment.TestTypeID = _TestTypeID;

            // السطر المعدل
            _TestAppointment.LocalDrivingLicenseApplicationID = _LocalDrivingLicenseApplicationID;

            string cleanText = "";
            if(lblTrial.Text == "0")
                cleanText = lblFees.Text.Replace(',', '.');
            else
                cleanText = lblTotalFees.Text.Replace(',', '.');
            if (decimal.TryParse(cleanText, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal testFees))
            {
                _TestAppointment.PaidFees = testFees;
            }
            _TestAppointment.CreatedByUserID = clsCurrentUser._UserID;

            // 3. الحفظ والتحقق من النتيجة
            clsTestAppointment.enSaveResult result = _TestAppointment.Save();

            switch (result)
            {
                case clsTestAppointment.enSaveResult.SavedSuccessfully:
                    MessageBox.Show($"Appointment saved successfully with ID = {_TestAppointment.TestAppointmentID}!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    _Mode = enMode.Edit;
                    _TestAppointmentID = _TestAppointment.TestAppointmentID;

                    DataBack?.Invoke(this, _TestAppointment.TestAppointmentID);
                    this.FindForm()?.Close();
                    break;

                case clsTestAppointment.enSaveResult.InvalidData:
                    MessageBox.Show("Selected date/time is invalid. Please make sure the appointment is at least 1 hour from now.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;

                case clsTestAppointment.enSaveResult.NoChanges:
                    MessageBox.Show("No changes were made to the appointment.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;

                case clsTestAppointment.enSaveResult.Failed:
                    MessageBox.Show("Failed to save the test appointment. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
            }
        }
    }
}