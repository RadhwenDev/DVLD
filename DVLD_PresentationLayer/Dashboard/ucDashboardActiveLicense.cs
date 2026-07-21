using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DVLD_PresentationLayer.Dashboard
{
    public partial class ucDashboardActiveLicense : UserControl
    {
        public ucDashboardActiveLicense()
        {
            InitializeComponent();
        }

        public void LoadLicenseInfo(DataRow row)
        {
            lblFullName.Text = row["FullName"].ToString();
            lblLicenseID.Text = '#' + row["LicenseID"].ToString();
            if (DateTime.TryParse(row["ExpirationDate"].ToString(), out DateTime appDate))
                lblDateExpiration.Text = appDate.ToString("MMM dd, yyyy", CultureInfo.InvariantCulture);
            else
                lblDateExpiration.Text = row["ExpirationDate"].ToString();
        }
    }
}
