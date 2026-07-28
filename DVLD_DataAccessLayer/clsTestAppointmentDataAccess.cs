using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLD_DataAccessLayer
{
    public class clsTestAppointmentDataAccess
    {
        public static string ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        public static DataTable visionTest(int ApplicationID)
        {
            DataTable dt = new DataTable();
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string query = @"SELECT 
                                    (P.FirstName + ' ' + P.SecondName + ' ' + P.ThirdName + ' ' + P.LastName) AS FullName, 
                                    P.PersonID, 
                                    P.ImagePath,
                                    CASE
                                        WHEN A.ApplicationStatus = 1 THEN 'New'
                                        WHEN A.ApplicationStatus = 2 THEN 'Cancelled'
                                        ELSE 'Completed'
                                    END AS [STATUS],
                                    A.LastStatusDate, 
                                    A.ApplicationDate, 
                                    A.ApplicationID,
                                    U.Username AS CreatedByUserName, 
                                    A.PaidFees AS ApplicationPaidFees,
                                    LDLA.LocalDrivingLicenseApplicationID,
                                    LC.ClassName,
                                    TA.TestTypeID
                                from LocalDrivingLicenseApplications LDLA
                                INNER JOIN Applications A ON LDLA.ApplicationID = A.ApplicationID
                                INNER JOIN LicenseClasses LC ON LDLA.LicenseClassID = LC.LicenseClassID
                                INNER JOIN TestAppointments TA ON LDLA.LocalDrivingLicenseApplicationID = TA.LocalDrivingLicenseApplicationID
                                INNER JOIN People P ON A.ApplicantPersonID = P.PersonID
                                INNER JOIN Users U ON A.CreatedByUserID = U.UserID
                                WHERE A.ApplicationID = @ApplicationID;";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ApplicationID", ApplicationID);
                    try
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                                dt.Load(reader);
                        }
                    }
                    catch (Exception) { }
                }
            }
            return dt;
        }
        public static DataTable visionTestDataGridView(int ApplicationID)
        {
            DataTable dt = new DataTable();

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                try
                {
                    connection.Open();

                    // 1. تسكير المواعيد اللي فات تاريخها أوتوماتيكياً قبل قراءة البيانات
                    using (SqlCommand spCommand = new SqlCommand("LockExpiredTests", connection))
                    {
                        spCommand.CommandType = CommandType.StoredProcedure;
                        spCommand.ExecuteNonQuery();
                    }

                    // 2. تجيب البيانات المحينة للـ DataGridView
                    string query = @"SELECT 
                                TA.TestAppointmentID, 
                                TA.AppointmentDate, 
                                TA.PaidFees AS AppointmentPaidFees,
                                CASE
                                    WHEN TA.IsLocked = 1 THEN 'Locked'
                                    ELSE 'Open'
                                END AS isLocked
                            FROM Applications A
                            INNER JOIN LocalDrivingLicenseApplications LDLA ON A.ApplicationID = LDLA.ApplicationID
                            INNER JOIN LicenseClasses LC ON LDLA.LicenseClassID = LC.LicenseClassID
                            LEFT JOIN TestAppointments TA ON LDLA.LocalDrivingLicenseApplicationID = TA.LocalDrivingLicenseApplicationID
                            WHERE A.ApplicationID = @ApplicationID;";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ApplicationID", ApplicationID);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                                dt.Load(reader);
                        }
                    }
                }
                catch (Exception){}
            }

            return dt;
        }
        public static DataTable getDataAppintment(int ApplicationID, int TestTypeID)
        {
            DataTable dt = new DataTable();
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string query = @"SELECT 
                                    -- 1. Local Application & Person Info
                                    LDLApp.LocalDrivingLicenseApplicationID AS [D.L.App.ID],
                                    LicenseClasses.ClassName AS [D.Class],
                                    People.FirstName + ' ' + People.SecondName + ' ' + ISNULL(People.ThirdName, '') + ' ' + People.LastName AS [Name],

                                    (
                                        SELECT COUNT(Tests.TestID)
                                        FROM Tests 
                                        INNER JOIN TestAppointments ON Tests.TestAppointmentID = TestAppointments.TestAppointmentID
                                        WHERE TestAppointments.LocalDrivingLicenseApplicationID = LDLApp.LocalDrivingLicenseApplicationID
                                          AND TestAppointments.TestTypeID = @TestTypeID
                                    ) AS [Trial],

                                    -- 3. Appointment Info
                                    TestAppointments.AppointmentDate AS [Date],
                                    TestTypes.TestTypeFees AS [Fees],

                                    -- 4. Retake Test Info
                                    ISNULL(Applications.PaidFees, 0) AS [R.App.Fees],
                                    (TestTypes.TestTypeFees + ISNULL(Applications.PaidFees, 0)) AS [Total Fees]

                                FROM LocalDrivingLicenseApplications LDLApp
                                INNER JOIN Applications ON LDLApp.ApplicationID = Applications.ApplicationID
                                INNER JOIN People ON Applications.ApplicantPersonID = People.PersonID
                                INNER JOIN LicenseClasses ON LDLApp.LicenseClassID = LicenseClasses.LicenseClassID
                                CROSS JOIN TestTypes
                                LEFT JOIN TestAppointments ON TestAppointments.LocalDrivingLicenseApplicationID = LDLApp.LocalDrivingLicenseApplicationID
                                                          AND TestAppointments.TestTypeID = TestTypes.TestTypeID

                                WHERE Applications.ApplicationID = @ApplicationID
                                  AND TestTypes.TestTypeID = @TestTypeID;";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ApplicationID", ApplicationID);
                    command.Parameters.AddWithValue("@TestTypeID", TestTypeID);
                    try
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                                dt.Load(reader);
                        }
                    }
                    catch (Exception) { }
                }
            }
            return dt;
        }

        public static DataTable getTsetAppointment(int ApplicationID)
        {
            DataTable dt = new DataTable();
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string query = @"SELECT 
                                    TA.TestTypeID
                                FROM Applications A
                                INNER JOIN LocalDrivingLicenseApplications LDLA ON A.ApplicationID = LDLA.ApplicationID
                                INNER JOIN LicenseClasses LC ON LDLA.LicenseClassID = LC.LicenseClassID
                                LEFT JOIN TestAppointments TA ON LDLA.LocalDrivingLicenseApplicationID = TA.LocalDrivingLicenseApplicationID
                                WHERE A.ApplicationID = @ApplicationID;";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ApplicationID", ApplicationID);
                    try
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                                dt.Load(reader);
                        }
                    }
                    catch (Exception) { }
                }
            }
            return dt;
        }


    }
}
