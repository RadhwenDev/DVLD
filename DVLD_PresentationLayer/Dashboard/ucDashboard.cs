using DVLD_BusinessLayer;
using DVLD_PresentationLayer.Applications;
using Guna.UI2.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DVLD_PresentationLayer.Dashboard
{
    public partial class ucDashboard : UserControl
    {
        public ucDashboard()
        {
            InitializeComponent();
        }

        private void btnNewApplication_Click(object sender, EventArgs e)
        {
            if (this.TopLevelControl is frmMain mainForm)
            {
                mainForm.OpenNewApplicationWizard();
            }
        }
       

        private void ucDashboard_Load(object sender, EventArgs e)
        {
            RefreshDashboard();
        }

        public void RefreshDashboard()
        {
            lblTotalPeople.Text = clsDashboard.GetTotalPeople().ToString();

            int totalPersonLastMonth =
                clsDashboard.GetTotalPeopleInThisMonth();

            if (totalPersonLastMonth > 0)
                lblLastTotalPeople.Text = $"+{totalPersonLastMonth} this month";
            else
                lblLastTotalPeople.Text = "No logins this month";

            lblPendingApps.Text =
                clsDashboard.getPendingApplicants().ToString();

            lblActiveLicenses.Text =
                clsLicenses.getTotalActiveLicenses().ToString();

            lblCompleted.Text =
                clsDashboard.GetCompletedApplicationsThisMonth().ToString();

            // Applications
            flowLayoutPanel1.SuspendLayout();
            flowLayoutPanel1.Controls.Clear();

            DataTable dtApplications =
                clsDashboard.GetApplicationPeopleInfo();

            if (dtApplications != null && dtApplications.Rows.Count > 0)
            {
                foreach (DataRow row in dtApplications.Rows)
                {
                    ucApplicationPersonInfos ucItem =
                        new ucApplicationPersonInfos();

                    ucItem.Width =
                        flowLayoutPanel1.DisplayRectangle.Width - 5;

                    ucItem.LoadApplicationInfo(row);
                    flowLayoutPanel1.Controls.Add(ucItem);
                }
            }

            flowLayoutPanel1.ResumeLayout();

            // Licenses
            flowLayoutPanel3.SuspendLayout();
            flowLayoutPanel3.Controls.Clear();

            DataTable dtLicenses =
                clsDashboard.GetLicensePeopleInfo();

            if (dtLicenses != null && dtLicenses.Rows.Count > 0)
            {
                foreach (DataRow row in dtLicenses.Rows)
                {
                    ucDashboardActiveLicense ucItem =
                        new ucDashboardActiveLicense();

                    ucItem.Width =
                        flowLayoutPanel3.DisplayRectangle.Width - 5;

                    ucItem.LoadLicenseInfo(row);
                    flowLayoutPanel3.Controls.Add(ucItem);
                }
            }

            flowLayoutPanel3.ResumeLayout();

            // Services
            flowLayoutPanel2.SuspendLayout();
            flowLayoutPanel2.Controls.Clear();

            DataTable dtService =
                clsDashboard.GetServiceBreakdown();

            if (dtService != null && dtService.Rows.Count > 0)
            {
                foreach (DataRow row in dtService.Rows)
                {
                    ucServiceBreakdown ucItem =
                        new ucServiceBreakdown();

                    ucItem.Width =
                        flowLayoutPanel2.DisplayRectangle.Width - 5;

                    ucItem.LoadServiceInfo(row);
                    flowLayoutPanel2.Controls.Add(ucItem);
                }
            }

            flowLayoutPanel2.ResumeLayout();
        }


        private void lblViewAllApps_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (this.TopLevelControl is frmMain mainForm)
            {
                mainForm.btnApplications_Click(sender, e);
            }
        }

        private void lblViewAllLicenses_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (this.TopLevelControl is frmMain mainForm)
            {
                mainForm.btnLicenses_Click(sender, e);
            }
        }

        private void ResizeItems1()
        {
            foreach (Control c in flowLayoutPanel1.Controls)
            {
                c.Width = flowLayoutPanel1.ClientSize.Width - 5;
            }
        }
        private void ResizeItems2()
        {
            foreach (Control c in flowLayoutPanel2.Controls)
            {
                c.Width = flowLayoutPanel2.ClientSize.Width - 5;
            }
        }
        private void ResizeItems3()
        {
            foreach (Control c in flowLayoutPanel3.Controls)
            {
                c.Width = flowLayoutPanel3.ClientSize.Width - 5;
            }
        }

        private void flowLayoutPanel1_Resize(object sender, EventArgs e)
        {

            ResizeItems1();
        }

        private void flowLayoutPanel3_Resize(object sender, EventArgs e)
        {
            ResizeItems3();
        }

        private void flowLayoutPanel2_Resize(object sender, EventArgs e)
        {
            ResizeItems2();
        }
    }
}
