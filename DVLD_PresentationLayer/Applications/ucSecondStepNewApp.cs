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
    public partial class ucSecondStepNewApp : UserControl
    {
        public int SelectedPersonID { get; private set; } = -1;
        public int SelectedApplicationTypeID { get; private set; } = -1;
        public int SelectedLicenseClassID { get; private set; } = -1;

        public event EventHandler OnBackButtonClicked;
        public event EventHandler OnStepTwoCompleted;

        public ucSecondStepNewApp()
        {
            InitializeComponent();
        }

        // استقبال البيانات كافة عند إنشاء الكنترول
        public ucSecondStepNewApp(int personID, int applicationTypeID, int licenseClassID) : this()
        {
            SelectedPersonID = personID;
            SelectedApplicationTypeID = applicationTypeID;
            SelectedLicenseClassID = licenseClassID;
        }

        private void ucSecondStepNewApp_Load(object sender, EventArgs e)
        {
            // 🔥 1. فك ارتباط الأحداث مؤقتاً لمنع الـ ComboBox من تصفير المتغيرات الممررة تلقائياً
            cbApplicationType.SelectedIndexChanged -= cbApplicationType_SelectedIndexChanged;
            cbLicenseClass.SelectedIndexChanged -= cbLicenseClass_SelectedIndexChanged;

            // 2. تعبئة أنواع الطلبات
            bool hasLicense = clsLicenses.hasLicense(SelectedPersonID);
            DataTable dtApplicantType = clsApplicant.getAllApplicationTypes(hasLicense);
            bool hasInternationalLicense = clsLicenses.hasInternationalLicense(SelectedPersonID);
            if (hasInternationalLicense)
            {
                dtApplicantType.DefaultView.RowFilter = "[ApplicationTypeTitle] <> 'New International License'";
            }
            DataRow defaultRow = dtApplicantType.NewRow();
            defaultRow["ApplicationTypeTitle"] = "Select the Application Type";
            defaultRow["ApplicationTypeID"] = -1;
            dtApplicantType.Rows.InsertAt(defaultRow, 0);

            cbApplicationType.DataSource = dtApplicantType;
            cbApplicationType.DisplayMember = "ApplicationTypeTitle";
            cbApplicationType.ValueMember = "ApplicationTypeID";

            // 3. تعبئة أصناف الرخص
            DataTable dtLicenseClass = clsLicenseClass.GetLicenseClassesName(SelectedPersonID);
            DataRow defaultRow2 = dtLicenseClass.NewRow();
            defaultRow2["ClassName"] = "Select the License Class";
            defaultRow2["LicenseClassID"] = -1;
            dtLicenseClass.Rows.InsertAt(defaultRow2, 0);

            cbLicenseClass.DataSource = dtLicenseClass;
            cbLicenseClass.DisplayMember = "ClassName";
            cbLicenseClass.ValueMember = "LicenseClassID";

            // 🔥 4. إعادة تعيين الحالات السابقة المخزنة مركزيّاً بدقة
            if (SelectedApplicationTypeID != -1)
            {
                cbApplicationType.SelectedValue = SelectedApplicationTypeID;

                if (SelectedApplicationTypeID == 1 || SelectedApplicationTypeID == 7)
                {
                    cbLicenseClass.Visible = true;
                    if (SelectedLicenseClassID != -1)
                    {
                        cbLicenseClass.SelectedValue = SelectedLicenseClassID;
                    }
                }
                else
                {
                    cbLicenseClass.Visible = false;
                }
            }
            else
            {
                cbApplicationType.SelectedValue = -1;
                cbLicenseClass.Visible = false;
            }

            // 🔥 5. إعادة ربط الأحداث مجدداً بعد استقرار البيانات المرجعة
            cbApplicationType.SelectedIndexChanged += cbApplicationType_SelectedIndexChanged;
            cbLicenseClass.SelectedIndexChanged += cbLicenseClass_SelectedIndexChanged;
        }

        private void cbApplicationType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbApplicationType.SelectedValue == null || !int.TryParse(cbApplicationType.SelectedValue.ToString(), out int selectedTypeID) || selectedTypeID == -1)
            {
                cbLicenseClass.Visible = false;
                SelectedApplicationTypeID = -1;
                return;
            }

            SelectedApplicationTypeID = selectedTypeID;

            if (SelectedApplicationTypeID == 1 || SelectedApplicationTypeID == 7)
            {
                cbLicenseClass.Visible = true;
            }
            else
            {
                cbLicenseClass.Visible = false;
                SelectedLicenseClassID = -1; // تصفير الصنف إن لم يكن مطلوباً
            }
        }

       

        private void btnContinue_Click(object sender, EventArgs e)
        {
            // التحقق من صحة المدخلات قبل الانتقال للخطوة 3
            if (SelectedApplicationTypeID == -1)
            {
                MessageBox.Show("Please select an application type.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cbLicenseClass.Visible && SelectedLicenseClassID == -1)
            {
                MessageBox.Show("Please select a license class.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            OnStepTwoCompleted?.Invoke(this, e);
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            OnBackButtonClicked?.Invoke(this, e);
        }

        private void cbLicenseClass_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbLicenseClass.SelectedValue != null && int.TryParse(cbLicenseClass.SelectedValue.ToString(), out int classID))
            {
                SelectedLicenseClassID = classID;
            }
        }
    }
}