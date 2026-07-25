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
    public class clsDriverDataAccess
    {
        public static string ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        public static DataTable getLicenseHistory(int ApplicationID)
        {
            DataTable dt = new DataTable();
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string query = @"SELECT P.PersonID, FullName = (P.FirstName + ' ' + P.SecondName + ' ' + P.ThirdName + ' ' + P.LastName),
                                 NationalNo, DateOfBirth, Address, Email, Phone, CountryName, ImagePath,
                                 CASE
                                    WHEN P.Gendor = 0 THEN 'Male'
                                    ELSE 'Female'
                                 END AS Gender
                                 from People P inner join Countries C on P.NationalityCountryID = C.CountryID
                                 inner join Applications A ON P.PersonID = A.ApplicantPersonID
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

        public static DataTable getLocalLicenseHistory(int ApplicationID)
        {
            DataTable dt = new DataTable();
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string query = @"SELECT 
                                    Licenses.LicenseID AS [Lic.ID],
                                    Licenses.ApplicationID AS [App.ID],
                                    LC.ClassName AS [Class Name],
                                    Licenses.IssueDate AS [Issue Date],
                                    Licenses.ExpirationDate AS [Expiration Date],
                                    Licenses.IsActive AS [Is Active]
                                FROM Licenses 
                                INNER JOIN LicenseClasses LC ON Licenses.LicenseClass = LC.LicenseClassID
                                INNER JOIN Drivers D ON Licenses.DriverID = D.DriverID
                                WHERE Licenses.ApplicationID = @ApplicationID";
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

        public static DataTable getInternationalLicenseHistory(int ApplicationID)
        {
            DataTable dt = new DataTable();
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string query = @"SELECT 
                                    IL.InternationalLicenseID AS [Int.Lic.ID],
                                    IL.ApplicationID AS [App.ID],
                                    IL.IssuedUsingLocalLicenseID AS [L.Lic.ID],
                                    IL.IssueDate AS [Issue Date],
                                    IL.ExpirationDate AS [Expiration Date],
                                    IL.IsActive AS [Is Active]
                                FROM InternationalLicenses IL
                                INNER JOIN Drivers D ON IL.DriverID = D.DriverID
                                WHERE IL.ApplicationID = @ApplicationID";
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
