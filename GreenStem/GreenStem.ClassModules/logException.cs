using System;
using System.IO;
using System.Windows.Forms;

namespace GreenStem.ClassModules
{
    public static class ExceptionLogger
    {
        public static void LogException(Exception ex, string fileName, string moduleName, string methodName, int lineNumber)
        {
            try
            {
                string logFileName = $"Bug-{DateTime.Now.ToString("dd-MM-yyyy")}.txt";
                string logFilePath = Path.Combine(Application.StartupPath, logFileName);

                using (StreamWriter sw = File.AppendText(logFilePath))
                {
                    // Write the header
                    sw.WriteLine($"File Name: {fileName}");
                    sw.WriteLine($"Module Name: {moduleName}");
                    sw.WriteLine($"Method Name: {methodName}");
                    sw.WriteLine($"Line Number: {lineNumber}");
                    sw.WriteLine($"Device Name: {Environment.MachineName}");
                    sw.WriteLine($"Timestamp: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                    sw.WriteLine($"Bug Description:");
                    sw.WriteLine("-------------------- || --------------------");

                    // Write the exception message
                    sw.WriteLine(ex.Message);
                    sw.WriteLine("-------------------- || --------------------");
                    // Add some space between entries
                    sw.WriteLine("\n");
                }
            }
            catch (Exception logEx)
            {
                // Handle any exceptions that occur while logging
                MessageBox.Show("An error occurred while logging the exception: " + logEx.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
