
using DevExpress.Drawing;
using DevExpress.Utils.Extensions;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base.ViewInfo;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraVerticalGrid;

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
using DevExpress.Office.Utils;

using GreenStem.ClassModules;
using GreenStem.LookUp;
using System.Threading;
using static GreenStem.ClassModules.LogGuserLocked;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraExport.Helpers;
using DevExpress.XtraExport.Xls;
using DevExpress.XtraGrid;
using GreenStem.AR.Properties;
using System.Resources;
using System.Configuration;
namespace GreenStem.AR
{
    public partial class frm_AR_Delivery_Invoice_Counter : DevExpress.XtraBars.Ribbon.RibbonForm
    {
    
        private string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connection_string"].ConnectionString;
        private List<LogData> logsToDelete;
        private DataTable customerTable;
        private DataTable stockTable;
        private Dictionary<string, int> columnLengths = new Dictionary<string, int>();
        private Dictionary<DataRowView, GridLookUpEdit> rowGridLookUpEditMap = new Dictionary<DataRowView, GridLookUpEdit>();
        private DataRowView rowView;
        private List<Control> dynamicControls = new List<Control>();
        private Dictionary<DataRowView, PictureBox> rowPictureBoxMap = new Dictionary<DataRowView, PictureBox>();
        DataRowView clickedRowView; // Store the DataRowView of the clicked row
        // Define the variables at a broader scope so they can be accessed by multiple methods
        int pictureBoxX, pictureBoxY, pictureBoxWidth, pictureBoxHeight;
        private EventWaitHandle logsDeletedEvent;
        private string invoiceNo;
        private bool stopListeningForLogs = false; // Flag to signal the thread to stop'
        private bool isButtonClickEvent = false;
        private TaskCompletionSource<bool> stockTableLoadedTcs = new TaskCompletionSource<bool>();
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frm_AR_Delivery_Invoice_Counter));
        public frm_AR_Delivery_Invoice_Counter()
        {
            InitializeComponent();
            LoadDataIntoGrid(null);
            // Load data from Customer_tbl
            Task.Run(async () => await LoadCustomerDataAsync());
            // Load stock data asynchronously
            Task.Run(async () => await LoadStockDataAsync());
            // Set up GridLookUpEdit
          
            this.SizeChanged += f5Form_SizeChanged;
            InitializeLogsDeletedEvent();
            //set minimum size of the screen
            MinimumSize = new System.Drawing.Size(1024, 768);
            this.Load += frmARDeliveryInvoiceCounter_Load;

            LoadStockLocationData(null);
            LoadInvoiceChargeDataIntoGrid(null);

         
            //when the form is closing, do some action
            this.FormClosing += frmARDeliveryInvoiceCounter_FormClosing;


            //create invoice payment table
            DataTable invoicePaymentTable = new DataTable();
            AddInvoicePayment(invoicePaymentTable);
            gridControl4.DataSource = invoicePaymentTable;
            gridView4.OptionsView.ColumnAutoWidth = true;
        }

        private void InitializeLogsDeletedEvent()
        {
            const string LogsDeletedEventName = "LogsDeletedEvent";
            logsDeletedEvent = new EventWaitHandle(false, EventResetMode.AutoReset, LogsDeletedEventName);

            // Start a thread to listen for the logs deleted event
            ThreadPool.QueueUserWorkItem(ListenForLogsDeleted);
        }
        private void frmARDeliveryInvoiceCounter_Load(object sender, EventArgs e)
        {
            // Now that the form is loaded, start listening for the logs deleted event
            ThreadPool.QueueUserWorkItem(ListenForLogsDeleted);
        }
        private void ListenForLogsDeleted(object state)
        {
            try
            {
                List<LogData> logsToDelete = state as List<LogData>; // Retrieve logsToDelete from the state

                // Wait indefinitely for the event to be signaled
                while (true)
                {
                    logsDeletedEvent.WaitOne();

                    // Invoke the UI update on the UI thread
                    // Check if the form's handle has been created
                    if (this.IsHandleCreated)
                    {
                        // Invoke the UI update on the UI thread
                        this.Invoke((MethodInvoker)delegate
                        {
                            // Update the UI safely
                            this.WindowState = FormWindowState.Normal;

                            this.Close();
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        //future
        //private void ListenForLogsDeleted(object state)
        //{
        //    try
        //    {
        //        // Wait indefinitely for the event to be signaled
        //        while (true)
        //        {
        //            logsDeletedEvent.WaitOne();

        //            // Process the logsToDelete
        //            ProcessLogsToDelete();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.ToString());
        //    }
        //}

        //private void ProcessLogsToDelete()
        //{
        //    // Process the logsToDelete here as needed
        //    foreach (var log in logsToDelete)
        //    {
        //        Console.WriteLine($"Deleting log - DocumentNo: {log.DocumentNo}, FormName: {log.FormName}, UserName: {log.UserName}");
        //    }

        //    // After processing, you can perform additional actions if needed
        //    this.Invoke((MethodInvoker)delegate
        //    {
        //        this.WindowState = FormWindowState.Normal;
        //        this.Close();
        //    });
        //}

        //private void LogManager_LogsToDeleteEvent(object sender, List<LogData> e)
        //{
        //    // Handle the LogsToDeleteEvent from LogManager
        //    logsToDelete = e;

        //    // Signal the event to start processing logs
        //    logsDeletedEvent.Set();
        //}



        // Subscribe to LogsToDeleteEvent from LogManager
        //LogManager.LogsToDeleteEvent += LogManager_LogsToDeleteEvent;

        //private EventWaitHandle logsDeletedEvent;
        //private List<LogData> logsToDelete;

        private void frmARDeliveryInvoiceCounter_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Set the flag to stop listening for logs
            stopListeningForLogs = true;
            // Perform cleanup actions, such as deleting the user lock
            LogGuserLocked.DeleteGUserLocked("INV", nameof(frm_AR_Delivery_Invoice_Counter), invoiceNo);
            logsDeletedEvent.Dispose();  // Dispose the EventWaitHandle
        }

        private void AddInvoicePayment(DataTable dataTable)
        {
            // Add columns with desired headers
            dataTable.Columns.Add("No", typeof(int));
            dataTable.Columns.Add("Payment Mode", typeof(string));
            dataTable.Columns.Add("Ref No (Credit Card No / Cheque)", typeof(string));
            dataTable.Columns.Add("Amount", typeof(decimal));
            dataTable.Columns.Add("Pay Date", typeof(DateTime));
        }

        private void txtCustomer_EditValueChanged(object sender, EventArgs e)
        {
            // Get the selected customer code
            string selectedCustomerCode = txtCustomer.EditValue as string;

            // Find the corresponding row in the customer table
            DataRow[] selectedRows = customerTable.Select($"[Customer Code] = '{selectedCustomerCode}'");

            // If a row is found, update the labels
            if (selectedRows.Length > 0)
            {
                string customerName = selectedRows[0]["Customer Name"].ToString();
                string customerEmail = selectedRows[0]["Customer Email"].ToString();

                lblCustName.Text = customerName;
                txtCustName.Text = customerName;
                lblTel.Visible = true;

            }
        }
        private void gridView2_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            DataRow focusedRow = gvInvoiceDetail.GetFocusedDataRow();
            if (focusedRow != null)
            {
                string stockCode = focusedRow["Stock"].ToString();
                if (!string.IsNullOrEmpty(stockCode))
                {
                    // Call a method to load data into vGridControl1 based on the selected stock code
                    LoadStockLocationData(stockCode);
                }
            }
        }
        private void LoadInvoiceChargeDataIntoGrid(string invoiceNo)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Construct the SQL query to select all columns from AR_Invoice_Charges table
                    // where Document No equals the provided invoice number
                    string query = $"SELECT * FROM [AR_Invoice_Charges] WHERE [Document No] = @InvoiceNo " + modPublicVariable.ReadLockType;

                    // Create a SqlCommand to execute the query
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Add parameter to prevent SQL injection
                        command.Parameters.AddWithValue("@InvoiceNo", invoiceNo);

                        // Create a SqlDataAdapter to fetch data from the database
                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            // Create a DataTable to hold the retrieved data
                            DataTable invoiceChargeData = new DataTable();

                            // Fill the DataTable with data from the database
                            adapter.Fill(invoiceChargeData);

                            // Set the data source for gridView3
                            gridControl2.DataSource = invoiceChargeData;
                            gridView3.OptionsView.ColumnAutoWidth = false;
                            gvInvoiceDetail.OptionsView.ColumnAutoWidth = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        private void LoadStockLocationData(string stockCode)
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
                    string query = $"SELECT [Location Code] AS Loc, [Current Quantity] AS 'INV Qty', [DO Quantity] AS 'DO Qty' FROM [Stock_Location] WHERE [Stock Code] = @StockCode";

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
            vGridControl1.RowHeaderWidth = 100;
            vGridControl1.LayoutStyle = DevExpress.XtraVerticalGrid.LayoutViewStyle.SingleRecordView;
            vGridControl1.Appearance.RowHeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            //vGridControl1.Appearance.RowHeaderPanel.BackColor = Color.Lavender;
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

        private async Task LoadCustomerDataAsync()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync(); // Open the connection asynchronously

                    string query = "SELECT [Customer Code], [Customer Name], [Salesman Code], [Area Code], [Customer Email] FROM [Customer_tbl]";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            customerTable = new DataTable();
                            await Task.Run(() => adapter.Fill(customerTable)); // Fill the DataTable asynchronously
                        }

                        // Check if the control's handle has been created before invoking
                      
                                txtCustomer.Properties.DataSource = customerTable; // Corrected to customerTable
                                txtCustomer.Properties.DisplayMember = "Customer Code"; // Display Customer Code
                                txtCustomer.Properties.ValueMember = "Customer Code";
                                txtCustomer.EditValueChanged += txtCustomer_EditValueChanged;
                        
                       
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        private async Task LoadStockDataAsync()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string columnQuery = "SELECT [Field Name], [Column Length] FROM [SDG_Setting_Detail_Tbl] WHERE [Code] = 10002";

                    using (SqlCommand columnCommand = new SqlCommand(columnQuery, connection))
                    {
                        using (SqlDataReader reader = await columnCommand.ExecuteReaderAsync())
                        {
                            StringBuilder columnList = new StringBuilder();

                            while (await reader.ReadAsync())
                            {
                                string columnName = reader["Field Name"].ToString();
                                int columnLength = Convert.ToInt32(reader["Column Length"]);
                                string columnNameWithoutBrackets = columnName.Trim('[', ']');
                                if (columnList.Length > 0)
                                    columnList.Append(", ");

                                columnList.Append(columnName);

                                columnLengths.Add(columnNameWithoutBrackets, columnLength);
                            }

                            reader.Close();

                            string query = $"SELECT {columnList.ToString()} FROM [Stock]";

                            using (SqlCommand command = new SqlCommand(query, connection))
                            {
                                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                                {
                                    stockTable = new DataTable();
                                    await Task.Run(() => adapter.Fill(stockTable));
                                    stockTableLoadedTcs.SetResult(true);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
   
        private void TxtCustomer_Popup(object sender, EventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView gridView = txtCustomer.Properties.View as DevExpress.XtraGrid.Views.Grid.GridView;
            if (gridView != null)
            {

                foreach (DevExpress.XtraGrid.Columns.GridColumn column in gridView.Columns)
                {
                    Console.WriteLine($"Column Name: {column.FieldName}, Display Caption: {column.Caption}");
                }
                //gridView.Columns["Customer Name"].Width = 300; // Adjust width of Customer Name column
                //gridView.Columns["Customer Email"].Width = 300; // Adjust width of Customer Email column
            }
        }


        private void LoadDataForSelectedCode(string invoiceNo)
        {
            string query = $"SELECT * FROM [AR_Invoice_Main] WHERE [Document No] = '{invoiceNo}'";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Fetch data from the first query
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        SqlDataReader reader = command.ExecuteReader();

                        if (reader.Read())
                        {
                            // Fill in the fields based on data retrieved from AR_Invoice_Main
                            txtInvoiceNo.Text = invoiceNo;
                            txtSalesType.Text = reader["Sales Type"].ToString();
                            txtTransportCode.Text = reader["Transporter Code"].ToString();
                            txtTransportName.Text = reader["Transporter Name"].ToString();
                            txtSalesmanCode.Text = reader["Salesman"].ToString();
                            txtIssueBy.Text = reader["Issue By"].ToString();
                            txtForexCode.Text = reader["Forex Code"].ToString(); // decimal field
                            txtForexRate.Text = reader["Forex Rate"].ToString(); // decimal field
                            txtLocalRate.Text = reader["Local Rate"].ToString(); // decimal field
                            txtDeliveryTo.Text = reader["Deliver To"].ToString();
                            txtRemarks.Text = reader["Remarks"].ToString();
                            txtIssueDate.Text = reader["Issue Date"].ToString();
                            txtCustName.Text = reader["Customer Name"].ToString();
                            txtIssueDate.Text = reader["Issue Date"].ToString();
                            txtTerm.Text = reader["Terms"].ToString();
                            txtPoNo.Text = reader["Po No"].ToString();
                            txtCustomer.Text = reader["Customer"].ToString();
                            lblCustName.Text = reader["Customer Name"].ToString();

                        }

                        reader.Close(); // Close the reader after retrieving data
                    }

                } // Connection will be automatically closed when leaving this block

                // After filling in the main details, load the data into the grid
                LoadDataIntoGrid(invoiceNo);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void LoadDataIntoGrid(string invoiceNo)
        {



            try
            {
              
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Query Grid_Detail_Tbl to determine which columns to select
                    string columnQuery = "SELECT [Column Name] FROM [Grid_Detail_Tbl] WHERE [Code] = 1002";

                    // Create a SqlCommand to execute the column query
                    using (SqlCommand columnCommand = new SqlCommand(columnQuery, connection))
                    {
                        // Create a SqlDataReader to read column names from the query result
                        using (SqlDataReader reader = columnCommand.ExecuteReader())
                        {
                            // Construct the column list for the SQL query
                            StringBuilder columnList = new StringBuilder();
                            while (reader.Read())
                            {
                                string columnName = reader["Column Name"].ToString();
                                if (columnList.Length > 0)
                                    columnList.Append(", ");
                                columnList.Append($"[{columnName}]");
                            }

                            // Close the reader after reading the column names
                            reader.Close();

                            // Construct the SQL query using the determined column list
                            string gridQuery = "SELECT * FROM [AR_Invoice_Detail] " + modPublicVariable.ReadLockType + " WHERE [Document No] = @InvoiceNo";

                            // Create a SqlDataAdapter to fetch data from the database
                            using (SqlDataAdapter dataAdapter = new SqlDataAdapter(gridQuery, connection))
                            {
                                // Create a DataTable to hold the query result
                                DataTable dataTable = new DataTable();
                                dataAdapter.SelectCommand.Parameters.AddWithValue("@InvoiceNo", invoiceNo);
                                // Fill the DataTable with data from the database
                                dataAdapter.Fill(dataTable);

                                // Add a column for the row numbers
                                dataTable.Columns.Add("No", typeof(int));
                                dataTable.Columns["No"].AutoIncrement = true;
                                dataTable.Columns["No"].AutoIncrementSeed = 1;
                                dataTable.Columns["No"].AutoIncrementStep = 1;

                                // Set the grid control's data source to the filled DataTable
                                gcInvoiceDetail.DataSource = dataTable;

                                // Disable column auto-width
                                gvInvoiceDetail.OptionsView.ColumnAutoWidth = false;

                                // Set column widths based on settings in Grid_Detail_Tbl
                                SetColumnWidths();
                                gvInvoiceDetail.Columns["Stock"].Width = 200; // Adjust width of Customer Name column

                                gvInvoiceDetail.FocusedRowChanged += gridView2_FocusedRowChanged;
                                gvInvoiceDetail.CustomRowCellEdit += gridView2_CustomRowCellEdit;
                                gvInvoiceDetail.MouseDown -= PictureBoxClickHandler;
                                gvInvoiceDetail.MouseDown += PictureBoxClickHandler;

                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void gridView2_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
        {
            if (e.Column.FieldName == "Stock" && e.RowHandle >= 0)
            {
                // Get the DataRowView for the current row
                DataRowView rowView = gvInvoiceDetail.GetRow(e.RowHandle) as DataRowView;

                if (rowView != null)
                {
                    // Create a repository item for the GridLookUpEdit
                    RepositoryItemGridLookUpEdit repositoryItem = new RepositoryItemGridLookUpEdit();
                    if (stockTable != null)
                    {

                        DataTable dataSourceTable = new DataTable();
                        dataSourceTable.Columns.Add("Stock Code"); // Adjust column name according to your data
                        dataSourceTable.Rows.Add(rowView["Stock"]); // Add the initial value to the DataTable

                        // Set the DataSource of the repository item to the DataTable
                        repositoryItem.DataSource = dataSourceTable;
                    
                        repositoryItem.DisplayMember = "Stock Code";
                        repositoryItem.ValueMember = "Stock Code";
                        repositoryItem.PopupFormSize = new Size(500, 200); // Adjust size as needed
                        repositoryItem.NullText = ""; // Remove default text
                        repositoryItem.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                        repositoryItem.SearchMode = DevExpress.XtraEditors.Repository.GridLookUpSearchMode.AutoSuggest;

                        repositoryItem.Popup += gridLookUp_Popup; // Handle Popup event
                        repositoryItem.EditValueChanged += (editSender, editArgs) =>
                        {
                            if (!isButtonClickEvent)
                            {
                                GridLookUpEdit editGridLookUpEdit = editSender as GridLookUpEdit;
                                object selectedValue = editGridLookUpEdit.EditValue;
                                if (selectedValue != null)
                                {
                                    // Retrieve additional data based on the selected value
                                    string stockValue = selectedValue.ToString();
                                    DataTable updatedData = GetDataFromDatabase(stockValue); // Implement this method to fetch data from [AR_Invoice_Detail] where [Stock] = stockValue

                                    // Check if the updatedData DataTable contains any rows
                                    if (updatedData.Rows.Count > 0)
                                    {

                                        // Update the row data in the grid view
                                        UpdateRowData(rowView, updatedData);
                                    }
                                    else
                                    {
                                        // Handle the case where no data is returned for the selected stockValue
                                        Console.WriteLine("No data found for the selected stock.");

                                    }
                                }
                            }
                        };
                    }

                    repositoryItem.Buttons.Clear(); // Clear existing buttons if necessary
                                                    // Load the original image


                    Image originalImage = ((System.Drawing.Image)(resources.GetObject("picInvoiceLookup.Image")));

                    // Resize the image to the desired size
                    Image resizedImage = new Bitmap(originalImage, new Size(20, 20)); // Set the desired size here

                    // Add the resized image to the EditorButton
                    repositoryItem.Buttons.Add(new EditorButton(ButtonPredefines.Glyph)
                    {
                        Image = resizedImage, // Use the resized image
                        ImageLocation = ImageLocation.MiddleRight, // Set the image location to the right
                        Kind = ButtonPredefines.Glyph // Specify the button kind
                    });


                    // Handle the ButtonClick event to perform the desired action
                    repositoryItem.ButtonClick += (clickSender, clickArgs) =>
                    {
                        isButtonClickEvent = true;
                        if (clickArgs.Button.Kind == ButtonPredefines.Glyph)
                        {
                            // Get the DataRowView of the current row
                            int rowHandle = gvInvoiceDetail.FocusedRowHandle;
                            DataRowView currentRowView = gvInvoiceDetail.GetRow(rowHandle) as DataRowView;

                            if (currentRowView != null)
                            {
                                // Call the method to open the lookup form and handle the selected data
                                frmLookUp.OpenLookupForm("10003", (selectedData) => HandleDataGridSelectedEvent(selectedData, currentRowView), this, false);
                            }
                        }

                        isButtonClickEvent = false;
                    };

                   
                     _ = SetRepositoryItemDataSourceAsync(repositoryItem);
                    // Assign the created repository item to the cell's Edit property
                    e.RepositoryItem = repositoryItem;
                }

            }
        }
    
        private async Task SetRepositoryItemDataSourceAsync(RepositoryItemGridLookUpEdit repositoryItem)
        {
            // Wait for the stockTable to be loaded
            await stockTableLoadedTcs.Task;
            // Now you can safely assign the DataSource
            if (stockTable != null)
            {
                // Use the form or a control's Invoke method
                this.Invoke((MethodInvoker)(() =>
                {

                    repositoryItem.DataSource = null;
                    repositoryItem.DataSource = stockTable;
                }));
            }
        }

        // Event handler for handling PictureBox click
        void PictureBoxClickHandler(object sender, MouseEventArgs args)
        {
            GridView gridView = sender as GridView;
            GridHitInfo hitInfo = gridView.CalcHitInfo(args.Location);


            if (args.Button == MouseButtons.Left)
            {
                // Check if the click is within the PictureBox area
                Rectangle pictureBoxRect = new Rectangle(pictureBoxX, pictureBoxY, pictureBoxWidth, pictureBoxHeight);
                if (pictureBoxRect.Contains(args.Location))
                {

                    DataRowView rowView = gridView.GetRow(hitInfo.RowHandle) as DataRowView;
                    // Handle PictureBox click event
                    frmLookUp.OpenLookupForm("10003", (selectedData) => HandleDataGridSelectedEvent(selectedData, rowView), this, false);

                }
            }
        }
        private void gridView2_MouseUp(object sender, MouseEventArgs e)
        {
            GridView view = sender as GridView;
            GridHitInfo hitInfo = view.CalcHitInfo(e.Location);

            if (hitInfo.InRowCell && hitInfo.Column.FieldName == "Stock" && e.Button == MouseButtons.Left)
            {
                DataRowView rowView = gvInvoiceDetail.GetRow(hitInfo.RowHandle) as DataRowView;
                frmLookUp.OpenLookupForm("10003", (selectedData) => HandleDataGridSelectedEvent(selectedData, rowView), this, false);
            }
        }
        private void gridView2_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            if (e.Column.FieldName == "Stock" && e.RowHandle >= 0)
            {
                // Draw PictureBox in the cell
                pictureBoxWidth = 20; // Width of the PictureBox
                pictureBoxHeight = 17; // Height of the PictureBox

                // Calculate the location for the PictureBox
                pictureBoxX = e.Bounds.Right - pictureBoxWidth;
                pictureBoxY = e.Bounds.Top + (e.Bounds.Height - pictureBoxHeight) / 2;

                // Draw PictureBox
                Bitmap image = new Bitmap(@"D:\Installer\SparePart-DotNet-GridViewResize\SparePart-DotNet-GridViewResize\GreenSystem\GreenStem\ICON\Search_24_256.bmp");
                e.Graphics.DrawImage(image, pictureBoxX, pictureBoxY, pictureBoxWidth, pictureBoxHeight);


            }
        }
        private void GridView_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            if (e.Column.FieldName == "Stock")
            {
                rowView = gvInvoiceDetail.GetRow(e.RowHandle) as DataRowView;

                // Check if the rowView is already in the dictionary
                if (!rowGridLookUpEditMap.ContainsKey(rowView))
                {
                    // Add the rowView to the dictionary only if it doesn't exist
                    int pictureBoxWidth = 25; // Width of the PictureBox
                    int pictureBoxHeight = 22; // Height of the PictureBox

                    // Calculate the location for the PictureBox
                    int pictureBoxX = e.Bounds.Right - pictureBoxWidth;
                    int pictureBoxY = e.Bounds.Top + (e.Bounds.Height - pictureBoxHeight) / 2;

                    // Draw PictureBox
                    PictureBox pictureBox = new PictureBox();
                    pictureBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
                    Bitmap image = new Bitmap(@"D:\Installer\SparePart-DotNet-GridViewResize\SparePart-DotNet-GridViewResize\GreenSystem\GreenStem\ICON\Search_24_256.bmp");
                    pictureBox.Image = image;
                    pictureBox.Location = new System.Drawing.Point(pictureBoxX, pictureBoxY);
                    pictureBox.Size = new System.Drawing.Size(pictureBoxWidth, pictureBoxHeight);
                    pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
                    pictureBox.TabStop = false;
                    pictureBox.WaitOnLoad = true;
                    pictureBox.Click += (picSender, picArgs) =>
                    {
                        // Get the DataRowView of the clicked row
                        DataRowView rowView = gvInvoiceDetail.GetRow(e.RowHandle) as DataRowView;

                        // Handle PictureBox click event
                        frmLookUp.OpenLookupForm("10003", (selectedData) => HandleDataGridSelectedEvent(selectedData, rowView), this, false);
                    };

                    gvInvoiceDetail.GridControl.Controls.Add(pictureBox);
                    dynamicControls.Add(pictureBox);
                    // Draw GridLookUpEdit
                    GridLookUpEdit gridLookUpEdit = new GridLookUpEdit();
                    gridLookUpEdit.Properties.DataSource = stockTable; // Set the DataSource
                    gridLookUpEdit.Properties.DisplayMember = "Stock Code";
                    gridLookUpEdit.Properties.ValueMember = "Stock Code";
                    gridLookUpEdit.Properties.PopupFormSize = new Size(500, 200); // Adjust size as needed
                    gridLookUpEdit.Properties.NullText = ""; // Remove default text

                    DevExpress.XtraGrid.Views.Grid.GridView gridView = gridLookUpEdit.Properties.View as DevExpress.XtraGrid.Views.Grid.GridView;

                    gridLookUpEdit.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                    gridLookUpEdit.Bounds = new Rectangle(e.Bounds.Location, new Size(e.Bounds.Width - pictureBoxWidth, e.Bounds.Height)); // Adjust the width to accommodate PictureBox
                    gridLookUpEdit.EditValue = gvInvoiceDetail.GetRowCellValue(e.RowHandle, e.Column);
                    gridLookUpEdit.Properties.Popup += gridLookUp_Popup; // Handle Popup event
                    gridLookUpEdit.Properties.SearchMode = DevExpress.XtraEditors.Repository.GridLookUpSearchMode.AutoSuggest;

                    gridLookUpEdit.Properties.EditValueChanged += (editSender, editArgs) =>
                    {
                        GridLookUpEdit editGridLookUpEdit = editSender as GridLookUpEdit;
                        object selectedValue = editGridLookUpEdit.EditValue;
                        if (selectedValue != null)
                        {
                            // Retrieve additional data based on the selected value
                            string stockValue = selectedValue.ToString();
                            DataTable updatedData = GetDataFromDatabase(stockValue); // Implement this method to fetch data from [AR_Invoice_Detail] where [Stock] = stockValue

                            // Check if the updatedData DataTable contains any rows
                            if (updatedData.Rows.Count > 0)
                            {
                                // Update the row data in the grid view
                                DataRowView rowView = gvInvoiceDetail.GetRow(e.RowHandle) as DataRowView;
                                UpdateRowData(rowView, updatedData);
                            }
                            else
                            {
                                // Handle the case where no data is returned for the selected stockValue
                                Console.WriteLine("No data found for the selected stock.");
                                // You can display a message to the user or take appropriate action.
                            }
                        }
                    };
                    gvInvoiceDetail.GridControl.Controls.Add(gridLookUpEdit);
                    dynamicControls.Add(gridLookUpEdit);
                    // Store the association between row index and gridLookUpEdit control
                    rowGridLookUpEditMap.Add(rowView, gridLookUpEdit);
                }
            }
        }
        private void picInvoiceLookup_Click(object sender, EventArgs e)
        {
            frmLookUp.OpenLookupForm("10020", HandleDataSelectedEvent, this, false);
        }

        private void HandleDataSelectedEvent(Dictionary<string, object> selectedData)
        {

            LogGuserLocked.DeleteGUserLocked("INV", nameof(frm_AR_Delivery_Invoice_Counter), invoiceNo);
            invoiceNo = selectedData["[Document No]"].ToString();
            if (LogGuserLocked.CheckGUserLocked(nameof(frm_AR_Delivery_Invoice_Counter), invoiceNo))
            {
                return;
            }
            LogGuserLocked.InsertGUserLocked("INV", nameof(frm_AR_Delivery_Invoice_Counter), invoiceNo);
            LoadDataForSelectedCode(invoiceNo);

            // Clear previous controls
            // gridView2.GridControl.Controls.Clear();

            foreach (System.Windows.Forms.Control control in dynamicControls)
            {
                control.Dispose();
            }
            // Clear the list
            dynamicControls.Clear();
            gvInvoiceDetail.Columns.Clear();
            gvInvoiceDetail.GridControl.DataSource = null;

            // Call LoadDataIntoGrid with the selectedCode
            LoadDataIntoGrid(invoiceNo);
            LoadInvoiceChargeDataIntoGrid(invoiceNo);
        }


        private void HandleDataGridSelectedEvent(Dictionary<string, object> selectedData, DataRowView rowView)
        {
            string selectedStockCode = selectedData["[Stock Code]"].ToString();

            // If the selected stock code is not null or empty, update the grid data
            if (!string.IsNullOrEmpty(selectedStockCode))
            {
                DataTable updatedData = GetDataFromDatabase(selectedStockCode);


                // Assuming 'gridControl' and 'gridView' are your GridControl and GridView

                // Update the row data in the grid view
                UpdateRowData(rowView, updatedData);
                gcInvoiceDetail.BeginUpdate();
                try
                {
                    // Refresh the grid view to show the updated value
                    gvInvoiceDetail.RefreshRow(gvInvoiceDetail.FindRow(rowView));
                }
                finally
                {
                    gcInvoiceDetail.EndUpdate();
                }



            }
        }
        private void UpdateRowData(DataRowView rowView, DataTable updatedData)
        {
            if (rowView != null && updatedData.Rows.Count > 0)
            {
                // Update other columns in the rowView
                rowView["Description"] = updatedData.Rows[0]["Description"];
                rowView["Brand Code"] = updatedData.Rows[0]["Brand Code"];
                rowView["Stock"] = updatedData.Rows[0]["Stock Code"];
                gvInvoiceDetail.RefreshRow(gvInvoiceDetail.FindRow(rowView));

            }

            // Refresh the grid view to show the updated value


        }



        private void gridLookUp_Popup(object sender, EventArgs e)
        {
            GridLookUpEdit gridLookUpEdit = sender as GridLookUpEdit;
            if (gridLookUpEdit != null)
            {
                DevExpress.XtraGrid.Views.Grid.GridView gridView = gridLookUpEdit.Properties.View as DevExpress.XtraGrid.Views.Grid.GridView;
                if (gridView != null)
                {
                    foreach (DevExpress.XtraGrid.Columns.GridColumn gridColumn in gridView.Columns)
                    {
                        string columnName = gridColumn.FieldName;
                        object column = columnLengths;
                        // Check if the column length is available in the dictionary
                        if (columnLengths.ContainsKey(columnName))
                        {
                            // Set the column width based on the column length from the dictionary
                            gridColumn.Width = columnLengths[columnName];
                        }
                    }
                    // gridView.CustomRowFilter += GridLookUpEdit_CustomStockRowFilter;


                }
            }
        }

        private DataTable GetDataFromDatabase(string stockValue)
        {
            DataTable dataTable = new DataTable();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Construct the SQL query to fetch data based on the selected stockValue
                    string query = $"SELECT * FROM [Stock] WHERE [Stock Code] = @Stock";

                    // Create a SqlCommand
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Add parameters to prevent SQL injection
                        command.Parameters.AddWithValue("@Stock", stockValue);

                        // Create a SqlDataAdapter to fetch data from the database
                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            // Fill the DataTable with data from the database
                            adapter.Fill(dataTable);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return dataTable;
        }
        private void SetColumnWidths()
        {
            // Create a SqlConnection
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Replace the query with the actual query to fetch column widths from the database
                string query = "SELECT [Column Name], [Column Length] FROM [Grid_Detail_Tbl] WHERE [Code] = 1002";

                // Create a SqlDataAdapter to fetch data from the database
                SqlDataAdapter dataAdapter = new SqlDataAdapter(query, connection);

                // Create a DataTable to hold the column width data
                DataTable columnWidthTable = new DataTable();

                // Fill the DataTable with column width data from the database
                dataAdapter.Fill(columnWidthTable);

                // Adjust column widths in the gridView1
                foreach (DataRow row in columnWidthTable.Rows)
                {
                    string columnName = row["Column Name"].ToString();
                    int columnWidth = Convert.ToInt32(row["Column Length"]);

                    // Find the column in gridView1 and set its width
                    if (gvInvoiceDetail.Columns[columnName] != null)
                    {
                        gvInvoiceDetail.Columns[columnName].Width = columnWidth * 10;
                    }
                    else
                    {
                        Console.WriteLine($"Column not match {columnName}");
                    }
                }
            }
        }

        private void f5Form_SizeChanged(object sender, EventArgs e)
        {

            AdjustView();
        }
        private void AdjustView()
        {
            // You can set a threshold value for the screen width to decide whether to enable auto-width or not
            int screenWidthThreshold = 1200; // Adjust as needed

            if (this.Width <= screenWidthThreshold)
            {
                gvInvoiceDetail.OptionsView.ColumnAutoWidth = false;


            }
            else
            {


                gvInvoiceDetail.OptionsView.ColumnAutoWidth = false;
            }
        }







    }


}
