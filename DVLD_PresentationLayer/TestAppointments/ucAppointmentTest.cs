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
        public ucAppointmentTest(/*int AppID*/)
        {
            InitializeComponent();
            //this._AppID = AppID;
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

/*

            DataTable dt = clsTestAppointment.getDataAppintment(_AppID, testType);
            lblLicenseID.Text = dt.Rows[0]["D.L.App.ID"].ToString();
            lblDClass.Text = dt.Rows[0]["D.Class"].ToString();
            lblName.Text = dt.Rows[0]["Name"].ToString();
            lblTrial.Text = dt.Rows[0]["Trial"].ToString();
            if (DateTime.TryParse(dt.Rows[0]["Date"].ToString(), out DateTime Date))
                lblDate.Text = Date.ToString("MMM dd, yyyy", CultureInfo.InvariantCulture);
            else
                lblDate.Text = dt.Rows[0]["Date"].ToString();
            lblFees.Text = dt.Rows[0]["Fees"].ToString();
            lblRAppFees.Text = dt.Rows[0]["R.App.Fees"].ToString();
            lblTotalFees.Text = dt.Rows[0]["Total Fees"].ToString();
            lblRTestAppID.Text = dt.Rows[0]["LicenseID"].ToString();*/
        }
    }
}
