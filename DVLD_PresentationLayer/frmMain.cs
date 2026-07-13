using DVLD_PresentationLayer.Applications;
using DVLD_PresentationLayer.Global;
using DVLD_PresentationLayer.Tests;
using DVLD_PresentationLayer.User;
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
            /*blUser.Text = clsCurrentUser.CurrentUser.UserName;
            if (!string.IsNullOrEmpty(clsCurrentPerson.CurrentPerson.ImagePath) && File.Exists(clsCurrentPerson.CurrentPerson.ImagePath))
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
            }*/
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
            /*ucUsers myUsersPage = new ucUsers();
            showUserControl(myUsersPage);*/
            pnlSidebar.Refresh();
        }

        private void btnDashboard_Paint(object sender, PaintEventArgs e)
        {
            DesignButton(sender, e);
        }

        private void btnApplications_Click(object sender, EventArgs e)
        {
            activeSidebarButton = btnApplications;
            lblBreadcrumb.Text = "DVLD > Applications";
            ucApplications myApplicationsPage = new ucApplications();
            showUserControl(myApplicationsPage);
            pnlSidebar.Refresh();
        }

        private void btnLicenses_Click(object sender, EventArgs e)
        {
            activeSidebarButton = btnLicenses;
            lblBreadcrumb.Text = "DVLD > Licenses";
            /*ucUsers myUsersPage = new ucUsers();
            showUserControl(myUsersPage);*/
            pnlSidebar.Refresh();
        }

        private void btnLicenseClasses_Click(object sender, EventArgs e)
        {
            activeSidebarButton = btnLicenses;
            lblBreadcrumb.Text = "DVLD > License Classes";
            /*ucUsers myUsersPage = new ucUsers();
            showUserControl(myUsersPage);*/
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
    }
}
