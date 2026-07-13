using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DVLD_DataAccessLayer;


namespace DVLD_BusinessLayer
{
    public class clsLoginLog
    {
        public static bool RegisterLogin(int UserID)
        {
            return clsLoginLogData.AddLoginLog(UserID) > 0;
        }
    }
}
