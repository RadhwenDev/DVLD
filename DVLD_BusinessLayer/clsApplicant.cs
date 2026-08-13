using DVLD_DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLD_BusinessLayer
{
    public class clsApplicant
    {
        public enum enMode { AddNew = 0, Update = 1 }
        public enMode Mode { get; set; } = enMode.AddNew;

        public enum enApplicationStatus { New = 1, Cancelled = 2, Completed = 3 }
        public enum enApplicationType
        {
            NewDrivingLicense = 1,
            RenewDrivingLicense = 2,
            ReplaceLostDrivingLicense = 3,
            ReplaceDamagedDrivingLicense = 4,
            ReleaseDetainedDrivingLicsense = 5,
            NewInternationalLicense = 6,
            RetakeTest = 7
        }

        public int ApplicationID { get; set; }
        public int ApplicantPersonID { get; set; }
        public DateTime ApplicationDate { get; set; }
        public int ApplicationTypeID { get; set; }
        public enApplicationStatus ApplicationStatus { get; set; }
        public DateTime LastStatusDate { get; set; }
        public decimal PaidFees { get; set; }
        public int CreatedByUserID { get; set; }

        // Constructors
        public clsApplicant()
        {
            this.ApplicationID = -1;
            this.ApplicantPersonID = -1;
            this.ApplicationDate = DateTime.Now;
            this.ApplicationTypeID = -1;
            this.ApplicationStatus = enApplicationStatus.New;
            this.LastStatusDate = DateTime.Now;
            this.PaidFees = 0;
            this.CreatedByUserID = -1;

            this.Mode = enMode.AddNew;
        }

        private clsApplicant(int applicationID, int applicantPersonID, DateTime applicationDate,
            int applicationTypeID, enApplicationStatus applicationStatus, DateTime lastStatusDate,
            decimal paidFees, int createdByUserID)
        {
            this.ApplicationID = applicationID;
            this.ApplicantPersonID = applicantPersonID;
            this.ApplicationDate = applicationDate;
            this.ApplicationTypeID = applicationTypeID;
            this.ApplicationStatus = applicationStatus;
            this.LastStatusDate = lastStatusDate;
            this.PaidFees = paidFees;
            this.CreatedByUserID = createdByUserID;

            this.Mode = enMode.Update;
        }

        #region Private Methods
        private bool _AddNewApplication()
        {
            this.ApplicationID = clsApplicationsDataAccess.AddNewApplication(
                this.ApplicantPersonID,
                this.ApplicationDate,
                this.ApplicationTypeID,
                (byte)this.ApplicationStatus,
                this.LastStatusDate,
                this.PaidFees,
                this.CreatedByUserID
            );

            return (this.ApplicationID != -1);
        }

        private bool _UpdateApplication()
        {
            // يمكنك استدعاء دالة UpdateApplication من الـ DataAccess إذا كانت موجودة لديك
            return false;
        }

        public static bool UpdateStatus(int applicationID, short newStatus)
        {
            return clsApplicationsDataAccess.UpdateStatus(applicationID, newStatus);
        }

        #endregion

        #region Public Methods & Queries

        public static bool UpdateToCaancelStatus(int ApplicationID)
        {
            return clsApplicationsDataAccess.UpdateToCaancelStatus(ApplicationID);
        }

        public bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    if (_AddNewApplication())
                    {
                        Mode = enMode.Update;
                        return true;
                    }
                    return false;

                case enMode.Update:
                    return _UpdateApplication();
            }

            return false;
        }

        #endregion

        #region Static Helper Data Methods (لخدمة الجداول والـ UI)

        public static DataTable getAllApplicants()
        {
            return clsApplicationsDataAccess.getAllApplicants();
        }

        public static DataTable getAllApplicationTypes()
        {
            return clsApplicationsDataAccess.getAllApplicationType();
        }
        public static DataTable getAllApplicationTypes(bool hasLicense)
        {
            return clsApplicationsDataAccess.getAllApplicationTypes(hasLicense);
        }


        public static DataTable getAllDetailsForShowButton(int ApplicationID)
        {
            return clsApplicationsDataAccess.getAllDetailsForShowButton(ApplicationID);
        }
        public static DataTable getApplicationTypesTitle_Fees(int ApplicationTypeID)
        {
            return clsApplicationsDataAccess.getApplicationTypesTitle_Fees(ApplicationTypeID);
        }
        public static int AddNewApplication(int ApplicantPersonID, DateTime ApplicationDate, int ApplicationTypeID, byte ApplicationStatus, DateTime LastStatusDate, decimal PaidFees, int CreatedByUserID)
        {
            return clsApplicationsDataAccess.AddNewApplication(ApplicantPersonID, ApplicationDate, ApplicationTypeID, ApplicationStatus, LastStatusDate, PaidFees, CreatedByUserID);
        }

        public static DataTable getAllApplicationStatus()
        {
            return clsApplicationsDataAccess.getAllApplicationStatus();
        }

        public static bool IsReleaseApplication(int ApplicationID)
        {
            return clsApplicationsDataAccess.IsReleaseApplication(ApplicationID);
        }
        public static bool IsInternationalApplication(int ApplicationID)
        {
            return clsApplicationsDataAccess.IsInternationalApplication(ApplicationID);
        }
        public static bool IsRenewApplication(int ApplicationID)
        {
            return clsApplicationsDataAccess.IsRenewApplication(ApplicationID);
        }
        public static bool IsReplacementApplication(int ApplicationID)
        {
            return clsApplicationsDataAccess.IsReplacementApplication(ApplicationID);
        }
        public static bool DeleteApplication(int ApplicationID)
        {
            return clsApplicationsDataAccess.DeleteApplication(ApplicationID);
        }
        #endregion
    }
}
