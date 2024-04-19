
using DevExpress.XtraBars.Navigation;
using DevExpress.XtraSplashScreen;
using DevExpress.XtraWaitForm;
using GreenStem.AR;
using GreenStem.Inventory;
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
using GreenStem.Security;
using System.IO;
using System.Runtime.InteropServices;
using GreenStem.Tool;
using GreenStem.Master;
using Greenstem.Std;
using GreenStem.LookUp;

namespace GreenStem.Std
{
    public partial class mdiMain : DevExpress.XtraBars.FluentDesignSystem.FluentDesignForm
    {
        private SqlConnection connectionString = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["connection_string"].ConnectionString);
        private globalKeyHandler keyHandler; // Declare GlobalKeyHandler instance
        private frm_AR_Delivery_Invoice_Counter deliveryInvoiceCounterForm;
        private frmTransactionDetailAnalysis transactionDetailAnalysisForm;
        private frmUserGroupAccess userGroupAccessForm;
        private frmUserGroup frmUserGroupForm;
        private frmLockRelease lockReleaseForm;
        private frmSalesman salesmanListForm;
        private frmGridDetailSetting gridDetailSettingForm;
        private frmLookUpSettings lookupSettingForm;
        private frmUserSetup userSetupForm;
        private frmSetupMdiMain setupMdiMainForm;
        public mdiMain(string userName)
        {
            InitializeComponent();
         
            // Subscribe to the KeyDown event of the form
            this.KeyPreview = true; // Set KeyPreview property to true
            //this.KeyDown += MainMenu_KeyDown;
            this.IsMdiContainer = true; // Set mdiMain as an MDI container
            keyHandler = new globalKeyHandler();
            keyHandler.KeyPressed += GlobalKeyHandler_KeyPressed;
            UserAccessControl(userName);
            // Register the GlobalKeyHandler with the application
            Application.AddMessageFilter(keyHandler);
            //future delete
            EnableAllMenuItems();
        }

          private void OpenOrBringToFrontForm<T>(ref T formInstance) where T : Form, new()
        {
            try
            {
                if (formInstance == null || formInstance.IsDisposed)
                {
                    // Form is not open or has been disposed, create a new instance
                    formInstance = new T();
                    formInstance.StartPosition = FormStartPosition.CenterScreen;
                    formInstance.Show();
                }
                else
                {
                    if (formInstance.WindowState == FormWindowState.Minimized)
                    {
                        // If the form is minimized, restore it to normal state
                        formInstance.WindowState = FormWindowState.Normal;
                    }
                    // Form is already open, bring it to the front
                    formInstance.BringToFront();
                }
        

            }
            catch (Exception ex)
            {
                // Log the exception with additional information
                ExceptionLogger.LogException(ex, nameof(mdiMain), "Std", "OpenOrBringToFrontForm", new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber());

            
              
            }
        }
        private void GlobalKeyHandler_KeyPressed(Keys keyData)
        {
            // Handle F5 key press
            if (keyData == Keys.F5)
            {
                OpenOrBringToFrontForm(ref deliveryInvoiceCounterForm);
            }
            // Handle F11 key press
            else if (keyData == Keys.F11)
            {
                OpenOrBringToFrontForm(ref transactionDetailAnalysisForm);
            }
        }
    
        private void MenuInvoiceCounter_Click(object senderr, EventArgs ee)
        {
            OpenOrBringToFrontForm(ref deliveryInvoiceCounterForm);
        }

        private void MenuAMSalesman_Click(object sender, EventArgs e)
        {
            OpenOrBringToFrontForm(ref salesmanListForm);
            
        }
        private void MenuAMUserGroupAccess_Click(object sender, EventArgs e)
        {
            OpenOrBringToFrontForm(ref userGroupAccessForm);
       
        }
        private void MenuAMUserGroup_Click(object sender, EventArgs e)
        {
            OpenOrBringToFrontForm(ref frmUserGroupForm);

        }

        private void MenuTLReleaseLocking_Click(object sender, EventArgs e)
        {
            OpenOrBringToFrontForm(ref lockReleaseForm);

        }
        private void MenuLogOut_Click(object sender, EventArgs e)
        {
            keyHandler.KeyPressed -= GlobalKeyHandler_KeyPressed;
            LicenseControl.DecreaseConcurrentUsing(modPublicVariable.Company, modPublicVariable.CompanyName);
            // Unload mdiMain
            this.Hide();
            // Close all existing forms or controls
            CloseAllOpenForms();

            frmLogin frmLogin = new frmLogin();
            frmLogin.StartPosition = FormStartPosition.CenterScreen;
            frmLogin.ShowDialog();
            this.Close();
            this.Dispose();


        }
      

    
        private void CloseAllOpenForms()
        {
            try
            {
                // Create a list to hold forms that need to be closed
                List<Form> formsToClose = new List<Form>();

                // Identify forms to be closed without modifying the OpenForms collection
                foreach (Form form in Application.OpenForms)
                {
                    if (form != this && form.GetType() != typeof(frmLogin) && form.GetType() != typeof(frmMultiCompanies))
                    {
                        formsToClose.Add(form);
                    }
                }

                // Close the identified forms
                foreach (Form form in formsToClose)
                {
                    form.Close();

                }
               
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex, nameof(mdiMain), "Std", "CloseAllOpenForms", new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
               
                // Handle the exception, log it, or show an error message
                MessageBox.Show("An error occurred while closing forms: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                
            }
        }

      


        private void UserAccessControl(string userName)
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
                         WHERE (GUSER_TBL.GUSER_ID = @UserName) AND (GXCS_TBL.GXCS_BLOCK = 1)
                         GROUP BY GUXCS_TBL.[Group ID], GXCS_TBL.GXCS_PRGID, GXCS_TBL.GXCS_GRDID,
                                  GXCS_TBL.GXCS_BLOCK, GXCS_TBL.GXCS_PRGID, GXCS_TBL.GXCS_MDLID,
                                  GPRG_TBL.GPRG_MENU, GPRG_TBL.GPRG_INDEX";

                // Create SqlCommand and set parameters
                SqlCommand command = new SqlCommand(query, connectionString);
                command.Parameters.AddWithValue("@UserName", userName);

                // Execute the query
                SqlDataReader reader = command.ExecuteReader();

                // Process the retrieved data
                while (reader.Read())
                {
                    string menuName = reader["GPRG_MENU"].ToString();
                    EnableMenuItem(menuName, MenuStrip1.Items);
                    string menuNameWith1 = menuName + "1";
                    EnableAccordionItem(menuNameWith1, accordionControl1.Elements);
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
        private void EnableAccordionItem(string menuName, AccordionControlElementCollection elements)
        {
            foreach (AccordionControlElement element in elements)
            {
                if (element is AccordionControlElement accordionElement)
                {
                    if (accordionElement.Name == menuName)
                    {
                        accordionElement.Enabled = true;
                        return;
                    }
                    else
                    {
                        EnableAccordionItem(menuName, accordionElement.Elements);
                    }
                }
            }
        }
        
        private void EnableMenuItem(string menuName, ToolStripItemCollection items)
        {
            foreach (ToolStripItem menuItem in items)
            {
                if (menuItem is ToolStripMenuItem menu)
                {
                    if (menu.Name == menuName)
                    {
                        menu.Enabled = true;
                        return;
                    }
                    else
                    {
                        EnableMenuItem(menuName, menu.DropDownItems);
                    }
                }
            }
        }

      
        private void EnableAllMenuItems()
        {
            foreach (ToolStripMenuItem menuItem in MenuStrip1.Items)
            {
                EnableMenuItemRecursive(menuItem);
            }
        }

        private void EnableMenuItemRecursive(ToolStripMenuItem menuItem)
        {
            menuItem.Enabled = true; // Enable the current menu item

            // Enable all sub-menu items recursively
            foreach (ToolStripItem subItem in menuItem.DropDownItems)
            {
                if (subItem is ToolStripMenuItem subMenuItem)
                {
                    EnableMenuItemRecursive(subMenuItem);
                }
            }
        }

        private void MenuAMGridDetailSetting_Click(object sender, EventArgs e)
        {
            OpenOrBringToFrontForm(ref gridDetailSettingForm);
         
        }

        private void MenuAMLookupSetting_Click(object sender, EventArgs e)
        {
            OpenOrBringToFrontForm(ref lookupSettingForm);
        }

        private void MenuAMUserSetup_Click(object sender, EventArgs e)
        {
            OpenOrBringToFrontForm(ref userSetupForm);
      
        }

        private void MenuSystemCustomize_Click(object sender, EventArgs e)
        {
            OpenOrBringToFrontForm(ref setupMdiMainForm);
        }

        private void tileItem90_ItemClick(object sender, DevExpress.XtraEditors.TileItemEventArgs e)
        {

        }

        private void MenuExit_Click(object sender, EventArgs e)
        {
            LicenseControl.DecreaseConcurrentUsing(modPublicVariable.Company, modPublicVariable.CompanyName);
        }
    }
}
