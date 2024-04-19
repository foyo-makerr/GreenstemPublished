
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

namespace GreenStem.Menu
{
    public partial class mdiMain : DevExpress.XtraBars.FluentDesignSystem.FluentDesignForm
    {
        private SqlConnection connectionString = new SqlConnection(clsConnection.strConnection);

        public mdiMain(string userName)
        {
            InitializeComponent();
            UserAcessControl(userName);
            // Subscribe to the KeyDown event of the form
            this.KeyPreview = true; // Set KeyPreview property to true
            //this.KeyDown += MainMenu_KeyDown;

        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
           
                // Handle F5 key press
                if (keyData == Keys.F5)
                {
                    // Create an instance of frmDeliveryInvoiceCounter1
                frm_AR_Delivery_Invoice_Counter frmDeliveryInvoiceCounter = new frm_AR_Delivery_Invoice_Counter();  
                frmDeliveryInvoiceCounter.StartPosition = FormStartPosition.CenterScreen;
                frmDeliveryInvoiceCounter.GotFocus += (sender, e) => { this.Focus(); }; // Redirect focus to MainMenu
                frmDeliveryInvoiceCounter.Show();
                }
                // Handle F11 key press
                else if (keyData == Keys.F11)
                {
                    // Create an instance of frmTransactionDetailAnalysis1
                frmTransactionDetailAnalysis frmTransactionDetailAnalysis = new frmTransactionDetailAnalysis();
                frmTransactionDetailAnalysis.StartPosition = FormStartPosition.CenterScreen;
                frmTransactionDetailAnalysis.GotFocus += (sender, e) => { this.Focus(); };
                frmTransactionDetailAnalysis.Show();
                }
          

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void UserAcessControl(string userName)
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

        private void tileBarItem11_ItemClick(object sender, DevExpress.XtraEditors.TileItemEventArgs e)
        {

        }

        private void accordionControlElement9_Click(object sender, EventArgs e)
        {

        }

        private void accordionControlElement1_Click(object sender, EventArgs e)
        {

        }

        private void accordionControlElement2_Click(object sender, EventArgs e)
        {

        }

        private void tileItem8_ItemClick(object sender, DevExpress.XtraEditors.TileItemEventArgs e)
        {

        }

        private void Menu3_Load(object sender, EventArgs e)
        {

        }

        private void MenuAMUserGroupAccess_Click(object sender, EventArgs e)
        {
            frmUserGroupAccess frmUserGroupAccess = new frmUserGroupAccess();
            frmUserGroupAccess.StartPosition = FormStartPosition.CenterScreen;

            frmUserGroupAccess.ShowDialog();    
        }

        private void MenuLogOut_Click(object sender, EventArgs e)
        {
            // Unload mdiMain
            this.Hide();

        
     
        }
        private void closeForm()
        {
            SplashScreenManager.CloseForm(false);
        }

        private void MenuAMUserGroup_Click(object sender, EventArgs e)
        {
            frmUserGroup frmUserGroup= new frmUserGroup();
            frmUserGroup.StartPosition = FormStartPosition.CenterScreen;

            frmUserGroup.ShowDialog();
        }
    }
}
