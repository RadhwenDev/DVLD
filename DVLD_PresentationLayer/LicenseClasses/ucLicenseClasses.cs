using DVLD_BusinessLayer;
using DVLD_PresentationLayer.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DVLD_PresentationLayer.Licenses
{
    public partial class ucLicenseClasses : UserControl
    {
        public ucLicenseClasses()
        {
            InitializeComponent();
        }

        private void LoadLicenseClasses()
        {
            DataTable dtLicenseClasses = clsLicenseClass.GetAllLicenseClasses();

            flowLayoutPanel1.Controls.Clear();

            foreach (DataRow row in dtLicenseClasses.Rows)
            {
                ucShowDetailsLicenseClass card = new ucShowDetailsLicenseClass();

                card.LicenseClassUpdated += Card_LicenseClassUpdated;

                card.LoadData(
                        Convert.ToInt32(row["LicenseClassID"]),
                        row["ClassName"].ToString().Contains('-')
                        ? row["ClassName"].ToString().Substring(row["ClassName"].ToString().IndexOf('-') + 1).Trim()
                        : row["ClassName"].ToString(),
                         row["ClassDescription"].ToString(),
                        Convert.ToInt32(row["MinimumAllowedAge"]),
                        Convert.ToInt32(row["DefaultValidityLength"]),
                        Convert.ToDecimal(row["ClassFees"])
                    );
                lblCountLicenseCategoriesConfigured.Text = dtLicenseClasses.Rows.Count.ToString() + " license categories configured";

                card.Width = flowLayoutPanel1.ClientSize.Width - 20;
                card.Margin = new Padding(0, 0, 0, 25);

                flowLayoutPanel1.Controls.Add(card);
            }
        }
        private void ucLicenseClasses_Load(object sender, EventArgs e)
        {
            LoadLicenseClasses();
        }

        private void flowLayoutPanel1_Resize(object sender, EventArgs e)
        {
            foreach (Control c in flowLayoutPanel1.Controls)
            {
                c.Width = flowLayoutPanel1.ClientSize.Width - 20;
            }
            flowLayoutPanel1.ResumeLayout();
        }

        private void Card_LicenseClassUpdated(object sender, int LicenseClassID)
        {
            LoadLicenseClasses();
        }

        private void btnAddClass_Click(object sender, EventArgs e)
        {
            ShowUserControl();
        }

        private void ShowUserControl()
        {
            using (Form overlay = new Form())
            {
                overlay.StartPosition = FormStartPosition.Manual;
                overlay.FormBorderStyle = FormBorderStyle.None;
                overlay.BackColor = Color.FromArgb(45, 55, 72);
                overlay.Opacity = 0.45d;
                overlay.Bounds = Screen.FromControl(this).Bounds;
                overlay.ShowInTaskbar = false;
                overlay.Show(this);

                using (Form frmContainer = new Form())
                {
                    frmContainer.FormBorderStyle = FormBorderStyle.None;
                    frmContainer.BackColor = Color.White;
                    frmContainer.StartPosition = FormStartPosition.CenterParent;

                    ucAddLicenseClass myAddLicenseClass = new ucAddLicenseClass();
                    frmContainer.Size = myAddLicenseClass.Size;
                    myAddLicenseClass.Dock = DockStyle.Fill;
                    frmContainer.Controls.Add(myAddLicenseClass);

                    // 🌟 السطر السحري: ربط الـ Delegate الخاص بالـ User Control بالدالة المخصصة للتحديث
                    myAddLicenseClass.DataBack += MyAddLicenseClass_DataBack;

                    Guna.UI2.WinForms.Guna2Elipse elipse = new Guna.UI2.WinForms.Guna2Elipse();
                    elipse.TargetControl = frmContainer;
                    elipse.BorderRadius = 16;
                    frmContainer.ShowDialog(overlay);
                }
            }
        }
        private void MyAddLicenseClass_DataBack(object sender, int PersonID)
        {
            LoadLicenseClasses();
        }
    }
}
