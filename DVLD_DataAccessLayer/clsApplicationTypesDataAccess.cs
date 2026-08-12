using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace DVLD_DataAccessLayer
{
    public class clsApplicationTypesDataAccess  
    {
        public static string ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        public static bool GetApplicationTypeInfoByID(int ID, ref string Title, ref decimal Fees)
        {
            bool isFound = false;

            SqlConnection connection = new SqlConnection(ConnectionString);
            string query = "SELECT * FROM ApplicationTypes WHERE ApplicationTypeID = @ApplicationTypeID";

            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@ApplicationTypeID", ID);

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    isFound = true;
                    Title = (string)reader["ApplicationTypeTitle"];
                    Fees = Convert.ToDecimal(reader["ApplicationFees"]);
                }

                reader.Close();
            }
            catch (Exception)
            {
                // EventLog output
                isFound = false;
            }
            finally
            {
                connection.Close();
            }

            return isFound;
        }

        public static DataTable GetAllApplicationTypes()
        {
            DataTable dt = new DataTable();

            SqlConnection connection = new SqlConnection(ConnectionString);
            string query = "SELECT * FROM ApplicationTypes ORDER BY ApplicationTypeID";

            SqlCommand command = new SqlCommand(query, connection);

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    dt.Load(reader);
                }

                reader.Close();
            }
            catch (Exception)
            {
                // EventLog output
            }
            finally
            {
                connection.Close();
            }

            return dt;
        }

        public static bool UpdateApplicationType(int ID, string Title, decimal Fees)
        {
            int rowsAffected = 0;

            SqlConnection connection = new SqlConnection(ConnectionString);
            string query = @"UPDATE ApplicationTypes
                             SET ApplicationTypeTitle = @Title,
                                 ApplicationFees = @Fees
                             WHERE ApplicationTypeID = @ApplicationTypeID";

            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@ApplicationTypeID", ID);
            command.Parameters.AddWithValue("@Title", Title);
            command.Parameters.AddWithValue("@Fees", Fees);

            try
            {
                connection.Open();
                rowsAffected = command.ExecuteNonQuery();
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                connection.Close();
            }

            return (rowsAffected > 0);
        }
    }
}