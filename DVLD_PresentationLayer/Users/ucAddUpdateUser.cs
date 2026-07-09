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
    public partial class ucAddUpdateUser : UserControl
    {
        public ucAddUpdateUser()
        {
            InitializeComponent();
        }
        public delegate void DataBackEventHandler(object sender, int PersonID);
        public event DataBackEventHandler DataBack;
        private void ucAddUpdateUser_Load(object sender, EventArgs e)
        {

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.FindForm()?.Close();
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            this.FindForm()?.Close();
        }
    }
}
