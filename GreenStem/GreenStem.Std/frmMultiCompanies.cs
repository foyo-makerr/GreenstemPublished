
using DevExpress.XtraEditors;
using GreenStem;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GreenStem.ClassModules;
using System.IO;
using DevExpress.XtraBars.Alerter;
using System.Drawing.Text;
using System.Configuration;
namespace GreenStem.Std

{
    public partial class frmMultiCompanies : Form
    {
        
        private const int AnimationDuration = 20; // Duration of animation in milliseconds
        private const int TimerInterval = 1; // Decrease the TimerInterval for a quicker animation
        private Timer animationTimer;
        private double opacityIncrement;
        public frmMultiCompanies()
        {
            LoadServerName(); // Load the server name dynamically
            InitializeComponent();
            EncryptConnectionString();

            if (!string.IsNullOrEmpty(clsConnection.TempSQLServerName))
            {
                // Get the original connection string from web.config
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                ConnectionStringsSection connectionStringsSection = config.ConnectionStrings;
                string originalConnectionString = connectionStringsSection.ConnectionStrings["connection_string"].ConnectionString;

                // Extract the existing server name
                string serverName = GetDataSource(originalConnectionString);

                // Construct the modified connection string with the new server name
                string modifiedConnectionString = originalConnectionString.Replace($"Data Source={serverName}", $"Data Source={clsConnection.TempSQLServerName}");

                // Extract the existing value of Initial Catalog
                string initialCatalog = GetInitialCatalog(modifiedConnectionString);

                // Replace Initial Catalog with "GreenPlus"
                modifiedConnectionString = modifiedConnectionString.Replace($"Initial Catalog={initialCatalog}", "Initial Catalog=GreenPlus");

                // Update the connection string in the configuration object
                connectionStringsSection.ConnectionStrings["connection_string"].ConnectionString = modifiedConnectionString;

                // Save the changes to the configuration file
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("connectionStrings");

             

                // Load data into the grid using the modified connection string
                LoadDataIntoGrid();
            }

            // Load data into the grid when the form is initialized
            gridView1.OptionsBehavior.Editable = false;
            gridView1.KeyDown += GridView1_KeyDown;
            this.Load += FrmMultiCompanies_Load;
            InitializeAnimation();
            this.Load += frm_Load;
        }

        // Method to extract the existing server name from the connection string
        private string GetDataSource(string connectionString)
        {
            int startIndex = connectionString.IndexOf("Data Source=") + "Data Source=".Length;
            int endIndex = connectionString.IndexOf(";", startIndex);
            return connectionString.Substring(startIndex, endIndex - startIndex);
        }
        private void EncryptConnectionString()
        {
            try
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                ConnectionStringsSection section = config.ConnectionStrings;

                if (section != null && !section.SectionInformation.IsProtected)
                {
                    // Encrypt the connectionStrings section only if it's not already protected
                    section.SectionInformation.ProtectSection("DataProtectionConfigurationProvider");
                    config.Save();
                   
                 

                    // Refresh the ConfigurationManager to reflect the changes
                    ConfigurationManager.RefreshSection("connectionStrings");

                    Console.WriteLine("Connection string encrypted successfully.");
                }
                else
                {
                    Console.WriteLine("Connection string section is already encrypted or protected.");
                }
            }
            catch (Exception ex)
            {
                // Handle the exception in a meaningful way, such as logging or displaying an error message.
                Console.WriteLine("Error encrypting connection string: " + ex.Message);
            }
        }
        // Method to extract the existing value of Initial Catalog from the connection string
        private string GetInitialCatalog(string connectionString)
        {
            int startIndex = connectionString.IndexOf("Initial Catalog=") + "Initial Catalog=".Length;
            int endIndex = connectionString.IndexOf(";", startIndex);
            return connectionString.Substring(startIndex, endIndex - startIndex);
        }

        
        private void InitializeAnimation()
        {
            // Set initial opacity to 0
            this.Opacity = 0;

            // Calculate the opacity increment per tick
            opacityIncrement = 1.0 / (AnimationDuration / TimerInterval);

            // Initialize the timer
            animationTimer = new Timer();
            animationTimer.Interval = TimerInterval;
            animationTimer.Tick += AnimationTimer_Tick;
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            // Increase the opacity gradually
            this.Opacity += opacityIncrement;

            // If the opacity reaches 1, stop the timer
            if (this.Opacity >= 1.0)
            {
                animationTimer.Stop();
            }
        }

        private void frm_Load(object sender, EventArgs e)
        {
            // Start the animation timer when the form is loaded
            animationTimer.Start();
        }
        private void FrmMultiCompanies_Load(object sender, EventArgs e)
        {
            // Set the initial focus to gridControl1
            gridControl1.Focus();
        }
        private void LoadServerName()
        {
            string serverNameFilePath = Path.Combine(Application.StartupPath, "ServerName.txt");
            if (File.Exists(serverNameFilePath))
            {
                try
                {
                    // Read the server name from the text file
                    string serverName = File.ReadAllText(serverNameFilePath).Trim();
                    if (!string.IsNullOrEmpty(serverName))
                    {
                        // Set the connection string using the server name
                        clsConnection.TempSQLServerName = serverName;
                    }
                }
                catch (Exception ex)
                {
                   Console.WriteLine(ex.ToString());
                   ExceptionLogger.LogException(ex, nameof(frmMultiCompanies), "Std", "LoadServerName", new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                }
            }
           
        }
        private void LoadDataIntoGrid()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["connection_string"].ConnectionString))
                {
                    connection.Open();
                    string sqlQuery = "SELECT [Company], [Company Name] FROM [MultiCompany]";
                    SqlCommand command = new SqlCommand(sqlQuery, connection);
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    // Bind the DataTable to gridControl1
                    gridControl1.DataSource = dataTable;
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions
                Console.WriteLine("Error: " + ex.Message);
                ExceptionLogger.LogException(ex, nameof(frmMultiCompanies), "Std", "LoadDataIntoGrid", new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
            }
        }

        private void GridView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                int selectedRowHandle = gridView1.FocusedRowHandle;
                if (selectedRowHandle >= 0)
                {
                    string selectedCompany = gridView1.GetRowCellValue(selectedRowHandle, "Company").ToString();
                    string selectedCompanyName = gridView1.GetRowCellValue(selectedRowHandle, "Company Name").ToString();

                    bool isConcurrentUsingValid = LicenseControl.CheckConcurrentUsing(selectedCompany, selectedCompanyName);
                    if (isConcurrentUsingValid)
                    {
                       
                        // Proceed with login actions
                        QueryDatabaseForServerAndName(selectedCompany, selectedCompanyName);
                        
                    }
                    else
                    {
                        // Fetch license control information from the database
                        string licenseControlInfo = LicenseControl.GetLicenseControlInfo(selectedCompany, selectedCompanyName);

                        // Display message box with license control information
                        MessageBox.Show($"Your license only allows {licenseControlInfo}. Please log out a user before logging in.");
                    }
                }
            }
        }


        private void QueryDatabaseForServerAndName(string selectedCompany, string selectedCompanyName)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["connection_string"].ConnectionString))
                {
                    connection.Open();
                    string sqlQuery = $"SELECT [Database Server], [Database Name] FROM [MultiCompany] " +
                                      $"WHERE [Company] = @Company AND [Company Name] = @CompanyName";
                    SqlCommand command = new SqlCommand(sqlQuery, connection);
                    command.Parameters.AddWithValue("@Company", selectedCompany);
                    command.Parameters.AddWithValue("@CompanyName", selectedCompanyName);
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        string dbServer = reader["Database Server"].ToString();
                        string dbName = reader["Database Name"].ToString();

                        // Get the initialized server name and database name from the current web.config file
                        string initializedConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connection_string"].ConnectionString;
                        string initializedServerName = GetDataSource(initializedConnectionString);
                        string initializedDatabaseName = GetInitialCatalog(initializedConnectionString);

                        // Modify the connection string with the retrieved server name and database name
                        string modifiedConnectionString = initializedConnectionString.Replace($"Data Source={initializedServerName}", $"Data Source={dbServer}")
                                                                             .Replace($"Initial Catalog={initializedDatabaseName}", $"Initial Catalog={dbName}");

                        // Update the connection string in the configuration object
                        Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                        ConnectionStringsSection connectionStringsSection = config.ConnectionStrings;
                        connectionStringsSection.ConnectionStrings["connection_string"].ConnectionString = modifiedConnectionString;
                        config.Save(ConfigurationSaveMode.Modified);
                        ConfigurationManager.RefreshSection("connectionStrings");


                      

                        // Other operations after updating the connection
                        modPublicVariable.CompanyName = selectedCompanyName;
                        modPublicVariable.Company = selectedCompany;
                        this.Hide();
                        frmLogin frmlogin = new frmLogin();
                        frmlogin.StartPosition = FormStartPosition.CenterScreen;
                        frmlogin.GotFocus += (sender, e) => { this.Focus(); }; // Redirect focus to MainMenu
                        frmlogin.ShowDialog();
                        this.Close();
                        this.Dispose();
                    }
                    else
                    {
                        MessageBox.Show("Database record not found for the selected company.");
                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions
                Console.WriteLine("Error: " + ex.Message);
                ExceptionLogger.LogException(ex, nameof(frmMultiCompanies), "Std", "QueryDatabaseForServerAndName", new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
            }
        }


        private void simpleButton1_Click(object sender, EventArgs e)
        {

        }
        private void SettingsForm_DataSavedHandler()
        {
            
            // Refresh data when data is saved in settings form
            if (!string.IsNullOrEmpty(clsConnection.TempSQLServerName))
            {
               

                // Get the original connection string from web.config
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                ConnectionStringsSection connectionStringsSection = config.ConnectionStrings;
                string originalConnectionString = connectionStringsSection.ConnectionStrings["connection_string"].ConnectionString;

                // Extract the existing server name
                string serverName = GetDataSource(originalConnectionString);

                // Construct the modified connection string with the new server name
                string modifiedConnectionString = originalConnectionString.Replace($"Data Source={serverName}", $"Data Source={clsConnection.TempSQLServerName}");

                // Extract the existing value of Initial Catalog
                string initialCatalog = GetInitialCatalog(modifiedConnectionString);

                // Replace Initial Catalog with "GreenPlus"
                modifiedConnectionString = modifiedConnectionString.Replace($"Initial Catalog={initialCatalog}", "Initial Catalog=GreenPlus");

                // Update the connection string in the configuration object
                connectionStringsSection.ConnectionStrings["connection_string"].ConnectionString = modifiedConnectionString;

                // Save the changes to the configuration file
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("connectionStrings");

                LoadDataIntoGrid();
            }
        }
        private void btnItemSetting_Click(object sender, EventArgs e)
        {
           
                frmCompanySettings frmCompanySettings = new frmCompanySettings();
                frmCompanySettings.StartPosition = FormStartPosition.CenterScreen;
                frmCompanySettings.DataSaved += SettingsForm_DataSavedHandler;
                frmCompanySettings.ShowDialog();    
            
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
            Environment.Exit(0);
            System.Diagnostics.Process.GetCurrentProcess().Kill();
            Application.Exit();
           
        }
    }


}