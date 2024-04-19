using DevExpress.XtraBars;
using GreenStem.ClassModules;
using GreenStem.LookUp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GreenStem
{
    public partial class frmSalesmanForm : DevExpress.XtraBars.Ribbon.RibbonForm
    {

        private SqlConnection connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["connection_string"].ConnectionString);
       private string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connection_string"].ConnectionString;
        private SqlCommand command;
        private SqlTransaction transaction;
        public event EventHandler DataUpdated;
        public delegate void RecordUpdatedEventHandler(string salesmanCode);
        public static event RecordUpdatedEventHandler RecordUpdated;
  
        public frmSalesmanForm()
        {
            initializeComponent();


        }
        public frmSalesmanForm(string code)
        {
            initializeComponent();
            LoadSalesmanDataFromDatabase(code);
          
        }
  
    
        public void initializeComponent()
        {
            InitializeComponent();
           
        }
       
        private void LoadSalesmanDataFromDatabase(string salesmanCode)
        {
            try
            {
                if (string.IsNullOrEmpty(salesmanCode))
                    return;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT * FROM Salesman_tbl WHERE [Salesman Code] = @SalesmanCode";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@SalesmanCode", salesmanCode);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                txtSalesmanCode.Text = salesmanCode;
                                txtSalesmanName.Text = reader["Salesman Name"] != DBNull.Value ? reader["Salesman Name"].ToString() : "";
                                txtContact.Text = reader["Salesman Contact"] != DBNull.Value ? reader["Salesman Contact"].ToString() : "";
                                txtCommision.Text = decimal.TryParse(reader["Commision"].ToString(), out decimal commissionValue) ? commissionValue.ToString("0.00") : "0.00";

                                if (!reader.IsDBNull(reader.GetOrdinal("Password")))
                                {
                                    string password = reader["Password"].ToString();
                                    txtPassword.Visible = string.IsNullOrEmpty(password);
                                    txtPassword.Text = password.Trim();
                                }
                                else
                                {
                                    txtPassword.Visible = true;
                                    txtPassword.Text = "";
                                }

                                txtMasterSalesman.Text = reader["MSalesman Code"] != DBNull.Value ? reader["MSalesman Code"].ToString().Trim() : "";
                                chkMobile.Checked = reader["Mobile"] != DBNull.Value ? Convert.ToBoolean(reader["Mobile"]) : false;
                                txtDays.Text = reader["Total Days"] != DBNull.Value ? reader["Total Days"].ToString() : "";
                                txtCreditLimit.Text = decimal.TryParse(reader["Credit Limit"].ToString(), out decimal creditLimitValue) ? creditLimitValue.ToString("0.00") : "0.00";
                                txtSalesTarget.Text = reader["Sales Target"] != DBNull.Value ? reader["Sales Target"].ToString() : "";
                                chkActive.Checked = reader["Active"] != DBNull.Value ? (reader["Active"].ToString() == "1") : false;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions, e.g., display an error message
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void salesManLookUp_Click(object sender, EventArgs e)
        {
            //call lookup form
            frmLookUp.OpenLookupForm("10016", HandleDataSelectedEvent, this, false);
        
        }

      
        private void HandleDataSelectedEvent(Dictionary<string, object> selectedData)
        {
            // Process the selected data here
            // Example:
            string salesmanCode = selectedData["[Salesman Code]"].ToString();

            // Call the method to retrieve data from the database
            LoadSalesmanDataFromDatabase(salesmanCode);
        }



  


        private void clearScreen()
        {
            // Clear the text of TextBoxes
            txtSalesmanCode.Text = string.Empty;
            txtSalesmanName.Text = string.Empty;
            txtContact.Text = string.Empty;
            txtCommision.Text = string.Empty;
            txtPassword.Text = string.Empty;
            txtMasterSalesman.Text = string.Empty;
            chkMobile.Checked = false;
            txtDays.Text = string.Empty;
            txtCreditLimit.Text = string.Empty;
            txtSalesTarget.Text = string.Empty;

            // Uncheck the CheckBox
            chkActive.Checked = false;

            // Set the focus to the first TextBox
            this.ActiveControl = txtSalesmanCode;
        }
        private void SaveOrUpdateRecord()
        {
            try
            {
                // Check for invalid data
                if (inValidData())
                {
                    return;
                }

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

                        // Check if the record exists
                        bool newRecord = CheckExist(txtSalesmanCode.Text.Trim());

                        // Perform actions based on newRecord value
                        if (newRecord)
                        {
                            // Update existing record
                            command.CommandText = "Update Salesman_tbl "+ modPublicVariable.UpdateLockType + " Set [Salesman Code]=@SalesmanCode, [Salesman Name]=@SalesmanName,[Salesman Contact]=@SalesmanContact,[Commision]=@Commision, " +
                                "[Password]=@Password,[MSalesman Code]=@MSalesmanCode,[Mobile]=@Mobile,[Total Days]=@TotalDays, " +
                                "[Active]=@Active,[Credit Limit]=@CreditLimit,[Sales Target]=@SalesTarget " +
                                "Where [Salesman Code] = @SalesmanCode";
                        }
                        else
                        {
                            // Insert new record
                            command.CommandText = "Insert Into Salesman_tbl([Salesman Code], [Salesman Name],[Salesman Contact],[Commision], " +
                                "[Password],[MSalesman Code],[Mobile],[Total Days],[Active],[Credit Limit],[Sales Target]) " +
                                "Values(@SalesmanCode,@SalesmanName,@SalesmanContact,@Commision, " +
                                "@Password,@MSalesmanCode,@Mobile,@TotalDays,@Active,@CreditLimit,@SalesTarget)";
                        }

                        // Add parameters
                        command.Parameters.AddWithValue("@SalesmanCode", txtSalesmanCode.Text.Trim());
                        command.Parameters.AddWithValue("@SalesmanName", txtSalesmanName.Text.Trim());
                        command.Parameters.AddWithValue("@SalesmanContact", txtContact.Text.Trim());
                        command.Parameters.AddWithValue("@Commision", decimal.TryParse(txtCommision.Text.Trim(), out decimal commisionValue) ? commisionValue : 0);
                        command.Parameters.AddWithValue("@Password", txtPassword.Text.Trim());
                        command.Parameters.AddWithValue("@MSalesmanCode", string.IsNullOrEmpty(txtMasterSalesman.Text.Trim()) ? txtSalesmanCode.Text.Trim() : txtMasterSalesman.Text.Trim());
                        command.Parameters.AddWithValue("@Mobile", chkMobile.Checked);
                        command.Parameters.AddWithValue("@TotalDays", txtDays.Text.ToString());
                        command.Parameters.AddWithValue("@Active", chkActive.Checked ? "1" : "0"); // Set Active based on the checked state
                        command.Parameters.AddWithValue("@CreditLimit", txtCreditLimit.Text.Trim());
                        command.Parameters.AddWithValue("@SalesTarget", decimal.TryParse(txtSalesTarget.Text.Trim(), out decimal salesTargetValue) ? salesTargetValue : 0);

                        // Execute command
                        command.ExecuteNonQuery();
                        // Clear existing parameters before adding new ones
                        command.Parameters.Clear();
                        // Check and handle MasterSalesman table
                        if (string.IsNullOrEmpty(txtMasterSalesman.Text.Trim()))
                        {

                            command.CommandText = "SELECT COUNT(*) FROM MasterSalesman_Tbl WHERE [Salesman Code]=@MasterSalesmanCode";
                            command.Parameters.AddWithValue("@MasterSalesmanCode", txtMasterSalesman.Text.Trim());
                            int count = (int)command.ExecuteScalar();

                            if (count == 0)
                            {
                                // Insert new MasterSalesman entry
                                command.CommandText = "Insert Into MasterSalesman_Tbl([Salesman Code], [Salesman Name]) " +
                                    "Values(@MasterSalesmanCode,@SalesmanName)";
                                command.Parameters.AddWithValue("@SalesmanName", txtSalesmanName.Text.Trim());
                                command.Parameters.AddWithValue("@MasterSalesmanCode", txtSalesmanName.Text.Trim());
                                command.ExecuteNonQuery();
                            }
                            else
                            {
                                command.CommandText = "delete from MasterSalesman_Tbl Where [Salesman Code]=@SalesmanCode";
                                command.Parameters.AddWithValue("@SalesmanCode", txtSalesmanCode.Text.Trim());
                                command.ExecuteNonQuery();
                            }
                        }

                        // Commit transaction
                        transaction.Commit();
                   

                      


                        MessageBox.Show("Save successful", "Save Record", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        // Raise the DataUpdated event
                        RecordUpdated?.Invoke(txtSalesmanCode.Text.Trim()); // Raise the event
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions

                MessageBox.Show($"Error: {ex.Message}");
                Console.WriteLine(ex.Message);
               
               
            }
        }

        //private void SaveOrUpdateRecord()
        //{
        //    try
        //    {
        //        // Check for invalid data
        //        if (inValidData())
        //        {
        //            return;
        //        }

        //        // Check if the connection is closed and open it if necessary
        //        if (connection.State == ConnectionState.Closed)
        //        {
        //            connection.Open();
        //        }

        //        // Begin transaction
        //        using (var transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted))
        //        {
        //            using (var command = connection.CreateCommand())
        //            {
        //                command.Transaction = transaction;
        //                command.CommandType = CommandType.StoredProcedure;
        //                command.CommandText = "SaveSalesmanData"; // Name of the stored procedure

        //                // Add parameters for the stored procedure
        //                command.Parameters.AddWithValue("@SalesmanCode", txtSalesmanCode.Text.Trim());
        //                command.Parameters.AddWithValue("@SalesmanName", txtSalesmanName.Text.Trim());
        //                command.Parameters.AddWithValue("@SalesmanContact", txtContact.Text.Trim());
        //                command.Parameters.AddWithValue("@Commision", decimal.TryParse(txtCommision.Text.Trim(), out decimal commisionValue) ? commisionValue : 0);
        //                command.Parameters.AddWithValue("@Password", txtPassword.Text.Trim());
        //                command.Parameters.AddWithValue("@MSalesmanCode", string.IsNullOrEmpty(txtMasterSalesman.Text.Trim()) ? DBNull.Value : (object)txtMasterSalesman.Text.Trim());
        //                command.Parameters.AddWithValue("@Mobile", chkMobile.Checked);
        //                command.Parameters.AddWithValue("@TotalDays", int.TryParse(txtDays.Text.Trim(), out int daysValue) ? daysValue : 0);
        //                command.Parameters.AddWithValue("@Active", chkActive.Checked);
        //                command.Parameters.AddWithValue("@CreditLimit", decimal.TryParse(txtCreditLimit.Text.Trim(), out decimal creditLimitValue) ? creditLimitValue : 0);
        //                command.Parameters.AddWithValue("@SalesTarget", decimal.TryParse(txtSalesTarget.Text.Trim(), out decimal salesTargetValue) ? salesTargetValue : 0);

        //                // Execute the stored procedure
        //                command.ExecuteNonQuery();

        //                // Commit transaction
        //                transaction.Commit();

        //                MessageBox.Show("Save successful", "Save Record", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //                RecordUpdated?.Invoke(txtSalesmanCode.Text.Trim()); // Raise the event
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // Handle exceptions
        //        MessageBox.Show($"Error: {ex.Message}");
        //        Console.WriteLine(ex.Message);


        //    }
        //}
        private bool CheckExist(string salesmanCode)
        {
            try
            {
                if (string.IsNullOrEmpty(salesmanCode))
                    return false;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();


                    string query = "SELECT COUNT(*) FROM Salesman_tbl WHERE [Salesman Code] = @SalesmanCode";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@SalesmanCode", salesmanCode);
                        int count = (int)command.ExecuteScalar();
                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions, e.g., display an error message
                MessageBox.Show($"Error: {ex.Message}");
                return false;
            }
        }

        public bool inValidData()
        {

            try
            {


                if (string.IsNullOrEmpty(txtSalesmanCode.Text.Trim()))
                {

                    MessageBox.Show("Salesman Code Cannot Be Left Blank", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtSalesmanCode.Focus();
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }


        private void btnClear_ItemClick(object sender, ItemClickEventArgs e)
        {
            clearScreen();
        }


        private void btnSave_ItemClick(object sender, ItemClickEventArgs e)
        {
            Stopwatch stopwatch1 = new Stopwatch();
            stopwatch1.Start();
            SaveOrUpdateRecord();
            stopwatch1.Stop();
            Console.WriteLine("Elapsed Time for SaveOrUpdateRecord using method of store procesure is : " + stopwatch1.Elapsed);
        }
        private void btnSaveClose_ItemClick(object sender, ItemClickEventArgs e)
        {
            SaveOrUpdateRecord();
            this.Close();
        }

        private void btnClose_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.Close();
        }

        private void btnDelete_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                // Check if the connection is closed and open it if necessary
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }

                

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
                                command.Parameters.AddWithValue("@SalesmanCode", txtSalesmanCode.Text.Trim());

                                // Execute the command
                                command.ExecuteNonQuery();

                                // Commit the transaction
                                transaction.Commit();
                            }
                        }
            
                   
                        this.Close();
                        RecordUpdated?.Invoke(txtSalesmanCode.Text.Trim()); // Raise the event


                }
            
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
                Console.WriteLine(ex.Message);
            }
        }

        private void btnResetChange_ItemClick(object sender, ItemClickEventArgs e)
        {
            // Call the method to retrieve data from the database
            LoadSalesmanDataFromDatabase(txtSalesmanCode.Text.Trim());
        }

        private void txtCommision_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Allowing only numeric input and control keys like backspace and delete
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
            }

            // Allowing only one decimal point
            if (e.KeyChar == '.' && (sender as TextBox).Text.Contains('.'))
            {
                e.Handled = true;
            }
        }

        private void txtSalesTarget_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Allowing only numeric input and control keys like backspace and delete
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
            }

            // Allowing only one decimal point
            if (e.KeyChar == '.' && (sender as TextBox).Text.Contains('.'))
            {
                e.Handled = true;
            }
        }

        private void txtCreditLimit_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Allowing only numeric input and control keys like backspace and delete
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
            }

            // Allowing only one decimal point
            if (e.KeyChar == '.' && (sender as TextBox).Text.Contains('.'))
            {
                e.Handled = true;
            }
        }

        private void txtSalesmanCode_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSalesmanCode.Text))
            {
                errorProvider1.SetError(txtSalesmanCode, "Salesman code cannot be empty.");
            }
            else
            {
                errorProvider1.SetError(txtSalesmanCode, ""); // Clear the error message if the text is not empty
            }
        }

        private void txtCommision_Enter(object sender, EventArgs e)
        {
            if (txtCommision.Text == "0.00")
            {
                txtCommision.Text = "";
            }
        }

        private void txtSalesTarget_Enter(object sender, EventArgs e)
        {
            if (txtSalesTarget.Text == "0.00")
            {
                txtSalesTarget.Text = "";
            }
        }

        private void txtCreditLimit_Enter(object sender, EventArgs e)
        {
            if (txtCreditLimit.Text == "0.00")
            {
                txtCreditLimit.Text = "";
            }
        }

        private void masterSalesmanLookUp_Click(object sender, EventArgs e)
        {
            frmLookUp.OpenLookupForm("10261", HandleDataMasterCode, this,false);
        }
        private void HandleDataMasterCode(Dictionary<string, object> selectedData)
        {
            string mSalesmanCode = selectedData["[Salesman Code]"].ToString();
            // Update form with the selected data
            txtMasterSalesman.Text = mSalesmanCode;


        }

  

      
    }
}