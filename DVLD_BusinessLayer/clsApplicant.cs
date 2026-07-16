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
            return clsApplicationsDataAccess.getAllApplicationTypes();
        }

        public static DataTable getAllApplicationStatus()
        {
            return clsApplicationsDataAccess.getAllApplicationStatus();
        }
    }
}
