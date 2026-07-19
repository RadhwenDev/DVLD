using DVLD_DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLD_BusinessLayer
{
    public class clsApplicant
    {
        public static DataTable getAllApplicants()
        {
            return clsApplicationsDataAccess.getAllApplicants();
        }
        public static DataTable getAllApplicationTypes()
        {
            return clsApplicationsDataAccess.getAllApplicationType();
        }
        public static DataTable getAllApplicationTypes(int ApplicantPersonID)
        {
            return clsApplicationsDataAccess.getAllApplicationTypes(ApplicantPersonID);
        }


        public static DataTable getAllDetailsForShowButton(int ApplicationID)
        {
            return clsApplicationsDataAccess.getAllDetailsForShowButton(ApplicationID);
        }
        public static DataTable getApplicationTypesTitle_Fees(int ApplicationTypeID)
        {
            return clsApplicationsDataAccess.getApplicationTypesTitle_Fees(ApplicationTypeID);
        }
        public static int AddNewApplication(int ApplicantPersonID, DateTime ApplicationDate, int ApplicationTypeID, byte ApplicationStatus, DateTime LastStatusDate, decimal PaidFees, int CreatedByUserID)
        {
            return clsApplicationsDataAccess.AddNewApplication(ApplicantPersonID, ApplicationDate, ApplicationTypeID, ApplicationStatus, LastStatusDate, PaidFees, CreatedByUserID);
        }

        public static DataTable getAllApplicationStatus()
        {
            return clsApplicationsDataAccess.getAllApplicationStatus();
        }
    }
}
