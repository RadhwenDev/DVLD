using System;
using System.Data;
using DVLD_DataAccessLayer;

namespace DVLD_BusinessLayer
{
    public class clsLicenses
    {
        public enum enMode { AddNew = 0, Update = 1 }
        public enMode Mode = enMode.AddNew;

        public enum enIssueReason { FirstTime = 1, Renew = 2, ReplacementForDamaged = 3, ReplacementForLost = 4 }

        // Properties
        public int LicenseID { get; set; }
        public int ApplicationID { get; set; }
        public int DriverID { get; set; }
        public int LicenseClass { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string Notes { get; set; }
        public decimal PaidFees { get; set; }
        public bool IsActive { get; set; }
        public enIssueReason IssueReason { get; set; }
        public int CreatedByUserID { get; set; }

        public clsLicenses()
        {
            this.LicenseID = -1;
            this.ApplicationID = -1;
            this.DriverID = -1;
            this.LicenseClass = -1;
            this.IssueDate = DateTime.Now;
            this.ExpirationDate = DateTime.Now;
            this.Notes = "";
            this.PaidFees = 0;
            this.IsActive = true;
            this.IssueReason = enIssueReason.FirstTime;
            this.CreatedByUserID = -1;

            Mode = enMode.AddNew;
        }

        private clsLicenses(int licenseID, int applicationID, int driverID, int licenseClass,
                    DateTime issueDate, DateTime expirationDate, string notes,
                    decimal paidFees, bool isActive, enIssueReason issueReason, int createdByUserID)
        {
            this.LicenseID = licenseID;
            this.ApplicationID = applicationID;
            this.DriverID = driverID;
            this.LicenseClass = licenseClass;
            this.IssueDate = issueDate;
            this.ExpirationDate = expirationDate;
            this.Notes = notes;
            this.PaidFees = paidFees;
            this.IsActive = isActive;
            this.IssueReason = issueReason;
            this.CreatedByUserID = createdByUserID;

            this.Mode = enMode.Update;
        }
        public static clsLicenses Find(int licenseID)
        {
            int applicationID = -1;
            int driverID = -1;
            int licenseClass = -1;
            DateTime issueDate = DateTime.Now;
            DateTime expirationDate = DateTime.Now;
            string notes = "";
            decimal paidFees = 0;
            bool isActive = false;
            byte issueReason = 1;
            int createdByUserID = -1;

            if (clsLicensesDataAccess.GetLicenseInfoByID(
                    licenseID, ref applicationID, ref driverID, ref licenseClass,
                    ref issueDate, ref expirationDate, ref notes, ref paidFees,
                    ref isActive, ref issueReason, ref createdByUserID))
            {
                return new clsLicenses(
                    licenseID, applicationID, driverID, licenseClass,
                    issueDate, expirationDate, notes, paidFees,
                    isActive, (enIssueReason)issueReason, createdByUserID
                );
            }
            else
            {
                return null;
            }
        }
        public static clsLicenses FindLastLicenseByPersonIDAndClass(int personID, int licenseClassID)
        {
            int licenseID = -1;
            int applicationID = -1;
            int driverID = -1;
            int licenseClass = -1;
            DateTime issueDate = DateTime.Now;
            DateTime expirationDate = DateTime.Now;
            string notes = "";
            decimal paidFees = 0;
            bool isActive = false;
            byte issueReason = 1;
            int createdByUserID = -1;

            if (clsLicensesDataAccess.GetLastLicenseByPersonIDAndClass(
                    personID, licenseClassID, ref licenseID, ref applicationID, ref driverID, ref licenseClass,
                    ref issueDate, ref expirationDate, ref notes, ref paidFees,
                    ref isActive, ref issueReason, ref createdByUserID))
            {
                return new clsLicenses(
                    licenseID, applicationID, driverID, licenseClass,
                    issueDate, expirationDate, notes, paidFees,
                    isActive, (enIssueReason)issueReason, createdByUserID
                );
            }
            else
            {
                return null;
            }
        }

        private bool _AddNewLicense()
        {
            this.LicenseID = clsLicensesDataAccess.AddNewLicense(
                this.ApplicationID,
                this.DriverID,
                this.LicenseClass,
                this.IssueDate,
                this.ExpirationDate,
                this.Notes,
                this.PaidFees,
                this.IsActive,
                (byte)this.IssueReason,
                this.CreatedByUserID
            );

            return (this.LicenseID != -1);
        }

        public bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    if (_AddNewLicense())
                    {
                        Mode = enMode.Update;
                        return true;
                    }
                    return false;

                case enMode.Update:
                    // التحديث يمكن إضافته عند الحاجة
                    return false;
            }

            return false;
        }
        public static DataTable getAllLicenses()
        {
            return clsLicensesDataAccess.getAllLicenses();
        }

        public static bool Deactivate(int licenseID)
        {
            return clsLicensesDataAccess.DeactivateLicense(licenseID);
        }

        public static int getTotalActiveLicenses()
        {
            return clsLicensesDataAccess.getTotalActiveLicenses();
        }
        public static int GetLicenseIDByApplicationID(int DriverID)
        {
            return clsLicensesDataAccess.GetLicenseIDByApplicationID(DriverID);
        }

        public static bool hasLicense(int personID)
        {
            return clsLicensesDataAccess.hasLicense(personID);
        }

        public static bool hasInternationalLicense(int personID)
        {
            return clsLicensesDataAccess.HasInternationalLicenseOrNonClass3(personID);
        }
        public static bool hasDetainedLicense(int personID)
        {
            return clsLicensesDataAccess.hasDetainedLicense(personID);
        }
        public static bool canRenewLocalLicense(int personID)
        {
            return clsLicensesDataAccess.canRenewLocalLicense(personID);
        }

        public static DataTable getShowLicense(int ApplicationID)
        {
            return clsLicensesDataAccess.getShowLicense(ApplicationID);
        }
        public static DataTable getShowLicenseRelease(int ApplicationID)
        {
            return clsLicensesDataAccess.getShowLicenseRelease(ApplicationID);
        }
        public static DataTable getShowInternationalLicense(int ApplicationID)
        {
            return clsLicensesDataAccess.getShowInternationalLicense(ApplicationID);
        }
        public static DataTable getShowRenewLicense(int ApplicationID)
        {
            return clsLicensesDataAccess.getShowRenewLicense(ApplicationID);
        }
        public static DataTable getShowReplacementLicense(int ApplicationID)
        {
            return clsLicensesDataAccess.getShowReplacementLicense(ApplicationID);
        }
    }
}