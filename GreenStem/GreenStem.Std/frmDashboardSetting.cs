using DevExpress.XtraBars;
using DevExpress.XtraEditors.Controls;
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

namespace GreenStem.Tool
{
    public partial class frmDashboardSetting : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        private string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connection_string"].ConnectionString;
        private Dictionary<string, string> menuIdMap = new Dictionary<string, string>();
        public frmDashboardSetting()
        {
            InitializeComponent();
            FillModuleComboBox();
            LoadMenuSetting();
        }

        private void btnItemSave_ItemClick(object sender, ItemClickEventArgs e)
        {
            // Get the selected menu item from the ComboBox
            string selectedMenuCaption = cbModule.SelectedItem?.ToString();
            if (!string.IsNullOrEmpty(selectedMenuCaption))
            {
                // Retrieve the corresponding menuId1 from the dictionary
                if (menuIdMap.TryGetValue(selectedMenuCaption, out string menuId1))
                {
                    // Get the display mode (Menu or Dashboard)
                    string displayMode = rbMenu.Checked ? "Menu" : "Dashboard";

                    try
                    {
                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {
                            connection.Open();

                            // Begin the transaction with the desired isolation level
                            SqlTransaction transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted);

                            try
                            {
                                string checkQuery = "SELECT COUNT(*) FROM [Menu_Setting] WHERE GUSER_ID = @UserId";
                                SqlCommand checkCommand = new SqlCommand(checkQuery, connection, transaction); // Associate the command with the transaction
                                checkCommand.Parameters.AddWithValue("@UserId", modPublicVariable.UserID);

                                int userCount = (int)checkCommand.ExecuteScalar();

                                string query;
                                SqlCommand command;

                                if (userCount > 0)
                                {
                                    // User exists, perform update
                                    query = "UPDATE [Menu_Setting] SET [DisplayMode] = @DisplayMode, [DisplayModule] = @DisplayModule WHERE GUSER_ID = @UserId";
                                    command = new SqlCommand(query, connection, transaction); // Associate the command with the transaction
                                }
                                else
                                {
                                    // User does not exist, perform insert
                                    query = "INSERT INTO [Menu_Setting] ([GUSER_ID], [DisplayMode], [DisplayModule]) VALUES (@UserId, @DisplayMode, @DisplayModule)";
                                    command = new SqlCommand(query, connection, transaction); // Associate the command with the transaction
                                }

                                command.Parameters.AddWithValue("@DisplayMode", displayMode);
                                command.Parameters.AddWithValue("@DisplayModule", menuId1); // Use retrieved menuId1 value
                                command.Parameters.AddWithValue("@UserId", modPublicVariable.UserID);
                                int rowsAffected = command.ExecuteNonQuery();

                                // Commit the transaction if the operation is successful
                                transaction.Commit();

                                if (rowsAffected > 0)
                                {
                                    MessageBox.Show("Menu setting saved successfully.");
                                    // Optionally, perform any additional actions after successful save
                                }
                                else
                                {
                                    MessageBox.Show("Failed to save menu setting.");
                                }
                            }
                            catch (Exception ex)
                            {
                                // Rollback the transaction in case of an exception
                                transaction.Rollback();
                                MessageBox.Show("Error: " + ex.Message);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message);
                    }
                }
                else
                {
                    MessageBox.Show("Failed to retrieve Menu Id for the selected module.");
                }
            }
            else
            {
                MessageBox.Show("Please select a module from the dropdown.");
            }
        }

        private void LoadMenuSetting()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT [DisplayMode], [DisplayModule] FROM [Menu_Setting] WHERE [GUSER_ID] = @UserId";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@UserId", modPublicVariable.UserID);
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        string displayMode = reader["DisplayMode"].ToString();
                        string displayModule = reader["DisplayModule"].ToString();

                        // Set the radio button based on the DisplayMode
                        if (displayMode == "Menu")
                            rbMenu.Checked = true;
                        else if (displayMode == "Dashboard")
                            rbDashboard.Checked = true;

                        // Select the item in the ComboBox based on the DisplayModule
                        cbModule.SelectedItem = GetMenuCaptionByModule(displayModule);
                    }

                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private string GetMenuCaptionByModule(string module)
        {
            foreach (string item in cbModule.Items)
            {
                if (menuIdMap.TryGetValue(item, out string menuId1) && menuId1 == module)
                    return item;
            }
            return null;
        }
        private void FillModuleComboBox()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT [Menu Caption], [menu id1] FROM MENU_TBL WHERE Right([Menu Id], 3) = '000' ORDER BY Seqno";
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();

                    // Clear existing items in the ComboBox and the menuIdMap
                    cbModule.Items.Clear();
                    menuIdMap.Clear();

                    while (reader.Read())
                    {
                        string menuCaption = reader["Menu Caption"].ToString();
                        string menuId1 = reader["menu id1"].ToString();

                        // Add menuCaption to ComboBox display
                        cbModule.Items.Add(menuCaption);

                        // Store the mapping between menuCaption and menuId1 in the dictionary
                        menuIdMap[menuCaption] = menuId1;
                    }

                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
    }
}