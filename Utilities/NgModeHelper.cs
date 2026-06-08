using RawMat.Controllers;
using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace RawMat.Utilities
{
    public static class NgModeHelper
    {
        public const string ColumnName = "NG_MODE_ID";

        public static DataTable LoadNgModeList()
        {
            DataTable source = new SettingControllers().GetNgModeList() ?? new DataTable();
            DataTable list = new DataTable();
            list.Columns.Add("VALUE", typeof(string));
            list.Columns.Add("TEXT", typeof(string));
            list.Rows.Add("", "");

            if (source.Columns.Contains("VALUE") && source.Columns.Contains("TEXT"))
            {
                foreach (DataRow sourceRow in source.Rows)
                {
                    list.Rows.Add(
                        sourceRow["VALUE"] == DBNull.Value ? "" : sourceRow["VALUE"].ToString(),
                        sourceRow["TEXT"] == DBNull.Value ? "" : sourceRow["TEXT"].ToString());
                }
            }

            return list;
        }

        public static void ApplyComboBoxColumn(DataGridView grid, DataTable ngModes)
        {
            if (grid == null || !grid.Columns.Contains(ColumnName))
            {
                return;
            }

            int oldIndex = grid.Columns[ColumnName].Index;
            DataGridViewColumn oldColumn = grid.Columns[ColumnName];
            grid.Columns.Remove(oldColumn);

            DataGridViewComboBoxColumn comboColumn = new DataGridViewComboBoxColumn
            {
                Name = ColumnName,
                HeaderText = "อาการเสีย",
                DataPropertyName = ColumnName,
                DataSource = ngModes ?? LoadNgModeList(),
                DisplayMember = "TEXT",
                ValueMember = "VALUE",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton,
                FlatStyle = FlatStyle.Flat,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            };

            grid.Columns.Insert(oldIndex, comboColumn);
        }

        public static bool IsMissing(object value)
        {
            return value == null || value == DBNull.Value || string.IsNullOrWhiteSpace(value.ToString());
        }

        public static void EnsureValueExists(DataTable list, object value)
        {
            if (list == null || IsMissing(value))
            {
                return;
            }

            string id = value.ToString();
            bool exists = list.AsEnumerable().Any(row => row["VALUE"].ToString() == id);
            if (!exists)
            {
                list.Rows.Add(id, id);
            }
        }
    }
}
