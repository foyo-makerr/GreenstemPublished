using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GreenStem.Std
{
    public partial class frmListOfServers : Form
    {
        // Define a DataTable field to store the passed DataTable
        private DataTable _dataTable;
        // Define a public event to notify the selection of a server
        public event EventHandler<string> ServerSelected;

        public frmListOfServers(DataTable dataTable)
        {
            InitializeComponent();

            // Assign the passed DataTable to the local field
            _dataTable = dataTable;

            // Bind the DataTable to gridControl1
            gridControl1.DataSource = _dataTable;
            gridView1.OptionsBehavior.Editable = false;

            // Subscribe to the DoubleClick event of gridView1
            gridView1.DoubleClick += gridView1_DoubleClick;
        }

        // Method to raise the ServerSelected event
        protected virtual void OnServerSelected(string serverName)
        {
            ServerSelected?.Invoke(this, serverName);
        }

        // Add an event handler for the grid view's DoubleClick event
        private void gridView1_DoubleClick(object sender, EventArgs e)
        {
            // Check if a row is selected
            if (gridView1.FocusedRowHandle >= 0)
            {
                // Get the selected row
                DataRow row = gridView1.GetDataRow(gridView1.FocusedRowHandle);

                // Extract the server name from the selected row
                string serverName = row["ServerName"].ToString();

                // Raise the ServerSelected event with the selected server name
                OnServerSelected(serverName);
         
            }
        }
    }
}
