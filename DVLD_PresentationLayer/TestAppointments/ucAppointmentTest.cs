using DVLD_BusinessLayer;
using DVLD_PresentationLayer.Applications;
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
    public partial class ucAppointmentTest : UserControl
    {
        public delegate void DataBackEventHandler(object sender, int testAppointmentID);
        public event DataBackEventHandler DataBack;

        private int _AppID = -1;
        private int _TestAppointmentID = -1;
        private clsTestAppointment _TestAppointment;

        public enum enMode { Add = 0, Edit = 1 }
        private enMode _Mode;

        // يمكنك تغيير هذا الحقل ليمثل نوع الاختبار الحركي (1 = Vision, 2 = Written, 3 = Street)
        private int _TestTypeID = 1;

        public ucAppointmentTest(int AppID, int Mode, int testAppointmentID = -1)
        {
            InitializeComponent();
            this._AppID = AppID;
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
            pbtypeTest.Image = (Image)resources.GetObject("visionAppointment");
            lblTestType.Text = "Vision Test";

            // إعداد عناصر التاريخ
            Date.FillColor = Color.FromArgb(248, 250, 252);
            Date.BorderColor = Color.FromArgb(213, 218, 223);
            Date.HoverState.BorderColor = Color.FromArgb(94, 148, 255);
            Date.HoverState.FillColor = Color.FromArgb(248, 250, 252);
            FillTimeSlots();
            if (_Mode == enMode.Add)
            {
                _TestAppointment = new clsTestAppointment();

                DataTable dt = clsTestAppointment.getDataAppintment(_AppID, _TestTypeID);
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
                }

                Date.Value = DateTime.Now;
                lblRAppFees.Text = "0";
                lblTotalFees.Text = lblFees.Text;
                lblRTestAppID.Text = "N/A";
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
                lblRAppFees.Text = "0";
                lblTotalFees.Text = lblFees.Text;
                lblRTestAppID.Text = _TestAppointment.RetakeTestApplicationID == -1 ? "N/A" : _TestAppointment.RetakeTestApplicationID.ToString();
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
            _TestAppointment.LocalDrivingLicenseApplicationID = _AppID;

            if (decimal.TryParse(lblFees.Text, out decimal fees))
                _TestAppointment.PaidFees = fees;

            // قم بتغيير هذا المعرف بـ User الحالي الحاصل على التسجيل (مثلاً clsGlobal.CurrentUser.UserID)
            _TestAppointment.CreatedByUserID = clsCurrentUser._UserID;

            // 2. الحفظ والتحقق من النتيجة
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