using DevExpress.Xpo.DB;
using DevExpress.XtraBars;
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
using System.Data.Sql;
using DevExpress.XtraSplashScreen;
using GreenStem.LookUp;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors;
using System.IO;
using System.Configuration;

namespace GreenStem.Std
{
    public partial class frmCompanySettings : DevExpress.XtraBars.Ribbon.RibbonForm
    {

        public delegate void DataSavedEventHandler();
        public event DataSavedEventHandler DataSaved;
        private string connectionString = "";
        private DataTable serverInstanceDt;
        frmListOfServers listOfServerForm;
        private DataTable databaseNames = new DataTable();
        public frmCompanySettings()
        {
            InitializeComponent();
            InitializeAsync(); // Call the asynchronous method without awaiting it
            if (!string.IsNullOrEmpty(clsConnection.TempSQLServerName))
            {
                txtServerName.Text = clsConnection.TempSQLServerName;

            }
            
        }
        private async void InitializeAsync()
        {
            if (!string.IsNullOrEmpty(clsConnection.TempSQLServerName))
            {
                connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connection_string"].ConnectionString;
                await LoadDataIntoGridAsync(); // Await the asynchronous method
               
            }
        }
        private async Task LoadDataIntoGridAsync()
        {
            try
            {
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(connectionString);
                builder.ConnectTimeout = 3; // Set a shorter connection timeout (1 second)

                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {
                    await connection.OpenAsync(); // Asynchronously open the connection
                    string sqlQuery = "SELECT * FROM [MultiCompany]";
                    SqlCommand command = new SqlCommand(sqlQuery, connection);
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    // Bind the DataTable to gridControl1
                    gridControl1.DataSource = dataTable;
                    //await SetupRepositoryItemsAsync(); // Call the asynchronous method
                    if (listOfServerForm != null)
                    {
                        listOfServerForm.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                txtServerName.Text = "";
                // Handle any exceptions
                Console.WriteLine("Error: " + ex.Message);
                MessageBox.Show("Please Try Other Server Instances " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private async Task SetupRepositoryItemsAsync()
        {

            // Create repository items for GridLookupEdits
            RepositoryItemGridLookUpEdit repositoryItemGridLookupDatabaseServer = new RepositoryItemGridLookUpEdit();

            // Initialize serverInstanceDt if it's null
            if (serverInstanceDt == null)
            {
                serverInstanceDt = new DataTable();
                serverInstanceDt.Columns.Add("ServerName", typeof(string));
            }

            // Fetch data from GreenPlus.dbo.MultiCompany
            DataTable multiCompanyData = GetDataFromDatabase("SELECT DISTINCT [Database Server] FROM [GreenPlus].[dbo].[MultiCompany]");


            // Populate serverInstanceDt with data from GreenPlus.dbo.MultiCompany
            if (multiCompanyData != null)
            {
                foreach (DataRow row in multiCompanyData.Rows)
                {
                    serverInstanceDt.Rows.Add(row["Database Server"].ToString());
                }
            }

            // Fetch additional data using SqlDataSourceEnumerator
            DataTable additionalData = await Task.Run(() =>
            {
                SqlDataSourceEnumerator instance = SqlDataSourceEnumerator.Instance;
                return instance.GetDataSources();
            });

            // Merge additionalData with serverInstanceDt
            if (additionalData != null)
            {
                foreach (DataRow row in additionalData.Rows)
                {
                    string serverName = row["ServerName"].ToString();
                    if (!serverInstanceDt.Rows.Cast<DataRow>().Any(r => r.Field<string>("ServerName") == serverName))
                    {
                        serverInstanceDt.Rows.Add(serverName);
                    }
                }
            }
            repositoryItemGridLookupDatabaseServer.DataSource = serverInstanceDt;
    
            repositoryItemGridLookupDatabaseServer.DisplayMember = "ServerName"; // Adjust column name
            repositoryItemGridLookupDatabaseServer.ValueMember = "ServerName"; // Adjust column name
            repositoryItemGridLookupDatabaseServer.PopupFormSize = new Size(600, 900);
            repositoryItemGridLookupDatabaseServer.NullText = ""; // Remove default text
            repositoryItemGridLookupDatabaseServer.SearchMode = DevExpress.XtraEditors.Repository.GridLookUpSearchMode.AutoSuggest;
            repositoryItemGridLookupDatabaseServer.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard; // Allow typing
                                                                                                                            // Set the repository item for the "Database Server" column
            gridView1.Columns["Database Server"].ColumnEdit = repositoryItemGridLookupDatabaseServer;

          



            repositoryItemGridLookupDatabaseServer.ProcessNewValue += (sender, e) =>
            {
                if (!e.Handled)
                {
                    // Check if the value is not null or empty and not already in the data source
                    if (!string.IsNullOrEmpty(e.DisplayValue.ToString()) && !serverInstanceDt.AsEnumerable().Any(row => row["ServerName"].ToString() == e.DisplayValue.ToString()))
                    {
                        // Add the new value to the data source
                        DataRow newRow = serverInstanceDt.NewRow();
                        newRow["ServerName"] = e.DisplayValue.ToString();
                        serverInstanceDt.Rows.Add(newRow);

                        // Set the Handled property to true to indicate that the new value has been processed
                        e.Handled = true;
                    }
                }
            };
           
        }
        private DataTable GetDataFromDatabase(string query)
        {
            DataTable dataTable = new DataTable();

            try
            {
                // Connect to your database and execute the query
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.Fill(dataTable);
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions here
                Console.WriteLine("Error executing SQL query: " + ex.Message);
            }

            return dataTable;
        }

        private void SaveServerNameIntoFile()
        {
            string serverName = txtServerName.Text.Trim();
            if (string.IsNullOrEmpty(serverName))
            {
                MessageBox.Show("Please enter a server name.");
                return;
            }

            string filePath = Path.Combine(Application.StartupPath, "ServerName.txt");
            try
            {
                // Check if the file exists
                if (!File.Exists(filePath))
                {
                    // Create a new file if it doesn't exist
                    using (StreamWriter sw = File.CreateText(filePath))
                    {
                        sw.WriteLine(serverName);
                    }
                }
                else
                {
                    // Write the server name to the existing file
                    File.WriteAllText(filePath, serverName);
                }

                clsConnection.TempSQLServerName = serverName;


                // Get the original connection string from web.config
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                ConnectionStringsSection connectionStringsSection = config.ConnectionStrings;
                string originalConnectionString = connectionStringsSection.ConnectionStrings["connection_string"].ConnectionString;

                // Extract the existing server name
                string originalserverName = GetDataSource(originalConnectionString);

                // Construct the modified connection string with the new server name
                string modifiedConnectionString = originalConnectionString.Replace($"Data Source={originalserverName}", $"Data Source={serverName}");



                // Update the connection string in the configuration object
                connectionStringsSection.ConnectionStrings["connection_string"].ConnectionString = modifiedConnectionString;

                // Save the changes to the configuration file
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("connectionStrings");
                // Invoke the DataSaved event
                DataSaved?.Invoke();


            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving server name: " + ex.Message);
                // Log the exception or handle it appropriately
            }
        }


        private void btnItemSave_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (gridView1.PostEditor())
                gridView1.UpdateCurrentRow();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Fetch current records from the database
                    DataTable dbRecords = new DataTable();
                    string fetchQuery = "SELECT * FROM [MultiCompany]";
                    SqlDataAdapter fetchAdapter = new SqlDataAdapter(fetchQuery, connection);
                    fetchAdapter.Fill(dbRecords);

                    // Save changes in the grid
                    foreach (DataRow row in ((DataTable)gridControl1.DataSource).Rows)
                    {
                        string company = row["Company"].ToString();
                        string companyName = row["Company Name"].ToString();
                        string dbServer = row["Database Server"].ToString();
                        string dbName = row["Database Name"].ToString();
                        int licenseControl = Convert.ToInt32(row["License Control"]); // Get the License Control value

                        // Check if the row is new (empty primary key) and skip saving if it is
                        if (string.IsNullOrWhiteSpace(company))
                            continue;

                        // Check if any required fields are empty, and skip saving if they are
                        if (string.IsNullOrWhiteSpace(companyName) || string.IsNullOrWhiteSpace(dbServer) || string.IsNullOrWhiteSpace(dbName))
                        {
                            MessageBox.Show("Please fill in all required fields (Company Name, Database Server, Database Name) before saving.");
                            return; // Stop saving and notify the user
                        }

                        // Check if the Company already exists in the database
                        string checkQuery = "SELECT COUNT(1) FROM [MultiCompany] WHERE [Company] = @Company";
                        SqlCommand checkCommand = new SqlCommand(checkQuery, connection);
                        checkCommand.Parameters.AddWithValue("@Company", company);
                        int existingCount = Convert.ToInt32(checkCommand.ExecuteScalar());

                        if (existingCount > 0)
                        {
                            // Company exists, update the record
                            string updateQuery = "UPDATE [MultiCompany] SET [Company Name] = @CompanyName, [Database Server] = @DbServer, [Database Name] = @DbName, [License Control] = @LicenseControl WHERE [Company] = @Company";
                            SqlCommand updateCommand = new SqlCommand(updateQuery, connection);
                            updateCommand.Parameters.AddWithValue("@Company", company);
                            updateCommand.Parameters.AddWithValue("@CompanyName", companyName);
                            updateCommand.Parameters.AddWithValue("@DbServer", dbServer);
                            updateCommand.Parameters.AddWithValue("@DbName", dbName);
                            updateCommand.Parameters.AddWithValue("@LicenseControl", licenseControl); // Add License Control parameter
                         
                            updateCommand.ExecuteNonQuery();
                        }
                        else
                        {
                            // Company does not exist, insert a new record
                            string insertQuery = "INSERT INTO [MultiCompany] ([Company], [Company Name], [Database Server], [Database Name], [License Control], [Concurrent Using]) VALUES (@Company, @CompanyName, @DbServer, @DbName, @LicenseControl, @ConcurrentUsing)";
                            SqlCommand insertCommand = new SqlCommand(insertQuery, connection);
                            insertCommand.Parameters.AddWithValue("@Company", company);
                            insertCommand.Parameters.AddWithValue("@CompanyName", companyName);
                            insertCommand.Parameters.AddWithValue("@DbServer", dbServer);
                            insertCommand.Parameters.AddWithValue("@DbName", dbName);
                            insertCommand.Parameters.AddWithValue("@LicenseControl", licenseControl); // Add License Control parameter
                            insertCommand.Parameters.AddWithValue("@ConcurrentUsing", 0); // Default value for Concurrent Using
                            insertCommand.ExecuteNonQuery();
                        }
                    }

                    // Compare with the fetched records and delete unmatched records
                    foreach (DataRow dbRow in dbRecords.Rows)
                    {
                        string dbCompany = dbRow["Company"].ToString();
                        if (!((DataTable)gridControl1.DataSource).Select($"Company = '{dbCompany}'").Any())
                        {
                            // Delete the record as it's not in the grid
                            string deleteQuery = "DELETE FROM [MultiCompany] WHERE [Company] = @Company";
                            SqlCommand deleteCommand = new SqlCommand(deleteQuery, connection);
                            deleteCommand.Parameters.AddWithValue("@Company", dbCompany);
                            deleteCommand.ExecuteNonQuery();
                        }
                    }

                    SaveServerNameIntoFile();
                    MessageBox.Show("Data saved successfully.");
                    // Invoke the DataSelected event with the selected data
                    DataSaved?.Invoke(); // Invoke the event if there are subscribers
                }
               
            }

            catch (Exception ex)
            {
                // Handle any exceptions
                MessageBox.Show("Error saving data: " + ex.Message);
            }
        }

        private void btnItemNew_ItemClick(object sender, ItemClickEventArgs e)
        {
            // Get the underlying DataTable from the grid's DataSource
            DataTable dataTable = (DataTable)gridControl1.DataSource;

            // Create a new DataRow and add it to the DataTable
            DataRow newRow = dataTable.NewRow();
      
            dataTable.Rows.Add(newRow);

            // Move the focus to the new row
            gridView1.FocusedRowHandle = gridView1.RowCount - 1;
        }

        private async void btnItemDelete_ItemClick(object sender, ItemClickEventArgs e)
        {
            // Check if there is a focused row
            if (gridView1.FocusedRowHandle >= 0)
            {
                // Get the focused row
                DataRow focusedRow = gridView1.GetDataRow(gridView1.FocusedRowHandle);

                if (focusedRow != null)
                {
                    string company = focusedRow["Company"].ToString();

                    try
                    {
                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {
                            connection.Open();

                            // Delete the record for the focused company
                            string deleteQuery = "DELETE FROM [MultiCompany] WHERE [Company] = @Company";
                            SqlCommand deleteCommand = new SqlCommand(deleteQuery, connection);
                            deleteCommand.Parameters.AddWithValue("@Company", company);
                            int rowsAffected = deleteCommand.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                // Refresh the grid after deletion
                                await LoadDataIntoGridAsync();
                                MessageBox.Show("Item deleted successfully.");
                                // Invoke the DataSelected event with the selected data
                                DataSaved?.Invoke(); // Invoke the event if there are subscribers
                            }
                            else
                            {
                                MessageBox.Show("No item deleted. Company not found.");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Handle any exceptions
                        MessageBox.Show("Error deleting item: " + ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("No item selected to delete.");
            }
        }

        private void btnGetAvailableServer_Click(object sender, EventArgs e)
        {
          

            try
            {
                SplashScreenManager.ShowForm(this, typeof(frmWaitForm), true, true, false);
                // Retrieve the enumerator instance and get the available SQL Server instances
                SqlDataSourceEnumerator instance = SqlDataSourceEnumerator.Instance;
                DataTable dataTable = instance.GetDataSources();
              
                // Create and show the form for listing servers
                listOfServerForm = new frmListOfServers(dataTable);
                listOfServerForm.StartPosition = FormStartPosition.CenterScreen;
                // Subscribe to the ServerSelected event
                listOfServerForm.ServerSelected += OnServerSelected;
                listOfServerForm.StartPosition = FormStartPosition.CenterScreen;
                SplashScreenManager.CloseForm(false);
                listOfServerForm.Show();
             
            }
            catch (Exception ex)
            {
            
                MessageBox.Show("Please Try Other Server Instances " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void OnServerSelected(object sender, string onSelectServerName)
        {
            try
            {
            
                // Update the text box with the selected server name
                txtServerName.Text = onSelectServerName;
              
                connectionString = $"Data Source={onSelectServerName};Initial Catalog=GreenPlus;Integrated Security=false;User ID=green;Password=Gb$$b62633933@#";


                await LoadDataIntoGridAsync();
            }
            catch (Exception ex)
            {
            
                MessageBox.Show("Please Try Other Server Instances " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private string GetDataSource(string connectionString)
        {
            int startIndex = connectionString.IndexOf("Data Source=") + "Data Source=".Length;
            int endIndex = connectionString.IndexOf(";", startIndex);
            return connectionString.Substring(startIndex, endIndex - startIndex);
        }

        private async void txtServerName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                // Construct the connection string based on the entered server name
                string serverName = txtServerName.Text.Trim();
                string connectionString = $"Data Source={serverName};Initial Catalog=GreenPlus;Integrated Security=false;User ID=green;Password=Gb$$b62633933@#";

                // Set the connection string
                this.connectionString = connectionString;

                // Load data into the grid asynchronously
                await LoadDataIntoGridAsync();
                SaveServerNameIntoFile();
                DataSaved?.Invoke(); // Invoke the event if there are subscribers
            }
        }

        private void txtServerName_TextChanged(object sender, EventArgs e)
        {

        }
    }

}