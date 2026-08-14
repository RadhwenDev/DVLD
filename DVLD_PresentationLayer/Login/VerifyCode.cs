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
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace DVLD_PresentationLayer.Login
{
    public partial class VerifyCode : Form
    {
        private int _UserID;
        private int _timeLeftInSeconds = 900;

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
            }
            else
            {
                expiryTimer.Stop();

                lblTimer.Text = "Code expired!";
                btnVerify.Enabled = false;
            }
        }
    }
}
