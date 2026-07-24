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
    public class clsTestAppointmentDataAccess
    {
        public static string ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        public static DataTable visionTest(int ApplicationID)
        {
            DataTable dt = new DataTable();
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string query = @"SELECT 
                                    (P.FirstName + ' ' + P.SecondName + ' ' + P.ThirdName + ' ' + P.LastName) AS FullName, 
                                    P.PersonID, 
                                    P.ImagePath,
                                    CASE
                                        WHEN A.ApplicationStatus = 1 THEN 'New'
                                        WHEN A.ApplicationStatus = 2 THEN 'Cancelled'
                                        ELSE 'Completed'
                                    END AS [STATUS],
                                    A.LastStatusDate, 
                                    A.ApplicationDate, 
                                    U.Username AS CreatedByUserName, 
                                    A.PaidFees AS ApplicationPaidFees,
                                    LDLA.LocalDrivingLicenseApplicationID,
                                    LC.ClassName,
                                    TA.TestTypeID
                                FROM People P 
                                INNER JOIN Users U ON P.PersonID = U.PersonID
                                INNER JOIN Applications A ON P.PersonID = A.ApplicantPersonID
                                INNER JOIN LocalDrivingLicenseApplications LDLA ON A.ApplicationID = LDLA.ApplicationID
                                INNER JOIN LicenseClasses LC ON LDLA.LicenseClassID = LC.LicenseClassID
                                LEFT JOIN TestAppointments TA ON LDLA.LocalDrivingLicenseApplicationID = TA.LocalDrivingLicenseApplicationID
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
        public static DataTable visionTestDataGridView(int ApplicationID)
        {
            DataTable dt = new DataTable();
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string query = @"SELECT 
                                    TA.TestAppointmentID, 
                                    TA.AppointmentDate, 
                                    TA.PaidFees AS AppointmentPaidFees,
                                    CASE
                                        WHEN TA.IsLocked = 1 THEN 'Active'
                                        ELSE 'Inactive'
                                    END AS isLocked
                                FROM Applications A
                                INNER JOIN LocalDrivingLicenseApplications LDLA ON A.ApplicationID = LDLA.ApplicationID
                                INNER JOIN LicenseClasses LC ON LDLA.LicenseClassID = LC.LicenseClassID
                                LEFT JOIN TestAppointments TA ON LDLA.LocalDrivingLicenseApplicationID = TA.LocalDrivingLicenseApplicationID
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

        public static DataTable getTsetAppointment(int ApplicationID)
        {
            DataTable dt = new DataTable();
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string query = @"SELECT 
                                    TA.TestTypeID
                                FROM Applications A
                                INNER JOIN LocalDrivingLicenseApplications LDLA ON A.ApplicationID = LDLA.ApplicationID
                                INNER JOIN LicenseClasses LC ON LDLA.LicenseClassID = LC.LicenseClassID
                                LEFT JOIN TestAppointments TA ON LDLA.LocalDrivingLicenseApplicationID = TA.LocalDrivingLicenseApplicationID
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


    }
}
