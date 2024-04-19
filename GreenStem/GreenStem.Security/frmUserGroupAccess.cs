using DevExpress.CodeParser;


using DevExpress.XtraGrid.Views.Grid;
using GreenStem;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Contexts;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using GreenStem.ClassModules;

namespace GreenStem.Security
{
    public partial class frmUserGroupAccess : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        // Define connection string
        private string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connection_string"].ConnectionString;

        private string menuId;
        private string menuName;
        private string key3;
        public frmUserGroupAccess()
        {
            InitializeComponent();
            LoadMenu();
            initializedGrid();


        }

        private void initializedGrid()


        {   // Create a DataTable to hold the data
            gridControl1.DataSource = null;
            DataTable dataTable = new DataTable();
            // Add a column for the row numbers
            AddColumnIntoGrid(dataTable);
            gridControl1.DataSource = dataTable;

        }
        private void AddColumnIntoGrid(DataTable dataTable)
        {
            dataTable.Columns.Add("Show", typeof(bool));

            // initialize other field name
            dataTable.Columns.Add("User ID", typeof(string));
            dataTable.Columns.Add("Add", typeof(bool));
            dataTable.Columns.Add("Change", typeof(bool));
            dataTable.Columns.Add("Delete", typeof(bool));
            dataTable.Columns.Add("Excel", typeof(bool));
            dataTable.Columns.Add("Report", typeof(bool));
            dataTable.Columns.Add("Print", typeof(bool));
            dataTable.Columns.Add("Print Full", typeof(bool));
            dataTable.Columns.Add("Print Half", typeof(bool));
            dataTable.Columns.Add("Counter", typeof(bool));
        }
        private void LoadMenu()
        {
            tvwDB.ImageList = imageList1;
            tvwDB.ImageIndex = 0;
            string query = "SELECT * FROM MENU_TBL " + modPublicVariable.ReadLockType + " WHERE Right([Menu Id], 3) = '000' ORDER BY Seqno";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(query, connection))
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string modi1 = reader["SEQNO"].ToString();
                        string nodKey = reader["MENU ID1"].ToString().Trim() + "|" + reader["MENU Name"].ToString().Trim() + "|" + reader["MENU seqno"].ToString().Trim() + "|" + modi1;
                        string nodText = reader["Menu Caption"].ToString();
                        TreeNode firstNode = tvwDB.Nodes.Add(nodKey, nodText);

                        // Retrieve data from reader before closing it
                        string menuId = reader["Menu ID"].ToString();

                        LoadSubMenu(firstNode, menuId);
                    }
                }
            }
        }

        private void LoadSubMenu(TreeNode parentNode, string menuId)
        {
            string subQuery = "SELECT * FROM MENU_TBL " + modPublicVariable.ReadLockType + " WHERE substring([Menu Id],1,3)=substring(@menuId,1,3) and [Menu Type]='000' And [Menu ID]<>@menuId ORDER BY [seqno]";

            using (SqlConnection innerConnection = new SqlConnection(connectionString))
            {
                innerConnection.Open();
                using (SqlCommand subCommand = new SqlCommand(subQuery, innerConnection))
                {
                    subCommand.Parameters.AddWithValue("@menuId", menuId);

                    using (SqlDataReader subReader = subCommand.ExecuteReader())
                    {
                        while (subReader.Read())
                        {
                            string modi1 = subReader["SEQNO"].ToString();
                            string nodKey = subReader["MENU ID1"].ToString().Trim() + "|" + subReader["MENU Name"].ToString().Trim() + "|" + subReader["MENU seqno"].ToString().Trim() + "|" + modi1;
                            string nodText = subReader["Menu Caption"].ToString();
                            TreeNode childNode = parentNode.Nodes.Add(nodKey, nodText);

                            string subMenuId = subReader["Menu ID"].ToString();
                            LoadSubMenuItems(childNode, subMenuId);
                        }
                    }
                }
            }
        }

        private void LoadSubMenuItems(TreeNode parentNode, string menuId)
        {
            string subQuery = "SELECT * FROM MENU_TBL " + modPublicVariable.ReadLockType + " WHERE [Menu ID]=@menuId AND [Menu Type]='001' ORDER BY [seqno]";

            using (SqlConnection subConnection = new SqlConnection(connectionString))
            {
                subConnection.Open();

                using (SqlCommand subCommand = new SqlCommand(subQuery, subConnection))
                {
                    subCommand.Parameters.AddWithValue("@menuId", menuId);

                    using (SqlDataReader subReader = subCommand.ExecuteReader())
                    {
                        while (subReader.Read())
                        {
                            string modi2 = subReader["SEQNO"].ToString();
                            string nodKey = subReader["MENU ID1"].ToString().Trim() + "|" + subReader["MENU Name"].ToString().Trim() + "|" + subReader["MENU seqno"].ToString().Trim() + "|" + modi2;
                            string nodText = subReader["Menu Caption"].ToString();
                            TreeNode childNode = parentNode.Nodes.Add(nodKey, nodText);

                            string subMenuId = subReader["Menu ID"].ToString();
                            string subMenuSubSeqno = subReader["Menu Sub Seqno"].ToString();
                            LoadSubMenuItemsDetails(childNode, subMenuId, subMenuSubSeqno);
                        }
                    }
                }
            }
        }

        private void LoadSubMenuItemsDetails(TreeNode parentNode, string menuId, string menuSubSeqno)
        {
            string subQuery = "SELECT * FROM MENU_TBL " + modPublicVariable.ReadLockType+ " WHERE [Menu ID]=@menuId AND [Menu Type]='002' AND [Menu Sub Seqno]=@menuSubSeqno ORDER BY [seqno]";

            using (SqlConnection subConnection = new SqlConnection(connectionString))
            {
                subConnection.Open();

                using (SqlCommand subCommand = new SqlCommand(subQuery, subConnection))
                {
                    subCommand.Parameters.AddWithValue("@menuId", menuId);
                    subCommand.Parameters.AddWithValue("@menuSubSeqno", menuSubSeqno);

                    using (SqlDataReader subReader = subCommand.ExecuteReader())
                    {
                        while (subReader.Read())
                        {
                            string modi3 = subReader["SEQNO"].ToString();
                            string nodKey = subReader["MENU ID1"].ToString().Trim() + "|" + subReader["MENU Name"].ToString().Trim() + "|" + subReader["MENU seqno"].ToString().Trim() + "|" + modi3;
                            string nodText = subReader["Menu Caption"].ToString();
                            parentNode.Nodes.Add(nodKey, nodText);
                        }
                    }
                }
            }
        }

        private void tvwDB_AfterSelect(object sender, TreeViewEventArgs e)
        {
            lblModulePath.Text = tvwDB.PathSeparator + e.Node.FullPath;
            string[] nodeData = e.Node.Name.Split('|');
            if (nodeData.Length >= 2)
            {
                 this.menuId = nodeData[0];
                 this.menuName = nodeData[1];
                 this.key3 = nodeData[3];


                // Call the LoadDataIntoGrid method with the retrieved menuId and menuName
                LoadDataIntoGrid(menuId, menuName);
            }
            else
            {
                initializedGrid();
            }
        }

        private void tvwDB_AfterCollapse(object sender, TreeViewEventArgs e)
        {

            e.Node.ImageIndex = 1;

        }

        private void tvwDB_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            e.Node.ImageIndex = 0;

        }


        private void LoadDataIntoGrid(string menuId, string menuName)
        {
            gridControl1.DataSource = null;
            // Create a DataTable to hold the data
            DataTable dataTable = new DataTable();
            // Add a column for the row numbers
            AddColumnIntoGrid(dataTable);

            try
            {
                // Create a SqlConnection for the first query
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Fetch the data for GGRP_TBL
                    string groupQuery = "SELECT * FROM GGRP_TBL " + modPublicVariable.ReadLockType+ " ORDER BY GGRP_ID";

                    using (SqlCommand groupCommand = new SqlCommand(groupQuery, connection))
                    using (SqlDataReader groupReader = groupCommand.ExecuteReader())
                    {
                        while (groupReader.Read())
                        {
                            string groupId = groupReader["GGRP_ID"].ToString();

                            string query = $"SELECT [GXCS_BLOCK] AS 'Show', [GXCS_GRDID] AS 'User ID', [GXCS_ADD] AS 'Add' , [GXCS_CHG] AS 'Change' , [GXCS_DEL] AS 'Delete', [GXCS_Excel] AS 'Excel', [GXCS_Report] AS 'Report',[GXCS_Print] AS 'Print',[GXCS_PrintFull] AS 'Print Full',[GXCS_PrintHalf] AS 'Print Half', [Gxcs_Counter] AS 'Counter' " +
                                  $" FROM [GXCS_TBL] WHERE [GXCS_MDLID] = '{menuId}' And [GXCS_MENU] = '{menuName}' And GXCS_GRDID='{groupId}'" +
                                  $" ORDER BY [GXCS_GRDID]";
                            // Create a new SqlConnection for the second query
                            using (SqlConnection secondConnection = new SqlConnection(connectionString))
                            {
                                secondConnection.Open();

                                using (SqlCommand command = new SqlCommand(query, secondConnection))
                                using (SqlDataReader reader = command.ExecuteReader())
                                {
                                    if (reader.HasRows) // Check if there are rows returned by the query
                                    {
                                        while (reader.Read())
                                        {
                                            // Create a new row
                                            DataRow newRow = dataTable.NewRow();

                                            // Add values to the row
                                            newRow["Show"] = Convert.ToInt32(reader["Show"]) == 1;
                                            newRow["User ID"] = groupId;
                                            newRow["Add"] = Convert.ToInt32(reader["Add"]) == 1;
                                            newRow["Change"] = Convert.ToInt32(reader["Change"]) == 1;
                                            newRow["Delete"] = Convert.ToInt32(reader["Delete"]) == 1;
                                            newRow["Excel"] = Convert.ToInt32(reader["Excel"]) == 1;
                                            newRow["Report"] = Convert.ToInt32(reader["Report"]) == 1;
                                            newRow["Print"] = Convert.ToInt32(reader["Print"]) == 1;
                                            newRow["Print Full"] = Convert.ToInt32(reader["Print Full"]) == 1;
                                            newRow["Print Half"] = Convert.ToInt32(reader["Print Half"]) == 1;
                                            newRow["Counter"] = Convert.ToInt32(reader["Counter"]) == 1;

                                            // Add the row to the DataTable
                                            dataTable.Rows.Add(newRow);
                                        }
                                    }
                                    else // No rows returned by the query, add default values
                                    {
                                        DataRow newRow = dataTable.NewRow();
                                        newRow["Show"] = false;
                                        newRow["User ID"] = groupId;
                                        newRow["Add"] = false;
                                        newRow["Change"] = false;
                                        newRow["Delete"] = false;
                                        newRow["Excel"] = false;
                                        newRow["Report"] = false;
                                        newRow["Print"] = false;
                                        newRow["Print Full"] = false;
                                        newRow["Print Half"] = false;
                                        newRow["Counter"] = false;

                                        dataTable.Rows.Add(newRow);
                                    }
                                }
                            }
                        }
                    }
                }

              

                // Reorder columns to ensure group columns are displayed at the end
                gridControl1.DataSource = dataTable;
                gridView1.Columns["User ID"].Width = 100;
                // Add CellValueChanged event handler to GridView
                gridView1.CellValueChanging += GridView1_CellValueChanging;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


        private void GridView1_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {

            
            gridView1.PostEditor();
            gridView1.UpdateCurrentRow();
            GridView view = sender as GridView;
            int rowHandle = view.FocusedRowHandle;

            // Update the underlying data source with the new value
            view.SetRowCellValue(rowHandle, e.Column, e.Value);
            if (e.Column.FieldName != "Show")
                return;

            view = sender as GridView;

            for (int i = 0; i < view.Columns.Count; i++)
            {
                if (view.Columns[i].Visible && view.GetFocusedRowCellValue(view.Columns[i]) is bool)
                    view.SetFocusedRowCellValue(view.Columns[i], e.Value);
            }
         
        }

        private void btnItemSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        SqlTransaction transaction = connection.BeginTransaction();
                        command.Transaction = transaction;

                        try
                        {
                            // Iterate through each row in the GridView
                            for (int i = 0; i < gridView1.RowCount; i++)
                            {
                                DataRowView rowView = (DataRowView)gridView1.GetRow(i);
                                DataRow row = rowView.Row;

                                // Retrieve values from the row
                                bool show = Convert.ToBoolean(row["Show"]);
                                string userId = row["User ID"].ToString();
                                bool add = Convert.ToBoolean(row["Add"]);
                                bool change = Convert.ToBoolean(row["Change"]);
                                bool delete = Convert.ToBoolean(row["Delete"]);
                                bool excel = Convert.ToBoolean(row["Excel"]);
                                bool report = Convert.ToBoolean(row["Report"]);
                                bool print = Convert.ToBoolean(row["Print"]);
                                bool printFull = Convert.ToBoolean(row["Print Full"]);
                                bool printHalf = Convert.ToBoolean(row["Print Half"]);
                                bool counter = Convert.ToBoolean(row["Counter"]);

                                // Check if the row exists in the database
                                string selectQuery = "SELECT COUNT(*) FROM GXCS_TBL WHERE GXCS_GRDID = @UserId AND GXCS_MDLID = @MenuId AND GXCS_MENU = @MenuName";

                                command.CommandText = selectQuery;
                                command.Parameters.Clear();
                                command.Parameters.AddWithValue("@UserId", userId);
                                command.Parameters.AddWithValue("@MenuId", menuId);
                                command.Parameters.AddWithValue("@MenuName", menuName);
                                int count = Convert.ToInt32(command.ExecuteScalar());

                                if (count > 0)
                                {
                                    // Row exists, update the record
                                    string updateQuery = "UPDATE GXCS_TBL " + modPublicVariable.UpdateLockType + " SET GXCS_BLOCK = @Show, GXCS_ADD = @Add, GXCS_CHG = @Change, GXCS_DEL = @Delete, GXCS_Excel = @Excel, GXCS_Report = @Report, GXCS_Print = @Print, GXCS_PrintFull = @PrintFull, GXCS_PrintHalf = @PrintHalf, GXCS_Counter = @Counter WHERE GXCS_GRDID = @UserId  AND GXCS_MDLID = @MenuId AND GXCS_MENU = @MenuName";
                                    command.CommandText = updateQuery;
                                    // Set parameter values
                                    command.Parameters.Clear();
                                    command.Parameters.AddWithValue("@UserId", userId);
                                    command.Parameters.AddWithValue("@Show", show ? 1 : 0);
                                    command.Parameters.AddWithValue("@Add", add ? 1 : 0);
                                    command.Parameters.AddWithValue("@Change", change ? 1 : 0);
                                    command.Parameters.AddWithValue("@Delete", delete ? 1 : 0);
                                    command.Parameters.AddWithValue("@Excel", excel ? 1 : 0);
                                    command.Parameters.AddWithValue("@Report", report ? 1 : 0);
                                    command.Parameters.AddWithValue("@Print", print ? 1 : 0);
                                    command.Parameters.AddWithValue("@PrintFull", printFull ? 1 : 0);
                                    command.Parameters.AddWithValue("@PrintHalf", printHalf ? 1 : 0);
                                    command.Parameters.AddWithValue("@Counter", counter ? 1 : 0);
                                    command.Parameters.AddWithValue("@MenuId", menuId);
                                    command.Parameters.AddWithValue("@MenuName", menuName);
                                    command.ExecuteNonQuery();
                                }
                                else
                                {
                                    // Execute the first query to retrieve data from GPRG_TBL
                                    string gprgQuery = "SELECT * FROM GPRG_TBL WHERE GPRG_MDLID = @MenuId AND GPRG_MENU = @MenuName";

                                    using (SqlCommand gprgCommand = new SqlCommand(gprgQuery, connection))
                                    {
                                        gprgCommand.Transaction = transaction; // Assign transaction to the command
                                        gprgCommand.Parameters.AddWithValue("@MenuId", menuId);
                                        gprgCommand.Parameters.AddWithValue("@MenuName", menuName);

                                        using (SqlDataReader gprgReader = gprgCommand.ExecuteReader())
                                        {
                                            string gxcsPrgId = "Default"; // Default value in case there are no rows returned

                                            if (gprgReader.Read()) // Check if there is data returned by the query
                                            {
                                                // Get the required data from the reader
                                                gxcsPrgId = gprgReader["GPRG_ID"].ToString();
                                            }

                                            // Close the reader before executing the insertCommand
                                            gprgReader.Close();

                                            // Now execute the second query to insert into GXCS_TBL
                                            string insertQuery = "INSERT INTO GXCS_TBL (GXCS_GRDID, GXCS_MDLID, GXCS_PRGID, GXCS_BLOCK, GXCS_ADD, GXCS_CHG, GXCS_DEL, GXCS_DATEIN, GXCS_MENU, GXCS_INDEX, GXCS_Excel, GXCS_Report, GXCS_Print, GXCS_PrintFull, GXCS_PrintHalf, GXCS_Counter) " +
                                                "VALUES (@GXCS_GRDID, @GXCS_MDLID, @GXCS_PRGID, @GXCS_BLOCK, @GXCS_ADD, @GXCS_CHG, @GXCS_DEL, @GXCS_DATEIN, @GXCS_MENU, @GXCS_INDEX, @GXCS_Excel, @GXCS_Report, @GXCS_Print, @GXCS_PrintFull, @GXCS_PrintHalf, @GXCS_Counter)";

                                            using (SqlCommand insertCommand = new SqlCommand(insertQuery, connection))
                                            {
                                                insertCommand.Transaction = transaction; // Assign transaction to the command
                                                insertCommand.Parameters.AddWithValue("@GXCS_GRDID", userId);
                                                insertCommand.Parameters.AddWithValue("@GXCS_MDLID", menuId);
                                                insertCommand.Parameters.AddWithValue("@GXCS_PRGID", gxcsPrgId); // Use the default or retrieved value
                                                insertCommand.Parameters.AddWithValue("@GXCS_BLOCK", show ? 1 : 0);
                                                insertCommand.Parameters.AddWithValue("@GXCS_ADD", add ? 1 : 0);
                                                insertCommand.Parameters.AddWithValue("@GXCS_CHG", change ? 1 : 0);
                                                insertCommand.Parameters.AddWithValue("@GXCS_DEL", delete ? 1 : 0);
                                                insertCommand.Parameters.AddWithValue("@GXCS_DATEIN", DateTime.Now);
                                                insertCommand.Parameters.AddWithValue("@GXCS_MENU", menuName);
                                                insertCommand.Parameters.AddWithValue("@GXCS_INDEX", key3);
                                                insertCommand.Parameters.AddWithValue("@GXCS_Excel", excel ? 1 : 0);
                                                insertCommand.Parameters.AddWithValue("@GXCS_Report", report ? 1 : 0);
                                                insertCommand.Parameters.AddWithValue("@GXCS_Print", print ? 1 : 0);
                                                insertCommand.Parameters.AddWithValue("@GXCS_PrintFull", printFull ? 1 : 0);
                                                insertCommand.Parameters.AddWithValue("@GXCS_PrintHalf", printHalf ? 1 : 0);
                                                insertCommand.Parameters.AddWithValue("@GXCS_Counter", counter ? 1 : 0);

                                                insertCommand.ExecuteNonQuery();
                                            }
                                        }
                                    }
                                }
                            


                            }

                            // Commit the transaction if all operations succeed
                            transaction.Commit();
                            MessageBox.Show("Data saved successfully.");
                          
                             
                            
                        }
                        catch (Exception ex)
                        {
                            // Rollback the transaction if an exception occurs
                            transaction.Rollback();
                            MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Console.Error.WriteLine(ex.Message);        
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

    }

}