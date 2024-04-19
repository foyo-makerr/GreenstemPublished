
using DevExpress.Xpo;
using DevExpress.XtraBars;
using DevExpress.XtraExport.Helpers;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using GreenStem.ClassModules;
using Microsoft.VisualBasic;
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

namespace GreenStem.Master
{
    public partial class frmSalesman : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        private SqlConnection connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["connection_string"].ConnectionString);
        private string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connection_string"].ConnectionString;
        private SqlCommand command;
        private SqlTransaction transaction;
        private DateTime lastClickTime = DateTime.MinValue;
        private int highlightedRowIndex = -1;// Variable to store the index of the previously highlighted row

        // Declare formBackground at the class level
        private Form formBackground;

        public frmSalesman()
        {
            InitializeComponent();
            LoadDataIntoGrid();
     
            Resize += FrmSalesmanDemo_Resize; 
            frmSalesmanForm.RecordUpdated += FrmSalesmanForm_RecordUpdated;
            DevExpress.XtraGrid.Views.Grid.GridView existingGridView = gvSalesman;
            gridViewCustomizer.ApplyCustomizations(existingGridView,true, false);
        }

        private void FrmSalesmanForm_RecordUpdated(string salesmanCode)
        {
            LoadDataIntoGrid(); // Refresh the grid after changes
                                // Highlight the row with the updated salesmanCode
            HighlightRow(salesmanCode);
        }
        private void HighlightRow(string salesmanCode)
        {
            // Iterate through the rows of the grid
            for (int i = 0; i < gvSalesman.RowCount; i++)
            {
                // Get the value of the "Salesman Code" column in the current row
                string code = gvSalesman.GetRowCellValue(i, "Salesman Code") as string;

                // If the salesmanCode matches the code in the current row
                if (code == salesmanCode)
                {
                    // Set the background color of the row to green
                    gvSalesman.Appearance.FocusedRow.BackColor = Color.LightGreen;
                    gvSalesman.Appearance.FocusedRow.BackColor2 = Color.LightGreen;
                    gvSalesman.Appearance.FocusedRow.Options.UseBackColor = true;

                    // Select and focus on the row to highlight it
                    gvSalesman.FocusedRowHandle = i;
                    gvSalesman.SelectRow(i);

                    // Update the highlighted row index
                    highlightedRowIndex = i;
                    return; // Exit the loop since the row has been found
                }
            }
        }
        private void FrmSalesmanDemo_Resize(object sender, EventArgs e)
        {
                // Dispose of the background form if it exists
                formBackground?.Dispose();
            
        }

        private void LoadDataIntoGrid()
        {
            try
            {
                // Create a SqlConnection
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                 
                    string query = "select [Salesman Code],[Salesman Name],[Salesman Contact],[MSalesman Code],[Active] from [Salesman_tbl] " + modPublicVariable.ReadLockType;

                    // Create a SqlDataAdapter to fetch data from the database
                    SqlDataAdapter dataAdapter = new SqlDataAdapter(query, connection);

                    // Create a DataTable to hold the data
                    DataTable dataTable = new DataTable();

                    // Fill the DataTable with data from the database
                    dataAdapter.Fill(dataTable);

                    // Disable editing in the GridView
                    gvSalesman.OptionsBehavior.Editable = false;

                    // Bind the DataTable to the gridControl1
                    gridControl1.DataSource = dataTable;
                    gvSalesman.FocusedRowChanged += gridView1_FocusedRowChanged;
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions, e.g., display an error message
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

  

        private void gridView1_DoubleClick(object sender, EventArgs e)
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
                int selectedRowHandle = gvSalesman.FocusedRowHandle;

                if (selectedRowHandle >= 0)
                {
                    string salesmanCode = gvSalesman.GetRowCellValue(selectedRowHandle, "Salesman Code") as string;
                    OpenSalesmanForm(salesmanCode);
                }
                else
                {
                    OpenSalesmanForm(null);
                }


            }
        }

        private void gridView1_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            // Reset the appearance settings of the previously highlighted row
            if (highlightedRowIndex >= 0)
            {
                gvSalesman.Appearance.FocusedRow.Reset();
                gvSalesman.Appearance.FocusedRow.Options.UseBackColor = false;
                highlightedRowIndex = -1; // Reset the highlighted row index
            }
        }

        private void btnItemDelete_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                // Check if the connection is closed and open it if necessary
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }

                // Check if a row is selected
                int selectedRowHandle = gvSalesman.FocusedRowHandle;
                if (selectedRowHandle >= 0)
                {
                    // Get the Salesman Code from the selected row
                    string salesmanCodeToDelete = gvSalesman.GetRowCellValue(selectedRowHandle, "Salesman Code") as string;

                    // Confirm with the user before deleting
                    DialogResult result = MessageBox.Show("Are you sure you want to delete this record?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        // Call the function to delete the record
                        using (var transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted))
                        {
                            // Initialize the command with the SQL query and transaction
                            using (var command = new SqlCommand("Delete from Salesman_tbl Where [Salesman Code] = @SalesmanCode", connection, transaction))
                            {
                                // Add parameters to the command
                                command.Parameters.AddWithValue("@SalesmanCode", salesmanCodeToDelete);

                                // Execute the command
                                command.ExecuteNonQuery();

                                // Commit the transaction
                                transaction.Commit();
                            }
                        }

                     
                      

                        LoadDataIntoGrid(); // Refresh the grid after changes
                    }
                }
                else
                {
                    MessageBox.Show("Please select a row to delete.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
                Console.WriteLine(ex.Message);
            }
        }

        private void btnItemNew_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                OpenSalesmanForm(null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                formBackground.Dispose();
            }
        }

        private void barButtonItem1_ItemClick(object sender, ItemClickEventArgs e)
        {

        }
        private void OpenSalesmanForm(string salesmanCode)
        {


            try
            {
                using (frmSalesmanForm salesmanForm = new frmSalesmanForm(salesmanCode))
                {
                    formBackground = new Form();
                    formBackground.StartPosition = FormStartPosition.Manual;
                    formBackground.FormBorderStyle = FormBorderStyle.None;
                    formBackground.Opacity = .50d;
                    formBackground.BackColor = Color.Black;
                    formBackground.WindowState = FormWindowState.Maximized;
                    formBackground.Location = this.Location;
                    formBackground.ShowInTaskbar = false;

                    formBackground.Show();
                    salesmanForm.Owner = formBackground;
                    // salesmanForm.StartPosition = FormStartPosition.CenterScreen;
                    // Get the screen's working area to calculate the center position
                    Rectangle workingArea = Screen.GetWorkingArea(this);
                    // Calculate the center position for the popup form
                    int posX = (workingArea.Width - salesmanForm.Width) / 2;
                    int posY = (workingArea.Height - salesmanForm.Height) / 2;
                    // Set the position of the popup form
                    salesmanForm.StartPosition = FormStartPosition.Manual;
                    salesmanForm.Location = new Point(posX, posY);
                    salesmanForm.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                formBackground.Dispose();
            }
        }

        private void btnEdit_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                int selectedRowHandle = gvSalesman.FocusedRowHandle;

                if (selectedRowHandle >= 0)
                {
                    string salesmanCode = gvSalesman.GetRowCellValue(selectedRowHandle, "Salesman Code") as string;
                    OpenSalesmanForm(salesmanCode);
                }
                else
                {
                    OpenSalesmanForm(null);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                formBackground.Dispose();
            }
        }
    }
}