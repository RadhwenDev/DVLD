using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLD_DataAccessLayer
{
    public class clsDashboardDataAccess
    {
        public static int getPendingApplicants()
        {
            int totalPending = 0;
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"select count(*) from Applications where ApplicationStatus = 1";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    try
                    {
                        connection.Open();

                        object result = command.ExecuteScalar();
                        if (result != null && int.TryParse(result.ToString(), out int insertedValue))
                        {
                            totalPending = insertedValue;
                        }
                    }
                    catch (Exception) { }
                }
            }

            return totalPending;
        }

        public static int GetCompletedApplicationsThisMonth()
        {
            int completedCount = 0;
            string query = @"SELECT COUNT(*) 
                             FROM Applications 
                             WHERE ApplicationStatus = 3 
                               AND MONTH(LastStatusDate) = MONTH(GETDATE()) 
                               AND YEAR(LastStatusDate) = YEAR(GETDATE())";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    try
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();

                        if (result != null && int.TryParse(result.ToString(), out int value))
                        {
                            completedCount = value;
                        }
                    }
                    catch (Exception) { }
                }
            }

            return completedCount;
        }

        public static int GetTotalPeople()
        {
            int totalPeople = 0;
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"select count(*) from People";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    try
                    {
                        connection.Open();

                        object result = command.ExecuteScalar();
                        if (result != null && int.TryParse(result.ToString(), out int insertedValue))
                        {
                            totalPeople = insertedValue;
                        }
                    }
                    catch (Exception) { }
                }
            }

            return totalPeople;
        }

        public static int GetTotalPeopleInThisMonth()
        {
            int totalPeople = 0;
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"SELECT COUNT(*) FROM People where MONTH(LoginTime) = MONTH(GETDATE()) AND YEAR(LoginTime) = YEAR(GETDATE())";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    try
                    {
                        connection.Open();

                        object result = command.ExecuteScalar();
                        if (result != null && int.TryParse(result.ToString(), out int insertedValue))
                        {
                            totalPeople = insertedValue;
                        }
                    }
                    catch (Exception) { }
                }
            }

            return totalPeople;
        }

        public static DataTable GetApplicationPeopleInfo()
        {
            DataTable dt = new DataTable();
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"SELECT TOP 5
                                    A.ApplicationID,
                                    P.FirstName + ' ' + P.LastName AS FullName, P.ImagePath,
                                    ApplicationTypes.ApplicationTypeTitle,
                                    A.ApplicationDate,
                                    CASE A.ApplicationStatus
                                        WHEN 1 THEN 'New'
                                        WHEN 2 THEN 'Cancelled'
                                        WHEN 3 THEN 'Completed'
                                        ELSE 'Unknown'
                                    END AS StatusName
                                FROM Applications A
                                INNER JOIN People P ON A.ApplicantPersonID = P.PersonID
                                INNER JOIN ApplicationTypes ON A.ApplicationTypeID = ApplicationTypes.ApplicationTypeID
                                order by ApplicationDate DESC;";
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
        public static DataTable GetLicensePeopleInfo()
        {
            DataTable dt = new DataTable();
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"SELECT TOP 2
                                    Licenses.LicenseID,
                                    People.FirstName + ' ' + People.LastName AS FullName,
                                    Licenses.ExpirationDate
                                FROM Licenses
                                INNER JOIN Applications ON Licenses.ApplicationID = Applications.ApplicationID
                                INNER JOIN People ON Applications.ApplicantPersonID = People.PersonID
                                ORDER BY Licenses.LicenseID DESC;";
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
        public static DataTable GetServiceBreakdown()
        {
            DataTable dt = new DataTable();
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"SELECT TOP 5
                                    ApplicationTypes.ApplicationTypeTitle AS ServiceName,
                                    COUNT(Applications.ApplicationID) AS TotalCount,
                                    SUM(COUNT(Applications.ApplicationID)) OVER() AS OverallTotal
                                FROM Applications
                                INNER JOIN ApplicationTypes ON Applications.ApplicationTypeID = ApplicationTypes.ApplicationTypeID
                                GROUP BY ApplicationTypes.ApplicationTypeTitle
                                ORDER BY TotalCount DESC;";
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
