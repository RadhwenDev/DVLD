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
        int licenseID = -1;
        private void renewLocalDrivingLicenseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dgvLicenses.CurrentRow != null && dgvLicenses.CurrentRow.Index >= 0)
            {
                licenseID = Convert.ToInt32(dgvLicenses.CurrentRow.Cells["LICENSE ID"].Value);

                // 1. جلب بيانات الرخصة القديمة كاملة مرة واحدة فقط
                clsLicenses oldLicense = clsLicenses.Find(licenseID);

                if (oldLicense == null)
                {
                    MessageBox.Show("License not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // 2. التحقق مما إذا كانت الرخصة نشطة وغير منتهية
                if (!oldLicense.IsActive)
                {
                    MessageBox.Show("This license is expired and cannot be renewed!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (oldLicense.ExpirationDate > DateTime.Now)
                {
                    MessageBox.Show("This license is still active and has not expired yet!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // 3. جلب PersonID الخاص بالسائق والتأكد من صحته
                int personID = clsDriver.FindPersonIDByDriverID(oldLicense.DriverID);
                if (personID == -1)
                {
                    MessageBox.Show("Driver/Person details not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // 4. قراءة رسوم طلب التجديد من جدول الأنواع
                DataTable dtAppType = clsApplicant.getApplicationTypesTitle_Fees((int)clsApplicant.enApplicationType.RenewDrivingLicense);
                decimal renewalAppFees = Convert.ToDecimal(dtAppType.Rows[0]["ApplicationFees"]);

                // 5. إنشاء وتعبئة طلب التجديد (Application)
                clsApplicant newApplication = new clsApplicant();
                newApplication.ApplicantPersonID = personID;
                newApplication.ApplicationDate = DateTime.Now;
                newApplication.ApplicationTypeID = (int)clsApplicant.enApplicationType.RenewDrivingLicense;
                newApplication.ApplicationStatus = clsApplicant.enApplicationStatus.Completed;
                newApplication.LastStatusDate = DateTime.Now;
                newApplication.PaidFees = renewalAppFees;
                newApplication.CreatedByUserID = clsCurrentUser._UserID;

                // 6. حفظ الطلب ثم إلغاء تفعيل الرخصة القديمة وإصدار الجديدة
                if (newApplication.Save())
                {
                    if (clsLicenses.Deactivate(licenseID))
                    {
                        clsLicenses newLicense = new clsLicenses();
                        newLicense.ApplicationID = newApplication.ApplicationID;
                        newLicense.DriverID = oldLicense.DriverID;
                        newLicense.LicenseClass = oldLicense.LicenseClass;
                        newLicense.IssueDate = DateTime.Now;
                        newLicense.ExpirationDate = DateTime.Now.AddYears(10);
                        newLicense.Notes = oldLicense.Notes;
                        newLicense.PaidFees = oldLicense.PaidFees; // رسوم الفئة
                        newLicense.IsActive = true;
                        newLicense.IssueReason = clsLicenses.enIssueReason.Renew;
                        newLicense.CreatedByUserID = clsCurrentUser._UserID;

                        if (newLicense.Save())
                        {
                            MessageBox.Show($"License renewed successfully! New License ID: {newLicense.LicenseID}",
                                            "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // تحديث الواجهة والـ GridView
                            _dtAllLicenses = clsLicenses.getAllLicenses();
                            dgvLicenses.DataSource = _dtAllLicenses;
                            ApplyCombinedFilter();
                        }
                        else
                        {
                            MessageBox.Show("Failed to issue the new license.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Failed to deactivate the old license.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Failed to create the renewal application.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void dgvLicenses_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex >= 0)
            {
                // إلغاء تحديد كافة الأسطر المحددة سابقاً
                dgvLicenses.ClearSelection();

                // تحديد السطر الذي تم النقر عليه بالزر الأيمن
                dgvLicenses.Rows[e.RowIndex].Selected = true;

                // جعل السطر المنقور هو الـ CurrentRow
                dgvLicenses.CurrentCell = dgvLicenses.Rows[e.RowIndex].Cells[e.ColumnIndex >= 0 ? e.ColumnIndex : 0];
            }
        }
    }
}