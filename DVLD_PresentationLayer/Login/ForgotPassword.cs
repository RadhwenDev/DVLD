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
using DVLD_EmailService;

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

            // Generate the reset code and save its hash in the database
            string resetCode = clsUsers.GeneratePasswordResetCode(userID);

            if (string.IsNullOrEmpty(resetCode))
            {
                MessageBox.Show("Failed to generate password reset code.");
                return;
            }

            // Send the reset code by email
            IEmailService emailService = new EmailService();

            string subject = "DVLD - Password Reset Code";

            string body =
                "Hello,\r\n\r\n" +
                "Your DVLD password reset code is:\r\n\r\n" +
                resetCode + "\r\n\r\n" +
                "This code will expire in 15 minutes.\r\n\r\n" +
                "If you did not request a password reset, please ignore this email.";

            bool emailSent = emailService.SendEmail(
                email,
                subject,
                body);

            if (!emailSent)
            {
                MessageBox.Show(
                    "Failed to send the verification code. Please try again.");
                return;
            }

            // Open verification form
            VerifyCode frm = new VerifyCode(userID);
            frm.ShowDialog();

            this.FindForm()?.Close();
        }
    }
}
