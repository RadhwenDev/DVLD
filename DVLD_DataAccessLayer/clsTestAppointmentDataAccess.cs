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
                catch (Exception) { }
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
        public static bool GetTestAppointmentInfoByID(
    int TestAppointmentID,
    ref int TestTypeID,
    ref int LocalDrivingLicenseApplicationID,
    ref DateTime AppointmentDate,
    ref decimal PaidFees,
    ref int CreatedByUserID,
    ref bool IsLocked,
    ref int RetakeTestApplicationID,
    ref string ClassName,
    ref string FullName,
    ref int TestTrialCount,
    ref int TestID)
        {
            bool isFound = false;

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string query = @"SELECT 
                                 TA.TestTypeID,
                                 TA.LocalDrivingLicenseApplicationID,
                                 TA.AppointmentDate,
                                 TA.PaidFees,
                                 TA.CreatedByUserID,
                                 TA.IsLocked,
                                 TA.RetakeTestApplicationID,
                                 LC.ClassName,
                                 (P.FirstName + ' ' + P.SecondName + ' ' + ISNULL(P.ThirdName, '') + ' ' + P.LastName) AS FullName,
                                 (
                                     SELECT COUNT(*) 
                                     FROM TestAppointments TA2
                                     INNER JOIN Tests T ON TA2.TestAppointmentID = T.TestAppointmentID
                                     WHERE TA2.LocalDrivingLicenseApplicationID = TA.LocalDrivingLicenseApplicationID
                                       AND TA2.TestTypeID = TA.TestTypeID
                                       AND TA2.TestAppointmentID < TA.TestAppointmentID
                                 ) AS TestTrialCount,
                                 ISNULL(T.TestID, -1) AS TestID
                             FROM TestAppointments TA
                             INNER JOIN LocalDrivingLicenseApplications LDL ON TA.LocalDrivingLicenseApplicationID = LDL.LocalDrivingLicenseApplicationID
                             INNER JOIN Applications App ON LDL.ApplicationID = App.ApplicationID
                             INNER JOIN People P ON App.ApplicantPersonID = P.PersonID
                             INNER JOIN LicenseClasses LC ON LDL.LicenseClassID = LC.LicenseClassID
                             LEFT JOIN Tests T ON TA.TestAppointmentID = T.TestAppointmentID
                             WHERE TA.TestAppointmentID = @TestAppointmentID;";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TestAppointmentID", TestAppointmentID);

                    try
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                isFound = true;

                                TestTypeID = (int)reader["TestTypeID"];
                                LocalDrivingLicenseApplicationID = (int)reader["LocalDrivingLicenseApplicationID"];
                                AppointmentDate = (DateTime)reader["AppointmentDate"];
                                PaidFees = (decimal)reader["PaidFees"];
                                CreatedByUserID = (int)reader["CreatedByUserID"];
                                IsLocked = (bool)reader["IsLocked"];
                                RetakeTestApplicationID = reader["RetakeTestApplicationID"] != DBNull.Value ? (int)reader["RetakeTestApplicationID"] : -1;

                                ClassName = reader["ClassName"].ToString();
                                FullName = reader["FullName"].ToString();
                                TestTrialCount = (int)reader["TestTrialCount"];
                                TestID = (int)reader["TestID"];
                            }
                        }
                    }
                    catch
                    {
                        isFound = false;
                    }
                }
            }

            return isFound;
        }

        public static byte GetPassedTestCount(int ApplicationID)
        {
            byte passedTestCount = 0;

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string query = @"SELECT COUNT(*) 
                         FROM Applications A
                         INNER JOIN LocalDrivingLicenseApplications LDLA
                             ON A.ApplicationID = LDLA.ApplicationID
                         INNER JOIN TestAppointments TA
                             ON LDLA.LocalDrivingLicenseApplicationID = TA.LocalDrivingLicenseApplicationID
                         INNER JOIN Tests T
                             ON TA.TestAppointmentID = T.TestAppointmentID
                         WHERE A.ApplicationID = @ApplicationID
                         AND T.TestResult = 1;";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ApplicationID", ApplicationID);

                    try
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();

                        if (result != null && int.TryParse(result.ToString(), out int count))
                        {
                            passedTestCount = (byte)count;
                        }
                    }
                    catch (Exception )
                    {
                        // يمكنك تسجيل الخطأ هنا عند الحاجة
                        passedTestCount = 0;
                    }
                }
            }

            return passedTestCount;
        }

        public static bool GetTestAppointmentInfoByID(
            int testAppointmentID,
            ref int testTypeID,
            ref int localDrivingLicenseApplicationID,
            ref DateTime appointmentDate,
            ref decimal paidFees,
            ref int createdByUserID,
            ref bool isLocked,
            ref int retakeTestApplicationID)
        {
            bool isFound = false;

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string query = @"SELECT * FROM TestAppointments 
                                 WHERE TestAppointmentID = @TestAppointmentID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TestAppointmentID", testAppointmentID);

                    try
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                isFound = true;

                                testTypeID = (int)reader["TestTypeID"];
                                localDrivingLicenseApplicationID = (int)reader["LocalDrivingLicenseApplicationID"];
                                appointmentDate = (DateTime)reader["AppointmentDate"];
                                paidFees = (decimal)reader["PaidFees"];
                                createdByUserID = (int)reader["CreatedByUserID"];
                                isLocked = (bool)reader["IsLocked"];

                                // RetakeTestApplicationID يمكن أن تكون NULL في قاعدة البيانات
                                if (reader["RetakeTestApplicationID"] == DBNull.Value)
                                {
                                    retakeTestApplicationID = -1;
                                }
                                else
                                {
                                    retakeTestApplicationID = (int)reader["RetakeTestApplicationID"];
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {
                        // يمكن تسجيل الخطأ (Logging) هنا حسب الحاجة
                        isFound = false;
                    }
                }
            }

            return isFound;
        }
        public static int AddNewTestAppointment(
            int testTypeID,
            int localDrivingLicenseApplicationID,
            DateTime appointmentDate,
            decimal paidFees,
            int createdByUserID,
            bool isLocked,
            int retakeTestApplicationID)
        {
            int testAppointmentID = -1;

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string query = @"INSERT INTO TestAppointments 
                                (
                                    TestTypeID, 
                                    LocalDrivingLicenseApplicationID, 
                                    AppointmentDate, 
                                    PaidFees, 
                                    CreatedByUserID, 
                                    IsLocked, 
                                    RetakeTestApplicationID
                                )
                                VALUES 
                                (
                                    @TestTypeID, 
                                    @LocalDrivingLicenseApplicationID, 
                                    @AppointmentDate, 
                                    @PaidFees, 
                                    @CreatedByUserID, 
                                    @IsLocked, 
                                    @RetakeTestApplicationID
                                );
                                SELECT SCOPE_IDENTITY();";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TestTypeID", testTypeID);
                    command.Parameters.AddWithValue("@LocalDrivingLicenseApplicationID", localDrivingLicenseApplicationID);
                    command.Parameters.AddWithValue("@AppointmentDate", appointmentDate);
                    command.Parameters.AddWithValue("@PaidFees", paidFees);
                    command.Parameters.AddWithValue("@CreatedByUserID", createdByUserID);
                    command.Parameters.AddWithValue("@IsLocked", isLocked);

                    // التعامل مع RetakeTestApplicationID إذا كانت -1 وتمرير DBNull.Value بدلاً منها
                    if (retakeTestApplicationID == -1)
                        command.Parameters.AddWithValue("@RetakeTestApplicationID", DBNull.Value);
                    else
                        command.Parameters.AddWithValue("@RetakeTestApplicationID", retakeTestApplicationID);

                    try
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();

                        if (result != null && int.TryParse(result.ToString(), out int insertedID))
                        {
                            testAppointmentID = insertedID;
                        }
                    }
                    catch (Exception)
                    {
                        // يمكن تسجيل الخطأ (Logging) هنا
                        testAppointmentID = -1;
                    }
                }
            }

            return testAppointmentID;
        }
        public static DataTable GetApplicationAppointments(int localDrivingLicenseApplicationID, int testTypeID)
        {
            DataTable dt = new DataTable();

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                // استعلام يسترجع المواعيد مع التحقق مما إذا كان الموعد قد تم إجراؤه (IsLocked)
                string query = @"SELECT TestAppointmentID, 
                                       AppointmentDate, 
                                       PaidFees, 
                                       IsLocked
                                FROM TestAppointments
                                WHERE LocalDrivingLicenseApplicationID = @LocalDrivingLicenseApplicationID
                                  AND TestTypeID = @TestTypeID
                                ORDER BY TestAppointmentID DESC";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@LocalDrivingLicenseApplicationID", localDrivingLicenseApplicationID);
                    command.Parameters.AddWithValue("@TestTypeID", testTypeID);

                    try
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                dt.Load(reader);
                            }
                        }
                    }
                    catch (Exception)
                    {
                        // يمكن تسجيل الخطأ (Logging) هنا
                    }
                }
            }

            return dt;
        }
        public static DataTable GetApplicationAppointmentsList(int localDrivingLicenseApplicationID)
        {
            DataTable dt = new DataTable();

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string query = @"SELECT 
                                    LDL.LocalDrivingLicenseApplicationID,
                                    LicenseClasses.ClassName,
                                    People.PersonID,
                                    (People.FirstName + ' ' + ISNULL(People.SecondName, '') + ' ' + ISNULL(People.ThirdName, '') + ' ' + People.LastName) AS FullName,
                                    Applications.ApplicationDate,
                                    Applications.LastStatusDate,
                                    Applications.PaidFees AS ApplicationPaidFees,
                                    Applications.ApplicationStatus AS STATUS,
                                    Users.UserName AS CreatedByUserName,
                                    People.ImagePath,

                                    CASE 
                                        WHEN (
                                            SELECT COUNT(*) 
                                            FROM TestAppointments TA
                                            INNER JOIN Tests T ON TA.TestAppointmentID = T.TestAppointmentID
                                            WHERE TA.LocalDrivingLicenseApplicationID = LDL.LocalDrivingLicenseApplicationID
                                            AND T.TestResult = 1
                                        ) = 0 THEN 1 
        
                                        WHEN (
                                            SELECT COUNT(*) 
                                            FROM TestAppointments TA
                                            INNER JOIN Tests T ON TA.TestAppointmentID = T.TestAppointmentID
                                            WHERE TA.LocalDrivingLicenseApplicationID = LDL.LocalDrivingLicenseApplicationID
                                            AND T.TestResult = 1
                                        ) = 1 THEN 2
        
                                        ELSE 3 
                                    END AS TestTypeID

                                FROM LocalDrivingLicenseApplications LDL
                                INNER JOIN Applications ON LDL.ApplicationID = Applications.ApplicationID
                                INNER JOIN People ON Applications.ApplicantPersonID = People.PersonID
                                INNER JOIN LicenseClasses ON LDL.LicenseClassID = LicenseClasses.LicenseClassID
                                INNER JOIN Users ON Applications.CreatedByUserID = Users.UserID
                                WHERE LDL.LocalDrivingLicenseApplicationID = @LocalDrivingLicenseApplicationID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@LocalDrivingLicenseApplicationID", localDrivingLicenseApplicationID);

                    try
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                dt.Load(reader);
                            }
                        }
                    }
                    catch (Exception)
                    {
                        // Logging/Handling Error if needed
                    }
                }
            }

            return dt;
        }
        public static bool UpdateTestAppointment(
            int testAppointmentID,
            int testTypeID,
            int localDrivingLicenseApplicationID,
            DateTime appointmentDate,
            decimal paidFees,
            int createdByUserID,
            bool isLocked,
            int retakeTestApplicationID)
        {
            int rowsAffected = 0;

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string query = @"UPDATE TestAppointments
                                SET TestTypeID = @TestTypeID,
                                    LocalDrivingLicenseApplicationID = @LocalDrivingLicenseApplicationID,
                                    AppointmentDate = @AppointmentDate,
                                    PaidFees = @PaidFees,
                                    CreatedByUserID = @CreatedByUserID,
                                    IsLocked = @IsLocked,
                                    RetakeTestApplicationID = @RetakeTestApplicationID
                                WHERE TestAppointmentID = @TestAppointmentID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TestAppointmentID", testAppointmentID);
                    command.Parameters.AddWithValue("@TestTypeID", testTypeID);
                    command.Parameters.AddWithValue("@LocalDrivingLicenseApplicationID", localDrivingLicenseApplicationID);
                    command.Parameters.AddWithValue("@AppointmentDate", appointmentDate);
                    command.Parameters.AddWithValue("@PaidFees", paidFees);
                    command.Parameters.AddWithValue("@CreatedByUserID", createdByUserID);
                    command.Parameters.AddWithValue("@IsLocked", isLocked);

                    // التعامل مع RetakeTestApplicationID إذا كانت -1 وتحويلها إلى DBNull.Value
                    if (retakeTestApplicationID == -1)
                        command.Parameters.AddWithValue("@RetakeTestApplicationID", DBNull.Value);
                    else
                        command.Parameters.AddWithValue("@RetakeTestApplicationID", retakeTestApplicationID);

                    try
                    {
                        connection.Open();
                        rowsAffected = command.ExecuteNonQuery();
                    }
                    catch (Exception)
                    {
                        // Logging/Handling Error if needed
                        return false;
                    }
                }
            }

            return (rowsAffected > 0);
        }
        public static int GetTotalTrialsPerTest(int LocalDrivingLicenseApplicationID, int TestTypeID)
        {
            int totalTrials = 0;

            // يمكنك تغيير اسم كلاس الاتصال بحسب ما هو معرف لديك في المشروع (مثل clsDataAccessSettings.ConnectionString)
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                // تحسب هذه الاستعلامات عدد الاختبارات الفاشلة (Total Failed Trials)
                // التي أداها الطالب لنفس الطلب ونوع الاختبار
                string query = @"SELECT COUNT(*) 
                                 FROM Tests 
                                 INNER JOIN TestAppointments 
                                     ON Tests.TestAppointmentID = TestAppointments.TestAppointmentID
                                 WHERE TestAppointments.LocalDrivingLicenseApplicationID = @LocalDrivingLicenseApplicationID
                                   AND TestAppointments.TestTypeID = @TestTypeID
                                   AND Tests.TestResult = 0;"; // 0 تعني فاشل (Failed)

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@LocalDrivingLicenseApplicationID", LocalDrivingLicenseApplicationID);
                    command.Parameters.AddWithValue("@TestTypeID", TestTypeID);

                    try
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();

                        if (result != null && int.TryParse(result.ToString(), out int count))
                        {
                            totalTrials = count;
                        }
                    }
                    catch (Exception)
                    {
                        // يمكنك تسجيل الخطأ هنا حسب نظام Logging لديك
                        // Console.WriteLine("Error: " + ex.Message);
                        totalTrials = 0;
                    }
                }
            }

            return totalTrials;
        }
         public static bool IsRetakeTest(int LocalDrivingLicenseApplicationID, int TestTypeID)
         {
             bool isRetake = false;

             using (SqlConnection connection = new SqlConnection(ConnectionString))
             {
                 // الاستعلام يفحص ما إذا كان هناك اختبار سابق بنفس النوع والطلب وكانت نتيجته رسوب (0)
                 string query = @"SELECT TOP 1 1
                                  FROM TestAppointments TA
                                  INNER JOIN Tests T
                                      ON T.TestAppointmentID = TA.TestAppointmentID
                                  WHERE TA.LocalDrivingLicenseApplicationID = @LocalDrivingLicenseApplicationID
                                    AND TA.TestTypeID = @TestTypeID
                                    AND T.TestResult = 0;";

                 using (SqlCommand command = new SqlCommand(query, connection))
                 {
                     command.Parameters.AddWithValue("@LocalDrivingLicenseApplicationID", LocalDrivingLicenseApplicationID);
                     command.Parameters.AddWithValue("@TestTypeID", TestTypeID);

                     try
                     {
                         connection.Open();
                         object result = command.ExecuteScalar();

                         if (result != null && result != DBNull.Value)
                         {
                             isRetake = true;
                         }
                     }
                     catch (Exception)
                     {
                         // Logging handling logic...
                         isRetake = false;
                     }
                 }
             }

             return isRetake;
         }
    }
}