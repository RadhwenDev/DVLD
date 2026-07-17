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
        public event EventHandler OnStepOneCompleted;
        public ucSecondStepNewApp()
        {
            InitializeComponent();
        }

        private void ucSecondStepNewApp_Load(object sender, EventArgs e)
        {
            DataTable dtApplicant = clsApplicant.getAllApplicationTypes();
            DataRow defaultRow = dtApplicant.NewRow();
            defaultRow["ApplicationTypeTitle"] = "Select the Application Type";
            defaultRow["ApplicationTypeID"] = -1;
            dtApplicant.Rows.InsertAt(defaultRow, 0);

            cbApplicationType.DataSource = dtApplicant;
            cbApplicationType.DisplayMember = "ApplicationTypeTitle";
            cbApplicationType.ValueMember = "ApplicationTypeID";
        }

        private void cbApplicationType_SelectedIndexChanged(object sender, EventArgs e)
        {
            // 1. التأكد أن القيمة المختارة صالحة وليست السطر الافتراضي (-1)
            if (cbApplicationType.SelectedValue == null || !int.TryParse(cbApplicationType.SelectedValue.ToString(), out int selectedTypeID) || selectedTypeID == -1)
            {
                cbLicenseClass.Visible = false;
                return;
            }

            // 2. الفحص الدقيق بناءً على الـ IDs الحقيقية في قاعدة بياناتك (1 و 8)
            if (selectedTypeID == 1 || selectedTypeID == 8)
            {
                // إظهار الـ ComboBox الخاص بأصناف الرخص
                cbLicenseClass.Visible = true;

                // تحريك الأزرار للأسفل ديناميكياً لتوفر مساحة للـ ComboBox
            }
            else
            {
                // إخفاء الـ ComboBox وإرجاع الأزرار لمكانها الأصلي في بقية الخدمات
                cbLicenseClass.Visible = false;
            }
        }
    }
}
