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

namespace DVLD_PresentationLayer.Tests
{
    public partial class ucTestTypes : UserControl
    {
        public ucTestTypes()
        {
            InitializeComponent();
        }

        private void ucTestTypes_Load(object sender, EventArgs e)
        {

            DataTable _dtAllTestTypes = clsTestTypes.getAllTestTypes();

            if (_dtAllTestTypes != null && _dtAllTestTypes.Rows.Count > 0)
            {
                lblFirstStep.Text = _dtAllTestTypes.Rows[0]["TestTypeTitle"].ToString();
                lblTSFirstStep.Text = lblFirstStep.Text;
                lblFirstTestFee.Text = "$ " + Convert.ToInt32(_dtAllTestTypes.Rows[0]["TestTypeFees"]).ToString();
                lblFirstDescription.Text = _dtAllTestTypes.Rows[0]["TestTypeDescription"].ToString();
                lblSecondStep.Text = _dtAllTestTypes.Rows[1]["TestTypeTitle"].ToString();
                lblTSSecondStep.Text = lblSecondStep.Text;
                lblSecondTestFee.Text = "$ " + Convert.ToInt32(_dtAllTestTypes.Rows[1]["TestTypeFees"]).ToString();
                lblSecondDescription.Text = _dtAllTestTypes.Rows[1]["TestTypeDescription"].ToString();
                lblThirdStep.Text = _dtAllTestTypes.Rows[2]["TestTypeTitle"].ToString();
                lblTSThirdStep.Text = lblThirdStep.Text;
                lblThirdTestFee.Text = "$ " + Convert.ToInt32(_dtAllTestTypes.Rows[2]["TestTypeFees"]).ToString();
                lblThirdDescription.Text = _dtAllTestTypes.Rows[2]["TestTypeDescription"].ToString();
            }

        }

        bool isUpdateMode = true;
        private void EditDesign(object sender, EventArgs e)
        {
            if (isUpdateMode)
            {
                btnFirstEdit.BackgroundImage = null;
                btnFirstEdit.Text = "❌";
                btnFirstSave.Visible = true;
            }
            else
            {
                

                isUpdateMode = true;
            }
        }
        private void btnFirstEdit_Click(object sender, EventArgs e)
        {
            EditDesign(sender, e);
        }

        private void btnSecondEdit_Click(object sender, EventArgs e)
        {
            EditDesign(sender, e);
        }

        private void btnThirdEdit_Click(object sender, EventArgs e)
        {
            EditDesign(sender, e);
        }
    }
}
