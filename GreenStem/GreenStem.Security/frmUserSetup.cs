using DevExpress.XtraBars;
using DevExpress.XtraGrid;
using GreenStem.ClassModules;
using GreenStem.LookUp;
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

namespace GreenStem.Security
{
    public partial class frmUserSetup : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        private string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connection_string"].ConnectionString;
        public frmUserSetup()
        {
            InitializeComponent();

            FillGroupComboBox(); // Call the method to fill the ComboBox when the form is initialized
            cboGroup.SelectedIndex = -1; // Deselect any selected item
        }


        private void pbGroupAccessLookUp_Click(object sender, EventArgs e)
        {
            frmLookUp.OpenLookupForm("10005", HandleDataSelectedEvent, this, true);
        }

        private void HandleDataSelectedEvent(Dictionary<string, object> selectedData)
        {
            txtPassword.Visible = false;
            string userId = selectedData["[GUSER_ID]"].ToString();
            txtUserId.Text = userId;

            string strSQL = "SELECT * FROM GUSER_TBL WHERE GUSER_ID = @UserID";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(strSQL, connection);
                command.Parameters.AddWithValue("@UserID", userId);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read()) // Assuming GUSER_ID is unique and there's only one record
                    {
                        // Assign the values from the reader to the text boxes
                        txtUserName.Text = reader["GUSER_NAME"].ToString();
                    }
                    else
                    {
                        MessageBox.Show("User not found.", "Lookup", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            string groupID = RetrieveGroup(userId);

            // Find the item in the ComboBox's items list that matches the groupID
            foreach (DataRowView item in cboGroup.Items)
            {
                if (item["GGRP_ID"].ToString() == groupID)
                {
                    cboGroup.SelectedItem = item;
                    break;
                }
            }


        }

        private string RetrieveGroup(string userId)
        {
            string groupID = ""; // Default value

            string strSQL = "SELECT [Group ID] FROM GUXCS_TBL WHERE [User ID] = @UserID";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(strSQL, connection);
                command.Parameters.AddWithValue("@UserID", userId);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    // Check if any rows are returned
                    if (reader.HasRows)
                    {
                        // Loop through the rows
                        while (reader.Read())
                        {
                            // Retrieve the value of the "Group ID" column
                            groupID = reader["Group ID"].ToString();
                            break; // Exit loop after first row
                        }
                    }
                    else
                    {
                        MessageBox.Show("No group found for the user ID: " + userId, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                    // Close the reader
                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                }
            }

            return groupID; // Return the retrieved group ID
        }
        private void FillGroupComboBox()
        {
            string strSQL = "SELECT [GGRP_ID] FROM [GGRP_TBL]";
            DataTable dtGroups = new DataTable();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(strSQL, connection);

                try
                {
                    connection.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.Fill(dtGroups);

                    if (dtGroups.Rows.Count > 0)
                    {
                        cboGroup.DataSource = dtGroups;
                        cboGroup.DisplayMember = "GGRP_ID"; // Update display member to match the actual column name
                        cboGroup.ValueMember = "GGRP_ID";


                    }
                    else
                    {
                        MessageBox.Show("No groups found.", "Group Lookup", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void btnChangePassword_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (string.IsNullOrEmpty(txtUserId.Text))
            {
                frmLookUp.OpenLookupForm("10005", HandleDataPasswordEvent, this, true);
            }
            else
            {
                string userId = txtUserId.Text;

                // Check if the user ID exists in the GUSER_TBL
                string strSQL = "SELECT [GUSER_ID] FROM GUSER_TBL WHERE GUSER_ID = @UserID";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(strSQL, connection);
                    command.Parameters.AddWithValue("@UserID", userId);

                    try
                    {
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();

                        if (reader.Read()) // User ID found
                        {
                            // Open the frmUserPassword form with the retrieved GUSER_ID
                            string retrievedUserId = reader["GUSER_ID"].ToString();
                            reader.Close();
                            frmUserPassword newPasswordForm = new frmUserPassword(retrievedUserId);
                            newPasswordForm.StartPosition = FormStartPosition.CenterScreen;
                            newPasswordForm.Show(); // Show the form as a dialog
                        }
                        else
                        {
                            // User ID not found, prompt the user to select a valid user ID
                            frmLookUp.OpenLookupForm("10005", HandleDataPasswordEvent, this, true);
                        }

                        // Close the reader
                        reader.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnItemSave_ItemClick(object sender, ItemClickEventArgs e)
        {
            string userId = txtUserId.Text;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
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

                            // Check if the user exists
                            command.CommandText = "SELECT COUNT(*) FROM GUSER_TBL WHERE GUSER_ID = @UserID";
                            command.Parameters.AddWithValue("@UserID", userId);
                            int userCount = (int)command.ExecuteScalar();

                            // Clear the parameters before reusing the command object
                            command.Parameters.Clear();

                            if (userCount > 0)
                            {
                                // Update user name in GUSER_TBL
                                command.CommandText = "UPDATE GUSER_TBL SET [GUSER_NAME] = @UserName WHERE GUSER_ID = @UserID";
                                command.Parameters.AddWithValue("@UserID", userId);
                                command.Parameters.AddWithValue("@UserName", txtUserName.Text);
                                command.ExecuteNonQuery();

                                // Clear the parameters before reusing the command object
                                command.Parameters.Clear();
                                string selectedItem = cboGroup.SelectedValue.ToString();
                                // Update group ID in GUXCS_TBL
                                command.CommandText = "UPDATE GUXCS_TBL SET [Group ID] = @GroupID WHERE [User ID] = @UserID";
                                command.Parameters.AddWithValue("@UserID", userId);
                                command.Parameters.AddWithValue("@GroupID", cboGroup.SelectedValue.ToString());
                                command.ExecuteNonQuery();
                            }
                            else
                            {
                                // Insert new record
                                command.CommandText = "INSERT INTO GUSER_TBL ([GUSER_ID], [GUSER_NAME]) VALUES (@UserID, @UserName)";
                                command.Parameters.AddWithValue("@UserID", userId);
                                command.Parameters.AddWithValue("@UserName", txtUserName.Text);
                                command.ExecuteNonQuery();

                                // Clear the parameters before reusing the command object
                                command.Parameters.Clear();

                                command.CommandText = "INSERT INTO GUXCS_TBL ([User ID], [Group ID]) VALUES (@UserID, @GroupID)";
                                command.Parameters.AddWithValue("@UserID", userId);
                                command.Parameters.AddWithValue("@GroupID", cboGroup.SelectedValue.ToString());
                                command.ExecuteNonQuery();
                            }

                            // Commit the transaction if all operations succeed
                            transaction.Commit();

                            MessageBox.Show("Changes saved successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    catch (Exception ex)
                    {
                        // Rollback the transaction if any operation fails
                        transaction.Rollback();
                        MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void HandleDataPasswordEvent(Dictionary<string, object> selectedData)
        {
            string userId = selectedData["[GUSER_ID]"].ToString();
            frmUserPassword newPasswordForm = new frmUserPassword(userId);
            newPasswordForm.StartPosition = FormStartPosition.CenterScreen;
            newPasswordForm.Show(); // Show the form as a dialog


        }

        private void btnItemNew_ItemClick(object sender, ItemClickEventArgs e)
        {
            clearScreen();
        }
        private void clearScreen()
        {
            {
                // Clear the content of cboGroup
                cboGroup.SelectedIndex = -1; // Deselect any selected item

                // Clear the content of txtUserName
                txtUserName.Text = "";

                // Clear the content of txtUserId
                txtUserId.Text = "";
                txtPassword.Visible = false;    
            }
        }
    }
    
}