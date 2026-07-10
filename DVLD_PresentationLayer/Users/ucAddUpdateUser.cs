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
using static DVLD_PresentationLayer.ucAddUpdatePerson;

namespace DVLD_PresentationLayer.Users
{
    public partial class ucAddUpdateUser : UserControl
    {
        enum enMode { AddNew, Update}
        private enMode _Mode;
        clsUsers _User;
        private int _UserID = -1;
        public ucAddUpdateUser()
        {
            InitializeComponent();
        }
        public delegate void DataBackEventHandler(object sender, int PersonID);
        public event DataBackEventHandler DataBack;
        private ErrorProvider errorProvider1 = new ErrorProvider();

        private void ucAddUpdateUser_Load(object sender, EventArgs e)
        {
            DataTable dtPeople = DVLD_BusinessLayer.clsPerson.GetPeople();

            if (dtPeople != null)
            {
                dtPeople.Columns.Add("USER", typeof(string));

                // 2. نمر على السطور ونقوم بالدمج والتنظيف معاً في الـ Loop
                foreach (DataRow row in dtPeople.Rows)
                {
                    string firstName = row["FirstName"]?.ToString() ?? "";
                    string secondName = row["SecondName"]?.ToString() ?? "";
                    string thirdName = row["ThirdName"]?.ToString() ?? "";
                    string lastName = row["LastName"]?.ToString() ?? "";

                    string fullName = $"{firstName} {secondName} {thirdName} {lastName}";

                    // تنظيف المسافات الزائدة
                    fullName = fullName.Replace("   ", " ").Replace("  ", " ").Trim();

                    row["USER"] = fullName;
                }

                // إضافة السطر الوهمي كالـ Placeholder الذكي
                foreach (DataColumn column in dtPeople.Columns)
                    column.AllowDBNull = true;

                // الآن الجدول أصبح مرناً تماماً، أضف سطر الـ Placeholder بأمان!
                DataRow dr = dtPeople.NewRow();
                dr["PersonID"] = 0;
                dr["USER"] = "Select a person ...";

                dtPeople.Rows.InsertAt(dr, 0);

                // نقل كود الربط إلى داخل الشرط للحماية الكاملة
                cbPerson.DataSource = dtPeople;
                cbPerson.DisplayMember = "USER";
                cbPerson.ValueMember = "PersonID";

                // تأكيد اختيار السطر الأول افتراضياً
                cbPerson.SelectedIndex = 0;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.FindForm()?.Close();
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            this.FindForm()?.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            errorProvider1.Clear();
            int selectedPersonID = (cbPerson.SelectedValue != null) ? (int)cbPerson.SelectedValue : 0;

            // الفحص الأول: التأكد من أنه لم يترك القائمة على "Select a person ..."
            if (selectedPersonID == 0)
            {
                errorProvider1.SetError(cbPerson, "Please select a valid person.");
                cbPerson.Focus();
                return;
            }
            // الفحص الثاني: التأكد من أن الشخص المختار ليس مستخدماً بالفعل (Déjà User)
            else if (DVLD_BusinessLayer.clsUsers.IsUserExistForPersonID(selectedPersonID))
            {
                errorProvider1.SetError(cbPerson, "This person is already a user in the system!");
                cbPerson.Focus();
                return;
            }
            else
            {
                // تنظيف الخطأ تماماً إذا كانت كل الشروط سليمة
                errorProvider1.SetError(cbPerson, "");
            }
            if (string.IsNullOrWhiteSpace(txtUserName.Text)) 
            { 
                errorProvider1.SetError(txtUserName, "User name is required."); 
                txtUserName.Focus(); 
                return; 
            }
            else if (DVLD_BusinessLayer.clsUsers.IsUserNameExistForPersonID(txtUserName.Text))
            {
                errorProvider1.SetError(txtUserName, "This username is already a used in the system!");
                txtUserName.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(txtPassword.Text)) { errorProvider1.SetError(txtPassword, "Password is required."); txtPassword.Focus(); return; }


            if (_Mode == enMode.AddNew)
                _User = new clsUsers(-1, (int)cbPerson.SelectedValue, txtUserName.Text, txtPassword.Text, -1, tsIsActive.Checked);
            switch (_User.Save())
            {
                case clsUsers.enSaveResult.SavedSuccessfully:
                    MessageBox.Show($"Person saved successfully with ID = {_User.PersonID}!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    _Mode = enMode.Update; // تغيير الوضع إلى تعديل بعد النجاح الفوري
                    _UserID = _User.PersonID;

                    DataBack?.Invoke(this, _User.PersonID);
                    this.FindForm()?.Close();
                    break;
                case clsUsers.enSaveResult.NoChanges:
                    MessageBox.Show("Nothing was changed");
                    break;
                case clsUsers.enSaveResult.Failed:
                    MessageBox.Show("Failed to save person data.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
            }
        }
    }
}
