using System;
using System.ComponentModel;
using System.Windows.Forms;
using DVLD_BusinessLayer;

namespace DVLD_PresentationLayer.ApplicationTypes
{
    public partial class ucEditApplication : UserControl
    {
        public event Action OnSaveCompleted;
        public event Action OnCancel;

        private int _ApplicationTypeID = -1;
        private clsApplicationType _ApplicationType;
        private ErrorProvider errorProvider1 = new ErrorProvider();

        public int ApplicationTypeID
        {
            get { return _ApplicationTypeID; }
        }

        public ucEditApplication()
        {
            InitializeComponent();
        }

        public void LoadApplicationTypeData(int applicationTypeID)
        {
            _ApplicationTypeID = applicationTypeID;
            _ApplicationType = clsApplicationType.Find(_ApplicationTypeID);

            if (_ApplicationType == null)
            {
                MessageBox.Show("No Application Type with ID = " + _ApplicationTypeID, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            lblID.Text = _ApplicationType.ID.ToString();
            txtTitle.Text = _ApplicationType.Title;
            txtFees.Text = _ApplicationType.Fees.ToString("0.00");
        }

        private void ucEditApplication_Load(object sender, EventArgs e)
        {
            if (_ApplicationTypeID != -1)
            {
                LoadApplicationTypeData(_ApplicationTypeID);
            }
        }


        private void txtTitle_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                e.Cancel = true;
                errorProvider1.SetError(txtTitle, "Title cannot be empty!");
            }
            else
            {
                e.Cancel = false;
                errorProvider1.SetError(txtTitle, "");
            }
        }

        private void txtFees_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFees.Text))
            {
                e.Cancel = true;
                errorProvider1.SetError(txtFees, "Fees cannot be empty!");
                return;
            }

            if (!decimal.TryParse(txtFees.Text.Trim(), out _))
            {
                e.Cancel = true;
                errorProvider1.SetError(txtFees, "Invalid Number!");
            }
            else
            {
                e.Cancel = false;
                errorProvider1.SetError(txtFees, "");
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            OnCancel?.Invoke();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!this.ValidateChildren())
            {
                MessageBox.Show("Some fields are not valid! Put the mouse over the red icon(s) to see the error", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            _ApplicationType.Title = txtTitle.Text.Trim();
            _ApplicationType.Fees = Convert.ToDecimal(txtFees.Text.Trim());

            clsApplicationType.enSaveResult saveResult = _ApplicationType.Save();

            switch (saveResult)
            {
                case clsApplicationType.enSaveResult.SavedSuccessfully:
                    MessageBox.Show("Data Saved Successfully.", "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    OnSaveCompleted?.Invoke();
                    break;

                case clsApplicationType.enSaveResult.NoChanges:
                    MessageBox.Show("No changes were made to save.", "No Changes", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;

                case clsApplicationType.enSaveResult.Failed:
                    MessageBox.Show("Error: Data Was not Saved Successfully.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
            }
        }
    }
}