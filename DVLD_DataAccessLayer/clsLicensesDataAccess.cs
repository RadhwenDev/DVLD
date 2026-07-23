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
                string query = @"SELECT [LICENSE ID] = L.LicenseID, DRIVER = (P.FirstName + ' ' + P.SecondName + ' ' + P.ThirdName + ' ' + P.LastName), CLASS = LC.ClassName,
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
                                    People.FirstName + ' ' + People.SecondName + ' ' + People.ThirdName + ' ' + People.LastName AS FullName,
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
    }
}
