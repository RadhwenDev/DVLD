using DVLD_BusinessLayer;
using DVLD_PresentationLayer.Global;
using DVLD_Security;
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
        enum enMode { AddNew, Update }
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
            _FillPeopleComboBox();

            if (_Mode == enMode.AddNew)
            {
                lblHeaderTitle.Text = "Add New User";
                btnSave.Text = "Add User";
                cbPerson.SelectedIndex = 0;
            }
        }

        private void _FillPeopleComboBox()
        {
            // فحص إذا كان الـ ComboBox ممتلئاً بالفعل لمنع التكرار
            if (cbPerson.DataSource != null) return;

            DataTable dtPeople = DVLD_BusinessLayer.clsPerson.GetPeopleFullName();

            if (dtPeople != null)
            {
                dtPeople.Columns.Add("USER", typeof(string));

                foreach (DataRow row in dtPeople.Rows)
                {
                    string firstName = row["FirstName"]?.ToString() ?? "";
                    string secondName = row["SecondName"]?.ToString() ?? "";
                    string thirdName = row["ThirdName"]?.ToString() ?? "";
                    string lastName = row["LastName"]?.ToString() ?? "";

                    string fullName = $"{firstName} {secondName} {thirdName} {lastName}";
                    fullName = fullName.Replace("    ", " ").Replace("  ", " ").Trim();

                    row["USER"] = fullName;
                }

                foreach (DataColumn column in dtPeople.Columns)
                    column.AllowDBNull = true;

                DataRow dr = dtPeople.NewRow();
                dr["PersonID"] = 0;
                dr["USER"] = "Select a person ...";
                dtPeople.Rows.InsertAt(dr, 0);

                cbPerson.DataSource = dtPeople;
                cbPerson.DisplayMember = "USER";
                cbPerson.ValueMember = "PersonID";

                cbPerson.SelectedIndex = 0;
            }
        }

        public void _LoadUpdateMode(int UserID)
        {
            _Mode = enMode.Update;
            lblHeaderTitle.Text = "Edit User";
            btnSave.Text = "Save Changes";

            _FillPeopleComboBox();

            _User = clsUsers.Find(UserID);

            if (_User == null)
            {
                MessageBox.Show("Could not find person with ID = " + _UserID, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            cbPerson.SelectedValue = Convert.ToInt32(_User.PersonID);
            cbPerson.Enabled = false;
            txtUserName.Text = _User.UserName;
            lblPassword.Visible = false;
            txtPassword.Visible = false;
            lblChangePassword.Visible = true;
            txtPassword.Text = _User.Password;
            tsIsActive.Checked = _User.isActive;
            int calculatedPermissions = _User.Permissions;

            if (calculatedPermissions == -1)
            {
                cbSuperUser.Checked = true;
            }
            else if (calculatedPermissions == 1023) cbFullPermissions.Checked = true;
            else
            {
                if ((calculatedPermissions & (int)enPermissions.ManagePeople) != 0) cbManagePeople.Checked = true;
                else cbManagePeople.Checked = false;

                if ((calculatedPermissions & (int)enPermissions.ManageUsers) != 0) cbManageUsers.Checked = true;
                else cbManageUsers.Checked = false;

                if ((calculatedPermissions & (int)enPermissions.ManageDrivers) != 0) cbManageDrivers.Checked = true;
                else cbManageDrivers.Checked = false;

                if ((calculatedPermissions & (int)enPermissions.ViewApplications) != 0) cbViewApplications.Checked = true;
                else cbViewApplications.Checked = false;

                if ((calculatedPermissions & (int)enPermissions.ProcessApplications) != 0) cbProcessApplications.Checked = true;
                else cbProcessApplications.Checked = false;

                if ((calculatedPermissions & (int)enPermissions.ManageApplicationTests) != 0) cbManageApplicationTests.Checked = true;
                else cbManageApplicationTests.Checked = false;

                if ((calculatedPermissions & (int)enPermissions.EnterTestResults) != 0) cbEnterTestResults.Checked = true;
                else cbEnterTestResults.Checked = false;

                if ((calculatedPermissions & (int)enPermissions.ManageDetainedLicenses) != 0) cbManageDetainedLicenses.Checked = true;
                else cbManageDetainedLicenses.Checked = false;

                if ((calculatedPermissions & (int)enPermissions.ManageSettingsFees) != 0) cbManageSettings_Fees.Checked = true;
                else cbManageSettings_Fees.Checked = false;

                if ((calculatedPermissions & (int)enPermissions.AuditLogs) != 0) cbAudit_System_Logs.Checked = true;
                else cbAudit_System_Logs.Checked = false;
            }
        }

        public void LoadUserData(int UserID)
        {
            _UserID = UserID;

            if (_UserID == -1)
            {
                _Mode = enMode.AddNew;
            }
            else
            {
                _Mode = enMode.Update;
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

        public enum enPermissions
        {
            SuperUser = -1,           // يمثل الـ Global Access (Super User)
            None = 0,                // لا يملك أي صلاحية
            ManagePeople = 1,
            ManageUsers = 2,
            ManageDrivers = 4,
            ViewApplications = 8,
            ProcessApplications = 16,
            ManageApplicationTests = 32,
            EnterTestResults = 64,
            ManageDetainedLicenses = 128,
            ManageSettingsFees = 256,
            AuditLogs = 512,

            FullPermissions = 1023    // يمثل الـ Global Access (Full Permissions)
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
            else if (DVLD_BusinessLayer.clsUsers.IsUserExistForPersonID(selectedPersonID) && _Mode != enMode.Update)
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
            else if (DVLD_BusinessLayer.clsUsers.IsUserNameExistForPersonID(txtUserName.Text) && _Mode != enMode.Update)
            {
                errorProvider1.SetError(txtUserName, "This username is already a used in the system!");
                txtUserName.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(txtPassword.Text)) { errorProvider1.SetError(txtPassword, "Password is required."); txtPassword.Focus(); return; }

            int calculatedPermissions = 0;
            if (cbSuperUser.Checked)
            {
                calculatedPermissions = (int)enPermissions.SuperUser;
            }
            else if (cbFullPermissions.Checked) calculatedPermissions += (int)enPermissions.FullPermissions;
            else
            {
                if (cbManagePeople.Checked) calculatedPermissions += (int)enPermissions.ManagePeople;
                if (cbManageUsers.Checked) calculatedPermissions += (int)enPermissions.ManageUsers;
                if (cbManageDrivers.Checked) calculatedPermissions += (int)enPermissions.ManageDrivers;
                if (cbViewApplications.Checked) calculatedPermissions += (int)enPermissions.ViewApplications;
                if (cbProcessApplications.Checked) calculatedPermissions += (int)enPermissions.ProcessApplications;
                if (cbManageApplicationTests.Checked) calculatedPermissions += (int)enPermissions.ManageApplicationTests;
                if (cbEnterTestResults.Checked) calculatedPermissions += (int)enPermissions.EnterTestResults;
                if (cbManageDetainedLicenses.Checked) calculatedPermissions += (int)enPermissions.ManageDetainedLicenses;
                if (cbManageSettings_Fees.Checked) calculatedPermissions += (int)enPermissions.ManageSettingsFees;
                if (cbAudit_System_Logs.Checked) calculatedPermissions += (int)enPermissions.AuditLogs;
            }

            if (_Mode == enMode.AddNew)
                _User = new clsUsers();
            _User.UserID = _UserID;
            _User.PersonID = (int)cbPerson.SelectedValue;
            _User.UserName = txtUserName.Text;
            string hashedPassword = HashHelper.ComputeSHA256(txtPassword.Text.Trim());
            _User.Password = hashedPassword;
            _User.Permissions = calculatedPermissions;
            _User.isActive = tsIsActive.Checked;

            switch (_User.Save())
            {
                case clsUsers.enSaveResult.SavedSuccessfully:
                    MessageBox.Show($"Person saved successfully with ID = {_User.PersonID}!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    _Mode = enMode.Update; // تغيير الوضع إلى تعديل بعد النجاح الفوري
                    _UserID = _User.PersonID;

                    if (clsCurrentUser.CurrentUser != null && clsCurrentUser.CurrentUser.UserID == _User.UserID)
                    {
                        clsCurrentUser.CurrentUser = _User; // أو تحديث الحقول يلي تغيرت فيه
                        clsCurrentUser.RaiseUserDataChanged(); // <-- هون الأهم
                    }

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
        private void SetAllIndividualCheckBoxes(bool isCheecked)
        {
            cbManagePeople.Checked = isCheecked;
            cbManageUsers.Checked = isCheecked;
            cbManageDrivers.Checked = isCheecked;
            cbViewApplications.Checked = isCheecked;
            cbProcessApplications.Checked = isCheecked;
            cbManageApplicationTests.Checked = isCheecked;
            cbEnterTestResults.Checked = isCheecked;
            cbManageDetainedLicenses.Checked = isCheecked;
            cbManageSettings_Fees.Checked = isCheecked;
            cbAudit_System_Logs.Checked = isCheecked;
        }

        private void SetAllIndividualCheckBoxesEnabled(bool isEnable)
        {
            cbManagePeople.Enabled = isEnable;
            cbManageUsers.Enabled = isEnable;
            cbManageDrivers.Enabled = isEnable;
            cbViewApplications.Enabled = isEnable;
            cbProcessApplications.Enabled = isEnable;
            cbManageApplicationTests.Enabled = isEnable;
            cbEnterTestResults.Enabled = isEnable;
            cbManageDetainedLicenses.Enabled = isEnable;
            cbManageSettings_Fees.Enabled = isEnable;
            cbAudit_System_Logs.Enabled = isEnable;
        }
        private void cbSuperUser_CheckedChanged(object sender, EventArgs e)
        {
            if (cbSuperUser.Checked)
            {
                SetAllIndividualCheckBoxes(true);
                SetAllIndividualCheckBoxesEnabled(false);
                cbFullPermissions.Enabled = false;

            }

            else
            {
                SetAllIndividualCheckBoxes(false);
                SetAllIndividualCheckBoxesEnabled(true);
                cbFullPermissions.Enabled = true;

            }

        }

        private void cbFullPermissions_CheckedChanged(object sender, EventArgs e)
        {
            if (cbFullPermissions.Checked)
            {
                SetAllIndividualCheckBoxes(true);
                SetAllIndividualCheckBoxesEnabled(false);
                cbSuperUser.Enabled = false;
            }

            else
            {
                SetAllIndividualCheckBoxes(false);
                SetAllIndividualCheckBoxesEnabled(true);
                cbSuperUser.Enabled = true;
            }
        }

        private void lblChangePassword_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
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

                    ucChangePassword  myChangePassword = new ucChangePassword(_User);
                    frmContainer.Size = myChangePassword.Size;
                    myChangePassword.Dock = DockStyle.Fill;
                    frmContainer.Controls.Add(myChangePassword);

                    Guna.UI2.WinForms.Guna2Elipse elipse = new Guna.UI2.WinForms.Guna2Elipse();
                    elipse.TargetControl = frmContainer;
                    elipse.BorderRadius = 16;
                    frmContainer.ShowDialog(overlay);
                }
            }
        }

    }
}
