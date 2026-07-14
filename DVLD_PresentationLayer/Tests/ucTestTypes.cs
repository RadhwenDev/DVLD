using DVLD_BusinessLayer;
using DVLD_PresentationLayer.Users;
using Guna.UI2.WinForms.Suite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DVLD_PresentationLayer.Tests
{
    public partial class ucTestTypes : UserControl
    {
        public ucTestTypes()
        {
            InitializeComponent();
        }
        private int _FirstTestTypeID = -1;
        private int _SecondTestTypeID = -1;
        private int _ThirdTestTypeID = -1;

        private int _FirstFee = 0;
        private int _SecondFee = 0;
        private int _ThirdFee = 0;
        private void ucTestTypes_Load(object sender, EventArgs e)
        {

            DataTable dtAllTestTypes = clsTestTypes.getAllTestTypes();

            if (dtAllTestTypes != null && dtAllTestTypes.Rows.Count > 0)
            {
                _FirstTestTypeID = Convert.ToInt32(dtAllTestTypes.Rows[0]["TestTypeID"]);
                lblFirstStep.Text = dtAllTestTypes.Rows[0]["TestTypeTitle"].ToString();
                lblTSFirstStep.Text = lblFirstStep.Text;
                _FirstFee = Convert.ToInt32(dtAllTestTypes.Rows[0]["TestTypeFees"]);
                lblFirstTestFee.Text = "$ " + _FirstFee;
                lblFirstDescription.Text = dtAllTestTypes.Rows[0]["TestTypeDescription"].ToString();

        
                _SecondTestTypeID = Convert.ToInt32(dtAllTestTypes.Rows[1]["TestTypeID"]);
                lblSecondStep.Text = dtAllTestTypes.Rows[1]["TestTypeTitle"].ToString();
                lblTSSecondStep.Text = lblSecondStep.Text;
                _SecondFee = Convert.ToInt32(dtAllTestTypes.Rows[1]["TestTypeFees"]);
                lblSecondTestFee.Text = "$ " + _SecondFee;
                lblSecondDescription.Text = dtAllTestTypes.Rows[1]["TestTypeDescription"].ToString();


                _ThirdTestTypeID = Convert.ToInt32(dtAllTestTypes.Rows[2]["TestTypeID"]);
                lblThirdStep.Text = dtAllTestTypes.Rows[2]["TestTypeTitle"].ToString();
                lblTSThirdStep.Text = lblThirdStep.Text;
                _ThirdFee = Convert.ToInt32(dtAllTestTypes.Rows[2]["TestTypeFees"]);
                lblThirdTestFee.Text = "$ " + _ThirdFee;
                lblThirdDescription.Text = dtAllTestTypes.Rows[2]["TestTypeDescription"].ToString();
            }

        }

        int selectedPersonID = -1;
        private void btnFirstEdit_Click(object sender, EventArgs e)
        {

            ShowUserControl(_FirstTestTypeID, lblFirstStep.Text, lblFirstDescription.Text, _FirstFee);
        }

        private void ShowUserControl(int testTypeID, string title, string description, int fee)
        {
            using (Form overlay = new Form())
            {
                overlay.StartPosition = FormStartPosition.Manual;
                overlay.FormBorderStyle = FormBorderStyle.None;
                overlay.BackColor = Color.FromArgb(45, 55, 72);
                overlay.Opacity = 0.45d;
                overlay.Bounds = Screen.FromControl(this).Bounds;
                overlay.ShowInTaskbar = false;
                overlay.Show(this);

                using (Form frmContainer = new Form())
                {
                    frmContainer.FormBorderStyle = FormBorderStyle.None;
                    frmContainer.BackColor = Color.White;
                    frmContainer.StartPosition = FormStartPosition.CenterParent;

                    ucEditTestTypes myEditTestTypes = new ucEditTestTypes(testTypeID, title, description, fee);
                    myEditTestTypes.DataBack += MyEditTestTypes_DataBack;
                    frmContainer.Size = myEditTestTypes.Size;
                    myEditTestTypes.Dock = DockStyle.Fill;
                    frmContainer.Controls.Add(myEditTestTypes);

                    // 🌟 السطر السحري: ربط الـ Delegate الخاص بالـ User Control بالدالة المخصصة للتحديث

                    Guna.UI2.WinForms.Guna2Elipse elipse = new Guna.UI2.WinForms.Guna2Elipse();
                    elipse.TargetControl = frmContainer;
                    elipse.BorderRadius = 16;
                    frmContainer.ShowDialog(overlay);
                }
            }
        }

        private void MyEditTestTypes_DataBack(object sender, int TestTypeID)
        {
            ucTestTypes_Load(null, null);
        }

        private void btnSecondEdit_Click(object sender, EventArgs e)
        {
            ShowUserControl(_SecondTestTypeID, lblSecondStep.Text, lblSecondDescription.Text, _SecondFee);
        }

        private void btnThirdEdit_Click(object sender, EventArgs e)
        {
            ShowUserControl(_ThirdTestTypeID, lblThirdStep.Text, lblThirdDescription.Text, _ThirdFee);
        }
    }
}
