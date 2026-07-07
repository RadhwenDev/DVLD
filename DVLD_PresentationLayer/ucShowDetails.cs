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

        public ucShowDetails()
        {
            InitializeComponent();
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            this.FindForm()?.Close();
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
            lblPersonID.Text =  "Person ID: " + _Person.PersonID.ToString(); // تأكد من عرض الـ ID أيضاً في مكانه المخصص
            lblNationalID.Text = _Person.NationalNo;
            lblPhone.Text = _Person.Phone;
            lblEmail.Text = _Person.Email;
            lblAddress.Text = _Person.Address;

            // 3. تنسيق التاريخ ليظهر بشكل جميل (يوم/شهر/سنة) بدلاً من الوقت الطويل
            lblDoB.Text = _Person.DateOfBirth.ToString("MMM dd, yyyy");

            // 4. عرض الجنس بدلاً من الأرقام
            lblGendor.Text = (_Person.Gendor == 0) ? "Male" : "Female";

            // 5. جلب اسم الجنسية/الدولة الفعلي من الـ Business Layer
            // (تأكد من اسم الكلاس والدالة لديك، غالباً تكون Find وبداخلها نمرر الـ ID)
            DataTable dtCountries = clsCountries.GetAllCountries();
            DataRow[] foundRows = dtCountries.Select($"CountryID = {_Person.NationalCountryID}");
            if (foundRows.Length > 0)
            {
                // 3. جلب اسم الدولة أو اسم الجنسية مباشرة من السطر الذي تم العثور عليه
                lblNationality.Text = foundRows[0]["CountryName"].ToString();
            }
            else
            {
                lblNationality.Text = "Unknown";
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
                    // في حال حدوث خطأ غير متوقع أثناء قراءة الصورة، ضع الصورة الافتراضية
                    //LoadDefaultAvatar();
                }
            }
            else
            {
                //LoadDefaultAvatar();
            }
        }

        // دالة مساعدة لوضع الصورة الافتراضية حسب جنس الشخص
       /* private void LoadDefaultAvatar()
        {
            if (_Person.Gendor == 0)
                pbImage.Image = Properties.Resources.; // استبدلها باسم صورتك في الـ Resources
            else
                pbImage.Image = Properties.Resources.default_female_avatar;
        }*/
    }
}
