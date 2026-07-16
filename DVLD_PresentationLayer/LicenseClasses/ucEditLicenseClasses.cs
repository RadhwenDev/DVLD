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
    public partial class ucEditLicenseClasses : UserControl
    {
        clsLicenseClass _LicenseClass;
        private int LicenseClassID = -1;
        string ClassName, ClassDescription;
        int MinimumAge, Validity, Fees;

        public delegate void DataBackEventHandler(object sender, int LicenseClassID);
        public event DataBackEventHandler DataBack;

        private void btnCancelEdit_Click(object sender, EventArgs e)
        {
            this.FindForm()?.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
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

        public ucEditLicenseClasses(int LicenseClassID, string ClassName, string ClassDescription, int MinimumAge, int Validity, int Fees)
        {
            InitializeComponent();
            _LicenseClass = new clsLicenseClass(LicenseClassID, ClassName, ClassDescription, MinimumAge, Validity, Fees);
        }

        private void ucEditLicenseClasses_Load(object sender, EventArgs e)
        {
            txtEditClassName.Text = _LicenseClass.ClassName;
            txtEditClassDescription.Text = _LicenseClass.ClassDescription;
            nudEditMinimumAge.Value = _LicenseClass.MinimumAge;
            nudEditValidity.Value = _LicenseClass.Validity;
            nudEditFees.Value = Convert.ToDecimal(_LicenseClass.Fees); ;
        }
    }
}
