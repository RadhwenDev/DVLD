using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace DVLD_PresentationLayer.Dashboard
{
    public partial class ucServiceBreakdown : UserControl
    {
        public ucServiceBreakdown()
        {
            InitializeComponent();
        }

        public void LoadServiceInfo(DataRow row)
        {
            int Count = int.Parse(row["TotalCount"].ToString());
            int Total = int.Parse(row["OverallTotal"].ToString());
            lblTotal.Text = Count.ToString();
            lblAppTypeTitle.Text = row["ServiceName"].ToString();
            if (Total > 0)
            {
                int percentage = (int)((double)Count / Total * 100);
                pbAppTypeTitle.Value = Math.Min(percentage, 100);
            }
            else
            {
                pbAppTypeTitle.Value = 0;
            }
        }
    }
}
