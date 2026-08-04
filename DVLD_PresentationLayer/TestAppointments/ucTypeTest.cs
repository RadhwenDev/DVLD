using DVLD_BusinessLayer;
using DVLD_PresentationLayer.Drivers;
using DVLD_PresentationLayer.Tests;
using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;

namespace DVLD_PresentationLayer.TestAppointments
{
    public partial class ucTypeTest : UserControl
    {
        public event ucAppointmentTest.DataBackEventHandler DataBack;
        public enum enTestType { Vision = 1, Written = 2, Street = 3 }

        private enTestType _TestType = enTestType.Vision;
        private int _AppID = -1;
        private int _LocalDrivingLicenseApplicationID = -1; // تم إضافة المتغير هنا
        private int _PersonID = -1;

        public ucTypeTest(int appID)
        {
            InitializeComponent();
            _AppID = appID;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.FindForm()?.Close();
        }

        private void ucVisionTest_Load(object sender, EventArgs e)
        {
            LoadApplicationInfo();
            ConfigureTestSteps();
            LoadAppointments();
        }

        #region Data Loading Methods

        private void LoadApplicationInfo()
        {
            DataTable dt = clsTestAppointment.GetAppointmentDetails(_AppID);
            if (dt == null || dt.Rows.Count == 0)
            {
                MessageBox.Show("No application data found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DataRow row = dt.Rows[0];

            lblStatus.Text = row["STATUS"]?.ToString() ?? "";
            lblFullName.Text = row["FullName"]?.ToString() ?? "";

            // تنسيق التواريخ بشكل آمن
            lblStatusDate.Text = FormatDate(row["LastStatusDate"]);
            lblAppDate.Text = FormatDate(row["ApplicationDate"]);

            // تحميل صورة الشخص
            LoadPersonImage(row["ImagePath"]?.ToString());

            lblUserName.Text = row["CreatedByUserName"]?.ToString() ?? "";

            if (int.TryParse(row["PersonID"]?.ToString(), out int pID))
            {
                _PersonID = pID;
                lblPersonID.Text = "ID: " + _PersonID;
            }

            // تنسيق الرسوم
            if (decimal.TryParse(row["ApplicationPaidFees"]?.ToString(), out decimal classFees))
                lblFees.Text = classFees.ToString("N2", CultureInfo.InvariantCulture);
            else
                lblFees.Text = row["ApplicationPaidFees"]?.ToString() ?? "0.00";

            // قراءة وحفظ LocalDrivingLicenseApplicationID
            if (int.TryParse(row["LocalDrivingLicenseApplicationID"]?.ToString(), out int localDLAppID))
            {
                _LocalDrivingLicenseApplicationID = localDLAppID;
                lblDLAppID.Text = _LocalDrivingLicenseApplicationID.ToString();
            }
            else
            {
                lblDLAppID.Text = "";
            }

            lblClassName.Text = row["ClassName"]?.ToString() ?? "";

            // تحديد نوع الاختبار
            if (row["TestTypeID"] != DBNull.Value && int.TryParse(row["TestTypeID"].ToString(), out int tTypeID))
            {
                _TestType = (enTestType)tTypeID;
            }

        }

        private void LoadAppointments()
        {
            // تم التعديل: تمرير _LocalDrivingLicenseApplicationID بدلاً من _AppID
            DataTable dgv = clsTestAppointment.GetApplicationAppointments(_LocalDrivingLicenseApplicationID, (int)_TestType);

            bool hasAppointments = dgv != null && dgv.Rows.Count > 0;

            if (hasAppointments)
            {
                dgvAppointments.DataSource = dgv;

                // قراءة قيمة IsLocked لأحدث موعد (الصف الأول لأن الاستعلام مرتب DESC)
                bool isLocked = Convert.ToBoolean(dgv.Rows[0]["IsLocked"]);

                // إذا كان الموعد الأخير مغلقاً (مُنجز)، نُتيح إضافة موعد جديد ونمنع تعديل الموعد المغلق
                btnNewAppointment.Enabled = isLocked;
            }
            else
            {
                // لا يوجد أي موعد سابق لهذا الاختبار
                dgvAppointments.DataSource = null;
                btnNewAppointment.Enabled = true;
            }
            UpdateContextMenu();
        }
        private void UpdateContextMenu()
        {
            if (dgvAppointments.CurrentRow == null)
            {
                editToolStripMenuItem.Enabled = false;
                takeTestToolStripMenuItem.Enabled = false;
                return;
            }
            bool isLocked = Convert.ToBoolean(dgvAppointments.CurrentRow.Cells["IsLocked"].Value);
            editToolStripMenuItem.Enabled = !isLocked;
            takeTestToolStripMenuItem.Enabled = !isLocked;
        }

        #endregion

        #region UI & Image Helpers

        private string FormatDate(object dateValue)
        {
            if (dateValue != null && DateTime.TryParse(dateValue.ToString(), out DateTime parsedDate))
                return parsedDate.ToString("MMM dd, yyyy", CultureInfo.InvariantCulture);

            return dateValue?.ToString() ?? "";
        }

        private void LoadPersonImage(string imagePath)
        {
            if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
            {
                try
                {
                    using (var stream = new FileStream(imagePath, FileMode.Open, FileAccess.Read))
                    {
                        pbCreator.Image = Image.FromStream(stream);
                    }
                }
                catch
                {
                    pbCreator.Image = null;
                }
            }
            else
            {
                pbCreator.Image = null;
            }
        }

        private void ConfigureTestSteps()
        {
            ComponentResourceManager resources = new ComponentResourceManager(typeof(ucTypeTest));
            byte passedTests = clsTestAppointment.GetPassedTestCountApplication(_AppID);

            switch (passedTests)
            {
                case 0:
                    lblPassedTests.Text = "0/3";
                    break;

                case 1:
                    lblPassedTests.Text = "1/3";
                    guna2Separator2.FillColor = Color.FromArgb(12, 155, 161);
                    guna2CirclePictureBox6.Image = null;
                    guna2CirclePictureBox6.FillColor = Color.FromArgb(12, 155, 161);
                    guna2CirclePictureBox7.Image = (Image)resources.GetObject("Written");
                    lblCurrentTest2.Text = "2. Vision Test (Done)";
                    lblCurrentTest3.Text = "3. Written Test (Current)";
                    lblPassedTestStatus2.Text = string.Empty;
                    lblPassedTestStatus3.Text = "1/3 Passed Tests";
                    break;

                case 2:
                    lblPassedTests.Text = "2/3";
                    guna2Separator2.FillColor = Color.FromArgb(12, 155, 161);
                    guna2Separator3.FillColor = Color.FromArgb(12, 155, 161);
                    guna2CirclePictureBox6.Image = null;
                    guna2CirclePictureBox6.FillColor = Color.FromArgb(12, 155, 161);
                    guna2CirclePictureBox7.Image = null;
                    guna2CirclePictureBox7.FillColor = Color.FromArgb(12, 155, 161);
                    guna2CirclePictureBox8.Image = (Image)resources.GetObject("Street");
                    lblCurrentTest2.Text = "2. Vision Test (Done)";
                    lblCurrentTest3.Text = "3. Written Test (Done)";
                    lblCurrentTest4.Text = "4. Street Test (Current)";
                    lblPassedTestStatus2.Text = string.Empty;
                    lblPassedTestStatus3.Text = string.Empty;
                    lblPassedTestStatus4.Text = "2/3 Passed Tests";
                    break;
            }
        }

        private void dgvAppointments_Paint(object sender, PaintEventArgs e)
        {
            if (dgvAppointments.Rows.Count == 0)
            {
                string noDataText = "No appointments scheduled. Click to add a new one.";

                using (Font font = new Font("Segoe UI", 11, FontStyle.Regular))
                using (Brush brush = new SolidBrush(Color.FromArgb(120, 144, 156)))
                {
                    Size textSize = TextRenderer.MeasureText(noDataText, font);
                    int headersHeight = dgvAppointments.ColumnHeadersVisible ? dgvAppointments.ColumnHeadersHeight : 0;

                    int x = (dgvAppointments.Width - textSize.Width) / 2;
                    int y = headersHeight + (dgvAppointments.Height - headersHeight - textSize.Height) - 30;

                    e.Graphics.DrawString(noDataText, font, brush, x, y);
                }
            }
        }

        #endregion

        #region Generic Modal Helper

        private void ShowModalUserControl(UserControl uc)
        {
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

                    frmContainer.Size = uc.Size;
                    uc.Dock = DockStyle.Fill;
                    frmContainer.Controls.Add(uc);

                    Guna.UI2.WinForms.Guna2Elipse elipse = new Guna.UI2.WinForms.Guna2Elipse
                    {
                        TargetControl = frmContainer,
                        BorderRadius = 16
                    };

                    frmContainer.ShowDialog(overlay);
                }
            }

            // إعادة تحديث المواعيد تلقائياً عند إغلاق النافذة المنبثقة
            LoadAppointments();
        }

        #endregion

        #region Helper Method for Selected Appointment

        private int GetSelectedTestAppointmentID()
        {
            if (dgvAppointments.CurrentRow != null && dgvAppointments.CurrentRow.Index >= 0)
            {
                if (dgvAppointments.CurrentRow.Cells["TestAppointmentID"] != null &&
                    int.TryParse(dgvAppointments.CurrentRow.Cells["TestAppointmentID"].Value?.ToString(), out int appointmentID))
                {
                    return appointmentID;
                }
            }
            return -1;
        }

        #endregion

        #region Actions & Events

        private void linkLblViewFullProfile_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (_PersonID <= 0) return;

            ucShowDetails myShowDetails = new ucShowDetails();
            myShowDetails.LoadPersonData(_PersonID);

            ShowModalUserControl(myShowDetails);
        }

        private void btnNewAppointment_Click(object sender, EventArgs e)
        {
            if (_LocalDrivingLicenseApplicationID <= 0)
            {
                MessageBox.Show("Invalid Local Driving License Application ID!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            ucAppointmentTest ucAppointment = new ucAppointmentTest(_LocalDrivingLicenseApplicationID, 0, -1);
            ucAppointment.DataBack += Uc_DataBack;
            ShowModalUserControl(ucAppointment);
        }

        private void Uc_DataBack(object sender, int testAppointmentID)
        {
            DataBack?.Invoke(this, testAppointmentID);
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int appointmentID = GetSelectedTestAppointmentID();

            if (appointmentID == -1)
            {
                MessageBox.Show("Please select an appointment to edit!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_LocalDrivingLicenseApplicationID <= 0)
            {
                MessageBox.Show("Invalid Local Driving License Application ID!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            ucAppointmentTest ucAppointment = new ucAppointmentTest(_LocalDrivingLicenseApplicationID, 1, appointmentID);
            ucAppointment.DataBack += Uc_DataBack;
            ShowModalUserControl(ucAppointment);
        }

        private void btnLicenseHistory_Click(object sender, EventArgs e)
        {
            if (_AppID <= 0) return;

            ucShowPersonLicenseHistory myLicenseHistory = new ucShowPersonLicenseHistory(_AppID);
            ShowModalUserControl(myLicenseHistory);
        }

        private void takeTestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int appointmentID = GetSelectedTestAppointmentID();

            if (appointmentID == -1)
            {
                MessageBox.Show("Please select an appointment to take the test!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ucTakeTest myTakeTest = new ucTakeTest(appointmentID, (int)_TestType);
            myTakeTest.OnTestSaved += MyTakeTest_OnTestSaved;
            ShowModalUserControl(myTakeTest);
        }

        private void MyTakeTest_OnTestSaved(object sender, bool isSuccess)
        {
            if (!isSuccess)
                return;

            DataBack?.Invoke(this, _AppID);

            this.FindForm()?.Close();
        }

        #endregion

        #region Drawing Custom Toggle Switch

        private void DrawToggleSwitch(Graphics g, Rectangle bounds, bool isActive)
        {
            int switchWidth = 44;
            int switchHeight = 22;

            int x = bounds.X + (bounds.Width - switchWidth) / 4;
            int y = bounds.Y + (bounds.Height - switchHeight) / 2;

            Rectangle switchRect = new Rectangle(x, y, switchWidth, switchHeight);

            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            Color toggleBg = isActive ? Color.FromArgb(211, 47, 47) : Color.FromArgb(190, 200, 210);
            using (Brush bgBrush = new SolidBrush(toggleBg))
            {
                using (System.Drawing.Drawing2D.GraphicsPath path = GetRoundedPath(switchRect, switchHeight))
                {
                    g.FillPath(bgBrush, path);
                }
            }

            int circleMargin = 2;
            int circleSize = switchHeight - (circleMargin * 2);

            int circleX = isActive ? (switchRect.Right - circleSize - circleMargin) : (switchRect.Left + circleMargin);
            Rectangle circleRect = new Rectangle(circleX, y + circleMargin, circleSize, circleSize);

            using (Brush circleBrush = new SolidBrush(Color.White))
            {
                g.FillEllipse(circleBrush, circleRect);
            }
        }

        private System.Drawing.Drawing2D.GraphicsPath GetRoundedPath(Rectangle rect, int radius)
        {
            System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
            path.AddArc(rect.Right - radius, rect.Y, radius, radius, 270, 90);
            path.AddArc(rect.Right - radius, rect.Bottom - radius, radius, radius, 0, 90);
            path.AddArc(rect.X, rect.Bottom - radius, radius, radius, 90, 90);
            path.CloseFigure();
            return path;
        }

        private void dgvAppointments_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                e.Handled = true;

                using (Brush backBrush = new SolidBrush(dgvAppointments.BackgroundColor))
                {
                    e.Graphics.FillRectangle(backBrush, e.CellBounds);
                }

                Rectangle cardBounds = new Rectangle(
                    e.CellBounds.X,
                    e.CellBounds.Y + 3,
                    e.CellBounds.Width,
                    e.CellBounds.Height - 6
                );

                bool isSelected = dgvAppointments.Rows[e.RowIndex].Selected;
                Color cardColor = isSelected ? Color.FromArgb(210, 224, 242) : Color.FromArgb(228, 236, 247);

                using (Brush cardBrush = new SolidBrush(cardColor))
                {
                    e.Graphics.FillRectangle(cardBrush, cardBounds);
                }

                string columnName = dgvAppointments.Columns[e.ColumnIndex].Name;
                string headerText = dgvAppointments.Columns[e.ColumnIndex].HeaderText;

                if (columnName.Equals("IsLocked", StringComparison.OrdinalIgnoreCase) ||
                    headerText.Equals("Is Locked", StringComparison.OrdinalIgnoreCase) ||
                    columnName.Equals("IsActive", StringComparison.OrdinalIgnoreCase) ||
                    headerText.Equals("Is Active", StringComparison.OrdinalIgnoreCase))
                {
                    bool isLocked = false;
                    if (e.Value != null && e.Value != DBNull.Value)
                    {
                        isLocked = Convert.ToBoolean(e.Value);
                    }

                    DrawToggleSwitch(e.Graphics, cardBounds, isLocked);
                }
                else
                {
                    TextRenderer.DrawText(
                        e.Graphics,
                        e.Value?.ToString() ?? "",
                        e.CellStyle.Font,
                        cardBounds,
                        e.CellStyle.ForeColor,
                        TextFormatFlags.VerticalCenter | TextFormatFlags.Left
                    );
                }
            }
        }

        #endregion

        private void dgvAppointments_SelectionChanged(object sender, EventArgs e)
        {
            UpdateContextMenu();
        }
    }
}