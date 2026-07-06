using DVLD_BusinessLayer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static DVLD_BusinessLayer.clsPerson;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace DVLD_PresentationLayer
{
    public partial class ucAddUpdatePerson : UserControl
    {
        private clsPerson _Person;
        public delegate void DataBackEventHandler(object sender, int PersonID);
        public event DataBackEventHandler DataBack;
        public enum enMode { AddNew = 0, Update = 1 };
        private enMode _Mode = enMode.AddNew;
        private int _PersonID = -1;
        private ErrorProvider errorProvider1 = new ErrorProvider();

        // الـ PictureBox البرمجي الذي سيتم زرعه داخل الـ ComboBox لعرض العلم
        private PictureBox _pbComboFlag;

        // كاش للرموز لضمان سرعة تحويل اسم الدولة إلى رمز ثنائي عند الحاجة
        private Dictionary<string, string> _countryCodesCache = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public ucAddUpdatePerson()
        {
            InitializeComponent();
        }

        private void LoadCountryCodesCache()
        {
            if (_countryCodesCache.Count > 0) return;
            try
            {
                string csvPath = Path.Combine(Application.StartupPath, "countries.csv");
                if (File.Exists(csvPath))
                {
                    var lines = File.ReadAllLines(csvPath);
                    for (int i = 1; i < lines.Length; i++)
                    {
                        string line = lines[i].Trim();
                        if (string.IsNullOrEmpty(line)) continue;
                        var columns = System.Text.RegularExpressions.Regex.Split(line, ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
                        if (columns.Length >= 4)
                        {
                            string name = columns[3].Replace("\"", "").Trim();
                            string code = columns[1].Replace("\"", "").Trim().ToLower();
                            if (!_countryCodesCache.ContainsKey(name)) _countryCodesCache.Add(name, code);
                        }
                    }
                }
            }
            catch { }
        }

        public void LoadPersonData(int PersonID)
        {
            _PersonID = PersonID;

            if (_PersonID == -1)
            {
                _Mode = enMode.AddNew;
            }
            else
            {
                _Mode = enMode.Update;
            }
        }

        private void _LoadAddNewMode()
        {
            lblHeaderTitle.Text = "Add New Person";
            btnSave.Text = "Add Person";
            _Person = new clsPerson(); // إنشاء كائن جديد فارغ لوضع الإضافة

            txtFirstName.Text = "";
            txtLastName.Text = "";
            dtpDateOfBirth.Value = DateTime.Today.AddYears(-18);
        }

        private void _LoadUpdateMode()
        {
            lblHeaderTitle.Text = "Update Person Info";
            btnSave.Text = "Update Person";

            // استدعاء دالة Find الاستاتيكية بنجاح
            _Person = DVLD_BusinessLayer.clsPerson.Find(_PersonID);

            if (_Person == null)
            {
                MessageBox.Show("Could not find person with ID = " + _PersonID, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // ضخ البيانات داخل عناصر التحكم بالواجهة
            txtFirstName.Text = _Person.FirstName;
            txtSecondName.Text = _Person.SecondName;
            txtThirdName.Text = _Person.ThirdName;
            txtLastName.Text = _Person.LastName;
            txtNationalID.Text = _Person.NationalNo;
            txtNationalID.ReadOnly = true;
            txtNationalID.ForeColor = Color.Gray;
            txtPhone.Text = _Person.Phone;
            txtEmail.Text = _Person.Email;
            txtAddress.Text = _Person.Address;
            dtpDateOfBirth.Value = _Person.DateOfBirth;
            cbGendor.SelectedIndex = _Person.Gendor;
            // تحديد الدولة الصحيحة بناءً على الـ ID الخاص بها
            cbNationality.SelectedValue = Convert.ToInt32(_Person.NationalCountryID);
            // معالجة الصورة الشخصية عند التحميل
            _SelectedImagePath = _Person.ImagePath;

            if (!string.IsNullOrEmpty(_Person.ImagePath) && File.Exists(_Person.ImagePath))
            {
                try
                {
                    using (var stream = new FileStream(_Person.ImagePath, FileMode.Open, FileAccess.Read))
                    {
                        picImage.Image = Image.FromStream(stream);
                    }
                    linkLblImage.Text = "Update Image";
                }
                catch
                {
                    picImage.Image = null;
                    linkLblImage.Text = "Set Image";
                }
            }
            else
            {
                picImage.Image = null;
                linkLblImage.Text = "Set Image";
            }
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            this.FindForm()?.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.FindForm()?.Close();
        }

        private void ucAddUpdatePerson_Load(object sender, EventArgs e)
        {
            if (_Mode == enMode.AddNew)
            {
                DateTime maxAllowedDate = DateTime.Today.AddYears(-18);
                dtpDateOfBirth.Value = maxAllowedDate;
                dtpDateOfBirth.MaxDate = maxAllowedDate.Date.AddDays(1).AddSeconds(-1);
                dtpDateOfBirth.Checked = false;
            }
            

            dtpDateOfBirth.FillColor = Color.FromArgb(248, 250, 252);
            dtpDateOfBirth.BorderColor = Color.FromArgb(213, 218, 223);
            dtpDateOfBirth.HoverState.BorderColor = Color.FromArgb(94, 148, 255);
            dtpDateOfBirth.HoverState.FillColor = Color.FromArgb(248, 250, 252);

            LoadCountryCodesCache();

            cbNationality.DrawMode = DrawMode.Normal;
            cbNationality.TextOffset = new Point(35, 0);

            _pbComboFlag = new PictureBox();
            _pbComboFlag.Size = new Size(24, 16);
            _pbComboFlag.SizeMode = PictureBoxSizeMode.Zoom;
            _pbComboFlag.BackColor = Color.Transparent;
            _pbComboFlag.Location = new Point(10, (cbNationality.Height - 24) / 2);
            cbNationality.Controls.Add(_pbComboFlag);

            cbNationality.SelectedIndexChanged += new EventHandler(cbNationality_SelectedIndexChanged);

            cbNationality.IntegralHeight = false;
            cbNationality.ItemHeight = 22;
            cbNationality.MaxDropDownItems = 8;
            cbNationality.DropDownHeight = 8 * 22;

            DataTable dtCountries = clsCountries.GetAllCountries();

            cbNationality.DataSource = null;
            cbNationality.DisplayMember = "CountryName";
            cbNationality.ValueMember = "CountryID";
            cbNationality.DataSource = dtCountries;
            if (_Mode == enMode.Update)
                _LoadUpdateMode();
            else
                _LoadAddNewMode();

            if (cbNationality.Items.Count > 0 && _Mode == enMode.AddNew)
                cbNationality.SelectedIndex = 0;
        }

        private void cbNationality_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbNationality.SelectedItem == null || _pbComboFlag == null) return;

            string selectedCountry = "";
            if (cbNationality.SelectedItem is DataRowView rowView)
            {
                selectedCountry = rowView["CountryName"].ToString().Trim();
            }
            else
            {
                selectedCountry = cbNationality.SelectedItem.ToString().Trim();
            }

            string flagPath = "";
            string pathByName = Path.Combine(Application.StartupPath, "Flags", $"{selectedCountry}.png");
            string altPathByName = "";
            if (Directory.GetParent(Application.StartupPath)?.Parent != null)
            {
                altPathByName = Path.Combine(Directory.GetParent(Application.StartupPath).Parent.FullName, "Flags", $"{selectedCountry}.png");
            }

            if (File.Exists(pathByName)) flagPath = pathByName;
            else if (File.Exists(altPathByName)) flagPath = altPathByName;
            else
            {
                if (_countryCodesCache.TryGetValue(selectedCountry, out string countryCode))
                {
                    string pathByCode = Path.Combine(Application.StartupPath, "Flags", $"{countryCode}.png");
                    string altPathByCode = "";

                    if (Directory.GetParent(Application.StartupPath)?.Parent != null)
                    {
                        altPathByCode = Path.Combine(Directory.GetParent(Application.StartupPath).Parent.FullName, "Flags", $"{countryCode}.png");
                    }

                    if (File.Exists(pathByCode)) flagPath = pathByCode;
                    else if (File.Exists(altPathByCode)) flagPath = altPathByCode;
                }
            }

            if (!string.IsNullOrEmpty(flagPath) && File.Exists(flagPath))
            {
                try
                {
                    using (var stream = new FileStream(flagPath, FileMode.Open, FileAccess.Read))
                    {
                        _pbComboFlag.Image = Image.FromStream(stream);
                    }
                }
                catch
                {
                    _pbComboFlag.Image = null;
                }
            }
            else
            {
                _pbComboFlag.Image = null;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            errorProvider1.Clear();

            // التحقق من الحقول الإلزامية
            if (string.IsNullOrWhiteSpace(txtFirstName.Text)) { errorProvider1.SetError(txtFirstName, "First name is required."); txtFirstName.Focus(); return; }
            if (string.IsNullOrWhiteSpace(txtLastName.Text)) { errorProvider1.SetError(txtLastName, "Last name is required."); txtLastName.Focus(); return; }
            if (string.IsNullOrWhiteSpace(txtNationalID.Text)) { errorProvider1.SetError(txtNationalID, "National ID number is required."); txtNationalID.Focus(); return; }
            if (string.IsNullOrWhiteSpace(txtPhone.Text)) { errorProvider1.SetError(txtPhone, "Phone number is required."); txtPhone.Focus(); return; }
            if (string.IsNullOrWhiteSpace(txtEmail.Text)) { errorProvider1.SetError(txtEmail, "Email address is required."); txtEmail.Focus(); return; }
            if (cbNationality.SelectedIndex == -1) { errorProvider1.SetError(cbNationality, "Nationality is required."); cbNationality.Focus(); return; }
            if (string.IsNullOrWhiteSpace(txtAddress.Text)) { errorProvider1.SetError(txtAddress, "Address is required."); txtAddress.Focus(); return; }

            if (_Mode == enMode.AddNew && picImage.Image == null)
            {
                errorProvider1.SetError(picImage, "A personal profile picture is required for new registrations.");
                MessageBox.Show("Please upload a personal photo before saving.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // معالجة وحفظ الصورة أولاً
            if (!_HandlePersonImage()) return;

            // 🌟 الحل السحري: الحفاظ على نفس الكائن عند التعديل لمنع حوادث التكرار
            if (_Mode == enMode.AddNew)
                _Person = new clsPerson();

            _Person.NationalNo = txtNationalID.Text.Trim();
            _Person.FirstName = txtFirstName.Text.Trim();
            _Person.SecondName = txtSecondName.Text.Trim();
            _Person.ThirdName = txtThirdName.Text.Trim();
            _Person.LastName = txtLastName.Text.Trim();
            _Person.DateOfBirth = dtpDateOfBirth.Value;
            _Person.Gendor = Convert.ToByte(cbGendor.SelectedIndex);
            _Person.Address = txtAddress.Text.Trim();
            _Person.Phone = txtPhone.Text.Trim();
            _Person.Email = txtEmail.Text.Trim();
            _Person.NationalCountryID = Convert.ToInt32(cbNationality.SelectedValue);
            _Person.ImagePath = _SelectedImagePath;

            switch (_Person.Save())
            {
                case clsPerson.enSaveResult.SavedSuccessfully:
                    MessageBox.Show($"Person saved successfully with ID = {_Person.PersonID}!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    _Mode = enMode.Update; // تغيير الوضع إلى تعديل بعد النجاح الفوري
                    _PersonID = _Person.PersonID;

                    DataBack?.Invoke(this, _Person.PersonID);
                    this.FindForm()?.Close();
                    break;
                case enSaveResult.NoChanges:
                    MessageBox.Show("Nothing was changed");
                    break;
                case enSaveResult.Failed:
                    MessageBox.Show("Failed to save person data.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
            }
        }

        private bool _HandlePersonImage()
        {
            // إذا لم يتم تحديد صورة جديدة أو بقيت الصورة القديمة كما هي
            if (string.IsNullOrEmpty(_SelectedImagePath) || (_Mode == enMode.Update && _SelectedImagePath == _Person.ImagePath))
                return true;

            try
            {
                string targetFolder = Path.Combine(Application.StartupPath, "Person_Images");
                if (!Directory.Exists(targetFolder)) Directory.CreateDirectory(targetFolder);

                string extension = Path.GetExtension(_SelectedImagePath);
                string newFileName = Guid.NewGuid().ToString() + extension;
                string destinationPath = Path.Combine(targetFolder, newFileName);

                File.Copy(_SelectedImagePath, destinationPath, true);
                _SelectedImagePath = destinationPath;
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error processing image: {ex.Message}", "Image Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private string _SelectedImagePath = "";

        private void linkLblImage_LinkClicked_1(object sender, LinkLabelLinkClickedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp";
            openFileDialog.Title = "Select Contact Image";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // 🌟 الحل السحري: تحميل الصورة باستخدام Stream لمنع قفل الملف بنظام التشغيل
                using (var stream = new FileStream(openFileDialog.FileName, FileMode.Open, FileAccess.Read))
                {
                    picImage.Image = Image.FromStream(stream);
                }

                _SelectedImagePath = openFileDialog.FileName;
                linkLblImage.Text = "Update Image";
            }
        }

        private void txtNationalID_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNationalID.Text))
            {
                e.Cancel = true;
                errorProvider1.SetError(txtNationalID, "National ID is required!");
                return;
            }

            // 🌟 تحقق ذكي: يمنع التكرار في الإضافة، ويسمح للشخص بالاحتفاظ برقم قومي الخاص به في التعديل مع منع سرقة أرقام الآخرين
            if (clsPerson.IsPersonExist(txtNationalID.Text.Trim()) && (_Mode == enMode.AddNew || txtNationalID.Text.Trim() != _Person.NationalNo))
            {
                e.Cancel = true;
                errorProvider1.SetError(txtNationalID, "This National ID already exists in the system!");
                MessageBox.Show("This National ID is already assigned to another person.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                errorProvider1.SetError(txtNationalID, "");
            }
        }


    }
}