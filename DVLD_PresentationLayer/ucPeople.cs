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
        // وضع هذا الجزء داخل الـ Class مباشرة
        [DllImport("dwmapi.dll")]
        public static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

        // ثوابت نظام التشغيل لتفعيل التأثيرات الزجاجية
        private const int DWMWA_SYSTEMBACKDROP_TYPE = 38;
        private const int DWMSBT_TABBEDWINDOW = 2; // تأثير Acrylic/Blur قوي وممتاز للـ Modals
        public ucPeople()
        {
            InitializeComponent();
        }

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
                MessageBox.Show("خطأ في الكاش: " + ex.Message);
            }
        }

        private void ucPeople_Load(object sender, EventArgs e)
        {
            guna2DataGridView1.CellPainting += guna2DataGridView1_CellPainting;

            LoadCountryCodesCache();
            DataTable _dtAllPeople = clsPerson.GetPeople();

            if (_dtAllPeople != null)
            {
                _dtAllPeople.Columns.Add("PERSON", typeof(string), "FirstName + ' ' + SecondName + ' ' + ThirdName + ' ' + LastName");
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

            guna2DataGridView1.DataSource = _dtAllPeople;
            int rowsCount = guna2DataGridView1.Rows.Count;

            if (rowsCount == 0)
            {
                lblCountRows.Text = "No Registered Individuals";
            }
            else if (rowsCount == 1)
            {
                lblCountRows.Text = "1 Registered Individual";
            }
            else
            {
                lblCountRows.Text = $"{rowsCount} Registered Individuals";
            }
            if (guna2DataGridView1.Columns["CountryCode"] != null)
            {
                guna2DataGridView1.Columns["CountryCode"].Visible = false;
            }

            if (guna2DataGridView1.Rows.Count > 0)
            {
                guna2DataGridView1.Columns["FirstName"].Visible = false;
                guna2DataGridView1.Columns["SecondName"].Visible = false;
                guna2DataGridView1.Columns["ThirdName"].Visible = false;
                guna2DataGridView1.Columns["LastName"].Visible = false;
                guna2DataGridView1.Columns["Gendor"].Visible = false;
                guna2DataGridView1.Columns["ImagePath"].Visible = false;
                guna2DataGridView1.Columns["Address"].Visible = false;
                guna2DataGridView1.Columns["NationalNo"].HeaderText = "NATIONAL ID";
                guna2DataGridView1.Columns["DateOfBirth"].DefaultCellStyle.Format = "MMM dd, yyyy";
                guna2DataGridView1.Columns["DateOfBirth"].HeaderText = "DATE OF BIRTH";
                guna2DataGridView1.Columns["Email"].HeaderText = "EMAIL";
                guna2DataGridView1.Columns["PERSON"].HeaderText = "  PERSON";
                guna2DataGridView1.Columns["PERSON"].DisplayIndex = 0;
                guna2DataGridView1.Columns["PERSON"].Width = 250;
                guna2DataGridView1.Columns["PERSON"].DefaultCellStyle.Padding = new Padding(10, 0, 0, 0);
            }
        }
        

        private void guna2DataGridView1_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.ColumnIndex >= 0 && e.RowIndex >= 0 && guna2DataGridView1.Columns[e.ColumnIndex].Name == "CountryName")
            {
                if (e.Value != null)
                {
                    // 1. رسم خلفية الخلية الافتراضية
                    e.PaintBackground(e.CellBounds, true);

                    string countryName = e.Value.ToString().Trim();
                    string countryCode = guna2DataGridView1.Rows[e.RowIndex].Cells["CountryCode"].Value?.ToString() ?? "unknown";

                    Image flagImage = null;
                    int imageWidth = 24;
                    int imageHeight = 16;

                    if (_flagImagesCache.TryGetValue(countryCode, out Image cachedImage))
                    {
                        flagImage = cachedImage;
                    }
                    else
                    {
                        string flagPath = Path.Combine(Application.StartupPath, "Flags", $"{countryCode}.png");
                        if (File.Exists(flagPath))
                        {
                            try
                            {
                                flagImage = Image.FromFile(flagPath);
                                _flagImagesCache.Add(countryCode, flagImage);
                            }
                            catch { }
                        }
                    }

                    // حساب الإحداثيات الآمنة للرسم في منتصف الخلية عمودياً
                    int startX = e.CellBounds.X + 8;
                    int startY = e.CellBounds.Y + ((e.CellBounds.Height - imageHeight) / 2);

                    if (flagImage != null)
                    {
                        // رسم العلم المأخوذ من الكاش
                        e.Graphics.DrawImage(flagImage, startX, startY, imageWidth, imageHeight);
                    }

                    // حساب موقع النص ليأتي بجانب العلم مباشرة
                    int textX = startX + (flagImage != null ? imageWidth + 8 : 0);
                    SizeF textSize = e.Graphics.MeasureString(countryName, e.CellStyle.Font);
                    float textY = e.CellBounds.Y + ((e.CellBounds.Height - textSize.Height) / 2);

                    // رسم اسم الدولة باللون الافتراضي المخصص للخلية
                    using (Brush textBrush = new SolidBrush(e.CellStyle.ForeColor))
                    {
                        e.Graphics.DrawString(countryName, e.CellStyle.Font, textBrush, textX, textY);
                    }

                    // 2. رسم الحدود الفاصلة للجدول فقط (تعديل السطر المسبب للخطأ)
                    e.Paint(e.CellBounds, DataGridViewPaintParts.Border);

                    // منع الأداة من إعادة رسم محتواها الافتراضي فوق شغلنا
                    e.Handled = true;
                }
            }
        }

        private void guna2TextBox1_TextChanged(object sender, EventArgs e)
        {
            
            if (guna2DataGridView1.DataSource is DataTable dt)
            {
                dt.DefaultView.RowFilter = string.Format("PERSON LIKE '%{0}%' OR NationalNo LIKE '%{0}%' OR Email LIKE '%{0}%'", guna2TextBox1.Text.Replace("'", "''"));
            }
            
            UpdateRowsCount();
        }
        private void UpdateRowsCount()
        {
            int rowsCount = guna2DataGridView1.Rows.Count;

            if (rowsCount == 0)
                lblCountRows.Text = "0 Registered Individuals";
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

                // 1. نضع لوناً مصمتاً (Solid) تماماً وبدون أي قيمة Alpha (تفادي الخطأ)
                // اخترنا لوناً رمادياً داكناً يميل للزرقة ليتماشى مع ثيم المشروع
                overlay.BackColor = Color.FromArgb(45, 55, 72);

                // 2. نتحكم في نسبة الشفافية (ظهور الهيكل الخلفي) عبر الـ Opacity
                // قيمة 0.45d تعني شفافية بنسبة 45% (وهي مكافئة تماماً للـ 110 السابقة)
                overlay.Opacity = 0.45d;

                overlay.Bounds = Screen.FromControl(this).Bounds;
                overlay.ShowInTaskbar = false;
                overlay.Show(this);

                // [باقي الكود الخاص بـ frmContainer يظل كما هو دون تغيير...]
                using (Form frmContainer = new Form())
                {
                    frmContainer.FormBorderStyle = FormBorderStyle.None;
                    frmContainer.BackColor = Color.White;
                    frmContainer.StartPosition = FormStartPosition.CenterParent;

                    ucAddUpdatePerson myAddPersonPage = new ucAddUpdatePerson();
                    frmContainer.Size = myAddPersonPage.Size;
                    myAddPersonPage.Dock = DockStyle.Fill;
                    frmContainer.Controls.Add(myAddPersonPage);

                    Guna.UI2.WinForms.Guna2Elipse elipse = new Guna.UI2.WinForms.Guna2Elipse();
                    elipse.TargetControl = frmContainer;
                    elipse.BorderRadius = 16;

                    frmContainer.ShowDialog(overlay);
                }
            }
        }
    }
}