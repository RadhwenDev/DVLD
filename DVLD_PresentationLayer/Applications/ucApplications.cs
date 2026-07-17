using DVLD_BusinessLayer;
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

        // 🌟 حفظ الجدول الأصلي على مستوى الكلاس لتسهيل الفلترة المشتركة
        private DataTable _dtAllApplicants;

        private void ucApplications_Load(object sender, EventArgs e)
        {
            typeof(DataGridView).InvokeMember("DoubleBuffered",
               System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.SetProperty,
               null, dgvApplications, new object[] { true });

            // تعبئة الجدول الأصلي
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

            // 1. تعبئة الـ ComboBox الخاص بالحالات (Statuses) مع "All Status" بأمان
            DataTable dtStatus = clsApplicant.getAllApplicationStatus();
            dtStatus.Columns["STATUS"].MaxLength = -1; // إلغاء تحديد الحد الأقصى للحروف لتفادي الـ Exception
            DataRow defaultRowStatus = dtStatus.NewRow();
            defaultRowStatus["STATUS"] = "All Status";
            defaultRowStatus["StatusID"] = (byte)255; // استخدام 255 كقيمة افتراضية للـ byte
            dtStatus.Rows.InsertAt(defaultRowStatus, 0);

            cbStatuses.DataSource = dtStatus;
            cbStatuses.DisplayMember = "STATUS";
            cbStatuses.ValueMember = "StatusID";

            // 2. تعبئة الـ ComboBox بالأنواع مع سطر "All Types"
            DataTable dtTypes = clsApplicant.getAllApplicationTypes();
            DataRow defaultRow = dtTypes.NewRow();
            defaultRow["ApplicationTypeTitle"] = "All Types";
            defaultRow["ApplicationTypeID"] = -1;
            dtTypes.Rows.InsertAt(defaultRow, 0);

            cbTypes.DataSource = dtTypes;
            cbTypes.DisplayMember = "ApplicationTypeTitle";
            cbTypes.ValueMember = "ApplicationTypeID";

            // ربط الـ Paint Event يدوياً لضمان ظهور رسالة "No Data"
            dgvApplications.Paint += dgvApplications_Paint;

            // 🌟 ربط الـ Events الخاصة بتغيير الاختيارات للـ ComboBoxes يدوياً
            cbTypes.SelectedIndexChanged += cbTypes_SelectedIndexChanged;
            cbStatuses.SelectedIndexChanged += cbStatuses_SelectedIndexChanged;

            UpdateRowsCount(_dtAllApplicants);
        }

        // 🌟 الميثود السحرية الموحدة والمطورة لدمج الفلاتر الثلاثة (TextBox + Types + Status)
        private void ApplyCombinedFilter()
        {
            if (_dtAllApplicants == null) return;

            List<string> filters = new List<string>();

            // 1️⃣ فلتر الـ TextBox (البحث بالاسم أو الـ ID)
            string textSearch = tbFilterNameAppID.Text.Replace("'", "''").Trim();
            if (!string.IsNullOrEmpty(textSearch))
            {
                filters.Add($"(APPLICANT LIKE '%{textSearch}%' OR CONVERT([  ID], 'System.String') LIKE '%{textSearch}%')");
            }

            // 2️⃣ فلتر الـ ComboBox (نوع الخدمة - cbTypes)
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

            // 3️⃣ فلتر الـ ComboBox الجديد (الحالة - cbStatuses)
            if (cbStatuses.SelectedValue != null)
            {
                if (byte.TryParse(cbStatuses.SelectedValue.ToString(), out byte selectedStatusID))
                {
                    // 255 تعني "All Status" (تخطي الفلترة للـ Status)
                    if (selectedStatusID != 255)
                    {
                        string selectedStatusName = cbStatuses.Text.Replace("'", "''");
                        filters.Add($"[STATUS] = '{selectedStatusName}'");
                    }
                }
            }

            // 4️⃣ دمج الفلاتر النشطة بـ AND وتطبيقها على الـ DefaultView
            string finalFilter = string.Join(" AND ", filters);
            _dtAllApplicants.DefaultView.RowFilter = finalFilter;

            // تحديث الـ Grid وحساب الأعداد الجديدة للسطور المفلترة
            UpdateRowsCount(_dtAllApplicants);
        }

        private void tbFilterNameAppID_TextChanged(object sender, EventArgs e)
        {
            ApplyCombinedFilter();
        }

        private void cbTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyCombinedFilter();
        }

        // 🌟 الـ Event الجديد لتحديث الفلتر فور تغيير الـ Status
        private void cbStatuses_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyCombinedFilter();
        }

        private void UpdateRowsCount(DataTable dt)
        {
            if (dt != null)
            {
                DataView dvFiltered = dt.DefaultView;
                // حساب الـ New/Pending بناءً على الفلترة الحالية النشطة
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

                        using (SolidBrush brush = new SolidBrush(badgeColor))
                        { e.Graphics.FillPath(brush, path); }

                        using (Pen pen = new Pen(textColor, 1))
                        { e.Graphics.DrawPath(pen, path); }

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

        private void showUserControl(UserControl userControl)
        {
            this.Controls.Clear();

            userControl.Dock = DockStyle.Fill;

            this.Controls.Add(userControl);

            userControl.BringToFront();
        }

        private void btnNewApplication_Click(object sender, EventArgs e)
        {
            ucNewApplication myNewApplication = new ucNewApplication();
            showUserControl(myNewApplication);
        }

        
    }
}