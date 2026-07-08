using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
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
            dgvUsers.DataSource = _dtAllUsers;

            if (dgvUsers.Rows.Count > 0)
            {
                // يمكنك هنا تنسيق أسماء الأعمدة وعرضها كما تريد
                dgvUsers.Columns["UserName"].HeaderText = "User Name";

                // نصيحة: إذا كنت تريد جعل الجدول يملأ الشاشة بشكل جميل ومطابق للـ UI الخاص بك:
                dgvUsers.Columns["UserName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
        }
    }
}