using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DVLD_PresentationLayer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
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
                btnMenu.Text = "X";
                btnMenu.Font = new Font("Arial", 12, FontStyle.Bold);
                btnMenu.ForeColor = Color.Black;

                isSidebarExpanded = true;
            }
            btnMenu.Invalidate();
            btnMenu.Update();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            lblDate.Text = DateTime.Today.Date.ToString("dddd , MMM dd, yyyy");
        }

        Guna.UI2.WinForms.Guna2Button activeSidebarButton = null;

        private void btnPeople_Click(object sender, EventArgs e)
        {
            activeSidebarButton = btnPeople;
            lblBreadcrumb.Text = "DVLD > People";
        }

        private void btnPeople_Paint(object sender, PaintEventArgs e)
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

    }
}
