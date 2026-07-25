using DVLD_DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLD_BusinessLayer
{
    public class clsDriver
    {
        public static DataTable getLicenseHistory(int AppID)
        {
            return clsDriverDataAccess.getLicenseHistory(AppID);
        }
        public static DataTable getLocalLicenseHistory(int AppID)
        {
            return clsDriverDataAccess.getLocalLicenseHistory(AppID);
        }
        public static DataTable getInternationalLicenseHistory(int AppID)
        {
            return clsDriverDataAccess.getInternationalLicenseHistory(AppID);
        }
    }
}
