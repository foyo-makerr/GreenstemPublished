

using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace GreenStem.ClassModules
{
    public static class LicenseControl
    {
        private static string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connection_string"].ConnectionString;

        public static void IncreaseConcurrentUsing(string company, string companyName)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Increment the Concurrent Using value in the database
                    string query = "UPDATE [MultiCompany] SET [Concurrent Using] = [Concurrent Using] + 1 WHERE [Company] = @Company AND [Company Name] = @CompanyName";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Company", company);
                    command.Parameters.AddWithValue("@CompanyName", companyName);

                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error increasing Concurrent Using: " + ex.Message);
            }
        }

        public static void DecreaseConcurrentUsing(string company, string companyName)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Decrement the Concurrent Using value in the database
                    string query = "UPDATE [MultiCompany] SET [Concurrent Using] = [Concurrent Using] - 1 WHERE [Company] = @Company AND [Company Name] = @CompanyName";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Company", company);
                    command.Parameters.AddWithValue("@CompanyName", companyName);

                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error decreasing Concurrent Using: " + ex.Message);
            }
        }

        public static bool CheckConcurrentUsing(string company, string companyName)
        {
            bool result = false;
            try
            {
                // Establish connection to the database
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Query the database for [License Control] and [Concurrent Using]
                    string query = "SELECT [License Control], [Concurrent Using] FROM [MultiCompany] WHERE [Company] = @Company AND [Company Name] = @CompanyName";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Company", company);
                    command.Parameters.AddWithValue("@CompanyName", companyName);

                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        // Get the value of Concurrent Using
                        int concurrentUsing = Convert.ToInt32(reader["Concurrent Using"]);
                        int licenseControl = Convert.ToInt32(reader["License Control"]);
                       
                        // Check if Concurrent Using is less than or equal to a certain value
                        if (concurrentUsing < licenseControl)
                        {
                            result = true; // Return true if Concurrent Using is less than or equal to the desired value
                        }
                    }

                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error querying database: " + ex.Message);
            }

            return result;
        }

        public static string GetLicenseControlInfo(string company, string companyName)
        {
            string licenseControlInfo = "";
            try
            {
                // Establish connection to the database
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Query the database for [License Control]
                    string query = "SELECT [License Control] FROM [GreenPlus].[dbo].[MultiCompany] WHERE [Company] = @Company AND [Company Name] = @CompanyName";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Company", company);
                    command.Parameters.AddWithValue("@CompanyName", companyName);

                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        // Get the value of License Control
                        int licenseControl = Convert.ToInt32(reader["License Control"]);
                        licenseControlInfo = $"for {licenseControl} concurrent users"; // License control information
                    }

                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error querying database: " + ex.Message);
            }

            return licenseControlInfo;
        }
    }
}
