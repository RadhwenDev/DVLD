using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLD_DataAccessLayer
{
    public class clsApplicationsDataAccess
    {
        public static DataTable getAllApplicants()
        {
            DataTable dt = new DataTable();
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
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

        public static DataTable getAllApplicationTypes()
        {
            DataTable dt = new DataTable();
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"SELECT ApplicationTypeID, ApplicationTypeTitle FROM ApplicationTypes;";
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

        public static DataTable getAllApplicationStatus()
        {
            DataTable dt = new DataTable();
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"select distinct StatusID = A.ApplicationStatus, 
                                CASE
                                    WHEN A.ApplicationStatus = 1 THEN 'New'
                                    WHEN A.ApplicationStatus = 2 THEN 'Cancelled'
                                    WHEN ApplicationStatus = 3 THEN 'Completed'
                                ELSE 'Unknown'
                                END as [STATUS] 
                                from Applications A;";
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
