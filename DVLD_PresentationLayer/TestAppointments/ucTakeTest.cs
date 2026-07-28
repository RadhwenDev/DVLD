using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DVLD_PresentationLayer.TestAppointments
{
    public partial class ucTakeTest : UserControl
    {
        int _AppID = -1;
        int _TestTypeID = -1;
        public ucTakeTest(int AppID, int TestTypeID)
        {
            InitializeComponent();
            this._AppID = AppID;
            this._TestTypeID = TestTypeID;
        }

        private void ucTakeTest_Load(object sender, EventArgs e)
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ucTakeTest));
            switch (_TestTypeID)
            {
                case 1:
                    lblTestType.Text = "Vision Test";
                    pbtypeTest.Image = (Image)resources.GetObject("visionAppointment");
                    break;
                case 2:
                    lblTestType.Text = "Written Test";
                    pbtypeTest.Image = (Image)resources.GetObject("Written");
                    break;
                case 3:
                    lblTestType.Text = "Street Test";
                    pbtypeTest.Image = (Image)resources.GetObject("Street");
                    break;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.FindForm()?.Close();
        }
    }
}
