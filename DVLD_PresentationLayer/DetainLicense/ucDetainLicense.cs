using DVLD_Business;
using DVLD_BusinessLayer;
using DVLD_PresentationLayer.Global;
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

namespace DVLD_PresentationLayer.DetainLicense
{
    public partial class ucDetainLicense : UserControl
    {
        DataTable _dtDetainedLicenses = clsDetainedLicense.GetAllDetainedLicenses();
        private string _currentStatusFilter = "All";

        public ucDetainLicense()
        {
            InitializeComponent();
        }
        private void _LoadDetainedLicensesData()
        {
            typeof(DataGridView).InvokeMember("DoubleBuffered",
               System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.SetProperty,
               null, dgvDetainedLicenses, new object[] { true });
            dgvDetainedLicenses.DataSource = _dtDetainedLicenses;
            
            dgvDetainedLicenses.RowTemplate.DefaultCellStyle.Padding = new Padding(15, 8, 15, 8);
            dgvDetainedLicenses.RowTemplate.Height = 40;

            UpdateRowsCount(_dtDetainedLicenses);

        }
        private void ApplyCombinedFilter()
        {
            if (_dtDetainedLicenses == null) return;

            List<string> filters = new List<string>();

            // 1. معالجة البحث بالنص (Detain ID, License ID, Driver Name)
            string textSearch = tbFilterNameLicenseID.Text.Replace("'", "''").Trim();
            if (!string.IsNullOrEmpty(textSearch))
            {
                filters.Add($"([Driver Name] LIKE '%{textSearch}%' OR CONVERT([License ID], 'System.String') LIKE '%{textSearch}%' OR CONVERT([Detain ID], 'System.String') LIKE '%{textSearch}%')");
            }

            // 2. معالجة الفلترة بأزرار الحالة (All, Released, Detained)
            if (_currentStatusFilter == "Released")
            {
                filters.Add("Status = 'Released'");
            }
            else if (_currentStatusFilter == "Detained")
            {
                filters.Add("Status = 'Detained'");
            }

            // 3. تطبيق الفلتر المدمج
            string finalFilter = string.Join(" AND ", filters);
            _dtDetainedLicenses.DefaultView.RowFilter = finalFilter;

            UpdateRowsCount(_dtDetainedLicenses);
        }
        private void UpdateRowsCount(DataTable dt)
        {            
            DataView dvFiltered = dt.DefaultView;
            int detainCount = dvFiltered.ToTable().Select("Status = 'Detained'").Length;
            int releasedCount = dvFiltered.Count - detainCount;
            lblTotalRecords.Text = $"{detainCount} currently detained • {releasedCount} released";
        }
        
        private void ucDetainLicense_Load(object sender, EventArgs e)
        {
            _LoadDetainedLicensesData();
        }

        private void dgvDetainedLicenses_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvDetainedLicenses.Columns[e.ColumnIndex].Name == "Fine Fees" && e.Value != null)
            {
                if (decimal.TryParse(e.Value.ToString(), out decimal fee))
                {
                    e.Value = string.Format("${0:N2}", fee);
                }
            }
        }

        private void dgvDetainedLicenses_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            string columnName = dgvDetainedLicenses.Columns[e.ColumnIndex].Name;

            // 1. رسم الـ Status كـ Badge/Pill محاذى على اليسار تحت العنوان مباشرة
            if (columnName == "Status" && e.Value != null)
            {
                e.PaintBackground(e.CellBounds, true);

                string status = e.Value.ToString();

                Color backColor = Color.Empty;
                Color textColor = Color.Empty;

                if (status == "Detained")
                {
                    backColor = Color.FromArgb(254, 226, 226); // Soft Red
                    textColor = Color.FromArgb(185, 28, 28);   // Dark Red
                }
                else if (status == "Released")
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
            if (columnName == "btnRelease")
            {
                string status = dgvDetainedLicenses.Rows[e.RowIndex].Cells["Status"].Value?.ToString();

                if (status == "Released")
                {
                    e.PaintBackground(e.CellBounds, true);
                    e.Handled = true;
                }
            }
        }

        private void dgvDetainedLicenses_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            // التأكد من وجود قيم قبل التحويل لمنع الـ NullReferenceException
            if (dgvDetainedLicenses.Rows[e.RowIndex].Cells["Detain ID"].Value == DBNull.Value ||
                dgvDetainedLicenses.Rows[e.RowIndex].Cells["Detain ID"].Value == null)
                return;

            int detainID = Convert.ToInt32(dgvDetainedLicenses.Rows[e.RowIndex].Cells["Detain ID"].Value);
            int licenseID = Convert.ToInt32(dgvDetainedLicenses.Rows[e.RowIndex].Cells["License ID"].Value);

            // عند الضغط على زر Release
            if (dgvDetainedLicenses.Columns[e.ColumnIndex].Name == "btnRelease")
            {
                string status = dgvDetainedLicenses.Rows[e.RowIndex].Cells["Status"].Value?.ToString();
                if (status == "Detained")
                {
                    // frmReleaseDetainedLicense frm = new frmReleaseDetainedLicense(licenseID);
                    // frm.ShowDialog();

                    _LoadDetainedLicensesData();
                }
            }

            // عند الضغط على زر View Details
            if (dgvDetainedLicenses.Columns[e.ColumnIndex].Name == "btnViewDetails")
            {
                // frmShowLicenseInfo frm = new frmShowLicenseInfo(licenseID);
                // frm.ShowDialog();
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

        private void dgvDetainedLicenses_Paint(object sender, PaintEventArgs e)
        {
            if (dgvDetainedLicenses.Rows.Count == 0)
            {
                string noDataText = "No Detain License match your search.";

                // اختيار الخط واللون المناسب (رمادي هادئ ومريح للعين)
                using (Font font = new Font("Segoe UI", 11, FontStyle.Regular))
                using (Brush brush = new SolidBrush(Color.FromArgb(120, 144, 156))) // Slate Gray
                {
                    // حساب قياسات النص لتوسيطه تماماً في وسط الـ Grid
                    Size textSize = TextRenderer.MeasureText(noDataText, font);

                    // نأخذ بعين الاعتبار ارتفاع الـ Headers باش يجي النص في وسط المساحة البيضاء بالظبط
                    int headersHeight = dgvDetainedLicenses.ColumnHeadersVisible ? dgvDetainedLicenses.ColumnHeadersHeight : 0;

                    int x = (dgvDetainedLicenses.Width - textSize.Width) / 2;
                    int y = headersHeight + (dgvDetainedLicenses.Height - headersHeight - textSize.Height) / 3;

                    // رسم النص
                    e.Graphics.DrawString(noDataText, font, brush, x, y);
                }
            }
        }

        private void btnAll_Click(object sender, EventArgs e)
        {
            _currentStatusFilter = "All";
            ApplyCombinedFilter();
        }

        private void btnReleased_Click(object sender, EventArgs e)
        {
            _currentStatusFilter = "Released";
            ApplyCombinedFilter();
        }

        private void btnDetained_Click(object sender, EventArgs e)
        {
            _currentStatusFilter = "Detained";
            ApplyCombinedFilter();
        }

        private void tbFilterNameLicenseID_TextChanged(object sender, EventArgs e)
        {
            ApplyCombinedFilter();
        }
        public event Action OnLicenseReleased;

        private void releaseDetainedLicenseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dgvDetainedLicenses.CurrentRow == null || dgvDetainedLicenses.CurrentRow.Index < 0) return;

            int detainID = Convert.ToInt32(dgvDetainedLicenses.CurrentRow.Cells["Detain ID"].Value);
            clsDetainedLicense detainRecord = clsDetainedLicense.Find(detainID);

            if (detainRecord == null)
            {
                MessageBox.Show("Detained record not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 1. التحقق المبكر من حالة الحجز
            if (detainRecord.IsReleased)
            {
                MessageBox.Show("This license is already released.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // 2. طلب تأكيد الإفراج
            DialogResult result = MessageBox.Show($"Are you sure you want to release Detained License ID [{detainID}]?",
                                                  "Confirm Release",
                                                  MessageBoxButtons.YesNo,
                                                  MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
                return;
            clsLicenses license = clsLicenses.Find(detainRecord.LicenseID);
            if (license == null)
            {
                MessageBox.Show("Associated license details not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            int personID = clsDriver.FindPersonIDByDriverID(license.DriverID);
            DataTable dtAppType = clsApplicant.getApplicationTypesTitle_Fees((int)clsApplicant.enApplicationType.ReleaseDetainedDrivingLicsense);
            decimal releaseAppFees = Convert.ToDecimal(dtAppType.Rows[0]["ApplicationFees"]);

            // 5. إنشاء طلب جديد لفك الحجز
            clsApplicant releaseApp = new clsApplicant();
            releaseApp.ApplicantPersonID = personID;
            releaseApp.ApplicationDate = DateTime.Now;
            releaseApp.ApplicationTypeID = (int)clsApplicant.enApplicationType.ReleaseDetainedDrivingLicsense;
            releaseApp.ApplicationStatus = clsApplicant.enApplicationStatus.Completed;
            releaseApp.LastStatusDate = DateTime.Now;
            releaseApp.PaidFees = releaseAppFees;
            releaseApp.CreatedByUserID = clsCurrentUser._UserID;

            if (releaseApp.Save())
            {
                // 6. تحديث سجل الحجز وتحويله إلى Released
                detainRecord.IsReleased = true;
                detainRecord.ReleaseDate = DateTime.Now;
                detainRecord.ReleasedByUserID = clsCurrentUser._UserID;
                detainRecord.ReleaseApplicationID = releaseApp.ApplicationID;

                if (detainRecord.Save())
                {
                    MessageBox.Show($"Detained License released successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // 7. تحديث واجهة الجدول بعد الفك
                    _dtDetainedLicenses = clsDetainedLicense.GetAllDetainedLicenses();
                    dgvDetainedLicenses.DataSource = _dtDetainedLicenses;
                    UpdateRowsCount(_dtDetainedLicenses);
                    OnLicenseReleased?.Invoke();
                }
                else
                {
                    MessageBox.Show("Failed to update detained record status.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Failed to create the release application.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvDetainedLicenses_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                // 1. إخفاء القائمة تماماً إذا كانت الجدول فارغاً
                if (dgvDetainedLicenses.Rows.Count == 0)
                {
                    dgvDetainedLicenses.ContextMenuStrip = null;
                    return;
                }

                DataGridView.HitTestInfo hit = dgvDetainedLicenses.HitTest(e.X, e.Y);

                // 2. إذا كان النقر فوق صف فعلي (وليس Header أو مكان فارغ)
                if (hit.Type == DataGridViewHitTestType.Cell && hit.RowIndex >= 0)
                {
                    dgvDetainedLicenses.ClearSelection();
                    dgvDetainedLicenses.Rows[hit.RowIndex].Selected = true;

                    // تحديد السجل الحالي ليكون هو الصف المضغوط عليه
                    dgvDetainedLicenses.CurrentCell = dgvDetainedLicenses.Rows[hit.RowIndex].Cells[hit.ColumnIndex];

                    // إظهار القائمة الخاصة بك (تأكد من مطابقة اسم الـ ContextMenuStrip)
                    dgvDetainedLicenses.ContextMenuStrip = guna2ContextMenuStrip1;
                }
                else
                {
                    // إخفاء القائمة عند النقر في المساحة البيضاء الفارغة
                    dgvDetainedLicenses.ContextMenuStrip = null;
                }
            }
        }
    }
}
