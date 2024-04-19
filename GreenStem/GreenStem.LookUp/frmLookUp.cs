
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Drawing;
using DevExpress.XtraReports.Design;
using DevExpress.CodeParser;
using DevExpress.Utils;


using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Tile;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraGrid.Views.Grid;
using System.Threading.Tasks;
using DevExpress.XtraSplashScreen;
using DevExpress.XtraWaitForm;
using DevExpress.Data.Linq;
using DevExpress.Data;
using System.Threading;
using System.Diagnostics;
using GreenStem.ClassModules;
using DevExpress.Utils.Filtering;
using DevExpress.XtraExport.Helpers;
using System.Linq;
using DevExpress.XtraReports.Parameters;
using static DevExpress.Utils.Svg.CommonSvgImages;
using DevExpress.XtraGrid.Columns;
namespace GreenStem.LookUp
{
    public partial class frmLookUp : Form
    {
        private Dictionary<string, Type> fieldTypes;
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
        private DataTable dt;
        private VirtualServerModeSource virtualServerModeSource;
        private Dictionary<string, string> fieldNameColumnMap;
        private ContextMenuStrip gridContextMenu;
        private CancellationTokenSource cancellationTokenSource;
        private bool isFiltering = false;
        private DataTable filteredDataTable = new DataTable();
        private System.Windows.Forms.Timer searchDelayTimer;
        private bool isSearchField = false;
        public frmLookUp()
        {

        }
        public async void InitializedLookup(string query, Dictionary<string, Type> fieldTypes, string lookUpName, string code, Dictionary<string, string> fieldNameColumnMap, bool autoWidth, bool isSearchField)
        {
            try
            {

                //this.Icon = new Icon($".\\Image\\images.jpg");
                InitializeComponent();
                SplashScreenManager.ShowForm(this, typeof(frmWaitForm), true, true, true);
                gridContextMenu = new ContextMenuStrip();
                ToolStripMenuItem copyItem = new ToolStripMenuItem("Copy to Clipboard");
                gridContextMenu.Items.Add(copyItem);

                // Handle the Click event of the "Copy to Clipboard" item
                copyItem.Click += (s, e) => CopyGridToClipboard();

                // Assign the context menu to the grid
                gridControl1.ContextMenuStrip = gridContextMenu;

                if (autoWidth)
                {
                    gridView1.OptionsView.ColumnAutoWidth = true;

                }
                else
                {
                    gridView1.OptionsView.ColumnAutoWidth = false;
                }
                // Best fit columns after data is loaded

                searchDelayTimer = new System.Windows.Forms.Timer();
                searchDelayTimer.Interval = 900; // Set the interval (milliseconds) to wait after text stops changing
                searchDelayTimer.Tick += SearchDelayTimer_Tick;
                gridView1.OptionsBehavior.Editable = false;
            
                this.query = query;
                this.fieldTypes = fieldTypes;
                this.lookUpName = lookUpName;
                this.code = code;
                this.fieldNameColumnMap = fieldNameColumnMap;
                this.isSearchField = isSearchField;
          

                searchByColumn.SelectedIndexChanged -= searchByColumn_SelectedIndexChanged_1;
                searchByColumn.Items.Add("SearchField");
                // Set the default selected item to "SearchField"
                searchByColumn.SelectedItem = "SearchField";
                // Add items to searchByColumn based on fieldTypes
                foreach (var fieldName in fieldTypes.Keys)
                {
                    searchByColumn.Items.Add(fieldName);
                }
                // Start the LoadDataIntoGrid task and await its completion
                cancellationTokenSource = new CancellationTokenSource();
                await LoadDataIntoGrid(cancellationTokenSource.Token);
             
                //// Add "SearchField" as the first item in the ComboBox
                // Subscribe back to the event
                searchByColumn.SelectedIndexChanged += searchByColumn_SelectedIndexChanged_1;

                // Subscribe to the EndSorting event
                gridView1.EndSorting += GridView1_EndSorting;

                //    await Task.Delay(0); // Adjust the delay time as needed
                //    ApplyColumnInfo();
                //    ApplyLookupSettings();
                //



            }

            catch (Exception ex)
            {

                Console.Error.WriteLine(ex.ToString());
            }
        }
        private void gridView_CustomDrawEmptyForeground(object sender, DevExpress.XtraGrid.Views.Base.CustomDrawEventArgs e)
        {
            GridView view = sender as GridView;
            if (filteredDataTable == null)
            {
                string text = "No data found for the search criteria.";
                Font font = new Font("Arial", 12, FontStyle.Bold); // Increase the font size here
                StringFormat format = new StringFormat();
                format.Alignment = StringAlignment.Center; // Center alignment for horizontal
                format.LineAlignment = StringAlignment.Center; // Center alignment for vertical

                // Use the entire bounds of the view for drawing the string
                Rectangle r = new Rectangle(e.Bounds.Left, e.Bounds.Top, e.Bounds.Width, e.Bounds.Height);
                e.Graphics.DrawString(text, font, Brushes.Black, r, format);
            }
        }
        private void CopyGridToClipboard()
        {

            // Get the GridView from the GridControl
            GridView view = gridControl1.MainView as GridView;

            if (view != null)
            {
                view.OptionsSelection.MultiSelect = true;
                // Select all rows
                view.SelectAll();

                // Copy the selected data to the clipboard
                view.CopyToClipboard();

            }
        }

        public frmLookUp(DataTable previewData, string code, string fontName, string fontStyle, float fontSize, Color foreColor, int sortType, string orderByColumnName)
        {
            InitializeComponent();
            gridView1.CustomDrawCell += gridView1_CustomDrawCell;
            gridView1.OptionsView.ColumnAutoWidth = false;
            // Initialize other properties and UI elements based on the provided parameters
            isPreview = true;
            btnSetting.Visible = false;
            this.previewData = previewData;
            // Load preview data into the grid
            List<string> fieldNames = new List<string>();
            fieldTypes = new Dictionary<string, Type>();

            foreach (DataRow row in previewData.Rows)
            {
                string fieldName = row["Field Name"].ToString();
                string columnName = row["Column Name"].ToString();
                if (!string.IsNullOrEmpty(Convert.ToString(fieldName)) && columnName != "SearchField")
                {

                    string fieldNameWithoutBrackets = fieldName.ToString().Trim('[', ']');
                    fieldNames.Add(fieldName);
                    searchByColumn.Items.Add(fieldNameWithoutBrackets);
                    fieldTypes.Add(fieldNameWithoutBrackets, typeof(string)); // Adjust the type based on your requirement
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
            string lookupQuery = $"SELECT {string.Join(", ", fieldNames)} FROM [{tableName}]";


            this.query = lookupQuery;
            LoadPreviewDataIntoGrid();
            // Add "SearchField" as the first item in the ComboBox
            searchByColumn.Items.Add("SearchField");
            // Set the default selected item to "SearchField"
            searchByColumn.SelectedItem = "SearchField";
            // Disable editing in the GridView
            gridView1.OptionsBehavior.Editable = false;
            // Apply styling properties
            ApplyStyling(fontName, fontStyle, fontSize, foreColor, sortType, orderByColumnName);
            // Subscribe to the EndSorting event
            gridView1.EndSorting += GridView1_EndSorting;
        }



        public async Task LoadDataIntoGrid(CancellationToken cancellationToken)
        {
            string orderByColumnName = "";
            int sortType = 0; // Default sort type (ascending)

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand orderByCommand = new SqlCommand($"SELECT [Order By],[Sort Type] FROM [LookupSetting_Tbl] WHERE [Lookup Name] = '{lookUpName}'", connection))
                    using (SqlDataReader reader = await orderByCommand.ExecuteReaderAsync(cancellationToken))
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
                        orderByColumnName = fieldTypes.Keys.First();
                    }
                }

                if (!orderByColumnName.StartsWith("[") && !orderByColumnName.EndsWith("]"))
                {
                    orderByColumnName = "[" + orderByColumnName + "]";
                }

                this.orderByColumnName = orderByColumnName;
                string sortDirection = sortType == 0 ? "ASC" : "DESC";

                await Task.Run(async () =>
                {
                    int batchSize = 1000;
                    int offset = 0;
                    bool moreDataAvailable = true;

                    while (moreDataAvailable)
                    {
                        DataTable batchTable = await LoadBatchFromDatabase(query, offset, batchSize, orderByColumnName, cancellationToken);

                        if (batchTable.Rows.Count == 0)
                        {
                            moreDataAvailable = false;
                            break;
                        }

                        this.Invoke((MethodInvoker)delegate
                        {
                            if (dt == null)
                            {
                                dt = batchTable.Copy();
                                gridControl1.DataSource = dt;
                                SplashScreenManager.CloseForm(false);
                              
                                gridView1.BestFitColumns();
                            }
                            else
                            {
                                dt.Merge(batchTable);
                            }
                        });

                        offset += batchSize;
                    }
                }, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                // Cleanup resources in the finally block
                if (dt != null)
                {
                    dt.Dispose(); // Dispose of the DataTable
                    dt = null;    // Set to null to release reference
                }
                Console.WriteLine("Loading data was canceleddd.");
            }
            catch (Exception ex)
            {

            }
        }

        private async Task<DataTable> LoadBatchFromDatabase(string query, int offset, int batchSize, string orderByColumnName, CancellationToken cancellationToken)
        {
            DataTable dataTable = new DataTable();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync(cancellationToken);

                string batchQuery = $"{query} ORDER BY {orderByColumnName} OFFSET {offset} ROWS FETCH NEXT {batchSize} ROWS ONLY";

                using (SqlDataAdapter dataAdapter = new SqlDataAdapter(batchQuery, connection))
                {
                    await Task.Run(() =>
                    {
                        if (cancellationTokenSource.IsCancellationRequested)
                        {
                            return;
                        }
                        dataAdapter.Fill(dataTable);
                    }, cancellationToken);
                }
            }

            return dataTable;
        }


        private async Task ApplyFilters()
        {
            SplashScreenManager.CloseForm(false);
            SplashScreenManager.ShowForm(this, typeof(frmWaitForm), true, true, true);

            try
            {
                isFiltering = true;
                gridControl1.DataSource = null;
                filteredDataTable = null;

                string searchText = txtSearchField.Text;
                string searchCondition = string.Empty;
                // Update the selected column based on the ComboBox selection
                selectedColumnName = searchByColumn.SelectedItem.ToString();
                if (!string.IsNullOrEmpty(searchText))
                {
                    if (!string.IsNullOrEmpty(selectedColumnName) && selectedColumnName != "SearchField")
                    {
                        string escapedColumnName = $"[{selectedColumnName}]";
                        searchCondition = $"{escapedColumnName} LIKE '%{searchText}%'";
                    }
                    else if (isSearchField)
                    {
                        // Directly compare with the SearchField column
                        searchCondition = $"[SearchField] LIKE '%{searchText}%'";
                    }
                    else
                    {
                        // Loop through all field names if not specifically searching in SearchField
                        foreach (var fieldName in fieldNameColumnMap.Keys)
                        {
                            if (!string.IsNullOrEmpty(searchCondition))
                            {
                                searchCondition += " OR ";
                            }
                            searchCondition += $"{fieldName} LIKE '%{searchText}%'";
                        }
                    }
                }

                int batchSize = 10;
                int offset = 0;
                bool moreDataAvailable = true;

                CancellationToken cancellationToken = cancellationTokenSource.Token;

                await Task.Run(async () =>
                {
                    while (moreDataAvailable)
                    {
                        string queryWithSearch = query;

                        if (!string.IsNullOrEmpty(searchCondition))
                        {
                            queryWithSearch += $" WHERE {searchCondition}";
                        }

                        DataTable batchTable = await LoadSearchBatchFromDatabase(queryWithSearch, offset, batchSize, orderByColumnName, cancellationToken);

                        if (batchTable.Rows.Count == 0)
                        {
                            gridView1.CustomDrawEmptyForeground += gridView_CustomDrawEmptyForeground;
                            moreDataAvailable = false;
                            SplashScreenManager.CloseForm(false);
                           
                            break;
                        }

                        this.Invoke((MethodInvoker)delegate
                        {
                            if (filteredDataTable == null)
                            {
                                filteredDataTable = batchTable.Copy();
                                gridControl1.DataSource = filteredDataTable;
                                SplashScreenManager.CloseForm(false);
                            }
                            else
                            {
                                filteredDataTable.Merge(batchTable);
                            }
                        });

                        offset += batchSize;
                    }
                }, cancellationToken);
            }
            catch (TaskCanceledException)
            {
                // Cleanup resources 
                if (filteredDataTable != null)
                {
                    filteredDataTable.Dispose(); // Dispose of the DataTable
                    filteredDataTable = null;    // Set to null to release reference
                }
                Console.WriteLine("Filtering was canceled");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
                ExceptionLogger.LogException(ex, nameof(frmLookUp), "LookUp", "ApplyFilters", new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
            }
            finally
            {
                isFiltering = false;
            }
        }


        private async Task<DataTable> LoadSearchBatchFromDatabase(string query, int offset, int batchSize, string orderByColumnName, CancellationToken cancellationToken)
        {
            DataTable dataTable = new DataTable();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                // Construct the query with pagination and ORDER BY clause
                string batchQuery = $"{query} ORDER BY {orderByColumnName} OFFSET {offset} ROWS FETCH NEXT {batchSize} ROWS ONLY";

                using (SqlDataAdapter dataAdapter = new SqlDataAdapter(batchQuery, connection))
                {
                    await Task.Run(() =>
                    {
                        // Check if cancellation has been requested
                        if (cancellationTokenSource.Token.IsCancellationRequested)
                        {
                            // Handle cancellation
                            // For example, throw an OperationCanceledException
                            dataTable.Dispose();
                            return;
                        }
                        // Fill the DataTable with data from the database
                        dataAdapter.Fill(dataTable);
                    }, cancellationToken);
                }
            }

            return dataTable;
        }


        private async void txtSearchField_TextChanged(object sender, EventArgs e)
        {

            gridView1.CustomDrawEmptyForeground -= gridView_CustomDrawEmptyForeground;
            if (string.IsNullOrEmpty(txtSearchField.Text))
            {
                gridControl1.DataSource = null;
                filteredDataTable = null;
                CancelPreviousOperation();
                cancellationTokenSource = new CancellationTokenSource();
                await LoadDataIntoGrid(cancellationTokenSource.Token);
            }
            else
            {
                CancelPreviousOperation();
                // Start or restart the timer when text changes
                searchDelayTimer.Stop();
                searchDelayTimer.Start();
            }
        }

        private void searchByColumn_SelectedIndexChanged_1(object sender, EventArgs e)
        {
         
            if (isPreview)
                LoadPreviewDataIntoGrid();
            else
            {
                CancelPreviousOperation();
                // Start or restart the timer when text changes
                searchDelayTimer.Stop();
                searchDelayTimer.Start();
            }

        }
        private async void SearchDelayTimer_Tick(object sender, EventArgs e)
        {
            // Stop the timer
            searchDelayTimer.Stop();


            if (isPreview)
            {
                LoadPreviewDataIntoGrid();
            }
            else
            {
                cancellationTokenSource = new CancellationTokenSource();
                await ApplyFilters();
            }

        }

        private void CancelPreviousOperation()
        {
            if (cancellationTokenSource != null)
            {
                cancellationTokenSource.Cancel(); // Cancel the previous operation
            }

            if (filteredDataTable != null)
            {
                filteredDataTable.Dispose(); // Dispose of the DataTable
                filteredDataTable = null;    // Set to null to release reference
            }
            if (dt != null)
            {
                dt.Dispose(); // Dispose of the DataTable
                dt = null;    // Set to null to release reference
            }
        }
        private async void btnSearch_Click(object sender, EventArgs e)
        {
            if (isPreview)
            {
                // Assuming LoadPreviewDataIntoGrid() is an asynchronous method
                LoadPreviewDataIntoGrid();
            }
            else
            {
                // Assuming ApplyFilters(dt) is an asynchronous method
                await ApplyFilters();
            }
        }


        // Add the LoadPreviewDataIntoGrid method to load preview data
        private void LoadPreviewDataIntoGrid()
        {
            try
            {
                // Create a SqlConnection
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Create a SqlDataAdapter to fetch data from the database using the provided query
                    SqlDataAdapter dataAdapter = new SqlDataAdapter(query, connection);

                    // Create a DataTable to hold the data
                    DataTable dataTable = new DataTable();

                    // Fill the DataTable with data from the database
                    dataAdapter.Fill(dataTable);

                    // Bind the DataTable to the gridControl1
                    gridControl1.DataSource = dataTable;

                    // Disable editing in the GridView
                    gridView1.OptionsBehavior.Editable = false;

                    //handle searching 
                    ApplyFilters();


                }
            }

            catch (Exception ex)
            {
                // Handle exceptions, e.g., display an error message
                MessageBox.Show($"Error: {ex.Message}");
                Console.Error.WriteLine(ex.ToString());
            }
        }


        // Add the ApplyStyling method to apply styling properties
        private void ApplyStyling(string fontName, string fontStyle, float fontSize, Color foreColor, int sortType, string orderByColumnName)
        {
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

                        foreach (var entry in fieldNameColumnMap)
                        {
                            string columnName = entry.Value; // Get the column name from the map

                            // Retrieve the cell value using the column name
                            object value = gridView1.GetRowCellValue(selectedRowHandle, columnName);

                            // Add the fieldName and its corresponding value to the selectedData dictionary
                            selectedData.Add(entry.Key, value);
                        }
                        this.Close();
                        // Invoke the DataSelected event with the selected data
                        DataSelected?.Invoke(selectedData);
                       
                    }
                }
            }
            catch (Exception ex)
            {
                // Display an error message to the user
                MessageBox.Show("An error occurred. Please contact your administrator.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // Log the exception or take further action as needed
                Console.WriteLine("Error: " + ex.Message);
            }
        }



        //private void ApplyFilters(DataTable dataTable)
        //{
        //    try
        //    {
        //        // Handle searching 
        //        if (!string.IsNullOrEmpty(txtSearchField.Text))
        //        {
        //            if (!string.IsNullOrEmpty(selectedColumnName) && selectedColumnName != "SearchField")
        //            {
        //                // Escape the column name to handle spaces or special characters
        //                string escapedColumnName = $"[{selectedColumnName}]";

        //                // Check if the selected column is a decimal type
        //                if (dataTable.Columns[selectedColumnName].DataType == typeof(decimal))
        //                {
        //                    // Convert the decimal column to string and then apply the filter
        //                    string filterExpression = $"CONVERT({escapedColumnName}, 'System.String') LIKE '%{txtSearchField.Text}%'";
        //                    dataTable.DefaultView.RowFilter = filterExpression;
        //                }
        //                else
        //                {
        //                    // Apply filter to non-decimal columns
        //                    dataTable.DefaultView.RowFilter = $"{escapedColumnName} LIKE '%{txtSearchField.Text}%'";
        //                }
        //            }
        //            else
        //            {
        //                // Apply filter based on txtSearchField only
        //                string combinedFilter = string.Empty;

        //                foreach (var fieldName in fieldTypes.Keys)
        //                {
        //                    if (dataTable.Columns.Contains(fieldName))
        //                    {
        //                        DataColumn column = dataTable.Columns[fieldName];
        //                        if (column.DataType != typeof(decimal) && column.DataType != typeof(int))
        //                        {
        //                            string escapedFieldName = $"[{fieldName}]";
        //                            if (!string.IsNullOrEmpty(combinedFilter))
        //                            {
        //                                combinedFilter += " OR ";
        //                            }
        //                            combinedFilter += $"{escapedFieldName} LIKE '%{txtSearchField.Text}%'";
        //                        }
        //                        else if (column.DataType == typeof(int))
        //                        {
        //                            // Convert the integer column to string and then apply the filter
        //                            string escapedFieldName = $"CONVERT([{fieldName}], 'System.String')";
        //                            if (!string.IsNullOrEmpty(combinedFilter))
        //                            {
        //                                combinedFilter += " OR ";
        //                            }
        //                            combinedFilter += $"{escapedFieldName} LIKE '%{txtSearchField.Text}%'";
        //                        }
        //                    }
        //                }

        //                dataTable.DefaultView.RowFilter = combinedFilter;
        //            }
        //        }
        //        else
        //        {
        //            // Clear any existing filters
        //            dataTable.DefaultView.RowFilter = string.Empty;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // Handle exceptions, e.g., display an error message
        //        MessageBox.Show($"Error applying filters: {ex.Message}");
        //        Console.Error.WriteLine(ex.ToString());
        //    }
        //}
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


        public static void OpenLookupForm(string code, DataSelectedHandler dataSelectedHandler, Form callingForm, bool autoWidth)
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connection_string"].ConnectionString;
            string lookupName = "";
            string tableName = "";
            string lookupSettingQuery = $"SELECT * FROM [LookupSetting_Tbl] WHERE [Code] = '{code}'";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand lookupSettingCommand = new SqlCommand(lookupSettingQuery, connection))
                    using (SqlDataReader lookupSettingReader = lookupSettingCommand.ExecuteReader())
                    {
                        if (lookupSettingReader.Read())
                        {
                            lookupName = lookupSettingReader["Lookup Name"].ToString();
                            tableName = lookupSettingReader["Table Name"].ToString();
                        }
                    }
                }

                string columnWidthQuery = $"SELECT [Field Name], [Column Name] FROM [LookupSetting_Detail_Tbl] WHERE [Code] = '{code}' ORDER BY [Column Index]";

                Dictionary<string, Type> fieldTypes = new Dictionary<string, Type>();

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(columnWidthQuery, connection))
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        List<string> fieldNames = new List<string>();
                        Dictionary<string, string> fieldNameColumnMap = new Dictionary<string, string>();
                        bool isSearchField = false; // Initialize isSearchField to false

                        while (reader.Read())
                        {
                            string fieldName = reader["Field Name"].ToString();
                            string columnName = reader["Column Name"].ToString();
                            string fieldNameWithoutBrackets = fieldName.Trim('[', ']');



                            //future delete
                            if (fieldName.ToLower().Contains(" as "))
                            {
                                // Skip this iteration if the fieldName contains " as "
                                continue;
                            }



                            // Check if the field name contains the keyword "AS"
                            //if (fieldName.ToLower().Contains(" as "))
                            //{

                            //    var processedNames = ProcessFieldName(fieldName);
                            //    if (!fieldTypes.ContainsKey(processedNames.withoutBrackets))
                            //    {
                            //        fieldTypes.Add(processedNames.withoutBrackets, typeof(string));
                            //        fieldNames.Add($"{processedNames.withBrackets}");
                            //        fieldNameColumnMap.Add(processedNames.withBrackets, columnName);
                            //    }

                            //}
                            else if (fieldName != "[SearchField]")
                            {
                                if (!fieldTypes.ContainsKey(fieldNameWithoutBrackets))
                                {
                                    fieldTypes.Add(fieldNameWithoutBrackets, typeof(string));
                                    fieldNames.Add($"{fieldName}");
                                    fieldNameColumnMap.Add(fieldName, columnName);
                                }

                            }

                            if (fieldName == "[SearchField]")
                            {
                                isSearchField = true; // Set isSearchField to true if fieldName is [SearchField]
                            }
                        }

                        List<string> selectedColumns = new List<string>();
                        foreach (string fieldName in fieldNames)
                        {

                            string columnName = fieldNameColumnMap[fieldName];
                            selectedColumns.Add($"{fieldName} AS '{columnName}'");

                        }

                        string lookupQuery = $"SELECT {string.Join(", ", selectedColumns)} FROM [{tableName}]";

                        frmLookUp lookupForm = new frmLookUp();
                        lookupForm.InitializedLookup(lookupQuery, fieldTypes, lookupName, code, fieldNameColumnMap, autoWidth, isSearchField); // Pass isSearchField value
                        lookupForm.DataSelected += dataSelectedHandler;
                        lookupForm.StartPosition = FormStartPosition.CenterScreen;
                        lookupForm.Show();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
                Console.WriteLine(ex.ToString());
            }
        }




        private void clearBtn_Click(object sender, EventArgs e)
        {
            clearFilters();
            txtSearchField.Clear();
        }

        private void clearFilters()
        {
            // Clear any existing filters
            DataTable dataTable = (DataTable)gridControl1.DataSource;
            if (dataTable != null)
            {
                dataTable.DefaultView.RowFilter = string.Empty;
            }
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
            lookUpSettings.ShowDialog();

        }

        private void gridView1_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            // Set the background color for alternate rows
            if (e.RowHandle % 2 == 1)
            {
                e.Appearance.BackColor = Color.Lavender;
            }
        }
        private void GridView1_EndSorting(object sender, EventArgs e)
        {
            // Set the focus row to the first row after sorting
            gridView1.FocusedRowHandle = 0;
        }

        private void ApplyLookupSettings()
        {

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string lookupSettingQuery = $"SELECT [Order By], [Sort Type], [FontName], [FontStyle], [ForeColor], [FontSize] FROM [LookupSetting_Tbl] WHERE [Lookup Name] = '{lookUpName}'";

                using (SqlCommand lookupSettingCommand = new SqlCommand(lookupSettingQuery, connection))
                using (SqlDataReader lookupSettingReader = lookupSettingCommand.ExecuteReader())
                {
                    if (lookupSettingReader.Read())
                    {
                        // Retrieve sorting information
                        string orderByColumnName = lookupSettingReader["Order By"].ToString();
                        int sortType = Convert.ToInt32(lookupSettingReader["Sort Type"]);

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
                        if (gridControl1.IsHandleCreated)
                        {
                            gridControl1.Invoke((Action)(() =>
                            {
                                gridView1.Appearance.Row.Font = new Font(fontName, fontSize, FontStyleFromString(fontStyle));
                                gridView1.Appearance.HeaderPanel.Font = new Font(fontName, fontSize, FontStyleFromString(fontStyle));
                                gridView1.Appearance.FooterPanel.Font = new Font(fontName, fontSize, FontStyleFromString(fontStyle));
                                gridView1.Appearance.Row.ForeColor = foreColor; // Set font color for rows
                            }));
                        }

                        // Apply sorting if applicable
                        if (!string.IsNullOrEmpty(orderByColumnName))
                        {
                            if (gridControl1.IsHandleCreated)
                            {
                                // Find the column by name
                                gridControl1.Invoke((Action)(() =>
                                {
                                    DevExpress.XtraGrid.Columns.GridColumn columnToSort = gridView1.Columns.ColumnByFieldName(orderByColumnName.Trim('[', ']'));

                                    if (columnToSort != null)
                                    {
                                        // Clear existing sort info
                                        gridView1.ClearSorting();

                                        // Set the sort order
                                        columnToSort.SortOrder = (sortType == 0) ? DevExpress.Data.ColumnSortOrder.Ascending : DevExpress.Data.ColumnSortOrder.Descending;
                                    }
                                }));
                            }
                        }
                    }
                }
            }

        }


        private void ApplyColumnInfo()
        {

            if (InvokeRequired)
            {
                // Invoke on the main UI thread
                Invoke((Action)ApplyColumnInfo);
                return;
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string columnInfoQuery = $"SELECT [Field Name], [Column Width], [Column Length], [Col Alignment] FROM [LookupSetting_Detail_Tbl] WHERE [Lookup Name] = '{lookUpName}'";

                using (SqlCommand columnInfoCommand = new SqlCommand(columnInfoQuery, connection))
                using (SqlDataReader columnInfoReader = columnInfoCommand.ExecuteReader())
                {
                    while (columnInfoReader.Read())
                    {
                        string fieldName = columnInfoReader["Field Name"].ToString().Trim('[', ']');
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


    }
}
