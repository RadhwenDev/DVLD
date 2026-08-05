using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace DVLD_DataAccess
{
    public class clsDataAccessInternationalLicense
    {
        public static string ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        public static bool GetInternationalLicenseInfoByID(int internationalLicenseID,
            ref int applicationID, ref int driverID, ref int issuedUsingLocalLicenseID,
            ref DateTime issueDate, ref DateTime expirationDate, ref bool isActive, ref int createdByUserID)
        {
            bool isFound = false;

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string query = @"SELECT * FROM InternationalLicenses 
                                WHERE InternationalLicenseID = @InternationalLicenseID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@InternationalLicenseID", internationalLicenseID);

                    try
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                isFound = true;

                                applicationID = (int)reader["ApplicationID"];
                                driverID = (int)reader["DriverID"];
                                issuedUsingLocalLicenseID = (int)reader["IssuedUsingLocalLicenseID"];
                                issueDate = (DateTime)reader["IssueDate"];
                                expirationDate = (DateTime)reader["ExpirationDate"];
                                isActive = (bool)reader["IsActive"];
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

        public static DataTable GetAllInternationalLicenses()
        {
            DataTable dt = new DataTable();

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string query = @"SELECT InternationalLicenseID, ApplicationID, DriverID, 
                                        IssuedUsingLocalLicenseID, IssueDate, ExpirationDate, 
                                        IsActive 
                                 FROM InternationalLicenses 
                                 ORDER BY InternationalLicenseID DESC";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
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
                        // Handle exceptions
                    }
                }
            }

            return dt;
        }

        public static int AddNewInternationalLicense(int applicationID, int driverID,
            int issuedUsingLocalLicenseID, DateTime issueDate, DateTime expirationDate,
            bool isActive, int createdByUserID)
        {
            int internationalLicenseID = -1;

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string query = @"INSERT INTO InternationalLicenses 
                                (ApplicationID, DriverID, IssuedUsingLocalLicenseID, IssueDate, ExpirationDate, IsActive, CreatedByUserID)
                                VALUES 
                                (@ApplicationID, @DriverID, @IssuedUsingLocalLicenseID, @IssueDate, @ExpirationDate, @IsActive, @CreatedByUserID);
                                SELECT SCOPE_IDENTITY();";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ApplicationID", applicationID);
                    command.Parameters.AddWithValue("@DriverID", driverID);
                    command.Parameters.AddWithValue("@IssuedUsingLocalLicenseID", issuedUsingLocalLicenseID);
                    command.Parameters.AddWithValue("@IssueDate", issueDate);
                    command.Parameters.AddWithValue("@ExpirationDate", expirationDate);
                    command.Parameters.AddWithValue("@IsActive", isActive);
                    command.Parameters.AddWithValue("@CreatedByUserID", createdByUserID);

                    try
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();

                        if (result != null && int.TryParse(result.ToString(), out int insertedID))
                        {
                            internationalLicenseID = insertedID;
                        }
                    }
                    catch (Exception)
                    {
                        // Handle exceptions
                    }
                }
            }

            return internationalLicenseID;
        }

        public static bool UpdateInternationalLicense(int internationalLicenseID, int applicationID,
            int driverID, int issuedUsingLocalLicenseID, DateTime issueDate, DateTime expirationDate,
            bool isActive, int createdByUserID)
        {
            int rowsAffected = 0;

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string query = @"UPDATE InternationalLicenses
                                SET ApplicationID = @ApplicationID,
                                    DriverID = @DriverID,
                                    IssuedUsingLocalLicenseID = @IssuedUsingLocalLicenseID,
                                    IssueDate = @IssueDate,
                                    ExpirationDate = @ExpirationDate,
                                    IsActive = @IsActive,
                                    CreatedByUserID = @CreatedByUserID
                                WHERE InternationalLicenseID = @InternationalLicenseID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@InternationalLicenseID", internationalLicenseID);
                    command.Parameters.AddWithValue("@ApplicationID", applicationID);
                    command.Parameters.AddWithValue("@DriverID", driverID);
                    command.Parameters.AddWithValue("@IssuedUsingLocalLicenseID", issuedUsingLocalLicenseID);
                    command.Parameters.AddWithValue("@IssueDate", issueDate);
                    command.Parameters.AddWithValue("@ExpirationDate", expirationDate);
                    command.Parameters.AddWithValue("@IsActive", isActive);
                    command.Parameters.AddWithValue("@CreatedByUserID", createdByUserID);

                    try
                    {
                        connection.Open();
                        rowsAffected = command.ExecuteNonQuery();
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                }
            }

            return (rowsAffected > 0);
        }

        public static int GetActiveInternationalLicenseIDByDriverID(int driverID)
        {
            int internationalLicenseID = -1;

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string query = @"SELECT TOP 1 InternationalLicenseID 
                                FROM InternationalLicenses 
                                WHERE DriverID = @DriverID AND IsActive = 1 AND ExpirationDate >= GETDATE()
                                ORDER BY ExpirationDate DESC";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@DriverID", driverID);

                    try
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();

                        if (result != null && int.TryParse(result.ToString(), out int insertedID))
                        {
                            internationalLicenseID = insertedID;
                        }
                    }
                    catch (Exception)
                    {
                        // Handle exceptions
                    }
                }
            }

            return internationalLicenseID;
        }
    }
}