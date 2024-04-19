using DevExpress.Images;
using DevExpress.Utils;
using DevExpress.Utils.DragDrop;
using DevExpress.Utils.Svg;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Navigation;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraSplashScreen;
using GreenStem.ClassModules;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GreenStem.Std
{
    public partial class frmSetupMdiMain : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        private SqlConnection connectionString = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["connection_string"].ConnectionString);
        private string menuId;
        private string menuName;
        private string key3;
        private string formName;
        private string menuCaption;
        private Dictionary<string, Tuple<string, string, string>> menuInfoMap = new Dictionary<string, Tuple<string, string, string>>();
        private Dictionary<string, Image> imageCache = new Dictionary<string, Image>(StringComparer.OrdinalIgnoreCase);
        private Dictionary<string, string> imageKeyMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        private string previousMenuId = "";
        public delegate void SaveButtonClickHandler();
        public event SaveButtonClickHandler SaveButtonClicked;
        //private frmIcon iconForm;
        public frmSetupMdiMain()
        {
            InitializeComponent();
            LoadMenu();
            UserAccessControl();
            InitializeGridControl1DragDrop();
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

        private void UserAccessControl()
        {
            try
            {
                // Open the connection
                connectionString.Open();

                // Define the SQL query
                string query = @"SELECT GUXCS_TBL.[Group ID], GXCS_TBL.GXCS_PRGID, GXCS_TBL.GXCS_GRDID, GXCS_TBL.GXCS_BLOCK, GPRG_TBL.GPRG_MENU, GPRG_TBL.GPRG_INDEX
                         FROM GXCS_TBL
                         INNER JOIN GUXCS_TBL ON GXCS_TBL.GXCS_GRDID = GUXCS_TBL.[Group ID]
                         INNER JOIN GUSER_TBL ON GUXCS_TBL.[User ID] = GUSER_TBL.GUSER_ID
                         INNER JOIN GPRG_TBL ON GXCS_TBL.GXCS_PRGID = GPRG_TBL.GPRG_ID 
                             AND GXCS_TBL.[GXCS_MDLID] = GPRG_TBL.GPRG_MDLID 
                             AND GXCS_TBL.[GXCS_MENU] = GPRG_TBL.GPRG_MENU
                         WHERE (GUSER_TBL.GUSER_ID = @UserName) AND (GXCS_TBL.GXCS_BLOCK = 0)
                         GROUP BY GUXCS_TBL.[Group ID], GXCS_TBL.GXCS_PRGID, GXCS_TBL.GXCS_GRDID,
                                  GXCS_TBL.GXCS_BLOCK, GXCS_TBL.GXCS_PRGID, GXCS_TBL.GXCS_MDLID,
                                  GPRG_TBL.GPRG_MENU, GPRG_TBL.GPRG_INDEX";

                // Create SqlCommand and set parameters
                SqlCommand command = new SqlCommand(query, connectionString);
                command.Parameters.AddWithValue("@UserName", modPublicVariable.UserID);

                // Execute the query
                SqlDataReader reader = command.ExecuteReader();

                // Process the retrieved data
                while (reader.Read())
                {
                    string menuName = reader["GPRG_MENU"].ToString();
                    EnableMenuItem(menuName, tvwDB.Nodes);

                }

                // Close the reader
                reader.Close();
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex, nameof(mdiMain), "Std", "UserAcessControl", new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Close the connection
                connectionString.Close();
            }
        }
        private void EnableMenuItem(string menuName, TreeNodeCollection nodes)
        {
            foreach (TreeNode node in nodes)
            {
                string nodeMenuName = "";
                string[] nodeData = node.Name.Split('|');
                if (nodeData.Length >= 2)
                {

                    nodeMenuName = nodeData[1];

                }

                // Check if the node text matches the menuName
                if (nodeMenuName == menuName)
                {
                    // Remove the node

                    node.Remove();
                    return; // Exit the method to avoid modification during enumeration
                }
                else
                {
                    // Recursively search through child nodes
                    EnableMenuItem(menuName, node.Nodes);
                }
            }
        }

        private void LoadMenu()
        {
           
            tvwDB.ImageList = imageList1;
            tvwDB.ImageIndex = 0;
            string query = "SELECT * FROM MENU_TBL " + modPublicVariable.ReadLockType + " WHERE Right([Menu Id], 3) = '000' ORDER BY Seqno";

            using (SqlConnection connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["connection_string"].ConnectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(query, connection))
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string modi1 = reader["SEQNO"].ToString();
                        string nodKey = reader["MENU ID1"].ToString().Trim() + "|" +
                         reader["MENU Name"].ToString().Trim() + "|" +
                         reader["MENU seqno"].ToString().Trim() + "|" +
                         modi1 + "|" +
                         reader["Form Name"].ToString().Trim();
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

            using (SqlConnection innerConnection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["connection_string"].ConnectionString))
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
                            string nodKey = subReader["MENU ID1"].ToString().Trim() + "|" +
                                subReader["MENU Name"].ToString().Trim() + "|" +
                                subReader["MENU seqno"].ToString().Trim() + "|" +
                                modi1 + "|" +
                                subReader["Form Name"].ToString().Trim();
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

            using (SqlConnection subConnection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["connection_string"].ConnectionString))
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
                            string nodKey = subReader["MENU ID1"].ToString().Trim() + "|" +
                               subReader["MENU Name"].ToString().Trim() + "|" +
                               subReader["MENU seqno"].ToString().Trim() + "|" +
                               modi2 + "|" +
                               subReader["Form Name"].ToString().Trim();
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
            string subQuery = "SELECT * FROM MENU_TBL " + modPublicVariable.ReadLockType + " WHERE [Menu ID]=@menuId AND [Menu Type]='002' AND [Menu Sub Seqno]=@menuSubSeqno ORDER BY [seqno]";

            using (SqlConnection subConnection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["connection_string"].ConnectionString))
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
                            string nodKey = subReader["MENU ID1"].ToString().Trim() + "|" +
                              subReader["MENU Name"].ToString().Trim() + "|" +
                              subReader["MENU seqno"].ToString().Trim() + "|" +
                              modi3 + "|" +
                              subReader["Form Name"].ToString().Trim();
                            string nodText = subReader["Menu Caption"].ToString();
                            parentNode.Nodes.Add(nodKey, nodText);
                        }
                    }
                }
            }
        }

        private void tvwDB_AfterSelect_1(object sender, TreeViewEventArgs e)
        {
            lblModulePath.Visible = true;
            lblModulePath.Text = ": " + tvwDB.PathSeparator + e.Node.FullPath;
            string[] nodeData = e.Node.Name.Split('|');
            if (nodeData.Length >= 2)
            {
                string menuId = nodeData[0];
                this.menuId = menuId;
                this.menuName = nodeData[1];
                this.key3 = nodeData[3];
                this.formName = nodeData[4];
                this.menuCaption = e.Node.Text;
                // Check if the menuId is different from the previousMenuId
                if (menuId != previousMenuId)
                {
                   
                    imageKeyMap.Clear();
                    // Call the LoadDataIntoGrid method with the retrieved menuId and menuName
                    LoadDataIntoGrid(menuId);

                    // Update the previousMenuId
                    previousMenuId = menuId;
                }
            }
        }

        private void LoadDataIntoGrid(string menuId)
        {
            gridControl1.DataSource = null; // Clear the GridControl
            string query = @"
    SELECT [Menu Caption], [Menu Image], [Group ID], [Item ID], [Item Size], [Menu Color]
    FROM [Menu_MainScreen]
    WHERE [menu id1] = @MenuId
    ORDER BY [Group ID] ASC, CAST([Item ID] AS INT) ASC";

            using (SqlConnection connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["connection_string"].ConnectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@MenuId", menuId);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    // Create a new DataTable with the required columns
                    DataTable dataTable = new DataTable();
                    dataTable.Columns.Add("Menu Caption", typeof(string));
                    dataTable.Columns.Add("Menu Image", typeof(SvgImage));
                    dataTable.Columns.Add("Group ID", typeof(string));
                    dataTable.Columns.Add("Item Size", typeof(string));
                    dataTable.Columns.Add("Menu Color", typeof(System.Drawing.Color));

                    // HashSet to store unique menu captions
                    HashSet<string> uniqueMenuCaptions = new HashSet<string>();

                    // Read data from the reader and add rows to the DataTable
                    int rowIndex = 0; // Track the row index
                    while (reader.Read())
                    {
                        string menuCaption = reader["Menu Caption"].ToString();

                        // Check if the menu caption is already added
                        if (!uniqueMenuCaptions.Contains(menuCaption))
                        {
                            string groupID = reader["Group ID"].ToString();
                            string itemSize = reader["Item Size"].ToString();
                            Color menuColor = ColorTranslator.FromHtml(reader["Menu Color"].ToString());

                            // Get the SVG image key from the database
                            string svgImageKey = reader["Menu Image"].ToString();

                            // Load the SVG image from resources
                            SvgImage menuImage = null;
                            if (!string.IsNullOrEmpty(svgImageKey))
                            {
                                // Assuming SvgImage is a custom class to handle SVG images
                                menuImage = ImageResourceCache.Default.GetSvgImage(svgImageKey);
                            }

                            // Create a new DataRow and populate it with the retrieved values
                            DataRow newRow = dataTable.NewRow();
                            newRow["Menu Caption"] = menuCaption;
                            newRow["Menu Image"] = menuImage;
                            newRow["Group ID"] = groupID;
                            newRow["Item Size"] = itemSize;
                            newRow["Menu Color"] = menuColor;

                            // Add the new row to the DataTable
                            dataTable.Rows.Add(newRow);

                            // Add the image key to the map with the corresponding row index
                            imageKeyMap.Add(menuCaption, svgImageKey);

                            // Add the menu caption to the HashSet
                            uniqueMenuCaptions.Add(menuCaption);

                            // Increment the row index
                            rowIndex++;
                        }
                    }

                    // Close the reader
                    reader.Close();

                    // Set the DataTable as the DataSource for the GridControl
                    gridControl1.DataSource = dataTable;
                    CustomizeGridColumns();
                 

                    // Make the [Menu Caption] column read-only
                    GridView view = (GridView)gridControl1.MainView;
                    view.Columns["Menu Caption"].OptionsColumn.ReadOnly = true;
                }
                catch (Exception ex)
                {
                    // Handle exceptions
                    Console.WriteLine("Error: " + ex.Message);
                }
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

        private void CustomizeGridColumns()
        {
            // Access the GridView from the GridControl
            GridView view = gridControl1.MainView as GridView;

            GridColumn existingColumn = view.Columns["Action"];
            if (existingColumn == null)
            {
                // Create a new RepositoryItemButtonEdit
                RepositoryItemButtonEdit riButtonEdit = new RepositoryItemButtonEdit();

                // Set up the button properties
                riButtonEdit.Buttons[0].Caption = "Edit Image";
                riButtonEdit.Buttons[0].Kind = ButtonPredefines.Glyph;

                // Set the button click event handler
                riButtonEdit.ButtonClick += RiButtonEdit_ButtonClick;

                // Set the text edit style
                riButtonEdit.TextEditStyle = TextEditStyles.HideTextEditor;

                // Add a column to the GridView and set its properties
                GridColumn column = view.Columns.AddField("Action"); // Set the column name to "Action"
                column.Visible = true;
                column.ColumnEdit = riButtonEdit;
            }

            // Customize the 'Group ID' column to only allow selection from 1 to 5
            RepositoryItemComboBox riComboBox = new RepositoryItemComboBox();
            riComboBox.Items.AddRange(new object[] { "1", "2", "3", "4", "5" });
            view.Columns["Group ID"].ColumnEdit = riComboBox;

            // Customize the 'Item Size' column to only allow 'Small', 'Medium', 'Big'
            RepositoryItemComboBox riComboBoxSize = new RepositoryItemComboBox();
            riComboBoxSize.Items.AddRange(new object[] { "Medium", "Big" });
            view.Columns["Item Size"].ColumnEdit = riComboBoxSize;

            // Customize the 'Menu Color' column to show a color picker

            GridColumn menuColorColumn = view.Columns["Menu Color"];
            if (menuColorColumn != null)
            {
                // Create a custom repository item for color editing
                RepositoryItemColorEdit riColorEdit = new RepositoryItemColorEdit();
                // Allow custom colors to be entered
                riColorEdit.CustomColors = new Color[] { Color.Red, Color.Green, Color.Blue }; // Example custom colors
                riColorEdit.ShowCustomColors = true;
                // Allow editing of the color's RGB values
                riColorEdit.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;

                // Handle the EditValueChanged event to validate and handle RGB values
                riColorEdit.EditValueChanged += (sender, e) =>
                {
                    ColorEdit colorEdit = sender as ColorEdit;
                    if (colorEdit != null)
                    {
                        // Validate and process RGB values here if needed
                        // Example validation:
                        if (!IsValidRGB(colorEdit.Color))
                        {
                            // Show error message or handle invalid color input
                            MessageBox.Show("Invalid RGB values entered. Please enter valid RGB values.");
                            colorEdit.Color = Color.White; // Reset to default color
                        }
                    }
                };

                // Set the custom editor for the 'Menu Color' column
                menuColorColumn.ColumnEdit = riColorEdit;
            }
        }
        private bool IsValidRGB(Color color)
        {
            // Validate RGB values (e.g., range from 0 to 255)
            return color.R >= 0 && color.R <= 255 &&
                   color.G >= 0 && color.G <= 255 &&
                   color.B >= 0 && color.B <= 255;
        }
        private void RiButtonEdit_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            GridView view = gridView1;

            int rowHandle = view.FocusedRowHandle;

            // Get the menu caption from the clicked row
            string menuCaption = view.GetRowCellValue(rowHandle, "Menu Caption").ToString();

            // Open the frmIcon form to edit the image for the clicked row
            frmIcon iconForm = new frmIcon();
            iconForm.GalleryItemClick += (s, args) => IconForm_GalleryItemClick(s, args, menuCaption); // Subscribe to the custom event
            iconForm.ShowDialog();
        }

        private void IconForm_GalleryItemClick(object sender, GalleryItemClickEventArgs e, string menuCaption)
        {
            // Retrieve the menu caption from the parameter
            string caption = menuCaption;

            // Access the GridView and get the data row using the menu caption
            GridView view = gridView1;
            DataRowView rowView = view.GetRow(view.LocateByValue("Menu Caption", caption)) as DataRowView;

            if (rowView != null)
            {
                // Access the data row's data as needed
                string svgImageKey = e.Item.Description;

                // Load the SVG image from resources
                SvgImage menuImage = null;
                if (!string.IsNullOrEmpty(svgImageKey))
                {
                    menuImage = ImageResourceCache.Default.GetSvgImage(svgImageKey);
                }

                // Update the "Menu Image" column in the DataRowView
                rowView["Menu Image"] = menuImage;

                // Update the imageKeyMap with the menu caption
                imageKeyMap[caption] = svgImageKey;

                // Refresh the view to reflect changes
                view.RefreshData();
            }

      // Close the frmIcon form
      ((frmIcon)sender).Close();
        }
        private void btnItemSave_ItemClick(object sender, ItemClickEventArgs e)
        {
            // Access the GridView from the GridControl
            if (gridView1.PostEditor())
                gridView1.UpdateCurrentRow();

            // Get the DataTable from the GridControl's DataSource
            if (gridControl1.DataSource is DataTable dataTable)
            {
                using (SqlConnection connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["connection_string"].ConnectionString))
                {
                    connection.Open();

                    // Begin the transaction with the desired isolation level
                    SqlTransaction transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted);

                    try
                    {
                        using (SqlCommand command = connection.CreateCommand())
                        {
                            // Assign the transaction to the command
                            command.Transaction = transaction;

                            // Fetch existing records from the database for the specific [menu id1]
                            string fetchQuery = "SELECT [Menu Caption] FROM [Menu_MainScreen] WHERE [menu id1] = @MenuId";
                            command.CommandText = fetchQuery;
                            command.Parameters.AddWithValue("@MenuId", menuId);

                            DataTable dbRecords = new DataTable();
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                dbRecords.Load(reader);
                            }

                            foreach (DataRow row in dataTable.Rows)
                            {
                                string menuCaption = row["Menu Caption"].ToString();
                                // Retrieve menuId, menuName, and formName from the map
                                int rowIndex = dataTable.Rows.IndexOf(row);
                                string menuImage = imageKeyMap.ContainsKey(menuCaption) ? imageKeyMap[menuCaption] : "";
                                string groupId = row["Group ID"].ToString();
                                string itemSize = row["Item Size"].ToString();
                                Color menuColor = (Color)row["Menu Color"];
                                string hexColor = ColorTranslator.ToHtml(menuColor);

                                string query;
                                if (dbRecords.Select($"[Menu Caption] = '{menuCaption}'").Any())
                                {
                                    // Update existing record
                                    query = @"UPDATE [Menu_MainScreen]
                                SET [Menu Image] = @MenuImage,
                                    [Group ID] = @GroupId,
                                    [Item Size] = @ItemSize,
                                    [Menu Color] = @MenuColor,
                                  
                                   
                                    [Item ID] = @ItemId 
                                WHERE [Menu Caption] = @MenuCaption";

                                    // Add parameters for update query
                                    command.Parameters.Clear();
                                    command.Parameters.AddWithValue("@MenuCaption", menuCaption);
                                    command.Parameters.AddWithValue("@MenuImage", menuImage);
                                    command.Parameters.AddWithValue("@GroupId", groupId);
                                    command.Parameters.AddWithValue("@ItemSize", itemSize);
                                    command.Parameters.AddWithValue("@MenuColor", hexColor);

                                    command.Parameters.AddWithValue("@ItemId", rowIndex); // Add [Item ID] parameter based on the rowIndex
                                }
                                else
                                {
                                    // Insert new record
                                    query = @"INSERT INTO [Menu_MainScreen] ([Menu Caption], [Menu Image], [Group ID], [Item Size], [Menu Color], [menu id1], [Menu Name], [Item ID], [Form Name])
                                        VALUES (@MenuCaption, @MenuImage, @GroupId, @ItemSize, @MenuColor, @MenuId1, @MenuName, @ItemId, @FormName)";

                                    if (menuInfoMap.ContainsKey(menuCaption))
                                    {
                                        Tuple<string, string, string> menuInfo = menuInfoMap[menuCaption];
                                        string menuId = menuInfo.Item1;
                                        string menuName = menuInfo.Item2;
                                        string formName = menuInfo.Item3;

                                        // Add parameters for insert query
                                        command.Parameters.Clear();
                                        command.Parameters.AddWithValue("@MenuCaption", menuCaption);
                                        command.Parameters.AddWithValue("@MenuImage", menuImage);
                                        command.Parameters.AddWithValue("@GroupId", groupId);
                                        command.Parameters.AddWithValue("@ItemSize", itemSize);
                                        command.Parameters.AddWithValue("@MenuColor", hexColor);
                                        command.Parameters.AddWithValue("@MenuId1", menuId); // Add [menu id1] parameter
                                        command.Parameters.AddWithValue("@MenuName", menuName); // Add [Menu Name] parameter
                                        command.Parameters.AddWithValue("@ItemId", rowIndex); // Add [Item ID] parameter based on the rowIndex
                                        command.Parameters.AddWithValue("@FormName", formName); // Add [Form Name] parameter
                                    }


                                }

                                // Set the command text and execute the command
                                command.CommandText = query;
                                command.ExecuteNonQuery();
                            }
                            // Compare with the fetched records and delete unmatched records
                            foreach (DataRow dbRow in dbRecords.Rows)
                            {
                                string dbMenuCaption = dbRow["Menu Caption"].ToString();
                                if (!((DataTable)gridControl1.DataSource).Select($"[Menu Caption] = '{dbMenuCaption}'").Any())
                                {
                                    // Delete the record as it's not in the grid
                                    string deleteQuery = "DELETE FROM [Menu_MainScreen] WHERE [Menu Caption] = @MenuCaption AND [menu id1] = @MenuId";
                                    command.CommandText = deleteQuery;
                                    command.Parameters.Clear();
                                    command.Parameters.AddWithValue("@MenuCaption", dbMenuCaption);
                                    command.Parameters.AddWithValue("@MenuId", menuId);
                                    command.ExecuteNonQuery();
                                }
                            }

                            // Commit the transaction
                            transaction.Commit();
                            SaveButtonClicked?.Invoke();
                         
                            MessageBox.Show("Data saved successfully.");

                        }
                    }
                    catch (Exception ex)
                    {
                        // Rollback the transaction in case of an error
                        transaction.Rollback();
                        MessageBox.Show("An error occurred while saving data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Console.Write(ex.ToString());       
                    }
                }
            }
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            // Check if a node is selected in the TreeView
            if (tvwDB.SelectedNode != null)
            {
                // Extract data from the selected node
                string menuId = this.menuId;
                string menuName = this.menuName;
                string formName = this.formName;
                string menuCaption = this.menuCaption;

                // Add the menuCaption and its hidden data to the gridControl
                DataTable dataSource = gridControl1.DataSource as DataTable;
                if (dataSource == null)
                {
                    dataSource = new DataTable();
                    dataSource.Columns.Add("Menu Caption", typeof(string));
                    gridControl1.DataSource = dataSource;
                }

                // Check if the menuCaption already exists in the gridControl1
                bool exists = false;
                foreach (DataRow row in dataSource.Rows)
                {
                    if (row["Menu Caption"].ToString() == menuCaption)
                    {
                        exists = true;
                        break;
                    }
                }

                if (!exists)
                {
                    dataSource.Rows.Add(menuCaption);

                    // Store the hidden data in the dictionary
                    menuInfoMap[menuCaption] = Tuple.Create(menuId, menuName, formName);
                }
                else
                {
                    MessageBox.Show("This Menu already exists in the grid.", "Duplicate Menu Caption", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }

        }

        private void btnDeSelect_Click(object sender, EventArgs e)
        {
            // Check if there is a selected row in the gridControl1
            if (gridView1.SelectedRowsCount > 0)
            {
                // Get the selected row handle
                int selectedRowHandle = gridView1.GetSelectedRows()[0];

                // Get the selected row data
                DataRowView selectedRow = (DataRowView)gridView1.GetRow(selectedRowHandle);

                // Get the menu caption of the selected row
                string menuCaption = selectedRow["Menu Caption"].ToString();

                // Remove the selected row from the gridControl1
                gridView1.DeleteRow(selectedRowHandle);

                // Remove the menu caption from the menuInfoMap dictionary
                if (menuInfoMap.ContainsKey(menuCaption))
                {
                    menuInfoMap.Remove(menuCaption);
                }
            }
            else
            {
                MessageBox.Show("Please select a row to delete.", "No Row Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}