using DVLD_DataAccess;
using System;
using System.Data;

namespace DVLD_Business
{
    public class clsDetainedLicense
    {
        public enum enMode { AddNew = 0, Update = 1 }
        public enMode Mode = enMode.AddNew;

        public int DetainID { get; set; }
        public int LicenseID { get; set; }
        public DateTime DetainDate { get; set; }
        public decimal FineFees { get; set; }
        public int CreatedByUserID { get; set; }
        public bool IsReleased { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public int? ReleasedByUserID { get; set; }
        public int? ReleaseApplicationID { get; set; }

        // Constructor لإنشاء كائن جديد
        public clsDetainedLicense()
        {
            this.DetainID = -1;
            this.LicenseID = -1;
            this.DetainDate = DateTime.Now;
            this.FineFees = 0;
            this.CreatedByUserID = -1;
            this.IsReleased = false;
            this.ReleaseDate = null;
            this.ReleasedByUserID = null;
            this.ReleaseApplicationID = null;

            this.Mode = enMode.AddNew;
        }

        // Constructor خاص بالتحميل من قاعدة البيانات
        private clsDetainedLicense(int detainID, int licenseID, DateTime detainDate, decimal fineFees,
            int createdByUserID, bool isReleased, DateTime? releaseDate, int? releasedByUserID, int? releaseApplicationID)
        {
            this.DetainID = detainID;
            this.LicenseID = licenseID;
            this.DetainDate = detainDate;
            this.FineFees = fineFees;
            this.CreatedByUserID = createdByUserID;
            this.IsReleased = isReleased;
            this.ReleaseDate = releaseDate;
            this.ReleasedByUserID = releasedByUserID;
            this.ReleaseApplicationID = releaseApplicationID;

            this.Mode = enMode.Update;
        }

        // إضافة سجل جديد في قاعدة البيانات
        private bool _AddNewDetainedLicense()
        {
            this.DetainID = clsDetainedLicenseDataAccess.AddNewDetainedLicense(
                this.LicenseID, this.DetainDate, this.FineFees, this.CreatedByUserID);

            return (this.DetainID != -1);
        }

        // تحديث بيانات احتجاز (تستخدم بشكل رئيسي للـ Release)
        private bool _UpdateDetainedLicense()
        {
            return clsDetainedLicenseDataAccess.UpdateDetainedLicense(
                this.DetainID, this.LicenseID, this.DetainDate, this.FineFees,
                this.CreatedByUserID, this.IsReleased, this.ReleaseDate,
                this.ReleasedByUserID, this.ReleaseApplicationID);
        }

        // البحث عن حجز بواسطة DetainID
        public static clsDetainedLicense Find(int detainID)
        {
            int licenseID = -1;
            DateTime detainDate = DateTime.Now;
            decimal fineFees = 0;
            int createdByUserID = -1;
            bool isReleased = false;
            DateTime? releaseDate = null;
            int? releasedByUserID = null;
            int? releaseApplicationID = null;

            if (clsDetainedLicenseDataAccess.GetDetainedLicenseInfoByID(detainID, ref licenseID, ref detainDate,
                ref fineFees, ref createdByUserID, ref isReleased, ref releaseDate, ref releasedByUserID, ref releaseApplicationID))
            {
                return new clsDetainedLicense(detainID, licenseID, detainDate, fineFees,
                    createdByUserID, isReleased, releaseDate, releasedByUserID, releaseApplicationID);
            }
            else
            {
                return null;
            }
        }

        // البحث عن حجز غير مفروج عنه بواسطة LicenseID
        public static clsDetainedLicense FindByLicenseID(int licenseID)
        {
            int detainID = -1;
            DateTime detainDate = DateTime.Now;
            decimal fineFees = 0;
            int createdByUserID = -1;
            bool isReleased = false;
            DateTime? releaseDate = null;
            int? releasedByUserID = null;
            int? releaseApplicationID = null;

            if (clsDetainedLicenseDataAccess.GetDetainedLicenseInfoByLicenseID(licenseID, ref detainID, ref detainDate,
                ref fineFees, ref createdByUserID, ref isReleased, ref releaseDate, ref releasedByUserID, ref releaseApplicationID))
            {
                return new clsDetainedLicense(detainID, licenseID, detainDate, fineFees,
                    createdByUserID, isReleased, releaseDate, releasedByUserID, releaseApplicationID);
            }
            else
            {
                return null;
            }
        }

        // جلب جميع الرخص المحتجزة
        public static DataTable GetAllDetainedLicenses()
        {
            return clsDetainedLicenseDataAccess.GetAllDetainedLicenses();
        }

        // التأكد مما إذا كانت الرخصة محتجزة حالياً
        public static bool IsLicenseDetained(int licenseID)
        {
            return clsDetainedLicenseDataAccess.IsLicenseDetained(licenseID);
        }

        // فك احتجاز الرخصة مباشرة
        public bool ReleaseDetainedLicense(int releasedByUserID, int releaseApplicationID)
        {
            return clsDetainedLicenseDataAccess.ReleaseDetainedLicense(
                this.DetainID, releasedByUserID, releaseApplicationID);
        }

        // حفظ البيانات (سواء إضافة أو تحديث)
        public bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    if (_AddNewDetainedLicense())
                    {
                        Mode = enMode.Update;
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                case enMode.Update:
                    return _UpdateDetainedLicense();
            }

            return false;
        }
    }
}