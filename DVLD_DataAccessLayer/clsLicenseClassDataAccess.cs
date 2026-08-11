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
    public class clsLicenseClassDataAccess
    {
        public static string ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        public static DataTable GetAllLicenseClasses()
        {
            DataTable dt = new DataTable();
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string query = @"SELECT * FROM LicenseClasses";
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

        public static DataTable GetAllLicenseClassesName()
        {
            DataTable dt = new DataTable();
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string query = @"SELECT LC.LicenseClassID, LC.ClassName FROM LicenseClasses LC";

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
                    catch (Exception ) { }
                }
            }
            return dt;
        }

        public static List<int> GetPersonLicenseClassIDs(int PersonID)
        {
            List<int> personClassIDs = new List<int>();
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string query = @"SELECT L.LicenseClass
                                 FROM Licenses L
                                 INNER JOIN Applications A ON L.ApplicationID = A.ApplicationID
                                 WHERE A.ApplicantPersonID = @PersonID AND L.IsActive = 1
                                 
                                 UNION
                                 
                                 SELECT LDL.LicenseClassID 
                                 FROM LocalDrivingLicenseApplications LDL
                                 INNER JOIN Applications A ON LDL.ApplicationID = A.ApplicationID
                                 WHERE A.ApplicantPersonID = @PersonID AND A.ApplicationStatus IN (1, 2)";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@PersonID", PersonID);
                    try
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                personClassIDs.Add(Convert.ToInt32(reader["LicenseClass"]));
                            }
                        }
                    }
                    catch (Exception) { }
                }
            }
            return personClassIDs;
        }

        public static bool GetLicenseClassInfoByPersonID(
    int PersonID,
    ref int LicenseClassID,
    ref string ClassName,
    ref string ClassDescription,
    ref byte MinimumAllowedAge,
    ref byte DefaultValidityLength,
    ref int ClassFees)
        {
            bool isFound = false;

            // جلب صنف آخر رخصة مسجلة للشخص
            string query = @"
        SELECT TOP 1 
            LC.LicenseClassID, 
            LC.ClassName, 
            LC.ClassDescription, 
            LC.MinimumAllowedAge, 
            LC.DefaultValidityLength, 
            LC.ClassFees
        FROM LicenseClasses LC
        INNER JOIN Licenses L ON LC.LicenseClassID = L.LicenseClass
        INNER JOIN Drivers D ON L.DriverID = D.DriverID
        WHERE D.PersonID = @PersonID
        ORDER BY L.LicenseID DESC;";

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@PersonID", PersonID);

                    try
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                isFound = true;

                                LicenseClassID = (int)reader["LicenseClassID"];
                                ClassName = (string)reader["ClassName"];
                                ClassDescription = reader["ClassDescription"] != DBNull.Value ? (string)reader["ClassDescription"] : "";
                                MinimumAllowedAge = Convert.ToByte(reader["MinimumAllowedAge"]);
                                DefaultValidityLength = Convert.ToByte(reader["DefaultValidityLength"]);
                                ClassFees = Convert.ToInt32(reader["ClassFees"]);
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

        public static DataTable GetLicenseClassesNameByID(int LicenseClassID)
        {
            DataTable dt = new DataTable();
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string query = @"SELECT LC.LicenseClassID, LC.ClassName 
                                 FROM LicenseClasses LC
                                 WHERE NOT EXISTS (
                                     SELECT 1 
                                     FROM Applications A
                                     INNER JOIN LocalDrivingLicenseApplications LDLA ON A.ApplicationID = LDLA.ApplicationID
                                     WHERE LDLA.LicenseClassID = LC.LicenseClassID
                                       AND A.ApplicantPersonID = @LicenseClassID
                                       AND A.ApplicationStatus = 1
                                 );";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@LicenseClassID", LicenseClassID);
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
        public static decimal GetClassFees(int LicenseClassID)
        {
            decimal classFees = 0;

            string query = @"SELECT ClassFees 
                    FROM LicenseClasses 
                    WHERE LicenseClassID = @LicenseClassID;";

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@LicenseClassID", LicenseClassID);

                    try
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();

                        if (result != null && decimal.TryParse(result.ToString(), out decimal fees))
                        {
                            classFees = fees;
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }

            return classFees;
        }
        public static bool UpdateLicenseClasses(int LicenseClassID, string ClassName, string ClassDescription, int MinimumAllowedAge, int DefaultValidityLength, int ClassFees)
        {
            int rowAffected = 0;
            SqlConnection connection = new SqlConnection(ConnectionString);
            string query = @"UPDATE LicenseClasses SET ClassName = @ClassName, ClassDescription = @ClassDescription, MinimumAllowedAge = @MinimumAllowedAge, DefaultValidityLength = @DefaultValidityLength, ClassFees = @ClassFees
                            WHERE LicenseClassID = @LicenseClassID";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@ClassName", ClassName);
            command.Parameters.AddWithValue("@ClassDescription", ClassDescription);
            command.Parameters.AddWithValue("@MinimumAllowedAge", MinimumAllowedAge);
            command.Parameters.AddWithValue("@DefaultValidityLength", DefaultValidityLength);
            command.Parameters.AddWithValue("@ClassFees", ClassFees);
            command.Parameters.AddWithValue("@LicenseClassID", LicenseClassID);
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


        public static bool Find(int LicenseClassID, ref string ClassName, ref string ClassDescription, ref int MinimumAllowedAge, ref int DefaultValidityLength, ref int ClassFees)
        {
            bool isFound = false;
            SqlConnection connection = new SqlConnection(ConnectionString);
            string query = "SELECT * FROM LicenseClasses WHERE LicenseClassID = @LicenseClassID";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@LicenseClassID", LicenseClassID);

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    isFound = true;
                    ClassName = (string)reader["ClassName"];
                    ClassDescription = (string)reader["ClassDescription"];
                    MinimumAllowedAge = Convert.ToInt32(reader["MinimumAllowedAge"]);
                    DefaultValidityLength = Convert.ToInt32(reader["DefaultValidityLength"]);
                    ClassFees = Convert.ToInt32(reader["ClassFees"]);
                }
                else
                {
                    isFound = false;
                }
                reader.Close();
            }
            catch (Exception)
            {
                isFound = false;
            }
            finally
            {
                connection.Close();
            }
            return isFound;
        }

        public static int AddNewLicenseClass(string ClassName, string ClassDescription, int MinimumAllowedAge, int DefaultValidityLength, int ClassFees)
        {
            int LicenseClassID = -1;

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                // استعلام الإضافة مع استرجاع الـ ID المولد تلقائياً
                string query = @"INSERT INTO LicenseClasses 
                         (ClassName, ClassDescription, MinimumAllowedAge, DefaultValidityLength, ClassFees)
                         VALUES 
                         (@ClassName, @ClassDescription, @MinimumAllowedAge, @DefaultValidityLength, @ClassFees);
                         SELECT SCOPE_IDENTITY();";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ClassName", ClassName);
                    command.Parameters.AddWithValue("@ClassDescription", ClassDescription);
                    command.Parameters.AddWithValue("@MinimumAllowedAge", MinimumAllowedAge);
                    command.Parameters.AddWithValue("@DefaultValidityLength", DefaultValidityLength);
                    command.Parameters.AddWithValue("@ClassFees", ClassFees);

                    try
                    {
                        connection.Open();

                        object result = command.ExecuteScalar();

                        if (result != null && int.TryParse(result.ToString(), out int insertedID))
                        {
                            LicenseClassID = insertedID;
                        }
                    }
                    catch (Exception)
                    {
                        LicenseClassID = -1;
                    }
                }
            }

            return LicenseClassID;
        }
        public static int GetDefaultValidityLength(int licenseClassID)
        {
            int defaultValidityLength = 10; 

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string query = "SELECT DefaultValidityLength FROM LicenseClasses WHERE LicenseClassID = @LicenseClassID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@LicenseClassID", licenseClassID);

                    try
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();

                        if (result != null && int.TryParse(result.ToString(), out int length))
                        {
                            defaultValidityLength = length;
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }

            return defaultValidityLength;
        }
    }
}
