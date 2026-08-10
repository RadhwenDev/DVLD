using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace DVLD_DataAccess
{
    public class clsDetainedLicenseDataAccess
    {
        public static string ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        public static bool GetDetainedLicenseInfoByID(int DetainID, ref int LicenseID, ref DateTime DetainDate,
            ref decimal FineFees, ref int CreatedByUserID, ref bool IsReleased,
            ref DateTime? ReleaseDate, ref int? ReleasedByUserID, ref int? ReleaseApplicationID)
        {
            bool isFound = false;

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string query = "SELECT * FROM DetainedLicenses WHERE DetainID = @DetainID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@DetainID", DetainID);

                    try
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                isFound = true;

                                LicenseID = (int)reader["LicenseID"];
                                DetainDate = (DateTime)reader["DetainDate"];
                                FineFees = Convert.ToDecimal(reader["FineFees"]);
                                CreatedByUserID = (int)reader["CreatedByUserID"];
                                IsReleased = (bool)reader["IsReleased"];

                                ReleaseDate = reader["ReleaseDate"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["ReleaseDate"];
                                ReleasedByUserID = reader["ReleasedByUserID"] == DBNull.Value ? (int?)null : (int)reader["ReleasedByUserID"];
                                ReleaseApplicationID = reader["ReleaseApplicationID"] == DBNull.Value ? (int?)null : (int)reader["ReleaseApplicationID"];
                            }
                        }
                    }
                    catch (Exception) { isFound = false; }
                }
            }

            return isFound;
        }

        // 2. جلب آخر سجل احتجاز غير مفروج عنه بواسطة LicenseID
        public static bool GetDetainedLicenseInfoByLicenseID(int LicenseID, ref int DetainID, ref DateTime DetainDate,
            ref decimal FineFees, ref int CreatedByUserID, ref bool IsReleased,
            ref DateTime? ReleaseDate, ref int? ReleasedByUserID, ref int? ReleaseApplicationID)
        {
            bool isFound = false;

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string query = @"SELECT TOP 1 * FROM DetainedLicenses 
                                 WHERE LicenseID = @LicenseID 
                                 ORDER BY DetainID DESC";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@LicenseID", LicenseID);

                    try
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                isFound = true;

                                DetainID = (int)reader["DetainID"];
                                DetainDate = (DateTime)reader["DetainDate"];
                                FineFees = Convert.ToDecimal(reader["FineFees"]);
                                CreatedByUserID = (int)reader["CreatedByUserID"];
                                IsReleased = (bool)reader["IsReleased"];

                                ReleaseDate = reader["ReleaseDate"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["ReleaseDate"];
                                ReleasedByUserID = reader["ReleasedByUserID"] == DBNull.Value ? (int?)null : (int)reader["ReleasedByUserID"];
                                ReleaseApplicationID = reader["ReleaseApplicationID"] == DBNull.Value ? (int?)null : (int)reader["ReleaseApplicationID"];
                            }
                        }
                    }
                    catch (Exception) { isFound = false; }
                }
            }

            return isFound;
        }
        public static bool GetDetainedLicenseInfoByPersonID(
    int personID, ref int detainID, ref int licenseID, ref DateTime detainDate,
    ref decimal fineFees, ref int createdByUserID, ref bool isReleased,
    ref DateTime? releaseDate, ref int? releasedByUserID, ref int? releaseApplicationID)
        {
            bool isFound = false;

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string query = @"
            SELECT TOP 1 DL.* 
            FROM DetainedLicenses DL
            INNER JOIN Licenses L ON DL.LicenseID = L.LicenseID
            INNER JOIN Drivers D ON L.DriverID = D.DriverID
            WHERE D.PersonID = @PersonID AND DL.IsReleased = 0
            ORDER BY DL.DetainID DESC";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@PersonID", personID);

                    try
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                isFound = true;

                                detainID = (int)reader["DetainID"];
                                licenseID = (int)reader["LicenseID"];
                                detainDate = (DateTime)reader["DetainDate"];
                                fineFees = (decimal)reader["FineFees"];
                                createdByUserID = (int)reader["CreatedByUserID"];
                                isReleased = (bool)reader["IsReleased"];

                                releaseDate = reader["ReleaseDate"] == DBNull.Value ? null : (DateTime?)reader["ReleaseDate"];
                                releasedByUserID = reader["ReleasedByUserID"] == DBNull.Value ? null : (int?)reader["ReleasedByUserID"];
                                releaseApplicationID = reader["ReleaseApplicationID"] == DBNull.Value ? null : (int?)reader["ReleaseApplicationID"];
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

        // 3. إضافة سجل احتجاز جديد
        public static int AddNewDetainedLicense(int LicenseID, DateTime DetainDate, decimal FineFees, int CreatedByUserID)
        {
            int detainID = -1;

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string query = @"INSERT INTO DetainedLicenses (LicenseID, DetainDate, FineFees, CreatedByUserID, IsReleased)
                                 VALUES (@LicenseID, @DetainDate, @FineFees, @CreatedByUserID, 0);
                                 SELECT SCOPE_IDENTITY();";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@LicenseID", LicenseID);
                    command.Parameters.AddWithValue("@DetainDate", DetainDate);
                    command.Parameters.AddWithValue("@FineFees", FineFees);
                    command.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);

                    try
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();

                        if (result != null && int.TryParse(result.ToString(), out int insertedID))
                        {
                            detainID = insertedID;
                        }
                    }
                    catch (Exception) { }
                }
            }

            return detainID;
        }

        // 4. تحديث سجل الاحتجاز كامل (Update)
        public static bool UpdateDetainedLicense(int DetainID, int LicenseID, DateTime DetainDate, decimal FineFees,
            int CreatedByUserID, bool IsReleased, DateTime? ReleaseDate, int? ReleasedByUserID, int? ReleaseApplicationID)
        {
            int rowsAffected = 0;

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string query = @"UPDATE DetainedLicenses
                                 SET LicenseID = @LicenseID,
                                     DetainDate = @DetainDate,
                                     FineFees = @FineFees,
                                     CreatedByUserID = @CreatedByUserID,
                                     IsReleased = @IsReleased,
                                     ReleaseDate = @ReleaseDate,
                                     ReleasedByUserID = @ReleasedByUserID,
                                     ReleaseApplicationID = @ReleaseApplicationID
                                 WHERE DetainID = @DetainID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@DetainID", DetainID);
                    command.Parameters.AddWithValue("@LicenseID", LicenseID);
                    command.Parameters.AddWithValue("@DetainDate", DetainDate);
                    command.Parameters.AddWithValue("@FineFees", FineFees);
                    command.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);
                    command.Parameters.AddWithValue("@IsReleased", IsReleased);

                    command.Parameters.AddWithValue("@ReleaseDate", (object)ReleaseDate ?? DBNull.Value);
                    command.Parameters.AddWithValue("@ReleasedByUserID", (object)ReleasedByUserID ?? DBNull.Value);
                    command.Parameters.AddWithValue("@ReleaseApplicationID", (object)ReleaseApplicationID ?? DBNull.Value);

                    try
                    {
                        connection.Open();
                        rowsAffected = command.ExecuteNonQuery();
                    }
                    catch (Exception) { return false; }
                }
            }

            return (rowsAffected > 0);
        }

        // 5. ميثود سريعة لفك الاحتجاز مباشرة (Release)
        public static bool ReleaseDetainedLicense(int DetainID, int ReleasedByUserID, int ReleaseApplicationID)
        {
            int rowsAffected = 0;

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string query = @"UPDATE DetainedLicenses
                                 SET IsReleased = 1,
                                     ReleaseDate = @ReleaseDate,
                                     ReleasedByUserID = @ReleasedByUserID,
                                     ReleaseApplicationID = @ReleaseApplicationID
                                 WHERE DetainID = @DetainID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@DetainID", DetainID);
                    command.Parameters.AddWithValue("@ReleaseDate", DateTime.Now);
                    command.Parameters.AddWithValue("@ReleasedByUserID", ReleasedByUserID);
                    command.Parameters.AddWithValue("@ReleaseApplicationID", ReleaseApplicationID);

                    try
                    {
                        connection.Open();
                        rowsAffected = command.ExecuteNonQuery();
                    }
                    catch (Exception) { return false; }
                }
            }

            return (rowsAffected > 0);
        }

        // 6. التحقق مما إذا كانت الرخصة محتجزة حالياً (غير مفروج عنها)
        public static bool IsLicenseDetained(int LicenseID)
        {
            bool isDetained = false;

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string query = @"SELECT IsDetained = 1 FROM DetainedLicenses 
                                 WHERE LicenseID = @LicenseID AND IsReleased = 0";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@LicenseID", LicenseID);

                    try
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();

                        if (result != null)
                        {
                            isDetained = Convert.ToBoolean(result);
                        }
                    }
                    catch (Exception) { }
                }
            }

            return isDetained;
        }

        // 7. جلب جميع الرخص المحتجزة
        public static DataTable GetAllDetainedLicenses()
        {
            DataTable dt = new DataTable();

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string query = @"SELECT [Detain ID] = DetainID, [License ID] = DetainedLicenses.LicenseID, 
                                 [Driver Name] = P.FirstName + ISNULL(' ' + NULLIF(P.SecondName, ''), '')  + ISNULL(' ' + NULLIF(P.ThirdName, ''), '') + ISNULL(' ' + NULLIF(P.LastName, ''), ''),
                                 [Detain Date] = DetainDate, [Fine Fees] = FineFees, [Status] = CASE WHEN IsReleased = 1 THEN 'Released' ELSE 'Detained' END FROM DetainedLicenses 
                                 INNER JOIN Licenses L ON DetainedLicenses.LicenseID = L.LicenseID INNER JOIN Drivers D ON L.DriverID = D.DriverID
                                 INNER JOIN People P ON D.PersonID = P.PersonID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
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