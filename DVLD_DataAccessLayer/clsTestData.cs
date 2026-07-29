using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLD_DataAccessLayer
{
    public class clsTestData
    {
        public static string ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

        public static int AddNewTest(int testAppointmentID, bool testResult, string notes, int createdByUserID)
        {
            int testID = -1;

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                // Stored Procedure أو Query لإضافة الاختبار وتحديث IsLocked في نفس الوقت
                string query = @"
                    INSERT INTO Tests (TestAppointmentID, TestResult, Notes, CreatedByUserID)
                    VALUES (@TestAppointmentID, @TestResult, @Notes, @CreatedByUserID);
                    SELECT SCOPE_IDENTITY();

                    -- تحديث الموعد ليصبح مغلقاً بعد تسجيل النتيجة
                    UPDATE TestAppointments 
                    SET IsLocked = 1 
                    WHERE TestAppointmentID = @TestAppointmentID;";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TestAppointmentID", testAppointmentID);
                    command.Parameters.AddWithValue("@TestResult", testResult);

                    if (string.IsNullOrEmpty(notes))
                        command.Parameters.AddWithValue("@Notes", DBNull.Value);
                    else
                        command.Parameters.AddWithValue("@Notes", notes);

                    command.Parameters.AddWithValue("@CreatedByUserID", createdByUserID);

                    try
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();

                        if (result != null && int.TryParse(result.ToString(), out int insertedID))
                        {
                            testID = insertedID;
                        }
                    }
                    catch (Exception)
                    {
                        // Log error if needed
                        testID = -1;
                    }
                }
            }

            return testID;
        }

        public static bool Find(int testID, ref int testAppointmentID, ref bool testResult, ref string notes, ref int createdByUserID)
        {
            bool isFound = false;

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string query = "SELECT * FROM Tests WHERE TestID = @TestID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TestID", testID);

                    try
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                isFound = true;
                                testAppointmentID = (int)reader["TestAppointmentID"];
                                testResult = (bool)reader["TestResult"];
                                notes = reader["Notes"] != DBNull.Value ? (string)reader["Notes"] : "";
                                createdByUserID = (int)reader["CreatedByUserID"];
                            }
                        }
                    }
                    catch (Exception)
                    {
                        isFound = false;
                    }
                }
            }

            return isFound;
        }
    }
}
