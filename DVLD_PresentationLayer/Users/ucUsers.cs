using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DVLD_BusinessLayer;

namespace DVLD_PresentationLayer.User
{
    public partial class ucUsers : UserControl
    {
        public ucUsers()
        {
            InitializeComponent();
        }

        // 🎯 هذا هو الحدث الصحيح الذي يشتغل تلقائياً عند تحميل الواجهة
        private void ucUsers_Load_1(object sender, EventArgs e)
        {
            DataTable _dtAllUsers = clsUsers.getAllUsers();

            if (_dtAllUsers != null)
            {
                _dtAllUsers.Columns.Add("USER", typeof(string), "FirstName + ' ' + SecondName + ' ' + ThirdName + ' ' + LastName");
            }

            if (!dgvUsers.Columns.Contains("Actions"))
            {
                DataGridViewImageColumn actionsCol = new DataGridViewImageColumn();
                actionsCol.Name = "Actions";
                actionsCol.HeaderText = "ACTIONS";
                actionsCol.Width = 120;
                actionsCol.DefaultCellStyle.NullValue = null;
                dgvUsers.Columns.Add(actionsCol);
            }

            dgvUsers.DataSource = _dtAllUsers;

            if (dgvUsers.Columns.Count > 0)
            {
                dgvUsers.Columns["UserName"].HeaderText = "USERNAME";
                dgvUsers.Columns["IsActive"].HeaderText = "Status";
                dgvUsers.Columns["USER"].HeaderText = "  USER";
                dgvUsers.Columns["USER"].DefaultCellStyle.Padding = new Padding(10, 0, 0, 0);

                dgvUsers.Columns["FirstName"].Visible = false;
                dgvUsers.Columns["SecondName"].Visible = false;
                dgvUsers.Columns["ThirdName"].Visible = false;
                dgvUsers.Columns["LastName"].Visible = false;

                dgvUsers.Columns["UserName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                // الترتيب الصارم للأعمدة
                dgvUsers.Columns["USER"].DisplayIndex = 0;
                dgvUsers.Columns["Actions"].DisplayIndex = dgvUsers.Columns.Count - 1;
            }

            UpdateRowsCount(_dtAllUsers);
        }

        private void dgvUsers_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0) return;

            // 🎯 1. رسم عمود العمليات (ACTIONS)
            if (e.ColumnIndex >= 0 && dgvUsers.Columns[e.ColumnIndex].Name == "Actions")
            {
                // رسم الخلفية مع فلاتر الدمج الافتراضية
                e.Paint(e.CellBounds, DataGridViewPaintParts.All & ~DataGridViewPaintParts.ContentForeground);

                Image imgShow = (Image)Properties.Resources.ResourceManager.GetObject("show_icon");
                Image imgEdit = (Image)Properties.Resources.ResourceManager.GetObject("edit_icon");
                Image imgDelete = (Image)Properties.Resources.ResourceManager.GetObject("delete_icon");

                if (imgShow != null && imgEdit != null && imgDelete != null)
                {
                    int iconSize = 20;
                    int margin = 8;
                    int totalWidth = (iconSize * 3) + (margin * 2);

                    int startX = e.CellBounds.Left + (e.CellBounds.Width - totalWidth) / 2;
                    int startY = e.CellBounds.Top + (e.CellBounds.Height - iconSize) / 2;

                    Rectangle rectShow = new Rectangle(startX, startY, iconSize, iconSize);
                    Rectangle rectEdit = new Rectangle(startX + iconSize + margin, startY, iconSize, iconSize);
                    Rectangle rectDelete = new Rectangle(startX + (iconSize * 2) + (margin * 2), startY, iconSize, iconSize);

                    e.Graphics.DrawImage(imgShow, rectShow);
                    e.Graphics.DrawImage(imgEdit, rectEdit);
                    e.Graphics.DrawImage(imgDelete, rectDelete);
                }

                e.Handled = true;
            }

            // 🎯 2. رسم عمود الحالة (Status / IsActive) بشكل عصري ومميز
            if (e.ColumnIndex >= 0 && dgvUsers.Columns[e.ColumnIndex].Name == "IsActive")
            {
                // مسح خانة الـ CheckBox الافتراضية ورسم الخلفية فقط
                e.Paint(e.CellBounds, DataGridViewPaintParts.All & ~DataGridViewPaintParts.ContentForeground);

                bool isActive = Convert.ToBoolean(dgvUsers.Rows[e.RowIndex].Cells["IsActive"].Value);

                // إعدادات الألوان والنصوص بناءً على الحالة
                string statusText = isActive ? "Active" : "Inactive";
                Color backColor = isActive ? Color.FromArgb(230, 248, 235) : Color.FromArgb(242, 244, 247);
                Color textColor = isActive ? Color.FromArgb(34, 154, 73) : Color.FromArgb(102, 112, 133);

                // تجهيز أبعاد الكبسولة الدائرية (Badge)
                int badgeWidth = 75;
                int badgeHeight = 24;
                int badgeX = e.CellBounds.Left + (e.CellBounds.Width - badgeWidth) / 2;
                int badgeY = e.CellBounds.Top + (e.CellBounds.Height - badgeHeight) / 2;
                Rectangle badgeRect = new Rectangle(badgeX, badgeY, badgeWidth, badgeHeight);

                // رسم الكبسولة الدائرية بنعومة عالية
                using (System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath())
                {
                    int radius = badgeHeight; // كبسولة دائرية تماماً
                    path.AddArc(badgeRect.X, badgeRect.Y, radius, radius, 180, 90);
                    path.AddArc(badgeRect.Right - radius, badgeRect.Y, radius, radius, 270, 90);
                    path.AddArc(badgeRect.Right - radius, badgeRect.Bottom - radius, radius, radius, 0, 90);
                    path.AddArc(badgeRect.X, badgeRect.Bottom - radius, radius, radius, 90, 90);
                    path.CloseFigure();

                    e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                    using (SolidBrush brush = new SolidBrush(backColor))
                        e.Graphics.FillPath(brush, path);

                    // اختياري: رسم حدود خفيفة جداً حول الكبسولة
                    using (Pen pen = new Pen(Color.FromArgb(180, textColor.R, textColor.G, textColor.B), 1))
                        e.Graphics.DrawPath(pen, path);
                }

                // كتابة النص داخل الكبسولة في السنتر
                using (Font textFont = new Font("Segoe UI", 9, FontStyle.Bold))
                using (SolidBrush textBrush = new SolidBrush(textColor))
                {
                    StringFormat sf = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    };
                    e.Graphics.DrawString(statusText, textFont, textBrush, badgeRect, sf);
                }

                e.Handled = true;
            }
        }

        private void guna2TextBox1_TextChanged(object sender, EventArgs e)
        {
            if (dgvUsers.DataSource is DataTable dt)
            {
                dt.DefaultView.RowFilter = string.Format("USER LIKE '%{0}%' OR UserName LIKE '%{0}%'", guna2TextBox1.Text.Replace("'", "''"));
                UpdateRowsCount(dt);
            }
           
        }

        private void UpdateRowsCount(DataTable dt)
        {
            if (dt != null)
            {
                DataView dvFiltered = dt.DefaultView;
                int activeCount = dvFiltered.ToTable().Select("IsActive = 1").Length;
                int totalFiltered = dvFiltered.Count;
                int inactiveCount = totalFiltered - activeCount;
                lblCountActiveAndInactive.Text = $"{activeCount} active . {inactiveCount} inactive";
            }
        }

        

        private void dgvUsers_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }
    }
}