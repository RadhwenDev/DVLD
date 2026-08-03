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



        public static int getTotalActiveLicenses()

        {

            return clsLicensesDataAccess.getTotalActiveLicenses();

        }



        public static DataTable getShowLicense(int ApplicationID)

        {

            return clsLicensesDataAccess.getShowLicense(ApplicationID);

        }
    }
}