using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DVLD_DataAccessLayer;

namespace DVLD_BusinessLayer
{
    public class clsUsers
    {
        public static DataTable getAllUsers()
        {
            return clsUsersDataAccess.getAllUsers();
        }
    }
}
