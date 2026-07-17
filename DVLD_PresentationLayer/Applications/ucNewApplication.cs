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
        public ucNewApplication()
        {
            InitializeComponent();
        }

        private void ucNewApplication_Load(object sender, EventArgs e)
        {
            // 1. إنشاء نسخة (Instance) من الـ UserControl الصغير
            ucFirstStepNewApp myStep = new ucFirstStepNewApp();

            // 2. جعل الـ UserControl يملأ الـ mainPanel بالكامل لكي يظهر بشكل صحيح
            myStep.Dock = DockStyle.Fill;

            // 3. الاشتراك في الـ Event باستخدام اسم الأوبجكت (myStep) وليس اسم الكلاس
            myStep.OnStepOneCompleted += UcFirstStepNewApp1_OnStepOneCompleted;

            // 4. إضافة الـ Control للحاوية وإظهاره
            mainPanel.Controls.Add(myStep);
            myStep.Show();
        }

        private void UcFirstStepNewApp1_OnStepOneCompleted(object sender, EventArgs e)
        {
            // تغيير لون الـ Button للون المطلوب بنجاح
            btnFirst.FillColor = Color.FromArgb(52, 77, 111);
            btnFirst.ForeColor = Color.White;
            btnFirst.Text = string.Empty;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ucNewApplication));
            btnFirst.Image = (Image)resources.GetObject("check_white");
            btnSecond.Enabled = true;
        }
    }
}