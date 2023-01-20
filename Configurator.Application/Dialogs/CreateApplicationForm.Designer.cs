namespace Configurator.Application.Dialogs
{
    partial class CreateApplicationForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CreateApplicationForm));
            this.comboBoxSelectOptions = new System.Windows.Forms.ComboBox();
            this.btnStart = new System.Windows.Forms.Button();
            this.rangeGroupBox = new System.Windows.Forms.GroupBox();
            this.textBoxTo = new System.Windows.Forms.TextBox();
            this.textBoxFrom = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.createAppBackgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.btnClose = new System.Windows.Forms.Button();
            this.progressLabel = new System.Windows.Forms.Label();
            this.rangeGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // comboBoxSelectOptions
            // 
            this.comboBoxSelectOptions.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxSelectOptions.FormattingEnabled = true;
            this.comboBoxSelectOptions.Items.AddRange(new object[] {
            "All",
            "Range"});
            this.comboBoxSelectOptions.Location = new System.Drawing.Point(12, 29);
            this.comboBoxSelectOptions.Name = "comboBoxSelectOptions";
            this.comboBoxSelectOptions.Size = new System.Drawing.Size(121, 21);
            this.comboBoxSelectOptions.TabIndex = 0;
            this.comboBoxSelectOptions.SelectedIndexChanged += new System.EventHandler(this.comboBoxSelectOptions_SelectedIndexChanged);
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(304, 27);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(71, 23);
            this.btnStart.TabIndex = 1;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.buttonCreateApp_Click);
            // 
            // rangeGroupBox
            // 
            this.rangeGroupBox.Controls.Add(this.textBoxTo);
            this.rangeGroupBox.Controls.Add(this.textBoxFrom);
            this.rangeGroupBox.Controls.Add(this.label2);
            this.rangeGroupBox.Controls.Add(this.label1);
            this.rangeGroupBox.Location = new System.Drawing.Point(12, 58);
            this.rangeGroupBox.Name = "rangeGroupBox";
            this.rangeGroupBox.Size = new System.Drawing.Size(363, 85);
            this.rangeGroupBox.TabIndex = 2;
            this.rangeGroupBox.TabStop = false;
            this.rangeGroupBox.Text = "Range";
            // 
            // textBoxTo
            // 
            this.textBoxTo.Location = new System.Drawing.Point(257, 37);
            this.textBoxTo.Name = "textBoxTo";
            this.textBoxTo.Size = new System.Drawing.Size(100, 22);
            this.textBoxTo.TabIndex = 5;
            // 
            // textBoxFrom
            // 
            this.textBoxFrom.Location = new System.Drawing.Point(127, 39);
            this.textBoxFrom.Name = "textBoxFrom";
            this.textBoxFrom.Size = new System.Drawing.Size(100, 22);
            this.textBoxFrom.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(231, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(21, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "to:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 42);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(114, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Serial Numbers from:";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(12, 178);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(284, 23);
            this.progressBar1.TabIndex = 3;
            this.progressBar1.Visible = false;
            // 
            // btnClose
            // 
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(304, 178);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 4;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            // 
            // progressLabel
            // 
            this.progressLabel.AutoSize = true;
            this.progressLabel.Location = new System.Drawing.Point(18, 155);
            this.progressLabel.Name = "progressLabel";
            this.progressLabel.Size = new System.Drawing.Size(0, 13);
            this.progressLabel.TabIndex = 5;
            // 
            // CreateApplicationDlg
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(392, 223);
            this.Controls.Add(this.progressLabel);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.rangeGroupBox);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.comboBoxSelectOptions);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "CreateApplicationDlg";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Create Application";
            this.Load += new System.EventHandler(this.CreateApplication_Load);
            this.rangeGroupBox.ResumeLayout(false);
            this.rangeGroupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBoxSelectOptions;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.GroupBox rangeGroupBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxTo;
        private System.Windows.Forms.TextBox textBoxFrom;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.ComponentModel.BackgroundWorker createAppBackgroundWorker;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label progressLabel;
    }
}