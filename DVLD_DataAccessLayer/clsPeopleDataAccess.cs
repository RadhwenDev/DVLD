using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLD_DataAccessLayer
{
    public class clsPeopleDataAccess
    {
        public static DataTable GetPeople()
        {
            DataTable dt = new DataTable();
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"select FirstName, SecondName, ThirdName, LastName, NationalNo, DateOfBirth, Gendor,
                            Address, Email, Phone, CountryName, ImagePath
                            from People P inner join Countries C on P.NationalityCountryID = C.CountryID;";
            SqlCommand command = new SqlCommand(query, connection);
            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.HasRows)
                {
                    dt.Load(reader);
                }
                reader.Close();
            }
            catch (Exception) { }
            finally { connection.Close(); }
            return dt;
        }
    }
}
