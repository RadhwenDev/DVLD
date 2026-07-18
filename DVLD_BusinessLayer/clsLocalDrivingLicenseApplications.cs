using DVLD_DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLD_BusinessLayer
{
    public class clsLocalDrivingLicenseApplications
    {
        public static int AddNewLocalDrivingLicenseApplications(int ApplicationID, int LicenseClassID)
        {
            return clsLocalDrivingLicenseApplicationsDataAccess.AddNewLocalDrivingLicenseApplications(ApplicationID, LicenseClassID);
        }
    }
}
