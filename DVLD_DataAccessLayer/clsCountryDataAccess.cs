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
    public class clsCountryDataAccess
    {
        public static string ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        public static DataTable GetAllCountries()
        {
            DataTable dt = new DataTable();

            // استخدام using يضمن إغلاق وتفريغ الـ Connection والـ Command من الذاكرة فوراً حتى لو حدث استثناء
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                // نطلب فقط الأعمدة التي نحتاجها بدقة للـ ComboBox والأعلام لزيادة سرعة النقل
                string query = "SELECT CountryID, CountryName FROM Countries ORDER BY CountryName ASC;";

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
                    catch (Exception) { }
                }
            } // يتم إغلاق الـ connection تلقائياً هنا وبشكل آمن جداً

            return dt;
        }
    }
}
