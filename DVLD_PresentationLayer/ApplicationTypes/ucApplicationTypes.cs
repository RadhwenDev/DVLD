using DVLD_BusinessLayer;
using DVLD_PresentationLayer.ApplicationTypes;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace DVLD_PresentationLayer
{
    public partial class ucApplicationTypes : UserControl
    {
        private DataTable _dtAllApplicationTypes;

        public ucApplicationTypes()
        {
            InitializeComponent();
        }

        private void _RefreshApplicationTypesList()
        {
            _dtAllApplicationTypes = clsApplicationType.GetAllApplicationTypes();
            dgvApplicationTypes.DataSource = _dtAllApplicationTypes;

            lblRecordsCount.Text = $"Total Records: {dgvApplicationTypes.Rows.Count}";

            if (dgvApplicationTypes.Rows.Count > 0)
            {
                dgvApplicationTypes.Columns[0].HeaderText = "ID";
                dgvApplicationTypes.Columns[0].Width = 100;

                dgvApplicationTypes.Columns[1].HeaderText = "Title";
                dgvApplicationTypes.Columns[1].Width = 350;

                dgvApplicationTypes.Columns[2].HeaderText = "Fees";
                dgvApplicationTypes.Columns[2].Width = 120;

                // زوز أرقام فقط بعد الفاصلة
                dgvApplicationTypes.Columns[2].DefaultCellStyle.Format = "N2";
            }
        }

        private void ucApplicationTypes_Load(object sender, EventArgs e)
        {
            _RefreshApplicationTypesList();
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dgvApplicationTypes.CurrentRow == null) return;

            int applicationTypeID = (int)dgvApplicationTypes.CurrentRow.Cells[0].Value;

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

                    // 1. إنشاء ucEditApplication للـ Update
                    ucEditApplication editControl = new ucEditApplication();

                    // 2. تحميل بيانات الـ Application المحدد
                    editControl.LoadApplicationTypeData(applicationTypeID);

                    frmContainer.Size = editControl.Size;
                    editControl.Dock = DockStyle.Fill;
                    frmContainer.Controls.Add(editControl);

                    // 3. تسكير الـ Container Form عند الحفظ أو Cancel
                    editControl.OnSaveCompleted += () => frmContainer.Close();
                    editControl.OnCancel += () => frmContainer.Close();

                    Guna.UI2.WinForms.Guna2Elipse elipse = new Guna.UI2.WinForms.Guna2Elipse();
                    elipse.TargetControl = frmContainer;
                    elipse.BorderRadius = 16;

                    frmContainer.ShowDialog(overlay);
                }
            }

            // تحديث القائمة بعد الغلق
            _RefreshApplicationTypesList();
        }

        private void dgvApplicationTypes_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (dgvApplicationTypes.Rows.Count == 0)
                {
                    dgvApplicationTypes.ContextMenuStrip = null;
                    return;
                }

                DataGridView.HitTestInfo hit = dgvApplicationTypes.HitTest(e.X, e.Y);

                if (hit.Type == DataGridViewHitTestType.Cell && hit.RowIndex >= 0)
                {
                    dgvApplicationTypes.ClearSelection();
                    dgvApplicationTypes.Rows[hit.RowIndex].Selected = true;
                    dgvApplicationTypes.CurrentCell = dgvApplicationTypes.Rows[hit.RowIndex].Cells[hit.ColumnIndex];
                    dgvApplicationTypes.ContextMenuStrip = guna2ContextMenuStrip1;
                }
                else
                {
                    dgvApplicationTypes.ContextMenuStrip = null;
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.FindForm()?.Close();
        }
    }
}