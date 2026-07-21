using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLD_DataAccessLayer
{
    public class clsLicensesDataAccess
    {
        public static DataTable getAllLicenses()
        {
            DataTable dt = new DataTable();
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
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
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
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
    }
}
