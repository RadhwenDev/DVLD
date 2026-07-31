using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace DVLD_DataAccessLayer
{
    public class clsApplicationsDataAccess
    {
        public static string ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        public static DataTable getAllApplicants()
        {
            DataTable dt = new DataTable();
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string query = @"SELECT [  ID] = A.ApplicationID, APPLICANT = (P.FirstName + ' ' + P.SecondName + ' ' + P.ThirdName + ' ' + P.LastName), [SERVICE TYPE] = Aty.ApplicationTypeTitle, [DATE] = A.LastStatusDate, [FEES PAID] = A.PaidFees,
                                 CASE
	                                 WHEN A.ApplicationStatus = 1 THEN 'New'
                                     WHEN A.ApplicationStatus = 2 THEN 'Cancelled'
                                     WHEN ApplicationStatus = 3 THEN 'Completed'
                                     ELSE 'Unknown'
                                 END as [STATUS]
                                 FROM Applications A INNER JOIN People P on A.ApplicantPersonID = P.PersonID
                                 INNER JOIN ApplicationTypes ATy on A.ApplicationTypeID = Aty.ApplicationTypeID
                                 ;";
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

        

        public static DataTable getAllDetailsForShowButton(int ApplicationID)
        {
            DataTable dt = new DataTable();
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string query = @"SELECT A.ApplicationID, A.ApplicationDate, A.LastStatusDate, A.PaidFees, (P.FirstName + ' ' + P.LastName) AS FullName,
                                    P.NationalNo, P.DateOfBirth, P.Phone, P.Address, P.ImagePath, AT.ApplicationTypeTitle, LC.ClassName, LC.MinimumAllowedAge, LC.DefaultValidityLength, LC.ClassFees, LC.ClassDescription,
                                    CASE A.ApplicationStatus
                                        WHEN 1 THEN 'New'
                                        WHEN 2 THEN 'Cancelled'
                                        WHEN 3 THEN 'Completed'
                                        ELSE 'Unknown'
                                    END AS StatusName,
                                    CASE P.Gendor
                                        WHEN 0 THEN 'Male'
                                        WHEN 1 THEN 'Female'
                                        ELSE 'Unknown'
                                    END AS GenderName
                                FROM Applications A
                                INNER JOIN People P ON A.ApplicantPersonID = P.PersonID
                                INNER JOIN ApplicationTypes AT ON A.ApplicationTypeID = AT.ApplicationTypeID
                                LEFT JOIN LocalDrivingLicenseApplications LDLA ON A.ApplicationID = LDLA.ApplicationID
                                LEFT JOIN LicenseClasses LC ON LDLA.LicenseClassID = LC.LicenseClassID
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

        public static DataTable getAllApplicationType()
        {
            DataTable dt = new DataTable();
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string query = @"SELECT ApplicationTypeID, ApplicationTypeTitle FROM ApplicationTypes";
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

        public static DataTable getAllApplicationTypes(int ApplicantPersonID)
        {
            DataTable dt = new DataTable();
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string query = @"SELECT A_T.ApplicationTypeID, A_T.ApplicationTypeTitle
                                FROM ApplicationTypes A_T
                                WHERE NOT EXISTS (
                                    SELECT 1 
                                    FROM Applications A
                                    INNER JOIN LocalDrivingLicenseApplications LDLA ON A.ApplicationID = LDLA.ApplicationID
                                    WHERE A.ApplicationTypeID = A_T.ApplicationTypeID
                                      AND A.ApplicantPersonID = 1024
                                      AND A.ApplicationStatus = 1
                                      AND (
                                          (A_T.ApplicationTypeID NOT IN (1, 7))
                                          OR 
                                          (A_T.ApplicationTypeID IN (1, 7) AND LDLA.LicenseClassID = @ApplicantPersonID)
                                      )
                                ); ";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ApplicantPersonID", ApplicantPersonID);
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

        public static DataTable getApplicationTypesTitle_Fees(int ApplicationTypeID)
        {
            DataTable dt = new DataTable();
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string query = @"SELECT ApplicationTypeTitle, ApplicationFees FROM ApplicationTypes WHERE ApplicationTypeID = @ApplicationTypeID";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ApplicationTypeID", ApplicationTypeID);
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

        public static DataTable getAllApplicationStatus()
        {
            DataTable dt = new DataTable();
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string query = @"SELECT 1 AS StatusID, 'New' AS [Status]
                                 UNION ALL
                                 SELECT 2, 'Cancelled'
                                 UNION ALL
                                 SELECT 3, 'Completed';";
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
        

        public static int AddNewApplication(int ApplicantPersonID, DateTime ApplicationDate, int ApplicationTypeID, byte ApplicationStatus, DateTime LastStatusDate, decimal PaidFees, int CreatedByUserID)
        {
            int ApplicationID = -1;

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                // 🌟 ملاحظة: تأكد إن كان الحقل في قاعدتك اسمه NationalityCountryID أو NationalCountryID وقمت بضبطه هنا
                string query = @"INSERT INTO Applications (ApplicantPersonID, ApplicationDate, ApplicationTypeID, ApplicationStatus, LastStatusDate, PaidFees, CreatedByUserID)
                                VALUES (@ApplicantPersonID, @ApplicationDate, @ApplicationTypeID, @ApplicationStatus, @LastStatusDate, @PaidFees, @CreatedByUserID);
                                SELECT SCOPE_IDENTITY();";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ApplicantPersonID", ApplicantPersonID);
                    command.Parameters.AddWithValue("@ApplicationDate", ApplicationDate);
                    command.Parameters.AddWithValue("@ApplicationTypeID", ApplicationTypeID);
                    command.Parameters.AddWithValue("@ApplicationStatus", ApplicationStatus);
                    command.Parameters.AddWithValue("@LastStatusDate", LastStatusDate);
                    command.Parameters.AddWithValue("@PaidFees", PaidFees);
                    command.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);

                    try
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();

                        if (result != null && int.TryParse(result.ToString(), out int insertedID))
                        {
                            ApplicationID = insertedID;
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine("SQL Error: " + ex.Message);
                        ApplicationID = -1;
                    }
                }
            }

            return ApplicationID;
        }

        public static bool UpdateToCaancelStatus(int ApplicationID)
        {
            int rowAffected = 0;
            SqlConnection connection = new SqlConnection(ConnectionString);
            string query = @"UPDATE Applications SET ApplicationStatus = 2  WHERE ApplicationID = @ApplicationID";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@ApplicationID", ApplicationID);


            try
            {
                connection.Open();
                rowAffected = command.ExecuteNonQuery();
            }
            catch (Exception) { return false; }
            finally
            {
                connection.Close();
            }

            return rowAffected != 0;
        }
    }
}
