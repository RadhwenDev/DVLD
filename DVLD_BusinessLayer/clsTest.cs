using DVLD_DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLD_BusinessLayer
{
    public class clsTest
    {
        public enum enMode { AddNew = 0, Update = 1 }
        public enMode Mode = enMode.AddNew;

        public int TestID { get; set; }
        public int TestAppointmentID { get; set; }
        public bool TestResult { get; set; }
        public string Notes { get; set; }
        public int CreatedByUserID { get; set; }
        public int TestTypeID { get; set; }

        public clsTest()
        {
            this.TestID = -1;
            this.TestAppointmentID = -1;
            this.TestResult = false;
            this.Notes = "";
            this.CreatedByUserID = -1;
            this.TestTypeID = -1;

            Mode = enMode.AddNew;
        }

        private clsTest(int testID, int testAppointmentID, bool testResult, string notes, int createdByUserID)
        {
            this.TestID = testID;
            this.TestAppointmentID = testAppointmentID;
            this.TestResult = testResult;
            this.Notes = notes;
            this.CreatedByUserID = createdByUserID;

            Mode = enMode.Update;
        }


        private bool _AddNewTest()
        {
            this.TestID = clsTestData.AddNewTest(this.TestAppointmentID, this.TestResult, this.Notes, this.CreatedByUserID, this.TestTypeID);
            if (this.TestID != -1)
            {
                if (this.TestTypeID == 3 && this.TestResult)
                {
                    _HandlePassingFinalStreetTest();
                }
                return true;
            }

            return false;
        }

        public static clsTest Find(int testID)
        {
            int testAppointmentID = -1;
            bool testResult = false;
            string notes = "";
            int createdByUserID = -1;

            if (clsTestData.Find(testID, ref testAppointmentID, ref testResult, ref notes, ref createdByUserID))
            {
                return new clsTest(testID, testAppointmentID, testResult, notes, createdByUserID);
            }
            else
            {
                return null;
            }
        }

        private void _HandlePassingFinalStreetTest()
        {
            // 1. جلب تفاصيل موعد الاختبار للحصول على LocalDrivingLicenseApplicationID
            clsTestAppointment appointment = clsTestAppointment.Find(this.TestAppointmentID);
            if (appointment == null) return;

            clsLocalDrivingLicenseApplications localApp = clsLocalDrivingLicenseApplications.FindByLocalDrivingAppID(appointment.LocalDrivingLicenseApplicationID);
            if (localApp == null) return;

            // 2. إضافة Driver جديد إن لم يكن موجوداً من قبل
            int driverID = -1;
            clsDriver driver = clsDriver.FindByPersonID(localApp.ApplicantPersonID);

            if (driver == null)
            {
                driver = new clsDriver();
                driver.PersonID = localApp.ApplicantPersonID;
                driver.CreatedByUserID = this.CreatedByUserID;
                driver.CreatedDate = DateTime.Now;

                if (driver.Save())
                {
                    driverID = driver.DriverID;
                }
            }
            else
            {
                driverID = driver.DriverID;
            }

            // 3. إنشاء وإضافة الرخصة الجديدة (License)
            if (driverID != -1)
            {
                clsLicenses license = new clsLicenses();
                license.ApplicationID = localApp.ApplicationID;
                license.DriverID = driverID;
                license.LicenseClass = localApp.LicenseClassID;
                license.IssueDate = DateTime.Now;
                clsLicenseClass licenseClass = clsLicenseClass.Find(localApp.LicenseClassID);
                if (licenseClass != null)
                {
                    license.ExpirationDate = DateTime.Now.AddYears(licenseClass.Validity);
                    license.PaidFees = licenseClass.Fees;
                }
                license.Notes = this.Notes;
                license.IsActive = true;
                license.IssueReason = clsLicenses.enIssueReason.FirstTime; // 1 - FirstTime
                license.CreatedByUserID = this.CreatedByUserID;

                if (license.Save())
                {
                    // 4. تحديث حالة الطلب إلى Completed (3)
                    localApp.SetComplete();
                }
            }
        }

        public bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    if (_AddNewTest())
                    {
                        Mode = enMode.Update;
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                case enMode.Update:
                    return false;
            }

            return false;
        }
    }
}
