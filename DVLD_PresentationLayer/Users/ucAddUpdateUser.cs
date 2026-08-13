using DVLD_BusinessLayer;
using DVLD_PresentationLayer.Global;
using DVLD_Security;
using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace DVLD_PresentationLayer.Users
{
    public partial class ucAddUpdateUser : UserControl
    {
        enum enMode { AddNew, Update }
        private enMode _Mode = enMode.AddNew;
        clsUsers _User;
        private int _UserID = -1;

        public ucAddUpdateUser()
        {
            InitializeComponent();
        }

        public delegate void DataBackEventHandler(object sender, int UserID);
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

                    string fullName = string.Join(" ", new[] { firstName, secondName, thirdName, lastName }.Where(s => !string.IsNullOrWhiteSpace(s)));
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
            _UserID = UserID; // تصحيح: حفظ الـ UserID

            lblHeaderTitle.Text = "Edit User";
            btnSave.Text = "Save Changes";

            _FillPeopleComboBox();

            _User = clsUsers.Find(_UserID);

            if (_User == null)
            {
                MessageBox.Show("Could not find user with ID = " + _UserID, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            cbPerson.SelectedValue = Convert.ToInt32(_User.PersonID);
            cbPerson.Enabled = false;
            txtUserName.Text = _User.UserName;

            // إخفاء خانات كلمة المرور وإظهار رابط التغيير
            lblPassword.Visible = false;
            txtPassword.Visible = false;
            lblChangePassword.Visible = true;

            tsIsActive.Checked = _User.isActive;
            int calculatedPermissions = _User.Permissions;

            if (calculatedPermissions == (int)clsUsers.enPermissions.SuperUser)
            {
                cbSuperUser.Checked = true;
            }
            else if (calculatedPermissions == (int)clsUsers.enPermissions.FullPermissions)
            {
                cbFullPermissions.Checked = true;
            }
            else
            {
                cbManagePeople.Checked = (calculatedPermissions & (int)clsUsers.enPermissions.ManagePeople) != 0;
                cbManageUsers.Checked = (calculatedPermissions & (int)clsUsers.enPermissions.ManageUsers) != 0;
                cbManageApplications.Checked = (calculatedPermissions & (int)clsUsers.enPermissions.ManageApplications) != 0;
                cbNewApplications.Checked = (calculatedPermissions & (int)clsUsers.enPermissions.NewApplications) != 0;
                cbManageApplicationTypes.Checked = (calculatedPermissions & (int)clsUsers.enPermissions.ManageApplicationTypes) != 0;
                cbManageTestTypes.Checked = (calculatedPermissions & (int)clsUsers.enPermissions.ManageTestTypes) != 0;
                cbManageLicenseClasses.Checked = (calculatedPermissions & (int)clsUsers.enPermissions.ManageLicenseClasses) != 0;
                cbManageDetainedLicenses.Checked = (calculatedPermissions & (int)clsUsers.enPermissions.ManageDetainedLicenses) != 0;
                cbManageInternationalApp.Checked = (calculatedPermissions & (int)clsUsers.enPermissions.ManageInternationalApp) != 0;
                cbAudit_System_Logs.Checked = (calculatedPermissions & (int)clsUsers.enPermissions.AuditLogs) != 0;
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
                _LoadUpdateMode(_UserID);
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

            if (selectedPersonID == 0)
            {
                errorProvider1.SetError(cbPerson, "Please select a valid person.");
                cbPerson.Focus();
                return;
            }
            else if (DVLD_BusinessLayer.clsUsers.IsUserExistForPersonID(selectedPersonID) && _Mode != enMode.Update)
            {
                errorProvider1.SetError(cbPerson, "This person is already a user in the system!");
                cbPerson.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtUserName.Text))
            {
                errorProvider1.SetError(txtUserName, "User name is required.");
                txtUserName.Focus();
                return;
            }

            // فحص كلمة المرور فقط عند إضافة مستخدم جديد
            if (_Mode == enMode.AddNew && string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                errorProvider1.SetError(txtPassword, "Password is required.");
                txtPassword.Focus();
                return;
            }

            int calculatedPermissions = 0;
            if (cbSuperUser.Checked)
            {
                calculatedPermissions = (int)clsUsers.enPermissions.SuperUser;
            }
            else if (cbFullPermissions.Checked)
            {
                calculatedPermissions = (int)clsUsers.enPermissions.FullPermissions;
            }
            else
            {
                if (cbManagePeople.Checked) calculatedPermissions += (int)clsUsers.enPermissions.ManagePeople;
                if (cbManageUsers.Checked) calculatedPermissions += (int)clsUsers.enPermissions.ManageUsers;
                if (cbManageApplications.Checked) calculatedPermissions += (int)clsUsers.enPermissions.ManageApplications;
                if (cbNewApplications.Checked) calculatedPermissions += (int)clsUsers.enPermissions.NewApplications;
                if (cbManageApplicationTypes.Checked) calculatedPermissions += (int)clsUsers.enPermissions.ManageApplicationTypes;
                if (cbManageTestTypes.Checked) calculatedPermissions += (int)clsUsers.enPermissions.ManageTestTypes;
                if (cbManageLicenseClasses.Checked) calculatedPermissions += (int)clsUsers.enPermissions.ManageLicenseClasses;
                if (cbManageDetainedLicenses.Checked) calculatedPermissions += (int)clsUsers.enPermissions.ManageDetainedLicenses;
                if (cbManageInternationalApp.Checked) calculatedPermissions += (int)clsUsers.enPermissions.ManageInternationalApp;
                if (cbAudit_System_Logs.Checked) calculatedPermissions += (int)clsUsers.enPermissions.AuditLogs;
            }

            if (_Mode == enMode.AddNew)
            {
                _User = new clsUsers();
                _User.Password = HashHelper.ComputeSHA256(txtPassword.Text.Trim());
            }

            _User.UserID = _UserID;
            _User.PersonID = selectedPersonID;
            _User.UserName = txtUserName.Text.Trim();
            _User.Permissions = calculatedPermissions;
            _User.isActive = tsIsActive.Checked;

            switch (_User.Save())
            {
                case clsUsers.enSaveResult.SavedSuccessfully:
                    MessageBox.Show($"User saved successfully with ID = {_User.UserID}!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    _Mode = enMode.Update;
                    _UserID = _User.UserID; // تصحيح: ربط الـ UserID

                    if (clsCurrentUser.CurrentUser != null && clsCurrentUser.CurrentUser.UserID == _User.UserID)
                    {
                        clsCurrentUser.CurrentUser = _User;
                        clsCurrentUser.RaiseUserDataChanged();
                    }

                    DataBack?.Invoke(this, _User.UserID); // تصحيح: إرجاع الـ UserID
                    this.FindForm()?.Close();
                    break;

                case clsUsers.enSaveResult.NoChanges:
                    MessageBox.Show("Nothing was changed.");
                    break;

                case clsUsers.enSaveResult.Failed:
                    MessageBox.Show("Failed to save user data.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
            }
        }

        private void SetAllIndividualCheckBoxes(bool isChecked)
        {
            cbManagePeople.Checked = isChecked;
            cbManageUsers.Checked = isChecked;
            cbManageInternationalApp.Checked = isChecked;
            cbManageApplications.Checked = isChecked;
            cbManageLicenseClasses.Checked = isChecked;
            cbManageTestTypes.Checked = isChecked;
            cbNewApplications.Checked = isChecked;
            cbManageDetainedLicenses.Checked = isChecked;
            cbManageApplicationTypes.Checked = isChecked;
            cbAudit_System_Logs.Checked = isChecked;
        }

        private void SetAllIndividualCheckBoxesEnabled(bool isEnable)
        {
            cbManagePeople.Enabled = isEnable;
            cbManageUsers.Enabled = isEnable;
            cbManageInternationalApp.Enabled = isEnable;
            cbManageApplications.Enabled = isEnable;
            cbManageLicenseClasses.Enabled = isEnable;
            cbManageTestTypes.Enabled = isEnable;
            cbNewApplications.Enabled = isEnable;
            cbManageDetainedLicenses.Enabled = isEnable;
            cbManageApplicationTypes.Enabled = isEnable;
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

                    ucChangePassword myChangePassword = new ucChangePassword(_User);
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