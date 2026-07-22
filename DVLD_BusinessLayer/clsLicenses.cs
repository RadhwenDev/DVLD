using DVLD_DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace DVLD_BusinessLayer
{
    public class clsLicenses
    {
        public static DataTable getAllLicenses()
        {
            return clsLicensesDataAccess.getAllLicenses();
        }

        public static int getTotalActiveLicenses()
        {
            return clsLicensesDataAccess.getTotalActiveLicenses();
        }

        public static DataTable getShowLicense(int ApplicationID)
        {
            return clsLicensesDataAccess.getShowLicense(ApplicationID);
        }
    }
}
