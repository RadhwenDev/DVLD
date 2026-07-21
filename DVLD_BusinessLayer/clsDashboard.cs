using DVLD_DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLD_BusinessLayer
{
    public class clsDashboard
    {
        public static int GetCompletedApplicationsThisMonth()
        {
            return clsDashboardDataAccess.GetCompletedApplicationsThisMonth();
        }

        public static int getPendingApplicants()
        {
            return clsDashboardDataAccess.getPendingApplicants();
        }

        public static int GetTotalPeople()
        {
            return clsDashboardDataAccess.GetTotalPeople();
        }

        public static int GetTotalPeopleInThisMonth()
        {
            return clsDashboardDataAccess.GetTotalPeopleInThisMonth();
        }

        public static DataTable GetApplicationPeopleInfo()
        {
            return clsDashboardDataAccess.GetApplicationPeopleInfo();
        }
        public static DataTable GetLicensePeopleInfo()
        {
            return clsDashboardDataAccess.GetLicensePeopleInfo();
        }
        public static DataTable GetServiceBreakdown()
        {
            return clsDashboardDataAccess.GetServiceBreakdown();
        }
    }
}
