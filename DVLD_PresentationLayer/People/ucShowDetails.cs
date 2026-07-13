using DVLD_BusinessLayer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DVLD_PresentationLayer
{
    public partial class ucShowDetails : UserControl
    {
        private clsPerson _Person;
        private Dictionary<string, string> _countryCodesCache = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public ucShowDetails()
        {
            InitializeComponent();
            // تحميل الكاش الخاص بأكواد الدول بمجرد إنشاء الواجهة
            LoadCountryCodesCache();
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            this.FindForm()?.Close();
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

        // دالة مساعدة عامة ومستقلة لجلب مسار العلم بناءً على اسم الدولة لتكرار استخدامها بسهولة
        private string _GetFlagPath(string countryName)
        {
            string flagPath = "";

            // 1. البحث باستخدام الاسم الكامل للدولة
            string pathByName = Path.Combine(Application.StartupPath, "Flags", $"{countryName}.png");
            string altPathByName = Directory.GetParent(Application.StartupPath)?.Parent != null
                ? Path.Combine(Directory.GetParent(Application.StartupPath).Parent.FullName, "Flags", $"{countryName}.png")
                : "";

            if (File.Exists(pathByName)) return pathByName;
            if (!string.IsNullOrEmpty(altPathByName) && File.Exists(altPathByName)) return altPathByName;

            // 2. البحث باستخدام كود الدولة الثنائي (ISO Code) من الـ Cache إذا لم ينجح البحث بالاسم
            if (_countryCodesCache.TryGetValue(countryName, out string countryCode))
            {
                string pathByCode = Path.Combine(Application.StartupPath, "Flags", $"{countryCode}.png");
                string altPathByCode = Directory.GetParent(Application.StartupPath)?.Parent != null
                    ? Path.Combine(Directory.GetParent(Application.StartupPath).Parent.FullName, "Flags", $"{countryCode}.png")
                    : "";

                if (File.Exists(pathByCode)) return pathByCode;
                if (!string.IsNullOrEmpty(altPathByCode) && File.Exists(altPathByCode)) return altPathByCode;
            }

            return flagPath;
        }

        // دالة مساعدة لعرض صورة العلم داخل الـ PictureBox المحدد بطريقة الـ Stream الآمنة
        private void _DisplayFlag(string countryName, PictureBox pbTarget)
        {
            if (pbTarget == null) return;

            string flagPath = _GetFlagPath(countryName);

            if (!string.IsNullOrEmpty(flagPath) && File.Exists(flagPath))
            {
                try
                {
                    using (var stream = new FileStream(flagPath, FileMode.Open, FileAccess.Read))
                    {
                        pbTarget.Image = Image.FromStream(stream);
                    }
                }
                catch
                {
                    pbTarget.Image = null;
                }
            }
            else
            {
                pbTarget.Image = null;
            }
        }

        public void LoadPersonData(int PersonID)
        {
            _Person = DVLD_BusinessLayer.clsPerson.Find(PersonID);

            if (_Person == null)
            {
                MessageBox.Show("Could not find person with ID = " + PersonID, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 1. دمج الاسم الكامل بشكل منسق مع مسافات
            lblName.Text = $"{_Person.FirstName} {_Person.SecondName} {_Person.ThirdName} {_Person.LastName}".Replace("  ", " ");

            // 2. إسناد البيانات للنصوص
            lblPersonID.Text = "Person ID: " + _Person.PersonID.ToString();
            lblNationalID.Text = _Person.NationalNo;
            lblPhone.Text = _Person.Phone;
            lblEmail.Text = _Person.Email;
            lblAddress.Text = _Person.Address;

            // 3. تنسيق التاريخ ليظهر بشكل جميل (يوم/شهر/سنة)
            lblDoB.Text = _Person.DateOfBirth.ToString("MMM dd, yyyy", System.Globalization.CultureInfo.InvariantCulture);

            // 4. عرض الجنس بدلاً من الأرقام
            lblGendor.Text = (_Person.Gendor == 0) ? "Male" : "Female";

            // 5. جلب اسم الجنسية/الدولة وعرض علمها تلقائياً من الـ GitHub Folder
            DataTable dtCountries = clsCountries.GetAllCountries();
            DataRow[] foundRows = dtCountries.Select($"CountryID = {_Person.NationalCountryID}");

            if (foundRows.Length > 0)
            {
                string countryName = foundRows[0]["CountryName"].ToString().Trim();
                lblNationality.Text = countryName;

                // 🌟 استدعاء دالة عرض العلم وتمرير أداة الـ PictureBox المجاورة للـ Label الخاص بالدولة
                _DisplayFlag(countryName, pbFlagCountry);
            }
            else
            {
                lblNationality.Text = "Unknown";
                if (pbFlagCountry != null) pbFlagCountry.Image = null;
            }

            // 6. معالجة الصورة الشخصية بطريقة الـ Stream الآمنة لتفادي قفل الملفات
            if (!string.IsNullOrEmpty(_Person.ImagePath) && File.Exists(_Person.ImagePath))
            {
                try
                {
                    using (var stream = new FileStream(_Person.ImagePath, FileMode.Open, FileAccess.Read))
                    {
                        pbImage.Image = Image.FromStream(stream);
                    }
                }
                catch
                {
                    LoadDefaultAvatar();
                }
            }
            else
            {
                LoadDefaultAvatar();
            }
        }

        // دالة وضع الصورة الافتراضية المفعلة الآن بالكامل بناءً على الجنس
        private void LoadDefaultAvatar()
        {
           /* if (_Person != null)
            {
                if (_Person.Gendor == 0)
                    pbImage.Image = Properties.Resources.default_male_avatar; // تأكد من مطابقة الاسم في الـ Resources لديك
                else
                    pbImage.Image = Properties.Resources.default_female_avatar;
            }*/
        }

    }
}