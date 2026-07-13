using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace DVLD_DataAccessLayer
{
    public class clsUsersDataAccess
    {
        public static DataTable getAllUsers()
        {
            DataTable dt = new DataTable();
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"SELECT P.FirstName, P.SecondName, P.ThirdName, P.LastName, U.UserID, U.UserName, U.Permissions, U.IsActive 
                                FROM Users U inner join People P ON U.PersonID = P.PersonID";
                using(SqlCommand command = new SqlCommand(query, connection))
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

        public static int AddNewUser(int PersonID, string UserName, string Password, bool isActive, int Permissions)
        {
            int UserID = -1;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                // 🌟 ملاحظة: تأكد إن كان الحقل في قاعدتك اسمه NationalityCountryID أو NationalCountryID وقمت بضبطه هنا
                string query = @"INSERT INTO Users (PersonID, UserName, Password, isActive, Permissions)
                         VALUES (@PersonID, @UserName, @Password, @isActive, @Permissions);
                         SELECT SCOPE_IDENTITY();";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@PersonID", PersonID);

                    command.Parameters.AddWithValue("@UserName", UserName);
                    command.Parameters.AddWithValue("@Password", Password);
                    command.Parameters.AddWithValue("@isActive", isActive);
                    command.Parameters.AddWithValue("@Permissions", Permissions);

                    try
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();

                        if (result != null && int.TryParse(result.ToString(), out int insertedID))
                        {
                            UserID = insertedID;
                        }
                    }
                    catch (Exception ex)
                    {
                        // 💡 لكي تعرف سبب المشكلة الحقيقي الآن:
                        // ضع Breakpoint هنا واقرأ محتوى الـ ex.Message لتعرف الحقل المسبب للأزمة!
                        System.Diagnostics.Debug.WriteLine("SQL Error: " + ex.Message);
                        UserID = -1;
                    }
                }
            }
            return UserID;
        }

        public static bool IsUserExistForPersonID(int PersonID)
        {
            bool isFound = false;
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT Found=1 FROM Users WHERE PersonID = @PersonID";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@PersonID", PersonID);
                    try
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();
                        if (result != null) isFound = true;
                    }
                    catch (Exception) { isFound = false; }
                }
            }
            return isFound;
        }

        public static bool IsUserNameExistForPersonID(string UserName)
        {
            bool isFound = false;
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT Found=1 FROM Users WHERE UserName = @UserName";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserName", UserName);
                    try
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();
                        if (result != null) isFound = true;
                    }
                    catch (Exception) { isFound = false; }
                }
            }
            return isFound;
        }

        

        

        public static bool GetUserInfoByID(int UserID, ref int PersonID, ref string UserName, ref string Password, ref int Permissions, ref bool isActive)
        {
            bool isFound = false;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "SELECT * FROM Users WHERE UserID = @UserID";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@UserID", UserID);

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    isFound = true;
                    PersonID = (int)reader["PersonID"];
                    UserName = (string)reader["UserName"];
                    Password = (string)reader["Password"];
                    Permissions = (int)reader["Permissions"];
                    isActive = (bool)reader["isActive"];
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

        public static bool Find(ref int UserID, ref int PersonID, string UserName, string Password, ref int Permissions, ref bool isActive)
        {
            bool isFound = false;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "SELECT * FROM Users WHERE UserName = @UserName and Password = @Password";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@UserName", UserName);
            command.Parameters.AddWithValue("@Password", Password);

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    isFound = true;
                    UserID = (int)reader["UserID"];
                    PersonID = (int)reader["PersonID"];
                    Permissions = (int)reader["Permissions"];
                    isActive = (bool)reader["isActive"];
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

        public static bool UpdateUser(int UserID, string UserName, string Password, bool isActive, int Permissions)
        {
            int rowAffected = 0;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"UPDATE Users SET UserName = @UserName, Password = @Password, isActive = @isActive, Permissions = @Permissions
                            WHERE UserID = @UserID";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@UserID", UserID);
            command.Parameters.AddWithValue("@UserName", UserName);
            command.Parameters.AddWithValue("@Password", Password);
            command.Parameters.AddWithValue("@isActive", isActive);
            command.Parameters.AddWithValue("@Permissions", Permissions);

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
