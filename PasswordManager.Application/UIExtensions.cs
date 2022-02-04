namespace PasswordManager.Application
{
    public static class UiExtensions
    {
        public static int GetSelectedRowId(this DataGridView dataGridView)
        {
            if (dataGridView.CurrentCell == null)
                return 0;

            DataGridViewRow selectedRow = dataGridView.CurrentCell.OwningRow;
            var idValue = dataGridView.Rows[selectedRow.Index].Cells[0].Value;
            return Convert.ToInt32(idValue);
        }

        public static void StretchLastColumn(this DataGridView dataGridView)
        {
            var lastColIndex = dataGridView.Columns.Count - 1;
            var lastCol = dataGridView.Columns[lastColIndex];
            lastCol.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }
    }
}
