using DVLD_BusinessLayer;
using DVLD_PresentationLayer.Applications;
using DVLD_PresentationLayer.Dashboard;
using DVLD_PresentationLayer.DetainLicense;
using DVLD_PresentationLayer.Global;
using DVLD_PresentationLayer.InternationalApplication;
using DVLD_PresentationLayer.Licenses;
using DVLD_PresentationLayer.Login;
using DVLD_PresentationLayer.Tests;
using DVLD_PresentationLayer.User;
using DVLD_PresentationLayer.Users;
using Guna.UI2.WinForms;
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
        private Guna2Button activeSidebarButton = null;
        private ucDashboard _dashboard;
        public frmMain()
        {
            InitializeComponent();
            btnDashboard.PerformClick();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            lblDate.Text = DateTime.Today.ToString("ddd , MMM dd, yyyy", System.Globalization.CultureInfo.InvariantCulture);

            // Subscribe to User Data Changes Event
            clsCurrentUser.UserDataChanged += UpdateUserData;

            // Load initial user data and permissions
            UpdateUserData();
        }

        // ==========================================================
        // 🔑 Access Rights Checking & Bitmask Handling
        // ==========================================================
        private bool CheckAccessRights(int userPermissions, clsUsers.enPermissions permissionToCheck)
        {
            // 1. Super User (-1) bypasses all permission checks
            if (userPermissions == (int)clsUsers.enPermissions.SuperUser)
                return true;

            // 2. Bitwise AND check for normal permissions
            return (userPermissions & (int)permissionToCheck) == (int)permissionToCheck;
        }

        private void _ApplyUserPermissions()
        {
            if (clsCurrentUser.CurrentUser == null) return;

            int permissions = clsCurrentUser.CurrentUser.Permissions;

            // Store permissions logically in the Tag property
            btnPeople.Tag = CheckAccessRights(permissions, clsUsers.enPermissions.ManagePeople);
            btnUsers.Tag = CheckAccessRights(permissions, clsUsers.enPermissions.ManageUsers);
            // 💡 السماح بالوصول لزر الطلبات إذا كان لدى المستخدم أي صلاحية من الصلاحيات الثلاث:
            // (View Applications OR Manage Applications OR Manage Application Types)
            bool hasViewApplications = CheckAccessRights(permissions, clsUsers.enPermissions.ManageApplications);
            bool hasNewApplications = CheckAccessRights(permissions, clsUsers.enPermissions.ManageApplicationTypes);
            bool hasManageAppTypes = CheckAccessRights(permissions, clsUsers.enPermissions.ManageApplicationTypes); // أو التسمية الموجودة عندك في الـ Enum

            btnApplications.Tag = hasViewApplications || hasNewApplications || hasManageAppTypes; btnLicenseClasses.Tag = CheckAccessRights(permissions, clsUsers.enPermissions.ManageLicenseClasses);

            // 💡 Allow access to Licenses if user has EITHER Detain Licenses OR International Applications permission
            bool hasDetainedPermission = CheckAccessRights(permissions, clsUsers.enPermissions.ManageDetainedLicenses);
            bool hasInternationalPermission = CheckAccessRights(permissions, clsUsers.enPermissions.ManageInternationalApp);
            btnLicenses.Tag = hasDetainedPermission || hasInternationalPermission;

            btnLicenseClasses.Tag = CheckAccessRights(permissions, clsUsers.enPermissions.ManageLicenseClasses);
            btnTestTypes.Tag = CheckAccessRights(permissions, clsUsers.enPermissions.ManageTestTypes);

            // Dashboard is accessible to everyone
            btnDashboard.Tag = true;

            pnlSidebar.Invalidate(true);
        }

        private void MyAddPersonPage_DataBack(object sender, int PersonID)
        {
            if (clsCurrentUser.CurrentUser != null)
            {
                lblUser.Text = clsCurrentUser.CurrentUser.UserName;
            }
        }

        public void UpdateUserData()
        {
            if (clsCurrentUser.CurrentUser != null)
            {
                lblUser.Text = clsCurrentUser.CurrentUser.UserName;

                // 1. Apply access permissions
                _ApplyUserPermissions();

                // 2. Load User Profile Image safely with FileStream
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

        private void LoadDefaultAvatar()
        {
          /*  if (clsCurrentPerson.CurrentPerson != null && clsCurrentPerson.CurrentPerson.Gendor == 1)
            {
                pbUser.Image = Properties.Resources.Female_User_Avatar;
            }
            else
            {
                pbUser.Image = Properties.Resources.Male_User_Avatar;
            }*/
        }

        public void SetActiveSidebarButton(Guna2Button btn, string breadcrumbText)
        {
            activeSidebarButton = btn;
            lblBreadcrumb.Text = breadcrumbText;
            pnlSidebar.Refresh();
        }

        private void showUserControl(UserControl userControl)
        {
            pnlContainer.Controls.Clear();
            userControl.Dock = DockStyle.Fill;
            userControl.Visible = true;
            pnlContainer.Controls.Add(userControl);
            userControl.BringToFront();
        }

        // ==========================================================
        // 🛡️ Navigation Permission Guard
        // ==========================================================
        private bool _CanNavigate(Guna2Button btn)
        {
            bool hasPermission = btn.Tag == null || (bool)btn.Tag;
            if (!hasPermission)
            {
                MessageBox.Show("Access Denied! You do not have permission to access this feature.", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        // ==========================================================
        // 🖥️ Sidebar Navigation Events
        // ==========================================================
        private void btnDashboard_Click(object sender, EventArgs e)
        {
            if (!_CanNavigate(btnDashboard)) return;

            activeSidebarButton = btnDashboard;
            lblBreadcrumb.Text = "DVLD > Dashboard";

            //ucDashboard myDashboardPage = new ucDashboard();
            if (_dashboard == null)
            {
                _dashboard = new ucDashboard();
            }


            showUserControl(_dashboard);

            pnlContainer.BringToFront();
            pnlSidebar.Refresh();
        }

        private void btnPeople_Click(object sender, EventArgs e)
        {
            if (!_CanNavigate(btnPeople)) return;

            activeSidebarButton = btnPeople;
            lblBreadcrumb.Text = "DVLD > People";
            ucPeople myPeoplePage = new ucPeople();
            showUserControl(myPeoplePage);
            pnlSidebar.Refresh();
        }

        private void btnUsers_Click(object sender, EventArgs e)
        {
            if (!_CanNavigate(btnUsers)) return;

            activeSidebarButton = btnUsers;
            lblBreadcrumb.Text = "DVLD > Users";
            ucUsers myUsersPage = new ucUsers();
            showUserControl(myUsersPage);
            pnlSidebar.Refresh();
        }

        public void btnApplications_Click(object sender, EventArgs e)
        {
            if (!_CanNavigate(btnApplications)) return;
            if (clsCurrentUser.CurrentUser == null) return;

            int permissions = clsCurrentUser.CurrentUser.Permissions;

            bool hasViewApp = CheckAccessRights(permissions, clsUsers.enPermissions.ManageApplications);
            bool hasNewApp = CheckAccessRights(permissions, clsUsers.enPermissions.NewApplications); // أو ProcessApplications
            bool hasAppTypes = CheckAccessRights(permissions, clsUsers.enPermissions.ManageApplicationTypes);

            activeSidebarButton = btnApplications;

            // 1. إذا كان لديه صلاحية العرض الشاملة (أو كل الصلاحيات)
            if (hasViewApp)
            {
                lblBreadcrumb.Text = "DVLD > Applications";
                ucApplications myApplicationsPage = new ucApplications();
                showUserControl(myApplicationsPage);
            }
            // 2. إذا كان يمتلك فقط صلاحية إضافة طلب جديد
            else if (hasNewApp)
            {
                MessageBox.Show("You only have permission to create/process new applications. Redirecting...",
                                "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);

                lblBreadcrumb.Text = "DVLD > Applications > New Application";
                ucNewApplication myNewAppPage = new ucNewApplication();
                showUserControl(myNewAppPage);
            }
            // 3. إذا كان يمتلك فقط صلاحية إدارة أنواع الطلبات والرسوم
            else if (hasAppTypes)
            {
                MessageBox.Show("You only have permission to Manage Application Types. Redirecting...",
                                "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);

                lblBreadcrumb.Text = "DVLD > Applications > Application Types";
                // توجيه مباشرة لشاشة أنواع الطلبات
                ucApplicationTypes myApplicationTypes = new ucApplicationTypes();
                showUserControl(myApplicationTypes);
            }

            pnlSidebar.Refresh();
        }

        public void btnNewApplication_Click(object sender, EventArgs e)
        {
            // 🛡️ 1. فحص صلاحية إضافة/معالجة طلب جديد
            if (!CheckAccessRights(clsCurrentUser.CurrentUser.Permissions, clsUsers.enPermissions.NewApplications))
            {
                MessageBox.Show("Access Denied! You do not have permission to create or process new applications.",
                                "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            activeSidebarButton = btnApplications;
            lblBreadcrumb.Text = "DVLD > Applications > New Application";
            ucNewApplication myNewApplicationsPage = new ucNewApplication();
            showUserControl(myNewApplicationsPage);
            pnlSidebar.Refresh();
        }

        public void btnLicenses_Click(object sender, EventArgs e)
        {
            if (!_CanNavigate(btnLicenses)) return;

            if (clsCurrentUser.CurrentUser == null) return;

            int permissions = clsCurrentUser.CurrentUser.Permissions;

            bool hasDetainedPermission = CheckAccessRights(permissions, clsUsers.enPermissions.ManageDetainedLicenses);
            bool hasInternationalPermission = CheckAccessRights(permissions, clsUsers.enPermissions.ManageInternationalApp);

            activeSidebarButton = btnLicenses;

            // 1. إذا كان لديه كلا الصلاحيتين: افتح الصفحة الرئيسية للرخص (أو الواجهة المزدوجة)
            if (hasDetainedPermission && hasInternationalPermission)
            {
                lblBreadcrumb.Text = "DVLD > Licenses";
                ucLicenses myLicensesPage = new ucLicenses();
                showUserControl(myLicensesPage);
                myLicensesPage.OnLicenseReplaced += () =>
                {
                    _dashboard?.RefreshDashboard();
                };
                myLicensesPage.OnLicenseReleased += () =>
                {
                    _dashboard?.RefreshDashboard();
                };
            }
            // 2. إذا كان لديه صلاحية الرخص المحجوزة فقط (Detained Licenses)
            else if (hasDetainedPermission)
            {
                MessageBox.Show("You only have permission to access 'Detained Licenses'. Redirecting...",
                        "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
                lblBreadcrumb.Text = "DVLD > Licenses > Detained Licenses";
                // 💡 استدعِ واجهة الرخص المحجوزة مباشرة هنا
                ucDetainLicense myDetainedPage = new ucDetainLicense();
                showUserControl(myDetainedPage);
                myDetainedPage.OnLicenseReleased += () =>
                {
                    _dashboard?.RefreshDashboard();
                };

            }
            // 3. إذا كان لديه صلاحية الرخص الدولية فقط (International Applications)
            else if (hasInternationalPermission)
            {
                MessageBox.Show("You only have permission to access 'International Applications'. Redirecting...",
                        "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
                lblBreadcrumb.Text = "DVLD > Licenses > International Applications";
                ucInternationalApplication myInternationalPage = new ucInternationalApplication();
                showUserControl(myInternationalPage);

            }

            pnlSidebar.Refresh();
        }

        private void btnLicenseClasses_Click(object sender, EventArgs e)
        {
            // التحقق المباشر من صلاحية إدارة أصناف الرخص (16)
            if (!CheckAccessRights(clsCurrentUser.CurrentUser.Permissions, clsUsers.enPermissions.ManageLicenseClasses))
            {
                MessageBox.Show("Access Denied! You do not have permission to access License Classes.", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            activeSidebarButton = btnLicenseClasses;
            lblBreadcrumb.Text = "DVLD > License Classes";
            ucLicenseClasses myLicenseClassesPage = new ucLicenseClasses();
            showUserControl(myLicenseClassesPage);
            pnlSidebar.Refresh();
        }

        private void btnTestTypes_Click(object sender, EventArgs e)
        {
            if (!_CanNavigate(btnTestTypes)) return;

            activeSidebarButton = btnTestTypes;
            lblBreadcrumb.Text = "DVLD > Test Types";
            ucTestTypes muTestTypesPage = new ucTestTypes();
            showUserControl(muTestTypesPage);
            pnlSidebar.Refresh();
        }

        // ==========================================================
        // 🧙‍♂️ New Application Wizard Handling
        // ==========================================================
        public void OpenNewApplicationWizard()
        {
            // 🛡️ فحص نفس الصلاحية للـ Wizard
            if (!CheckAccessRights(clsCurrentUser.CurrentUser.Permissions, clsUsers.enPermissions.NewApplications))
            {
                MessageBox.Show("Access Denied! You do not have permission to create or process new applications.",
                                "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SetActiveSidebarButton(btnApplications, "DVLD > Applications > New Application");
            ucNewApplication myNewApplication = new ucNewApplication
            {
                Dock = DockStyle.Fill,
                Name = "ucNewApplicationWizard"
            };

            myNewApplication.OnApplicationSaved += MyNewApplication_OnApplicationSaved;

            foreach (Control ctrl in pnlContainer.Controls)
            {
                ctrl.Visible = false;
            }

            pnlContainer.Controls.Add(myNewApplication);
            myNewApplication.BringToFront();
        }
        private void MyNewApplication_OnApplicationSaved(object sender, int ApplicationID)
        {
            Control wizardCtrl = pnlContainer.Controls["ucNewApplicationWizard"];
            if (wizardCtrl != null)
            {
                pnlContainer.Controls.Remove(wizardCtrl);
                wizardCtrl.Dispose();
            }

            SetActiveSidebarButton(btnApplications, "DVLD > Applications");

            pnlContainer.Controls.Clear();
            ucApplications appPage = new ucApplications
            {
                Dock = DockStyle.Fill
            };
            pnlContainer.Controls.Add(appPage);
            appPage.BringToFront();
            _dashboard?.RefreshDashboard();
        }

        // ==========================================================
        // 🎨 Custom UI Button Painting (Unified Clean Look)
        // ==========================================================
        private void DesignButton(object sender, PaintEventArgs e)
        {
            Guna2Button btn = (Guna2Button)sender;

            // 1. Active Selected Button state
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
            // 2. Normal Button state
            else
            {
                btn.FillColor = Color.Transparent;
                btn.ForeColor = Color.FromArgb(226, 232, 240);
                btn.HoverState.FillColor = Color.FromArgb(45, 52, 71);
                btn.HoverState.ForeColor = Color.White;
            }
        }

        private void btnPeople_Paint(object sender, PaintEventArgs e) => DesignButton(sender, e);
        private void btnUsers_Paint_1(object sender, PaintEventArgs e) => DesignButton(sender, e);
        private void btnDashboard_Paint(object sender, PaintEventArgs e) => DesignButton(sender, e);
        private void btnApplications_Paint(object sender, PaintEventArgs e) => DesignButton(sender, e);
        private void btnLicenses_Paint(object sender, PaintEventArgs e) => DesignButton(sender, e);
        private void btnLicenseClasses_Paint(object sender, PaintEventArgs e) => DesignButton(sender, e);
        private void btnTestTypes_Paint(object sender, PaintEventArgs e) => DesignButton(sender, e);

        // ==========================================================
        // 👤 User Profile Avatar & Context Menu Actions
        // ==========================================================
        private void pbUser_Click(object sender, EventArgs e)
        {
            guna2ContextMenuStrip1.Show(pbUser, new Point(0, -guna2ContextMenuStrip1.Height));
        }

        private void logoutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to logout?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (clsCurrentUser.CurrentUser != null)
                {
                    clsUsers.Logout(clsCurrentUser.CurrentUser.UserID);
                }

                clsCurrentUser.CurrentUser = null;
                clsCurrentPerson.CurrentPerson = null;
                    
                this.Hide();
                frmLogin loginForm = new frmLogin();

                if (loginForm.ShowDialog() == DialogResult.OK)
                {
                    UpdateUserData();
                    btnDashboard.PerformClick();
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