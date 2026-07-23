using DVLD_BusinessLayer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DVLD_PresentationLayer.TestAppointments
{
    public partial class ucVisionTest : UserControl
    {
        int _AppID = -1;
        public ucVisionTest(int appID)
        {
            InitializeComponent();
            _AppID = appID;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.FindForm()?.Close();
        }
        string PersonID = "";
        private void ucVisionTest_Load(object sender, EventArgs e)
        {
            DataTable dt = clsTestAppointment.visionTest(_AppID);
            lblStatus.Text = dt.Rows[0]["STATUS"].ToString();
            lblFullName.Text = dt.Rows[0]["FullName"].ToString();
            if (DateTime.TryParse(dt.Rows[0]["LastStatusDate"].ToString(), out DateTime statusDate))
                lblStatusDate.Text = statusDate.ToString("MMM dd, yyyy", CultureInfo.InvariantCulture);
            else
                lblStatusDate.Text = dt.Rows[0]["LastStatusDate"].ToString();
            if (DateTime.TryParse(dt.Rows[0]["ApplicationDate"].ToString(), out DateTime appDate))
                lblAppDate.Text = appDate.ToString("MMM dd, yyyy", CultureInfo.InvariantCulture);
            else
                lblAppDate.Text = dt.Rows[0]["ApplicationDate"].ToString();
            string imagePath = dt.Rows[0]["ImagePath"].ToString();

            if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
            {
                try
                {
                    using (var stream = new FileStream(imagePath, FileMode.Open, FileAccess.Read))
                    {
                        pbCreator.Image = System.Drawing.Image.FromStream(stream);
                    }
                }
                catch
                {
                }
            }
            else
            {
            }
            lblUserName.Text = dt.Rows[0]["CreatedByUserName"].ToString();
            PersonID = dt.Rows[0]["PersonID"].ToString();
            lblPersonID.Text = "ID: " + PersonID;
            if (decimal.TryParse(dt.Rows[0]["ApplicationPaidFees"].ToString(), out decimal classFees))
                lblFees.Text = classFees.ToString("N2", CultureInfo.InvariantCulture);
            else
                lblFees.Text = dt.Rows[0]["ApplicationPaidFees"].ToString();
            lblDLAppID.Text = dt.Rows[0]["LocalDrivingLicenseApplicationID"].ToString();
            lblClassName.Text = dt.Rows[0]["ClassName"].ToString();
            int testTypeID = 1;

            if (dt.Rows.Count > 0 && dt.Rows[0]["TestTypeID"] != DBNull.Value)
            {
                testTypeID = Convert.ToInt32(dt.Rows[0]["TestTypeID"]);
            }
            switch (testTypeID)
            {
                case 1:
                    lblPassedTests.Text = "0/3"; break;
                case 2:
                    lblPassedTests.Text = "1/3"; break;
                case 3:
                    lblPassedTests.Text = "2/3"; break;
            }
            DataTable dgv = clsTestAppointment.visionTestDataGridView(_AppID);
            bool hasAppointments = dgv.Rows.Count > 0 && dgv.Rows[0]["TestAppointmentID"] != DBNull.Value;

            if (hasAppointments)
            {
                dgvAppointments.DataSource = dgv;
            }
            
        }

        private void dgvAppointments_Paint(object sender, PaintEventArgs e)
        {
            if (dgvAppointments.Rows.Count == 0)
            {
                string noDataText = "No appointments scheduled. Click to add a new one.";

                // اختيار الخط واللون المناسب (رمادي هادئ ومريح للعين)
                using (Font font = new Font("Segoe UI", 11, FontStyle.Regular))
                using (Brush brush = new SolidBrush(Color.FromArgb(120, 144, 156))) // Slate Gray
                {
                    // حساب قياسات النص لتوسيطه تماماً في وسط الـ Grid
                    Size textSize = TextRenderer.MeasureText(noDataText, font);

                    // نأخذ بعين الاعتبار ارتفاع الـ Headers باش يجي النص في وسط المساحة البيضاء بالظبط
                    int headersHeight = dgvAppointments.ColumnHeadersVisible ? dgvAppointments.ColumnHeadersHeight : 0;

                    int x = (dgvAppointments.Width - textSize.Width) / 2;
                    int y = headersHeight + (dgvAppointments.Height - headersHeight - textSize.Height) - 30;

                    // رسم النص
                    e.Graphics.DrawString(noDataText, font, brush, x, y);
                }
            }
        }

        private void linkLblViewFullProfile_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            int selectedPersonID = Convert.ToInt32(PersonID);


            using (Form overlay = new Form())
            {
                overlay.StartPosition = FormStartPosition.Manual;
                overlay.FormBorderStyle = FormBorderStyle.None;
                overlay.BackColor = Color.FromArgb(45, 55, 72);
                overlay.Opacity = 0.45d;
                overlay.Bounds = Screen.FromControl(this).Bounds;
                overlay.ShowInTaskbar = false;
                overlay.Show(this);

                using (Form frmContainer = new Form())
                {
                    frmContainer.FormBorderStyle = FormBorderStyle.None;
                    frmContainer.BackColor = Color.White;
                    frmContainer.StartPosition = FormStartPosition.CenterParent;

                    ucShowDetails myShowDetails = new ucShowDetails();

                    // 🌟 هنا نقوم بتمرير الـ ID المجلوب مباشرة ليتحول الـ User Control إلى وضع الـ Update تلقائياً
                    myShowDetails.LoadPersonData(selectedPersonID);

                    frmContainer.Size = myShowDetails.Size;
                    myShowDetails.Dock = DockStyle.Fill;
                    frmContainer.Controls.Add(myShowDetails);

                    Guna.UI2.WinForms.Guna2Elipse elipse = new Guna.UI2.WinForms.Guna2Elipse();
                    elipse.TargetControl = frmContainer;
                    elipse.BorderRadius = 16;

                    frmContainer.ShowDialog(overlay);
                }
            }
        }
    }
}
