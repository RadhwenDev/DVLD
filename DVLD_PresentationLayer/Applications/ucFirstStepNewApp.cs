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
        // 1. تعريف المتغير العام في أعلى الكلاس خارج كل الميثودز
        private int _selectedPersonID = -1;
        public event EventHandler OnStepOneCompleted;
        public ucFirstStepNewApp()
        {
            InitializeComponent();
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
        }

        private void cbPerson_SelectedIndexChanged(object sender, EventArgs e)
        {
            // 2. التحقق من أن القيمة الحالية ليست DataRowView (لتفادي الـ Crash وقت الـ Load)
            if (cbPerson.SelectedIndex != -1 && cbPerson.SelectedValue != null && !(cbPerson.SelectedValue is DataRowView))
            {
                // الآن نقوم بالتحويل ونحن متأكدون أن القيمة عبارة عن رقم (PersonID)
                int selectedPersonID = Convert.ToInt32(cbPerson.SelectedValue);

                if (selectedPersonID != -1)
                {
                    _selectedPersonID = selectedPersonID;
                    btnContinue.Enabled = true;
                }
                else
                {
                    _selectedPersonID = -1;
                    btnContinue.Enabled = false;
                }
            }
        }

        private void btnContinue_Click(object sender, EventArgs e)
        {
            if (_selectedPersonID != -1)
            {
                // 2. إطلاق الـ Event وتنبيه الـ UserControl الكبير
                // (الـ ?. تضمن أن البرنامج ما يعملش كراش لو ما ثماش شكون يستمع للحدث)
                OnStepOneCompleted?.Invoke(this, e);
            }
        }

        
    }
}