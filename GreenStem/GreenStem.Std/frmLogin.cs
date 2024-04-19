using DevExpress.XtraEditors;
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
using DevExpress.Utils;
using DevExpress.XtraWaitForm;
using DevExpress.XtraBars.Alerter;
using System.Runtime.InteropServices;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using System.Drawing.Text;

namespace GreenStem.Std
{



    public partial class frmLogin : Form
    {
     
        private const int AnimationDuration = 20; // Duration of animation in milliseconds
        private const int TimerInterval = 1; // Decrease the TimerInterval for a quicker animation
        private Timer animationTimer;
        private double opacityIncrement;
        public frmLogin()
        {
            InitializeComponent();
            txtUserName.Focus();
            InitializeAnimation();
            this.Load += frm_Load;
            this.lblCompanyName.Text = modPublicVariable.CompanyName;
           


        }
      
        
        private void InitializeAnimation()
        {
            // Set initial opacity to 0
            this.Opacity = 0;

            // Calculate the opacity increment per tick
            opacityIncrement = 1.0 / (AnimationDuration / TimerInterval);

            // Initialize the timer
            animationTimer = new Timer();
            animationTimer.Interval = TimerInterval;
            animationTimer.Tick += AnimationTimer_Tick;
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            // Increase the opacity gradually
            this.Opacity += opacityIncrement;

            // If the opacity reaches 1, stop the timer
            if (this.Opacity >= 1.0)
            {
                animationTimer.Stop();
            }
        }

        private void frm_Load(object sender, EventArgs e)
        {
            // Start the animation timer when the form is loaded
            animationTimer.Start();
        }
        private void Timer1_Tick_1(object sender, EventArgs e)
        {
            lblTime.Text = DateTime.Now.ToString("dd-MMM-yyyy    hh:mm:ss tt");
        }

     
        private void btnLogin_Click(object sender, EventArgs e)
        {
            bool isConcurrentUsingValid = LicenseControl.CheckConcurrentUsing(modPublicVariable.Company, modPublicVariable.CompanyName);
            if (isConcurrentUsingValid)
            {

             string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connection_string"].ConnectionString;
             string strSQL = "SELECT * FROM GUSER_TBL With(NoLock) WHERE GUSER_ID = @UserID";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(strSQL, connection);
                    command.Parameters.AddWithValue("@UserID", txtUserName.Text.Trim());

                    try
                    {
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();

                        if (reader.HasRows)
                        {
                            reader.Read();
                            string storedHash = reader["GUSER_PASS"].ToString(); // Assuming this is the SHA256 hash stored in the database

                            // Use the VerifyPassword method to compare the entered password with the stored hash
                            if (clsHash.VerifyPassword(txtPassword.Text.Trim().ToUpper(), storedHash))
                            {
                                LicenseControl.IncreaseConcurrentUsing(modPublicVariable.Company, modPublicVariable.CompanyName);
                                string userName = txtUserName.Text.Trim().ToUpper();
                                modPublicVariable.UserID = userName;
                                Timer1.Stop();
                                this.Hide();
                                mdiMainCopyCopy mainMenu = new mdiMainCopyCopy(userName);
                                mainMenu.ShowDialog();
                                this.Close();
                                this.Dispose();

                            }
                            else
                            {
                                MessageBox.Show("Incorrect User Password", "Login", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                txtPassword.Focus();
                            }
                        }
                        else
                        {
                            MessageBox.Show("Incorrect User ID", "Login", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            txtUserName.Focus();
                        }
                        reader.Close();
                    }

                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        ExceptionLogger.LogException(ex, nameof(frmLogin), "Std", "btnLogin_Click", new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
                    }
                }
            }
            else
            {
                // Fetch license control information from the database
                string licenseControlInfo = LicenseControl.GetLicenseControlInfo(modPublicVariable.Company, modPublicVariable.CompanyName);

                // Display message box with license control information
                MessageBox.Show($"Your license only allows {licenseControlInfo}. Please log out a user before logging in.");
            }
        }

        private void btnnCancel_Click(object sender, EventArgs e)
        {

        }

        private void txtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                // Call the cmdOK_Click_2 method
                btnLogin_Click(sender, e);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Hide();
            frmMultiCompanies frmMultiComp = new frmMultiCompanies();
            frmMultiComp.StartPosition = FormStartPosition.CenterScreen;
            frmMultiComp.ShowDialog();
            frmMultiComp.Show(); // Show the existing instance of frmMultiCompanies
            this.Close();
            this.Dispose();

        }

       
    }
}