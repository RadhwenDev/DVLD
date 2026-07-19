using DVLD_DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static DVLD_BusinessLayer.clsPerson;

namespace DVLD_BusinessLayer
{
    public class clsUsers
    {
        private clsUsers _OriginalUser;
        public enum enSaveResult
        {
            SavedSuccessfully,
            NoChanges,
            Failed
        }
        enum enMode { AddNew, Update };
        enMode Mode;
        public int UserID { set; get; }
        public int PersonID { set; get; }
        public string UserName { set; get; }
        public string Password { set; get; }
        public int Permissions {  set; get; }
        public bool isActive { set; get; }

        public clsUsers()
        {
            this.UserID = -1;
            this.PersonID = -1;
            this.UserName = "";
            this.Password = "";
            this.Permissions = 0;
            this.isActive = false;
            Mode = enMode.AddNew;
        }
        public clsUsers(int UserID, int PersonID, string UserName, string Password, int Permission, bool isActive)
        {
            this.UserID = UserID;
            this.PersonID = PersonID;
            this.UserName = UserName;
            this.Password = Password;
            this.Permissions = Permission;
            this.isActive = isActive;

            _OriginalUser = new clsUsers();
            _OriginalUser.UserID = UserID;
            _OriginalUser.PersonID = PersonID;
            _OriginalUser.UserName = UserName;
            _OriginalUser.Password = Password;
            _OriginalUser.isActive = isActive;
            _OriginalUser.Permissions = Permissions;

            Mode = enMode.Update;
        }


        public static DataTable getAllUsers()
        {
            return clsUsersDataAccess.getAllUsers();
        }

        public static DataTable getAllDetailsForShowButton(int UserID)
        {
            return clsUsersDataAccess.getAllDetailsForShowButton(UserID);
        }

        public bool AddNewPerson()
        {
            this.PersonID = clsUsersDataAccess.AddNewUser(this.PersonID, this.UserName, this.Password, this.isActive, this.Permissions);
            return (this.PersonID != -1);
        }
        public enSaveResult UpdatePerson()
        {
            if (clsUsersDataAccess.UpdateUser(this.UserID, this.UserName, this.isActive, this.Permissions))
            {
                _OriginalUser.UserID = this.UserID;
                _OriginalUser.UserName = this.UserName;
                _OriginalUser.isActive = this.isActive;
                _OriginalUser.Permissions = this.Permissions;
                return enSaveResult.SavedSuccessfully;
            }
            return enSaveResult.Failed;
        }

        public enSaveResult UpdateUserPassword()
        {
            if (clsUsersDataAccess.UpdateUserPassword(this.UserID, this.Password))
            {
                _OriginalUser.Password = this.Password;
                return enSaveResult.SavedSuccessfully;
            }
            return enSaveResult.Failed;
        }

        public static bool IsUserExistForPersonID(int PersonID)
        {
            return clsUsersDataAccess.IsUserExistForPersonID(PersonID);
        }

        public static bool IsUserNameExistForPersonID(string UserName)
        {
            return clsUsersDataAccess.IsUserNameExistForPersonID(UserName);
        }

        public static clsUsers Find(int UserID)
        {
            string UserName = "", Password = "";
            int PersonID = -1, Permissions = 0;
            bool isActive = false;
            if (clsUsersDataAccess.GetUserInfoByID(UserID, ref PersonID, ref UserName, ref Password, ref Permissions, ref isActive))
                return new clsUsers(UserID, PersonID, UserName, Password, Permissions, isActive);
            return null;
        }

        public static clsUsers Find(string UserName, string Password)
        {
            int UserID = -1, PersonID = -1, Permissions = 0;
            string hashedPasswordFromDB = "";
            bool isActive = false;

            // نجلب بيانات المستخدم بالـ اسم فقط
            if (clsUsersDataAccess.GetUserInfoByUserName(UserName, ref UserID, ref PersonID, ref hashedPasswordFromDB, ref Permissions, ref isActive))
            {
                string hashedInput = clsCryptoSettings.ComputeSha256Hash(Password.Trim());

                if (hashedInput == hashedPasswordFromDB)
                {
                    return new clsUsers(UserID, PersonID, UserName, hashedPasswordFromDB, Permissions, isActive);
                }
            }
            return null;
        }

        public enSaveResult SavePassword()
        {
            if (!isPasswordChanged())
                return enSaveResult.NoChanges;
            return UpdateUserPassword();
        }

        public enSaveResult Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    if (AddNewPerson())
                    {
                        Mode = enMode.Update;
                        return enSaveResult.SavedSuccessfully;
                    }
                    else
                    {
                        return enSaveResult.Failed;
                    }
                case enMode.Update:
                    if (!HasChanges())
                        return enSaveResult.NoChanges;

                    return UpdatePerson();

            }
            return enSaveResult.Failed;
        }
        private bool HasChanges()
        {
            return
                UserName != _OriginalUser.UserName ||
                Permissions != _OriginalUser.Permissions ||
                isActive != _OriginalUser.isActive;
        }
        private bool isPasswordChanged()
        {
            return Password != _OriginalUser.Password;

        }
    }
}
