using DVLD_BusinessLayer;
using DVLD_PresentationLayer.Drivers;
using DVLD_PresentationLayer.Licenses;
using DVLD_PresentationLayer.TestAppointments;
using Guna.UI2.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DVLD_PresentationLayer.Applications
{
    public partial class ucApplications : UserControl
    {
        public ucApplications()
        {
            InitializeComponent();
        }

        private DataTable _dtAllApplicants;

        private void ucApplications_Load(object sender, EventArgs e)
        {
            typeof(DataGridView).InvokeMember("DoubleBuffered",
               System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.SetProperty,
               null, dgvApplications, new object[] { true });

            _dtAllApplicants = clsApplicant.getAllApplicants();
            dgvApplications.DataSource = _dtAllApplicants;

            if (!dgvApplications.Columns.Contains("ACTIONS"))
            {
                DataGridViewImageColumn actionsCol = new DataGridViewImageColumn();
                actionsCol.Name = "ACTIONS";
                actionsCol.Width = 120;
                actionsCol.DefaultCellStyle.NullValue = null;
                dgvApplications.Columns.Add(actionsCol);
            }

            if (dgvApplications.Columns.Count > 0)
            {
                dgvApplications.Columns["  ID"].DefaultCellStyle.Padding = new Padding(10, 0, 0, 0);
                dgvApplications.CellBorderStyle = DataGridViewCellBorderStyle.None;
                dgvApplications.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
                dgvApplications.RowHeadersVisible = false;
                dgvApplications.Columns["  ID"].Width = 100;
                dgvApplications.Columns["APPLICANT"].Width = 200;
                dgvApplications.Columns["SERVICE TYPE"].Width = 200;
                dgvApplications.Columns["ACTIONS"].Width = 100;
                dgvApplications.Columns["DATE"].DefaultCellStyle.Format = "MMM dd, yyyy";
            }
            dgvApplications.RowTemplate.DefaultCellStyle.Padding = new Padding(15, 8, 15, 8);
            dgvApplications.RowTemplate.Height = 40;

            dgvApplications.CellBorderStyle = DataGridViewCellBorderStyle.None;
            dgvApplications.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;

            dgvApplications.DefaultCellStyle.SelectionBackColor = Color.FromArgb(243, 244, 246);
            dgvApplications.DefaultCellStyle.SelectionForeColor = Color.Black;

            dgvApplications.Columns["FEES PAID"].DefaultCellStyle.Format = "N0";

            DataTable dtStatus = clsApplicant.getAllApplicationStatus();
            dtStatus.Columns["STATUS"].MaxLength = -1;
            DataRow defaultRowStatus = dtStatus.NewRow();
            defaultRowStatus["STATUS"] = "All Status";
            defaultRowStatus["StatusID"] = (byte)255;
            dtStatus.Rows.InsertAt(defaultRowStatus, 0);

            cbStatuses.DataSource = dtStatus;
            cbStatuses.DisplayMember = "STATUS";
            cbStatuses.ValueMember = "StatusID";

            DataTable dtTypes = clsApplicant.getAllApplicationTypes();
            DataRow defaultRow = dtTypes.NewRow();
            defaultRow["ApplicationTypeTitle"] = "All Types";
            defaultRow["ApplicationTypeID"] = -1;
            dtTypes.Rows.InsertAt(defaultRow, 0);

            cbTypes.DataSource = dtTypes;
            cbTypes.DisplayMember = "ApplicationTypeTitle";
            cbTypes.ValueMember = "ApplicationTypeID";

            dgvApplications.Paint += dgvApplications_Paint;

            cbTypes.SelectedIndexChanged += cbTypes_SelectedIndexChanged;
            cbStatuses.SelectedIndexChanged += cbStatuses_SelectedIndexChanged;

            UpdateRowsCount(_dtAllApplicants);

        }


        private void ApplyCombinedFilter()
        {
            if (_dtAllApplicants == null) return;

            List<string> filters = new List<string>();

            string textSearch = tbFilterNameAppID.Text.Replace("'", "''").Trim();
            if (!string.IsNullOrEmpty(textSearch))
            {
                filters.Add($"(APPLICANT LIKE '%{textSearch}%' OR CONVERT([  ID], 'System.String') LIKE '%{textSearch}%')");
            }

            if (cbTypes.SelectedValue != null)
            {
                if (int.TryParse(cbTypes.SelectedValue.ToString(), out int selectedTypeID))
                {
                    if (selectedTypeID != -1)
                    {
                        string selectedTypeName = cbTypes.Text.Replace("'", "''");
                        filters.Add($"[SERVICE TYPE] = '{selectedTypeName}'");
                    }
                }
            }

            if (cbStatuses.SelectedValue != null)
            {
                if (byte.TryParse(cbStatuses.SelectedValue.ToString(), out byte selectedStatusID))
                {
                    if (selectedStatusID != 255)
                    {
                        string selectedStatusName = cbStatuses.Text.Replace("'", "''");
                        filters.Add($"[STATUS] = '{selectedStatusName}'");
                    }
                }
            }

            string finalFilter = string.Join(" AND ", filters);
            _dtAllApplicants.DefaultView.RowFilter = finalFilter;

            UpdateRowsCount(_dtAllApplicants);
        }

        private void tbFilterNameAppID_TextChanged(object sender, EventArgs e) => ApplyCombinedFilter();
        private void cbTypes_SelectedIndexChanged(object sender, EventArgs e) => ApplyCombinedFilter();
        private void cbStatuses_SelectedIndexChanged(object sender, EventArgs e) => ApplyCombinedFilter();

        private void UpdateRowsCount(DataTable dt)
        {
            if (dt != null)
            {
                DataView dvFiltered = dt.DefaultView;
                int pendingCount = dvFiltered.ToTable().Select("STATUS = 'New'").Length;
                int totalFiltered = dvFiltered.Count;
                lblCountTotalAndPending.Text = $"{totalFiltered} total • {pendingCount} pending";
            }
        }

        private GraphicsPath _GetRoundedRectPath(Rectangle baseRect, int radius)
        {
            int diameter = radius * 2;
            GraphicsPath path = new GraphicsPath();
            path.AddArc(baseRect.X, baseRect.Y, diameter, diameter, 180, 90);
            path.AddArc(baseRect.Right - diameter, baseRect.Y, diameter, diameter, 270, 90);
            path.AddArc(baseRect.Right - diameter, baseRect.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(baseRect.X, baseRect.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();
            return path;
        }

        private void dgvApplications_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0) return;

            // 🎯 1. عمود الـ Actions
            if (e.ColumnIndex >= 0 && dgvApplications.Columns[e.ColumnIndex].Name == "ACTIONS")
            {
                e.Paint(e.CellBounds, DataGridViewPaintParts.Background | DataGridViewPaintParts.SelectionBackground);

                System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ucApplications));
                Image imgShow = (Image)resources.GetObject("scheduleVisionTestToolStripMenuItem.Image");

                int iconSize = 20;
                int totalWidth = iconSize; // بما أنك حالياً ترسم أيقونة واحدة فقط (Show) الرسم يتمحور حولها

                int startX = e.CellBounds.Left + (e.CellBounds.Width - totalWidth) / 2;
                int startY = e.CellBounds.Top + (e.CellBounds.Height - iconSize) / 2;

                Rectangle rectShow = new Rectangle(startX, startY, iconSize, iconSize);

                // رسم تأثير الـ Hover الخلفي الدائري للأيقونة المحددة
                if (e.RowIndex == _hoveredRowIndex && e.ColumnIndex == _hoveredColumnIndex && _hoveredIconIndex == 0)
                {
                    int padding = 4;
                    Rectangle bgRect = new Rectangle(rectShow.X - padding, rectShow.Y - padding, rectShow.Width + (padding * 2), rectShow.Height + (padding * 2));
                    using (SolidBrush hoverBrush = new SolidBrush(Color.FromArgb(40, 0, 120, 215)))
                    {
                        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                        e.Graphics.FillEllipse(hoverBrush, bgRect);
                    }
                }

                if (imgShow != null)
                {
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    e.Graphics.DrawImage(imgShow, rectShow);
                }

                e.Handled = true;
            }

            // 🎯 2. عمود الـ Status البادج الملون
            if (e.ColumnIndex >= 0 && e.RowIndex >= 0 && dgvApplications.Columns[e.ColumnIndex].Name == "STATUS")
            {
                e.Paint(e.CellBounds, DataGridViewPaintParts.Background | DataGridViewPaintParts.SelectionBackground);

                if (e.Value != null)
                {
                    string status = e.Value.ToString();
                    Color badgeColor;
                    Color textColor;

                    if (status == "Completed")
                    {
                        badgeColor = Color.FromArgb(232, 245, 233);
                        textColor = Color.FromArgb(56, 142, 60);
                    }
                    else if (status == "Cancelled")
                    {
                        badgeColor = Color.FromArgb(254, 234, 234);
                        textColor = Color.FromArgb(183, 28, 28);
                    }
                    else if (status == "New")
                    {
                        badgeColor = Color.FromArgb(239, 246, 255);
                        textColor = Color.FromArgb(40, 90, 231);
                    }
                    else { return; }

                    Rectangle badgeRect = e.CellBounds;
                    badgeRect.Inflate(-6, -6);

                    using (GraphicsPath path = _GetRoundedRectPath(badgeRect, 12))
                    {
                        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                        using (SolidBrush brush = new SolidBrush(badgeColor)) { e.Graphics.FillPath(brush, path); }
                        using (Pen pen = new Pen(textColor, 1)) { e.Graphics.DrawPath(pen, path); }

                        TextRenderer.DrawText(e.Graphics, status, new Font(e.CellStyle.Font, FontStyle.Bold), badgeRect, textColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                    }
                }
                e.Handled = true;
            }
        }

        private void dgvApplications_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvApplications.Columns[e.ColumnIndex].Name == "DATE" && e.Value != null)
            {
                if (e.Value is DateTime dateValue)
                {
                    e.Value = dateValue.ToString("MMM dd, yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    e.FormattingApplied = true;
                }
            }
        }

        private void dgvApplications_Paint(object sender, PaintEventArgs e)
        {
            if (dgvApplications.Rows.Count == 0)
            {
                string noDataText = "No applications match your search.";
                using (Font font = new Font("Segoe UI", 11, FontStyle.Regular))
                using (Brush brush = new SolidBrush(Color.FromArgb(120, 144, 156)))
                {
                    Size textSize = TextRenderer.MeasureText(noDataText, font);
                    int headersHeight = dgvApplications.ColumnHeadersVisible ? dgvApplications.ColumnHeadersHeight : 0;
                    int x = (dgvApplications.Width - textSize.Width) / 2;
                    int y = headersHeight + (dgvApplications.Height - headersHeight - textSize.Height) / 3;
                    e.Graphics.DrawString(noDataText, font, brush, x, y);
                }
            }
        }

        private void btnNewApplication_Click(object sender, EventArgs e)
        {
            ucNewApplication myNewApplication = new ucNewApplication();
            myNewApplication.Dock = DockStyle.Fill;
            myNewApplication.Name = "ucNewApplicationWizard";

            foreach (Control ctrl in this.Controls)
            {
                ctrl.Visible = false;
            }

            myNewApplication.OnApplicationSaved += MyNewApplication_OnApplicationSaved;
            this.Controls.Add(myNewApplication);
            myNewApplication.BringToFront();
        }

        private void MyNewApplication_OnApplicationSaved(object sender, int ApplicationID)
        {
            Control wizardCtrl = this.Controls["ucNewApplicationWizard"];
            if (wizardCtrl != null)
            {
                this.Controls.Remove(wizardCtrl);
                wizardCtrl.Dispose();
            }

            foreach (Control ctrl in this.Controls)
            {
                ctrl.Visible = true;
            }

            _dtAllApplicants = clsApplicant.getAllApplicants();
            dgvApplications.DataSource = _dtAllApplicants;
            UpdateRowsCount(_dtAllApplicants);

            dgvApplications.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCellsExceptHeader);

            if (dgvApplications.Columns.Contains("ACTIONS"))
            {
                dgvApplications.Columns["ACTIONS"].Width = 120;
            }

            this.BringToFront();
        }

        private int _hoveredRowIndex = -1;
        private int _hoveredColumnIndex = -1;
        private int _hoveredIconIndex = -1;

        private void dgvApplications_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0 && dgvApplications.Columns[e.ColumnIndex].Name == "ACTIONS")
            {
                // 🌟 الاعتماد على الإحداثيات المحلية المارة داخل الـ CellDisplay لضمان دقة المساحة والنقر
                Rectangle cellRect = dgvApplications.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false);
                Point clickPoint = dgvApplications.PointToClient(Cursor.Position);

                int iconSize = 20;
                int startX = cellRect.Left + (cellRect.Width - iconSize) / 2;
                int startY = cellRect.Top + (cellRect.Height - iconSize) / 2;

                Rectangle rectShow = new Rectangle(startX, startY, iconSize, iconSize);
                int AppID = Convert.ToInt32(dgvApplications.Rows[e.RowIndex].Cells["  ID"].Value);

                if (rectShow.Contains(clickPoint))
                {
                    ucShowApplicationDetails myShowApplicationDetailsPage = new ucShowApplicationDetails(AppID);
                    myShowApplicationDetailsPage.Dock = DockStyle.Fill;
                    foreach (Control ctrl in this.Controls)
                    {
                        ctrl.Visible = false;
                    }
                    this.Controls.Add(myShowApplicationDetailsPage);
                    myShowApplicationDetailsPage.BringToFront();
                }
            }
        }

        private void dgvApplications_CellMouseMove(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0 && dgvApplications.Columns[e.ColumnIndex].Name == "ACTIONS")
            {
                // 🌟 الحساب المحلي المتناسق مع معادلة الـ Painting الفوقية لمنع اهتزاز الماوس
                Rectangle cellRect = dgvApplications.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false);
                Point localPoint = dgvApplications.PointToClient(Cursor.Position);

                int iconSize = 20;
                int startX = cellRect.Left + (cellRect.Width - iconSize) / 2;
                int startY = cellRect.Top + (cellRect.Height - iconSize) / 2;

                Rectangle rectShow = new Rectangle(startX, startY, iconSize, iconSize);
                int currentIconIndex = -1;

                if (rectShow.Contains(localPoint)) currentIconIndex = 0;

                if (e.RowIndex != _hoveredRowIndex || e.ColumnIndex != _hoveredColumnIndex || currentIconIndex != _hoveredIconIndex)
                {
                    _hoveredRowIndex = e.RowIndex;
                    _hoveredColumnIndex = e.ColumnIndex;
                    _hoveredIconIndex = currentIconIndex;

                    dgvApplications.Cursor = (currentIconIndex != -1) ? Cursors.Hand : Cursors.Default;
                    dgvApplications.InvalidateCell(e.ColumnIndex, e.RowIndex);
                }
            }
            else
            {
                ResetHoverEffect();
            }
        }

        private void ResetHoverEffect()
        {
            if (_hoveredRowIndex != -1 || _hoveredColumnIndex != -1 || _hoveredIconIndex != -1)
            {
                int oldRow = _hoveredRowIndex;
                int oldCol = _hoveredColumnIndex;

                _hoveredRowIndex = -1;
                _hoveredColumnIndex = -1;
                _hoveredIconIndex = -1;
                dgvApplications.Cursor = Cursors.Default;

                if (oldRow >= 0 && oldCol >= 0 && oldRow < dgvApplications.RowCount && oldCol < dgvApplications.ColumnCount)
                {
                    dgvApplications.InvalidateCell(oldCol, oldRow);
                }
            }
        }


        int appID = -1;
        private void guna2ContextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            Point mousePos = dgvApplications.PointToClient(Control.MousePosition);
            DataGridView.HitTestInfo hitInfo = dgvApplications.HitTest(mousePos.X, mousePos.Y);

            // إذا لم يضغط المستخدم فوق صف حقيقي، نلغي فتح القائمة
            if (hitInfo.RowIndex < 0)
            {
                e.Cancel = true;
                return;
            }

            // 2. جعل الصف الذي فوقه الماوس هو المختار تلقائياً
            dgvApplications.ClearSelection();
            dgvApplications.Rows[hitInfo.RowIndex].Selected = true;

            // 3. جلب الـ Status والـ ID بأمان تام وبدون أخطاء
            string status = dgvApplications.Rows[hitInfo.RowIndex].Cells["STATUS"].Value.ToString();
            appID = Convert.ToInt32(dgvApplications.Rows[hitInfo.RowIndex].Cells["  ID"].Value);
            if (status == "New")
            {
                editApplicationToolStripMenuItem.Enabled = true;
                deleteApplicationToolStripMenuItem.Enabled = true;
                cancelApplicationToolStripMenuItem.Enabled = true;
                scheduleTestsToolStripMenuItem.Enabled = true;
                issueDrivingLicenseToolStripMenuItem.Enabled = false; // مثلاً رخصة السياقة ممنوعة في الحالة الجديدة
                showLicenseToolStripMenuItem.Enabled = false;
                showPersonToolStripMenuItem.Enabled = true;
                byte passedTests = clsTestAppointment.GetPassedTestCountApplication(appID);

                switch (passedTests)
                {
                    case 0:
                        scheduleVisionTestToolStripMenuItem.Enabled = true;
                        scheduleWrittenTestToolStripMenuItem.Enabled = false;
                        scheduleStreetTestToolStripMenuItem.Enabled = false;
                        break;
                    case 1:
                        scheduleVisionTestToolStripMenuItem.Enabled = false;
                        scheduleWrittenTestToolStripMenuItem.Enabled = true;
                        scheduleStreetTestToolStripMenuItem.Enabled = false;
                        break;
                    case 2:
                        scheduleVisionTestToolStripMenuItem.Enabled = false;
                        scheduleWrittenTestToolStripMenuItem.Enabled = false;
                        scheduleStreetTestToolStripMenuItem.Enabled = true;
                        break;
                }
            }
            else if (status == "Completed")
            {
                editApplicationToolStripMenuItem.Enabled = false;
                deleteApplicationToolStripMenuItem.Enabled = false;
                cancelApplicationToolStripMenuItem.Enabled = false; // ما تنجمش تلغى حاجة مكتملة
                scheduleTestsToolStripMenuItem.Enabled = false;
                issueDrivingLicenseToolStripMenuItem.Enabled = false;
                bool hasDriverLicense = clsTestAppointment.IsRetakeTest(appID);
                showPersonToolStripMenuItem.Enabled = !hasDriverLicense;
                showLicenseToolStripMenuItem.Enabled = !hasDriverLicense;
            }
            else
            {
                editApplicationToolStripMenuItem.Enabled = false;
                deleteApplicationToolStripMenuItem.Enabled = true;
                cancelApplicationToolStripMenuItem.Enabled = false;
                scheduleTestsToolStripMenuItem.Enabled = false;
                issueDrivingLicenseToolStripMenuItem.Enabled = false;
                showLicenseToolStripMenuItem.Enabled = false;
            }
        }

        private void cancelApplicationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (appID == -1) return;

            // سؤال التأكيد قبل الإلغاء
            DialogResult result = MessageBox.Show(
                "Are you sure you want to cancel this application?",
                "Confirm Cancellation",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            // إذا اختار المستخدم "No" نخرج من الدالة بدون تنفيذ الإلغاء
            if (result == DialogResult.No) return;

            // تنفيذ عملية الإلغاء في حال الموافقة
            if (clsApplicant.UpdateToCaancelStatus(appID))
            {
                MessageBox.Show("Application Cancelled Successfully.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);

                _dtAllApplicants = clsApplicant.getAllApplicants();
                dgvApplications.DataSource = _dtAllApplicants;
                UpdateRowsCount(_dtAllApplicants);
            }
            else
            {
                MessageBox.Show("Failed to cancel application.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void showLicenseToolStripMenuItem_Click(object sender, EventArgs e)
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

                    ucShowLicense myShowLicensePage = new ucShowLicense(appID);
                    frmContainer.Size = myShowLicensePage.Size;
                    myShowLicensePage.Dock = DockStyle.Fill;
                    frmContainer.Controls.Add(myShowLicensePage);

                    Guna.UI2.WinForms.Guna2Elipse elipse = new Guna.UI2.WinForms.Guna2Elipse();
                    elipse.TargetControl = frmContainer;
                    elipse.BorderRadius = 16;

                    frmContainer.ShowDialog(overlay);
                }
            }
        }

        private void scheduleVisionTestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TestAppointment();
        }

        private void TestAppointment()
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

                    ucTypeTest myVisionTestPage = new ucTypeTest(appID);
                    myVisionTestPage.DataBack += Uc_DataBack;
                    frmContainer.Size = myVisionTestPage.Size;
                    myVisionTestPage.Dock = DockStyle.Fill;
                    frmContainer.Controls.Add(myVisionTestPage);

                    Guna.UI2.WinForms.Guna2Elipse elipse = new Guna.UI2.WinForms.Guna2Elipse();
                    elipse.TargetControl = frmContainer;
                    elipse.BorderRadius = 16;

                    frmContainer.ShowDialog(overlay);
                }
            }
        }
        private void Uc_DataBack(object sender, int testAppointmentID)
        {
            _dtAllApplicants = clsApplicant.getAllApplicants();
            dgvApplications.DataSource = _dtAllApplicants;
            UpdateRowsCount(_dtAllApplicants);
        }

        private void showPersonToolStripMenuItem_Click(object sender, EventArgs e)
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

                    ucShowPersonLicenseHistory myVisionTestPage = new ucShowPersonLicenseHistory(appID);
                    frmContainer.Size = myVisionTestPage.Size;
                    myVisionTestPage.Dock = DockStyle.Fill;
                    frmContainer.Controls.Add(myVisionTestPage);

                    Guna.UI2.WinForms.Guna2Elipse elipse = new Guna.UI2.WinForms.Guna2Elipse();
                    elipse.TargetControl = frmContainer;
                    elipse.BorderRadius = 16;

                    frmContainer.ShowDialog(overlay);
                }
            }
        }

        private void scheduleWrittenTestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TestAppointment();
        }

        private void scheduleStreetTestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TestAppointment();
        }

    }
}