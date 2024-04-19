
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Drawing;


using System.Linq;
using GreenStem.ClassModules;
using DevExpress.XtraSplashScreen;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;

using DevExpress.Utils;

namespace GreenStem.LookUp
{
    public partial class frmLookUpCopy : Form
    {
        private Dictionary<string, Type> fieldTypes;
        private Dictionary<string, Type> previewFieldTypes;
        private Dictionary<string, string> fieldNameColumnMap;
        private string query;
        private string orderByColumnName;
        private int sortType;
        private string selectedColumnName;
        private string lookUpName;
        private string code;
        private string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connection_string"].ConnectionString;
        private bool isPreview = false;
        int maxColumnLength = 0;
        private DataTable previewData;
        private DateTime lastClickTime = DateTime.MinValue;
        private int currentPage = 1;
        private int pageSize = 20;
        private int totalRecords = 0;
        private int totalPages = 0;
        private DataTable lookupData;
        private int previewSortType;
        private string previewOrderColumn;

        public frmLookUpCopy()
        {

        }
        public void InitializedLookup(string query, Dictionary<string, Type> fieldTypes, string lookUpName, string code, Dictionary<string, string> fieldNameColumnMap)
        {
            try
            {

                //this.Icon = new Icon($".\\Image\\images.jpg");
                InitializeComponent();
                DevExpress.XtraGrid.Views.Grid.GridView existingGridView = gridView1; 
                gridViewCustomizer.ApplyCustomizations(existingGridView,true, false);
                this.query = query;
                this.fieldTypes = fieldTypes;
                this.lookUpName = lookUpName;
                this.code = code;
                this.fieldNameColumnMap = fieldNameColumnMap;   
                // Start the LoadDataIntoGrid task and await its completion
                LoadDataIntoGrid();
                IntializedButton();
                searchByColumn.SelectedIndexChanged -= searchByColumn_SelectedIndexChanged_1;
                searchByColumn.Items.Add("SearchField");
                // Set the default selected item to "SearchField"
                searchByColumn.SelectedItem = "SearchField";
                // Add items to searchByColumn based on fieldTypes
                foreach (var fieldName in fieldTypes.Keys)
                {
                    searchByColumn.Items.Add(fieldName);
                }
                //// Add "SearchField" as the first item in the ComboBox
                // Subscribe back to the event
                searchByColumn.SelectedIndexChanged += searchByColumn_SelectedIndexChanged_1;

             
         
                gridView1.OptionsView.ColumnAutoWidth = false;

                ApplyColumnInfo();
                ApplyLookupSettings();

            }

            catch (Exception ex)
            {

                Console.Error.WriteLine(ex.ToString());
            }
        }

        public frmLookUpCopy(DataTable previewData, string code, string fontName, string fontStyle, float fontSize, Color foreColor, int sortType, string orderByColumnName)
        {
            InitializeComponent();

            DevExpress.XtraGrid.Views.Grid.GridView existingGridView = gridView1; 
            gridViewCustomizer.ApplyCustomizations(existingGridView,true, false );
            gridView1.OptionsView.ColumnAutoWidth = false;
            // Initialize other properties and UI elements based on the provided parameters
            isPreview = true;
            btnSetting.Visible = false;
     
            this.previewSortType = sortType;
            this.previewOrderColumn = orderByColumnName;
            this.previewData = previewData;

            // Load preview data into the grid
            List<string> fieldNames = new List<string>();
            previewFieldTypes = new Dictionary<string, Type>();
            fieldNameColumnMap = new Dictionary<string, string>(); // Map to store field name as key and column name as value
            foreach (DataRow row in previewData.Rows)
            {
               
     
                string fieldName = row["Field Name"].ToString();
                string columnName = row["Column Name"].ToString();
                fieldNameColumnMap.Add(fieldName, columnName);
                if (!string.IsNullOrEmpty(Convert.ToString(fieldName)) && columnName != "SearchField")
                {
                    string fieldNameWithoutBrackets = fieldName.Trim('[', ']');
                    // Check if the field name already exists in the dictionary
                    if (!previewFieldTypes.ContainsKey(fieldNameWithoutBrackets) && !fieldNames.Contains(fieldNameWithoutBrackets))
                    {
                        fieldNames.Add(fieldName);
                        searchByColumn.Items.Add(fieldNameWithoutBrackets);
                        previewFieldTypes.Add(fieldNameWithoutBrackets, typeof(string)); // Adjust the type based on your requirement
                    }
                }
            }

                string tableName = "";
                string lookupSettingQuery = $"SELECT * FROM [LookupSetting_Tbl] WHERE [Code] = '{code}'";

                try
                {
                    // Create a SqlConnection
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        // Fetch data from the LookupSetting_Tbl table
                        using (SqlCommand lookupSettingCommand = new SqlCommand(lookupSettingQuery, connection))
                        using (SqlDataReader lookupSettingReader = lookupSettingCommand.ExecuteReader())
                        {
                            if (lookupSettingReader.Read())
                            {
                                tableName = lookupSettingReader["Table Name"].ToString();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Handle exceptions, e.g., display an error message
                    MessageBox.Show($"Error: {ex.Message}");
                }
            // Construct the dynamic lookupQuery
            List<string> selectedColumns = new List<string>();
            foreach (string fieldName in fieldNames)
            {
                // Map the field name to its corresponding column name
                string columnName = fieldNameColumnMap[fieldName];
                selectedColumns.Add($"{fieldName} AS '{columnName}'");
            }

            // Construct the dynamic lookupQuery
            //  string lookupQuery = $"SELECT {string.Join(", ", fieldNames)} FROM [{tableName}]";
            string lookupQuery = $"SELECT {string.Join(", ", selectedColumns)} FROM [{tableName}]";
       


                this.query = lookupQuery;
                LoadPreviewDataIntoGrid();
                IntializedButton();
                // Add "SearchField" as the first item in the ComboBox
                searchByColumn.Items.Add("SearchField");
                // Set the default selected item to "SearchField"
                searchByColumn.SelectedItem = "SearchField";
                // Disable editing in the GridView
                gridView1.OptionsBehavior.Editable = false;

                // Apply styling properties
                ApplyStyling(fontName, fontStyle, fontSize, foreColor, sortType, orderByColumnName);
                // Subscribe to the EndSorting event
               
           }
        
    
        // Add the LoadPreviewDataIntoGrid method to load preview data
        private void LoadPreviewDataIntoGrid()
        {
            SplashScreenManager.ShowForm(this, typeof(frmWaitForm), true, true, false);
            try
            {
                DataTable dataTable;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Calculate the total number of records
                    using (SqlCommand countCommand = new SqlCommand($"SELECT COUNT(*) FROM ({query}) AS subQuery", connection))
                    {
                        totalRecords = (int)countCommand.ExecuteScalar();
                    }

                    // Calculate the total number of pages
                    totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

                  
                    if (string.IsNullOrEmpty(previewOrderColumn))
                    {
                        if (previewFieldTypes != null && previewFieldTypes.Count > 0)
                        {
                            previewOrderColumn = $"{previewFieldTypes.Keys.First()}"; // Get the first field name and enclose it in []
                        }
                    }
                    if (!previewOrderColumn.StartsWith("[") && !previewOrderColumn.EndsWith("]"))
                    {
                        // Enclose the string with square brackets
                        previewOrderColumn = "[" + previewOrderColumn + "]";
                    }
                    // Determine the sort direction based on the sort type
                    string sortDirection = previewSortType == 0 ? "ASC" : "DESC";

                    // Modify the SQL query to include pagination, dynamic order by, and sort direction
                    string paginatedQuery = "";
                    if (pageSize > 0)
                    {
                        // Calculate the offset based on the current page and page size
                        int offset = (currentPage - 1) * pageSize;
                        paginatedQuery = $"{query} ORDER BY {previewOrderColumn} {sortDirection} OFFSET {offset} ROWS FETCH NEXT {pageSize} ROWS ONLY";
                        lblPageInfo.Text = $"Page {currentPage} of {totalPages}";
                    }
                    else
                    {
                        // If pageSize is -1 (indicating "All" records), fetch all rows without pagination
                        paginatedQuery = $"{query} ORDER BY {previewOrderColumn} {sortDirection}";
                        lblPageInfo.Text = $"Page 1 of 1";
                    }

                    // Create a SqlDataAdapter
                    SqlDataAdapter dataAdapter = new SqlDataAdapter(paginatedQuery, connection);

                    // Create a DataTable to hold the data
                    dataTable = new DataTable();

                    // Fill the DataTable with data from the database
                    dataAdapter.Fill(dataTable);
                    lookupData = new DataTable();
                    this.lookupData = dataTable;
                }

                // Bind the DataTable to the gridControl1
                gridControl1.DataSource = dataTable;

                // Update the UI to display the current page information

                lblRecoundCount.Text = totalRecords.ToString();
                SplashScreenManager.CloseForm(false);
            }
            catch (Exception ex)
            {
                // Handle exceptions
                MessageBox.Show($"Error: {ex.Message}");
                Console.Error.WriteLine(ex.ToString());
                SplashScreenManager.CloseForm(false);
                ExceptionLogger.LogException(ex, nameof(frmLookUp), "LookUp", "LoadPreviewDataIntoGrid", new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
            }
        }

        private void LoadDataIntoGrid()
        {
            SplashScreenManager.ShowForm(this, typeof(frmWaitForm), true, true, false);
            try
            {
                DataTable dataTable;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Calculate the total number of records
                    using (SqlCommand countCommand = new SqlCommand($"SELECT COUNT(*) FROM ({query}) AS subQuery", connection))
                    {
                        totalRecords = (int)countCommand.ExecuteScalar();
                    }

                    // Calculate the total number of pages
                    totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

                    // Fetch the order by column and sort type from the database
                    string orderByColumnName = "";
                    int sortType = 0; // Default sort type (ascending)
                    using (SqlCommand orderByCommand = new SqlCommand($"SELECT [Order By],[Sort Type] FROM [LookupSetting_Tbl] WHERE [Lookup Name] = '{lookUpName}'", connection))
                    {
                        using (SqlDataReader reader = orderByCommand.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                orderByColumnName = reader["Order By"].ToString();
                                sortType = Convert.ToInt32(reader["Sort Type"]);
                            }
                        }
                    }

                    if (string.IsNullOrEmpty(orderByColumnName))
                    {
                        if (fieldTypes != null && fieldTypes.Count > 0)
                        {
                            orderByColumnName = $"{fieldTypes.Keys.First()}"; // Get the first field name and enclose it in []
                        }
                    }
                    if (!orderByColumnName.StartsWith("[") && !orderByColumnName.EndsWith("]"))
                    {
                        // Enclose the string with square brackets
                        orderByColumnName = "[" + orderByColumnName + "]";
                    }
                    // Determine the sort direction based on the sort type
                    string sortDirection = sortType == 0 ? "ASC" : "DESC";

                    // Modify the SQL query to include pagination, dynamic order by, and sort direction
                    string paginatedQuery = "";
                    if (pageSize > 0)
                    {
                        // Calculate the offset based on the current page and page size
                        int offset = (currentPage - 1) * pageSize;
                        paginatedQuery = $"{query} ORDER BY {orderByColumnName} {sortDirection} OFFSET {offset} ROWS FETCH NEXT {pageSize} ROWS ONLY";
                        lblPageInfo.Text = $"Page {currentPage} of {totalPages}";
                    }
                    else
                    {
                        // If pageSize is -1 (indicating "All" records), fetch all rows without pagination
                        paginatedQuery = $"{query} ORDER BY {orderByColumnName} {sortDirection}";
                        lblPageInfo.Text = $"Page 1 of 1";
                    }

                    // Create a SqlDataAdapter
                    SqlDataAdapter dataAdapter = new SqlDataAdapter(paginatedQuery, connection);

                    // Create a DataTable to hold the data
                    dataTable = new DataTable();

                    // Fill the DataTable with data from the database
                    dataAdapter.Fill(dataTable);
                    lookupData = new DataTable();
                    this.lookupData = dataTable;
                }

                // Bind the DataTable to the gridControl1
                gridControl1.DataSource = dataTable;

                // Update the UI to display the current page information

                lblRecoundCount.Text = totalRecords.ToString();
                SplashScreenManager.CloseForm(false);
            }
            catch (Exception ex)
            {
                // Handle exceptions
                MessageBox.Show($"Error: {ex.Message}");
                Console.Error.WriteLine(ex.ToString());
                SplashScreenManager.CloseForm(false);
                ExceptionLogger.LogException(ex, nameof(frmLookUp), "LookUp", "LoadDataIntoGrid", new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
            }
        }

        // Add the ApplyStyling method to apply styling properties
        private void ApplyStyling(string fontName, string fontStyle, float fontSize, Color foreColor, int sortType, string orderByColumnName)
        {
            if (fontSize <= 0)
            {
                // Set a default font size or handle this case differently
                fontSize = 10; // You can adjust the default font size as needed
            }
            // Apply font-related properties to the entire GridView
            gridView1.Appearance.Row.Font = new Font(fontName, fontSize, FontStyleFromString(fontStyle));
            gridView1.Appearance.HeaderPanel.Font = new Font(fontName, fontSize, FontStyleFromString(fontStyle));
            gridView1.Appearance.FooterPanel.Font = new Font(fontName, fontSize, FontStyleFromString(fontStyle));
            gridView1.Appearance.Row.ForeColor = foreColor; // Set font color for rows
            gridView1.Appearance.HeaderPanel.ForeColor = foreColor; // Set font color for headers
            gridView1.Appearance.FooterPanel.ForeColor = foreColor; // Set font color for footers

            // Apply sorting if applicable
            if (!string.IsNullOrEmpty(orderByColumnName))
            {
                // Find the column by name
                DevExpress.XtraGrid.Columns.GridColumn columnToSort = gridView1.Columns.ColumnByFieldName(orderByColumnName.Trim('[', ']'));

                if (columnToSort != null)
                {
                    // Clear existing sort info
                    gridView1.ClearSorting();

                    // Set the sort order
                    columnToSort.SortOrder = (sortType == 0) ? DevExpress.Data.ColumnSortOrder.Ascending : DevExpress.Data.ColumnSortOrder.Descending;
                }
            }
            foreach (DataRow row in previewData.Rows)
            {
                string fieldName = row["Field Name"].ToString();
                string columnName = row["Column Name"].ToString();
                if (!string.IsNullOrEmpty(Convert.ToString(fieldName)) && columnName != "SearchField")
                {

                    int columnWidth = Convert.ToInt32(row["Column Width"]);
                    int columnLength = Convert.ToInt32(row["Column Length"]);
                    string colAlignment = row["Col Alignment"].ToString();
                    // Set the column width, length, and alignment dynamically based on the retrieved values
                    if (gridView1.Columns[columnName] != null)
                    {
                        gridView1.Columns[columnName].Width = columnWidth;
                        // Update maxColumnLength if needed
                        maxColumnLength = Math.Max(maxColumnLength, columnLength);



                        // Set the column alignment
                        gridView1.Columns[columnName].AppearanceCell.TextOptions.HAlignment = ConvertColAlignment(colAlignment);
                    }

                }

            }
            gridView1.RowHeight = maxColumnLength;
        }

        public delegate void DataSelectedHandler(Dictionary<string, object> selectedData);
        public event DataSelectedHandler DataSelected;

        private void gridView1_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                GridView view = sender as GridView;
                GridHitInfo hitInfo = view.CalcHitInfo(view.GridControl.PointToClient(Control.MousePosition));

                // Calculate the time elapsed since the last double-click
                TimeSpan elapsed = DateTime.Now - lastClickTime;

                // If less than 300 milliseconds have passed since the last double-click, ignore this event
                if (elapsed.TotalMilliseconds < 200)
                {
                    return;
                }

                // Update the last click time
                lastClickTime = DateTime.Now;

                // Check if the double-click event occurred on a row
                if (hitInfo.InRow)
                {
                    int selectedRowHandle = gridView1.FocusedRowHandle;

                    if (selectedRowHandle >= 0)
                    {
                        Dictionary<string, object> selectedData = new Dictionary<string, object>();

                        foreach (var fieldName in fieldTypes.Keys)
                        {
                            string fieldNameKey = $"[{fieldName}]";
                            if (fieldNameColumnMap.ContainsKey(fieldNameKey))
                            {
                                // Use the fieldNameColumnMap to map the fieldName to its corresponding column name
                                string columnName = fieldNameColumnMap[fieldNameKey];

                                // Retrieve the cell value using the column name
                                object value = gridView1.GetRowCellValue(selectedRowHandle, columnName);

                                // Add the fieldName and its corresponding value to the selectedData dictionary
                                selectedData.Add(fieldName, value);
                            }
                            else
                            {
                                Console.WriteLine($"Column mapping not found for field: {fieldName}");
                                // Handle the case where the column mapping is missing
                            }
                        }

                        // Invoke the DataSelected event with the selected data
                        DataSelected?.Invoke(selectedData);
                        this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle the exception, e.g., log it or show an error message
                Console.WriteLine($"Error in gridView1_DoubleClick: {ex.Message}");
                MessageBox.Show("An error occurred while processing the double-click event.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        // Handle the event when the first page button is clicked
        private void BtnFirst_Click(object sender, EventArgs e)
        {
            currentPage = 1;
            if (isPreview)
                LoadPreviewDataIntoGrid();
            else
            {
                LoadDataIntoGrid();
            }
        }

        // Handle the event when the next page button is clicked
        private void BtnNext_Click(object sender, EventArgs e)
        {
            if (currentPage < totalPages)
            {
                currentPage++;
                if (isPreview)
                    LoadPreviewDataIntoGrid();
                else
                {
                    LoadDataIntoGrid();
                }
            }
        }

        // Handle the event when the page size is changed
        private void CmbPageSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbPageSize.SelectedItem.ToString() == "All")
            {
                // Set pageSize to -1 to indicate showing all records
                pageSize = -1;
            }
            else
            {
                // Set pageSize to the selected value
                pageSize = Convert.ToInt32(cmbPageSize.SelectedItem);
            }

            currentPage = 1; // Reset to the first page
            if (isPreview)
                LoadPreviewDataIntoGrid();
            else
            {
                LoadDataIntoGrid();
            }
        }

        private void ApplyFilters(DataTable dataTable)
        {
            try
            {
                string searchCondition = string.Empty;
                if (!string.IsNullOrEmpty(txtSearchField.Text))
                {
                    if (!string.IsNullOrEmpty(selectedColumnName) && selectedColumnName != "SearchField")
                    {

                        // Escape the column name to handle spaces or special characters
                        string escapedColumnName = $"[{selectedColumnName}]";


                        // Apply filter to non-decimal columns
                        searchCondition = $"{escapedColumnName} LIKE '%{txtSearchField.Text}%'";

                    }
                    else
                    {
                        // Construct the search condition dynamically
                        foreach (DataColumn column in dataTable.Columns)
                        {
                            string escapedColumnName = $"[{column.ColumnName}]";

                            if (!string.IsNullOrEmpty(searchCondition))
                            {
                                searchCondition += " OR ";
                            }
                            searchCondition += $"{escapedColumnName} LIKE '%{txtSearchField.Text}%'";
                        }
                    }

                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        string queryWithSearch = query;
                        if (!string.IsNullOrEmpty(searchCondition))
                        {
                            queryWithSearch += $" WHERE {searchCondition}";
                        }

                        // Fetch the data from the database
                        using (SqlCommand command = new SqlCommand(queryWithSearch, connection))
                        {
                            SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                            DataTable filteredDataTable = new DataTable();
                            dataAdapter.Fill(filteredDataTable);

                            // Bind the filtered DataTable to the gridControl1
                            gridControl1.DataSource = filteredDataTable;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                // Handle exceptions, e.g., display an error message
                MessageBox.Show($"Error applying filters: {ex.Message}");
                Console.Error.WriteLine(ex.ToString());
                ExceptionLogger.LogException(ex, nameof(frmLookUp), "LookUp", "ApplyFilters", new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
            }
        }
        private HorzAlignment ConvertColAlignment(string colAlignment)
        {
            switch (colAlignment.ToLower())
            {
                case "left":
                    return HorzAlignment.Near;
                case "center":
                    return HorzAlignment.Center;
                case "right":
                    return HorzAlignment.Far;
                default:
                    return HorzAlignment.Default;
            }
        }
        private FontStyle FontStyleFromString(string fontStyle)
        {
            switch (fontStyle.ToLower())
            {
                case "bold":
                    return FontStyle.Bold;
                case "italic":
                    return FontStyle.Italic;
                case "underline":
                    return FontStyle.Underline;
                // Add more cases as needed
                default:
                    return FontStyle.Regular;
            }
        }


        public static void OpenLookupForm(string code, DataSelectedHandler dataSelectedHandler, Form callingForm)
        {

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connection_string"].ConnectionString;
            string lookupName = "";
            string tableName = "";
            string lookupSettingQuery = $"SELECT * FROM [LookupSetting_Tbl] WHERE [Code] = '{code}'";

            try
            {



                // Create a SqlConnection
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Fetch data from the LookupSetting_Tbl table
                    using (SqlCommand lookupSettingCommand = new SqlCommand(lookupSettingQuery, connection))
                    using (SqlDataReader lookupSettingReader = lookupSettingCommand.ExecuteReader())
                    {
                        if (lookupSettingReader.Read())
                        {
                            // Retrieve data from the database
                            lookupName = lookupSettingReader["Lookup Name"].ToString();
                            tableName = lookupSettingReader["Table Name"].ToString();

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions, e.g., display an error message
                MessageBox.Show($"Error: {ex.Message}");
                Console.WriteLine(ex.ToString());
                ExceptionLogger.LogException(ex, nameof(frmLookUp), "LookUp", "OpenLookupForm", new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
            }

            // Specify query to retrieve column width and length from LookupSetting_Detail_Tbl
            string columnWidthQuery = $"SELECT [Field Name], [Column Name] FROM [LookupSetting_Detail_Tbl] WHERE [Code] = '{code}' AND [Column Name] <> 'SearchField'" +
                                       $" ORDER BY [Column Index]";


            Dictionary<string, Type> fieldTypes = new Dictionary<string, Type>();



            try
            {


                // Create a SqlConnection
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Create a SqlCommand to execute the query
                    using (SqlCommand command = new SqlCommand(columnWidthQuery, connection))
                    {
                        // Execute the query and get the SqlDataReader
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            // Build the dynamic lookupQuery
                            List<string> fieldNames = new List<string>();
                            Dictionary<string, string>  fieldNameColumnMap = new Dictionary<string, string>(); // Map to store field name as key and column name as value
                            while (reader.Read())
                            {
                                string fieldName = reader["Field Name"].ToString();
                       
                                string columnName = reader["Column Name"].ToString();
                                string fieldNameWithoutBrackets = fieldName.Trim('[', ']'); // Remove square brackets

                                // Skip the SearchField column
                                if (fieldName != "SearchField")
                                {
                                    if (!fieldTypes.ContainsKey(fieldNameWithoutBrackets))
                                    {
                                        fieldTypes.Add(fieldNameWithoutBrackets, typeof(string)); // Adjust the type based on your requirement
                                        fieldNames.Add($"{fieldName}");
                                        fieldNameColumnMap.Add(fieldName, columnName);
                                    }

                                }
                            }
                            // Construct the dynamic lookupQuery
                            List<string> selectedColumns = new List<string>();
                            foreach (string fieldName in fieldNames)
                            {
                                // Map the field name to its corresponding column name
                                string columnName = fieldNameColumnMap[fieldName];
                                selectedColumns.Add($"{fieldName} AS '{columnName}'");
                            }

                            // Construct the dynamic lookupQuery
                          //  string lookupQuery = $"SELECT {string.Join(", ", fieldNames)} FROM [{tableName}]";
                            string lookupQuery = $"SELECT {string.Join(", ", selectedColumns)} FROM [{tableName}]";
                            // Create an instance of the LookUp form with the specified query and field types
                            frmLookUpCopy lookupForm = new frmLookUpCopy();
                            lookupForm.InitializedLookup(lookupQuery, fieldTypes, lookupName, code, fieldNameColumnMap);
                            // Subscribe to the DataSelected event to handle the selected data
                            lookupForm.DataSelected += dataSelectedHandler;
                            lookupForm.Owner = callingForm; // Set the owner to the main form

                            // lookupForm.GotFocus += (sender, e) => { callingForm.Focus(); }; // Redirect focus to MainMenu]
                            lookupForm.Resize += (sender, e) =>
                            {
                                // Check if the form is minimized
                                if (lookupForm.WindowState == FormWindowState.Minimized)
                                {
                                    callingForm.Activate();
                                  
                                }
                            };
                            // Handle the FormClosing event of the lookup form
                            lookupForm.FormClosing += (sender, e) =>
                            {
                                callingForm.Activate();
                                // Set the focus back to the calling form when the lookup form is closing
                                //callingForm.Focus();
                            };

                            lookupForm.StartPosition = FormStartPosition.CenterScreen;
                            // Show the LookUp form
                            lookupForm.Show();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions, e.g., display an error message
                MessageBox.Show($"Error: {ex.Message}");
                Console.WriteLine(ex.ToString());
                ExceptionLogger.LogException(ex, nameof(frmLookUp), "LookUp", "OpenLookupForm", new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
            }
        }



        private void txtSearchField_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSearchField.Text)) // Check if the text is empty or contains only white spaces
            {
                if (isPreview)
                    LoadPreviewDataIntoGrid();
                else
                {
                    LoadDataIntoGrid();
                }
                return;
            }
           
              ApplyFilters(lookupData);
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
          
                ApplyFilters(lookupData);
        }

        private void clearBtn_Click(object sender, EventArgs e)
        {
            clearFilters();
            txtSearchField.Clear();
        }

        private void clearFilters()
        {
            LoadDataIntoGrid();
        }



        private void searchByColumn_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            // Update the selected column based on the ComboBox selection
            selectedColumnName = searchByColumn.SelectedItem.ToString();
           
                ApplyFilters(lookupData);

        }

        private void previousBtn_Click(object sender, EventArgs e)
        {
            gridView1.FocusedRowHandle = 0;
        }

        private void forwardBtn_Click(object sender, EventArgs e)
        {
            gridView1.FocusedRowHandle = gridView1.RowCount - 1;
        }

        private void btnSetting_Click(object sender, EventArgs e)
        {

            frmLookUpSettings lookUpSettings = new frmLookUpSettings(code);
            lookUpSettings.StartPosition = FormStartPosition.CenterScreen;
            lookUpSettings.Show();

        }

     

        private void ApplyLookupSettings()
        {

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string lookupSettingQuery = $"SELECT [FontName], [FontStyle], [ForeColor], [FontSize] FROM [LookupSetting_Tbl] WHERE [Lookup Name] = '{lookUpName}'";

                using (SqlCommand lookupSettingCommand = new SqlCommand(lookupSettingQuery, connection))
                using (SqlDataReader lookupSettingReader = lookupSettingCommand.ExecuteReader())
                {
                    if (lookupSettingReader.Read())
                    {
                       

                        // Retrieve font-related properties
                        string fontName = lookupSettingReader["FontName"].ToString();
                        string fontStyle = lookupSettingReader["FontStyle"].ToString();
                        int foreColorValue = Convert.ToInt32(lookupSettingReader["ForeColor"]);
                        float fontSize = Convert.ToSingle(lookupSettingReader["FontSize"]);
                        if (fontSize <= 0)
                        {
                            // Set a default font size or handle this case differently
                            fontSize = 10; // You can adjust the default font size as needed
                        }
                        // Convert foreColorValue to Color object
                        Color foreColor = Color.FromArgb(foreColorValue);

                        // Apply font-related properties to the entire GridView

                        gridView1.Appearance.Row.Font = new Font(fontName, fontSize, FontStyleFromString(fontStyle));
                        gridView1.Appearance.HeaderPanel.Font = new Font(fontName, fontSize, FontStyleFromString(fontStyle));
                        gridView1.Appearance.FooterPanel.Font = new Font(fontName, fontSize, FontStyleFromString(fontStyle));
                        gridView1.Appearance.Row.ForeColor = foreColor; // Set font color for rows

                     
                    }
                }
            }

        }

        private void ApplyColumnInfo()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string columnInfoQuery = $"SELECT [Field Name],[Column Name],[Column Width], [Column Length], [Col Alignment] FROM [LookupSetting_Detail_Tbl] WHERE [Lookup Name] = '{lookUpName}'";

                using (SqlCommand columnInfoCommand = new SqlCommand(columnInfoQuery, connection))
                using (SqlDataReader columnInfoReader = columnInfoCommand.ExecuteReader())
                {
                    while (columnInfoReader.Read())
                    {
                        string fieldName = columnInfoReader["Column Name"].ToString().Trim('[', ']');

                        int columnWidth = Convert.ToInt32(columnInfoReader["Column Width"]);
                        int columnLength = Convert.ToInt32(columnInfoReader["Column Length"]);
                        string colAlignment = columnInfoReader["Col Alignment"].ToString();

                        // Marshal UI update to the main thread

                        // Set the column width, length, and alignment dynamically based on the retrieved values
                        if (gridView1.Columns[fieldName] != null)
                        {
                            gridView1.Columns[fieldName].Width = columnLength * 10;
                            // Update maxColumnLength if needed
                            maxColumnLength = Math.Max(maxColumnLength, columnLength);

                            // Set the column alignment
                            gridView1.Columns[fieldName].AppearanceCell.TextOptions.HAlignment = ConvertColAlignment(colAlignment);
                        }

                    }
                }
            }
        }
        private void IntializedButton()
        {
            try
            {
                btnFirst.Click += BtnFirst_Click;
                btnNext.Click += BtnNext_Click;
                cmbPageSize.SelectedIndexChanged += CmbPageSize_SelectedIndexChanged;
                cmbPageSize.Items.Add("20");
                cmbPageSize.Items.Add("50");
                cmbPageSize.Items.Add("100");
                cmbPageSize.Items.Add("All");
                // Set the default selected item to "20"
                cmbPageSize.SelectedItem = "20";
            }
            catch (Exception ex)
            {
                // Print out the exception message to the console
                Console.WriteLine($"An error occurred: {ex.Message}");
                ExceptionLogger.LogException(ex, nameof(frmLookUp), "LookUp", "IntializedButton", new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
            }
        }


    }

}

