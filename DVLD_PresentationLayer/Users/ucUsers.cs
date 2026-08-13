using DVLD_BusinessLayer;
using DVLD_PresentationLayer.Users;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace DVLD_PresentationLayer.User
{
    public partial class ucUsers : UserControl
    {
        private enum enActions { AddNew, Show, Edit, Delete }
        private enActions _Action;
        private int _hoveredRowIndex = -1;
        private int _hoveredColumnIndex = -1;
        private int _hoveredIconIndex = -1;

        public ucUsers()
        {
            InitializeComponent();
        }

        private void ucUsers_Load_1(object sender, EventArgs e)
        {
            // 🚀 تفعيل الـ Double Buffering لمنع الوميض
            typeof(DataGridView).InvokeMember("DoubleBuffered",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.SetProperty,
                null, dgvUsers, new object[] { true });

            // تحميل وتنسيق البيانات لأول مرة
            _RefreshPeopleList();
        }

        private void _RefreshPeopleList()
        {
            DataTable dtAllUsers = clsUsers.getAllUsers();

            if (dtAllUsers != null)
            {
                dtAllUsers.Columns.Add("USER", typeof(string));

                foreach (DataRow row in dtAllUsers.Rows)
                {
                    string firstName = row["FirstName"]?.ToString() ?? "";
                    string secondName = row["SecondName"]?.ToString() ?? "";
                    string thirdName = row["ThirdName"]?.ToString() ?? "";
                    string lastName = row["LastName"]?.ToString() ?? "";

                    string fullName = $"{firstName} {secondName} {thirdName} {lastName}";
                    fullName = fullName.Replace("    ", " ").Replace("  ", " ").Trim();

                    row["USER"] = fullName;
                }
            }

            dgvUsers.DataSource = dtAllUsers;

            if (!dgvUsers.Columns.Contains("Actions"))
            {
                DataGridViewImageColumn actionsCol = new DataGridViewImageColumn();
                actionsCol.Name = "Actions";
                actionsCol.HeaderText = "ACTIONS";
                actionsCol.Width = 120;
                actionsCol.DefaultCellStyle.NullValue = null;
                dgvUsers.Columns.Add(actionsCol);
            }

            if (dgvUsers.Columns.Count > 0)
            {
                dgvUsers.Columns["UserName"].HeaderText = "USERNAME";
                dgvUsers.Columns["IsActive"].HeaderText = "Status";
                dgvUsers.Columns["USER"].HeaderText = "   USER";
                dgvUsers.Columns["USER"].DefaultCellStyle.Padding = new Padding(10, 0, 0, 0);

                dgvUsers.Columns["FirstName"].Visible = false;
                dgvUsers.Columns["SecondName"].Visible = false;
                dgvUsers.Columns["ThirdName"].Visible = false;
                dgvUsers.Columns["LastName"].Visible = false;
                dgvUsers.Columns["UserID"].Visible = false;

                if (dgvUsers.Columns.Contains("Password")) dgvUsers.Columns["Password"].Visible = false;
                if (dgvUsers.Columns.Contains("PersonID")) dgvUsers.Columns["PersonID"].Visible = false;

                dgvUsers.Columns["UserName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                dgvUsers.Columns["USER"].DisplayIndex = 0;
                dgvUsers.Columns["UserName"].DisplayIndex = 1;

                if (dgvUsers.Columns.Contains("Permissions")) dgvUsers.Columns["Permissions"].DisplayIndex = 2;


                dgvUsers.CellBorderStyle = DataGridViewCellBorderStyle.None;
                dgvUsers.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
                dgvUsers.RowHeadersVisible = false;
            }

            UpdateRowsCount(dtAllUsers);
        }

        private void dgvUsers_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex >= 0 && e.RowIndex >= 0 && dgvUsers.Columns[e.ColumnIndex].Name == "Actions")
            {
                Point clickPoint = dgvUsers.PointToClient(Cursor.Position);
                Rectangle cellRect = dgvUsers.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false);

                int iconSize = 20, margin = 8;
                int totalWidth = (iconSize * 3) + (margin * 2);
                int startX = cellRect.Left + (cellRect.Width - totalWidth) / 2;
                int startY = cellRect.Top + (cellRect.Height - iconSize) / 2;

                Rectangle rectShow = new Rectangle(startX, startY, iconSize, iconSize);
                Rectangle rectEdit = new Rectangle(startX + iconSize + margin, startY, iconSize, iconSize);
                Rectangle rectDelete = new Rectangle(startX + (iconSize * 2) + (margin * 2), startY, iconSize, iconSize);

                int userID = Convert.ToInt32(dgvUsers.Rows[e.RowIndex].Cells["UserID"].Value);

                if (rectShow.Contains(clickPoint))
                {
                    _Action = enActions.Show;
                    ShowUserControl(userID);
                }
                else if (rectEdit.Contains(clickPoint))
                {
                    _Action = enActions.Edit;
                    ShowUserControl(userID);
                }
                else if (rectDelete.Contains(clickPoint))
                {
                    if (MessageBox.Show($"Are you sure you want to delete User [{userID}]?", "Confirm Delete", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                    {
                        if (clsUsers.DeleteUser(userID))
                        {
                            MessageBox.Show("User Deleted Successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            _RefreshPeopleList();
                        }
                        else
                        {
                            MessageBox.Show("User was not deleted because it is linked to other records in the system.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
        }

        private void btnAddUser_Click(object sender, EventArgs e)
        {
            _Action = enActions.AddNew;
            ShowUserControl(-1);
        }

        private void ShowUserControl(int userID)
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

                    switch (_Action)
                    {
                        case enActions.AddNew:
                            ucAddUpdateUser myAddUser = new ucAddUpdateUser();
                            myAddUser.LoadUserData(-1);
                            frmContainer.Size = myAddUser.Size;
                            myAddUser.Dock = DockStyle.Fill;
                            frmContainer.Controls.Add(myAddUser);
                            myAddUser.DataBack += MyAddPersonPage_DataBack;
                            break;

                        case enActions.Edit:
                            ucAddUpdateUser myUpdateUser = new ucAddUpdateUser();
                            myUpdateUser.LoadUserData(userID);
                            frmContainer.Size = myUpdateUser.Size;
                            myUpdateUser.Dock = DockStyle.Fill;
                            frmContainer.Controls.Add(myUpdateUser);
                            myUpdateUser._LoadUpdateMode(userID);
                            myUpdateUser.DataBack += MyAddPersonPage_DataBack;
                            break;

                        case enActions.Show:
                            ucShowDetailsUser myShowDetailsUserPage = new ucShowDetailsUser(userID);
                            frmContainer.Size = myShowDetailsUserPage.Size;
                            myShowDetailsUserPage.Dock = DockStyle.Fill;
                            frmContainer.Controls.Add(myShowDetailsUserPage);
                            break;
                    }

                    Guna.UI2.WinForms.Guna2Elipse elipse = new Guna.UI2.WinForms.Guna2Elipse();
                    elipse.TargetControl = frmContainer;
                    elipse.BorderRadius = 16;

                    frmContainer.ShowDialog(overlay);
                }
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
                lblCountActiveAndInactive.Text = $"{activeCount} active • {inactiveCount} inactive";
            }
        }

        private void MyAddPersonPage_DataBack(object sender, int PersonID)
        {
            _RefreshPeopleList();
        }

        private void dgvUsers_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            e.PaintParts &= ~DataGridViewPaintParts.Border;
        }

        private void dgvUsers_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0) return;

            if (e.ColumnIndex >= 0 && dgvUsers.Columns[e.ColumnIndex].Name == "Actions")
            {
                e.Paint(e.CellBounds, DataGridViewPaintParts.Background | DataGridViewPaintParts.SelectionBackground);

                if (imageList1 != null && imageList1.Images.Count >= 3)
                {
                    Image imgShow = imageList1.Images[0];
                    Image imgEdit = imageList1.Images[1];
                    Image imgDelete = imageList1.Images[2];

                    int iconSize = 20, margin = 8;
                    int totalWidth = (iconSize * 3) + (margin * 2);
                    int startX = e.CellBounds.Left + (e.CellBounds.Width - totalWidth) / 2;
                    int startY = e.CellBounds.Top + (e.CellBounds.Height - iconSize) / 2;

                    Rectangle rectShow = new Rectangle(startX, startY, iconSize, iconSize);
                    Rectangle rectEdit = new Rectangle(startX + iconSize + margin, startY, iconSize, iconSize);
                    Rectangle rectDelete = new Rectangle(startX + (iconSize * 2) + (margin * 2), startY, iconSize, iconSize);

                    if (e.RowIndex == _hoveredRowIndex && e.ColumnIndex == _hoveredColumnIndex)
                    {
                        int padding = 4;
                        if (_hoveredIconIndex == 0)
                        {
                            Rectangle bgRect = new Rectangle(rectShow.X - padding, rectShow.Y - padding, rectShow.Width + (padding * 2), rectShow.Height + (padding * 2));
                            using (SolidBrush hoverBrush = new SolidBrush(Color.FromArgb(40, 0, 120, 215)))
                                e.Graphics.FillEllipse(hoverBrush, bgRect);
                        }
                        else if (_hoveredIconIndex == 1)
                        {
                            Rectangle bgRect = new Rectangle(rectEdit.X - padding, rectEdit.Y - padding, rectEdit.Width + (padding * 2), rectEdit.Height + (padding * 2));
                            using (SolidBrush hoverBrush = new SolidBrush(Color.FromArgb(40, 34, 154, 73)))
                                e.Graphics.FillEllipse(hoverBrush, bgRect);
                        }
                        else if (_hoveredIconIndex == 2)
                        {
                            Rectangle bgRect = new Rectangle(rectDelete.X - padding, rectDelete.Y - padding, rectDelete.Width + (padding * 2), rectDelete.Height + (padding * 2));
                            using (SolidBrush hoverBrush = new SolidBrush(Color.FromArgb(40, 211, 47, 47)))
                                e.Graphics.FillEllipse(hoverBrush, bgRect);
                        }
                    }

                    e.Graphics.DrawImage(imgShow, rectShow);
                    e.Graphics.DrawImage(imgEdit, rectEdit);
                    e.Graphics.DrawImage(imgDelete, rectDelete);
                }

                e.Handled = true;
            }

            if (e.ColumnIndex >= 0 && dgvUsers.Columns[e.ColumnIndex].Name == "IsActive")
            {
                e.Paint(e.CellBounds, DataGridViewPaintParts.Background | DataGridViewPaintParts.SelectionBackground);

                bool isActive = Convert.ToBoolean(dgvUsers.Rows[e.RowIndex].Cells["IsActive"].Value);

                string statusText = isActive ? "Active" : "Inactive";
                Color backColor = isActive ? Color.FromArgb(230, 248, 235) : Color.FromArgb(242, 244, 247);
                Color textColor = isActive ? Color.FromArgb(34, 154, 73) : Color.FromArgb(102, 112, 133);

                int badgeWidth = 75, badgeHeight = 24;
                int badgeX = e.CellBounds.Left + (e.CellBounds.Width - badgeWidth) / 2;
                int badgeY = e.CellBounds.Top + (e.CellBounds.Height - badgeHeight) / 2;
                Rectangle badgeRect = new Rectangle(badgeX, badgeY, badgeWidth, badgeHeight);

                using (System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath())
                {
                    int radius = badgeHeight;
                    path.AddArc(badgeRect.X, badgeRect.Y, radius, radius, 180, 90);
                    path.AddArc(badgeRect.Right - radius, badgeRect.Y, radius, radius, 270, 90);
                    path.AddArc(badgeRect.Right - radius, badgeRect.Bottom - radius, radius, radius, 0, 90);
                    path.AddArc(badgeRect.X, badgeRect.Bottom - radius, radius, radius, 90, 90);
                    path.CloseFigure();

                    e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                    using (SolidBrush brush = new SolidBrush(backColor))
                        e.Graphics.FillPath(brush, path);

                    using (Pen pen = new Pen(Color.FromArgb(180, textColor.R, textColor.G, textColor.B), 1))
                        e.Graphics.DrawPath(pen, path);
                }

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
            dgvUsers.Columns["Actions"].DisplayIndex = dgvUsers.Columns.Count - 1;
        }

        private void dgvUsers_CellMouseMove(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0 && dgvUsers.Columns[e.ColumnIndex].Name == "Actions")
            {
                Point localPoint = dgvUsers.PointToClient(Cursor.Position);
                Rectangle cellRect = dgvUsers.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false);

                int iconSize = 20, margin = 8;
                int totalWidth = (iconSize * 3) + (margin * 2);
                int startX = cellRect.Left + (cellRect.Width - totalWidth) / 2;
                int startY = cellRect.Top + (cellRect.Height - iconSize) / 2;

                Rectangle rectShow = new Rectangle(startX, startY, iconSize, iconSize);
                Rectangle rectEdit = new Rectangle(startX + iconSize + margin, startY, iconSize, iconSize);
                Rectangle rectDelete = new Rectangle(startX + (iconSize * 2) + (margin * 2), startY, iconSize, iconSize);

                int currentIconIndex = -1;

                if (rectShow.Contains(localPoint)) currentIconIndex = 0;
                else if (rectEdit.Contains(localPoint)) currentIconIndex = 1;
                else if (rectDelete.Contains(localPoint)) currentIconIndex = 2;

                if (e.RowIndex != _hoveredRowIndex || e.ColumnIndex != _hoveredColumnIndex || currentIconIndex != _hoveredIconIndex)
                {
                    _hoveredRowIndex = e.RowIndex;
                    _hoveredColumnIndex = e.ColumnIndex;
                    _hoveredIconIndex = currentIconIndex;

                    dgvUsers.Cursor = (currentIconIndex != -1) ? Cursors.Hand : Cursors.Default;
                    dgvUsers.InvalidateCell(e.ColumnIndex, e.RowIndex);
                }
            }
            else
            {
                ResetHoverEffect();
            }
        }

        private void dgvUsers_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
            ResetHoverEffect();
        }

        private void ResetHoverEffect()
        {
            if (_hoveredRowIndex != -1 || _hoveredColumnIndex != -1 || _hoveredIconIndex != -1)
            {
                int oldRow = _hoveredRowIndex;
                int oldCol = _hoveredColumnIndex;

                _hoveredRowIndex = -1;
                _hoveredColumnIndex = -1;
                _hoveredIconIndex = -1;
                dgvUsers.Cursor = Cursors.Default;

                if (oldRow >= 0 && oldCol >= 0 && oldRow < dgvUsers.RowCount && oldCol < dgvUsers.ColumnCount)
                {
                    dgvUsers.InvalidateCell(oldCol, oldRow);
                }
            }
        }

        private void dgvUsers_Paint(object sender, PaintEventArgs e)
        {
            if (dgvUsers.Rows.Count == 0)
            {
                string noDataText = "No users match your search.";

                using (Font font = new Font("Segoe UI", 11, FontStyle.Regular))
                using (Brush brush = new SolidBrush(Color.FromArgb(120, 144, 156)))
                {
                    Size textSize = TextRenderer.MeasureText(noDataText, font);
                    int headersHeight = dgvUsers.ColumnHeadersVisible ? dgvUsers.ColumnHeadersHeight : 0;

                    int x = (dgvUsers.Width - textSize.Width) / 2;
                    int y = headersHeight + (dgvUsers.Height - headersHeight - textSize.Height) / 3;

                    e.Graphics.DrawString(noDataText, font, brush, x, y);
                }
            }
        }
    }
}