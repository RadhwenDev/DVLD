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
    }
}
