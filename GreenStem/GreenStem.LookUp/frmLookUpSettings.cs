using DevExpress.CodeParser;
using DevExpress.Utils;
using DevExpress.Utils.Behaviors;
using DevExpress.Utils.DragDrop;
using DevExpress.Xpo.DB.Helpers;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraReports.Parameters;
using GreenStem.ClassModules;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;


namespace GreenStem.LookUp
{
    public partial class frmLookUpSettings : DevExpress.XtraBars.Ribbon.RibbonForm
    {

        private bool isNewRow = false;
        private string _lookupName; // Rename the private field
        private string _tableName;
        private string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connection_string"].ConnectionString;
        private SqlCommand command;
        private SqlTransaction transaction;
        private string returnField;
        private string code;
        private int originalPanel1Width;
        private int originalPanel2Width;
        private int originalPanel2Left;
        private int originalBtnForeColorLeft;
        private int originalFontLeft;
        Dictionary<string, string> columnMappings = new Dictionary<string, string>();
        private ArrayList deleteFieldNames = new ArrayList();
        HashSet<DataRow> addedRows = new HashSet<DataRow>();
        public frmLookUpSettings()
        {

            initializeComponent();


        }
        private void SettingLookUp_Click(object sender, EventArgs e)
        {

            frmLookUp.OpenLookupForm("10040", HandleDataSelectedEvent, this,false);
        }
        public frmLookUpSettings(string code)
        {

            this.code = code;
            initializeComponent();
            LoadDataForSelectedCode(code);
            // Load data into the grid using the retrieved code
            LoadDataIntoGrid(code);
      

        }


        public void initializeComponent()
        {
            InitializeComponent();
            // Store the original widths when the form loads
            initializeOriginalPosition();
            gridView1.CustomDrawCell += gridView1_CustomDrawCell;
            InitializeGridControl1DragDrop();
            gridView1.KeyDown += gridView1_KeyDown;
            // Attach the CellValueChanged event handler
            gridView1.CellValueChanged += gridView1_CellValueChanged;
            // Handle the form size changed event
            this.SizeChanged += LookUpSettingsForm_SizeChanged;
            AdjustView();
        
        }
   
        private void gridView1_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {


            object cellValue = gridView1.GetRowCellValue(e.RowHandle, e.Column);
            Console.WriteLine($"Cell Valueinrowcell: {cellValue}");

        }
        // Add a method to retrieve [Next Line] and [Next Column] values from the database
        private (bool, bool) GetNextLineAndColumnValues(string columnName, string selectedCode)
        {
            bool nextLine = false;
            bool nextColumn = false;

            // Replace the connection string with your actual database connection string
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Create a SqlCommand to fetch [Next Line] and [Next Column] values
                string query = $"SELECT [Next Line], [Next Column] FROM [Grid_Detail_Tbl] WHERE [Code] = '{selectedCode}' AND [Column Name] = '{columnName}'";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        // Read [Next Line] and [Next Column] values
                        nextLine = Convert.ToBoolean(reader["Next Line"]);
                        nextColumn = Convert.ToBoolean(reader["Next Column"]);
                    }
                }
            }

            return (nextLine, nextColumn);
        }

        private void gridView1_KeyDown(object sender, KeyEventArgs e)
        {
            // Your logic for handling key presses, e.g., delete a row
            if (e.KeyCode == Keys.Delete)
            {
                // Call the method to delete the row
                HandleDeleteKeyPressed();
            }
            else if (e.KeyCode == Keys.Insert) // Check if the Insert key is pressed
            {
                // Add a new row when the Insert key is pressed
                gridView1.AddNewRow();
                e.Handled = true; // Prevent further processing of the key event
            }
            else if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
            {
                try
                {
                    int focusedRowHandle = gridView1.FocusedRowHandle;

                    // Check if gridView1.FocusedColumn is not null before accessing its properties
                    if (gridView1.FocusedColumn != null)
                    {
                        int focusedColumnHandle = gridView1.FocusedColumn.VisibleIndex;

                        string focusedColumnName = gridView1.FocusedColumn.FieldName;
                        string selectedCode = "1110";
                        //update the current cell value
                        gridView1.PostEditor();
                        gridView1.UpdateCurrentRow();

                        // Check if the current cell is empty
                        Console.WriteLine($"Cell Value: {gridView1.GetRowCellValue(focusedRowHandle, focusedColumnName)}");
                        object cellValue = gridView1.GetRowCellValue(focusedRowHandle, focusedColumnName);

                        // Check if the cell is empty
                        if (string.IsNullOrEmpty(cellValue?.ToString()))
                        {
                            string errorMessage = $"{focusedColumnName} cannot be empty.";
                            MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                            // Prevent moving to the next cell
                            e.Handled = true;
                            return;
                        }
                        // Get the [Next Line] and [Next Column] values for the focused column from the database
                        (bool nextLine, bool nextColumn) = GetNextLineAndColumnValues(focusedColumnName, selectedCode);

                        if (nextLine)
                        {
                            // Check if there's already an empty new row
                            int emptyNewRowIndex = gridView1.LocateByValue(0, gridView1.Columns["Field Name"], DBNull.Value);

                            Console.WriteLine(emptyNewRowIndex);
                            if (emptyNewRowIndex != DevExpress.XtraGrid.GridControl.InvalidRowHandle)
                            {
                                // Set focus to the empty new row
                                gridView1.FocusedRowHandle = emptyNewRowIndex;
                                gridView1.FocusedColumn = gridView1.VisibleColumns[1];
                            }
                            else
                            {
                                // Open a new row and focus on the next column
                                gridView1.AddNewRow();
                                gridView1.FocusedColumn = gridView1.VisibleColumns[1];
                            }
                        }
                        else if (nextColumn)
                        {
                            // Focus on the next column
                            gridView1.FocusedColumn = GetNextVisibleColumnWithNextColumnTrue(gridView1, focusedColumnHandle + 1);
                        }
                        else
                        {
                            // Start editing the current cell
                            gridView1.ShowEditor();

                            // Move to the next row
                            gridView1.MoveNext();
                        }

                    }

                    else
                    {
                        // Handle the case where gridView1.FocusedColumn is null
                        MessageBox.Show("It Might Due To That You Havent Set The Focus To Yes When Next Line Is Yes, Please Go To The Grid Detail Setting To Change", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private GridColumn GetNextVisibleColumnWithNextColumnTrue(DevExpress.XtraGrid.Views.Grid.GridView gridView, int startColumnIndex)
        {
            for (int i = startColumnIndex; i < gridView.VisibleColumns.Count; i++)
            {
                GridColumn column = gridView.VisibleColumns[i];
                string columnName = column.FieldName;
                string selectedCode = "1110"; 

                // Get the [Next Line] and [Next Column] values for the current column from the database
                (bool nextLine, bool nextColumn) = GetNextLineAndColumnValues(columnName, selectedCode);

                // If nextColumn is true, return this column
                if (nextColumn)
                {
                    return column;
                }
            }

            // If no nextColumn is found, return null
            return null;
        }


        private void HandleDeleteKeyPressed()
        {
            int focusedRowHandle = gridView1.FocusedRowHandle;

            if (focusedRowHandle >= 0 && focusedRowHandle < gridView1.DataRowCount)
            {
                string fieldName = gridView1.GetRowCellValue(focusedRowHandle, "Field Name") as string;

                // DeleteRowFromDatabase(primaryKeyValue);
                gridView1.DeleteRow(focusedRowHandle);
                this.deleteFieldNames.Add(fieldName);

            }
        }
        private void LookUpSettingsForm_SizeChanged(object sender, EventArgs e)
        {

            AdjustView();
        }

        private void AdjustView()
        {
            // You can set a threshold value for the screen width to decide whether to enable auto-width or not
            int screenWidthThreshold = 1200; // Adjust as needed

            if (this.Width <= screenWidthThreshold)
            {
                gridView1.OptionsView.ColumnAutoWidth = false;

                // Reset all components to their original state
                ResetToOriginalState();
            }
            else
            {
                // Adjust the components for a larger screen
                ExpandSizeForLargerScreen();

                gridView1.OptionsView.ColumnAutoWidth = true;
            }
        }

        private void LoadDataIntoGrid(string selectedCode)
        {
            // Replace the connection string with your actual database connection string



            try
            {
                // Create a SqlConnection
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Call GetGroupColumns to get the list of group columns
                    List<string> groupColumns = GetGroupColumns(connection);

                    // Create a dictionary to dynamically map [LookupSetting_Detail_Tbl] group columns to desired headers

                    columnMappings.Clear();
                    for (int i = 0; i < groupColumns.Count; i++)
                    {
                        string columnName = groupColumns[i];
                        columnMappings.Add("Group" + (i + 1), columnName);
                    }

                    // Create a SqlDataAdapter to fetch data from the database using the provided query
                    string gridQuery = $"SELECT [Field Name], [Column Name], [Column Width], [Column Length], [Col Alignment], " +
                        string.Join(", ", columnMappings.Keys.Select(c => $"CASE WHEN [{c}] IS NOT NULL AND [{c}] != '' THEN 1 ELSE 0 END AS [{columnMappings[c]}]")) +
                        $" FROM [LookupSetting_Detail_Tbl] WHERE [Code] = '{selectedCode}' " +
                        $" ORDER BY [Column Index]";

                    SqlDataAdapter dataAdapter = new SqlDataAdapter(gridQuery, connection);

                    // Create a DataTable to hold the data
                    DataTable dataTable = new DataTable();
                    // Add a column for the row numbers
                    dataTable.Columns.Add("No", typeof(int));
                    dataTable.Columns["No"].AutoIncrement = true;
                    dataTable.Columns["No"].AutoIncrementSeed = 1;
                    dataTable.Columns["No"].AutoIncrementStep = 1;
                    // initialize other field name

                    dataTable.Columns.Add("Field Name", typeof(string));
                    dataTable.Columns.Add("Column Name", typeof(string));
                    dataTable.Columns.Add("Column Width", typeof(string));
                    dataTable.Columns.Add("Column Length", typeof(string));
                    dataTable.Columns.Add("Col Alignment", typeof(string));

                    for (int i = 0; i < groupColumns.Count; i++)
                    {
                        string columnName = groupColumns[i];
                        dataTable.Columns.Add(columnName, typeof(bool));
                    }
                    // Fill the DataTable with data from the database
                    dataAdapter.Fill(dataTable);
                    // Reorder columns to ensure group columns are displayed at the end

                    gridControl1.DataSource = dataTable;


                    DataRow newRow = dataTable.NewRow();
                    // Set other column values
                    dataTable.Rows.Add(newRow);

                    // Track the newly added row
                    addedRows.Add(newRow);

                    // Assuming you have the columns added to the GridView
                    GridColumn colFieldName = gridView1.Columns["Field Name"];
                    GridColumn colColumnName = gridView1.Columns["Column Name"];

                    // // Create a ComboBox repository item for the "Field Name" column
                    RepositoryItemComboBox repositoryItemComboBoxFieldName = new RepositoryItemComboBox();

                    string lookupQuery = $"SELECT DISTINCT [Table Name], [Lookup Name], [Code] FROM [LookupSetting_Detail_Tbl] WHERE [Code] = '{selectedCode}'";

                    try
                    {
                        using (SqlCommand command = new SqlCommand(lookupQuery, connection))
                        {
                            SqlDataReader reader = command.ExecuteReader();

                            if (reader.Read())
                            {
                                string tableName = reader["Table Name"].ToString();
                                string lookupName = reader["Lookup Name"].ToString();


                                this._lookupName = lookupName; // Use the renamed private field
                                this._tableName = tableName; // Use the renamed private field
                                string code = reader["Code"].ToString();

                                // Store values before closing the reader
                                reader.Close();

                                if (!string.IsNullOrEmpty(tableName))
                                {
                                    // Now that you have the table name, use it in the column query
                                    string columnQuery = $"SELECT [Column_Name] FROM INFORMATION_SCHEMA.COLUMNS WHERE [TABLE_NAME] = '{tableName}'";

                                    using (SqlCommand fieldNameCommand = new SqlCommand(columnQuery, connection))
                                    {
                                        SqlDataReader fieldNameReader = fieldNameCommand.ExecuteReader();
                                        while (fieldNameReader.Read())
                                        {
                                            string fieldName = fieldNameReader["Column_Name"].ToString();
                                            cboReturnField.Items.Add($"[{fieldName}]");

                                            repositoryItemComboBoxFieldName.Items.Add($"[{fieldName}]");
                                        }
                                    }
                                }
                                if (!string.IsNullOrEmpty(returnField))
                                {
                                    cboReturnField.SelectedItem = returnField;

                                }


                                else
                                {
                                    Console.WriteLine("Table name not found.");
                                }

                            }
                            else
                            {
                                Console.WriteLine("No rows found.");
                            }
                        }
                
                        gridControl1.RepositoryItems.Add(repositoryItemComboBoxFieldName);
                        colFieldName.ColumnEdit = repositoryItemComboBoxFieldName;

                     
                        // Dynamically obtain the column name from the DataTable schema
                        string colAlignmentColumnName = dataTable.Columns["Col Alignment"].ColumnName;

                        GridColumn colAlignment = gridView1.Columns["Col Alignment"];
                        // Create a ComboBox repository item for the "Column Align" column
                        RepositoryItemComboBox repositoryItemComboBoxColumnAlign = new RepositoryItemComboBox();
                        repositoryItemComboBoxColumnAlign.Items.Add("Left");
                        repositoryItemComboBoxColumnAlign.Items.Add("Center");
                        repositoryItemComboBoxColumnAlign.Items.Add("Right");
                        repositoryItemComboBoxColumnAlign.EditValueChanged += RComboBoxEdit_EditValueChanged;
                        gridControl1.RepositoryItems.Add(repositoryItemComboBoxColumnAlign);
                        colAlignment.ColumnEdit = repositoryItemComboBoxColumnAlign;

                        SetColumnWidths();
                    }

                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        ExceptionLogger.LogException(ex, nameof(frmLookUpSettings), "LookUp", "LoadDataIntoGrid", new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ExceptionLogger.LogException(ex, nameof(frmLookUpSettings), "LookUp", "LoadDataIntoGrid", new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
            }
        }
        private void RComboBoxEdit_EditValueChanged(object sender, EventArgs e)
        {
            if (gridView1.PostEditor())
                gridView1.UpdateCurrentRow();
        }
        private void SetColumnWidths()
        {
            // Create a SqlConnection
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Replace the query with the actual query to fetch column widths from the database
                string query = "SELECT [Column Name], [Column Width] FROM [Grid_Detail_Tbl] WHERE [Code] = 1110";

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
                    int columnWidth = Convert.ToInt32(row["Column Width"]);

                    // Find the column in gridView1 and set its width
                    if (gridView1.Columns[columnName] != null)
                    {
                        gridView1.Columns[columnName].Width = columnWidth;
                    }
                }
            }
        }
        private List<string> GetGroupColumns(SqlConnection connection)
        {
            // Fetch column headers from GGRP_TBL
            List<string> groupColumns = new List<string>();

            string groupColumnsQuery = "SELECT * FROM GGRP_TBL";
            int count = 1;
            using (SqlCommand groupColumnsCommand = new SqlCommand(groupColumnsQuery, connection))
            {
                using (SqlDataReader groupColumnsReader = groupColumnsCommand.ExecuteReader())
                {
                    while (groupColumnsReader.Read())
                    {
                        string columnName = groupColumnsReader["GGRP_ID"].ToString();
                        groupColumns.Add(columnName);
                        count++;
                    }
                }
            }

            return groupColumns;
        }
        private void LoadDataForSelectedCode(string selectedCode)
        {

            string query = $"SELECT * FROM [LookupSetting_Tbl] WHERE Code = '{selectedCode}'";
            string orderBy = "";


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
                            // Update UI controls with the retrieved data
                            txtCode.Text = reader["Code"].ToString();
                            txtLookUpName.Text = reader["Lookup Name"].ToString();
                            txtDescription.Text = reader["Description"].ToString();
                            txtTableName.Text = reader["Table Name"].ToString();
                            txtRemark.Text = reader["Remark"].ToString();
                            txtFontName.Text = reader["FontName"].ToString();
                            txtFontStyle.Text = reader["FontStyle"].ToString();
                            txtFontSize.Text = reader["FontSize"].ToString();

                            int foreColorValue;
                            if (!Int32.TryParse(reader["ForeColor"].ToString(), out foreColorValue))
                            {
                                Console.WriteLine("Error: ForeColor value could not be converted to an integer.");
                                Color foreColor = Color.FromArgb(foreColorValue);

                                // Apply the color to the PictureBox
                                PictureBox1.BackColor = foreColor;
                            }

                            // Add items to the ComboBox
                            cbosortType.Items.Add("Ascending");
                            cbosortType.Items.Add("Descending");

                            // Set the selected item based on the value in the database
                            if (reader["Sort Type"].ToString() == "1")
                            {
                                cbosortType.SelectedItem = "Descending";
                            }
                            else
                            {
                                cbosortType.SelectedItem = "Ascending";
                            }
                            this.returnField = reader["Return Field"].ToString();
                             orderBy = reader["Order By"].ToString();

                            // Check if orderBy contains square brackets
                            if (!orderBy.StartsWith("[") && !orderBy.EndsWith("]"))
                            {
                                // Add square brackets around orderBy
                                orderBy = $"[{orderBy}]";
                            }

                        }

                        reader.Close(); // Close the reader after retrieving data
                    }

                    // Fetch the Field Names from LookupSetting_Detail_Tbl based on the selected code
                    string fieldQuery = $"SELECT [Column Name], [Field Name] FROM [LookupSetting_Detail_Tbl] WHERE [Code] = {selectedCode}";
                    List<string> fieldNames = new List<string>();

                    using (SqlCommand fieldCommand = new SqlCommand(fieldQuery, connection))
                    {
                        SqlDataReader fieldReader = fieldCommand.ExecuteReader();

                        while (fieldReader.Read())
                        {
                            string fieldName = fieldReader["Field Name"].ToString();
                            string columnName = fieldReader["Column Name"].ToString();
                            // Exclude "SearchField" from being added to the list
                            if (!columnName.Equals("SearchField", StringComparison.OrdinalIgnoreCase))
                            {
                                fieldNames.Add(fieldName);
                            }
                        }
                    }

                    // Update the ComboBox with the fetched Field Names
                    cboOrderby.DataSource = fieldNames;
                    cboOrderby.SelectedItem = orderBy;

                    //cboReturnField.DataSource = fieldNames;
                    //cboReturnField.SelectedItem = returnField;
                } // Connection will be automatically closed when leaving this block


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        void InitializeGridControl1DragDrop()
        {
            behaviorManager1.Attach<DragDropBehavior>(gridView1, behavior =>
            {
                behavior.DragDrop += Behavior_DragDropToGrid1;
            });
        }
        void Behavior_DragDropToGrid1(object sender, DragDropEventArgs e)
        {
            // Assuming "No" is the first column in your DataTable
            DataTable dataTable = (DataTable)gridControl1.DataSource;

        }

        private void SaveChangesToDatabase()
        {
            Console.WriteLine("Saving");
            // Assuming your DataTable is bound to the gridControl1
            DataTable dataTable = (DataTable)gridControl1.DataSource;

            // Replace the connection string with your actual database connection string
        

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    // Check if the connection is closed and open it if necessary
                    if (connection.State == ConnectionState.Closed)
                    {
                        connection.Open();
                    }

                    // Begin transaction
                    using (var transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted))
                    {
                        try
                        {
                            int columnSeqno = 1;

                            // Initialize command within the transaction scope
                            using (var command = connection.CreateCommand())
                            {
                                command.Transaction = transaction;

                                foreach (DataRow row in dataTable.Rows)
                                {
                                    if (row.RowState == DataRowState.Deleted)
                                    {
                                        // Handle deleted row (if needed)
                                        continue;
                                    }

                                    string columnName = row["Column Name"].ToString();
                                    string fieldName = row["Field Name"].ToString();

                                    if (!CheckIfRowFieldNameExistsInDatabase(fieldName,command))
                                    {
                                        // Call InsertNewRowIntoDatabase only for new rows
                                        InsertNewRowIntoDatabase(row, columnSeqno, command);
                                    }
                                    else
                                    {
                                        // Create an update command based on your table structure
                                        string updateQuery = $"UPDATE [LookupSetting_Detail_Tbl] " +
                                                             $"SET [Column Name] = @ColumnName, " +
                                                             $"[Column Width] = @ColumnWidth, " +
                                                             $"[Column Length] = @ColumnLength, " +
                                                             $"[Col Alignment] = @ColAlignment, " +
                                                             $"[Column Index] = @Seqno " + // Include the row number in the update

                                                             // Add other columns to update...
                                                             $"WHERE [Code] = @Code AND [Field Name] = @FieldName";

                                        command.CommandText = updateQuery;
                                        command.Parameters.Clear();
                                        command.Parameters.AddWithValue("@Seqno", columnSeqno); // Pass the row number
                                        command.Parameters.AddWithValue("@ColumnName", columnName); // Use the column name variable
                                        command.Parameters.AddWithValue("@ColumnWidth", row["Column Width"]);
                                        command.Parameters.AddWithValue("@ColumnLength", row["Column Length"]);
                                        command.Parameters.AddWithValue("@ColAlignment", row["Col Alignment"]);
                                        // Add other parameters...
                                        command.Parameters.AddWithValue("@Code", code);
                                        command.Parameters.AddWithValue("@FieldName", row["Field Name"]);

                                        command.ExecuteNonQuery();
                                    }

                                    if (!string.IsNullOrEmpty(fieldName))
                                    {
                                        foreach (KeyValuePair<string, string> mapping in columnMappings)
                                        {
                                            string groupColumnName = mapping.Value;

                                            // Check if the column value is DBNull before converting
                                            object columnValueObject = row[groupColumnName];
                                            string columnValue = Convert.IsDBNull(columnValueObject) ? string.Empty : Convert.ToBoolean(columnValueObject) ? groupColumnName : string.Empty;

                                            // Update the corresponding value in the database
                                            string updateGroupColumnQuery = $"UPDATE [LookupSetting_Detail_Tbl] " +
                                                                            $"SET [{mapping.Key}] = @ColumnValue " +
                                                                            $"WHERE [Code] = @Code AND [Field Name] = @FieldName";

                                            command.CommandText = updateGroupColumnQuery;
                                            command.Parameters.Clear();
                                            command.Parameters.AddWithValue("@ColumnValue", columnValue);
                                            command.Parameters.AddWithValue("@Code", code);
                                            command.Parameters.AddWithValue("@FieldName", fieldName);

                                            command.ExecuteNonQuery();
                                        }
                                    }

                                    columnSeqno++;
                                }

                                // Check if there are field names to delete
                                if (deleteFieldNames.Count > 0)
                                {
                                    foreach (string deleteFieldName in deleteFieldNames)
                                    {
                                        // Delete the field name from the database
                                        string deleteQuery = $"DELETE FROM [LookupSetting_Detail_Tbl] WHERE [Field Name] = @DeleteFieldName";

                                        command.CommandText = deleteQuery;
                                        command.Parameters.Clear();
                                        command.Parameters.AddWithValue("@DeleteFieldName", deleteFieldName);

                                        command.ExecuteNonQuery();
                                    }
                                    deleteFieldNames.Clear();
                                }

                                SaveAdditionalFields(command);

                                // Check if table name or lookup name has changed
                                if (_lookupName != txtLookUpName.Text || _tableName != txtTableName.Text)
                                {
                                    // Update [LookupSetting_Detail_Tbl] for changed table name and lookup name
                                    UpdateLookupSettingDetail(command, txtLookUpName.Text, txtTableName.Text, code);
                                    _lookupName = txtLookUpName.Text;
                                    _tableName = txtTableName.Text;
                                }
                            }

                            transaction.Commit();

                            MessageBox.Show("Save Successfully!");

                            // Refresh the grid view
                            gridView1.RefreshData();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            Console.WriteLine($"Error within transaction: {ex.Message}");
                            ExceptionLogger.LogException(ex, nameof(frmLookUpSettings), "LookUp", "SaveChangesToDatabase", new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ExceptionLogger.LogException(ex, nameof(frmLookUpSettings), "LookUp", "LoadDataIntoGrid", new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
            }
        }


        private void btn_preview_Click(object sender, EventArgs e)
        {

       
            // Populate previewData with the desired data
            DataTable previewData = (DataTable)gridControl1.DataSource;
            // Other parameters for styling
            string fontName = txtFontName.Text;
            string fontStyle = txtFontStyle.Text;
            float fontSize = float.Parse(txtFontSize.Text);
            Color foreColor = PictureBox1.BackColor;
            int sortType = cbosortType.SelectedItem.Equals("Ascending") ? 0 : 1;
            string orderByColumnName = cboOrderby.SelectedItem != null ? cboOrderby.SelectedItem.ToString() : null;

            // Open the LookUp form for preview
            frmLookUp previewForm = new frmLookUp(previewData, this.code, fontName, fontStyle, fontSize, foreColor, sortType, orderByColumnName);
            previewForm.StartPosition = FormStartPosition.CenterScreen;
            previewForm.Show();
        }
        private void UpdateLookupSettingDetail(SqlCommand command, string newLookupName, string newTableName, string code)
        {
            // Update [LookupSetting_Detail_Tbl] for changed table name and lookup name
            string updateDetailQuery =
                $"UPDATE [LookupSetting_Detail_Tbl] " +
                $"SET [Lookup Name] = @NewLookupName, [Table Name] = @NewTableName " +
                $"WHERE [Code] = @Code";

            command.CommandText = updateDetailQuery;
            command.Parameters.Clear();
            command.Parameters.AddWithValue("@NewLookupName", newLookupName);
            command.Parameters.AddWithValue("@NewTableName", newTableName);
            command.Parameters.AddWithValue("@Code", code);

            command.ExecuteNonQuery();
        }
        private bool CheckIfRowFieldNameExistsInDatabase(string fieldName, SqlCommand command)
        {
            string query = $"SELECT COUNT(*) FROM [LookupSetting_Detail_Tbl] WHERE [Field Name] = @FieldName";

            // Set the command's parameters
            command.CommandText = query;
            command.Parameters.Clear();
            command.Parameters.AddWithValue("@FieldName", fieldName);

            // Execute the query within the existing transaction
            int rowCount = (int)command.ExecuteScalar();

            // If rowCount is greater than 0, it means the field name already exists
            return rowCount > 0;
        }

        private void InsertNewRowIntoDatabase(DataRow newRow, int columnIndex, SqlCommand command)
        {
            Console.WriteLine("Inserting");

            try
            {
                if (!string.IsNullOrEmpty(Convert.ToString(newRow["Field Name"])))
                {
                    // Find the next available Column Seqno for the given Code
                    int nextAvailableSeqno = FindNextAvailableColumnSeqno(Convert.ToString(code), command);

                    // Create an insert command based on your table structure
                    string insertQuery = $"INSERT INTO [LookupSetting_Detail_Tbl] " +
                                         $"([Code], [Lookup Name], [Field Name], [Column Name], [Column Width], [Column Length], [Col Alignment],[Column Index], [Table Name],[Column Seqno],[Description]) " +
                                         $"VALUES (@Code, @LookupName, @FieldName, @ColumnName, @ColumnWidth, @ColumnLength, @ColAlignment, @ColumnIndex, @TableName,@ColumnSeqno,@Description)";

                    command.CommandText = insertQuery;
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@Code", this.code);
                    command.Parameters.AddWithValue("@LookupName", _lookupName); // Add the Lookup Name parameter
                    command.Parameters.AddWithValue("@FieldName", newRow["Field Name"]);
                    command.Parameters.AddWithValue("@ColumnName", newRow["Column Name"]);
                    command.Parameters.AddWithValue("@ColumnWidth", newRow["Column Width"]);
                    command.Parameters.AddWithValue("@ColumnLength", newRow["Column Length"]);
                    command.Parameters.AddWithValue("@ColAlignment", newRow["Col Alignment"]);
                    command.Parameters.AddWithValue("@ColumnIndex", columnIndex);
                    command.Parameters.AddWithValue("@TableName", _tableName); // Add the Table Name parameter
                    command.Parameters.AddWithValue("@ColumnSeqno", nextAvailableSeqno);
                    command.Parameters.AddWithValue("@Description", _lookupName);
                    command.ExecuteNonQuery();

                    Console.WriteLine("exiting");
                }
            }
            catch (Exception ex)
            {
                // Handle the exception, e.g., log it or show an error message
                Console.WriteLine($"Error inserting new row: {ex.Message}");

                // Throw the exception to let the calling method handle it
                throw;
            }
        }
        private int FindNextAvailableColumnSeqno(string code, SqlCommand command)
        {
            // Query existing Column Seqno values for the given Code
            string query = $"SELECT [Column Seqno] FROM [LookupSetting_Detail_Tbl] WHERE [Code] = @Code ORDER BY [Column Seqno]";

            // Set the command's text and parameters
            command.CommandText = query;
            command.Parameters.Clear();
            command.Parameters.AddWithValue("@Code", code);

            using (SqlDataReader reader = command.ExecuteReader())
            {
                int expectedSeqno = 1;

                while (reader.Read())
                {
                    int currentSeqno = Convert.ToInt32(reader["Column Seqno"]);

                    // Check if the current value is the expected one
                    if (currentSeqno == expectedSeqno)
                    {
                        expectedSeqno++; // Move on to the next expected value
                    }
                    else
                    {
                        // Gap found, return the expected value
                        return expectedSeqno;
                    }
                }

                // If the loop completes, all existing values are in sequence, so return the next expected value
                return expectedSeqno;
            }
        }


        private void SaveAdditionalFields(SqlCommand command)
        {
            try
            {
                // Save additional fields from the form to [LookupSetting_Tbl]
                string updateLookupSettingQuery =
                    $"UPDATE [LookupSetting_Tbl] " +
                    $"SET [Description] = @Description, " +
                    $"[Remark] = @Remark, " +
                    $"[FontSize] = @FontSize, " +
                    $"[FontStyle] = @FontStyle, " +
                    $"[FontName] = @FontName, " +
                    $"[ForeColor] = @ForeColor, " +
                    $"[Sort Type] = @SortType, " +
                    $"[MultiSelect Lookup] = @MultiSelectLookup, " +
                    $"[SQLClauseTrimStockFullStop] = @SQLClauseTrimStockFullStop, " +
                    $"[Return Field] = @ReturnField, " +
                    $"[Table Name] = @TableName, " +
                    $"[Lookup Name] = @NewLookupName, " +
                    $"[Order By] = @OrderBy " + // Include [Order By] in the update statement
                    $"WHERE [Code] = @Code";

                command.CommandText = updateLookupSettingQuery;
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@Description", txtDescription.Text);
                command.Parameters.AddWithValue("@Remark", txtRemark.Text);
                command.Parameters.AddWithValue("@FontSize", txtFontSize.Text);
                command.Parameters.AddWithValue("@FontStyle", txtFontStyle.Text);
                command.Parameters.AddWithValue("@FontName", txtFontName.Text);
                command.Parameters.AddWithValue("@ForeColor", PictureBox1.BackColor.ToArgb());
                command.Parameters.AddWithValue("@SortType", cbosortType.SelectedItem.Equals("Ascending") ? 0 : 1);
                command.Parameters.AddWithValue("@MultiSelectLookup", chkMultiSelect.Checked ? 1 : 0);
                command.Parameters.AddWithValue("@SQLClauseTrimStockFullStop", chkTrimStock.Checked ? 1 : 0);
                command.Parameters.AddWithValue("@NewLookupName", txtLookUpName.Text); // Assuming _lookupName is already set
                command.Parameters.AddWithValue("@ReturnField", cboReturnField.SelectedItem != null ? cboReturnField.SelectedItem.ToString() : ""); // Handle null values
                command.Parameters.AddWithValue("@TableName", txtTableName.Text);
                command.Parameters.AddWithValue("@OrderBy", cboOrderby.SelectedItem != null ? cboOrderby.SelectedItem.ToString() : "");
                command.Parameters.AddWithValue("@Code", code);

                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occurred in SaveAdditionalFields: " + ex.Message);

                // Throw the exception to let the calling method handle it
                throw;
            }
        }



        // Call this method HandleDataSelectedEvent method
        private void HandleDataSelectedEvent(Dictionary<string, object> selectedData)
        {
            string code = selectedData["[Code]"].ToString();
            this.code = code;
            LoadDataForSelectedCode(code);
            // Get the selected code using your logic (replace this with your actual code)


            // Call LoadDataIntoGrid with the selectedCode
            LoadDataIntoGrid(code);



        }
        private void gridView1_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            // Set the background color for alternate rows
            if (e.RowHandle % 2 == 1)
            {
                e.Appearance.BackColor = Color.Lavender;
            }
        }

        private void btnFont_Click(object sender, EventArgs e)
        {
            using (FontDialog fontDialog = new FontDialog())
            {
                if (fontDialog.ShowDialog() == DialogResult.OK)
                {
                    // Set the font properties to the text boxes
                    txtFontSize.Text = fontDialog.Font.Size.ToString();
                    txtFontStyle.Text = fontDialog.Font.Style.ToString();
                    txtFontName.Text = fontDialog.Font.Name;
                }
            }
        }

        private void btnForeColor_Click(object sender, EventArgs e)
        {
            using (ColorDialog colorDialog = new ColorDialog())
            {
                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    // Set the selected color to the PictureBox
                    PictureBox1.BackColor = colorDialog.Color;
                }
            }
        }
        private void ResetToOriginalState()
        {
            // Restore original widths
            panel1.Width = originalPanel1Width;
            panel2.Width = originalPanel2Width;
            panel2.Left = originalPanel2Left;
            btnForeColor.Left = originalBtnForeColorLeft;
            btnFont.Left = originalFontLeft;

            // Additional reset logic as needed
        }

        private void ExpandSizeForLargerScreen()
        {
            int halfScreenWidth = this.Width / 2;

            // Adjust the components for a larger screen
            panel1.Width = halfScreenWidth + 200;
            panel2.Left = panel1.Right + 20;
            panel2.Width = halfScreenWidth - 260;
            btnFont.Left = txtFontName.Right + 10;
            btnForeColor.Left = PictureBox1.Right + 10;
        }

        private void initializeOriginalPosition()
        {
            originalPanel1Width = panel1.Width;
            originalPanel2Width = panel2.Width;
            originalPanel2Left = panel2.Left;
            originalBtnForeColorLeft = btnForeColor.Left;
            originalFontLeft = btnFont.Left;

        }
        private void btnSave_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (gridView1.PostEditor())
                gridView1.UpdateCurrentRow();
            SaveChangesToDatabase();
        }



    }



}