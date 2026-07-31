using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLD_DataAccessLayer
{
    public class clsLocalDrivingLicenseApplicationsDataAccess
    {
        public static string ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        public static int AddNewLocalDrivingLicenseApplications(int ApplicationID, int LicenseClassID)
        {
            int LocalDrivingLicenseApplications = -1;

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string query = @"INSERT INTO LocalDrivingLicenseApplications (ApplicationID, LicenseClassID)
                                 VALUES (@ApplicationID, @LicenseClassID);
                                 SELECT SCOPE_IDENTITY();";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ApplicationID", ApplicationID);
                    command.Parameters.AddWithValue("@LicenseClassID", LicenseClassID);

                    try
                    {
                        connection.Open();

                        object result = command.ExecuteScalar();

                        if (result != null && int.TryParse(result.ToString(), out int insertedID))
                        {
                            LocalDrivingLicenseApplications = insertedID;
                        }
                    }
                    catch (Exception)
                    {
                        LocalDrivingLicenseApplications = -1;
                    }
                }
            }

            return LocalDrivingLicenseApplications;
        }
        public static bool GetLocalDrivingLicenseApplicationInfoByID(
    int LocalDrivingLicenseApplicationID,
    ref int ApplicationID,
    ref int ApplicantPersonID,
    ref int LicenseClassID)
        {
            bool isFound = false;

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                // نربط مع جدول Applications لجلب ApplicantPersonID في نفس الاستعلام
                string query = @"SELECT LDL.LocalDrivingLicenseApplicationID, 
                                LDL.ApplicationID, 
                                LDL.LicenseClassID, 
                                A.ApplicantPersonID
                         FROM LocalDrivingLicenseApplications LDL
                         INNER JOIN Applications A ON LDL.ApplicationID = A.ApplicationID
                         WHERE LDL.LocalDrivingLicenseApplicationID = @LDLAppID;";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@LDLAppID", LocalDrivingLicenseApplicationID);

                    try
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                isFound = true;
                                ApplicationID = (int)reader["ApplicationID"];
                                LicenseClassID = (int)reader["LicenseClassID"];
                                ApplicantPersonID = (int)reader["ApplicantPersonID"];
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
    }
}
