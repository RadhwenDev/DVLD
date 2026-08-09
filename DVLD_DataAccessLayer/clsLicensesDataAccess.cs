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
    public class clsLicensesDataAccess
    {
        public static string ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        public static DataTable getAllLicenses()
        {
            DataTable dt = new DataTable();
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string query = @"SELECT [LICENSE ID] = L.LicenseID, DRIVER =P.FirstName + ISNULL(' ' + NULLIF(P.SecondName, ''), '')  + ISNULL(' ' + NULLIF(P.ThirdName, ''), '') + ISNULL(' ' + NULLIF(P.LastName, ''), ''), CLASS = LC.ClassName,
                                 [ISSUE DATE] = L.IssueDate, EXPIRATION = L.ExpirationDate, REASON = A_T.ApplicationTypeTitle, 
                                  CASE
                                      WHEN L.IsActive = 0 THEN 'Expired'
                                      WHEN L.IsActive = 1 THEN 'Active'
                                      ELSE 'Unknown'
                                  END as [STATUS]
                                 FROM Licenses L INNER JOIN Drivers D ON L.DriverID = D.DriverID
                                 INNER JOIN People P ON D.PersonID = P.PersonID
                                 INNER JOIN LicenseClasses LC ON L.LicenseClass = LC.LicenseClassID
                                 INNER JOIN Applications A ON L.ApplicationID = A.ApplicationID
                                 INNER JOIN ApplicationTypes A_T ON A.ApplicationTypeID = A_T.ApplicationTypeID;";
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

        public static int getTotalActiveLicenses()
        {
            int totalActiveLicense = 0;
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string query = @"select count(*) from Licenses where isActive = 1";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    try
                    {
                        connection.Open();

                        object result = command.ExecuteScalar();
                        if (result != null && int.TryParse(result.ToString(), out int insertedValue))
                        {
                            totalActiveLicense = insertedValue;
                        }
                    }
                    catch (Exception) { }
                }
            }

            return totalActiveLicense;
        }
        public static DataTable getShowLicense(int ApplicationID)
        {
            DataTable dt = new DataTable();
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string query = @"SELECT 
                                    Licenses.LicenseID,
                                    Licenses.DriverID,
                                    People.FirstName + ISNULL(' ' + NULLIF(People.SecondName, ''), '')  + ISNULL(' ' + NULLIF(People.ThirdName, ''), '') + ISNULL(' ' + NULLIF(People.LastName, ''), '') AS FullName,
                                    People.NationalNo,
	                                CASE 
                                        WHEN People.Gendor = 0 THEN 'Male' 
                                        ELSE 'Female' 
                                    END AS Gender,
                                    People.DateOfBirth,
                                    People.ImagePath,
                                    LicenseClasses.ClassName AS LicenseClass,
                                    Licenses.IssueDate,
                                    Licenses.ExpirationDate,
	                                CASE 
                                        WHEN Licenses.IsActive = 0 THEN 'No' 
                                        ELSE 'Yes' 
                                    END AS IsActive,
                                    Licenses.Notes,
                                    Licenses.PaidFees,
                                    ApplicationTypes.ApplicationTypeTitle AS IssueReason,
                                    CASE 
                                        WHEN DetainedLicenses.LicenseID IS NULL THEN 'No' 
                                        ELSE 'Yes' 
                                    END AS IsDetained
                                FROM Licenses
                                INNER JOIN Drivers ON Licenses.DriverID = Drivers.DriverID
                                INNER JOIN People ON Drivers.PersonID = People.PersonID
                                INNER JOIN LicenseClasses ON Licenses.LicenseClass = LicenseClasses.LicenseClassID
                                INNER JOIN Applications ON Licenses.ApplicationID = Applications.ApplicationID
                                INNER JOIN ApplicationTypes ON Applications.ApplicationTypeID = ApplicationTypes.ApplicationTypeID
                                LEFT JOIN DetainedLicenses ON Licenses.LicenseID = DetainedLicenses.LicenseID AND DetainedLicenses.IsReleased = 0
                                WHERE Licenses.ApplicationID = @ApplicationID;";

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
        public static DataTable getShowLicenseRelease(int ApplicationID)
        {
            DataTable dt = new DataTable();
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string query = @"SELECT 
	                                Licenses.LicenseID,
                                    Licenses.DriverID,
	                                People.FirstName + ISNULL(' ' + NULLIF(People.SecondName, ''), '')  + ISNULL(' ' + NULLIF(People.ThirdName, ''), '') + ISNULL(' ' + NULLIF(People.LastName, ''), '') AS FullName,
                                    People.NationalNo,
	                                CASE 
                                        WHEN People.Gendor = 0 THEN 'Male' 
                                        ELSE 'Female' 
                                    END AS Gender,
                                    People.DateOfBirth,
                                    People.ImagePath,
	                                LC.ClassName AS LicenseClass,
	                                Licenses.IssueDate,
                                    Licenses.ExpirationDate,
	                                CASE 
                                        WHEN Licenses.IsActive = 0 THEN 'No' 
                                        ELSE 'Yes' 
                                    END AS IsActive,
                                    Licenses.Notes,
                                    Licenses.PaidFees,
	                                A_T.ApplicationTypeTitle AS IssueReason,
	                                CASE 
                                        WHEN DL.LicenseID IS NULL THEN 'No' 
                                        ELSE 'Yes' 
                                    END AS IsDetained
                                FROM Applications A
                                INNER JOIN People ON A.ApplicantPersonID = People.PersonID
                                INNER JOIN DetainedLicenses DL ON A.ApplicationID = DL.ReleaseApplicationID		
                                INNER JOIN Licenses ON DL.LicenseID = Licenses.LicenseID
                                INNER JOIN LicenseClasses LC ON Licenses.LicenseClass = LC.LicenseClassID
                                INNER JOIN ApplicationTypes A_T ON A.ApplicationTypeID = A_T.ApplicationTypeID
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
        public static bool hasLicense(int personID)
        {
            bool isFound = false;

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string query = @"SELECT Found = 1 FROM Licenses L
                                 INNER JOIN Applications A ON L.ApplicationID = A.ApplicationID
                                 WHERE A.ApplicantPersonID = @PersonID
                                 And L.IsActive = 1";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@PersonID", personID);

                    try
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();

                        if (result != null)
                        {
                            isFound = true;
                        }
                    }
                    catch (Exception )
                    {
                        isFound = false;
                    }
                }
            }

            return isFound;
        }
        public static bool hasInternationalLicense(int personID)
        {
            bool isFound = false;

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string query = @"SELECT Found = 1 FROM InternationalLicenses IL
                                 INNER JOIN Drivers D ON IL.DriverID = D.DriverID
                                 WHERE D.PersonID = @PersonID
                                 AND IL.IsActive = 1";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@PersonID", personID);

                    try
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();

                        if (result != null)
                        {
                            isFound = true;
                        }
                    }
                    catch (Exception )
                    {
                        isFound = false;
                    }
                }
            }

            return isFound;
        }
        public static int AddNewLicense(int ApplicationID, int DriverID, int LicenseClass,
            DateTime IssueDate, DateTime ExpirationDate, string Notes, decimal PaidFees,
            bool IsActive, byte IssueReason, int CreatedByUserID)
        {
            int LicenseID = -1;

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string query = @"INSERT INTO Licenses 
                                (ApplicationID, DriverID, LicenseClass, IssueDate, ExpirationDate, Notes, PaidFees, IsActive, IssueReason, CreatedByUserID)
                                VALUES 
                                (@ApplicationID, @DriverID, @LicenseClass, @IssueDate, @ExpirationDate, @Notes, @PaidFees, @IsActive, @IssueReason, @CreatedByUserID);
                                SELECT SCOPE_IDENTITY();";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ApplicationID", ApplicationID);
                    command.Parameters.AddWithValue("@DriverID", DriverID);
                    command.Parameters.AddWithValue("@LicenseClass", LicenseClass);
                    command.Parameters.AddWithValue("@IssueDate", IssueDate);
                    command.Parameters.AddWithValue("@ExpirationDate", ExpirationDate);

                    if (string.IsNullOrEmpty(Notes))
                        command.Parameters.AddWithValue("@Notes", DBNull.Value);
                    else
                        command.Parameters.AddWithValue("@Notes", Notes);

                    command.Parameters.AddWithValue("@PaidFees", PaidFees);
                    command.Parameters.AddWithValue("@IsActive", IsActive);
                    command.Parameters.AddWithValue("@IssueReason", IssueReason);
                    command.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);

                    try
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();

                        if (result != null && int.TryParse(result.ToString(), out int insertedID))
                        {
                            LicenseID = insertedID;
                        }
                    }
                    catch (Exception)
                    {
                        LicenseID = -1;
                    }
                }
            }

            return LicenseID;
        }
        public static int GetLicenseIDByApplicationID(int DriverID)
        {
            int licenseID = -1;

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string query = @"SELECT LicenseClass FROM Licenses L INNER JOIN Drivers D ON L.DriverID = D.DriverID WHERE D.DriverID = @DriverID;";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@DriverID", DriverID);

                    try
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();

                        if (result != null && int.TryParse(result.ToString(), out int foundID))
                        {
                            licenseID = foundID;
                        }
                    }
                    catch (Exception)
                    {
                        licenseID = -1;
                    }
                }
            }

            return licenseID;
        }
        public static bool DeactivateLicense(int LicenseID)
        {
            int rowsAffected = 0;

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string query = @"UPDATE Licenses 
                         SET IsActive = 0 
                         WHERE LicenseID = @LicenseID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@LicenseID", LicenseID);

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
        public static bool GetLicenseInfoByID(
    int LicenseID,
    ref int ApplicationID,
    ref int DriverID,
    ref int LicenseClass,
    ref DateTime IssueDate,
    ref DateTime ExpirationDate,
    ref string Notes,
    ref decimal PaidFees,
    ref bool IsActive,
    ref byte IssueReason,
    ref int CreatedByUserID)
        {
            bool isFound = false;

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string query = "SELECT * FROM Licenses WHERE LicenseID = @LicenseID";

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
                                // Record found
                                isFound = true;

                                ApplicationID = (int)reader["ApplicationID"];
                                DriverID = (int)reader["DriverID"];
                                LicenseClass = (int)reader["LicenseClass"];
                                IssueDate = (DateTime)reader["IssueDate"];
                                ExpirationDate = (DateTime)reader["ExpirationDate"];

                                Notes = reader["Notes"] != DBNull.Value ? (string)reader["Notes"] : "";
                                PaidFees = Convert.ToDecimal(reader["PaidFees"]);
                                IsActive = (bool)reader["IsActive"];
                                IssueReason = Convert.ToByte(reader["IssueReason"]);
                                CreatedByUserID = (int)reader["CreatedByUserID"];
                            }
                        }
                    }
                    catch (Exception)
                    {
                        isFound = false;
                        // يمكنك تسجيل الخطأ هنا (Logging) حسب نظام المشروع لديك
                    }
                }
            }

            return isFound;
        }
    }
}
