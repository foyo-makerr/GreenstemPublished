using DevExpress.XtraBars;
using GreenStem.ClassModules;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GreenStem.Tool
{
    public partial class frmLockRelease : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public frmLockRelease()
        {
            InitializeComponent();
        }


        private void btnItemDelete_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (rbComputer.Checked)
            {
                if (LogGuserLocked.DeleteLogsForComputer(txtLockName.Text))
                {
                    MessageBox.Show("Logs deleted successfully for the specified computer.", "Deletion Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("No logs found for the specified computer name.", "Deletion Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else if (rbUser.Checked)
            {
                if (LogGuserLocked.DeleteLogsForUser(txtLockName.Text))
                {
                    MessageBox.Show("Logs deleted successfully for the specified user.", "Deletion Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("No logs found for the specified user name.", "Deletion Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                LogGuserLocked.DeleteAllLogs();
                
                MessageBox.Show("All logs deleted successfully.", "Deletion Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
               
            }
        }

        private void rbUser_CheckedChanged(object sender, EventArgs e)
        {
            lblSelection.Visible = true;
            txtLockName.Visible = true;
            lblSelection.Text = "User Name:";
            
        }

        private void rbAll_CheckedChanged(object sender, EventArgs e)
        {
            lblSelection.Visible = false;
            txtLockName.Visible = false;
        }

        private void rbComputer_CheckedChanged(object sender, EventArgs e)
        {
            lblSelection.Visible = true;
            txtLockName.Visible = true;
            lblSelection.Text = "Computer Name:";
        }
    }
}