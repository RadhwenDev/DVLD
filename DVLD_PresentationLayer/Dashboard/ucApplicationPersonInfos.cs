using DVLD_BusinessLayer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DVLD_PresentationLayer.Dashboard
{
    public partial class ucApplicationPersonInfos : UserControl
    {
        public ucApplicationPersonInfos()
        {
            InitializeComponent();
        }

        public void LoadApplicationInfo(DataRow row)
        {
            lblFullName.Text = row["FullName"].ToString();
            string AppType = row["ApplicationTypeTitle"].ToString();
            string AppID = row["ApplicationID"].ToString(); // التصحيح هنا لاستخدام الـ ApplicationID
            lblAppType_AppID.Text = $"{AppType} · #{AppID}";

            if (DateTime.TryParse(row["ApplicationDate"].ToString(), out DateTime appDate))
                lblDateApp.Text = appDate.ToString("MMM dd", CultureInfo.InvariantCulture);
            else
                lblDateApp.Text = row["ApplicationDate"].ToString();

            string statusName = row["StatusName"].ToString();
            switch (statusName)
            {
                case "New":
                    statusBadge.Text = "New";
                    statusBadge.FillColor = Color.FromArgb(239, 246, 255);
                    statusBadge.ForeColor = Color.FromArgb(40, 90, 231);
                    statusBadge.BorderColor = Color.FromArgb(40, 90, 231);
                    break;
                case "Completed":
                    statusBadge.Text = "Completed";
                    statusBadge.FillColor = Color.FromArgb(232, 245, 233);
                    statusBadge.ForeColor = Color.FromArgb(56, 142, 60);
                    statusBadge.BorderColor = Color.FromArgb(56, 142, 60);
                    break;
                case "Cancelled":
                    statusBadge.Text = "Cancelled";
                    statusBadge.FillColor = Color.FromArgb(254, 234, 234);
                    statusBadge.ForeColor = Color.FromArgb(183, 28, 28);
                    statusBadge.BorderColor = Color.FromArgb(183, 28, 28);
                    break;
            }

            string imagePath = row["ImagePath"].ToString();
            if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
            {
                try
                {
                    using (var stream = new FileStream(imagePath, FileMode.Open, FileAccess.Read))
                    {
                        pbImagePerson.Image = System.Drawing.Image.FromStream(stream);
                    }
                }
                catch { }
            }
        }
    }
}
