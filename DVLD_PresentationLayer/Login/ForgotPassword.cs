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

namespace DVLD_PresentationLayer.Login
{
    public partial class ForgotPassword : Form
    {
        public ForgotPassword()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.FindForm()?.Close();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            string userName = txtUsername.Text.Trim();

            if (string.IsNullOrEmpty(userName))
            {
                MessageBox.Show("Please enter your username.");
                txtUsername.Focus();
                return;
            }

            int userID = -1;
            int personID = -1;
            string email = "";

            bool userFound = clsUsers.GetUserInfoForPasswordReset(
                userName,
                ref userID,
                ref personID,
                ref email);

            if (!userFound)
            {
                MessageBox.Show("Username not found.");
                return;
            }
            if (string.IsNullOrWhiteSpace(email))
            {
                MessageBox.Show("This user does not have an email address.");
                return;
            }

            MessageBox.Show("User found. Email: " + email);
        }
    }
}
