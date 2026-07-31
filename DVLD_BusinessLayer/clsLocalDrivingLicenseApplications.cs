using DVLD_DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLD_BusinessLayer
{
    public class clsLocalDrivingLicenseApplications
    {
        public enum enMode { AddNew = 0, Update = 1 }
        public enMode Mode { get; set; } = enMode.AddNew;

        public int LocalDrivingLicenseApplicationID { get; set; }
        public int ApplicationID { get; set; }
        public int ApplicantPersonID { get; set; } // خاصية مهمة للإعادة
        public int LicenseClassID { get; set; }

        public clsLocalDrivingLicenseApplications()
        {
            this.LocalDrivingLicenseApplicationID = -1;
            this.ApplicationID = -1;
            this.ApplicantPersonID = -1;
            this.LicenseClassID = -1;
            this.Mode = enMode.AddNew;
        }

        private clsLocalDrivingLicenseApplications(int localDrivingLicenseApplicationID, int applicationID, int applicantPersonID, int licenseClassID)
        {
            this.LocalDrivingLicenseApplicationID = localDrivingLicenseApplicationID;
            this.ApplicationID = applicationID;
            this.ApplicantPersonID = applicantPersonID;
            this.LicenseClassID = licenseClassID;
            this.Mode = enMode.Update;
        }

        // دالة الإضافة السابقة الخاصة بك
        public static int AddNewLocalDrivingLicenseApplications(int ApplicationID, int LicenseClassID)
        {
            return clsLocalDrivingLicenseApplicationsDataAccess.AddNewLocalDrivingLicenseApplications(ApplicationID, LicenseClassID);
        }

        // دالة البحث المطلوبة لجلب التفاصيل وبدء منطق الـ Retake
        public static clsLocalDrivingLicenseApplications FindByLocalDrivingAppID(int localDrivingLicenseApplicationID)
        {
            int applicationID = -1, applicantPersonID = -1, licenseClassID = -1;

            bool isFound = clsLocalDrivingLicenseApplicationsDataAccess.GetLocalDrivingLicenseApplicationInfoByID(
                localDrivingLicenseApplicationID,
                ref applicationID,
                ref applicantPersonID,
                ref licenseClassID
            );

            if (isFound)
            {
                return new clsLocalDrivingLicenseApplications(localDrivingLicenseApplicationID, applicationID, applicantPersonID, licenseClassID);
            }
            else
            {
                return null;
            }
        }
    }
}
