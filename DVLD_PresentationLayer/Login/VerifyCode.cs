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

        public VerifyCode(int UserID)
        {
            InitializeComponent();

            _UserID = UserID;
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

                // بعدين نفتح Reset Password
                ResetPassword frm = new ResetPassword(_UserID);
                frm.ShowDialog();

                this.Close();
            }
            else
            {
                MessageBox.Show("Invalid or expired code.");
            }
        }
    }
}
