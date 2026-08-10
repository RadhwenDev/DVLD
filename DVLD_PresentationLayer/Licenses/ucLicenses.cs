using DVLD_Business;
using DVLD_BusinessLayer;
using DVLD_PresentationLayer.Applications;
using DVLD_PresentationLayer.DetainLicense;
using DVLD_PresentationLayer.Global;
using DVLD_PresentationLayer.InternationalApplication;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static DVLD_BusinessLayer.clsLicenses;
using static System.Net.Mime.MediaTypeNames;

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
                if (clsDetainedLicense.IsLicenseDetained(oldLicense.LicenseID))
                {
                    MessageBox.Show("This license is currently detained! You must release it before renewing.",
                                    "Not Allowed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                // 2. التحقق مما إذا كانت الرخصة نشطة وغير منتهية
                if (!oldLicense.IsActive)
                {
                    MessageBox.Show("This license is inactive and cannot be renewed!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (oldLicense.ExpirationDate > DateTime.Now)
                {
                    MessageBox.Show("This license is still active and has not expired yet!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                DialogResult result = MessageBox.Show($"Are you sure you want to renew License ID [{licenseID}]?",
                                              "Confirm Renewal",
                                              MessageBoxButtons.YesNo,
                                              MessageBoxIcon.Question);

                if (result != DialogResult.Yes)
                {
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
                        int defaultValidityYears = clsLicenseClass.GetDefaultValidityLength(oldLicense.LicenseClass);
                        newLicense.ExpirationDate = DateTime.Now.AddYears(defaultValidityYears > 0 ? defaultValidityYears : 10);
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
        public enum enReplacementReason { Damaged = 1, Lost = 2 }

        private void ReplaceLicense(enReplacementReason replacementReason)
        {
            if (dgvLicenses.CurrentRow == null || dgvLicenses.CurrentRow.Index < 0)
                return;

            int licenseID = Convert.ToInt32(dgvLicenses.CurrentRow.Cells["LICENSE ID"].Value);

            // 1. جلب بيانات الرخصة القديمة
            clsLicenses oldLicense = clsLicenses.Find(licenseID);

            if (oldLicense == null)
            {
                MessageBox.Show("License not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (clsDetainedLicense.IsLicenseDetained(oldLicense.LicenseID))
            {
                MessageBox.Show("This license is currently detained! You must release it before renewing.",
                                "Not Allowed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. التحقق من أن الرخصة نشطة (لا يمكن استبدال رخصة غير نشطة)
            if (!oldLicense.IsActive)
            {
                MessageBox.Show("This license is not active and cannot be replaced!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (oldLicense.ExpirationDate < DateTime.Now)
            {
                MessageBox.Show("This license is expired! You should renew it instead of replacing it.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string issueReasonText = (replacementReason == enReplacementReason.Damaged) ? "Damaged" : "Lost";
            DialogResult result = MessageBox.Show($"Are you sure you want to issue a replacement for a {issueReasonText} license?",
                                                  "Confirm Replacement",
                                                  MessageBoxButtons.YesNo,
                                                  MessageBoxIcon.Question);
            if (result != DialogResult.Yes)
            {
                return;
            }

            // 3. جلب الـ PersonID الخص بالسائق
            int personID = clsDriver.FindPersonIDByDriverID(oldLicense.DriverID);
            if (personID == -1)
            {
                MessageBox.Show("Driver/Person details not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 4. تحديد نوع الطلب والرسوم وسبب الإصدار بناءً على اختيار المستخدم
            clsApplicant.enApplicationType appType;
            clsLicenses.enIssueReason issueReason;

            if (replacementReason == enReplacementReason.Damaged)
            {
                appType = clsApplicant.enApplicationType.ReplaceDamagedDrivingLicense;
                issueReason = clsLicenses.enIssueReason.ReplacementForDamaged;
            }
            else
            {
                appType = clsApplicant.enApplicationType.ReplaceLostDrivingLicense;
                issueReason = clsLicenses.enIssueReason.ReplacementForLost;
            }

            // قراءة رسوم طلب الاستبدال
            DataTable dtAppType = clsApplicant.getApplicationTypesTitle_Fees((int)appType);
            decimal replacementAppFees = Convert.ToDecimal(dtAppType.Rows[0]["ApplicationFees"]);

            // 5. إنشاء طلب الاستبدال (Application)
            clsApplicant newApplication = new clsApplicant();
            newApplication.ApplicantPersonID = personID;
            newApplication.ApplicationDate = DateTime.Now;
            newApplication.ApplicationTypeID = (int)appType;
            newApplication.ApplicationStatus = clsApplicant.enApplicationStatus.Completed;
            newApplication.LastStatusDate = DateTime.Now;
            newApplication.PaidFees = replacementAppFees;
            newApplication.CreatedByUserID = clsCurrentUser._UserID;

            // 6. حفظ الطلب ثم إلغاء تفعيل الرخصة القديمة وإصدار الرخصة الجديدة
            if (newApplication.Save())
            {
                if (clsLicenses.Deactivate(licenseID))
                {
                    clsLicenses newLicense = new clsLicenses();
                    newLicense.ApplicationID = newApplication.ApplicationID;
                    newLicense.DriverID = oldLicense.DriverID;
                    newLicense.LicenseClass = oldLicense.LicenseClass;
                    newLicense.IssueDate = DateTime.Now;

                    // 🌟 تنبيه هام: تاريخ الانتهاء هو نفسه تاريخ انتهاء الرخصة القديمة
                    newLicense.ExpirationDate = oldLicense.ExpirationDate;

                    newLicense.Notes = oldLicense.Notes;
                    newLicense.PaidFees = 0; // عادة تكون رسوم الرخصة 0 في البدل ويُكتفي برسوم الطلب
                    newLicense.IsActive = true;
                    newLicense.IssueReason = issueReason; // ReplacementForLost أو ReplacementForDamaged
                    newLicense.CreatedByUserID = clsCurrentUser._UserID;

                    if (newLicense.Save())
                    {
                        MessageBox.Show($"License replaced successfully! New License ID: {newLicense.LicenseID}",
                                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // تحديث الـ GridView بالشاشة
                        _dtAllLicenses = clsLicenses.getAllLicenses();
                        dgvLicenses.DataSource = _dtAllLicenses;
                        ApplyCombinedFilter();
                    }
                    else
                    {
                        MessageBox.Show("Failed to issue the new replacement license.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Failed to deactivate the old license.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Failed to create the replacement application.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void replacementForDamagedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ReplaceLicense(enReplacementReason.Damaged);
        }

        private void replacementForLostToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ReplaceLicense(enReplacementReason.Lost);
        }

        private void btnDetainLicense_Click(object sender, EventArgs e)
        {
            ucDetainLicense myDetainLicensen = new ucDetainLicense();
            myDetainLicensen.Dock = DockStyle.Fill;
            myDetainLicensen.Name = "ucDetainLicense";

            foreach (Control ctrl in this.Controls)
            {
                ctrl.Visible = false;
            }

            this.Controls.Add(myDetainLicensen);
            myDetainLicensen.BringToFront();    
        }

        private void detainLicenseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dgvLicenses.CurrentRow == null || dgvLicenses.CurrentRow.Index < 0)
                return;

            int licenseID = Convert.ToInt32(dgvLicenses.CurrentRow.Cells["LICENSE ID"].Value);

            // 1. جلب بيانات الرخصة والتأكد من وجودها
            clsLicenses license = clsLicenses.Find(licenseID);

            if (license == null)
            {
                MessageBox.Show("License not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (clsDetainedLicense.IsLicenseDetained(license.LicenseID))
            {
                MessageBox.Show("This license is currently detained! You must release it before renewing.",
                                "Not Allowed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. التحقق مما إذا كانت الرخصة نشطة
            if (!license.IsActive)
            {
                MessageBox.Show("This license is not active and cannot be detained!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 3. التحقق مما إذا كانت الرخصة محجوزة بالفعل من قبل
            if (clsDetainedLicense.IsLicenseDetained(licenseID))
            {
                MessageBox.Show("This license is already detained!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 4. طلب قيمة الغرامة (Fine Fees) من المستخدم عبر InputBox أو نموذج إدخال بسيط
            string inputFees = clsUtility.ShowInputBox("Please enter the fine fees for detaining this license:", "Enter Fine Fees");
            if (string.IsNullOrWhiteSpace(inputFees))
                return; // قام المستخدم بالضغط على Cancel أو ترك الحقل فارغاً

            if (!decimal.TryParse(inputFees, out decimal fineFees) || fineFees < 0)
            {
                MessageBox.Show("Invalid fee amount. Please enter a valid non-negative number.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 5. تأكيد العملية من المستخدم
            DialogResult result = MessageBox.Show($"Are you sure you want to detain License ID [{licenseID}] with Fine Fees: {fineFees:C}?",
                                                  "Confirm Detain",
                                                  MessageBoxButtons.YesNo,
                                                  MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
                return;

            // 6. إنشاء كائن الحجز وتعبئة البيانات
            clsDetainedLicense detainedLicense = new clsDetainedLicense();
            detainedLicense.LicenseID = licenseID;
            detainedLicense.DetainDate = DateTime.Now;
            detainedLicense.FineFees = fineFees;
            detainedLicense.CreatedByUserID = clsCurrentUser._UserID;
            detainedLicense.IsReleased = false;

            // 7. حفظ بيانات الحجز وتحديث الواجهة
            if (detainedLicense.Save())
            {
                MessageBox.Show($"License ID [{licenseID}] has been detained successfully with Detain ID [{detainedLicense.DetainID}].",
                                "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // إعادة تحميل البيانات وتطبيق الفلتر المشترك لإنعاش الـ GridView
                _dtAllLicenses = clsLicenses.getAllLicenses();
                dgvLicenses.DataSource = _dtAllLicenses;
                ApplyCombinedFilter();
            }
            else
            {
                MessageBox.Show("Failed to detain the license. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnInternationalApp_Click(object sender, EventArgs e)
        {
            ucInternationalApplication myInternationalLicensen = new ucInternationalApplication();
            myInternationalLicensen.Dock = DockStyle.Fill;
            myInternationalLicensen.Name = "ucDetainLicense";

            foreach (Control ctrl in this.Controls)
            {
                ctrl.Visible = false;
            }

            this.Controls.Add(myInternationalLicensen);
            myInternationalLicensen.BringToFront();
        }

        private void newInternationalApplicationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // 1. التأكد من تحديد صف في الـ DataGridView
            if (dgvLicenses.CurrentRow == null || dgvLicenses.CurrentRow.Index < 0)
                return;

            int licenseID = Convert.ToInt32(dgvLicenses.CurrentRow.Cells["LICENSE ID"].Value);
            clsLicenses license = clsLicenses.Find(licenseID);

            if (license == null)
            {
                MessageBox.Show("License details not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 2. التحقق من أن الرخصة نشطة
            if (!license.IsActive)
            {
                MessageBox.Show("Selected local license is not active! Cannot issue an international license.",
                                "Not Allowed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 3. التحقق من أن الرخصة من الصنف 3 (Ordinary Driving License)
            if (license.LicenseClass != 3)
            {
                MessageBox.Show("International licenses can only be issued for Ordinary Driving Licenses (Class 3).",
                                "Not Allowed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 4. التحقق مما إذا كانت الرخصة محتجزة
            if (clsDetainedLicense.IsLicenseDetained(license.LicenseID))
            {
                MessageBox.Show("This local license is currently detained! You must release it first.",
                                "Not Allowed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 5. التحقق مما إذا كان لدى السائق رخصة دولية نشطة بالفعل
            if (clsInternationalLicense.hasInternationalLicense(license.DriverID))
            {
                MessageBox.Show("This driver already has an active International License!",
                                "Not Allowed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            DialogResult confirmResult = MessageBox.Show(
        $"Are you sure you want to issue an International License for Local License ID [{license.LicenseID}]?",
        "Confirm Issuance",
        MessageBoxButtons.YesNo,
        MessageBoxIcon.Question);

            if (confirmResult != DialogResult.Yes)
                return;
            // 6. الحصول على الشخص المرتبط بالسائق
            int personID = clsDriver.FindPersonIDByDriverID(license.DriverID);
            if (personID == -1)
            {
                MessageBox.Show("Driver/Person details not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 7. جلب رسوم الطلب
            DataTable dtFees = clsApplicant.getApplicationTypesTitle_Fees((int)clsApplicant.enApplicationType.NewInternationalLicense);
            decimal fees = Convert.ToDecimal(dtFees.Rows[0]["ApplicationFees"]);

            // 8. إنشاء الطلب الرئيسي (Application)
            clsApplicant newApplication = new clsApplicant();
            newApplication.ApplicantPersonID = personID;
            newApplication.ApplicationDate = DateTime.Now;
            newApplication.ApplicationTypeID = (int)clsApplicant.enApplicationType.NewInternationalLicense;
            newApplication.ApplicationStatus = clsApplicant.enApplicationStatus.Completed;
            newApplication.LastStatusDate = DateTime.Now;
            newApplication.PaidFees = fees;
            newApplication.CreatedByUserID = clsCurrentUser._UserID;

            if (newApplication.Save())
            {
                // 9. إنشاء سجل الرخصة الدولية
                clsInternationalLicense internationalLicense = new clsInternationalLicense();
                internationalLicense.ApplicationID = newApplication.ApplicationID; // 👈 ربطه بالطلب المنشأ
                internationalLicense.DriverID = license.DriverID;
                internationalLicense.IssuedUsingLocalLicenseID = license.LicenseID; // 👈 ربطها برخصة القيادة المحلية المحددة
                internationalLicense.IssueDate = DateTime.Now;
                internationalLicense.ExpirationDate = DateTime.Now.AddYears(1);
                internationalLicense.IsActive = true;
                internationalLicense.CreatedByUserID = clsCurrentUser._UserID;

                if (internationalLicense.Save())
                {
                    MessageBox.Show($"International License issued successfully with ID = {internationalLicense.InternationalLicenseID}!",
                                    "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // 10. تحديث الجدول بالشاشة
                    _dtAllLicenses = clsLicenses.getAllLicenses();
                    dgvLicenses.DataSource = _dtAllLicenses;
                    ApplyCombinedFilter();
                }
                else
                {
                    MessageBox.Show("Failed to save International License data.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Failed to create the International License application.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}