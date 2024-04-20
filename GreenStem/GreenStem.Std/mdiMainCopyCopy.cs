
using DevExpress.XtraBars.Navigation;
using DevExpress.XtraSplashScreen;
using DevExpress.XtraWaitForm;
using GreenStem.AR;
using GreenStem.Inventory;
using GreenStem.ClassModules;
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
using GreenStem.Security;
using System.IO;
using System.Runtime.InteropServices;
using GreenStem.Tool;
using GreenStem.Master;
using Greenstem.Std;
using GreenStem.LookUp;
using GreenStem.Std.Properties;
using System.Resources;
using DevExpress.XtraEditors;
using DevExpress.Images;
using System.Reflection;
using DevExpress.XtraPrinting;
using static GreenStem.Std.frmSetupMdiMain;
using DevExpress.XtraCharts;
using System.Globalization;
using DevExpress.Data.Filtering;
using DevExpress.Charts.Native;


namespace GreenStem.Std
{
    public partial class mdiMainCopyCopy : DevExpress.XtraBars.FluentDesignSystem.FluentDesignForm
    {
        
        private globalKeyHandler keyHandler; // Declare GlobalKeyHandler instance
        private frm_AR_Delivery_Invoice_Counter frm_AR_Delivery_Invoice_Counter;
        private frmTransactionDetailAnalysis transactionDetailAnalysisForm;
        private frmUserGroupAccess userGroupAccessForm;
        private frmUserGroup frmUserGroupForm;
        private frmLockRelease lockReleaseForm;
        private frmSalesman salesmanListForm;
        private frmGridDetailSetting gridDetailSettingForm;
        private frmLookUpSettings lookupSettingForm;
        private frmUserSetup userSetupForm;
        private frmSetupUserMdiMain setupMdiMainForm;
        private frmDashboardSetting dashboardSetting;
        private string menuId = "FAME_AR";
        DevExpress.XtraCharts.Series seriesSales;
        DevExpress.XtraCharts.Series seriesCollection;
        private string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connection_string"].ConnectionString;
        private ContextMenuStrip chartContextMenu;
        public mdiMainCopyCopy(string userName)
        {
            InitializeComponent();
    
            InitializeTileGroup();
            LoadMenuSettings();
            UserAccessControl(userName);


            // Subscribe to the KeyDown event of the form
            this.KeyPreview = true; // Set KeyPreview property to true
            //this.KeyDown += MainMenu_KeyDown;
            this.IsMdiContainer = true; // Set mdiMain as an MDI container
            keyHandler = new globalKeyHandler();
            keyHandler.KeyPressed += GlobalKeyHandler_KeyPressed;
          
            // Register the GlobalKeyHandler with the application
            Application.AddMessageFilter(keyHandler);
            InitializeSalesAndCollectionChart();
            InitializeSalesAndCollectionChart2();
            InitializeChartControl3();
            InitializeChartControl4();
            InitlizeYearCombobox();
            //future delete
            EnableAllMenuItems();

            // Create the context menu and the "Copy to Clipboard" item
            chartContextMenu = new ContextMenuStrip();
            ToolStripMenuItem copyItem = new ToolStripMenuItem("Copy to Clipboard");
            chartContextMenu.Items.Add(copyItem);

            // Handle the Click event of the "Copy to Clipboard" item
            copyItem.Click += (s, e) => CopyChartToClipboard();

            // Assign the context menu to the chart
            chartControl1.ContextMenuStrip = chartContextMenu;
        }

        private void CopyChartToClipboard()
        {
            // Render the chart as an image
            Bitmap chartImage = new Bitmap(chartControl1.Width, chartControl1.Height);
            chartControl1.DrawToBitmap(chartImage, new Rectangle(0, 0, chartControl1.Width, chartControl1.Height));

            // Copy the image to the clipboard
            Clipboard.SetDataObject(chartImage);
        }

        private void EnableAllMenuItems()
        {
            foreach (ToolStripMenuItem menuItem in MenuStrip1.Items)
            {
                EnableMenuItemRecursive(menuItem);
            }
        }

        private void EnableMenuItemRecursive(ToolStripMenuItem menuItem)
        {
            menuItem.Enabled = true; // Enable the current menu item

            // Enable all sub-menu items recursively
            foreach (ToolStripItem subItem in menuItem.DropDownItems)
            {
                if (subItem is ToolStripMenuItem subMenuItem)
                {
                    EnableMenuItemRecursive(subMenuItem);
                }
            }
        }


        private void InitializeChartControl4()
        {
            try
            {
                // Execute SQL query to fetch data
                string query = @"SELECT [Salesman Code], SUM([Total Amount]) AS TotalSales 
                         FROM  [dbo].[AR_Tran_Main_tbl]
                         WHERE [Transaction Type] = 'Ri' 
                         GROUP BY [Salesman Code]";

                DataTable salesData = GetDataFromDatabase(query);

                // Create a series for the chart
                DevExpress.XtraCharts.Series seriesSales = new DevExpress.XtraCharts.Series("Total Sales", DevExpress.XtraCharts.ViewType.Bar);

                // Bind data to the series
                foreach (DataRow row in salesData.Rows)
                {
                    string salesmanCode = row["Salesman Code"].ToString();
                    double totalSales = Convert.ToDouble(row["TotalSales"]);

                    // Add data points to the series
                    seriesSales.Points.Add(new DevExpress.XtraCharts.SeriesPoint(salesmanCode, totalSales));
                }

                // Add the series to the chart
                chartControl4.Series.Add(seriesSales);

                // Customize the chart appearance if needed
                chartControl4.Legend.Visibility = DevExpress.Utils.DefaultBoolean.True;
                chartControl4.Legend.AlignmentHorizontal = DevExpress.XtraCharts.LegendAlignmentHorizontal.Right;
                chartControl4.Legend.AlignmentVertical = DevExpress.XtraCharts.LegendAlignmentVertical.Top;

                DevExpress.XtraCharts.ChartTitle chartTitle = new DevExpress.XtraCharts.ChartTitle();
                chartTitle.Text = "Total Sales by Salesman";
                chartTitle.Alignment = StringAlignment.Center;
                chartControl4.Titles.Add(chartTitle);

                // Refresh the chart to display the data
                chartControl4.RefreshData();
            }
            catch (Exception ex)
            {
                // Handle the exception here (e.g., show an error message)
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Method to execute SQL query and fetch data into a DataTable
        DataTable GetDataFromDatabase(string query)
        {
            DataTable dataTable = new DataTable();

            try
            {
                using (SqlConnection connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["connection_string"].ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        SqlDataAdapter adapter = new SqlDataAdapter(command);
                        adapter.Fill(dataTable);
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions here
                Console.WriteLine("Error executing SQL query: " + ex.Message);
            }

            return dataTable;
        }
        private void InitializeChartControl3()
        {
            try
            {
                // Fetch overall sum data for "Ri" and "RR" transactions
                double totalSales = GetTotalAmount(GetChartSalesAndCollectionData("Ri",null,null));
                double totalCollection = GetTotalAmount(GetChartSalesAndCollectionData("RR", null, null));

                // Create a series for the chart
                DevExpress.XtraCharts.Series series = new DevExpress.XtraCharts.Series("Sales and Collection", DevExpress.XtraCharts.ViewType.Pie3D);

                // Add data points to the series
                series.Points.Add(new DevExpress.XtraCharts.SeriesPoint("Sales", totalSales));
                series.Points.Add(new DevExpress.XtraCharts.SeriesPoint("Collection", totalCollection));

                // Set LegendTextPattern
                series.LegendTextPattern = "{A} - {V}";

                // Add the series to the chart
                chartControl3.Series.Add(series);

                // Customize the chart appearance if needed
                chartControl3.Legend.Visibility = DevExpress.Utils.DefaultBoolean.True;
                chartControl3.Legend.AlignmentHorizontal = DevExpress.XtraCharts.LegendAlignmentHorizontal.Right;
                chartControl3.Legend.AlignmentVertical = DevExpress.XtraCharts.LegendAlignmentVertical.Bottom;

                DevExpress.XtraCharts.ChartTitle chartTitle = new DevExpress.XtraCharts.ChartTitle();
                chartTitle.Text = "Sales And Collection";
                chartTitle.Alignment = StringAlignment.Center;
                chartControl3.Titles.Add(chartTitle);

                // Refresh the chart to display the data
                chartControl3.RefreshData();
            }
            catch (Exception ex)
            {
                // Handle the exception here (e.g., show an error message)
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //asssume to get the total amount
        double GetTotalAmount(DataTable dataTable)
        {
            double totalAmount = 0;
            foreach (DataRow row in dataTable.Rows)
            {
                // Check if the "Total Amount" column is not null or empty
                if (!string.IsNullOrEmpty(row["Total Amount"]?.ToString()))
                {
                    // Add the total amount to the running sum
                    totalAmount += Convert.ToDouble(row["Total Amount"]);
                }
            }
            return totalAmount;
        }
        private void InitializeSalesAndCollectionChart2()
        {
            try
            {
                // Fetch data for "Ri" and "RR" transactions grouped by fiscal month
                DataTable salesData = GetChartSalesAndCollectionData("Ri", null, null);
                DataTable collectionData = GetChartSalesAndCollectionData("RR", null, null);

                // Create two series for the chart
                DevExpress.XtraCharts.Series seriesSales = new DevExpress.XtraCharts.Series("Sales", DevExpress.XtraCharts.ViewType.Line);
                DevExpress.XtraCharts.Series seriesCollection = new DevExpress.XtraCharts.Series("Collection", DevExpress.XtraCharts.ViewType.Line);

                // Add the series to the chart
                chartControl2.Series.Add(seriesSales);
                chartControl2.Series.Add(seriesCollection);

                // Bind data to the series
                for (int month = 1; month <= 12; month++)
                {
                    string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);
                    double totalSales = GetTotalAmount(salesData, month);
                    double totalCollection = GetTotalAmount(collectionData, month);

                    // Add data points to the "Sales" series
                    seriesSales.Points.Add(new DevExpress.XtraCharts.SeriesPoint(monthName, totalSales));

                    // Add data points to the "Collection" series
                    seriesCollection.Points.Add(new DevExpress.XtraCharts.SeriesPoint(monthName, totalCollection));
                }
              

                // Customize the chart appearance if needed
                chartControl2.Legend.Visibility = DevExpress.Utils.DefaultBoolean.True;
                chartControl2.Legend.AlignmentHorizontal = DevExpress.XtraCharts.LegendAlignmentHorizontal.Right;
                chartControl2.Legend.AlignmentVertical = DevExpress.XtraCharts.LegendAlignmentVertical.Bottom;

              

                DevExpress.XtraCharts.ChartTitle chartTitle = new DevExpress.XtraCharts.ChartTitle();
                chartTitle.Text = "Sales And Collection";
                chartTitle.Alignment = StringAlignment.Center;
                chartControl2.Titles.Add(chartTitle);

                // Refresh the chart to display the data
                chartControl2.RefreshData();
            }
            catch (Exception ex)
            {
                // Handle the exception here (e.g., show an error message)
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void InitializeSalesAndCollectionChart()
        {
            try
            {
                // Fetch data for "Ri" and "RR" transactions grouped by fiscal month
                DataTable salesData = GetChartSalesAndCollectionData("Ri");
                DataTable collectionData = GetChartSalesAndCollectionData("RR");

                // Create two series for the chart
                seriesSales = new DevExpress.XtraCharts.Series("Sales", DevExpress.XtraCharts.ViewType.Bar);
                seriesCollection = new DevExpress.XtraCharts.Series("Collection", DevExpress.XtraCharts.ViewType.Bar);

                seriesSales.FilterCriteria = CriteriaOperator.Parse("[Document Date] >= #01/01/2022# AND [Document Date] <= #12/31/2022#");
                seriesCollection.FilterCriteria = CriteriaOperator.Parse("[Document Date] >= #01/01/2022# AND [Document Date] <= #12/31/2022#");
                // Add the series to the chart
                chartControl1.Series.Add(seriesSales);
                chartControl1.Series.Add(seriesCollection);

                // Bind data to the series
                for (int month = 1; month <= 12; month++)
                {
                    string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);
                    double totalSales = GetTotalAmount(salesData, month);
                    double totalCollection = GetTotalAmount(collectionData, month);

                    // Add data points to the "Sales" series
                    seriesSales.Points.Add(new DevExpress.XtraCharts.SeriesPoint(monthName, totalSales));

                    // Add data points to the "Collection" series
                    seriesCollection.Points.Add(new DevExpress.XtraCharts.SeriesPoint(monthName, totalCollection));
                }
             
                // Customize the chart appearance if needed
                chartControl1.Legend.Visibility = DevExpress.Utils.DefaultBoolean.True;
                chartControl1.Legend.AlignmentHorizontal = DevExpress.XtraCharts.LegendAlignmentHorizontal.Right;
                chartControl1.Legend.AlignmentVertical = DevExpress.XtraCharts.LegendAlignmentVertical.Top;

                DevExpress.XtraCharts.ChartTitle chartTitle = new DevExpress.XtraCharts.ChartTitle();
                chartTitle.Text = "Sales And Collection";
                chartTitle.Alignment = StringAlignment.Center;
                chartControl1.Titles.Add(chartTitle);
                // Refresh the chart to display the data
                chartControl1.RefreshData();
            }
            catch (Exception ex)
            {
                // Handle the exception here (e.g., show an error message)
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitlizeYearCombobox()
        {
            // Get the current year
            int currentYear = DateTime.Now.Year;

            // Populate the ComboBox cbFromYear and cbToYear with years from 2010 to the current year
            for (int year = 2010; year <= currentYear; year++)
            {
                cbFromYear.Items.Add(year);
                cbToYear.Items.Add(year);
            }

            // Set the default selected values for cbFromYear and cbToYear
            cbFromYear.SelectedItem = DateTime.Now.AddYears(-2).Year;
            cbToYear.SelectedItem = currentYear;
        }

        private void btnFilter_Click(object sender, EventArgs e)
        {
            // Get the selected years from the ComboBoxes
            int fromYear = (int)cbFromYear.SelectedItem;
            int toYear = (int)cbToYear.SelectedItem;

            // Fetch data for "Ri" and "RR" transactions grouped by fiscal month within the selected years
            DataTable salesData = GetChartSalesAndCollectionData("Ri", fromYear, toYear);
            DataTable collectionData = GetChartSalesAndCollectionData("RR", fromYear, toYear);

            // Clear the existing points in the series
            seriesSales.Points.Clear();
            seriesCollection.Points.Clear();

            // Bind data to the series
            for (int month = 1; month <= 12; month++)
            {
                string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);
                double totalSales = GetTotalAmount(salesData, month);
                double totalCollection = GetTotalAmount(collectionData, month);

                // Add data points to the "Sales" series
                seriesSales.Points.Add(new DevExpress.XtraCharts.SeriesPoint(monthName, totalSales));

                // Add data points to the "Collection" series
                seriesCollection.Points.Add(new DevExpress.XtraCharts.SeriesPoint(monthName, totalCollection));
            }

            // Refresh the chart to display the new data
            chartControl1.RefreshData();
        }
        // Method to fetch data for a given transaction type grouped by fiscal month
        DataTable GetChartSalesAndCollectionData(string transactionType, int? fromYear = null, int? toYear = null)
        {
            DataTable dataTable = new DataTable();

            try
            {
                // Establish a connection to the database
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query;

                    // Check if fromYear and toYear are null
                    if (fromYear == null || toYear == null)
                    {
                        // Calculate the date 2 years ago from the current date
                        DateTime twoYearsAgo = DateTime.Now.AddYears(-2);

                        // Use the first query
                        query = $@"SELECT SUM([Total Amount]) AS [Total Amount], MONTH([Document Date]) AS [Extracted Month]
                            FROM [dbo].[AR_Tran_Main_tbl]
                            WHERE [Transaction Type] = '{transactionType}'
                            AND [Document Date] >= @TwoYearsAgo
                            GROUP BY MONTH([Document Date])";
                    }
                    else
                    {
                        // Use the second query
                        query = $@"SELECT SUM([Total Amount]) AS [Total Amount], MONTH([Document Date]) AS [Extracted Month]
                            FROM [dbo].[AR_Tran_Main_tbl]
                            WHERE [Transaction Type] = '{transactionType}'
                            AND YEAR([Document Date]) >= @FromYear AND YEAR([Document Date]) <= @ToYear
                            GROUP BY MONTH([Document Date])";
                    }

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Set the parameters based on the query
                        if (fromYear == null || toYear == null)
                        {
                            command.Parameters.AddWithValue("@TwoYearsAgo", DateTime.Now.AddYears(-2));
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@FromYear", fromYear);
                            command.Parameters.AddWithValue("@ToYear", toYear);
                        }

                        // Open the connection
                        connection.Open();

                        // Execute the command and fill the DataTable with the result
                        SqlDataAdapter adapter = new SqlDataAdapter(command);
                        adapter.Fill(dataTable);
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions here
                Console.WriteLine("Error executing SQL query: " + ex.Message);
            }

            return dataTable;
        }

        // Helper method to calculate total amount for a specific month
        double GetTotalAmount(DataTable dataTable, int month)
        {
            double totalAmount = 0;
            foreach (DataRow row in dataTable.Rows)
            {
                // Check if the "Extracted Month" column is not null or empty
                if (!string.IsNullOrEmpty(row["Extracted Month"]?.ToString()))
                {
                    // Convert the "Extracted Month" value to an integer
                    int extractedMonth = Convert.ToInt32(row["Extracted Month"]);

                    // Check if the extracted month matches the specified month
                    if (extractedMonth == month)
                    {
                        // Add the total amount to the running sum
                        totalAmount += Convert.ToDouble(row["Total Amount"]);
                        return totalAmount;
                    }
                }
            }
            return totalAmount;
        }






        private void OpenOrBringToFrontForm<T>(ref T formInstance) where T : Form, new()
         {
            try
            {
                if (formInstance == null || formInstance.IsDisposed)
                {
                    // Form is not open or has been disposed, create a new instance
                    formInstance = new T();
                    formInstance.StartPosition = FormStartPosition.CenterScreen;
                    // Subscribe to the SaveButtonClicked event if the form has the event
                    if (formInstance is frmSetupUserMdiMain setupMdiMainForm)
                    {
                        setupMdiMainForm.SaveButtonClicked += saveButtonClickedHandler;
                    }
                    formInstance.Show();
                }
                else
                {
                    if (formInstance.WindowState == FormWindowState.Minimized)
                    {
                        // If the form is minimized, restore it to normal state
                        formInstance.WindowState = FormWindowState.Normal;
                    }
                    // Form is already open, bring it to the front
                    formInstance.BringToFront();
                }
        

            }
            catch (Exception ex)
            {
                // Log the exception with additional information
                ExceptionLogger.LogException(ex, nameof(mdiMain), "Std", "OpenOrBringToFrontForm", new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber());

            
              
            }
        }
        private void saveButtonClickedHandler()
        {
            // Call the InitializeTileGroup method when the save button is clicked in frmSetupMdiMain
            InitializeTileGroup();
        }
        private void GlobalKeyHandler_KeyPressed(Keys keyData)
        {
            // Handle F5 key press
            if (keyData == Keys.F5)
            {
                OpenOrBringToFrontForm(ref frm_AR_Delivery_Invoice_Counter);
            }
            // Handle F11 key press
            else if (keyData == Keys.F11)
            {
                OpenOrBringToFrontForm(ref transactionDetailAnalysisForm);
            }
        }
    
        private void MenuInvoiceCounter_Click(object senderr, EventArgs ee)
        {
            OpenOrBringToFrontForm(ref frm_AR_Delivery_Invoice_Counter);
        }

        private void MenuAMSalesman_Click(object sender, EventArgs e)
        {
            OpenOrBringToFrontForm(ref salesmanListForm);
            
        }
        private void MenuAMUserGroupAccess_Click(object sender, EventArgs e)
        {
            OpenOrBringToFrontForm(ref userGroupAccessForm);
       
        }
        private void MenuAMUserGroup_Click(object sender, EventArgs e)
        {
            OpenOrBringToFrontForm(ref frmUserGroupForm);

        }

        private void MenuTLReleaseLocking_Click(object sender, EventArgs e)
        {
            OpenOrBringToFrontForm(ref lockReleaseForm);

        }
        private void MenuLogOut_Click(object sender, EventArgs e)
        {
            keyHandler.KeyPressed -= GlobalKeyHandler_KeyPressed;
            LicenseControl.DecreaseConcurrentUsing(modPublicVariable.Company, modPublicVariable.CompanyName);
            // Unload mdiMain
            this.Hide();
            // Close all existing forms or controls
            CloseAllOpenForms();

            frmLogin frmLogin = new frmLogin();
            frmLogin.StartPosition = FormStartPosition.CenterScreen;
            frmLogin.ShowDialog();
            this.Close();
            this.Dispose();
          

        }


        private void InitializeTileGroup()
        {
            foreach (TileGroup group in tileControl1.Groups)
            {
                group.Items.Clear();
            }



            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // Check if there are records in [Menu_UserMainScreen] for the specified menuId and user
                connection.Open();
                string countQuery = "SELECT COUNT(*) FROM [Menu_UserMainScreen] WHERE [menu id1] = @MenuId AND [GUSER_ID] = @UserName";
                SqlCommand countCommand = new SqlCommand(countQuery, connection);
                countCommand.Parameters.AddWithValue("@MenuId", menuId);
                countCommand.Parameters.AddWithValue("@UserName", modPublicVariable.UserID);
                int recordCount = (int)countCommand.ExecuteScalar();

                // Determine which query to use based on record count
                for (int i = 1; i <= 5; i++)
                {
                    string query = "";
                    if (recordCount > 0)
                    {
                        // Query when records exist in [Menu_UserMainScreen] for the user
                        query = $@"
                        SELECT [Menu Name], [Menu Caption], [Menu Image], [Item Size], [Menu Color], [Form Name]
                        FROM [Menu_UserMainScreen]
                        WHERE [menu id1] = @MenuId AND [GUSER_ID] = @UserName AND [Group ID] = {i} 
                        ORDER BY CAST([Item ID] AS INT) ASC";
                    }
                    else
                    {
                        // Query when no records exist in [Menu_UserMainScreen] for the user, fall back to the default query
                        query = $@"
                        SELECT [Menu Name], [Menu Caption], [Menu Image], [Item Size], [Menu Color], [Form Name]
                        FROM [Menu_MainScreen]
                        WHERE [menu id1] = @MenuId AND [Group ID] = {i} 
                        ORDER BY CAST([Item ID] AS INT) ASC";
                    }

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@MenuId", menuId); // Assuming this.menuId is set somewhere
                        command.Parameters.AddWithValue("@UserName", modPublicVariable.UserID);

                        try
                        {
                            SqlDataReader reader = command.ExecuteReader();

                            while (reader.Read())
                            {
                                string itemSize = reader["Item Size"].ToString();

                                if (itemSize.Equals("Big", StringComparison.OrdinalIgnoreCase))
                                {
                                    AddBigTileItem(reader, i);
                                }
                                else if (itemSize.Equals("Medium", StringComparison.OrdinalIgnoreCase))
                                {
                                    AddMediumTileItem(reader, i);
                                }
                            }

                            reader.Close();
                        }
                        catch (Exception ex)
                        {
                            // Handle exceptions
                            Console.WriteLine("Error: " + ex.Message);
                        }
                        finally
                        {
                            command.Parameters.Clear(); // Clear parameters for the next iteration
                        }
                    }
                }
            }

            UserAccessControl(modPublicVariable.UserID);
        }


        private void AddBigTileItem(SqlDataReader reader, int i)
        {
            string menuCaption = reader["Menu Caption"].ToString();
            string svgImageKey = reader["Menu Image"].ToString();
            string menuName = reader["Menu Name"].ToString();
            Color menuColor = ColorTranslator.FromHtml(reader["Menu Color"].ToString());
            string formName = reader["Form Name"].ToString();
            TileItem tileItem = new TileItem();
            tileItem.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.True;
            tileItem.ItemSize = DevExpress.XtraEditors.TileItemSize.Wide;
            tileItem.Name = menuName;
            tileItem.Enabled = false;
            tileItem.AppearanceItem.Normal.BackColor = menuColor;
            tileItem.AppearanceItem.Normal.BorderColor = menuColor;
            tileItem.AppearanceItem.Normal.Options.UseBackColor = true;
            tileItem.AppearanceItem.Normal.Options.UseBorderColor = true;

            TileItemElement tileItemElement = new TileItemElement();
            tileItemElement.Appearance.Normal.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold);
            tileItemElement.Appearance.Normal.Options.UseFont = true;
            tileItemElement.ImageOptions.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.TopCenter;
            tileItemElement.ImageOptions.ImageSize = new System.Drawing.Size(150, 150);
            tileItemElement.ImageOptions.SvgImage = ImageResourceCache.Default.GetSvgImage(svgImageKey);
            tileItemElement.ImageOptions.SvgImageSize = new System.Drawing.Size(70, 70);
            tileItemElement.Text = menuCaption;
            tileItemElement.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.BottomCenter;
            tileItemElement.TextLocation = new System.Drawing.Point(3, 0);
            tileItem.Elements.Add(tileItemElement);

            string tileGroupName = "tileGroup" + i.ToString();
            FieldInfo field = this.GetType().GetField(tileGroupName, BindingFlags.Instance | BindingFlags.NonPublic);
            if (field != null)
            {
                DevExpress.XtraEditors.TileGroup tileGroup = (DevExpress.XtraEditors.TileGroup)field.GetValue(this);
                tileGroup.Items.Add(tileItem);
            }

            // Open the form when the tile item is clicked
            tileItem.ItemClick += (sender, e) =>
            {
                if (e.Item == tileItem)
                {
                    OpenFormByFormName(formName);
                }
            };
        }

        private void AddMediumTileItem(SqlDataReader reader, int i)
        {
            string menuCaption = reader["Menu Caption"].ToString();
            string svgImageKey = reader["Menu Image"].ToString();
            string menuName = reader["Menu Name"].ToString();
            Color menuColor = ColorTranslator.FromHtml(reader["Menu Color"].ToString());
            string formName = reader["Form Name"].ToString();
            TileItem tileItem = new TileItem();
            tileItem.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.True;
            tileItem.ItemSize = DevExpress.XtraEditors.TileItemSize.Medium;
            tileItem.Name = menuName;
            tileItem.Enabled = false;    
            tileItem.AppearanceItem.Normal.BackColor = menuColor;
            tileItem.AppearanceItem.Normal.BorderColor = menuColor;
            tileItem.AppearanceItem.Normal.Options.UseBackColor = true;
            tileItem.AppearanceItem.Normal.Options.UseBorderColor = true;

            TileItemElement tileItemElement = new TileItemElement();
            tileItemElement.ImageOptions.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
            tileItemElement.ImageOptions.ImageSize = new System.Drawing.Size(150, 150);
            tileItemElement.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.TileControlImageToTextAlignment.Top;
            tileItemElement.ImageOptions.SvgImage = ImageResourceCache.Default.GetSvgImage(svgImageKey);
            tileItemElement.Text = menuCaption;
            tileItemElement.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.Manual;
            tileItemElement.Appearance.Normal.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            tileItemElement.TextLocation = new System.Drawing.Point(4, 0);
            tileItem.Elements.Add(tileItemElement);

            string tileGroupName = "tileGroup" + i.ToString();
            FieldInfo field = this.GetType().GetField(tileGroupName, BindingFlags.Instance | BindingFlags.NonPublic);
            if (field != null)
            {
                DevExpress.XtraEditors.TileGroup tileGroup = (DevExpress.XtraEditors.TileGroup)field.GetValue(this);
                tileGroup.Items.Add(tileItem);
            }

            // Open the form when the tile item is clicked
            // Handle item click event
            tileItem.ItemClick += (sender, e) =>
            {
                if (e.Item == tileItem)
                {
                    OpenFormByFormName(formName);
                }
            };
        }
        private void OpenFormByFormName(string formName)
        {
            try
            {
                // Get the field in the class that matches the formName
                FieldInfo formField = this.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
                    .FirstOrDefault(f => f.FieldType.IsSubclassOf(typeof(Form)) && f.Name.StartsWith(formName));
                if (formField != null)
                {
                    // Get the form instance from the field
                    Form formInstance = (Form)formField.GetValue(this);

                    // If the form instance is null or disposed, create a new instance
                    if (formInstance == null || formInstance.IsDisposed)
                    {
                        formInstance = (Form)Activator.CreateInstance(formField.FieldType);
                        formField.SetValue(this, formInstance); // Set the new instance to the field
                    }

                    // Open or bring the form to front
                    if (formInstance.WindowState == FormWindowState.Minimized)
                    {
                        formInstance.WindowState = FormWindowState.Normal;
                    }
                    formInstance.BringToFront();
                    if (!formInstance.Visible)
                    {
                        formInstance.StartPosition = FormStartPosition.CenterScreen;
                        formInstance.Show();
                    }
                }
                else
                {
                    MessageBox.Show($"Form instance for '{formName}' not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening form '{formName}': {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private DevExpress.XtraEditors.TileItemSize GetTileItemSize(string itemSize)
        {
            switch (itemSize.ToLower())
            {
                case "big":
                    return DevExpress.XtraEditors.TileItemSize.Wide;
                case "medium":
                    return DevExpress.XtraEditors.TileItemSize.Medium;
                case "small":
                    return DevExpress.XtraEditors.TileItemSize.Small;
                default:
                    return DevExpress.XtraEditors.TileItemSize.Default;
            }
        }
        private void CloseAllOpenForms()
        {
            try
            {
                // Create a list to hold forms that need to be closed
                List<Form> formsToClose = new List<Form>();

                // Identify forms to be closed without modifying the OpenForms collection
                foreach (Form form in Application.OpenForms)
                {
                    if (form != this && form.GetType() != typeof(frmLogin) && form.GetType() != typeof(frmMultiCompanies))
                    {
                        formsToClose.Add(form);
                    }
                }

                // Close the identified forms
                foreach (Form form in formsToClose)
                {
                    form.Close();

                }
               
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex, nameof(mdiMain), "Std", "CloseAllOpenForms", new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
               
                // Handle the exception, log it, or show an error message
                MessageBox.Show("An error occurred while closing forms: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                
            }
        }




        private void UserAccessControl(string userName)
        {
          

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    // Open the connection
                    connection.Open();

                    // Define the SQL query
                    string query = @"SELECT GUXCS_TBL.[Group ID], GXCS_TBL.GXCS_PRGID, GXCS_TBL.GXCS_GRDID, GXCS_TBL.GXCS_BLOCK, GPRG_TBL.GPRG_MENU, GPRG_TBL.GPRG_INDEX
                      FROM GXCS_TBL
                      INNER JOIN GUXCS_TBL ON GXCS_TBL.GXCS_GRDID = GUXCS_TBL.[Group ID]
                      INNER JOIN GUSER_TBL ON GUXCS_TBL.[User ID] = GUSER_TBL.GUSER_ID
                      INNER JOIN GPRG_TBL ON GXCS_TBL.GXCS_PRGID = GPRG_TBL.GPRG_ID 
                          AND GXCS_TBL.[GXCS_MDLID] = GPRG_TBL.GPRG_MDLID 
                          AND GXCS_TBL.[GXCS_MENU] = GPRG_TBL.GPRG_MENU
                      WHERE (GUSER_TBL.GUSER_ID = @UserName) AND (GXCS_TBL.GXCS_BLOCK = 1)
                      GROUP BY GUXCS_TBL.[Group ID], GXCS_TBL.GXCS_PRGID, GXCS_TBL.GXCS_GRDID,
                               GXCS_TBL.GXCS_BLOCK, GXCS_TBL.GXCS_PRGID, GXCS_TBL.GXCS_MDLID,
                               GPRG_TBL.GPRG_MENU, GPRG_TBL.GPRG_INDEX";

                    // Create SqlCommand and set parameters
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@UserName", userName);

                    // Execute the query
                    SqlDataReader reader = command.ExecuteReader();

                    // Process the retrieved data
                    while (reader.Read())
                    {
                        string menuName = reader["GPRG_MENU"].ToString();
                        EnableMenuItem(menuName, MenuStrip1.Items);
                        string menuNameWith1 = menuName + "1";
                        EnableAccordionItem(menuNameWith1, accordionControl1.Elements);
                        EnableTileItemByName(menuName);
                    }

                    // Close the reader
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex, nameof(mdiMain), "Std", "UserAcessControl", new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        // Call this method passing the name of the TileItem you want to enable
        private void EnableTileItemByName(string itemName)
        {
            foreach (TileGroup group in tileControl1.Groups)
            {
                EnableTileItem(itemName, group.Items);
            }
        }
        private void EnableTileItem(string itemName, TileItemCollection items)
        {
            foreach (TileItem item in items)
            {
                if (item.Name == itemName)
                {
                    item.Enabled = true;
                    return;
                }
            }
        }

        private void EnableAccordionItem(string menuName, AccordionControlElementCollection elements)
        {
            foreach (AccordionControlElement element in elements)
            {
                if (element is AccordionControlElement accordionElement)
                {
                    if (accordionElement.Name == menuName)
                    {
                        accordionElement.Enabled = true;
                        return;
                    }
                    else
                    {
                        EnableAccordionItem(menuName, accordionElement.Elements);
                    }
                }
            }
        }
        private void EnableMenuItem(string menuName, ToolStripItemCollection items)
        {
            foreach (ToolStripItem menuItem in items)
            {
                if (menuItem is ToolStripMenuItem menu)
                {
                    if (menu.Name == menuName)
                    {
                        menu.Enabled = true;
                        return;
                    }
                    else
                    {
                        EnableMenuItem(menuName, menu.DropDownItems);
                    }
                }
            }
        }

        private void MenuAMGridDetailSetting_Click(object sender, EventArgs e)
        {
            OpenOrBringToFrontForm(ref gridDetailSettingForm);
         
        }

        private void MenuAMLookupSetting_Click(object sender, EventArgs e)
        {
            OpenOrBringToFrontForm(ref lookupSettingForm);
        }

        private void MenuAMUserSetup_Click(object sender, EventArgs e)
        {
            OpenOrBringToFrontForm(ref userSetupForm);
      
        }

        private void accordionControlElement2_Click(object sender, EventArgs e)
        {

        }

     


        private void MenuGLGeneralLedger1_Click(object sender, EventArgs e)
        {
           

        }

        private void MenuAMMaintenance1_Click(object sender, EventArgs e)
        {
            this.menuId = "FAME_AM";
            InitializeTileGroup();
        }

        private void accordionControlElement5_Click(object sender, EventArgs e)
        {

        }

        private void MenuARReceivables2_Click(object sender, EventArgs e)
        {
          
            this.menuId = "FAME_AR";
            InitializeTileGroup();

        }

        private void MenuSystemCustomize_Click(object sender, EventArgs e)
        {
            OpenOrBringToFrontForm(ref setupMdiMainForm);
        }

        private void tileControl1_Click(object sender, EventArgs e)
        {

        }

        private void MenuExit_Click(object sender, EventArgs e)
        {
            LicenseControl.DecreaseConcurrentUsing(modPublicVariable.Company, modPublicVariable.CompanyName);
        }

        private void tileControl1_Click_1(object sender, EventArgs e)
        {

        }

   

        private void navButton1_ElementClick(object sender, NavElementEventArgs e)
        {

        }

        private void navBtnMenu_ElementClick_1(object sender, NavElementEventArgs e)
        {
            visibleMenu();
        }

        private void navBtnDashboard_ElementClick_1(object sender, NavElementEventArgs e)
        {
            visibleDashboard();
        }
        
        private void visibleMenu()
        {
            chartControl1.Visible = false;
            tileControl1.Visible = true;
            tableLayoutPanel2.Visible = false;
            tableLayoutPanel3.Visible = false;
            tileControlPanel.Visible = false;
        }
        private void visibleDashboard()
        {

            tileControl1.Visible = false;
            chartControl1.Visible = true;
            tableLayoutPanel2.Visible = true;
            tileControlPanel.Visible = true;
            tableLayoutPanel3.Visible = true;
        }

        private void LoadMenuSettings()
        {
            try
            {
                // Check if the GUSER_ID exists for the current user
                if (CheckUserExistence(modPublicVariable.UserID))
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        string query = "SELECT [DisplayMode], [DisplayModule], [GUSER_ID] FROM [Menu_Setting] WHERE [GUSER_ID] = @UserId";
                        SqlCommand command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@UserId", modPublicVariable.UserID);

                        SqlDataReader reader = command.ExecuteReader();

                        if (reader.Read())
                        {
                            string displayMode = reader["DisplayMode"].ToString();
                            string displayModule = reader["DisplayModule"].ToString();
                            string guserId = reader["GUSER_ID"].ToString();

                            // Check if DisplayMode is "Dashboard"
                            if (displayMode == "Dashboard")
                            {
                                visibleDashboard();
                            }
                            else
                            {
                                visibleMenu();
                            }

                            this.menuId = displayModule;
                        }
                        

                        reader.Close();
                    }
                }
            
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
        private bool CheckUserExistence(string userId)
        {
            bool exists = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string checkQuery = "SELECT COUNT(*) FROM [Menu_Setting] WHERE [GUSER_ID] = @UserId";
                    SqlCommand checkCommand = new SqlCommand(checkQuery, connection);
                    checkCommand.Parameters.AddWithValue("@UserId", userId);

                    int count = (int)checkCommand.ExecuteScalar();
                    exists = count > 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error checking user existence: " + ex.Message);
            }
            return exists;
        }
        private void navBtnSetting_ElementClick(object sender, NavElementEventArgs e)
        {

        }

       
    }
}
