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

namespace DVLD_PresentationLayer.Users
{
    public partial class ucShowDetailsUser : UserControl
    {
        int _UserID = -1;
        public ucShowDetailsUser(int UserID)
        {
            InitializeComponent();
            _UserID = UserID;
        }

        private void ucShowDetailsUser_Load(object sender, EventArgs e)
        {
            DataTable dt = clsUsers.getAllDetailsForShowButton(_UserID);

            string statusName = dt.Rows[0]["Status"].ToString();
            if (statusName == "Active")
            {
                statusBadge.Text = "Active";
                statusBadge.FillColor = Color.FromArgb(232, 245, 233); // أخضر فاتح
                statusBadge.ForeColor = Color.FromArgb(56, 142, 60);
                statusBadge.BorderColor = Color.FromArgb(56, 142, 60);
            }
            else
            {
                statusBadge.Text = "Inactive";
                statusBadge.FillColor = Color.FromArgb(254, 234, 234); // أحمر فاتح
                statusBadge.ForeColor = Color.FromArgb(183, 28, 28);
                statusBadge.BorderColor = Color.FromArgb(183, 28, 28);
            }

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.FindForm()?.Close();
        }
    }
}
