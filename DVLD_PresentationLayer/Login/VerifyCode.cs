using DVLD_BusinessLayer;
using DVLD_EmailService;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace DVLD_PresentationLayer.Login
{
    public partial class VerifyCode : Form
    {
        private int _UserID;
        private int _timeLeftInSeconds = 900;
        private int _resendCooldown = 0;
        private Timer _resendTimer;

        public VerifyCode(int UserID)
        {
            InitializeComponent();

            _UserID = UserID;

            expiryTimer.Start();
        }

        private void btnVerify_Click(object sender, EventArgs e)
        {
            string code = txtCode.Text.Trim();

            if (string.IsNullOrEmpty(code))
            {
                MessageBox.Show("Please enter the verification code.");
                txtCode.Focus();
                return;
            }

            if (clsUsers.VerifyPasswordResetCode(_UserID, code))
            {
                MessageBox.Show("Code verified successfully.");

                ResetPassword frm = new ResetPassword(_UserID);
                frm.ShowDialog();

                this.FindForm().Close();
            }
            else
            {
                MessageBox.Show("Invalid or expired code.");
            }
        }

        private void expiryTimer_Tick(object sender, EventArgs e)
        {
            if (_timeLeftInSeconds > 0)
            {
                _timeLeftInSeconds--;

                TimeSpan time = TimeSpan.FromSeconds(_timeLeftInSeconds);
                lblTimer.Text = $"{time:mm\\:ss}";

                if (_timeLeftInSeconds <= 60)
                {
                    lblTimer.ForeColor = Color.Red;
                }
                else if (_timeLeftInSeconds <= 300)
                {
                    lblTimer.ForeColor = Color.Orange;
                }
                else
                {
                    lblTimer.ForeColor = Color.Black;
                }
            }
            else
            {
                expiryTimer.Stop();

                lblTimer.Text = "Code expired!";
                lblTimer.ForeColor = Color.Red;

                btnVerify.Enabled = false;
            }
        }
        private void ResendResetCode()
        {
            string resetCode = clsUsers.GeneratePasswordResetCode(_UserID);

            if (string.IsNullOrEmpty(resetCode))
            {
                MessageBox.Show("Failed to generate a new verification code.");
                return;
            }

            string email = clsUsers.GetUserEmailByID(_UserID);

            if (string.IsNullOrWhiteSpace(email))
            {
                MessageBox.Show("This user does not have an email address.");
                return;
            }

            IEmailService emailService = new EmailService();

            bool emailSent = emailService.SendEmail(
                email,
                "DVLD - New Password Reset Code",
                "Your new DVLD password reset code is:\r\n\r\n" +
                resetCode +
                "\r\n\r\nThis code will expire in 15 minutes.");

            if (emailSent)
            {
                MessageBox.Show("A new verification code has been sent to your email.");

                _timeLeftInSeconds = 900;
                lblTimer.Text = "15:00";
                lblTimer.ForeColor = Color.Black;

                _resendCooldown = 60;
                lblResendCode.Enabled = false;
                lblResendCode.Text = $"Resend Code ({_resendCooldown}s)";

                resendTimer.Start();
            }
            else
            {
                MessageBox.Show("Failed to send the verification code.");
            }
        }

        private void lblResendCode_Click(object sender, EventArgs e)
        {
            if (_resendCooldown > 0)
                return;
            ResendResetCode();
        }

        private void resendTimer_Tick(object sender, EventArgs e)
        {
            _resendCooldown--;

            if (_resendCooldown <= 0)
            {
                resendTimer.Stop();
                lblResendCode.Enabled = true;
                lblResendCode.Text = "Resend Code";
            }
            else
            {
                lblResendCode.Text = $"Resend Code ({_resendCooldown}s)";
            }
        }
    }
}
