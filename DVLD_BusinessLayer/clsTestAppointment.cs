using DVLD_DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLD_BusinessLayer
{
    public class clsTestAppointment
    {
        public static DataTable visionTest(int ApplicationID)
        {
            return clsTestAppointmentDataAccess.visionTest(ApplicationID);
        }
        public static DataTable visionTestDataGridView(int ApplicationID)
        {
            return clsTestAppointmentDataAccess.visionTestDataGridView(ApplicationID);
        }
        public static DataTable getTsetAppointment(int ApplicationID)
        {
            return clsTestAppointmentDataAccess.getTsetAppointment(ApplicationID);
        }
        public static DataTable getDataAppintment(int ApplicationID, int TestTypeID)
        {
            return clsTestAppointmentDataAccess.getDataAppintment(ApplicationID, TestTypeID);
        }
    }
}
