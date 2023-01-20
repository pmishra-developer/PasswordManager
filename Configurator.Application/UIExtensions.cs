namespace Configurator.Application
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

        public static void TextBoxVisible(this TextBox textBox, bool visibility)
        {
            textBox.Invoke((MethodInvoker)delegate {
                // Running on the UI thread
                textBox.Visible = visibility;
            });
        }

        public static void ProgressBarVisible(this ProgressBar progressBar, bool visibility = true, bool enable = true)
        {
            progressBar.Invoke((MethodInvoker)delegate {
                // Running on the UI thread
                progressBar.Visible = visibility;
            });
        }

        public static void SetButtonVisible(this Button button, bool visibility = true)
        {
            button.Invoke((MethodInvoker)delegate {
                // Running on the UI thread
                button.Visible = visibility;
            });
        }

        public static void SetButtonEnable(this Button button, bool enabled = true)
        {
            button.Invoke((MethodInvoker)delegate {
                // Running on the UI thread
                button.Enabled = enabled;
            });
        }

        public static void SetMaximumValue(this ProgressBar progressBar, int value)
        {
            progressBar.Invoke((MethodInvoker)delegate {
                // Running on the UI thread
                progressBar.Maximum = value;
            });
        }

        public static void SetValue(this ProgressBar progressBar, int value)
        {
            progressBar.Invoke((MethodInvoker)delegate {
                // Running on the UI thread
                progressBar.Value = value;
            });
        }

        public static void WriteTextOnTextBox(this TextBox textBox, string txtToWrite)
        {
            textBox.Invoke((MethodInvoker)delegate {
                // Running on the UI thread
                textBox.Text = txtToWrite;
            });
        }

        public static void WriteTextOnLabel(this Label label, string txtToWrite)
        {
            label.Invoke((MethodInvoker)delegate {
                // Running on the UI thread
                label.Text = txtToWrite;
            });
        }

        public static void SetTextBoxColor(this TextBox textBox, Color clrToSet)
        {
            textBox.Invoke((MethodInvoker)delegate {
                // Running on the UI thread
                textBox.BackColor = clrToSet;
            });
        }
    }
}
