using DVLD_BusinessLayer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace DVLD_PresentationLayer
{
    public partial class ucPeople : UserControl
    {
        [DllImport("dwmapi.dll")]
        public static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

        private const int DWMWA_SYSTEMBACKDROP_TYPE = 38;
        private const int DWMSBT_TABBEDWINDOW = 2;

        public ucPeople()
        {
            InitializeComponent();
        }

        // استخدام اسم الدولة كـ مفتاح أساسي للكاش لتوحيد المنطق مع الـ ComboBox
        private Dictionary<string, string> _countryCodesCache = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        private Dictionary<string, Image> _flagImagesCache = new Dictionary<string, Image>(StringComparer.OrdinalIgnoreCase);

        private void LoadCountryCodesCache()
        {
            try
            {
                string csvPath = Path.Combine(Application.StartupPath, "countries.csv");

                if (File.Exists(csvPath))
                {
                    var lines = File.ReadAllLines(csvPath);
                    for (int i = 1; i < lines.Length; i++)
                    {
                        string line = lines[i].Trim();
                        if (string.IsNullOrEmpty(line)) continue;

                        var columns = System.Text.RegularExpressions.Regex.Split(line, ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");

                        if (columns.Length >= 4)
                        {
                            string name = columns[3].Replace("\"", "").Trim();
                            string code = columns[1].Replace("\"", "").Trim().ToLower();

                            if (!_countryCodesCache.ContainsKey(name))
                            {
                                _countryCodesCache.Add(name, code);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in Country Cache: " + ex.Message, "Cache Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void _RefreshPeopleList()
        {
            DataTable _dtAllPeople = clsPerson.GetPeople();

            if (_dtAllPeople != null)
            {
                // إضافة الأعمدة المحسوبة ديناميكياً
                _dtAllPeople.Columns.Add("PERSON", typeof(string),"TRIM(FirstName + " + "IIF(ISNULL(SecondName, '') = '', '', ' ' + SecondName) + " + "IIF(ISNULL(ThirdName, '') = '', '', ' ' + ThirdName) + " + "' ' + LastName)"); 
                _dtAllPeople.Columns.Add("CountryCode", typeof(string));

                foreach (DataRow row in _dtAllPeople.Rows)
                {
                    string countryName = row["CountryName"].ToString().Trim();

                    if (_countryCodesCache.TryGetValue(countryName, out string code))
                    {
                        row["CountryCode"] = code;
                    }
                }
            }

            dgvPeople.DataSource = _dtAllPeople;
            UpdateRowsCount();

            // إخفاء الأعمدة الإضافية بعد ربط الـ DataSource الجديد
            if (dgvPeople.Columns["CountryCode"] != null)
            {
                dgvPeople.Columns["CountryCode"].Visible = false;
            }

            if (dgvPeople.Rows.Count > 0)
            {
                dgvPeople.Columns["PersonID"].Visible = false;
                dgvPeople.Columns["FirstName"].Visible = false;
                dgvPeople.Columns["SecondName"].Visible = false;
                dgvPeople.Columns["ThirdName"].Visible = false;
                dgvPeople.Columns["LastName"].Visible = false;
                dgvPeople.Columns["Gendor"].Visible = false;
                dgvPeople.Columns["ImagePath"].Visible = false;
                dgvPeople.Columns["Address"].Visible = false;
                dgvPeople.Columns["NationalNo"].HeaderText = "NATIONAL ID";
                dgvPeople.Columns["DateOfBirth"].DefaultCellStyle.Format = "MMM dd, yyyy";
                dgvPeople.Columns["DateOfBirth"].HeaderText = "DATE OF BIRTH";
                dgvPeople.Columns["Email"].HeaderText = "EMAIL";
                dgvPeople.Columns["PERSON"].HeaderText = "  PERSON";
                dgvPeople.Columns["PERSON"].DisplayIndex = 0;
                dgvPeople.Columns["PERSON"].Width = 250;
                dgvPeople.Columns["PERSON"].DefaultCellStyle.Padding = new Padding(10, 0, 0, 0);
            }
        }
        private void ucPeople_Load(object sender, EventArgs e)
        {
            dgvPeople.CellPainting += guna2DataGridView1_CellPainting;
            LoadCountryCodesCache();

            // استدعاء دالة التحديث لأول مرة عند تحميل الصفحة
            _RefreshPeopleList();
        }

        private void guna2DataGridView1_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.ColumnIndex >= 0 && e.RowIndex >= 0 && dgvPeople.Columns[e.ColumnIndex].Name == "CountryName")
            {
                if (e.Value != null)
                {
                    e.PaintBackground(e.CellBounds, true);

                    string countryName = e.Value.ToString().Trim();
                    string countryCode = dgvPeople.Rows[e.RowIndex].Cells["CountryCode"].Value?.ToString() ?? "";

                    Image flagImage = null;
                    int imageWidth = 24;
                    int imageHeight = 16;

                    // 🌟 البحث الذكي في الكاش: باستخدام اسم الدولة أولاً للتوحيد
                    if (_flagImagesCache.TryGetValue(countryName, out Image cachedImage))
                    {
                        flagImage = cachedImage;
                    }
                    else
                    {
                        // 1. محاولة البحث باسم الصورة الثنائي (مثال: tn.png)
                        string flagPath = Path.Combine(Application.StartupPath, "Flags", $"{countryCode}.png");

                        // 2. خطة بديلة: البحث باسم الدولة كاملاً (مثال: Tunisia.png) ليتوافق مع فورم الإضافة
                        if (!File.Exists(flagPath))
                        {
                            flagPath = Path.Combine(Application.StartupPath, "Flags", $"{countryName}.png");
                        }

                        // 3. البحث في مجلد المشروع الأب للتسهيل أثناء الـ Debugging
                        if (!File.Exists(flagPath) && Directory.GetParent(Application.StartupPath)?.Parent != null)
                        {
                            string alternativePath = Path.Combine(Directory.GetParent(Application.StartupPath).Parent.FullName, "Flags", $"{countryCode}.png");
                            if (File.Exists(alternativePath)) flagPath = alternativePath;
                        }

                        if (File.Exists(flagPath))
                        {
                            try
                            {
                                flagImage = Image.FromFile(flagPath);
                                _flagImagesCache.Add(countryName, flagImage); // الحفظ باسم الدولة للسرعة
                            }
                            catch { }
                        }
                    }

                    int startX = e.CellBounds.X + 8;
                    int startY = e.CellBounds.Y + ((e.CellBounds.Height - imageHeight) / 2);

                    if (flagImage != null)
                    {
                        e.Graphics.DrawImage(flagImage, startX, startY, imageWidth, imageHeight);
                    }

                    // توسيط وحسابات النص
                    int textX = startX + (flagImage != null ? imageWidth + 10 : 0);
                    Font cellFont = e.CellStyle.Font ?? dgvPeople.Font;
                    SizeF textSize = e.Graphics.MeasureString(countryName, cellFont);
                    float textY = e.CellBounds.Y + ((e.CellBounds.Height - textSize.Height) / 2);

                    using (Brush textBrush = new SolidBrush(e.CellStyle.ForeColor))
                    {
                        e.Graphics.DrawString(countryName, cellFont, textBrush, textX, textY);
                    }

                    e.Paint(e.CellBounds, DataGridViewPaintParts.Border);
                    e.Handled = true;
                }
            }
        }

        private void guna2TextBox1_TextChanged(object sender, EventArgs e)
        {
            if (dgvPeople.DataSource is DataTable dt)
            {
                dt.DefaultView.RowFilter = string.Format("PERSON LIKE '%{0}%' OR NationalNo LIKE '%{0}%' OR Email LIKE '%{0}%'", guna2TextBox1.Text.Replace("'", "''"));
            }
            UpdateRowsCount();
        }

        private void UpdateRowsCount()
        {
            int rowsCount = dgvPeople.Rows.Count;

            if (rowsCount == 0)
                lblCountRows.Text = "No Registered Individuals";
            else if (rowsCount == 1)
                lblCountRows.Text = "1 Registered Individual";
            else
                lblCountRows.Text = $"{rowsCount} Registered Individuals";
        }

        private void btnAddPerson_Click(object sender, EventArgs e)
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

                    ucAddUpdatePerson myAddPersonPage = new ucAddUpdatePerson();
                    myAddPersonPage.LoadPersonData(-1);
                    frmContainer.Size = myAddPersonPage.Size;
                    myAddPersonPage.Dock = DockStyle.Fill;
                    frmContainer.Controls.Add(myAddPersonPage);

                    // 🌟 السطر السحري: ربط الـ Delegate الخاص بالـ User Control بالدالة المخصصة للتحديث
                    myAddPersonPage.DataBack += MyAddPersonPage_DataBack;

                    Guna.UI2.WinForms.Guna2Elipse elipse = new Guna.UI2.WinForms.Guna2Elipse();
                    elipse.TargetControl = frmContainer;
                    elipse.BorderRadius = 16;

                    frmContainer.ShowDialog(overlay);
                }
            }
        }
        private void MyAddPersonPage_DataBack(object sender, int PersonID)
        {
            _RefreshPeopleList();
        }

        private void dgvPeople_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex >= 0)
            {
                dgvPeople.CurrentCell = dgvPeople.Rows[e.RowIndex].Cells[e.ColumnIndex >= 0 ? e.ColumnIndex : 0];

                // التحديد البصري (إضافي للتأكيد)
                dgvPeople.ClearSelection();
                dgvPeople.Rows[e.RowIndex].Selected = true;
            }
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // 1. التأكد من أن هناك سطر محدد في الـ DataGridView الخاص بك
            // استبدل dgvPeople بالاسم الحقيقي للـ DataGridView لديك
            if (dgvPeople.CurrentRow == null)
            {

                MessageBox.Show("Please select a person first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. جلب الـ ID من العمود الخاص به (تأكد من كتابة اسم العمود بدقة كما هو في قاعدة البيانات أو الـ DataGridView)
            int selectedPersonID = Convert.ToInt32(dgvPeople.CurrentRow.Cells["PersonID"].Value);


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

                    ucAddUpdatePerson myAddPersonPage = new ucAddUpdatePerson();

                    // 🌟 هنا نقوم بتمرير الـ ID المجلوب مباشرة ليتحول الـ User Control إلى وضع الـ Update تلقائياً
                    myAddPersonPage.LoadPersonData(selectedPersonID);

                    frmContainer.Size = myAddPersonPage.Size;
                    myAddPersonPage.Dock = DockStyle.Fill;
                    frmContainer.Controls.Add(myAddPersonPage);

                    // ربط الـ Delegate الخاص بالـ User Control بالدالة المخصصة للتحديث
                    myAddPersonPage.DataBack += MyAddPersonPage_DataBack;

                    Guna.UI2.WinForms.Guna2Elipse elipse = new Guna.UI2.WinForms.Guna2Elipse();
                    elipse.TargetControl = frmContainer;
                    elipse.BorderRadius = 16;

                    frmContainer.ShowDialog(overlay);
                }
            }
        }

        private void showDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dgvPeople.CurrentRow == null)
            {

                MessageBox.Show("Please select a person first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. جلب الـ ID من العمود الخاص به (تأكد من كتابة اسم العمود بدقة كما هو في قاعدة البيانات أو الـ DataGridView)
            int selectedPersonID = Convert.ToInt32(dgvPeople.CurrentRow.Cells["PersonID"].Value);


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

                    ucShowDetails myShowDetails = new ucShowDetails();

                    // 🌟 هنا نقوم بتمرير الـ ID المجلوب مباشرة ليتحول الـ User Control إلى وضع الـ Update تلقائياً
                    myShowDetails.LoadPersonData(selectedPersonID);

                    frmContainer.Size = myShowDetails.Size;
                    myShowDetails.Dock = DockStyle.Fill;
                    frmContainer.Controls.Add(myShowDetails);

                    Guna.UI2.WinForms.Guna2Elipse elipse = new Guna.UI2.WinForms.Guna2Elipse();
                    elipse.TargetControl = frmContainer;
                    elipse.BorderRadius = 16;

                    frmContainer.ShowDialog(overlay);
                }
            }

        }

        private void dgvPeople_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            // التأكد من أننا داخل العمود الخاص بتاريخ الميلاد وأن الخلية تحتوي على قيمة
            if (dgvPeople.Columns[e.ColumnIndex].Name == "DateOfBirth" && e.Value != null)
            {
                if (e.Value is DateTime dateValue)
                {
                    // هنا نجبر الخلية على طباعة التاريخ باللغة الإنجليزية القياسية مهما كانت لغة الجهاز
                    e.Value = dateValue.ToString("MMM dd, yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    e.FormattingApplied = true; // إعلام نظام الـ Grid بأنه تم تطبيق التنسيق بنجاح
                }
            }
        }

        private void dgvPeople_Paint(object sender, PaintEventArgs e)
        {
            if (dgvPeople.Rows.Count == 0)
            {
                string noDataText = "No people match your search.";

                // اختيار الخط واللون المناسب (رمادي هادئ ومريح للعين)
                using (Font font = new Font("Segoe UI", 11, FontStyle.Regular))
                using (Brush brush = new SolidBrush(Color.FromArgb(120, 144, 156))) // Slate Gray
                {
                    // حساب قياسات النص لتوسيطه تماماً في وسط الـ Grid
                    Size textSize = TextRenderer.MeasureText(noDataText, font);

                    // نأخذ بعين الاعتبار ارتفاع الـ Headers باش يجي النص في وسط المساحة البيضاء بالظبط
                    int headersHeight = dgvPeople.ColumnHeadersVisible ? dgvPeople.ColumnHeadersHeight : 0;

                    int x = (dgvPeople.Width - textSize.Width) / 2;
                    int y = headersHeight + (dgvPeople.Height - headersHeight - textSize.Height) / 3;

                    // رسم النص
                    e.Graphics.DrawString(noDataText, font, brush, x, y);
                }
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dgvPeople.CurrentRow == null) return;

            int selectedPersonID = Convert.ToInt32(dgvPeople.CurrentRow.Cells["PersonID"].Value);

            if (MessageBox.Show($"Are you sure you want to delete Person [{selectedPersonID}]?", "Confirm Delete", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                if (clsPerson.DeletePerson(selectedPersonID))
                {
                    MessageBox.Show("Person Deleted Successfully.", "Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    _RefreshPeopleList(); // استدعي دالة إعادة تحميل الجدول
                }
                else
                {
                    MessageBox.Show("Person was not deleted because it has data linked to it in the system.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}