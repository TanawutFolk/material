using System;
using System.Drawing;
using System.Windows.Forms;

namespace RawMat.Views.Setting
{
    internal static class SettingGridActionMenu
    {
        public static void Show(DataGridView grid, int columnIndex, int rowIndex, Action editAction, Action deleteAction)
        {
            if (grid == null || rowIndex < 0 || columnIndex < 0)
            {
                return;
            }

            var menu = new ContextMenuStrip();
            menu.Items.Add("Edit", null, (sender, args) => editAction?.Invoke());
            menu.Items.Add("Delete", null, (sender, args) => deleteAction?.Invoke());
            menu.Closed += (sender, args) =>
            {
                if (!grid.IsDisposed && grid.IsHandleCreated)
                {
                    grid.BeginInvoke(new Action(menu.Dispose));
                }
            };

            Rectangle cellBounds = grid.GetCellDisplayRectangle(columnIndex, rowIndex, true);
            menu.Show(grid, new Point(cellBounds.Left, cellBounds.Bottom));
        }
    }
}
