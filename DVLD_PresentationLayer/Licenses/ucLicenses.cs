using DVLD_BusinessLayer;
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

namespace DVLD_PresentationLayer.Licenses
{
    public partial class ucLicenses : UserControl
    {
        public ucLicenses()
        {
            InitializeComponent();
        }

        DataTable _dtAllLicenses = clsLicenses.getAllLicenses();

        // 🌟 متغير لحفظ حالة الفلتر الحالية الخاصة بالـ Buttons
        // القيم الممكنة: "All", "Active", "Expired"
        private string _currentStatusFilter = "All";

        private void ucLicenses_Load(object sender, EventArgs e)
        {
            typeof(DataGridView).InvokeMember("DoubleBuffered",
               System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.SetProperty,
               null, dgvLicenses, new object[] { true });

            dgvLicenses.DataSource = _dtAllLicenses;

            if (dgvLicenses.Columns.Count > 0)
            {
                dgvLicenses.Columns["LICENSE ID"].DefaultCellStyle.Padding = new Padding(10, 0, 0, 0);
                dgvLicenses.CellBorderStyle = DataGridViewCellBorderStyle.None;
                dgvLicenses.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
                dgvLicenses.RowHeadersVisible = false;
                dgvLicenses.Columns["License ID"].Width = 100;
                dgvLicenses.Columns["DRIVER"].Width = 200;
                dgvLicenses.Columns["CLASS"].Width = 200;
                dgvLicenses.Columns["ISSUE DATE"].DefaultCellStyle.Format = "MMM dd, yyyy";
                dgvLicenses.Columns["EXPIRATION"].DefaultCellStyle.Format = "MMM dd, yyyy";
                dgvLicenses.Columns["REASON"].Width = 100;
                dgvLicenses.Columns["STATUS"].Width = 100;
            }
            dgvLicenses.RowTemplate.DefaultCellStyle.Padding = new Padding(15, 8, 15, 8);
            dgvLicenses.RowTemplate.Height = 40;

            UpdateRowsCount(_dtAllLicenses);
        }

        private void dgvLicenses_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if ((dgvLicenses.Columns[e.ColumnIndex].Name == "ISSUE DATE" || dgvLicenses.Columns[e.ColumnIndex].Name == "EXPIRATION") && e.Value != null)
            {
                if (e.Value is DateTime dateValue)
                {
                    e.Value = dateValue.ToString("MMM dd, yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    e.FormattingApplied = true;
                }
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

        private void dgvLicenses_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.ColumnIndex >= 0 && e.RowIndex >= 0 && dgvLicenses.Columns[e.ColumnIndex].Name == "STATUS")
            {
                e.Paint(e.CellBounds, DataGridViewPaintParts.Background | DataGridViewPaintParts.SelectionBackground);

                if (e.Value != null)
                {
                    string status = e.Value.ToString();
                    Color badgeColor;
                    Color textColor;

                    if (status == "Active")
                    {
                        badgeColor = Color.FromArgb(232, 245, 233);
                        textColor = Color.FromArgb(56, 142, 60);
                    }
                    else if (status == "Expired")
                    {
                        badgeColor = Color.FromArgb(254, 234, 234);
                        textColor = Color.FromArgb(183, 28, 28);
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

        // 🌟 الميثود الموحدة لدمج فلتر الـ Text وفلتر الـ Status
        private void ApplyCombinedFilter()
        {
            if (_dtAllLicenses == null) return;

            List<string> filters = new List<string>();

            // 1. معالجة فلتر الـ TextBox
            string textSearch = tbFilterNameLicenseID.Text.Replace("'", "''").Trim();
            if (!string.IsNullOrEmpty(textSearch))
            {
                filters.Add($"(DRIVER LIKE '%{textSearch}%' OR CONVERT([LICENSE ID], 'System.String') LIKE '%{textSearch}%')");
            }

            // 2. معالجة فلتر الـ Status (بناءً على الزر المضغوط)
            if (_currentStatusFilter == "Active")
            {
                filters.Add("STATUS = 'Active'");
            }
            else if (_currentStatusFilter == "Expired")
            {
                filters.Add("STATUS = 'Expired'");
            }
            // لو كان "All" ما نزيدو شيء للـ Filter الخاص بالـ Status

            // 3. دمج الفلاتر بـ AND وتطبيقها
            string finalFilter = string.Join(" AND ", filters);
            _dtAllLicenses.DefaultView.RowFilter = finalFilter;

            UpdateRowsCount(_dtAllLicenses);
        }

        private void tbFilterNameLicenseID_TextChanged(object sender, EventArgs e)
        {
            // عند تغيير النص، نطبق الفلتر المشترك مباشرة
            ApplyCombinedFilter();
        }

        private void UpdateRowsCount(DataTable dt)
        {
            if (dt != null)
            {
                DataView dvFiltered = dt.DefaultView;
                int activeCount = dvFiltered.ToTable().Select("STATUS = 'Active'").Length;
                int expiredCount = dvFiltered.Count - activeCount;
                lblCountActiveAndExpire.Text = $"{activeCount} active • {expiredCount} expired";
            }
        }

        // 🌟 تعديل الأزرار لتحديث الحالة فقط واستدعاء الفلتر المشترك مع حماية النتيجة الفارغة

        private void btnAll_Click(object sender, EventArgs e)
        {
            // إذا كان التكست مكتوب فيه والنتيجة فارغة أصلاً، نمنع تفاعل الزر
            if (!string.IsNullOrEmpty(tbFilterNameLicenseID.Text) && dgvLicenses.Rows.Count == 0)
                return;

            _currentStatusFilter = "All";
            ApplyCombinedFilter();
        }

        private void btnActive_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(tbFilterNameLicenseID.Text) && dgvLicenses.Rows.Count == 0)
                return;

            _currentStatusFilter = "Active";
            ApplyCombinedFilter();
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(tbFilterNameLicenseID.Text) && dgvLicenses.Rows.Count == 0)
                return;

            _currentStatusFilter = "Expired";
            ApplyCombinedFilter();
        }

        private void dgvLicenses_Paint(object sender, PaintEventArgs e)
        {
            if (dgvLicenses.Rows.Count == 0)
            {
                string noDataText = "No licenses match your search.";

                // اختيار الخط واللون المناسب (رمادي هادئ ومريح للعين)
                using (Font font = new Font("Segoe UI", 11, FontStyle.Regular))
                using (Brush brush = new SolidBrush(Color.FromArgb(120, 144, 156))) // Slate Gray
                {
                    // حساب قياسات النص لتوسيطه تماماً في وسط الـ Grid
                    Size textSize = TextRenderer.MeasureText(noDataText, font);

                    // نأخذ بعين الاعتبار ارتفاع الـ Headers باش يجي النص في وسط المساحة البيضاء بالظبط
                    int headersHeight = dgvLicenses.ColumnHeadersVisible ? dgvLicenses.ColumnHeadersHeight : 0;

                    int x = (dgvLicenses.Width - textSize.Width) / 2;
                    int y = headersHeight + (dgvLicenses.Height - headersHeight - textSize.Height) / 3;

                    // رسم النص
                    e.Graphics.DrawString(noDataText, font, brush, x, y);
                }
            }
        }
    }
}