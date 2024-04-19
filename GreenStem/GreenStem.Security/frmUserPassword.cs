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
    public partial class frmUserPassword : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        private string connectionString  = System.Configuration.ConfigurationManager.ConnectionStrings["connection_string"].ConnectionString;
        private string userID;
        public frmUserPassword(string userID)
        {
            InitializeComponent();
            this.userID = userID;   
           

        }

        private void btnItemSave_ItemClick(object sender, ItemClickEventArgs e)
        {
            // Perform validation
            if (ValidateInput())
            {
                try
                {
                    // Update the password in the database
                    UpdatePassword(clsHash.HashPassword(txtNewPassword.Text.Trim().ToUpper()));

                    // Display success message
                    MessageBox.Show("Password updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Close the form
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        // Method to perform input validation
        private bool ValidateInput()
        {
            // Existing validation checks
            if (string.IsNullOrWhiteSpace(txtCurrentPassword.Text) ||
                string.IsNullOrWhiteSpace(txtNewPassword.Text) ||
                string.IsNullOrWhiteSpace(txtReenterPassword.Text))
            {
                MessageBox.Show("Please fill in all fields.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (txtNewPassword.Text != txtReenterPassword.Text)
            {
                MessageBox.Show("New password and re-entered password do not match.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            // Retrieve the current stored hash from the database
            string storedHash = GetCurrentStoredHashForUser(userID); // Implement this method to fetch the hash

            // Check if the current password is correct
            if (!clsHash.VerifyPassword(txtCurrentPassword.Text.Trim().ToUpper(), storedHash))
            {
                MessageBox.Show("The current password is incorrect.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            // Check if the new password is different from the current password
            if (clsHash.VerifyPassword(txtNewPassword.Text.Trim().ToUpper(), storedHash))
            {
                MessageBox.Show("The new password must be different from the current password.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            // Add more validation rules as needed

            return true;
        }
        // Method to retrieve the current stored hash for the user
        private string GetCurrentStoredHashForUser(string userID)
        {
            string storedHash = null;
           
            string strSQL = "SELECT GUSER_PASS FROM GUSER_TBL WHERE GUSER_ID = @UserID";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(strSQL, connection);
                command.Parameters.AddWithValue("@UserID", userID);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        storedHash = reader["GUSER_PASS"].ToString();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred while retrieving the password: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            return storedHash;
        }
        // Method to update password in the database
        private void UpdatePassword(string hashedPassword)
        {
            string strSQL = "UPDATE GUSER_TBL " +modPublicVariable.UpdateLockType+" SET GUSER_PASS = @Password WHERE GUSER_ID = @UserID";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(strSQL, connection);
                command.Parameters.AddWithValue("@Password", hashedPassword);
                command.Parameters.AddWithValue("@UserID", userID);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

    }
}