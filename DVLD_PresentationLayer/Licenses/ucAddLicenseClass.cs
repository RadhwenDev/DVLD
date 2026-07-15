using DVLD_BusinessLayer;
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
    public partial class ucAddLicenseClass : UserControl
    {
        clsLicenseClass _LicenseClass;
        private int LicenseClassID = -1;
        string ClassName, ClassDescription;
        int MinimumAge, Validity, Fees;

        public delegate void DataBackEventHandler(object sender, int LicenseClassID);
        public event DataBackEventHandler DataBack;

        private ErrorProvider errorProvider1 = new ErrorProvider();

        public ucAddLicenseClass()
        {
            InitializeComponent();
            _LicenseClass = new clsLicenseClass();
        }
        string namePattern = @"^[a-zA-Z\u0600-\u06FF\s\'\s]+$";
        private void txtEditClassName_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtEditClassName.Text))
            {
                errorProvider1.SetError(txtEditClassName, "Class name is required.");
                txtEditClassName.Focus();
                return;
            }
            if (!System.Text.RegularExpressions.Regex.Match(txtEditClassName.Text.Trim(), namePattern).Success)
            {
                errorProvider1.SetError(txtEditClassName, "Class name must contain letters only.");
                txtEditClassName.Focus();
                return;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.FindForm()?.Close();
        }

        private void btnCancelX_Click(object sender, EventArgs e)
        {
            this.FindForm()?.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            errorProvider1.Clear();
            if (string.IsNullOrWhiteSpace(txtEditClassName.Text)) { errorProvider1.SetError(txtEditClassName, "Class name is required."); txtEditClassName.Focus(); return; }


            _LicenseClass.ClassName = txtEditClassName.Text.Trim();
            _LicenseClass.ClassDescription = txtEditClassDescription.Text.Trim();
            _LicenseClass.MinimumAge = Convert.ToInt32(nudEditMinimumAge.Value);
            _LicenseClass.Validity = Convert.ToInt32(nudEditValidity.Value);
            _LicenseClass.Fees = Convert.ToInt32(nudEditFees.Value);
            switch (_LicenseClass.Save())
            {
                case clsLicenseClass.enSaveResult.SavedSuccessfully:
                    MessageBox.Show($"Test Type saved successfully with ID = {_LicenseClass.LicenseClassID}!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LicenseClassID = _LicenseClass.LicenseClassID;

                    DataBack?.Invoke(this, _LicenseClass.LicenseClassID);
                    this.FindForm()?.Close();
                    break;
                case clsLicenseClass.enSaveResult.NoChanges:
                    MessageBox.Show("Nothing was changed");
                    break;
                case clsLicenseClass.enSaveResult.Failed:
                    MessageBox.Show("Failed to save person data.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
            }
        }
    }
}
