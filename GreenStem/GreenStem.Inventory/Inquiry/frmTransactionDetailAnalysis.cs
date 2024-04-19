
using DevExpress.CodeParser;
using DevExpress.Utils.Extensions;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraSplashScreen;
using DevExpress.XtraVerticalGrid;
using DevExpress.XtraVerticalGrid.Rows;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using static DevExpress.Utils.Drawing.Helpers.NativeMethods;
using static DevExpress.XtraPrinting.Native.ExportOptionsPropertiesNames;
using GreenStem.ClassModules;
using GreenStem.LookUp;
using System.Windows.Forms;
using System.Diagnostics;
using System.Configuration;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraExport.Helpers;
using System.Threading;
using DevExpress.XtraGrid.Columns;
using System.ComponentModel.DataAnnotations;
using DevExpress.XtraPrinting;
using System.Runtime.CompilerServices;
using System.Collections;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid;
using DevExpress.Utils.Win;
using DevExpress.XtraEditors.Popup;

namespace GreenStem.Inventory
{
    public partial class frmTransactionDetailAnalysis : DevExpress.XtraBars.Ribbon.RibbonForm
    {

        private string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connection_string"].ConnectionString;
        private string selectedCode;
        private List<KeyValuePair<string, int>> fieldColumnLengthList;
        private DataTable stockTable = new DataTable();
        private bool isLookUpSearch = false;

        private System.Windows.Forms.Timer searchDelayTimer;
        private bool rightClickPaste = false;
        private bool isClosePopup = false;
        private DateTime lastExecutionTime = DateTime.MinValue;
        private System.Windows.Forms.Timer resetProcessingFlagTimer = new System.Windows.Forms.Timer();
        private bool isEnterClicked = false;
        private bool pasteOperation = false;
        bool alreadyHandledPaste = false;
        private string previousText = "";
        private System.Threading.Timer debounceTimer;
        int NUM = 1;
        private bool enterKeyProcessed = false;
        private int previousTextLength = 0;
        bool isPasting = false;
        private bool isProgrammaticChange = false;
        private CancellationTokenSource cts;

        private bool resultSearchEmpty = false;
        public frmTransactionDetailAnalysis()
        {
            InitializeComponent();
            LoadStockLocationDataIntoGrid(null);
            LoadInterchangeDataIntoGrid(null);
            LoadOEMDataIntoGrid(null);
            txtFromDate.DateTime = new DateTime(2000, 1, 1); // Set txtFromDate to January 1, 2000
            txtToDate.DateTime = DateTime.Today;


            //Add Column for stock other infor
            DataTable stockOtherInfor = new DataTable();
            AddStockOtherInfoColumns(stockOtherInfor);
            gridControl4.DataSource = stockOtherInfor;
            gridView4.OptionsView.ColumnAutoWidth = true;
            DataTable stockRequests = new DataTable();


            // Add columns for stock requests
            AddStockRequestColumns(stockRequests);
            gridControl5.DataSource = stockRequests;
            gridView5.OptionsView.ColumnAutoWidth = true;


            //intialize with custom gridivew color
            DevExpress.XtraGrid.Views.Grid.GridView existingGridView = gvStockDetailAnalysis;
            gridViewCustomizer.ApplyCustomizations(existingGridView, false, false);




            InitializationFieldName();
            // initialize stock code pop up suggestion asynchronously
            InitializedStockCodeAsync();
           gridView7.CustomDrawEmptyForeground += gridView_CustomDrawEmptyForeground;

            // search delay timer
            searchDelayTimer = new System.Windows.Forms.Timer();
            searchDelayTimer.Interval = 500; // Set the interval (milliseconds) to wait after text stops changing
            searchDelayTimer.Tick += SearchDelayTimer_Tick;


            txtStockCode.Focus();
            txtStockCode.KeyDown += txtStockCode_KeyDown;

            txtStockCode.Popup += TxtStockCode_Popup;


            //txtStockCode.PreviewKeyDown += (sender, e) =>
            //{
            //    if (e.Control && e.KeyCode == Keys.V)
            //    {
            //        // Set the flag if Ctrl+V is pressed
            //        pasteOperation = true;
            //        HandleTextChanged();
            //    }
            //};
            //txtStockCode.MaskBox.TextChanged += (sender, e) =>
            //{

            //    //var currentText = txtStockCode.Text;
            //    //if (String.IsNullOrEmpty(currentText))
            //    //{
            //    //    return;
            //    //}

            //    if (!isEnterClicked)
            //            {
            //        Console.WriteLine(" textchanged " +txtStockCode.Text.ToString());
            //                HandleTextChanged();
            //        txtStockCode.SelectionStart = txtStockCode.Text.Length;
            //    }
            //            else
            //            {
            //                isEnterClicked = false;
            //            }
            //     
            //};
            txtStockCode.MaskBox.PreviewKeyDown += (sender, e) =>
            {
                // Check if Ctrl+V is pressed
                if (e.Control && e.KeyCode == Keys.V)
                {
                    isPasting = true;
                    //HandleTextChanged();
                }
                // Check if Down or Up arrow key is pressed
                else if (e.KeyCode == Keys.Down || e.KeyCode == Keys.Up)
                {
                    isProgrammaticChange = true;
                }
            };
            txtStockCode.MaskBox.TextChanged += (sender, e) =>
            {
                resultSearchEmpty = false;
                if (pasteOperation)
                {
                    pasteOperation = false;
                    return;
                }
                if (isProgrammaticChange)
                {
                    // Reset the flag and exit the event handler early
                    isProgrammaticChange = false;
                    return;
                }
                // Check if the current clipboard content is equal to the text in the control
                // and if the text length is greater than the previous text length (indicating a paste operation)
                if (Clipboard.GetText() == txtStockCode.Text && txtStockCode.Text.Length > previousTextLength)
                {
                    // Handle the paste event
                    pasteOperation = true;
                string stockCode = txtStockCode.Text;   
                    txtStockCode.SelectionStart = txtStockCode.Text.Length;
                    isProgrammaticChange = true;
                    txtStockCode.ShowPopup();
                    PerformCustomSearchAsync();
                    isProgrammaticChange = true;
                    txtStockCode.Text = stockCode;
                   
                }
                else
                {
                    if (!pasteOperation)
                    { 

                        // Handle normal text change
                        HandleTextChanged();
                        txtStockCode.SelectionStart = txtStockCode.Text.Length;
                    }
                    else
                    {
                        pasteOperation = false;
                        //add a break here, dont want continue other operation anymore
                    }
                }

                // Update previousTextLength for the next TextChanged event
                previousTextLength = txtStockCode.Text.Length;
            };


            //txtStockCode.MouseDown += (sender, e) =>
            //{
            //    if (e.Button == MouseButtons.Right)
            //    {
            //        // Set the flag if right-click paste is detected
            //        pasteOperation = Clipboard.ContainsText();
            //    }
            //};
            MinimumSize = new System.Drawing.Size(1024, 768);



        }

        private void gridView_CustomDrawEmptyForeground(object sender, DevExpress.XtraGrid.Views.Base.CustomDrawEventArgs e)
        {
            GridView view = sender as GridView;
            if (resultSearchEmpty)
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
        private void HandleTextChanged()
        {
            txtStockCode.ShowPopup();
            Console.WriteLine("handle text " + txtStockCode.Text.ToString());
            if (String.IsNullOrEmpty(txtStockCode.Text))
            {
                stockTable.Rows.Clear();
                InitializedStockCodeAsync(); // Reload data asynchronously
                txtStockCode.ShowPopup();
            }
            else
            {


               

                searchDelayTimer.Stop();
                searchDelayTimer.Start();
            }
        }

        private void SearchDelayTimer_Tick(object sender, EventArgs e)
        {
            searchDelayTimer.Stop();


            PerformCustomSearchAsync();
        }

        private async void PerformCustomSearchAsync()
        {
            if (!isEnterClicked) // Check if Enter key was pressed
            {

               
                Console.WriteLine("Perform custom " + txtStockCode.Text.ToString());


                string searchText = txtStockCode.Text.Trim().ToUpper();
                Console.WriteLine("Searching " + NUM + searchText);
                NUM++;
                if (String.IsNullOrEmpty(searchText))
                {
                    return;
                }

                DevExpress.XtraGrid.Views.Grid.GridView gridView = txtStockCode.Properties.View as DevExpress.XtraGrid.Views.Grid.GridView;
                gridView.ShowLoadingPanel();
                stockTable.Rows.Clear();
                try
                {
                    string fieldNames = string.Join(", ", fieldColumnLengthList.Select(pair => $"{pair.Key}"));

                    DataTable searchResultsTable = new DataTable();
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        await connection.OpenAsync(); // Open the connection asynchronously

                        string stockQuery = $"SELECT {fieldNames} FROM [Stock] WHERE [Stock].[Stock Code] LIKE '{searchText}%' ORDER BY [Stock].[Stock Code]";
                        SqlDataAdapter adapter = new SqlDataAdapter(stockQuery, connection);

                        adapter.Fill(searchResultsTable);

                        // Filter out rows from searchResultsTable that already exist in stockTable based on "Stock Code"
                        List<string> existingStockCodes = stockTable.AsEnumerable().Select(r => r.Field<string>("Stock Code")).ToList();
                        IEnumerable<DataRow> filteredRows = searchResultsTable.AsEnumerable()
                            .Where(r => !existingStockCodes.Contains(r.Field<string>("Stock Code")));


                        if (searchResultsTable.Rows.Count <= 0)
                        {
                            resultSearchEmpty = true;
                        }
                        if (filteredRows.Any()) // Check if there are rows to copy
                        {
                           DataTable filteredResultsTable = filteredRows.CopyToDataTable();
                           
                           
                            // Merge filteredResultsTable into stockTable
                            stockTable.Merge(filteredResultsTable);
                        }

                        // Introduce a delay before hiding the loading panel
                        await Task.Delay(200);

                        gridView.HideLoadingPanel();
                    }

                    // Update GridLookUpEdit properties with search results
                   // Show dropdown with search results
                                              // txtStockCode.Text = searchText; // Set the text to the original search text
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred while performing stock code search: " + ex.Message);
                    // Handle the error as needed
                    gridView.HideLoadingPanel();
                }
            }
            else
            {
                isEnterClicked = false; // Reset the flag
                return;
            }
        }

        private void gridView7_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {

            ProcessSelectedStockCode(null);
            e.Handled = true; // Mark the event as handled to prevent further processing

        }
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && !enterKeyProcessed)
            {
                enterKeyProcessed = true;
                Console.WriteLine("OnKeyDown Enter key pressed");
                ProcessSelectedStockCode(null);

                // Start a timer to reset the flag after a short delay
                var timer = new System.Windows.Forms.Timer { Interval = 300 }; // 300 milliseconds delay
                timer.Tick += (s, args) =>
                {
                    enterKeyProcessed = false; // Reset the flag
                    timer.Stop(); // Stop the timer
                };
                timer.Start();

                e.Handled = true; // Mark the event as handled to prevent further processing
            }
        }
        private void ProcessSelectedStockCode(string filterSearch)
        {
            isEnterClicked = true;
            string selectedStockCode = !string.IsNullOrEmpty(filterSearch) ? filterSearch :
                txtStockCode.Properties.View.GetFocusedRowCellValue("Stock Code")?.ToString();

            if (!string.IsNullOrEmpty(selectedStockCode))
            {
                Dictionary<string, object> selectedData = new Dictionary<string, object>();
                selectedData["[Stock Code]"] = selectedStockCode;
                isProgrammaticChange = true;
                txtStockCode.ClosePopup();
                HandleDataSelectedEvent(selectedData);
            }
        }

        private void InitializationFieldName()
        {
            string query = "SELECT [Field Name], [Column Length] FROM [SDG_Setting_Detail_Tbl] " + modPublicVariable.ReadLockType + " WHERE Code = '10003'";

            fieldColumnLengthList = new List<KeyValuePair<string, int>>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open(); // Open the connection asynchronously
                SqlCommand command = new SqlCommand(query, connection);
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    string fieldName = reader["Field Name"].ToString();
                    int columnLength = Convert.ToInt32(reader["Column Length"]);
                    fieldColumnLengthList.Add(new KeyValuePair<string, int>(fieldName, columnLength));
                }
            }
        }
        private void InitializedStockCodeAsync()
        {

            DevExpress.XtraGrid.Views.Grid.GridView gridView = txtStockCode.Properties.View as DevExpress.XtraGrid.Views.Grid.GridView;
            gridView.ShowLoadingPanel();


            string fieldNames = string.Join(", ", fieldColumnLengthList.Select(pair => $"{pair.Key}"));

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open(); // Open the connection asynchronously
                string stockQuery = $"SELECT TOP 100 {fieldNames} FROM [Stock]";
                SqlDataAdapter adapter = new SqlDataAdapter(stockQuery, connection);

                adapter.Fill(stockTable); // Fill the DataTable asynchronously
            }


            txtStockCode.Properties.DataSource = stockTable;
            txtStockCode.Properties.DisplayMember = "Stock Code"; // Assuming "Stock Code" is one of the columns in the DataTable
            txtStockCode.Properties.ValueMember = "Stock Code";
            gridView = txtStockCode.Properties.View as DevExpress.XtraGrid.Views.Grid.GridView;
            gridView.BestFitColumns();
            // txtStockCode.EditValueChanged += txtStockCode_EditValueChanged;
            gridView.HideLoadingPanel();
            gridView7.RowCellClick += gridView7_RowCellClick;



        }


        //private void txtStockCode_EditValueChanged(object sender, EventArgs e)
        //{

        //    if (isLookUpSearch)
        //    {

        //        return;

        //    }
        //    // txtStockCode.ClosePopup();

        //    if (selectionFromPopup)
        //    {
        //        // Reset the flag
        //        selectionFromPopup = false;

        //        ProcessSelectedStockCode();
        //    }
        //}
        private void txtStockCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && !enterKeyProcessed)
            {
                enterKeyProcessed = true;
                Console.WriteLine("txtStockCode_KeyDown Enter key pressed");
                string filterSearch = txtStockCode.Text;
                ProcessSelectedStockCode(filterSearch);

                // Start a timer to reset the flag after a short delay
                var timer = new System.Windows.Forms.Timer { Interval = 300 }; // 300 milliseconds delay
                timer.Tick += (s, args) =>
                {
                    enterKeyProcessed = false; // Reset the flag
                    timer.Stop(); // Stop the timer
                };
                timer.Start();

                e.Handled = true; // Mark the event as handled to prevent further processing
            }
        }
        private void TxtStockCode_Popup(object sender, EventArgs e)
        {

            DevExpress.XtraGrid.Views.Grid.GridView gridView = txtStockCode.Properties.View as DevExpress.XtraGrid.Views.Grid.GridView;

            GridLookUpEdit edit = sender as GridLookUpEdit;
            PopupGridLookUpEditForm form = (edit as IPopupControl).PopupWindow as PopupGridLookUpEditForm;
            form.KeyDown += OnKeyDown;
            if (gridView != null)
            {
                // Step 5: Set the column width of the popup grid
                foreach (KeyValuePair<string, int> pair in fieldColumnLengthList)
                {


                    string fieldName = pair.Key.Trim('[', ']');
                    int columnLength = pair.Value;
                    // Check if the column exists in the DataTable
                    if (stockTable.Columns.Contains(fieldName))
                    {
                        // Set column width based on column length
                        int columnWidth = columnLength * 10;
                        // Set the column width of the DataTable
                        gridView.Columns[fieldName].Width = columnWidth;
                    }
                    else
                    {
                        Console.WriteLine($"Column '{fieldName}' does not exist in the DataTable.");
                    }
                }

            }


        }


        private void LoadStockLocationDataIntoGrid(string stockCode)
        {
            try
            {
                // Check if stockCode is null or empty
                if (string.IsNullOrEmpty(stockCode))
                {
                    // Load vGridControl with row headers only
                    LoadRowHeadersOnly();
                    return;
                }

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Construct the SQL query to select desired columns from Stock_Location table
                    string query = $"SELECT [Location Code] AS Loc, CAST([Current Quantity] AS INT) AS 'INV Qty', CAST([DO Quantity] AS INT) AS 'DO Qty' FROM [Stock_Location] WHERE [Stock Code] = @StockCode";

                    // Create a SqlCommand to execute the query
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Add parameters to prevent SQL injection
                        command.Parameters.AddWithValue("@StockCode", stockCode);

                        // Create a SqlDataAdapter to fetch data from the database
                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            // Create a DataTable to hold the retrieved data
                            DataTable stockLocationData = new DataTable();

                            // Fill the DataTable with data from the database
                            adapter.Fill(stockLocationData);

                            // Set the data source for vGridControl1
                            vGridControl1.DataSource = stockLocationData;

                            // Enable horizontal scrolling and other configurations
                            ConfigureVGridControl();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        private void LoadRowHeadersOnly()
        {
            // Set the data source for vGridControl1 to null to clear any existing data
            vGridControl1.DataSource = null;

            // Add empty DataTable to create columns and row headers
            DataTable emptyData = new DataTable();
            emptyData.Columns.Add("Loc");
            emptyData.Columns.Add("INV Qty");
            emptyData.Columns.Add("DO Qty");
            vGridControl1.DataSource = emptyData;


        }

        private void ConfigureVGridControl()
        {
            // Enable horizontal scrolling and other configurations
            vGridControl1.OptionsView.AutoScaleBands = true;
            vGridControl1.OptionsView.MaxRowAutoHeight = 50;
            vGridControl1.ForceInitialize();
            vGridControl1.RowHeaderWidth = 50;
            vGridControl1.Appearance.RowHeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            vGridControl1.CustomDrawRowHeaderCell += VGridControl1_CustomDrawRowHeaderCell;

            // Loop through each row in vGridControl1 and set the alignment for all cells to the left
            foreach (DevExpress.XtraVerticalGrid.Rows.BaseRow row in vGridControl1.Rows)
            {
                row.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            }
        }
        private void VGridControl1_CustomDrawRowHeaderCell(object sender, DevExpress.XtraVerticalGrid.Events.CustomDrawRowHeaderCellEventArgs e)
        {

            VGridControl grid = sender as VGridControl;
            object gri1 = grid;
            if (grid.IsCategoryRow(e.Row))
                return;
            Rectangle captionBounds = e.Bounds;

            captionBounds.X = 0;
            captionBounds.Width = e.Bounds.Right - captionBounds.X;
            e.Graphics.FillRectangle(Brushes.Lavender, captionBounds);
            captionBounds.Offset(4, 1);
            e.Cache.DrawString(e.Caption, e.Appearance.Font, e.Appearance.GetForeBrush(e.Cache), captionBounds, StringFormat.GenericDefault);
            e.Handled = true;
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            frmLookUp.OpenLookupForm("10003", HandleDataSelectedEvent, this, false);
        }

        private async void HandleDataSelectedEvent(Dictionary<string, object> selectedData)
        {

            isLookUpSearch = true;

            try
            {
                string selectedStockCode = selectedData["[Stock Code]"].ToString();
                this.selectedCode = selectedStockCode;
                isProgrammaticChange = true;
                txtStockCode.Text = selectedStockCode;
                // Load data for selected stock code
                LoadDataForSelectedCode(selectedStockCode);

                // Load stock location data into grid
                LoadStockLocationDataIntoGrid(selectedStockCode);

                // Load interchange data into grid
                LoadInterchangeDataIntoGrid(selectedStockCode);

                // Load OEM data into grid
                LoadOEMDataIntoGrid(selectedStockCode);

                // Adjust layout for vGridControl1 (assuming this is a specific grid control)
                ComplexBestFit(vGridControl1);

                // Load stock detail into grid for the selected stock code and date range
                await LoadStockDetailIntoGrid(selectedStockCode, txtFromDate.DateTime.Date, txtToDate.DateTime.Date);

                // Load OEMS data into grid
                LoadOEMSDataIntoGrid(selectedStockCode);



            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }
            finally
            {
                isLookUpSearch = false;
            }

        }
        private void LoadOEMSDataIntoGrid(string selectedStockCode)
        {
            try
            {
                DataTable oemData = new DataTable();



                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Fetch the OEM code associated with the selected stock code
                    string oemCodeQuery = "SELECT [OEM Code] FROM [STOCK_OEM] WHERE [Stock Code] = @SelectedStockCode";
                    SqlCommand oemCodeCommand = new SqlCommand(oemCodeQuery, connection);
                    oemCodeCommand.Parameters.AddWithValue("@SelectedStockCode", selectedStockCode);
                    string oemCode = Convert.ToString(oemCodeCommand.ExecuteScalar());

                    // Add columns for OEM data
                    oemData.Columns.Add(oemCode, typeof(string));
                    oemData.Columns.Add("Nett", typeof(int));
                    // Fetch all stock codes with the same OEM code except the selected stock code
                    string oemDataQuery = @"
                    SELECT [Stock Code], COUNT(*) AS Quantity 
                    FROM [STOCK_OEM] 
                    WHERE [OEM Code] = @OEMCode AND [Stock Code] != @SelectedStockCode
                    GROUP BY [Stock Code]";
                    SqlCommand oemDataCommand = new SqlCommand(oemDataQuery, connection);
                    oemDataCommand.Parameters.AddWithValue("@OEMCode", oemCode);
                    oemDataCommand.Parameters.AddWithValue("@SelectedStockCode", selectedStockCode);

                    using (SqlDataReader reader = oemDataCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string stockCode = Convert.ToString(reader["Stock Code"]);
                            int quantity = Convert.ToInt32(reader["Quantity"]);
                            oemData.Rows.Add(stockCode, quantity);
                        }
                    }
                }

                // Bind the DataTable to gridControl6
                gridControl6.DataSource = oemData;
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while loading OEM data: " + ex.Message);
                ExceptionLogger.LogException(ex, nameof(frmTransactionDetailAnalysis), "Inventory", "LoadOEMSDataIntoGrid", new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
            }
        }
        //private async Task LoadStockDetailIntoGrid(string selectedStockCode, DateTime fromDate, DateTime toDate)
        //{
        //    if (selectedStockCode == null)
        //    {
        //        return;
        //    }
        //    Stopwatch stopwatch = Stopwatch.StartNew();
        //    gvStockDetailAnalysis.ShowLoadingPanel();

        //    try
        //    {
        //        // Define the maximum number of concurrent tasks
        //        int maxConcurrentTasks = Environment.ProcessorCount;
        //        using (var semaphoreSlim = new SemaphoreSlim(maxConcurrentTasks))
        //        {
        //            Stopwatch stopwatch1 = Stopwatch.StartNew();
        //            List<TransactionDetail> transactionDetails = await GetStockTransactions(selectedStockCode, fromDate, toDate);
        //            double elapsedTimeInSeconds = stopwatch1.ElapsedMilliseconds / 1000.0;
        //            Console.WriteLine($"Total time for retrieving data: {elapsedTimeInSeconds:F2} seconds");

        //            // Bind the list to the grid with CostPrice as empty
        //            gridControl3.DataSource = transactionDetails;
        //            gvStockDetailAnalysis.Columns["CostPrice"].Visible = false;
        //            gvStockDetailAnalysis.HideLoadingPanel();

        //            // Process the CostPrice asynchronously with concurrency limit
        //            var tasks = transactionDetails.Select(async detail =>
        //            {
        //                await semaphoreSlim.WaitAsync();
        //                try
        //                {
        //                    await ProcessTransactionDetailAsync(detail);
        //                }
        //                finally
        //                {
        //                    semaphoreSlim.Release();
        //                }
        //            });

        //            await Task.WhenAll(tasks);

        //            gvStockDetailAnalysis.Columns["CostPrice"].Visible = true;
        //            // Refresh the grid to show the updated CostPrice values
        //            gridControl3.RefreshDataSource();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("An error occurred in LoadStockDetailIntoGrid: " + ex.Message);
        //    }
        //    finally
        //    {
        //        stopwatch.Stop();
        //        double elapsedTimeInSeconds = stopwatch.ElapsedMilliseconds / 1000.0;
        //        Console.WriteLine($"Total time taken: {elapsedTimeInSeconds:F2} seconds");
        //    }
        //}

        private async Task LoadStockDetailIntoGrid(string selectedStockCode, DateTime fromDate, DateTime toDate)
        {
            if (selectedStockCode == null)
            {
                return;
            }
            if ((DateTime.Now - lastExecutionTime).TotalSeconds < 1)
            {
                Console.WriteLine("LoadStockDetailIntoGrid is being called too frequently. Execution skipped.");
                return;
            }

            // Update the last execution time
            lastExecutionTime = DateTime.Now;

            cts = new CancellationTokenSource();
            Stopwatch stopwatch = Stopwatch.StartNew();
            gvStockDetailAnalysis.ShowLoadingPanel();

            try
            {
                DataTable transactionTable = await GetStockTransactions(selectedStockCode, fromDate, toDate, cts.Token);
                double elapsedTimeInSeconds = stopwatch.ElapsedMilliseconds / 1000.0;
                Console.WriteLine($"Total time for retrieving data: {elapsedTimeInSeconds:F2} seconds");

                // Bind the DataTable to the grid
                gridControl3.DataSource = transactionTable;
                gvStockDetailAnalysis.Columns["Cost / Price Loading"].Visible = true;
                gvStockDetailAnalysis.Columns["Cost / Price"].Visible = false;
                gvStockDetailAnalysis.HideLoadingPanel();
                SetColumnWidths();

                // Process Cost / Price Conversion asynchronously for each row in the DataTable
                List<Task> conversionTasks = new List<Task>();
                foreach (DataRow row in transactionTable.Rows)
                {
                    conversionTasks.Add(ProcessCostPriceConversionAsync(row, cts.Token));
                }

                await Task.WhenAll(conversionTasks);

                gvStockDetailAnalysis.Columns["Cost / Price Loading"].Visible = false;
                gvStockDetailAnalysis.Columns["Cost / Price"].Visible = true;
                // Refresh the grid to show the updated CostPrice values
                gridControl3.RefreshDataSource();
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred in LoadStockDetailIntoGrid: " + ex.Message);
                cts.Cancel(); // Cancel all pending asynchronous operations
                gvStockDetailAnalysis.HideLoadingPanel();
              
            }
            finally
            {
                stopwatch.Stop();
                double elapsedTimeInSeconds = stopwatch.ElapsedMilliseconds / 1000.0;
                Console.WriteLine($"Total time taken: {elapsedTimeInSeconds:F2} seconds");
                cts.Dispose(); // Dispose of the CancellationTokenSource
            }
        }

        private async Task ProcessCostPriceConversionAsync(DataRow row, CancellationToken cancellationToken)
        {
            try
            {
                // Get the original Cost / Price value as a string
                string costPriceStr = row["Cost / Price"].ToString();

                // Extract the numeric part of the value
                string numericPart = ExtractNumericPart(costPriceStr);

                // Convert the numeric part to a decimal
                decimal costPriceValue = Convert.ToDecimal(numericPart);

                // Determine the conversion function based on the last character ('A' or 'B')
                if (costPriceStr.EndsWith("A"))
                {
                    string costPriceConverted = await Udf_Stock_CostConvAsync(costPriceValue, cancellationToken);
                    row["Cost / Price"] = costPriceConverted;
                }
                else if (costPriceStr.EndsWith("B"))
                {
                    string costPriceConverted = await Udf_Stock_PriceConvAsync(costPriceValue, cancellationToken);
                    row["Cost / Price"] = costPriceConverted;
                }
                else
                {
                    // Handle unexpected case where no 'A' or 'B' suffix is found
                    Console.WriteLine("Unexpected format for Cost / Price value");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while processing Cost / Price: {ex.Message}");
                // Handle the exception as needed, e.g., log the error or set a default value
                row["Cost / Price"] = "Error"; // Set a default value in case of error
            }
        }

        private string ExtractNumericPart(string value)
        {
            // Trim the trailing character ('A' or 'B') from the value
            if (!string.IsNullOrEmpty(value) && (value.EndsWith("A") || value.EndsWith("B")))
            {
                return value.Substring(0, value.Length - 1); // Remove the last character
            }

            return value; // Return the original value if no trailing character is found
        }
        private async Task<string> Udf_Stock_CostConvAsync(decimal costPrice, CancellationToken cancellationToken)
        {
            string costPriceConv = "";

            try
            {
                // Create a new connection and command for this method
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync(cancellationToken); // Use cancellation token for opening connection

                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText = "SELECT dbo.Udf_Stock_CostConv(@CostPrice)";
                        command.Parameters.AddWithValue("@CostPrice", costPrice);

                        // Execute the command asynchronously with cancellation token
                        object result = await command.ExecuteScalarAsync(cancellationToken);

                        if (result != null && result != DBNull.Value)
                        {
                            costPriceConv = result.ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred in Udf_Stock_CostConvAsync: " + ex.Message);
                throw; // Rethrow the exception to propagate it back to the caller
            }

            return costPriceConv;
        }
        private async Task<string> Udf_Stock_PriceConvAsync(decimal price, CancellationToken cancellationToken)
        {
            string priceConv = "";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync(cancellationToken); // Use cancellation token for opening connection

                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText = "SELECT dbo.Udf_Stock_PriceConv(@Price)";
                        command.Parameters.AddWithValue("@Price", price);

                        // Execute the command asynchronously with cancellation token
                        object result = await command.ExecuteScalarAsync(cancellationToken);

                        if (result != null && result != DBNull.Value)
                        {
                            priceConv = result.ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred in Udf_Stock_PriceConvAsync: " + ex.Message);
                throw; // Rethrow the exception to propagate it back to the caller
            }

            return priceConv;
        }
        public async Task<DataTable> GetStockTransactions(string selectedStockCode, DateTime fromDate, DateTime toDate, CancellationToken cancellationToken)
        {
            DataTable tempBalances = new DataTable();

            try
            {
                string fromDateFormatted = fromDate.ToString("yyyy-MM-dd");
                string toDateFormatted = toDate.ToString("yyyy-MM-dd");

                // Add columns to the DataTable
                AddColumns(tempBalances);

                int balance = 0;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    SqlCommand command = new SqlCommand();
                    command.Connection = connection;

                    // Set the transaction isolation level
                    command.CommandText = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED";
                    await command.ExecuteNonQueryAsync(cancellationToken);

                    command.CommandText = @"
                SELECT [Transaction Type], [Quantity], [Document Date], [Customer Name], [Supplier Name], [Remarks],
                       [Document No], [Cost / Price Conv], [Part No], [Particular], [Office], [Vehicle No],
                       [Chassis No], [Engine No], [Hist], [Location], [Location From], [Location To],
                       [Supp Tran No], [DO No], [UOM], [Item Discount], [ORI_Price Conv], [Base Rate],
                       [DO Date], [Bin / Shelf No], [From Shelf No], [To Shelf NO], [Duty Code],
                       [Tax INCLUSIVE], [Issue By], [Forex Code], [Supplier Code], [Details Remarks],
                       [Salesman Code], [Service Advisor], [Brand Code], [Transporter Code], [Transporter Name]
                FROM [dbo].[Udf_Get_StockTranDetail](@SelectedStockCode, @FromDate, @ToDate)";

                    command.Parameters.AddWithValue("@SelectedStockCode", selectedStockCode);
                    command.Parameters.AddWithValue("@FromDate", fromDateFormatted);
                    command.Parameters.AddWithValue("@ToDate", toDateFormatted);

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {
                            string transactionType = reader["Transaction Type"].ToString();
                            int quantity = Convert.ToInt32(reader["Quantity"]);
                            DateTime documentDate = Convert.ToDateTime(reader["Document Date"]);

                            string customerSupplierRemarks = "";
                            if (!string.IsNullOrEmpty(reader["Customer Name"].ToString()))
                                customerSupplierRemarks += reader["Customer Name"] + " / ";
                            if (!string.IsNullOrEmpty(reader["Supplier Name"].ToString()))
                                customerSupplierRemarks += reader["Supplier Name"] + " / ";
                            if (!string.IsNullOrEmpty(reader["Remarks"].ToString()))
                                customerSupplierRemarks += reader["Remarks"];

                            int? inQuantity = null;
                            int? outQuantity = null;

                            if (transactionType == "POREC" || transactionType == "REC" || transactionType == "PRE" ||
                                transactionType == "ADJ" || transactionType == "WRT" || transactionType == "TRF" ||
                                transactionType == "TRFB" || transactionType == "PRI" || transactionType == "BTI" ||
                                transactionType == "COS" || transactionType == "CIS")
                            {
                                inQuantity = quantity;
                                balance += quantity;
                            }
                            else if (transactionType == "DO" || transactionType == "INV" || transactionType == "SAL" ||
                                     transactionType == "SRE" || transactionType == "BTO" || transactionType == "COC" ||
                                     transactionType == "CIC" || transactionType == "CLM")
                            {
                                outQuantity = quantity;
                                balance -= quantity;
                            }

                            tempBalances.Rows.Add(
                                reader["Document No"],
                                documentDate,
                                transactionType,
                                customerSupplierRemarks,
                                inQuantity,
                                outQuantity,
                                balance,

                                reader["Cost / Price Conv"],
                                 "Loading...",
                                null, // SC (to be updated later)
                                reader["Part No"],
                                reader["Particular"],
                                reader["Office"],
                                null, // CN No (to be updated later)
                                0.00, // CN Amt (to be updated later)
                                reader["Vehicle No"],
                                reader["Chassis No"],
                                reader["Engine No"],
                                null, // DB Name (to be updated later)
                                reader["Hist"],
                                reader["Location"],
                                reader["Location From"],
                                reader["Location To"],
                                reader["Supp Tran No"],
                                reader["DO No"],
                                reader["UOM"],
                                reader["Item Discount"],
                                reader["ORI_Price Conv"],
                                reader["Base Rate"],
                                reader["DO Date"],
                                reader["Bin / Shelf No"],
                                reader["From Shelf No"],
                                reader["To Shelf NO"],
                                reader["Duty Code"],
                                reader["Tax INCLUSIVE"],
                                0.00, // Cost / Price + Tax (to be updated later)
                                reader["Issue By"],
                                inQuantity,
                                outQuantity,
                                reader["Forex Code"],
                                reader["Supplier Code"],
                                reader["Details Remarks"],
                                reader["Salesman Code"],
                                reader["Service Advisor"],
                                null, // DN No (to be updated later)
                                0.00, // DN Amt (to be updated later)
                                null, // CN Date (to be updated later)
                                null, // PONo (to be updated later)
                                null, // POLine (to be updated later)
                                reader["Brand Code"],
                                reader["Transporter Code"],
                                reader["Transporter Name"]
                            );
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred in GetStockTransactions: " + ex.Message);
                throw;
            }

            return tempBalances;
        }
        private string GetCustomerSupplierRemarks(SqlDataReader reader)
        {
            string remarks = "";

            if (!string.IsNullOrEmpty(reader["Customer Name"].ToString()))
                remarks += reader["Customer Name"].ToString() + " / ";
            if (!string.IsNullOrEmpty(reader["Supplier Name"].ToString()))
                remarks += reader["Supplier Name"].ToString() + " / ";
            if (!string.IsNullOrEmpty(reader["Remarks"].ToString()))
                remarks += reader["Remarks"].ToString();

            return remarks;
        }

        private int GetInQuantity(SqlDataReader reader)
        {
            string transactionType = reader["Transaction Type"].ToString();
            int quantity = Convert.ToInt32(reader["Quantity"]);

            return (transactionType == "POREC" || transactionType == "REC" || transactionType == "PRE" ||
                    transactionType == "ADJ" || transactionType == "WRT" || transactionType == "TRF" ||
                    transactionType == "TRFB" || transactionType == "PRI" || transactionType == "BTI" ||
                    transactionType == "COS" || transactionType == "CIS") ? quantity : 0;
        }

        private int GetOutQuantity(SqlDataReader reader)
        {
            string transactionType = reader["Transaction Type"].ToString();
            int quantity = Convert.ToInt32(reader["Quantity"]);

            return (transactionType == "DO" || transactionType == "INV" || transactionType == "SAL" ||
                    transactionType == "SRE" || transactionType == "BTO" || transactionType == "COC" ||
                    transactionType == "CIC" || transactionType == "CLM") ? quantity : 0;
        }

        private int CalculateBalance(SqlDataReader reader)
        {
            string transactionType = reader["Transaction Type"].ToString();
            int quantity = Convert.ToInt32(reader["Quantity"]);
            int balance = 0;

            if (transactionType == "POREC" || transactionType == "REC" || transactionType == "PRE" ||
                transactionType == "ADJ" || transactionType == "WRT" || transactionType == "TRF" ||
                transactionType == "TRFB" || transactionType == "PRI" || transactionType == "BTI" ||
                transactionType == "COS" || transactionType == "CIS")
            {
                balance += quantity;
            }
            else if (transactionType == "DO" || transactionType == "INV" || transactionType == "SAL" ||
                     transactionType == "SRE" || transactionType == "BTO" || transactionType == "COC" ||
                     transactionType == "CIC" || transactionType == "CLM")
            {
                balance -= quantity;
            }

            return balance;
        }


        private async void chkTransDesc_CheckedChanged(object sender, ItemClickEventArgs e)
        {
            if (chkTransDesc.Checked)
            {
                // Get the DataTable from the gridControl3's DataSource
                DataTable dataTable = (DataTable)gridControl3.DataSource;

                // Sort the DataTable by the "Doc No" column in descending order
                dataTable.DefaultView.Sort = "Date DESC,Doc No DESC";

                // Rebind the sorted DataTable to gridControl3
                gridControl3.DataSource = dataTable;

            }
            else
            {
                ((DataTable)gridControl3.DataSource).DefaultView.Sort = "";

                if (string.IsNullOrEmpty(selectedCode))
                {
                    // If chkTransDesc is unchecked and selectedCode is null or empty,
                    // reload the data with the original sorting
                    await LoadStockDetailIntoGrid(selectedCode, txtFromDate.DateTime.Date, txtToDate.DateTime.Date);
                }
            }
        }
        private DataTable CreateEmptyDataTable()
        {
            DataTable tempBalances = new DataTable();

            AddColumns(tempBalances);

            return tempBalances;
        }

        private void AddColumns(DataTable dataTable)
        {
            // Add columns with desired headers
            dataTable.Columns.Add("Doc No", typeof(string));
            dataTable.Columns.Add("Date", typeof(DateTime));
            dataTable.Columns.Add("Type", typeof(string));
            dataTable.Columns.Add("Customer / Supplier / Remarks", typeof(string));
            dataTable.Columns.Add("In", typeof(int));
            dataTable.Columns.Add("Out", typeof(int));
            dataTable.Columns.Add("Balance", typeof(int));
            dataTable.Columns.Add("Cost / Price", typeof(string));
            dataTable.Columns.Add("Cost / Price Loading", typeof(string));
            dataTable.Columns.Add("SC", typeof(string));
            dataTable.Columns.Add("Part No", typeof(string));
            dataTable.Columns.Add("Stock Desc", typeof(string));
            dataTable.Columns.Add("Office", typeof(string));
            dataTable.Columns.Add("CN No", typeof(string));
            dataTable.Columns.Add("CN Amt", typeof(decimal));
            dataTable.Columns.Add("Vehicle No", typeof(string));
            dataTable.Columns.Add("Chassis No", typeof(string));
            dataTable.Columns.Add("Engine No", typeof(string));
            dataTable.Columns.Add("DB Name", typeof(string));
            dataTable.Columns.Add("Hist", typeof(string));
            dataTable.Columns.Add("Loc", typeof(string));
            dataTable.Columns.Add("Loc From", typeof(string));
            dataTable.Columns.Add("Loc To", typeof(string));
            dataTable.Columns.Add("SuppInvNo", typeof(string));
            dataTable.Columns.Add("DONo", typeof(string));
            dataTable.Columns.Add("UOM", typeof(string));
            dataTable.Columns.Add("Disc", typeof(string));
            dataTable.Columns.Add("Ori Cost/ Price", typeof(string));
            dataTable.Columns.Add("Base Rate", typeof(decimal));
            dataTable.Columns.Add("DO Date", typeof(DateTime));
            dataTable.Columns.Add("Bin", typeof(string));
            dataTable.Columns.Add("From Bin", typeof(string));
            dataTable.Columns.Add("To Bin", typeof(string));
            dataTable.Columns.Add("Duty", typeof(string));
            dataTable.Columns.Add("Tax", typeof(decimal));
            dataTable.Columns.Add("Cost / Price + Tax", typeof(decimal));
            dataTable.Columns.Add("Issue By", typeof(string));
            dataTable.Columns.Add("Qty In", typeof(int));
            dataTable.Columns.Add("Qty Out", typeof(int));
            dataTable.Columns.Add("Forex", typeof(string));
            dataTable.Columns.Add("Code", typeof(string));
            dataTable.Columns.Add("Details Remarks", typeof(string));
            dataTable.Columns.Add("Salesman / Mechanic", typeof(string));
            dataTable.Columns.Add("Service Advisor", typeof(string));
            dataTable.Columns.Add("DN No", typeof(string));
            dataTable.Columns.Add("DN Amt", typeof(decimal));
            dataTable.Columns.Add("CN Date", typeof(DateTime));
            dataTable.Columns.Add("PONo", typeof(string));
            dataTable.Columns.Add("POLine", typeof(string));
            dataTable.Columns.Add("Brand", typeof(string));
            dataTable.Columns.Add("TransporterCode", typeof(string));
            dataTable.Columns.Add("TransporterName", typeof(string));
        }
        private void AddStockOtherInfoColumns(DataTable dataTable)
        {
            // Add columns with desired headers
            dataTable.Columns.Add("LN", typeof(int));
            dataTable.Columns.Add("Date", typeof(DateTime));
            dataTable.Columns.Add("Time", typeof(TimeSpan));
            dataTable.Columns.Add("Issue", typeof(string));
            dataTable.Columns.Add("Stock", typeof(string));
            dataTable.Columns.Add("Customer", typeof(string));
            dataTable.Columns.Add("Name", typeof(string));
            dataTable.Columns.Add("Qty", typeof(int));
            dataTable.Columns.Add("Price", typeof(decimal));
            dataTable.Columns.Add("Remarks", typeof(string));
        }


        private void AddStockRequestColumns(DataTable dataTable)
        {
            // Add columns with desired headers
            dataTable.Columns.Add("Doc No", typeof(string));
            dataTable.Columns.Add("Date", typeof(DateTime));
            dataTable.Columns.Add("Supplier", typeof(string));
            dataTable.Columns.Add("Request", typeof(string));
            dataTable.Columns.Add("Issue By", typeof(string));
            dataTable.Columns.Add("Remarks", typeof(string));
            dataTable.Columns.Add("Description", typeof(string));
            dataTable.Columns.Add("Quantity", typeof(int));
            dataTable.Columns.Add("UOM", typeof(string));
            dataTable.Columns.Add("Part No", typeof(string));
        }

        private void LoadDataForSelectedCode(string stockCode)
        {
            string query = $"SELECT * FROM [Stock] WHERE [Stock Code] = @Stock";


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Fetch data from the first query
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Add the @Stock parameter to the command
                        command.Parameters.AddWithValue("@Stock", stockCode);

                        SqlDataReader reader = command.ExecuteReader();

                        if (reader.Read())
                        {
                            // Fill in the fields based on data retrieved from AR_Invoice_Main
                            txtStockCode.Text = stockCode;
                            txtDescription.Text = reader["Description"].ToString();
                            txtSOQty.Text = "0";
                            txtPORec.Text = "0";
                            txtDOQty.Text = Convert.ToDecimal(reader["DO Quantity"]).ToString("#,0");
                            txtInvQty.Text = Convert.ToDecimal(reader["Inv Quantity"]).ToString("#,0");
                            txtPOQty.Text = Convert.ToDecimal(reader["POREC Qty"]).ToString("#,0");
                            txtNetQty.Text = Convert.ToDecimal(reader["Nett Qty"]).ToString("#,0");
                            txtStdCost.Text = Convert.ToDouble(reader["Standard Cost"]).ToString("0.00");
                            txtAvgCost.Text = "0";
                            txtAvlQty.Text = "0";
                            txtAlloc.Text = "0";
                            txtCategory.Text = reader["Category Code"].ToString();
                            txtBrand.Text = reader["Brand Code"].ToString();
                            txtBinNo.Text = reader["Bin / Shelf No"].ToString();
                            txtClassCode.Text = reader["Class Code"].ToString();
                            txtModel.Text = reader["Model Code"].ToString();
                            txtMinLevel.Text = Convert.ToDecimal(reader["Min Level"]).ToString("#,0");
                            txtMaxLevel.Text = Convert.ToDecimal(reader["Max Level"]).ToString("#,0");
                            txtStockRemarks.Text = reader["Remarks"].ToString();
                            txtStockRemarks.ForeColor = Color.Red;
                        }

                        reader.Close(); // Close the reader after retrieving data
                    }

                } // Connection will be automatically closed when leaving this block
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ExceptionLogger.LogException(ex, nameof(frmTransactionDetailAnalysis), "Inventory", "LoadDataForSelectedCode", new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
            }
        }

        private void LoadInterchangeDataIntoGrid(string stockCode)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Construct the SQL query to select all columns from AR_Invoice_Charges table
                    // where Document No equals the provided invoice number if stockCode is not empty,
                    // otherwise fetch the header only
                    string query = string.IsNullOrEmpty(stockCode)
                        ? "SELECT TOP 0 [Interchange],[Remarks],[Price] AS 'Unit Price', [Supplier Code], [Supplier Name] FROM [Stock_Interchange]"
                        : $"SELECT [Interchange],[Remarks],[Price] AS 'Unit Price', [Supplier Code], [Supplier Name] FROM [Stock_Interchange] WHERE [Stock Code] = @StockCode";

                    // Create a SqlCommand to execute the query
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // If stockCode is not empty, add it as a parameter
                        if (!string.IsNullOrEmpty(stockCode))
                            command.Parameters.AddWithValue("@StockCode", stockCode);

                        // Create a SqlDataAdapter to fetch data from the database
                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            // Create a DataTable to hold the retrieved data
                            DataTable interchargeData = new DataTable();

                            // Fill the DataTable with data from the database
                            adapter.Fill(interchargeData);

                            // Set the data source for gridView3
                            gridControl1.DataSource = interchargeData;


                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ExceptionLogger.LogException(ex, nameof(frmTransactionDetailAnalysis), "Inventory", "LoadInterchangeDataIntoGrid", new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
            }
        }


        private void LoadOEMDataIntoGrid(string stockCode)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Construct the SQL query to select all columns from Stock_OEM table
                    // where Stock Code equals the provided stock code if stockCode is not empty,
                    // otherwise fetch the header only
                    string query = string.IsNullOrEmpty(stockCode)
                        ? "SELECT TOP 0 [LN], [OEM Code] AS 'OEM' FROM [Stock_OEM]"
                        : $"SELECT [LN], [OEM Code] AS 'OEM' FROM [Stock_OEM] WHERE [Stock Code] = @StockCode";

                    // Create a SqlCommand to execute the query
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // If stockCode is not empty, add it as a parameter
                        if (!string.IsNullOrEmpty(stockCode))
                            command.Parameters.AddWithValue("@StockCode", stockCode);

                        // Create a SqlDataAdapter to fetch data from the database
                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            // Create a DataTable to hold the retrieved data
                            DataTable OEMData = new DataTable();

                            // Fill the DataTable with data from the database
                            adapter.Fill(OEMData);

                            // Set the data source for gridView3
                            gridControl2.DataSource = OEMData;

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ExceptionLogger.LogException(ex, nameof(frmTransactionDetailAnalysis), "Inventory", "LoadOEMDataIntoGrid", new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
            }
        }


        private void SetColumnWidths()
        {
            // Create a SqlConnection
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Replace the query with the actual query to fetch column widths from the database
                string query = "SELECT [Column Name], [Column Length] FROM [Grid_Detail_Tbl] WHERE [Code] = 1003";

                // Create a SqlDataAdapter to fetch data from the database
                SqlDataAdapter dataAdapter = new SqlDataAdapter(query, connection);

                // Create a DataTable to hold the column width data
                DataTable columnWidthTable = new DataTable();

                // Fill the DataTable with column width data from the database
                dataAdapter.Fill(columnWidthTable);



                foreach (DataRow row in columnWidthTable.Rows)
                {
                    string columnName = row["Column Name"].ToString();
                    int columnWidth = Convert.ToInt32(row["Column Length"]);

                    if (gvStockDetailAnalysis.Columns[columnName] != null)
                    {
                        // Set the width of the existing column
                        gvStockDetailAnalysis.Columns[columnName].Width = columnWidth * 10;

                        // Check if the current column is "Cost / Price" to synchronize with "Cost / Price Loading"
                        if (columnName == "Cost / Price")
                        {
                            // Find or create the "Cost / Price Loading" column
                            gvStockDetailAnalysis.Columns["Cost / Price Loading"].Width = columnWidth * 10;


                        }
                    }
                    else
                    {
                        Console.WriteLine($"Column not found: {columnName}");
                    }
                }
            }
        }

        public void ComplexBestFit(VGridControl grid)
        {
            int maxRowHeaderWidth = -1;
            int maxRecordWidth = -1;

            grid.BestFit();
            if (grid.RowHeaderWidth > maxRowHeaderWidth)
                maxRowHeaderWidth = grid.RowHeaderWidth;
            int recordWidth = CalcBestRecordWidth(grid);
            if (recordWidth > maxRecordWidth)
                maxRecordWidth = recordWidth;



            grid.RowHeaderWidth = maxRowHeaderWidth;
            grid.RecordWidth = maxRecordWidth;

        }

        public int CalcBestRecordWidth(VGridControl vertGrid)
        {
            int minRecordWidth = 40;
            int recordCount = vertGrid.RecordCount;
            Graphics measureGraphics = vertGrid.CreateGraphics();
            foreach (BaseRow row in vertGrid.Rows)
                if (row.Visible)
                {
                    Font rowFont = row.Appearance.Font;
                    for (int currCell = 0; currCell < recordCount; currCell++)
                    {
                        string text = vertGrid.GetCellDisplayText(row, currCell);
                        int desiredRecordWidth = (int)measureGraphics.MeasureString(text, rowFont).Width;
                        if (desiredRecordWidth > minRecordWidth)
                            minRecordWidth = desiredRecordWidth;
                    }
                }
            return minRecordWidth;
        }

        private async void txtFromDate_Properties_EditValueChanged(object sender, EventArgs e)
        {
            await LoadStockDetailWithLatestDatesAsync();
        }

        private async void txtToDate_EditValueChanged(object sender, EventArgs e)
        {
            await LoadStockDetailWithLatestDatesAsync();
        }

        private async Task LoadStockDetailWithLatestDatesAsync()
        {
            // Get the selected stock code


            // Get the latest from and to dates from the date pickers
            DateTime fromDate = txtFromDate.DateTime.Date;
            DateTime toDate = txtToDate.DateTime.Date;

            // Call LoadStockDetailIntoGrid asynchronously with the latest dates
            await LoadStockDetailIntoGrid(this.selectedCode, fromDate, toDate);
        }


    }

}