using DevExpress.CodeParser;
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraReports;
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
using static DevExpress.XtraEditors.Mask.MaskSettings;
using System.Windows.Input;
using System.Reflection;
using DevExpress.XtraGrid.Views.Grid;
using GreenStem.LookUp;
using GreenStem.ClassModules;

namespace GreenStem.Security
{
    public partial class frmUserGroup : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        private string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connection_string"].ConnectionString;
        private string GGRPID;
        Dictionary<string, string> moduleMapping = new Dictionary<string, string>();
        public frmUserGroup()
        {
            InitializeComponent();
            DataTable groupModules = new DataTable();
            AddGroupModulesColumns(groupModules);
            gridControl1.DataSource = groupModules;
            LoadGroupModulesData();
            // Subscribe to the CellValueChanged event
            gridView1.CellValueChanging += GridView1_CellValueChanging;
            gridView1.Columns["Control"].Width = 20; // Adjust the width as per your requirement
        }
        private void AddGroupModulesColumns(DataTable dataTable)
        {
            // Add columns with desired headers
            dataTable.Columns.Add("Control", typeof(bool));
            dataTable.Columns.Add("Module ID", typeof(string));
            dataTable.Columns.Add("Description", typeof(string));
        }
        private void LoadGroupModulesData()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT [GMDL_ID], [GMDL_DESC] FROM GMDL_TBL";
                SqlCommand command = new SqlCommand(query, connection);
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    string moduleId = reader["GMDL_ID"].ToString();
                    string description = reader["GMDL_DESC"].ToString();
                    // Add a row to the DataTable
                    ((DataTable)gridControl1.DataSource).Rows.Add(false, moduleId, description);
                }
                reader.Close();
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
        }
        private void pbGroupAccessLookUp_Click(object sender, EventArgs e)
        {
            frmLookUp.OpenLookupForm("10004", HandleDataSelectedEvent, this, true  );
        }

        private void HandleDataSelectedEvent(Dictionary<string, object> selectedData)
        {
            this.GGRPID = selectedData["[GGRP_ID]"].ToString();
            string desc = selectedData["[GGRP_DESC]"].ToString();
            txtGroupId.Text = GGRPID.ToString();
            txtDescription.Text = desc;        
            // After handling all rows, uncheck the Control column for rows that were not updated
            foreach (DataRow row in ((DataTable)gridControl1.DataSource).Rows)
            {
                    // If the Module ID is not found in the mapping, set Control to false
               row["Control"] = false;
                
            }
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    // First, get the list of menu IDs and names
                    string getMenuQuery = "SELECT * FROM MENU_TBL WHERE Right([Menu Id], 3) = '000'";
                    SqlCommand getMenuCommand = new SqlCommand(getMenuQuery, connection);
                    SqlDataReader menuReader = getMenuCommand.ExecuteReader();

                    while (menuReader.Read())
                    {
                        // Next, for each menu, retrieve the associated module IDs
                        string menuId = menuReader["Menu ID"].ToString();
                        string menuName = menuReader["Menu Name"].ToString();

                         string modi2 = menuReader["SEQNO"].ToString();
                         string nodKey = menuReader["MENU ID1"].ToString().Trim() + "|" + menuReader["MENU Name"].ToString().Trim() + "|" + menuReader["MENU seqno"].ToString().Trim() + "|" + modi2;


                        // Create a new connection for moduleReader
                        using (SqlConnection moduleConnection = new SqlConnection(connectionString))
                        {
                            moduleConnection.Open();

                            string getModuleQuery = @"SELECT GMDL_TBL.GMDL_ID,GMDL_TBL.GMDL_DESC
                                  FROM GMDL_TBL
                                  INNER JOIN GXCS_TBL ON GMDL_TBL.GMDL_ID = GXCS_TBL.GXCS_MDLID
                                  WHERE GXCS_GRDID = @GGRPID
                                  AND GXCS_TBL.GXCS_MENU = @MenuName
                                  AND GXCS_TBL.GXCS_BLOCK = 1
                                  GROUP BY  GMDL_TBL.GMDL_ID ,GMDL_TBL.GMDL_DESC";

                            SqlCommand getModuleCommand = new SqlCommand(getModuleQuery, moduleConnection);
                            getModuleCommand.Parameters.AddWithValue("@GGRPID", GGRPID);
                            getModuleCommand.Parameters.AddWithValue("@MenuName", menuName);

                            // Add mapping to the dictionary
                            string menuIdmap = menuReader["MENU ID1"].ToString().Trim();
                            if (!moduleMapping.ContainsKey(menuIdmap))
                            {
                                // If the key doesn't exist, add it to the dictionary
                                moduleMapping.Add(menuIdmap, nodKey);
                            }



                            SqlDataReader moduleReader = getModuleCommand.ExecuteReader();

                            while (moduleReader.Read())
                            {
                                string moduleId = moduleReader["GMDL_ID"].ToString().Trim();


                                // Iterate over the existing rows in the DataTable
                                foreach (DataRow row in ((DataTable)gridControl1.DataSource).Rows)
                                {

                                    // Check if the Module ID in the existing grid matches with the current Module ID from moduleReader
                                    if (string.Equals(row["Module ID"].ToString().Trim(), moduleId))
                                    {
                                        // Update the Control column to true
                                        row["Control"] = true;
                                        break; // No need to continue iterating once a match is found
                                    }
                                }
                            }
                            moduleReader.Close();
                        }
                    }

                    menuReader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred while processing data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Console.WriteLine(ex.Message);
                }
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



                                // Check if the row exists in the database
                                string selectQuery = "SELECT COUNT(*) FROM GXCS_TBL WHERE GXCS_GRDID = @UserId AND GXCS_MDLID = @MenuId AND GXCS_MENU = @MenuName";

                                command.CommandText = selectQuery;
                                command.Parameters.Clear();
                                command.Parameters.AddWithValue("@UserId", GGRPID);
                                command.Parameters.AddWithValue("@MenuId", row["Module ID"].ToString());
                                string menuName = "";
                                string menuId = "";
                                string seqno = "";

                                object modulemap = moduleMapping;

                                foreach (KeyValuePair<string, string> entry in moduleMapping)
                                {
                                    string moduleId = entry.Key;
                                    string nodKey = entry.Value;

                                    // Compare the Module ID with the dictionary keys
                                    if (string.Equals(row["Module ID"].ToString().Trim(), moduleId, StringComparison.OrdinalIgnoreCase))
                                    {
                                        // Update the command parameters with nodKey
                                        string[] nodeData = nodKey.Split('|');
                                        if (nodeData.Length >= 2)
                                        {
                                            menuName = nodeData[1];
                                            command.Parameters.AddWithValue("@MenuName", menuName);

                                            menuId = nodeData[0];
                                            menuName = nodeData[1];
                                            seqno = nodeData[3];


                                            break; // No need to continue iterating once a match is found
                                        }
                                    }
                                }


                                int count = Convert.ToInt32(command.ExecuteScalar());
                                // Retrieve the value of the Control column
                                bool show = Convert.ToBoolean(row["Control"]);

                                if (count > 0)
                                {
                                    // Row exists, update the record
                                    string updateQuery = "UPDATE GXCS_TBL " + modPublicVariable.UpdateLockType +" SET GXCS_BLOCK = @Show, GXCS_ADD = @Add, GXCS_CHG = @Change, GXCS_DEL = @Delete, GXCS_Excel = @Excel, GXCS_Report = @Report, GXCS_Print = @Print, GXCS_PrintFull = @PrintFull, GXCS_PrintHalf = @PrintHalf, GXCS_Counter = @Counter WHERE GXCS_GRDID = @UserId  AND GXCS_MDLID = @MenuId AND GXCS_MENU = @MenuName";
                                    command.CommandText = updateQuery;
                                    // Set parameter values
                                    command.Parameters.Clear();
                                    command.Parameters.AddWithValue("@UserId", GGRPID);

                                    // Set the value of the @Show parameter based on the value of the Control column
                                    command.Parameters.AddWithValue("@Show", show ? 1 : 0);
                                    command.Parameters.AddWithValue("@Add", 1);
                                    command.Parameters.AddWithValue("@Change", 1);
                                    command.Parameters.AddWithValue("@Delete", 1);
                                    command.Parameters.AddWithValue("@Excel", 1);
                                    command.Parameters.AddWithValue("@Report", 1);
                                    command.Parameters.AddWithValue("@Print", 1);
                                    command.Parameters.AddWithValue("@PrintFull", 1);
                                    command.Parameters.AddWithValue("@PrintHalf", 1);
                                    command.Parameters.AddWithValue("@Counter", 1);
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
                                            if (gprgReader.Read()) // Check if there is data returned by the query
                                            {
                                                // Get the required data from the reader
                                                string gxcsPrgId = gprgReader["GPRG_ID"].ToString();
                                                // Close the reader before executing the insertCommand
                                                gprgReader.Close();
                                                // Now execute the second query to insert into GXCS_TBL
                                                string insertQuery = "INSERT INTO GXCS_TBL (GXCS_GRDID, GXCS_MDLID, GXCS_PRGID, GXCS_BLOCK, GXCS_ADD, GXCS_CHG, GXCS_DEL, GXCS_DATEIN, GXCS_MENU, GXCS_INDEX, GXCS_Excel, GXCS_Report, GXCS_Print, GXCS_PrintFull, GXCS_PrintHalf, GXCS_Counter) " +
                                                    "VALUES (@GXCS_GRDID, @GXCS_MDLID, @GXCS_PRGID, @GXCS_BLOCK, @GXCS_ADD, @GXCS_CHG, @GXCS_DEL, @GXCS_DATEIN, @GXCS_MENU, @GXCS_INDEX, @GXCS_Excel, @GXCS_Report, @GXCS_Print, @GXCS_PrintFull, @GXCS_PrintHalf, @GXCS_Counter)";

                                                using (SqlCommand insertCommand = new SqlCommand(insertQuery, connection))
                                                {
                                                    insertCommand.Transaction = transaction; // Assign transaction to the command
                                                    insertCommand.Parameters.AddWithValue("@GXCS_GRDID", GGRPID);
                                                    insertCommand.Parameters.AddWithValue("@GXCS_MDLID", menuId);
                                                    insertCommand.Parameters.AddWithValue("@GXCS_PRGID", gxcsPrgId);
                                                    insertCommand.Parameters.AddWithValue("@GXCS_BLOCK", show ? 1 : 0);
                                                    insertCommand.Parameters.AddWithValue("@GXCS_ADD", 1);
                                                    insertCommand.Parameters.AddWithValue("@GXCS_CHG", 1);
                                                    insertCommand.Parameters.AddWithValue("@GXCS_DEL", 1);
                                                    insertCommand.Parameters.AddWithValue("@GXCS_DATEIN", DateTime.Now);
                                                    insertCommand.Parameters.AddWithValue("@GXCS_MENU", menuName);
                                                    insertCommand.Parameters.AddWithValue("@GXCS_INDEX", seqno);
                                                    insertCommand.Parameters.AddWithValue("@GXCS_Excel", 1);
                                                    insertCommand.Parameters.AddWithValue("@GXCS_Report", 1);
                                                    insertCommand.Parameters.AddWithValue("@GXCS_Print", 1);
                                                    insertCommand.Parameters.AddWithValue("@GXCS_PrintFull", 1);
                                                    insertCommand.Parameters.AddWithValue("@GXCS_PrintHalf", 1);
                                                    insertCommand.Parameters.AddWithValue("@GXCS_Counter", 1);

                                                    insertCommand.ExecuteNonQuery();
                                                }
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

        private void txtGroupId_TextChanged(object sender, EventArgs e)
        {

        }
    }


}
