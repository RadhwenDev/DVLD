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
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"select FirstName, SecondName, ThirdName, LastName, NationalNo, DateOfBirth, Gendor,
                         Address, Email, Phone, CountryName, ImagePath
                         from People P inner join Countries C on P.NationalityCountryID = C.CountryID;";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    try
                    {
                        connection.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                dt.Load(reader);
                            }
                        } 
                    }
                    catch (Exception){}
                } 
            }

            return dt;
        }
        public static int AddNewPerson(string NationalNo, string FirstName, string SecondName, string ThirdName, string LastName, DateTime DateOfBirth, byte Gendor, string Address, string Phone, string Email, int NationalCountryID, string ImagePath)
        {
            int PersonID = -1;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                // 🌟 ملاحظة: تأكد إن كان الحقل في قاعدتك اسمه NationalityCountryID أو NationalCountryID وقمت بضبطه هنا
                string query = @"INSERT INTO People (NationalNo, FirstName, SecondName, ThirdName, LastName, DateOfBirth, Gendor, Address, Phone, Email, NationalityCountryID, ImagePath)
                         VALUES (@NationalNo, @FirstName, @SecondName, @ThirdName, @LastName, @DateOfBirth, @Gendor, @Address, @Phone, @Email, @NationalCountryID, @ImagePath);
                         SELECT SCOPE_IDENTITY();";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@NationalNo", NationalNo);
                    command.Parameters.AddWithValue("@FirstName", FirstName);

                    // 🌟 الحل هنا: إرسال نص فارغ "" بدل DBNull لأن الجدول مصمم NOT NULL
                    command.Parameters.AddWithValue("@SecondName", string.IsNullOrWhiteSpace(SecondName) ? "" : SecondName.Trim());
                    command.Parameters.AddWithValue("@ThirdName", string.IsNullOrWhiteSpace(ThirdName) ? "" : ThirdName.Trim());

                    command.Parameters.AddWithValue("@LastName", LastName);
                    command.Parameters.AddWithValue("@DateOfBirth", DateOfBirth);
                    command.Parameters.AddWithValue("@Gendor", Gendor);
                    command.Parameters.AddWithValue("@Address", Address);
                    command.Parameters.AddWithValue("@Phone", Phone);

                    command.Parameters.AddWithValue("@Email", string.IsNullOrWhiteSpace(Email) ? "" : Email.Trim());
                    command.Parameters.AddWithValue("@NationalCountryID", NationalCountryID);

                    if (!string.IsNullOrEmpty(ImagePath))
                        command.Parameters.AddWithValue("@ImagePath", ImagePath);
                    else
                        command.Parameters.AddWithValue("@ImagePath", ""); // أو اتركها System.DBNull.Value إذا كان حقل الصورة يقبل NULL في الداتابيز

                    try
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();

                        if (result != null && int.TryParse(result.ToString(), out int insertedID))
                        {
                            PersonID = insertedID;
                        }
                    }
                    catch (Exception ex)
                    {
                        // 💡 لكي تعرف سبب المشكلة الحقيقي الآن:
                        // ضع Breakpoint هنا واقرأ محتوى الـ ex.Message لتعرف الحقل المسبب للأزمة!
                        System.Diagnostics.Debug.WriteLine("SQL Error: " + ex.Message);
                        PersonID = -1;
                    }
                }
            }

            return PersonID;
        }
        public static bool IsPersonExist(string NationalNo)
        {
            bool isFound = false;
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT Found=1 FROM People WHERE NationalNo = @NationalNo";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@NationalNo", NationalNo);
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
    }
}
