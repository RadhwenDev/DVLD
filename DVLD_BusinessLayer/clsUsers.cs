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
            Mode = enMode.Update;
        }


        public static DataTable getAllUsers()
        {
            return clsUsersDataAccess.getAllUsers();
        }

        public bool AddNewPerson()
        {
            this.PersonID = clsUsersDataAccess.AddNewUser(this.PersonID, this.UserName, this.Password, this.isActive, this.Permissions);
            return (this.PersonID != -1);
        }
        public enSaveResult UpdatePerson()
        {
            if (clsUsersDataAccess.UpdateUser(this.UserID, this.UserName, this.Password, this.isActive, this.Permissions))
                return enSaveResult.SavedSuccessfully;
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
            return true;
                /*FirstName != _OriginalPerson.FirstName ||
                SecondName != _OriginalPerson.SecondName ||
                ThirdName != _OriginalPerson.ThirdName ||
                LastName != _OriginalPerson.LastName ||
                NationalNo != _OriginalPerson.NationalNo ||
                Phone != _OriginalPerson.Phone ||
                Email != _OriginalPerson.Email ||
                Address != _OriginalPerson.Address ||
                DateOfBirth != _OriginalPerson.DateOfBirth.Date ||
                Gendor != _OriginalPerson.Gendor ||
                NationalCountryID != _OriginalPerson.NationalCountryID ||
                ImagePath != _OriginalPerson.ImagePath;*/
        }
    }
}
