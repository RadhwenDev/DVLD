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
using DVLD_Security;

namespace DVLD_PresentationLayer.Users
{
    public partial class ucChangePassword : UserControl
    {
        clsUsers _User = new clsUsers();
        public ucChangePassword(clsUsers User)
        {
            InitializeComponent();
            _User = User;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.FindForm()?.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtCurrentPassword.Text) ||
                string.IsNullOrEmpty(txtNewPassword.Text) ||
                string.IsNullOrEmpty(txtConfirmPassword.Text))
            {
                MessageBox.Show("Please fill in all password fields.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            string hashedPassword = HashHelper.ComputeSHA256(txtCurrentPassword.Text.Trim());

            if (_User.Password != hashedPassword)
            {
                MessageBox.Show("The current password you entered is incorrect!\nTry again!!", "Wrong Password", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (txtNewPassword.Text != txtConfirmPassword.Text)
            {
                MessageBox.Show("The new password and confirmation password do not match!\nEnter the same password in both boxes (New - Confirm)!!", "Password Mismatch", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string hashedNewPassword = HashHelper.ComputeSHA256(txtNewPassword.Text.Trim());
            _User.Password = hashedNewPassword;

            switch (_User.SavePassword())
            {
                case clsUsers.enSaveResult.SavedSuccessfully:
                    MessageBox.Show($"Password saved successfully with ID = {_User.PersonID}!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.FindForm()?.Close();
                    break;

                case clsUsers.enSaveResult.NoChanges:
                    MessageBox.Show("Nothing was changed.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;
            }
        }

    }
}
