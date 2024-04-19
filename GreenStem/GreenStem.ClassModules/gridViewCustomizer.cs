using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenStem.ClassModules
{
    public class gridViewCustomizer
    {
        public static void ApplyCustomizations(DevExpress.XtraGrid.Views.Grid.GridView gridView, bool subscribeEndSorting, bool autoWidth)
        {
            gridView.OptionsBehavior.Editable = false;
            gridView.CustomDrawCell += (sender, e) =>
            {
                // Set the background color for alternate rows
                if (e.RowHandle % 2 == 0)
                {
                    e.Appearance.BackColor = Color.Lavender;
                }
            };

            // Subscribe or unsubscribe based on the parameter
            if (subscribeEndSorting)
            {
                gridView.EndSorting += GridView_EndSorting;
            }
            else
            {
                gridView.EndSorting -= GridView_EndSorting;
            }
            if (autoWidth)
            {
                gridView.OptionsView.ColumnAutoWidth = true;
              
            }
            else
            {
                gridView.OptionsView.ColumnAutoWidth = false;
            }
        }

        private static void GridView_EndSorting(object sender, EventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView gridView = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            if (gridView != null)
            {
                // Set the focus row to the first row after sorting
                gridView.FocusedRowHandle = 0;
            }
        }
    }
}
