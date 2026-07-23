using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLD_DataAccessLayer
{
    public class clsLoginLogData
    {
        public static string ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        public static int AddLoginLog(int UserID)
        {
            int LogID = -1;

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string query = @"INSERT INTO LoginLogs (UserID) VALUES (@UserID);
                            SELECT SCOPE_IDENTITY()";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserID", UserID);
                    try
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();

                        if (result != null && int.TryParse(result.ToString(), out int insertedID))
                        {
                            LogID = insertedID;
                        }
                    }
                    catch (Exception ex)
                    {
                        // 💡 لكي تعرف سبب المشكلة الحقيقي الآن:
                        // ضع Breakpoint هنا واقرأ محتوى الـ ex.Message لتعرف الحقل المسبب للأزمة!
                        System.Diagnostics.Debug.WriteLine("SQL Error: " + ex.Message);
                        LogID = -1;
                    }
                }
            }
            return LogID;
        }
        
    }
}
