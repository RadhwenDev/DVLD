using System;
using System.Data;
using DVLD_DataAccessLayer;

namespace DVLD_BusinessLayer
{
    public class clsTestAppointment
    {
        public enum enMode { AddNew = 0, Update = 1 }
        public enMode Mode { get; private set; } = enMode.AddNew;

        public enum enSaveResult
        {
            SavedSuccessfully,
            NoChanges,
            Failed,
            InvalidData
        }

        private clsTestAppointment _OriginalTestAppointment;

        // Properties (Entities matching DB structure)
        public int TestAppointmentID { get; set; }
        public int TestTypeID { get; set; }
        public int LocalDrivingLicenseApplicationID { get; set; }
        public DateTime AppointmentDate { get; set; }
        public decimal PaidFees { get; set; }
        public int CreatedByUserID { get; set; }
        public bool IsLocked { get; set; }
        public int RetakeTestApplicationID { get; set; }
        // الخصائص هذي للعرض فقط (Read-Only من الـ UI)
        public string ClassName { get; private set; } = "";
        public string FullName { get; private set; } = "";
        public int TestTrialCount { get; private set; } = 0;
        public int TestID { get; private set; } = -1;

        // Navigation Property
        public clsApplicant RetakeTestAppInfo { get; set; }

        // Default Constructor (AddNew Mode)
        // 1. Default Constructor (AddNew Mode)
        public clsTestAppointment()
        {
            this.TestAppointmentID = -1;
            this.TestTypeID = -1;
            this.LocalDrivingLicenseApplicationID = -1;
            this.AppointmentDate = DateTime.Now;
            this.PaidFees = 0;
            this.CreatedByUserID = -1;
            this.IsLocked = false;
            this.RetakeTestApplicationID = -1;
            this.RetakeTestAppInfo = null;

            // الخصائص المضافة للعرض في الـ UI
            this.ClassName = "";
            this.FullName = "";
            this.TestTrialCount = 0;
            this.TestID = -1;

            this.Mode = enMode.AddNew;
        }

        // 2. Private Constructor (Update Mode / Factory pattern via Find)
        private clsTestAppointment(
            int testAppointmentID,
            int testTypeID,
            int localDrivingLicenseApplicationID,
            DateTime appointmentDate,
            decimal paidFees,
            int createdByUserID,
            bool isLocked,
            int retakeTestApplicationID,
            string className,
            string fullName,
            int testTrialCount,
            int testID)
        {
            this.TestAppointmentID = testAppointmentID;
            this.TestTypeID = testTypeID;
            this.LocalDrivingLicenseApplicationID = localDrivingLicenseApplicationID;
            this.AppointmentDate = appointmentDate;
            this.PaidFees = paidFees;
            this.CreatedByUserID = createdByUserID;
            this.IsLocked = isLocked;
            this.RetakeTestApplicationID = retakeTestApplicationID;

            // إسناد الخصائص المضافة للـ UI
            this.ClassName = className;
            this.FullName = fullName;
            this.TestTrialCount = testTrialCount;
            this.TestID = testID;

            if (this.RetakeTestApplicationID != -1)
            {
                // this.RetakeTestAppInfo = clsApplicant.FindBaseApplication(this.RetakeTestApplicationID);
            }
            else
            {
                this.RetakeTestAppInfo = null;
            }

            // Save state snapshot using MemberwiseClone or CreateSnapshot
            RefreshOriginalValues();
            this.Mode = enMode.Update;
        }

        // 3. Snapshot Creation
        private clsTestAppointment CreateSnapshot()
        {
            return new clsTestAppointment
            {
                TestAppointmentID = TestAppointmentID,
                TestTypeID = TestTypeID,
                LocalDrivingLicenseApplicationID = LocalDrivingLicenseApplicationID,
                AppointmentDate = AppointmentDate,
                PaidFees = PaidFees,
                CreatedByUserID = CreatedByUserID,
                IsLocked = IsLocked,
                RetakeTestApplicationID = RetakeTestApplicationID,

                // نسخ خصائص العرض في الـ Snapshot
                ClassName = ClassName,
                FullName = FullName,
                TestTrialCount = TestTrialCount,
                TestID = TestID
            };
        }

        private void RefreshOriginalValues()
        {
            _OriginalTestAppointment = CreateSnapshot();
        }

        #region Business Rules & Validation

        private bool IsValid()
        {
            if (AppointmentDate < DateTime.Now.AddHours(1))
                return false;

            return true;
        }

        #endregion

        private bool IsAppointmentDateChanged()
        {
            if (_OriginalTestAppointment == null)
                return true;

            return AppointmentDate != _OriginalTestAppointment.AppointmentDate;
        }


        #region CRUD Methods

        public static clsTestAppointment Find(int testAppointmentID)
        {
            // المتغيرات الأساسية لجدول TestAppointments
            int testTypeID = -1, localDrivingLicenseApplicationID = -1, createdByUserID = -1, retakeTestApplicationID = -1;
            DateTime appointmentDate = DateTime.Now;
            decimal paidFees = 0;
            bool isLocked = false;

            // المتغيرات المضافة للعرض فقط (Read-Only UI Properties)
            string className = "";
            string fullName = "";
            int testTrialCount = 0;
            int testID = -1;

            bool isFound = clsTestAppointmentDataAccess.GetTestAppointmentInfoByID(
                testAppointmentID,
                ref testTypeID,
                ref localDrivingLicenseApplicationID,
                ref appointmentDate,
                ref paidFees,
                ref createdByUserID,
                ref isLocked,
                ref retakeTestApplicationID,
                ref className,
                ref fullName,
                ref testTrialCount,
                ref testID
            );

            if (isFound)
            {
                return new clsTestAppointment(
                    testAppointmentID,
                    testTypeID,
                    localDrivingLicenseApplicationID,
                    appointmentDate,
                    paidFees,
                    createdByUserID,
                    isLocked,
                    retakeTestApplicationID,
                    className,
                    fullName,
                    testTrialCount,
                    testID
                );
            }

            return null;
        }
        public static clsTestAppointment FindTakeTest(int testAppointmentID)
        {
            int testTypeID = -1;
            int localDrivingLicenseApplicationID = -1;
            int createdByUserID = -1;
            int retakeTestApplicationID = -1;
            DateTime appointmentDate = DateTime.Now;
            decimal paidFees = 0;
            bool isLocked = false;

            // المتغيرات الجديدة التي يجلبها الاستعلام المحدث
            string className = "";
            string fullName = "";
            int testTrialCount = 0;
            int testID = -1;

            bool isFound = clsTestAppointmentDataAccess.GetTestAppointmentInfoByID(
                testAppointmentID,
                ref testTypeID,
                ref localDrivingLicenseApplicationID,
                ref appointmentDate,
                ref paidFees,
                ref createdByUserID,
                ref isLocked,
                ref retakeTestApplicationID,
                ref className,
                ref fullName,
                ref testTrialCount,
                ref testID
            );

            if (isFound)
            {
                return new clsTestAppointment(
                    testAppointmentID,
                    testTypeID,
                    localDrivingLicenseApplicationID,
                    appointmentDate,
                    paidFees,
                    createdByUserID,
                    isLocked,
                    retakeTestApplicationID,
                    className,
                    fullName,
                    testTrialCount,
                    testID
                );
            }

            return null;
        }

        private bool _AddNewTestAppointment()
        {
            this.TestAppointmentID = clsTestAppointmentDataAccess.AddNewTestAppointment(
                this.TestTypeID, this.LocalDrivingLicenseApplicationID,
                this.AppointmentDate, this.PaidFees, this.CreatedByUserID,
                this.IsLocked, this.RetakeTestApplicationID);

            return (this.TestAppointmentID != -1);
        }

        private bool _UpdateTestAppointment()
        {
            return clsTestAppointmentDataAccess.UpdateTestAppointment(
                this.TestAppointmentID, this.TestTypeID, this.LocalDrivingLicenseApplicationID,
                this.AppointmentDate, this.PaidFees, this.CreatedByUserID,
                this.IsLocked, this.RetakeTestApplicationID);
        }

        public enSaveResult Save()
        {
            if (!IsValid())
                return enSaveResult.InvalidData;

            switch (Mode)
            {
                case enMode.AddNew:
                    if (_AddNewTestAppointment())
                    {
                        Mode = enMode.Update;
                        RefreshOriginalValues();
                        return enSaveResult.SavedSuccessfully;
                    }
                    else
                    {
                        return enSaveResult.Failed;
                    }

                case enMode.Update:
                    if (!IsAppointmentDateChanged())
                        return enSaveResult.NoChanges;

                    if (_UpdateTestAppointment())
                    {
                        RefreshOriginalValues();
                        return enSaveResult.SavedSuccessfully;
                    }
                    else
                    {
                        return enSaveResult.Failed;
                    }
            }

            return enSaveResult.Failed;
        }

        #endregion

        #region Data Queries / DataGrids (Read-Only Service Operations)

        public static DataTable GetApplicationAppointments(int localDrivingLicenseApplicationID, int testTypeID)
        {
            return clsTestAppointmentDataAccess.GetApplicationAppointments(localDrivingLicenseApplicationID, testTypeID);
        }
        public static DataTable getTsetAppointment(int AppID)
        {
            return clsTestAppointmentDataAccess.getTsetAppointment(AppID);
        }
        public static DataTable getDataAppintment(int AppID, int testID)
        {
            return clsTestAppointmentDataAccess.getDataAppintment(AppID, testID);
        }

        public static DataTable GetApplicationAppointmentsList(int localDrivingLicenseApplicationID)
        {
            return clsTestAppointmentDataAccess.GetApplicationAppointmentsList(localDrivingLicenseApplicationID);
        }

        #endregion

        #region Data Queries / DataGrids (Read-Only Service Operations)

        // جلب تفاصيل الطلب الرئيسي
        public static DataTable GetAppointmentDetails(int localDrivingLicenseApplicationID)
        {
            // يستدعي الدالة المناسبة من طبقة البيانات (DataAccessLayer)
            return clsTestAppointmentDataAccess.GetApplicationAppointmentsList(localDrivingLicenseApplicationID);
            // أو قم بربطها بالدالة المخصصة لجلب تفاصيل الرخصة محلياً لديك
        }

        // جلب قائمة المواعيد المخصصة لجدول العرض
        public static DataTable visionTestDataGridView(int localDrivingLicenseApplicationID, int testTypeID)
        {
            return clsTestAppointmentDataAccess.GetApplicationAppointments(localDrivingLicenseApplicationID, testTypeID);
        }

        public static DataTable visionTestDataGridView(int localDrivingLicenseApplicationID)
        {
            return clsTestAppointmentDataAccess.GetApplicationAppointmentsList(localDrivingLicenseApplicationID);
        }

        #endregion
    }
}