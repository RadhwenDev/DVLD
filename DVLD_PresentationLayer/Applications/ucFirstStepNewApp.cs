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

namespace DVLD_PresentationLayer.Applications
{
    public partial class ucFirstStepNewApp : UserControl
    {
        // تغيير المتغير إلى التسمية العامة الصحيحة للوصول إليه من الحاوية الكبرى
        public int SelectedPersonID { get; private set; } = -1;
        public event EventHandler OnStepOneCompleted;

        public ucFirstStepNewApp()
        {
            InitializeComponent();
        }

        // Constructor يستقبل القيمة المحفوظة مركزياً
        public ucFirstStepNewApp(int personID) : this()
        {
            SelectedPersonID = personID;
        }

        private void ucFirstStepNewApp_Load(object sender, EventArgs e)
        {
            DataTable dtTypes = clsPerson.GetPeopleAplicationFullName();
            DataRow defaultRow = dtTypes.NewRow();
            defaultRow["FullName"] = "Select the Person";
            defaultRow["PersonID"] = -1;
            dtTypes.Rows.InsertAt(defaultRow, 0);

            cbPerson.DataSource = dtTypes;
            cbPerson.DisplayMember = "FullName";
            cbPerson.ValueMember = "PersonID";

            // إعادة تحديد الشخص المختار سابقاً إن وجد
            if (SelectedPersonID != -1)
            {
                cbPerson.SelectedValue = SelectedPersonID;
                btnContinue.Enabled = true;
            }
        }

        private void cbPerson_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbPerson.SelectedIndex != -1 && cbPerson.SelectedValue != null && !(cbPerson.SelectedValue is DataRowView))
            {
                int selectedPersonID = Convert.ToInt32(cbPerson.SelectedValue);

                if (selectedPersonID != -1)
                {
                    SelectedPersonID = selectedPersonID;
                    btnContinue.Enabled = true;
                }
                else
                {
                    SelectedPersonID = -1;
                    btnContinue.Enabled = false;
                }
            }
        }

        private void btnContinue_Click(object sender, EventArgs e)
        {
            if (SelectedPersonID != -1)
            {
                OnStepOneCompleted?.Invoke(this, e);
            }
        }
    }
}