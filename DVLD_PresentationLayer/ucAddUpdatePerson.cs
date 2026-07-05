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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace DVLD_PresentationLayer
{
    public partial class ucAddUpdatePerson : UserControl
    {
        public delegate void DataBackEventHandler(object sender, int PersonID);
        public event DataBackEventHandler DataBack;
        public enum enMode { AddNew = 0, Update = 1 };
        private enMode _Mode = enMode.AddNew;
        private int _PersonID = -1;
        private ErrorProvider errorProvider1 = new ErrorProvider();

        // 🌟 الـ PictureBox البرمجي الذي سيتم زرعه داخل الـ ComboBox لعرض العلم
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
                _LoadAddNewMode();
            }
            else
            {
                _Mode = enMode.Update;
                _LoadUpdateMode();
            }
        }

        private void _LoadAddNewMode()
        {
            lblHeaderTitle.Text = "Add New Person";
            btnSave.Text = "Add Person";

            txtFirstName.Text = "";
            txtLastName.Text = "";
            dtpDateOfBirth.Value = DateTime.Today.AddYears(-18);
        }

        private void _LoadUpdateMode()
        {
            lblHeaderTitle.Text = "Update Person Info";
            btnSave.Text = "Update Person";
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
            DateTime maxAllowedDate = DateTime.Today.AddYears(-18);
            dtpDateOfBirth.Value = maxAllowedDate;
            dtpDateOfBirth.MaxDate = maxAllowedDate.Date.AddDays(1).AddSeconds(-1);
            dtpDateOfBirth.Checked = false;

            dtpDateOfBirth.FillColor = Color.FromArgb(248, 250, 252);
            dtpDateOfBirth.BorderColor = Color.FromArgb(213, 218, 223);
            dtpDateOfBirth.HoverState.BorderColor = Color.FromArgb(94, 148, 255);
            dtpDateOfBirth.HoverState.FillColor = Color.FromArgb(248, 250, 252);

            // 1. شحن كاش الـ CSV احتياطاً للأعلام المسماة بالرموز
            LoadCountryCodesCache();

            // 2. إعداد الـ Guna2ComboBox برمجياً لترك مساحة واسعة للعلم على اليسار
            cbNationality.DrawMode = DrawMode.Normal;
            cbNationality.TextOffset = new Point(35, 0);

            // 3. إنشاء الـ PictureBox المخصص للعلم برمجياً وتحديد أبعاده ومكانه
            _pbComboFlag = new PictureBox();
            _pbComboFlag.Size = new Size(24, 16);
            _pbComboFlag.SizeMode = PictureBoxSizeMode.Zoom;
            _pbComboFlag.BackColor = Color.Transparent;

            // وضعه في أقصى اليسار بالمنتصف عمودياً داخل الكومبوبوكس
            _pbComboFlag.Location = new Point(10, (cbNationality.Height - 24) / 2);

            // زرع الـ PictureBox داخل عناصر تحكم الكومبوبوكس نفسه
            cbNationality.Controls.Add(_pbComboFlag);

            // 4. ربط حدث تغير الاختيار لتحديث العلم فوراً
            cbNationality.SelectedIndexChanged += new EventHandler(cbNationality_SelectedIndexChanged);

            // 🌟 5. ضبط إعدادات الـ Dropdown (تم تقليل العناصر لـ 4 لضمان ملاءمة المساحة السفلية)
            cbNationality.IntegralHeight = false; // جرب تحويلها إلى false هنا مع الحجم الثابت
            cbNationality.ItemHeight = 22;        // تقليص الارتفاع قليلاً ليتناسب مع المساحة
            cbNationality.MaxDropDownItems = 8;   // 4 عناصر كافية جداً لمنع القائمة من القفز للأعلى
            cbNationality.DropDownHeight = 8 * 22; // إجبار الحجم الكلي يدوياً

            // 6. جلب الدول من قاعدة البيانات وربطها بالـ ComboBox
            DataTable dtCountries = clsCountries.GetAllCountries();

            // تأمين فصل الـ DataSource أولاً
            cbNationality.DataSource = null;
            cbNationality.DisplayMember = "CountryName";
            cbNationality.ValueMember = "CountryID";

            // ربط البيانات النهائي
            cbNationality.DataSource = dtCountries;

            if (cbNationality.Items.Count > 0)
                cbNationality.SelectedIndex = 0;
        }

        private void cbNationality_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbNationality.SelectedItem == null || _pbComboFlag == null) return;

            // جلب اسم الدولة الحالي بدقة
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

            // 🌟 الخطة A: البحث عن العلم باسم الدولة كاملاً (مثل: Flags/Tunisia.png)
            string pathByName = Path.Combine(Application.StartupPath, "Flags", $"{selectedCountry}.png");

            string altPathByName = "";
            if (Directory.GetParent(Application.StartupPath)?.Parent != null)
            {
                altPathByName = Path.Combine(Directory.GetParent(Application.StartupPath).Parent.FullName, "Flags", $"{selectedCountry}.png");
            }

            if (File.Exists(pathByName))
            {
                flagPath = pathByName;
            }
            else if (File.Exists(altPathByName))
            {
                flagPath = altPathByName;
            }
            else
            {
                // 🌟 الخطة B: إذا لم يجد الاسم، يبحث بالرمز الثنائي عبر كاش الـ CSV (مثل: Flags/tn.png)
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

            // 🌟 شحن الصورة داخل الـ PictureBox المزروع برمجياً
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

            if (string.IsNullOrWhiteSpace(txtFirstName.Text))
            {
                errorProvider1.SetError(txtFirstName, "First name is required.");
                txtFirstName.Focus();
                MessageBox.Show("Please fill in all required fields properly before saving.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtLastName.Text))
            {
                errorProvider1.SetError(txtLastName, "Last name is required.");
                txtLastName.Focus();
                MessageBox.Show("Please fill in all required fields properly before saving.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtNationalID.Text))
            {
                errorProvider1.SetError(txtNationalID, "National ID number is required.");
                txtNationalID.Focus();
                MessageBox.Show("Please fill in all required fields properly before saving.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtPhone.Text))
            {
                errorProvider1.SetError(txtPhone, "Phone number is required.");
                txtPhone.Focus();
                MessageBox.Show("Please fill in all required fields properly before saving.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                errorProvider1.SetError(txtEmail, "Email address is required.");
                txtEmail.Focus();
                MessageBox.Show("Please fill in all required fields properly before saving.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cbNationality.SelectedIndex == -1)
            {
                errorProvider1.SetError(cbNationality, "Nationality is required.");
                cbNationality.Focus();
                MessageBox.Show("Please fill in all required fields properly before saving.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtAddress.Text))
            {
                errorProvider1.SetError(txtAddress, "Address is required.");
                txtAddress.Focus();
                MessageBox.Show("Please fill in all required fields properly before saving.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (_Mode == enMode.AddNew && picImage.Image == null)
            {
                errorProvider1.SetError(picImage, "A personal profile picture is required for new registrations.");
                MessageBox.Show("Please upload a personal photo before saving.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!_HandlePersonImage())
            {
                return;
            }

            clsPerson Person = new clsPerson();

            Person.NationalNo = txtNationalID.Text.Trim();
            Person.FirstName = txtFirstName.Text.Trim();
            Person.SecondName = txtSecondName.Text.Trim();
            Person.ThirdName = txtThirdName.Text.Trim();
            Person.LastName = txtLastName.Text.Trim();
            Person.DateOfBirth = dtpDateOfBirth.Value;
            Person.Gendor = Convert.ToByte(cbGendor.SelectedIndex);
            Person.Address = txtAddress.Text.Trim();
            Person.Phone = txtPhone.Text.Trim();
            Person.Email = txtEmail.Text.Trim();
            Person.NationalCountryID = Convert.ToInt32(cbNationality.SelectedValue);
            Person.ImagePath = _SelectedImagePath;
            if (Person.Save())
            {
                MessageBox.Show($"Person saved successfully with ID = {Person.PersonID}!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                DataBack?.Invoke(this, Person.PersonID);
                this.ParentForm.Close();
            }
            else
            {
                MessageBox.Show("Failed to save person data.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool _HandlePersonImage()
        {
            if (string.IsNullOrEmpty(_SelectedImagePath))
                return true;

            try
            {
                string targetFolder = Path.Combine(Application.StartupPath, "Person_Images");

                if (!Directory.Exists(targetFolder))
                {
                    Directory.CreateDirectory(targetFolder);
                }

                string extension = Path.GetExtension(_SelectedImagePath);
                string newFileName = Guid.NewGuid().ToString() + extension;
                string destinationPath = Path.Combine(targetFolder, newFileName);

                File.Copy(_SelectedImagePath, destinationPath, true);

                _SelectedImagePath = destinationPath; // شحن المسار الجديد للـ Database
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
                picImage.Image = Image.FromFile(openFileDialog.FileName);
                _SelectedImagePath = openFileDialog.FileName;
                linkLblImage.Text = "Update Image";
            }
        }

        private void txtNationalID_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNationalID.Text))
            {
                errorProvider1.SetError(txtNationalID, "National ID is required!");
                return;
            }

            // التحقق في حالة الإضافة الجديدة فقط
            if (_Mode == enMode.AddNew && clsPerson.IsPersonExist(txtNationalID.Text.Trim()))
            {
                e.Cancel = true; // منع الانتقال للحقل التالي
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