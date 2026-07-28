using DVLD_BusinessLayer;
using DVLD_PresentationLayer.Applications;
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
        int _AppID = -1;
        enum enMode { Add = 0, Edit = 1}
        enMode _Mode;
        public ucAppointmentTest(int AppID, int Mode)
        {
            InitializeComponent();
            this._AppID = AppID;
            this._Mode = (enMode)Mode;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.FindForm()?.Close();
        }
        int testType = 0;
        private void ucAppointmentTest_Load(object sender, EventArgs e)
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ucAppointmentTest));
            pbtypeTest.Image = (Image)resources.GetObject("visionAppointment");
            lblTestType.Text = "Vision Test";



            DataTable dt = clsTestAppointment.getDataAppintment(_AppID, 1);
            lblLicenseID.Text = dt.Rows[0]["D.L.App.ID"].ToString();
            lblDClass.Text = dt.Rows[0]["D.Class"].ToString();
            lblName.Text = dt.Rows[0]["Name"].ToString();
            lblTrial.Text = dt.Rows[0]["Trial"].ToString();

            Date.Value = Convert.ToDateTime(dt.Rows[0]["Date"].ToString());
            if (_Mode == enMode.Add)
            {
                Date.Checked = false;
            }


            Date.FillColor = Color.FromArgb(248, 250, 252);
            Date.BorderColor = Color.FromArgb(213, 218, 223);
            Date.HoverState.BorderColor = Color.FromArgb(94, 148, 255);
            Date.HoverState.FillColor = Color.FromArgb(248, 250, 252);

            if (decimal.TryParse(dt.Rows[0]["Fees"].ToString(), out decimal classFees))
                lblFees.Text = classFees.ToString("N2", CultureInfo.InvariantCulture);
            else
                lblFees.Text = dt.Rows[0]["Fees"].ToString();
            lblRAppFees.Text = "0";
            lblTotalFees.Text = lblFees.Text;
            lblRTestAppID.Text = "N/A";
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            /*switch (_Person.Save())
            {
                case clsPerson.enSaveResult.SavedSuccessfully:
                    MessageBox.Show($"Person saved successfully with ID = {_AppID}!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    _Mode = enMode.Update; // تغيير الوضع إلى تعديل بعد النجاح الفوري
                    _PersonID = _Person.PersonID;

                    DataBack?.Invoke(this, _Person.PersonID);
                    this.FindForm()?.Close();
                    break;
                case enSaveResult.NoChanges:
                    MessageBox.Show("Nothing was changed");
                    break;
                case enSaveResult.Failed:
                    MessageBox.Show("Failed to save person data.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
            }*/
        }
    }
}
