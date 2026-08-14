using DVLD_BusinessLayer;
using DVLD_Security;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DVLD_PresentationLayer.Login
{
    public partial class ResetPassword : Form
    {
        private int _UserID;
        public ResetPassword(int UserID)
        {
            InitializeComponent();
            _UserID = UserID;
        }

        private void btnResetPassword_Click(object sender, EventArgs e)
        {
            string newPassword = txtNewPassword.Text;
            string confirmPassword = txtConfirmPassword.Text;

            if (string.IsNullOrWhiteSpace(newPassword))
            {
                MessageBox.Show("Please enter a new password.");
                txtNewPassword.Focus();
                return;
            }

            if (newPassword != confirmPassword)
            {
                MessageBox.Show("Passwords do not match.");
                txtConfirmPassword.Focus();
                return;
            }
            string hashedPassword =
    HashHelper.ComputeSHA256(newPassword);

            clsUsers user = new clsUsers(_UserID);

            user.Password = hashedPassword;

            clsUsers.enSaveResult result = user.SavePassword();

            if (result == clsUsers.enSaveResult.SavedSuccessfully)
            {
                clsUsers.MarkResetCodeAsUsed(_UserID);

                MessageBox.Show(
                    "Password reset successfully.",
                    "Success",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show(
                    "Failed to reset password.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
