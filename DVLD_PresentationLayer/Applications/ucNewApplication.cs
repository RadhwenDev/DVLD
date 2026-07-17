using DVLD_PresentationLayer.Properties;
using DVLD_PresentationLayer.Tests;
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
    public partial class ucNewApplication : UserControl
    {
        enum enSteps { FirstStep, SecondStep, ThirdStep }
        enSteps _Step = enSteps.FirstStep;

        public ucNewApplication()
        {
            InitializeComponent();
        }

        private void ucNewApplication_Load(object sender, EventArgs e)
        {
            changeUserControl();
        }

        // حدث اكتمال الخطوة الأولى
        private void UcFirstStepNewApp1_OnStepOneCompleted(object sender, EventArgs e)
        {
            // 1. تحويل الزر الأول لعلامة الصح البيضاء بالخلفية الملوّنة
            btnFirst.FillColor = Color.FromArgb(52, 77, 111);
            btnFirst.ForeColor = Color.White;
            btnFirst.Text = string.Empty;

            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ucNewApplication));
            btnFirst.Image = (Image)resources.GetObject("check_white");

            // 2. تفعيل زر الخطوة الثانية والانتقال لها
            btnSecond.Enabled = true;
            _Step = enSteps.SecondStep;
            changeUserControl();
        }

        // حدث اكتمال الخطوة الثانية (تم إصلاح المراجع هنا لتخص الزر الثاني)
        private void ucSecondStepNewApp1_OnStepOneCompleted(object sender, EventArgs e)
        {
            // 1. تحويل الزر الثاني لعلامة الصح البيضاء بالخلفية الملوّنة
            btnSecond.FillColor = Color.FromArgb(52, 77, 111);
            btnSecond.ForeColor = Color.White;
            btnSecond.Text = string.Empty;

            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ucNewApplication));
            btnSecond.Image = (Image)resources.GetObject("check_white");

            // 2. تفعيل زر الخطوة الثالثة والانتقال لها
            //if (btnThird != null) btnThird.Enabled = true;
            _Step = enSteps.ThirdStep;
            changeUserControl();
        }

        private void changeUserControl()
        {
            // 🔥 خطوة أساسية: إفراغ الـ Panel تماماً من الخطوة السابقة قبل إضافة الخطوة الجديدة
            mainPanel.Controls.Clear();

            switch (_Step)
            {
                case enSteps.FirstStep:
                    ucFirstStepNewApp myStepF = new ucFirstStepNewApp();
                    myStepF.Dock = DockStyle.Fill;
                    myStepF.OnStepOneCompleted += UcFirstStepNewApp1_OnStepOneCompleted;

                    mainPanel.Controls.Add(myStepF);
                    myStepF.Show();
                    break;

                case enSteps.SecondStep:
                    ucSecondStepNewApp myStepS = new ucSecondStepNewApp();
                    myStepS.Dock = DockStyle.Fill;
                    myStepS.OnStepOneCompleted += ucSecondStepNewApp1_OnStepOneCompleted;

                    mainPanel.Controls.Add(myStepS);
                    myStepS.Show();
                    break;

                case enSteps.ThirdStep:
                    // هوني تنجم تضيف الـ UserControl الثالث الخاص بالخطوة الأخيرة بنفس الطريقة لاحقاً
                    break;
            }
        }
    }
}