using DVLD_BusinessLayer;
using DVLD_PresentationLayer.Tests;
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
    public partial class ucShowDetailsLicenseClass : UserControl
    {
        public event EventHandler<int> LicenseClassUpdated;
        public ucShowDetailsLicenseClass()
        {
            InitializeComponent();
        }
        private int _LicenseClassID = -1;
        string ClassName = "";
        string ClassDescription = "";
        int MinimumAllowedAge = 18;
        int DefaultValidityLength = 0;
        int ClassFees = 0;

        public void LoadData(int LicenseClassID, string ClassName, string Description, int MinAge, int Validity, decimal Fees)
        {
            _LicenseClassID = LicenseClassID;

            // ربط البيانات بالـ Labels في واجهة العرض (pnlShowInfo)
            lblClassName.Text = ClassName;
            this.ClassName = ClassName;
            lblClassDescription.Text = Description;
            this.ClassDescription = Description;
            lblMinAge.Text = MinAge.ToString() + " yrs";
            this.MinimumAllowedAge = MinAge;
            lblValidity.Text = Validity.ToString() + " yrs";
            this.DefaultValidityLength = Validity;
            lblFee.Text = "$ " + Convert.ToInt32(Fees);
            this.ClassFees = (int)Fees;

            // ربط الأيقونة المناسبة حسب الـ ID (مثل فكرة الـ TestTypes التي قمنا بها سابقاً)
            // pbClassIcon.Image = ...
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            ShowUserControl(_LicenseClassID, ClassName, ClassDescription, MinimumAllowedAge, DefaultValidityLength, ClassFees);
        }
        private void ShowUserControl(int LicenseClassID, string ClassName, string ClassDescription, int MinimumAllowedAge, int DefaultValidityLength, int ClassFees)
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

                    ucEditLicenseClasses myEditLicenseClasses = new ucEditLicenseClasses(LicenseClassID, ClassName, ClassDescription, MinimumAllowedAge, DefaultValidityLength, ClassFees);
                    myEditLicenseClasses.DataBack += MyEditLicenseClasses_DataBack;
                    frmContainer.Size = myEditLicenseClasses.Size;
                    myEditLicenseClasses.Dock = DockStyle.Fill;
                    frmContainer.Controls.Add(myEditLicenseClasses);

                    // 🌟 السطر السحري: ربط الـ Delegate الخاص بالـ User Control بالدالة المخصصة للتحديث

                    Guna.UI2.WinForms.Guna2Elipse elipse = new Guna.UI2.WinForms.Guna2Elipse();
                    elipse.TargetControl = frmContainer;
                    elipse.BorderRadius = 16;
                    frmContainer.ShowDialog(overlay);
                }
            }
        }

        private void MyEditLicenseClasses_DataBack(object sender, int LicenseClassID)
        {
            LicenseClassUpdated?.Invoke(this, LicenseClassID);
        }
    }
}
