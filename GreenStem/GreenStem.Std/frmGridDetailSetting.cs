using DevExpress.CodeParser;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.Drawing;
using DevExpress.Xpo.DB.Helpers;
using DevExpress.XtraBars;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using GreenStem.ClassModules;
using GreenStem.LookUp;
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

namespace Greenstem.Std
{
    public partial class frmGridDetailSetting : DevExpress.XtraBars.Ribbon.RibbonForm
    {

        private string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connection_string"].ConnectionString;
        private HashSet<DataRow> addedRows = new HashSet<DataRow>();
        private HashSet<string> existingFieldNames = new HashSet<string>();
        private ArrayList deleteFieldNames = new ArrayList();
        private string code;

        public frmGridDetailSetting()
        {
            InitializeComponent();
            LoadDataIntoGrid("1001");
            gridView1.CustomDrawCell += gridView1_CustomDrawCell;
            gridView1.CellValueChanged += gridView1_CellValueChanged;
            gridView1.KeyDown += gridView1_KeyDown;
            LoadExistingFieldNames();
            
        }

        private void SettingLookUp_Click(object sender, EventArgs e)
        {

            frmLookUp.OpenLookupForm("10277", HandleDataSelectedEvent, this, false  );
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
        // Load existing field names into HashSet
        private void LoadExistingFieldNames()
        {
            try
            {
                string query = "SELECT [Field Name] FROM [Grid_Detail_Tbl]";

                using (SqlConnection connection = new SqlConnection(connectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        existingFieldNames.Add(reader["Field Name"].ToString());
                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading existing field names: {ex.Message}");
                // Handle the exception as needed
            }
        }
        private void gridView1_KeyDown(object sender, KeyEventArgs e)
        {
            // Your logic for handling key presses, e.g., delete a row
            if (e.KeyCode == Keys.Delete)
            {
                // Call the method to delete the row
                HandleDeleteKeyPressed();
            }
        }
        private void HandleDataSelectedEvent(Dictionary<string, object> selectedData)
        {
            try
            {
                // Example: Retrieving the code from the selected data
                string code = selectedData["[Code]"].ToString();
                this.code = code;
   

                // Fetch data from the database based on the selected code
                string query = $"SELECT * FROM [GSTSTD].[dbo].[Grid_Main_Tbl] WHERE [Code] = '{code}'";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(query, connection))
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Example: Populating textboxes with retrieved data
                            txtGridName.Text = reader["Grid Name"].ToString();
                            txtDescription.Text = reader["Description"].ToString();
                            txtRemark.Text = reader["Remarks"].ToString();
                            txtCode.Text = code;
                        }
                        else
                        {
                            // Handle the case where no data is found for the given code
                            MessageBox.Show("No data found for the selected code.");
                        }
                    }
                }

                LoadDataIntoGrid(code);
            }
            catch (Exception ex)
            {
                // Handle exceptions, e.g., display an error message
                MessageBox.Show($"Error: {ex.Message}");
                ExceptionLogger.LogException(ex, nameof(frmGridDetailSetting), "Std", "HandleDataSelectedEvent", new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
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


                    // Create a SqlDataAdapter to fetch data from the database using the provided query
                    string query = "SELECT [Field Name], [Column Name], [Column Width], [Column Length], [Col Alignment], [Remarks]," +
                   "CASE WHEN [Next Line] = 1 THEN 'Yes' ELSE 'No' END AS [Next Line], " +
                   "CASE WHEN [Next Column] = 1 THEN 'Yes' ELSE 'No' END AS [Focus], [Column Before Index] AS [Index]" +
                   $"FROM [Grid_Detail_Tbl] WHERE [Code] = '{selectedCode}' " +
                   "ORDER BY [LNo]";


                    SqlDataAdapter dataAdapter = new SqlDataAdapter(query, connection);

                    // Create a DataTable to hold the data
                    DataTable dataTable = new DataTable();
                    // Add a column for the row numbers
                    dataTable.Columns.Add("No", typeof(int));
                    dataTable.Columns["No"].AutoIncrement = true;
                    dataTable.Columns["No"].AutoIncrementSeed = 1;
                    dataTable.Columns["No"].AutoIncrementStep = 1;
                    // initialize other field name


                    // Fill the DataTable with data from the database
                    dataAdapter.Fill(dataTable);
                    // Reorder columns to ensure group columns are displayed at the end

                    gridControl1.DataSource = dataTable;

                    // Assuming you have the columns added to the GridView
                    GridColumn colFieldName = gridView1.Columns["Field Name"];
                    GridColumn colColumnName = gridView1.Columns["Column Name"];

                    // Set the width for "Field Name" column
                    // colFieldName.OptionsColumn.FixedWidth = true;
                    colFieldName.Width = 150;  // Adjust the width as needed

                    // Set the width for "Column Name" column
                    //  colColumnName.OptionsColumn.FixedWidth = true;
                    colColumnName.Width = 200;  // Adjust the width as needed
                                                // gridView1.OptionsView.ColumnAutoWidth = false;

                    DataRow newRow = dataTable.NewRow();
                    // Set other column values
                    dataTable.Rows.Add(newRow);

                    // Track the newly added row
                    addedRows.Add(newRow);



                    // // Create a ComboBox repository item for the "Field Name" column
                    RepositoryItemComboBox repositoryItemComboBoxFieldName = new RepositoryItemComboBox();


                    gridView1.OptionsView.ColumnAutoWidth = false;


                    // Now that you have the table name, use it in the column query

                    foreach (DataRow row in dataTable.Rows)
                    {
                        if (!string.IsNullOrEmpty(row["Field Name"].ToString()))
                        {
                            repositoryItemComboBoxFieldName.Items.Add($"{row["Field Name"]}");
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

                    gridControl1.RepositoryItems.Add(repositoryItemComboBoxColumnAlign);
                    colAlignment.ColumnEdit = repositoryItemComboBoxColumnAlign;


                    GridColumn nextLine = gridView1.Columns["Next Line"];
                    // Create a ComboBox repository item for the "Column Align" column
                    RepositoryItemComboBox repositoryItemComboBoxNextLine = new RepositoryItemComboBox();
                    repositoryItemComboBoxNextLine.Items.Add("Yes");
                    repositoryItemComboBoxNextLine.Items.Add("No");

                    gridControl1.RepositoryItems.Add(repositoryItemComboBoxNextLine);
                    nextLine.ColumnEdit = repositoryItemComboBoxNextLine;

                    GridColumn focus = gridView1.Columns["Focus"];
                    // Create a ComboBox repository item for the "Column Align" column
                    RepositoryItemComboBox repositoryItemComboBoxFocus = new RepositoryItemComboBox();
                    repositoryItemComboBoxFocus.Items.Add("Yes");
                    repositoryItemComboBoxFocus.Items.Add("No");

                    repositoryItemComboBoxFocus.EditValueChanged += RComboBoxEdit_EditValueChanged;
                    gridControl1.RepositoryItems.Add(repositoryItemComboBoxFocus);
                    focus.ColumnEdit = repositoryItemComboBoxNextLine;




                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ExceptionLogger.LogException(ex, nameof(frmGridDetailSetting), "Std", "LoadDataIntoGrid", new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
            }
        }
        private void RComboBoxEdit_EditValueChanged(object sender, EventArgs e)
        {
            if (gridView1.PostEditor())
                gridView1.UpdateCurrentRow();
        }
        private void gridView1_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            // Set the background color for alternate rows
            if (e.RowHandle % 2 == 1)
            {
                e.Appearance.BackColor = Color.Lavender;
            }
        }
        private void gridView1_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            // Check if the changed cell is in the last visible row
            if (e.RowHandle == gridView1.DataRowCount - 1 && e.Column != null)
            {
                // Check if the user has entered some value in the cell
                if (gridView1.GetRowCellValue(e.RowHandle, e.Column) != DBNull.Value)
                {
                    // Add a new row
                    DataRow newRow = ((DataTable)gridControl1.DataSource).NewRow();


                    ((DataTable)gridControl1.DataSource).Rows.Add(newRow);
                    // Track the newly added row
                    addedRows.Add(newRow);

                    // Focus the new row
                    gridView1.FocusedRowHandle = gridView1.DataRowCount - 1;
                }
            }
        }

        private void btnSave_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (gridView1.PostEditor())
                gridView1.UpdateCurrentRow();
            SaveChangesToDatabase();
        }

        private void SaveChangesToDatabase()
        {
            Console.WriteLine("Saving");
            // Assuming your DataTable is bound to the gridControl1
            DataTable dataTable = (DataTable)gridControl1.DataSource;
        
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
                        // Initialize command within the transaction scope
                        using (var command = connection.CreateCommand())
                        {
                            command.Transaction = transaction;

                            try
                            {
                                int columnSeqno = 1;

                                foreach (DataRow row in dataTable.Rows)
                                {
                                    if (row.RowState == DataRowState.Deleted)
                                    {
                                        // Handle deleted row (if needed)
                                        continue; // Move on to the next iteration
                                    }

                                    string columnName = row["Column Name"].ToString();
                                    string fieldName = row["Field Name"].ToString();

                                    Console.WriteLine(fieldName);

                                    if (!CheckIfRowFieldNameExistsInDatabase(command, fieldName))
                                    {
                                       
                                        // Call InsertNewRowIntoDatabase only for new rows
                                        InsertNewRowIntoDatabase(row, columnSeqno, command);
                                    }
                                    else
                                    {
                                        // Clear existing parameters before adding new ones
                                        command.Parameters.Clear();
                                        // Create an update command based on your table structure
                                        string updateQuery = $"UPDATE [Grid_Detail_Tbl] " +
                                                     $"SET [Column Name] = @ColumnName, " +
                                                     $"[Column Width] = @ColumnWidth, " +
                                                     $"[Column Length] = @ColumnLength, " +
                                                     $"[Col Alignment] = @ColAlignment, " +

                                                     $"[Field Name] = @FieldName, " +
                                                     $"[Remarks] = @Remarks, " +
                                                     $"[Next Line] = @NextLine, " +
                                                     $"[Next Column] = @NextColumn " +
                                                     // Add other columns to update...
                                                     $"WHERE [Code] = @Code AND [Field Name] = @FieldToUpdate";

                                        command.CommandText = updateQuery;

                                        command.Parameters.AddWithValue("@ColumnName", columnName); // Use the column name variable
                                        command.Parameters.AddWithValue("@ColumnWidth", row["Column Width"]);
                                        command.Parameters.AddWithValue("@ColumnLength", row["Column Length"]);
                                        command.Parameters.AddWithValue("@ColAlignment", row["Col Alignment"]);
                                        // Add other parameters...
                                        command.Parameters.AddWithValue("@FieldName", fieldName);
                                        command.Parameters.AddWithValue("@Remarks", row["Remarks"]);


                                        // Convert "Next Line" to bool (true for "Yes", false otherwise)
                                        bool nextLineValue = row["Next Line"].ToString().Equals("Yes", StringComparison.OrdinalIgnoreCase);

                                        // Convert "Focus" to bool (true for "Yes", false otherwise)
                                        bool focusValue = row["Focus"].ToString().Equals("Yes", StringComparison.OrdinalIgnoreCase);

                                        // Add parameters to the SqlCommand
                                        command.Parameters.AddWithValue("@NextLine", nextLineValue);
                                        command.Parameters.AddWithValue("@NextColumn", focusValue);
                                        command.Parameters.AddWithValue("@FieldToUpdate", fieldName);
                                        command.Parameters.AddWithValue("@Code", code);

                                        // Execute the update command
                                        command.ExecuteNonQuery();

                                        // Clear existing parameters before adding new ones
                                        command.Parameters.Clear();
                                    }

                                    columnSeqno++;
                                }

                                if (deleteFieldNames.Count > 0)
                                {
                                    foreach (string deleteFieldName in deleteFieldNames)
                                    {
                                        if (!string.IsNullOrEmpty(deleteFieldName))
                                        {
                                            // Delete the field name from the database
                                            DeleteRowFromDatabase(deleteFieldName, command);
                                        }
                                    }
                                    deleteFieldNames.Clear();
                                }

                                SaveAdditionalFields(command);

                                // Commit transaction if everything succeeds
                                transaction.Commit();

                                MessageBox.Show("Save Successfully!");
                                LoadExistingFieldNames();
                                // Refresh the grid view
                                gridView1.RefreshData();
                            }
                            catch (Exception ex)
                            {
                                // Rollback the transaction if an exception occurs within the transaction
                                transaction.Rollback();
                                Console.WriteLine($"Error within transaction: {ex.Message}");
                                ExceptionLogger.LogException(ex, nameof(frmGridDetailSetting), "Std", "SaveChangesToDatabase", new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ExceptionLogger.LogException(ex, nameof(frmGridDetailSetting), "Std", "SaveChangesToDatabase", new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
            }
        }

        private bool CheckIfRowFieldNameExistsInDatabase(SqlCommand command, string fieldName)
        {
            string query = $"SELECT COUNT(*) FROM [Grid_Detail_Tbl] WHERE [Field Name] = @FieldName";

            // Set the command's text and parameters
            command.CommandText = query;
            command.Parameters.Clear();
            command.Parameters.AddWithValue("@FieldName", fieldName);

            // Execute the query using the provided command object
            int rowCount = (int)command.ExecuteScalar();

            // If rowCount is greater than 0, it means the field name already exists
            return rowCount > 0;
        }
        private void DeleteRowFromDatabase(string fieldName, SqlCommand command)
        {
            // Clear existing parameters before adding new ones
            command.Parameters.Clear();
            // Create your delete command here
            command.CommandText = "DELETE FROM [Grid_Detail_Tbl] WHERE [Field Name] = @FieldName";

          
            // Add parameters to the command
            command.Parameters.AddWithValue("@FieldName", fieldName);

            // Execute the delete command
            command.ExecuteNonQuery();

          
        }
        private void InsertNewRowIntoDatabase(DataRow newRow, int columnIndex, SqlCommand command)
        {
            try
            {
                if (!string.IsNullOrEmpty(Convert.ToString(newRow["Field Name"])))
                {
                    // Find the next available Column Seqno for the given Code
                    int nextAvailableSeqno = FindNextAvailableColumnSeqno(command, Convert.ToString(code));

                    // Create an insert command based on your table structure
                    string insertQuery = $"INSERT INTO [Grid_Detail_Tbl] " +
                        $"([Code], [LNo], [Field Name], [Column Name], [Column Width], [Column Length], [Col Alignment], [Column Before Index], [Remarks], [Next Line], [Next Column]) " +
                        $"VALUES (@Code, @LNo, @FieldName, @ColumnName, @ColumnWidth, @ColumnLength, @ColAlignment, @ColumnBeforeIndex, @Remarks, @NextLine, @NextColumn)";

                    command.CommandText = insertQuery;
                    command.Parameters.Clear(); // Clear existing parameters before adding new ones
                    command.Parameters.AddWithValue("@Code", this.code);
                    command.Parameters.AddWithValue("@LNo", nextAvailableSeqno);
                    command.Parameters.AddWithValue("@FieldName", newRow["Field Name"]);
                    command.Parameters.AddWithValue("@ColumnName", newRow["Column Name"]);
                    command.Parameters.AddWithValue("@ColumnWidth", newRow["Column Width"]);
                    command.Parameters.AddWithValue("@ColumnLength", newRow["Column Length"]);
                    command.Parameters.AddWithValue("@ColAlignment", newRow["Col Alignment"]);
                    command.Parameters.AddWithValue("@ColumnBeforeIndex", nextAvailableSeqno);
                    command.Parameters.AddWithValue("@Remarks", newRow["Remarks"]);

                    // Convert "Next Line" to bool (true for "Yes", false otherwise)
                    bool nextLineValue = newRow["Next Line"].ToString().Equals("Yes", StringComparison.OrdinalIgnoreCase);
                    command.Parameters.AddWithValue("@NextLine", nextLineValue);

                    // Convert "Focus" to bool (true for "Yes", false otherwise)
                    bool focusValue = newRow["Focus"].ToString().Equals("Yes", StringComparison.OrdinalIgnoreCase);
                    command.Parameters.AddWithValue("@NextColumn", focusValue);

                    // Execute the command
                    command.ExecuteNonQuery();
                   
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inserting new row: {ex.Message}");
                throw;
            }
        }

        private int FindNextAvailableColumnSeqno(SqlCommand command, string code)
        {
            int expectedSeqno = 0;

            // Query existing Column Seqno values for the given Code
            string query = $"SELECT [LNo] FROM [Grid_Detail_Tbl] WHERE [Code] = @Code ORDER BY [LNo]";

            command.CommandText = query; // Set the command's text
            command.Parameters.Clear(); // Clear existing parameters before adding new ones
            command.Parameters.AddWithValue("@Code", code); // Add the parameter

            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    int currentSeqno = Convert.ToInt32(reader["LNo"]);

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
            }

            // If the loop completes, all existing values are in sequence, so return the next expected value
            return expectedSeqno;
        }
        private void SaveAdditionalFields(SqlCommand command)
        {
            try
            {
                string updateLookupSettingQuery =
                    $"UPDATE [Grid_Main_Tbl] " +
                    $"SET [Description] = @Description, " +
                    $"[Remarks] = @Remark, " +
                    $"[Grid Name] = @GridName " +
                    $"WHERE [Code] = @Code";

                command.CommandText = updateLookupSettingQuery;
                command.Parameters.Clear(); // Clear existing parameters before adding new ones
                command.Parameters.AddWithValue("@Description", txtDescription.Text);
                command.Parameters.AddWithValue("@Remark", txtRemark.Text);
                command.Parameters.AddWithValue("@GridName", txtGridName.Text);
                command.Parameters.AddWithValue("@Code", code);

                // Execute the command
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occurred in SaveAdditionalFields: " + ex.Message);
                throw;
            }
        }

    }
}