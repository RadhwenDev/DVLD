using DVLD_BusinessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLD_PresentationLayer.Global
{
    public static class clsCurrentUser
    {
        public static clsUsers CurrentUser { get; set; }
        public static int _UserID = CurrentUser.UserID;
    }
}
