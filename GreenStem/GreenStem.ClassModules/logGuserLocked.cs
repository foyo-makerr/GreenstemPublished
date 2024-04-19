using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GreenStem.ClassModules
{
    public static class LogGuserLocked
    {
       
        public static void InsertGUserLocked(string transactionType, string formName, string documentNo)
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connection_string"].ConnectionString; // Update with your actual connection string
             
            try
            {
                // Get the current computer name
                string computerName = Environment.MachineName;

                // SQL query to insert into the GUser_Locked_tbl table
                string insertQuery = "INSERT INTO [GUser_Locked_tbl] ([Transaction Type], [Computer Name], [User Name], [Form Name], [Document No]) " +
                    "VALUES (@TransactionType, @ComputerName, @UserName, @FormName, @DocumentNo)";

                // Create and open a SqlConnection
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Create a SqlCommand with parameters
                    using (SqlCommand command = new SqlCommand(insertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@TransactionType", transactionType);
                        command.Parameters.AddWithValue("@ComputerName", computerName);
                        command.Parameters.AddWithValue("@FormName", formName);
                        command.Parameters.AddWithValue("@DocumentNo", documentNo);
                        command.Parameters.AddWithValue("@UserName", modPublicVariable.UserID);
                        // Execute the command
                        command.ExecuteNonQuery();
                    }
                }

                Console.WriteLine("Record inserted successfully.");
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex, nameof(globalKeyHandler), "ClassModules", "InsertGUserLocked", new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                Console.WriteLine("Error inserting record: " + ex.Message);
            }
        }  
        public static bool CheckGUserLocked(string formName, string documentNo)
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connection_string"].ConnectionString; // Update with your actual connection string
            try
            {
                string selectQuery = "SELECT COUNT(1), [User Name], [Computer Name] FROM [GUser_Locked_tbl] WHERE [Form Name] = @FormName AND [Document No] = @DocumentNo GROUP BY [User Name], [Computer Name]";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(selectQuery, connection))
                    {
                        command.Parameters.AddWithValue("@FormName", formName);
                        command.Parameters.AddWithValue("@DocumentNo", documentNo);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    string lockedUserName = reader["User Name"].ToString();
                                    string lockedComputerName = reader["Computer Name"].ToString();

                                    // Get the current user name and computer name
                                    string currentUserName = modPublicVariable.UserID;
                                    string currentComputerName = Environment.MachineName;

                                    // Check if the locked user and computer are different from the current user and computer
                                    if (!string.Equals(lockedUserName, currentUserName, StringComparison.OrdinalIgnoreCase) ||
                                        !string.Equals(lockedComputerName, currentComputerName, StringComparison.OrdinalIgnoreCase))
                                    {
                                        // Show error message
                                        MessageBox.Show($"User - {lockedUserName} on Computer - {lockedComputerName} is using this form ({formName}) with Document No {documentNo}. You cannot use it concurrently.", "Concurrent Usage Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        return true; // Form is locked
                                    }
                                }
                            }
                        }
                    }
                }
                return false; // If no rows found or same user and computer, form is not locked
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex, nameof(globalKeyHandler), "ClassModules", "CheckGUserLocked", new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                Console.WriteLine("Error checking locked status: " + ex.Message);
                return false; // Assume form is not locked on error
            }
        }
        public static string GetLockedUserInfo(string formName, string documentNo)
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connection_string"].ConnectionString; // Update with your actual connection string
            string selectQuery = "SELECT [User Name], [Computer Name] FROM [GUser_Locked_tbl] WHERE [Form Name] = @FormName AND [Document No] = @DocumentNo";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(selectQuery, connection))
                    {
                        command.Parameters.AddWithValue("@FormName", formName);
                        command.Parameters.AddWithValue("@DocumentNo", documentNo);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string userName = reader["User Name"].ToString();
                                string computerName = reader["Computer Name"].ToString();
                                return $"{userName} with {computerName}";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex, nameof(globalKeyHandler), "ClassModules", "GetLockedUserInfo", new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                Console.WriteLine("Error retrieving locked user info: " + ex.Message);
            }

            return "Unknown User";
        }
        public static void DeleteGUserLocked(string transactionType, string formName, string documentNo)
        {
            if (string.IsNullOrEmpty(documentNo))
            {
                return;
            }
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connection_string"].ConnectionString; // Update with your actual connection string
            try
            {
                // SQL query to delete from the GUser_Locked_tbl table
                string deleteQuery = "DELETE FROM [GUser_Locked_tbl] WHERE [Transaction Type] = @TransactionType " +
                    "AND [Form Name] = @FormName AND [Document No] = @DocumentNo AND [User Name] = @UserName";

                // Create and open a SqlConnection
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Create a SqlCommand with parameters
                    using (SqlCommand command = new SqlCommand(deleteQuery, connection))
                    {
                        command.Parameters.AddWithValue("@TransactionType", transactionType);
                        command.Parameters.AddWithValue("@FormName", formName);
                        command.Parameters.AddWithValue("@DocumentNo", documentNo);
                        command.Parameters.AddWithValue("@UserName", modPublicVariable.UserID);
                        // Execute the command
                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            Console.WriteLine("Record deleted successfully.");
                        }
                        else
                        {
                            Console.WriteLine("No matching records found to delete.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex, nameof(globalKeyHandler), "ClassModules", "DeleteGUserLocked", new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                Console.WriteLine("Error deleting record: " + ex.Message);
            }
        }
        public static bool DeleteLogsForComputer(string computerName)
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connection_string"].ConnectionString; // Update with your actual connection string
            try
            {
                string deleteQuery = "DELETE FROM [GUser_Locked_tbl] WHERE LOWER([Computer Name]) = LOWER(@ComputerName)";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(deleteQuery, connection))
                    {
                        command.Parameters.AddWithValue("@ComputerName", computerName);
                        int rowsAffected = command.ExecuteNonQuery();
                        Console.WriteLine($"{rowsAffected} rows deleted for computer: {computerName}");
                        return rowsAffected > 0; // Return true if rows were deleted
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error deleting logs for computer: " + ex.Message);
                ExceptionLogger.LogException(ex, nameof(globalKeyHandler), "ClassModules", "DeleteLogsForComputer", new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                return false; // Return false if an exception occurred
            }
        }

        public static bool DeleteLogsForUser(string userName)
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connection_string"].ConnectionString; // Update with your actual connection string
            try
            {
                string deleteQuery = "DELETE FROM [GUser_Locked_tbl] WHERE LOWER([User Name]) = LOWER(@UserName)";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(deleteQuery, connection))
                    {
                        command.Parameters.AddWithValue("@UserName", userName);
                        int rowsAffected = command.ExecuteNonQuery();
                        Console.WriteLine($"{rowsAffected} rows deleted for user: {userName}");
                        return rowsAffected > 0; // Return true if rows were deleted
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error deleting logs for user: " + ex.Message);
                ExceptionLogger.LogException(ex, nameof(globalKeyHandler), "ClassModules", "DeleteLogsForUser", new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                return false; // Return false if an exception occurred
            }
        }
        public static void DeleteAllLogs()
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connection_string"].ConnectionString; // Update with your actual connection string
            try
            {
                List<LogData> logsToDelete = new List<LogData>(); // Create a list to store log data

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Select specific columns from the table
                    string selectQuery = "SELECT [Document No], [Form Name], [User Name] FROM [GUser_Locked_tbl]";
                    using (SqlCommand selectCommand = new SqlCommand(selectQuery, connection))
                    {
                        using (SqlDataReader reader = selectCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // Read data from the reader and create LogData objects
                                LogData log = new LogData
                                {
                                    DocumentNo = reader["Document No"].ToString(),
                                    FormName = reader["Form Name"].ToString(),
                                    UserName = reader["User Name"].ToString()
                                };
                                logsToDelete.Add(log); // Add the log data to the list
                            }
                        }
                    }

                    // Now logsToDelete list contains the data to be deleted
                    // Proceed with the deletion
                    string deleteQuery = "DELETE FROM [GUser_Locked_tbl]";
                    using (SqlCommand deleteCommand = new SqlCommand(deleteQuery, connection))
                    {
                        int rowsAffected = deleteCommand.ExecuteNonQuery();
                        Console.WriteLine($"{rowsAffected} rows deleted from the log table");

                        // NotifyLogsDeleted with the list of logsToDelete
                        LogManager.NotifyLogsDeleted(logsToDelete);
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex, nameof(globalKeyHandler), "ClassModules", "DeleteAllLogs", new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                Console.WriteLine("Error deleting all logs: " + ex.Message);
            }
        }
        public static void NotifyLogsDeleted()
        {
            // Use a named event for signaling between processes
            const string eventName = "LogsDeletedEvent";

            // Create the event (or open if it already exists)
            EventWaitHandle logsDeletedEvent = new EventWaitHandle(false, EventResetMode.AutoReset, eventName);

            // Signal the event to notify other instances
            logsDeletedEvent.Set();
            logsDeletedEvent.Dispose(); // Dispose after signaling
        }
        public class LogData
        {
            public string DocumentNo { get; set; }
            public string FormName { get; set; }
            public string UserName { get; set; }
        }
    }



}
