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
using DVLD_Security;

namespace DVLD_BusinessLayer
{
    public class clsUsers
    {
        [Flags]
        public enum enPermissions
        {
            SuperUser = -1,
            None = 0,
            ManagePeople = 1,
            ManageUsers = 2,
            ManageApplications = 4,
            NewApplications = 8,
            ManageApplicationTypes = 16,
            ManageTestTypes = 32,
            ManageLicenseClasses = 64,
            ManageDetainedLicenses = 128,
            ManageInternationalApp = 256,
            AuditLogs = 512,

            FullPermissions = ManagePeople | ManageUsers | ManageInternationalApp |
                              ManageApplications | ManageLicenseClasses | ManageTestTypes |
                              NewApplications | ManageDetainedLicenses | ManageApplicationTypes | AuditLogs
        }

        private clsUsers _OriginalUser;
        private const int RememberMeDays = 30;

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
        public int Permissions { set; get; }
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

        public clsUsers(int UserID)
        {
            this.UserID = UserID;

            // نستدعي الدالة Find لجلب البيانات من قاعدة البيانات وتعبئة الـ Properties
            clsUsers user = Find(UserID);

            if (user != null)
            {
                this.PersonID = user.PersonID;
                this.UserName = user.UserName;
                this.Password = user.Password;
                this.Permissions = user.Permissions;
                this.isActive = user.isActive;

                // ونحفظ نسخة منها في الـ OriginalUser
                _OriginalUser = new clsUsers(UserID, PersonID, UserName, Password, Permissions, isActive);
                Mode = enMode.Update;
            }
            else
            {
                Mode = enMode.AddNew;
            }
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
            this.UserID = clsUsersDataAccess.AddNewUser(this.PersonID, this.UserName, this.Password, this.isActive, this.Permissions);
            return (this.UserID != -1);
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

                // تحسين: الأمن أولاً، تغيير كلمة السر يلغي الجلسة المحفوظة
                clsUsersDataAccess.ClearRememberToken(this.UserID);
                RememberMeManager.DeleteToken();

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

        public static bool DeleteUser(int UserID)
        {
            return clsUsersDataAccess.DeleteUser(UserID);
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
            UserName = UserName.Trim().ToLower();

            // نجلب بيانات المستخدم باسم المستخدم فقط
            if (clsUsersDataAccess.GetUserInfoByUserName(UserName, ref UserID, ref PersonID, ref hashedPasswordFromDB, ref Permissions, ref isActive))
            {
                if (hashedPasswordFromDB == Password)
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

        public static bool RememberUser(int userID)
        {
            string plainToken = TokenHelper.GenerateToken();
            string tokenHash = HashHelper.ComputeSHA256(plainToken);
            DateTime expiry = DateTime.Now.AddDays(RememberMeDays);

            if (clsUsersDataAccess.UpdateRememberToken(userID, tokenHash, expiry))
            {
                RememberMeManager.SaveToken(plainToken);
                return true;
            }
            return false;
        }

        public static clsUsers TryLoginWithRememberMe()
        {
            if (!RememberMeManager.TryLoadToken(out string plainToken))
                return null;

            string tokenHash = HashHelper.ComputeSHA256(plainToken);
            int userID = clsUsersDataAccess.GetUserByRememberTokenHash(tokenHash);

            if (userID != -1)
            {
                return new clsUsers(userID);
            }
            else
            {
                RememberMeManager.DeleteToken();
                return null;
            }
        }

        public static void Logout(int userID)
        {
            clsUsersDataAccess.ClearRememberToken(userID);
            RememberMeManager.DeleteToken();
        }

        public bool CheckAccessPermission(enPermissions permission)
        {
            if (this.Permissions == (int)enPermissions.SuperUser)
                return true;

            if ((this.Permissions & (int)permission) == (int)permission)
                return true;

            return false;
        }
        public static bool GetUserInfoForPasswordReset(
    string UserName,
    ref int UserID,
    ref int PersonID,
    ref string Email)
        {
            return clsUsersDataAccess.GetUserInfoForPasswordReset(
                UserName,
                ref UserID,
                ref PersonID,
                ref Email);
        }

        public static string GeneratePasswordResetCode(int UserID)
        {
            // Generate 6-digit code
            string resetCode = TokenHelper.GenerateResetCode();

            // Hash the code before storing it in database
            string resetCodeHash = HashHelper.ComputeSHA256(resetCode);

            // Code expires after 15 minutes
            DateTime expiration = DateTime.Now.AddMinutes(15);

            // Save hash + expiration in database
            bool saved = clsUsersDataAccess.SaveResetCode(
                UserID,
                resetCodeHash,
                expiration);

            if (!saved)
                return null;

            // Return the original code so it can be sent by email
            return resetCode;
        }

        public static bool VerifyPasswordResetCode(int UserID, string resetCode)
        {
            string resetCodeHash = "";
            DateTime resetCodeExpiration = DateTime.MinValue;
            bool isResetCodeUsed = false;
            int resetCodeAttempts = 0;

            bool found = clsUsersDataAccess.GetResetCodeInfo(
                UserID,
                ref resetCodeHash,
                ref resetCodeExpiration,
                ref isResetCodeUsed,
                ref resetCodeAttempts);

            if (!found)
                return false;

            // Code already used or invalidated
            if (isResetCodeUsed)
                return false;

            // Code expired
            if (DateTime.Now > resetCodeExpiration)
                return false;

            // Maximum attempts reached
            if (resetCodeAttempts >= 5)
            {
                clsUsersDataAccess.MarkResetCodeAsUsed(UserID);
                return false;
            }

            // Hash the code entered by the user
            string enteredCodeHash =
                HashHelper.ComputeSHA256(resetCode);

            // Correct code
            if (enteredCodeHash == resetCodeHash)
            {
                return true;
            }

            // Wrong code
            clsUsersDataAccess.IncrementResetCodeAttempts(UserID);

            return false;
        }
        public static bool MarkResetCodeAsUsed(int UserID)
        {
            return clsUsersDataAccess.MarkResetCodeAsUsed(UserID);
        }
        public static bool IncrementResetCodeAttempts(int UserID)
        {
            return clsUsersDataAccess.IncrementResetCodeAttempts(UserID);
        }
    }
}