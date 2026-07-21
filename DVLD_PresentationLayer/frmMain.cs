using DVLD_BusinessLayer;
using DVLD_PresentationLayer.Applications;
using DVLD_PresentationLayer.Dashboard;
using DVLD_PresentationLayer.Global;
using DVLD_PresentationLayer.Licenses;
using DVLD_PresentationLayer.Login;
using DVLD_PresentationLayer.Tests;
using DVLD_PresentationLayer.User;
using DVLD_PresentationLayer.Users;
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

namespace DVLD_PresentationLayer
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
            btnDashboard.PerformClick();
        }
        bool isSidebarExpanded = true;
        private void btnMenu_Click(object sender, EventArgs e)
        {
            if (isSidebarExpanded)
            {
                pnlSidebar.Width = 0;
                btnMenu.Text = "≡";
                btnMenu.Font = new Font("Arial", 18, FontStyle.Bold);
                btnMenu.ForeColor = Color.Black;
                isSidebarExpanded = false; 
            }
            else
            {
                pnlSidebar.Width = 260;
                btnMenu.Text = "✖️";
                btnMenu.Font = new Font("Arial", 12, FontStyle.Bold);
                btnMenu.ForeColor = Color.Black;

                isSidebarExpanded = true;
            }
            btnMenu.Invalidate();
            btnMenu.Update();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            lblDate.Text = DateTime.Today.Date.ToString("ddd , MMM dd, yyyy", System.Globalization.CultureInfo.InvariantCulture);

            // استدعاء دالة التحديث لتعبئة البيانات أول ما يفتح البرنامج
            UpdateUserData();
        }

        private void UpdateUserData()
        {
            // التأكد أن الـ CurrentUser ليس فارغاً تجنباً للـ Exception
            if (clsCurrentUser.CurrentUser != null)
            {
                lblUser.Text = clsCurrentUser.CurrentUser.UserName;

                // التحقق من وجود الشخص وصورته
                if (clsCurrentPerson.CurrentPerson != null &&
                    !string.IsNullOrEmpty(clsCurrentPerson.CurrentPerson.ImagePath) &&
                    File.Exists(clsCurrentPerson.CurrentPerson.ImagePath))
                {
                    try
                    {
                        using (var stream = new FileStream(clsCurrentPerson.CurrentPerson.ImagePath, FileMode.Open, FileAccess.Read))
                        {
                            pbUser.Image = Image.FromStream(stream);
                        }
                    }
                    catch
                    {
                        LoadDefaultAvatar();
                    }
                }
                else
                {
                    LoadDefaultAvatar();
                }
            }
        }


        // دالة وضع الصورة الافتراضية المفعلة الآن بالكامل بناءً على الجنس
        private void LoadDefaultAvatar()
        {
            /* if (_Person != null)
             {
                 if (_Person.Gendor == 0)
                     pbImage.Image = Properties.Resources.default_male_avatar; // تأكد من مطابقة الاسم في الـ Resources لديك
                 else
                     pbImage.Image = Properties.Resources.default_female_avatar;
             }*/
        }

        Guna.UI2.WinForms.Guna2Button activeSidebarButton = null;

        private void btnPeople_Click(object sender, EventArgs e)
        {
            activeSidebarButton = btnPeople;
            lblBreadcrumb.Text = "DVLD > People";
            ucPeople myPeoplePage = new ucPeople();
            showUserControl(myPeoplePage);
            pnlSidebar.Refresh();
        }

        private void DesignButton(object sender, PaintEventArgs e)
        {
            Guna.UI2.WinForms.Guna2Button btn = (Guna.UI2.WinForms.Guna2Button)sender;
            if (btn == activeSidebarButton)
            {
                Color activeColor = Color.FromArgb(37, 99, 235);
                btn.FillColor = activeColor;
                btn.ForeColor = Color.White;

                btn.HoverState.FillColor = activeColor;
                btn.HoverState.ForeColor = Color.White;

                string arrow = ">";
                Font font = new Font("Arial", 11, FontStyle.Bold);
                Brush brush = Brushes.White;

                int x = btn.Width - 25;
                int y = (btn.Height - (int)e.Graphics.MeasureString(arrow, font).Height) / 2;

                e.Graphics.DrawString(arrow, font, brush, x, y);
            }
            else
            {
                btn.FillColor = Color.Transparent;
                btn.ForeColor = Color.FromArgb(226, 232, 240);
                btn.HoverState.FillColor = Color.FromArgb(45, 52, 71);
                btn.HoverState.ForeColor = Color.White;
            }
        }

        private void btnPeople_Paint(object sender, PaintEventArgs e)
        {
            DesignButton(sender, e);
        }

        private void showUserControl(UserControl userControl)
        {
            pnlContainer.Controls.Clear();

            userControl.Dock = DockStyle.Fill;

            pnlContainer.Controls.Add(userControl);

            userControl.BringToFront();
        }

        private void btnUsers_Click(object sender, EventArgs e)
        {
            activeSidebarButton = btnUsers;
            lblBreadcrumb.Text = "DVLD > Users";
            ucUsers myUsersPage = new ucUsers();
            showUserControl(myUsersPage);
            pnlSidebar.Refresh();
        }

        private void btnUsers_Paint_1(object sender, PaintEventArgs e)
        {
            DesignButton(sender, e);
        }
        
        private void btnDashboard_Click(object sender, EventArgs e)
        {
            activeSidebarButton = btnDashboard;
            lblBreadcrumb.Text = "DVLD > Dashboard";
            ucDashboard myDashboardPage = new ucDashboard();
            showUserControl(myDashboardPage);
            pnlContainer.BringToFront();
            pnlSidebar.Refresh();
        }

        private void btnDashboard_Paint(object sender, PaintEventArgs e)
        {
            DesignButton(sender, e);
        }

        public void btnApplications_Click(object sender, EventArgs e)
        {
            activeSidebarButton = btnApplications;
            lblBreadcrumb.Text = "DVLD > Applications";
            ucApplications myApplicationsPage = new ucApplications();
            showUserControl(myApplicationsPage);
            pnlSidebar.Refresh();
        }

        public void btnNewApplication_Click(object sender, EventArgs e)
        {
            activeSidebarButton = btnApplications;
            lblBreadcrumb.Text = "DVLD > Applications";
            ucNewApplication myNewApplicationsPage = new ucNewApplication();
            showUserControl(myNewApplicationsPage);
            pnlSidebar.Refresh();
        }

        public void btnLicenses_Click(object sender, EventArgs e)
        {
            activeSidebarButton = btnLicenses;
            lblBreadcrumb.Text = "DVLD > Licenses";
            ucLicenses myLicensesPage = new ucLicenses();
            showUserControl(myLicensesPage);
            pnlSidebar.Refresh();
        }

        private void btnLicenseClasses_Click(object sender, EventArgs e)
        {
            activeSidebarButton = btnLicenseClasses;
            lblBreadcrumb.Text = "DVLD > License Classes";
            ucLicenseClasses myLicenseClassesPage = new ucLicenseClasses();
            showUserControl(myLicenseClassesPage);
            pnlSidebar.Refresh();
        }

        private void btnTestTypes_Click(object sender, EventArgs e)
        {
            activeSidebarButton = btnTestTypes;
            lblBreadcrumb.Text = "DVLD > Test Types";
            ucTestTypes muTestTypesPage = new ucTestTypes();
            showUserControl(muTestTypesPage);
            pnlSidebar.Refresh();
        }

        private void btnApplications_Paint(object sender, PaintEventArgs e)
        {
            DesignButton(sender, e);
        }

        private void btnLicenses_Paint(object sender, PaintEventArgs e)
        {
            DesignButton(sender, e);
        }

        private void btnLicenseClasses_Paint(object sender, PaintEventArgs e)
        {
            DesignButton(sender, e);
        }

        private void btnTestTypes_Paint(object sender, PaintEventArgs e)
        {
            DesignButton(sender, e);
        }

        private void pbUser_Click(object sender, EventArgs e)
        {
            guna2ContextMenuStrip1.Show(pbUser, new Point(0, -guna2ContextMenuStrip1.Height));
        }

        private void logoutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to logout?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                // 1. تنظيف الـ Remember Me
                if (clsCurrentUser.CurrentUser != null)
                {
                    clsUsers.Logout(clsCurrentUser.CurrentUser.UserID);
                }

                // 2. تنظيف الذاكرة
                clsCurrentUser.CurrentUser = null;
                clsCurrentPerson.CurrentPerson = null;

                // 3. العودة لشاشة الـ Login
                this.Hide();
                frmLogin loginForm = new frmLogin();

                if (loginForm.ShowDialog() == DialogResult.OK)
                {
                    // <-- هنا الحل: قم بتحديث واجهة المستخدم بالبيانات الجديدة للمستخدم الذي سجل دخول للتو -->
                    UpdateUserData();

                    this.Show();
                }
                else
                {
                    this.Close();
                }
            }
        }

        private void showDetailsToolStripMenuItem_Click(object sender, EventArgs e)
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

                    ucShowDetailsUser myShowDetailsUserPage = new ucShowDetailsUser(clsCurrentUser._UserID);
                    frmContainer.Size = myShowDetailsUserPage.Size;
                    myShowDetailsUserPage.Dock = DockStyle.Fill;
                    frmContainer.Controls.Add(myShowDetailsUserPage);


                    Guna.UI2.WinForms.Guna2Elipse elipse = new Guna.UI2.WinForms.Guna2Elipse();
                    elipse.TargetControl = frmContainer;
                    elipse.BorderRadius = 16;

                    frmContainer.ShowDialog(overlay);
                }
            }
        }

        
    }
}
