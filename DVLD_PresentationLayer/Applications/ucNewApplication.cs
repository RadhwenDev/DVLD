using DVLD_PresentationLayer.Properties;
using DVLD_PresentationLayer.Tests;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace DVLD_PresentationLayer.Applications
{
    public partial class ucNewApplication : UserControl
    {
        public delegate void DataBackEventHandler(object sender, int ApplicationID);
        public event DataBackEventHandler OnApplicationSaved;
        enum enSteps { FirstStep, SecondStep, ThirdStep }
        enSteps _Step = enSteps.FirstStep;

        // 🔥 المخزن المركزي للبيانات بين الخطوات 🔥
        public int SelectedPersonID { get; set; } = -1;
        public int SelectedApplicationTypeID { get; set; } = -1;
        public int SelectedLicenseClassID { get; set; } = -1;

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
            if (sender is ucFirstStepNewApp firstStep)
            {
                SelectedPersonID = firstStep.SelectedPersonID;
            }

            btnFirst.FillColor = Color.FromArgb(52, 77, 111);
            btnFirst.ForeColor = Color.White;
            btnFirst.Text = string.Empty;

            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ucNewApplication));
            btnFirst.Image = (Image)resources.GetObject("check_white");

            btnSecond.Enabled = true;
            _Step = enSteps.SecondStep;
            changeUserControl();
        }

        // حدث اكتمال الخطوة الثانية
        public void ucSecondStepNewApp1_OnStepTwoCompleted(object sender, EventArgs e)
        {
            // استقبال البيانات من الخطوة الثانية وتخزينها في المخزن المركزي
            if (sender is ucSecondStepNewApp secondStep)
            {
                SelectedApplicationTypeID = secondStep.SelectedApplicationTypeID;
                SelectedLicenseClassID = secondStep.SelectedLicenseClassID;
            }

            btnSecond.FillColor = Color.FromArgb(52, 77, 111);
            btnSecond.ForeColor = Color.White;
            btnSecond.Text = string.Empty;

            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ucNewApplication));
            btnSecond.Image = (Image)resources.GetObject("check_white");

            btnThird.Enabled = true;
            _Step = enSteps.ThirdStep;
            changeUserControl();
        }

        private void UcSecondStepNewApp1_OnBackButtonClicked(object sender, EventArgs e)
        {
            btnFirst.FillColor = Color.White;
            btnFirst.ForeColor = Color.Black;
            btnFirst.Text = "1";
            btnFirst.Image = null;
            btnSecond.Enabled = false;

            _Step = enSteps.FirstStep;
            changeUserControl();
        }

        private void ucThirdStepNewApp1_OnStepThirdCompleted(object sender, EventArgs e)
        {
            btnThird.FillColor = Color.FromArgb(52, 77, 111);
            btnThird.ForeColor = Color.White;
            btnThird.Text = string.Empty;

            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ucNewApplication));
            btnThird.Image = (Image)resources.GetObject("check_white");
        }

        private void ucThirdStepNewApp1_DataBack(object sender, int ApplicationID)
        {
            OnApplicationSaved?.Invoke(this, ApplicationID);
        }

        private void UcThirdStepNewApp1_OnBackButtonClicked(object sender, EventArgs e)
        {
            btnSecond.FillColor = Color.White;
            btnSecond.ForeColor = Color.Black;
            btnSecond.Text = "2";
            btnSecond.Image = null;
            btnThird.Enabled = false;

            _Step = enSteps.SecondStep;
            changeUserControl();
        }


        private void changeUserControl()
        {
            mainPanel.Controls.Clear();

            switch (_Step)
            {
                case enSteps.FirstStep:
                    ucFirstStepNewApp myStepF = new ucFirstStepNewApp(SelectedPersonID);
                    myStepF.Dock = DockStyle.Fill;
                    myStepF.OnStepOneCompleted += UcFirstStepNewApp1_OnStepOneCompleted;

                    mainPanel.Controls.Add(myStepF);
                    myStepF.Show();
                    break;

                case enSteps.SecondStep:
                    ucSecondStepNewApp myStepS = new ucSecondStepNewApp(SelectedPersonID, SelectedApplicationTypeID, SelectedLicenseClassID);
                    myStepS.Dock = DockStyle.Fill;
                    myStepS.OnStepTwoCompleted += ucSecondStepNewApp1_OnStepTwoCompleted;
                    myStepS.OnBackButtonClicked += UcSecondStepNewApp1_OnBackButtonClicked;

                    mainPanel.Controls.Add(myStepS);
                    myStepS.Show();
                    break;

                case enSteps.ThirdStep:
                    ucThirdStepNewApp myStepT = new ucThirdStepNewApp(SelectedPersonID, SelectedApplicationTypeID, SelectedLicenseClassID);
                    myStepT.Dock = DockStyle.Fill;

                    // 🌟 ربط الأحداث بشكل سليم ودون أخطاء بناء
                    myStepT.OnStepThirdCompleted += ucThirdStepNewApp1_OnStepThirdCompleted;
                    myStepT.OnBackButtonClicked += UcThirdStepNewApp1_OnBackButtonClicked;
                    myStepT.DataBack += ucThirdStepNewApp1_DataBack;

                    mainPanel.Controls.Add(myStepT);
                    myStepT.Show();
                    break;
            }
        }
    }
}