using DVLD_Business;
using DVLD_BusinessLayer;
using DVLD_PresentationLayer.Drivers;
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

namespace DVLD_PresentationLayer.InternationalApplication
{
    public partial class ucInternationalApplication : UserControl
    {
        DataTable _dtInternationalLicenses = clsInternationalLicense.GetInternationalLicenses();
        private string _currentStatusFilter = "All";
        public ucInternationalApplication()
        {
            InitializeComponent();
        }
        private void _LoadDetainedLicensesData()
        {
            typeof(DataGridView).InvokeMember("DoubleBuffered",
               System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.SetProperty,
               null, dgvInternationalLicenses, new object[] { true });
            dgvInternationalLicenses.DataSource = _dtInternationalLicenses;

            if(dgvInternationalLicenses.Rows.Count > 0 )
                dgvInternationalLicenses.Columns["Application ID"].Visible = false;
            dgvInternationalLicenses.RowTemplate.DefaultCellStyle.Padding = new Padding(15, 8, 15, 8);
            dgvInternationalLicenses.RowTemplate.Height = 40;

            UpdateRowsCount(_dtInternationalLicenses);

        }

        private void ucInternationalApplication_Load(object sender, EventArgs e)
        {
            _LoadDetainedLicensesData();
        }
        private void ApplyCombinedFilter()
        {
            if (_dtInternationalLicenses == null) return;

            List<string> filters = new List<string>();

            string textSearch = tbFilterNameLicenseID.Text.Replace("'", "''").Trim();
            if (!string.IsNullOrEmpty(textSearch))
            {
                filters.Add($"([Driver Name] LIKE '%{textSearch}%' OR CONVERT([Int.License ID], 'System.String') LIKE '%{textSearch}%')");
            }

            if (_currentStatusFilter == "Active")
            {
                filters.Add("Status = 'Active'");
            }
            else if (_currentStatusFilter == "Expired")
            {
                filters.Add("Status = 'Expired'");
            }

            // 3. تطبيق الفلتر المدمج
            string finalFilter = string.Join(" AND ", filters);
            _dtInternationalLicenses.DefaultView.RowFilter = finalFilter;

            UpdateRowsCount(_dtInternationalLicenses);
        }
        private void UpdateRowsCount(DataTable dt)
        {
            DataView dvFiltered = dt.DefaultView;
            int activeCount = dvFiltered.ToTable().Select("Status = 'Active'").Length;
            int expiredCount = dvFiltered.Count - activeCount;
            lblCountActiveAndExpire.Text = $"{activeCount} active • {expiredCount} expired";
        }

        private void btnAll_Click(object sender, EventArgs e)
        {
            _currentStatusFilter = "All";
            ApplyCombinedFilter();
        }

        private void btnActive_Click(object sender, EventArgs e)
        {
            _currentStatusFilter = "Active";
            ApplyCombinedFilter();
        }

        private void btnExpired_Click(object sender, EventArgs e)
        {
            _currentStatusFilter = "Expired";
            ApplyCombinedFilter();
        }

        private void tbFilterNameLicenseID_TextChanged(object sender, EventArgs e)
        {
            ApplyCombinedFilter();
        }

        private void dgvInternationalLicenses_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                // 1. إخفاء القائمة تماماً إذا كانت الجدول فارغاً
                if (dgvInternationalLicenses.Rows.Count == 0)
                {
                    dgvInternationalLicenses.ContextMenuStrip = null;
                    return;
                }

                DataGridView.HitTestInfo hit = dgvInternationalLicenses.HitTest(e.X, e.Y);

                // 2. إذا كان النقر فوق صف فعلي (وليس Header أو مكان فارغ)
                if (hit.Type == DataGridViewHitTestType.Cell && hit.RowIndex >= 0)
                {
                    dgvInternationalLicenses.ClearSelection();
                    dgvInternationalLicenses.Rows[hit.RowIndex].Selected = true;

                    // تحديد السجل الحالي ليكون هو الصف المضغوط عليه
                    dgvInternationalLicenses.CurrentCell = dgvInternationalLicenses.Rows[hit.RowIndex].Cells[hit.ColumnIndex];

                    // إظهار القائمة الخاصة بك (تأكد من مطابقة اسم الـ ContextMenuStrip)
                    dgvInternationalLicenses.ContextMenuStrip = guna2ContextMenuStrip1;
                }
                else
                {
                    // إخفاء القائمة عند النقر في المساحة البيضاء الفارغة
                    dgvInternationalLicenses.ContextMenuStrip = null;
                }
            }
        }

        private void dgvInternationalLicenses_Paint(object sender, PaintEventArgs e)
        {
            if (dgvInternationalLicenses.Rows.Count == 0)
            {
                string noDataText = "No International License match your search.";

                // اختيار الخط واللون المناسب (رمادي هادئ ومريح للعين)
                using (Font font = new Font("Segoe UI", 11, FontStyle.Regular))
                using (Brush brush = new SolidBrush(Color.FromArgb(120, 144, 156))) // Slate Gray
                {
                    // حساب قياسات النص لتوسيطه تماماً في وسط الـ Grid
                    Size textSize = TextRenderer.MeasureText(noDataText, font);

                    // نأخذ بعين الاعتبار ارتفاع الـ Headers باش يجي النص في وسط المساحة البيضاء بالظبط
                    int headersHeight = dgvInternationalLicenses.ColumnHeadersVisible ? dgvInternationalLicenses.ColumnHeadersHeight : 0;

                    int x = (dgvInternationalLicenses.Width - textSize.Width) / 2;
                    int y = headersHeight + (dgvInternationalLicenses.Height - headersHeight - textSize.Height) / 3;

                    // رسم النص
                    e.Graphics.DrawString(noDataText, font, brush, x, y);
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

        private void dgvInternationalLicenses_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            // التأكد من وجود قيم قبل التحويل لمنع الـ NullReferenceException
            if (dgvInternationalLicenses.Rows[e.RowIndex].Cells["Detain ID"].Value == DBNull.Value ||
                dgvInternationalLicenses.Rows[e.RowIndex].Cells["Detain ID"].Value == null)
                return;

            int detainID = Convert.ToInt32(dgvInternationalLicenses.Rows[e.RowIndex].Cells["Detain ID"].Value);
            int licenseID = Convert.ToInt32(dgvInternationalLicenses.Rows[e.RowIndex].Cells["License ID"].Value);

            // عند الضغط على زر Release
            if (dgvInternationalLicenses.Columns[e.ColumnIndex].Name == "btnRelease")
            {
                string status = dgvInternationalLicenses.Rows[e.RowIndex].Cells["Status"].Value?.ToString();
                if (status == "Detained")
                {
                    // frmReleaseDetainedLicense frm = new frmReleaseDetainedLicense(licenseID);
                    // frm.ShowDialog();

                    _LoadDetainedLicensesData();
                }
            }

            // عند الضغط على زر View Details
            if (dgvInternationalLicenses.Columns[e.ColumnIndex].Name == "btnViewDetails")
            {
                // frmShowLicenseInfo frm = new frmShowLicenseInfo(licenseID);
                // frm.ShowDialog();
            }
        }

        private void dgvInternationalLicenses_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            string columnName = dgvInternationalLicenses.Columns[e.ColumnIndex].Name;

            // 1. رسم الـ Status كـ Badge/Pill محاذى على اليسار تحت العنوان مباشرة
            if (columnName == "Status" && e.Value != null)
            {
                e.PaintBackground(e.CellBounds, true);

                string status = e.Value.ToString();

                Color backColor = Color.Empty;
                Color textColor = Color.Empty;

                if (status == "Expired")
                {
                    backColor = Color.FromArgb(254, 226, 226); // Soft Red
                    textColor = Color.FromArgb(185, 28, 28);   // Dark Red
                }
                else if (status == "Active")
                {
                    backColor = Color.FromArgb(220, 252, 231); // Soft Green
                    textColor = Color.FromArgb(21, 128, 61);   // Dark Green
                }

                if (backColor != Color.Empty)
                {
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                    int badgeWidth = 95;
                    int badgeHeight = 26;

                    // 💡 تغيير الإحداثيات: المحاذاة على اليسار مع مسافة جانبية (Margin) قدرها 10 بكسل
                    int x = e.CellBounds.X + 10;
                    int y = e.CellBounds.Y + (e.CellBounds.Height - badgeHeight) / 2;

                    Rectangle badgeRect = new Rectangle(x, y, badgeWidth, badgeHeight);

                    using (GraphicsPath path = _GetRoundedRectPath(badgeRect, 12))
                    using (SolidBrush bgBrush = new SolidBrush(backColor))
                    {
                        e.Graphics.FillPath(bgBrush, path);
                    }

                    TextRenderer.DrawText(
                        e.Graphics,
                        status,
                        new Font("Segoe UI", 9.5f, FontStyle.Bold),
                        badgeRect,
                        textColor,
                        TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter
                    );
                }

                e.Handled = true;
            }

            // 2. معالجة زر الإفراج btnRelease
            if (columnName == "btnActive")
            {
                string status = dgvInternationalLicenses.Rows[e.RowIndex].Cells["Status"].Value?.ToString();

                if (status == "Active")
                {
                    e.PaintBackground(e.CellBounds, true);
                    e.Handled = true;
                }
            }
        }

        private void showPersonToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dgvInternationalLicenses.CurrentRow == null || dgvInternationalLicenses.CurrentRow.Index < 0) return;
            int appID = Convert.ToInt32(dgvInternationalLicenses.CurrentRow.Cells["Application ID"].Value);
            
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
    }
}
