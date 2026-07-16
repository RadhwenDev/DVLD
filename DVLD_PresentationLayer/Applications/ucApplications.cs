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

        private void ucApplications_Load(object sender, EventArgs e)
        {
            typeof(DataGridView).InvokeMember("DoubleBuffered",
               System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.SetProperty,
               null, dgvApplications, new object[] { true });

            DataTable _dtAllApplicants = clsApplicant.getAllApplicants();
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
                // 💎 تصفير الحدود تماماً من الإعدادات الأساسية
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

            // ضبط الارتفاع ليتناسب مع البادينغ الجديد
            dgvApplications.RowTemplate.Height = 40;

            dgvApplications.CellBorderStyle = DataGridViewCellBorderStyle.None;
            dgvApplications.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;

            dgvApplications.DefaultCellStyle.SelectionBackColor = Color.FromArgb(243, 244, 246); // رمادي خفيف جداً
            dgvApplications.DefaultCellStyle.SelectionForeColor = Color.Black;

            // أو إذا كنت تريد التخلص من الفواصل تماماً لتبدو أرقاماً صحيحة نظيفة (مثال: 20)
            dgvApplications.Columns["FEES PAID"].DefaultCellStyle.Format = "N0";


            // ربط حد
            UpdateRowsCount(_dtAllApplicants);
        }

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
            if (e.ColumnIndex >= 0 && e.RowIndex >= 0 && dgvApplications.Columns[e.ColumnIndex].Name == "STATUS")
            {
                // 🌟 تعديل: رسم الخلفية بدون الحواف والحدود الافتراضية للـ Grid
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
                        textColor = Color.FromArgb(40, 90, 231);    // 💡 تم تعديل لون الكومنت هنا للأزرق
                    }
                    else { return; }

                    // حساب أبعاد الـ Badge مع موازنة المسافات العمودية والأفقية
                    Rectangle badgeRect = e.CellBounds;
                    badgeRect.Inflate(-6, -6); // زيادة الفراغ قليلاً ليفصل الـ Badge عن حدود السطر تماماً

                    using (GraphicsPath path = _GetRoundedRectPath(badgeRect, 12))
                    {
                        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                        // 1. ملء الـ Badge باللون الباهت
                        using (SolidBrush brush = new SolidBrush(badgeColor))
                        { e.Graphics.FillPath(brush, path); }

                        // 2. رسم الحافة (Border)
                        using (Pen pen = new Pen(textColor, 1))
                        { e.Graphics.DrawPath(pen, path); }

                        // 3. رسم النص
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
                    // هنا نجبر الخلية على طباعة التاريخ باللغة الإنجليزية القياسية مهما كانت لغة الجهاز
                    e.Value = dateValue.ToString("MMM dd, yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    e.FormattingApplied = true; // إعلام نظام الـ Grid بأنه تم تطبيق التنسيق بنجاح
                }
            }
        }

        private void tbFilterNameAppID_TextChanged(object sender, EventArgs e)
        {
            if (dgvApplications.DataSource is DataTable dt)
            {
                dt.DefaultView.RowFilter = string.Format("APPLICANT LIKE '%{0}%' OR CONVERT([  ID], 'System.String') LIKE '%{0}%'", tbFilterNameAppID.Text.Replace("'", "''"));
                UpdateRowsCount(dt);
            }
        }

        private void dgvApplications_Paint(object sender, PaintEventArgs e)
        {
            if (dgvApplications.Rows.Count == 0)
            {
                string noDataText = "No applications match your search.";

                // اختيار الخط واللون المناسب (رمادي هادئ ومريح للعين)
                using (Font font = new Font("Segoe UI", 11, FontStyle.Regular))
                using (Brush brush = new SolidBrush(Color.FromArgb(120, 144, 156))) // Slate Gray
                {
                    // حساب قياسات النص لتوسيطه تماماً في وسط الـ Grid
                    Size textSize = TextRenderer.MeasureText(noDataText, font);

                    // نأخذ بعين الاعتبار ارتفاع الـ Headers باش يجي النص في وسط المساحة البيضاء بالظبط
                    int headersHeight = dgvApplications.ColumnHeadersVisible ? dgvApplications.ColumnHeadersHeight : 0;

                    int x = (dgvApplications.Width - textSize.Width) / 2;
                    int y = headersHeight + (dgvApplications.Height - headersHeight - textSize.Height) / 3;

                    // رسم النص
                    e.Graphics.DrawString(noDataText, font, brush, x, y);
                }
            }
        }
    }
}
