using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace PasswordManager
{
    public class DataGridControl
    {
        #region DataGrid

        public static DataGridView? ResultDataGridView;

        private static readonly DataGridViewCellStyle DataGridViewCellStyle1 = new DataGridViewCellStyle();
        private static readonly DataGridViewCellStyle DataGridViewCellStyle2 = new DataGridViewCellStyle();

        #endregion

        public static DataGridView Get()
        {
            ResultDataGridView = new DataGridView();
            ((ISupportInitialize)(ResultDataGridView)).BeginInit();

            ResultDataGridView.BackgroundColor = SystemColors.ActiveCaption;
            ResultDataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            ResultDataGridView.Dock = DockStyle.Fill;
            ResultDataGridView.Location = new Point(0, 0);
            ResultDataGridView.Name = "ResultDataGridView";
            ResultDataGridView.Size = new Size(672, 238);
            ResultDataGridView.TabIndex = 0;

            ResultDataGridView.AllowUserToAddRows = false;
            ResultDataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            ResultDataGridView.RowHeadersVisible = false;
            ResultDataGridView.AllowUserToDeleteRows = false;
            ResultDataGridView.AllowUserToResizeRows = false;
            ResultDataGridView.AllowUserToOrderColumns = true;

            ResultDataGridView.AlternatingRowsDefaultCellStyle = DataGridViewCellStyle1;
            ResultDataGridView.CellBorderStyle = DataGridViewCellBorderStyle.None;
            ResultDataGridView.EditMode = DataGridViewEditMode.EditProgrammatically;

            ResultDataGridView.BackgroundColor = Color.White;

            DataGridViewCellStyle1.BackColor = Color.FromArgb(230, 240, 250);
            DataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
            DataGridViewCellStyle2.BackColor = SystemColors.Window;
            DataGridViewCellStyle2.Font = new Font("Tahoma", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);

            DataGridViewCellStyle2.ForeColor = SystemColors.ControlText;
            DataGridViewCellStyle2.SelectionBackColor = SystemColors.Highlight;
            DataGridViewCellStyle2.SelectionForeColor = SystemColors.HighlightText;
            DataGridViewCellStyle2.WrapMode = DataGridViewTriState.False;

            return ResultDataGridView;
        }
    }
}