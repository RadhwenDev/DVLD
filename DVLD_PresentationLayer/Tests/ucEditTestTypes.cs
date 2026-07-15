using DVLD_BusinessLayer;
using DVLD_PresentationLayer.Properties;
using Guna.UI2.WinForms.Suite;
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

namespace DVLD_PresentationLayer.Tests
{
    public partial class ucEditTestTypes : UserControl
    {
        clsTestTypes _TestTypes;
        private int _TestID = -1;
        string Step, Description; int Fees;
        public delegate void DataBackEventHandler(object sender, int PersonID);
        public event DataBackEventHandler DataBack;
        public ucEditTestTypes(int TestID, string Step, string Description, int Fees)
        {
            InitializeComponent();
            _TestTypes = new clsTestTypes(TestID, Step, Description, Fees);
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ucEditTestTypes));
            pbTestTypeImage.SizeMode = PictureBoxSizeMode.Zoom;

            switch (TestID)
            {
                case 1: // Vision Test (درجة أزرق هادئ ولطيف مثل الصورة الأولى)
                    pbTestTypeImage.Image = (Image)resources.GetObject("Vision_Test_32");
                    pbTestTypeImage.BackColor = Color.FromArgb(239, 246, 255); // الأزرق الشفاف الاحترافي
                    break;

                case 2: // Written Test (درجة أرجواني/بنفسجي هادئ يتناسق مع أيقونة الاختبار المكتوب)
                    pbTestTypeImage.Image = (Image)resources.GetObject("Written_Test_32");
                    pbTestTypeImage.BackColor = Color.FromArgb(250, 245, 255); // أرجواني شفاف ناعم
                    break;

                case 3: // Practical Test / Street (درجة أخضر هادئ ترمز للمرور والانطلاق)
                    pbTestTypeImage.Image = (Image)resources.GetObject("Street_Test_32");
                    pbTestTypeImage.BackColor = Color.FromArgb(255, 251, 235); // الأخضر الشفاف الأصلي الخاص بك
                    break;
            }
        }


        private void btnFirstCancelEdit_Click_1(object sender, EventArgs e)
        {
            this.FindForm()?.Close();
        }

        private void btnFirstSave_Click(object sender, EventArgs e)
        {
            _TestTypes.Description = txtDescriptionFirstEdit.Text.Trim();
            _TestTypes.Fees = Convert.ToInt32(nudFirstEditFees.Value);
            switch (_TestTypes.Save())
            {
                case clsTestTypes.enSaveResult.SavedSuccessfully:
                    MessageBox.Show($"Test Type saved successfully with ID = {_TestTypes.TestID}!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    _TestID = _TestTypes.TestID;

                    DataBack?.Invoke(this, _TestTypes.TestID);
                    this.FindForm()?.Close();
                    break;
                case clsTestTypes.enSaveResult.NoChanges:
                    MessageBox.Show("Nothing was changed");
                    break;
                case clsTestTypes.enSaveResult.Failed:
                    MessageBox.Show("Failed to save person data.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
            }
        }


        private void ucEditTestTypes_Load(object sender, EventArgs e)
        {
            lblFirstStepEdit.Text = _TestTypes.Step;
            txtDescriptionFirstEdit.Text = _TestTypes.Description;
            nudFirstEditFees.Value = decimal.Parse(_TestTypes.Fees.ToString());
            
        }
    }
}
