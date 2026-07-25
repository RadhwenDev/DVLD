using DVLD_BusinessLayer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DVLD_PresentationLayer.Drivers
{
    public partial class ucShowPersonLicenseHistory : UserControl
    {
        int _AppID = -1;

        public ucShowPersonLicenseHistory(int AppID)
        {
            InitializeComponent();
            this._AppID = AppID;
        }

        private void ucShowPersonLicenseHistory_Load(object sender, EventArgs e)
        {
            // 1. تفعيل التنسيق الخاص بالـ DataGridView
            SetupCustomDataGridViewL();
            SetupCustomDataGridViewI();

            // 2. تحميل بيانات السائق والـ License History
            DataTable dt = clsDriver.getLicenseHistory(_AppID);

            if (dt != null && dt.Rows.Count > 0)
            {
                string imagePath = dt.Rows[0]["ImagePath"].ToString();

                if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
                {
                    try
                    {
                        using (var stream = new FileStream(imagePath, FileMode.Open, FileAccess.Read))
                        {
                            pbImageDriver.Image = System.Drawing.Image.FromStream(stream);
                        }
                    }
                    catch { }
                }

                lblPersonID.Text = dt.Rows[0]["PersonID"].ToString();
                lblName.Text = dt.Rows[0]["FullName"].ToString();
                lblNationalNo.Text = dt.Rows[0]["NationalNo"].ToString();

                if (DateTime.TryParse(dt.Rows[0]["DateOfBirth"].ToString(), out DateTime birthDate))
                    lblDoB.Text = birthDate.ToString("MMM dd, yyyy", CultureInfo.InvariantCulture);
                else
                    lblDoB.Text = dt.Rows[0]["DateOfBirth"].ToString();

                lblGender.Text = dt.Rows[0]["Gender"].ToString();
                lblPhone.Text = dt.Rows[0]["Phone"].ToString();
                lblEmail.Text = dt.Rows[0]["Email"].ToString();
                lblCountry.Text = dt.Rows[0]["CountryName"].ToString();
                lblAddress.Text = dt.Rows[0]["Address"].ToString();
            }

            // 3. ربط الجدول بالبيانات
            DataTable dgvL = clsDriver.getLocalLicenseHistory(_AppID);
            if (dgvL != null && dgvL.Rows.Count > 0)
            {
                dgvLocalLicense.DataSource = dgvL;
                dgvLocalLicense.Columns["Class Name"].Width = 220;
                dgvLocalLicense.Columns["Issue Date"].Width = 120;
                dgvLocalLicense.Columns["Expiration Date"].Width = 120;
                dgvLocalLicense.Columns["App.ID"].Width = 100;
                dgvLocalLicense.Columns["Lic.ID"].Width = 100;
            }

            DataTable dgvI = clsDriver.getInternationalLicenseHistory(_AppID);
            if (dgvI != null && dgvI.Rows.Count > 0)
            {
                dgvInternationalLicense.DataSource = dgvI;
            }
        }

        private void SetupCustomDataGridViewI()
        {
            // تسريع الرسم ومطبقة الـ DoubleBuffer لمنع التقطيع
            PropertyInfo pi = typeof(DataGridView).GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            pi?.SetValue(dgvInternationalLicense, true, null);

            // إلغاء الحدود التقليدية وشريط الأسطر الجانبي
            dgvInternationalLicense.BorderStyle = BorderStyle.None;
            dgvInternationalLicense.CellBorderStyle = DataGridViewCellBorderStyle.None;
            dgvInternationalLicense.RowHeadersVisible = false;
            dgvInternationalLicense.EnableHeadersVisualStyles = false;
            dgvInternationalLicense.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvInternationalLicense.AllowUserToAddRows = false;
            // خلفية الجدول
            dgvInternationalLicense.BackgroundColor = Color.FromArgb(240, 244, 248);

            // تنسيق الهيدر (Header)
            dgvInternationalLicense.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgvInternationalLicense.ColumnHeadersHeight = 40;
            dgvInternationalLicense.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(240, 244, 248);
            dgvInternationalLicense.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(15, 45, 110);
            dgvInternationalLicense.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dgvInternationalLicense.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            // تنسيق الأسطر (Rows)
            dgvInternationalLicense.RowTemplate.Height = 45;
            dgvInternationalLicense.DefaultCellStyle.BackColor = Color.FromArgb(228, 236, 247);
            dgvInternationalLicense.DefaultCellStyle.ForeColor = Color.FromArgb(40, 40, 40);
            dgvInternationalLicense.DefaultCellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);
            dgvInternationalLicense.DefaultCellStyle.SelectionBackColor = Color.FromArgb(210, 224, 242);
            dgvInternationalLicense.DefaultCellStyle.SelectionForeColor = Color.Black;
        }
        private void SetupCustomDataGridViewL()
        {
            // تسريع الرسم ومطبقة الـ DoubleBuffer لمنع التقطيع
            PropertyInfo pi = typeof(DataGridView).GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            pi?.SetValue(dgvLocalLicense, true, null);

            // إلغاء الحدود التقليدية وشريط الأسطر الجانبي
            dgvLocalLicense.BorderStyle = BorderStyle.None;
            dgvLocalLicense.CellBorderStyle = DataGridViewCellBorderStyle.None;
            dgvLocalLicense.RowHeadersVisible = false;
            dgvLocalLicense.EnableHeadersVisualStyles = false;
            dgvLocalLicense.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvLocalLicense.AllowUserToAddRows = false;
            // خلفية الجدول
            dgvLocalLicense.BackgroundColor = Color.FromArgb(240, 244, 248);

            // تنسيق الهيدر (Header)
            dgvLocalLicense.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgvLocalLicense.ColumnHeadersHeight = 40;
            dgvLocalLicense.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(240, 244, 248);
            dgvLocalLicense.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(15, 45, 110);
            dgvLocalLicense.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dgvLocalLicense.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            // تنسيق الأسطر (Rows)
            dgvLocalLicense.RowTemplate.Height = 45;
            dgvLocalLicense.DefaultCellStyle.BackColor = Color.FromArgb(228, 236, 247);
            dgvLocalLicense.DefaultCellStyle.ForeColor = Color.FromArgb(40, 40, 40);
            dgvLocalLicense.DefaultCellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);
            dgvLocalLicense.DefaultCellStyle.SelectionBackColor = Color.FromArgb(210, 224, 242);
            dgvLocalLicense.DefaultCellStyle.SelectionForeColor = Color.Black;
        }

        private void dgvLocalLicense_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            // رسم الأسطر فقط (تجاهل الهيدر)
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                e.Handled = true; // نتحكم في الرسم يدوياً

                // 1. رسم خلفية الجدول الأصلية
                using (Brush backBrush = new SolidBrush(dgvLocalLicense.BackgroundColor))
                {
                    e.Graphics.FillRectangle(backBrush, e.CellBounds);
                }

                // 2. تحديد ابعاد الكارت الداخلي
                Rectangle cardBounds = new Rectangle(
                    e.CellBounds.X,
                    e.CellBounds.Y + 3,
                    e.CellBounds.Width,
                    e.CellBounds.Height - 6
                );

                bool isSelected = dgvLocalLicense.Rows[e.RowIndex].Selected;
                Color cardColor = isSelected ? Color.FromArgb(210, 224, 242) : Color.FromArgb(228, 236, 247);

                using (Brush cardBrush = new SolidBrush(cardColor))
                {
                    e.Graphics.FillRectangle(cardBrush, cardBounds);
                }

                // 3. فحص ما إذا كانت الخلية هي Is Active لرسم الـ Toggle Switch
                string columnName = dgvLocalLicense.Columns[e.ColumnIndex].Name;
                string headerText = dgvLocalLicense.Columns[e.ColumnIndex].HeaderText;

                if (columnName.Equals("IsActive", StringComparison.OrdinalIgnoreCase) ||
                    headerText.Equals("Is Active", StringComparison.OrdinalIgnoreCase))
                {
                    bool isActive = false;
                    if (e.Value != null && e.Value != DBNull.Value)
                    {
                        isActive = Convert.ToBoolean(e.Value);
                    }
                    DrawToggleSwitch(e.Graphics, cardBounds, isActive);
                }
                else
                {
                    // رسم النصوص العادية متناسقة في الوسط
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

        private void DrawToggleSwitch(Graphics g, Rectangle bounds, bool isActive)
        {
            // 1. الأبعاد المستهدفة لتطابق الصورة
            int switchWidth = 44;   // تكبير العرض
            int switchHeight = 22;  // تكبير الارتفاع

            // 2. حساب المركز لتكون في منتصف الخلية تماماً عمودياً وأفقياً
            int x = bounds.X + (bounds.Width - switchWidth) / 4;
            int y = bounds.Y + (bounds.Height - switchHeight) / 2;

            Rectangle switchRect = new Rectangle(x, y, switchWidth, switchHeight);

            // تفعيل تنعيم الحواف (Anti-aliasing) لرسم انسيابي وبدون زوايا حادة
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // 3. تحديد اللون المطابق للصورة (أخضر زاهي عند التشغيل، رمادي هادئ عند الإيقاف)
            Color toggleBg = isActive ? Color.FromArgb(46, 172, 89) : Color.FromArgb(190, 200, 210);

            using (Brush bgBrush = new SolidBrush(toggleBg))
            {
                // رسم الخلفية المنحنية
                using (System.Drawing.Drawing2D.GraphicsPath path = GetRoundedPath(switchRect, switchHeight))
                {
                    g.FillPath(bgBrush, path);
                }
            }

            // 4. رسم الدائرة البيضاء داخل الـ Toggle
            int circleMargin = 2;
            int circleSize = switchHeight - (circleMargin * 2); // حجم الدائرة بناءً على الارتفاع

            // مكان الدائرة (يمين إذا كان Active، يسار إذا كان Inactive)
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

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.FindForm()?.Close();
        }

        private void dgvLocalLicense_Paint(object sender, PaintEventArgs e)
        {
            if (dgvLocalLicense.Rows.Count == 0)
            {
                string noDataText = "No Local License scheduled.";
                using (Font font = new Font("Segoe UI", 11, FontStyle.Regular))
                using (Brush brush = new SolidBrush(Color.FromArgb(120, 144, 156)))
                {
                    Size textSize = TextRenderer.MeasureText(noDataText, font);
                    int headersHeight = dgvLocalLicense.ColumnHeadersVisible ? dgvLocalLicense.ColumnHeadersHeight : 0;
                    int x = (dgvLocalLicense.Width - textSize.Width) / 2;
                    int y = headersHeight + (dgvLocalLicense.Height - headersHeight - textSize.Height) / 2;
                    e.Graphics.DrawString(noDataText, font, brush, x, y);
                }
            }
        }

        private void dgvInternationalLicense_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                e.Handled = true; // نتحكم في الرسم يدوياً

                // 1. رسم خلفية الجدول الأصلية
                using (Brush backBrush = new SolidBrush(dgvInternationalLicense.BackgroundColor))
                {
                    e.Graphics.FillRectangle(backBrush, e.CellBounds);
                }

                // 2. تحديد ابعاد الكارت الداخلي
                Rectangle cardBounds = new Rectangle(
                    e.CellBounds.X,
                    e.CellBounds.Y + 3,
                    e.CellBounds.Width,
                    e.CellBounds.Height - 6
                );

                bool isSelected = dgvInternationalLicense.Rows[e.RowIndex].Selected;
                Color cardColor = isSelected ? Color.FromArgb(210, 224, 242) : Color.FromArgb(228, 236, 247);

                using (Brush cardBrush = new SolidBrush(cardColor))
                {
                    e.Graphics.FillRectangle(cardBrush, cardBounds);
                }

                // 3. فحص ما إذا كانت الخلية هي Is Active لرسم الـ Toggle Switch
                string columnName = dgvInternationalLicense.Columns[e.ColumnIndex].Name;
                string headerText = dgvInternationalLicense.Columns[e.ColumnIndex].HeaderText;

                if (columnName.Equals("IsActive", StringComparison.OrdinalIgnoreCase) ||
                    headerText.Equals("Is Active", StringComparison.OrdinalIgnoreCase))
                {
                    bool isActive = false;
                    if (e.Value != null && e.Value != DBNull.Value)
                    {
                        isActive = Convert.ToBoolean(e.Value);
                    }
                    DrawToggleSwitch(e.Graphics, cardBounds, isActive);
                }
                else
                {
                    // رسم النصوص العادية متناسقة في الوسط
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

        private void dgvInternationalLicense_Paint(object sender, PaintEventArgs e)
        {
            if (dgvInternationalLicense.Rows.Count == 0)
            {
                string noDataText = "No International License scheduled.";
                using (Font font = new Font("Segoe UI", 11, FontStyle.Regular))
                using (Brush brush = new SolidBrush(Color.FromArgb(120, 144, 156)))
                {
                    Size textSize = TextRenderer.MeasureText(noDataText, font);
                    int headersHeight = dgvInternationalLicense.ColumnHeadersVisible ? dgvInternationalLicense.ColumnHeadersHeight : 0;
                    int x = (dgvInternationalLicense.Width - textSize.Width) / 2;
                    int y = headersHeight + (dgvInternationalLicense.Height - headersHeight - textSize.Height) / 2;
                    e.Graphics.DrawString(noDataText, font, brush, x, y);
                }
            }
        }
    }
}