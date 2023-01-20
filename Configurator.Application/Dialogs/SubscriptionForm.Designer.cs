
namespace Configurator.Application.Dialogs
{
    partial class SubscriptionForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SubscriptionForm));
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxSerialNumber = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.dateTimePickerTo = new System.Windows.Forms.DateTimePicker();
            this.label4 = new System.Windows.Forms.Label();
            this.dateTimePickerFrom = new System.Windows.Forms.DateTimePicker();
            this.btnSave = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.textBoxUserName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.userNameVerify = new System.Windows.Forms.PictureBox();
            this.userNamePanel = new System.Windows.Forms.Panel();
            this.iCodeSerialNumberVerify = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.userNameVerify)).BeginInit();
            this.userNamePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.iCodeSerialNumberVerify)).BeginInit();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 34);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(115, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "iCode Serial Number:";
            // 
            // textBoxSerialNumber
            // 
            this.textBoxSerialNumber.Location = new System.Drawing.Point(136, 31);
            this.textBoxSerialNumber.Name = "textBoxSerialNumber";
            this.textBoxSerialNumber.Size = new System.Drawing.Size(270, 22);
            this.textBoxSerialNumber.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(14, 152);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(66, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "Expiry date:";
            // 
            // dateTimePickerTo
            // 
            this.dateTimePickerTo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerTo.Location = new System.Drawing.Point(100, 146);
            this.dateTimePickerTo.Name = "dateTimePickerTo";
            this.dateTimePickerTo.Size = new System.Drawing.Size(355, 22);
            this.dateTimePickerTo.TabIndex = 9;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(14, 107);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(80, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Effective from:";
            // 
            // dateTimePickerFrom
            // 
            this.dateTimePickerFrom.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerFrom.Location = new System.Drawing.Point(100, 102);
            this.dateTimePickerFrom.Name = "dateTimePickerFrom";
            this.dateTimePickerFrom.Size = new System.Drawing.Size(355, 22);
            this.dateTimePickerFrom.TabIndex = 7;
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnSave.Enabled = false;
            this.btnSave.Location = new System.Drawing.Point(277, 191);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 11;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button2.Location = new System.Drawing.Point(367, 191);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(88, 23);
            this.button2.TabIndex = 12;
            this.button2.Text = "Cancel";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // textBoxUserName
            // 
            this.textBoxUserName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxUserName.Location = new System.Drawing.Point(113, 4);
            this.textBoxUserName.Name = "textBoxUserName";
            this.textBoxUserName.Size = new System.Drawing.Size(287, 22);
            this.textBoxUserName.TabIndex = 14;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 13);
            this.label1.TabIndex = 13;
            this.label1.Text = "User Name :";
            // 
            // userNameVerify
            // 
            this.userNameVerify.Image = ((System.Drawing.Image)(resources.GetObject("userNameVerify.Image")));
            this.userNameVerify.Location = new System.Drawing.Point(405, 3);
            this.userNameVerify.Name = "userNameVerify";
            this.userNameVerify.Size = new System.Drawing.Size(36, 24);
            this.userNameVerify.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.userNameVerify.TabIndex = 15;
            this.userNameVerify.TabStop = false;
            this.userNameVerify.Click += new System.EventHandler(this.userNameVerify_Click);
            // 
            // userNamePanel
            // 
            this.userNamePanel.Controls.Add(this.userNameVerify);
            this.userNamePanel.Controls.Add(this.label1);
            this.userNamePanel.Controls.Add(this.textBoxUserName);
            this.userNamePanel.Location = new System.Drawing.Point(6, 62);
            this.userNamePanel.Name = "userNamePanel";
            this.userNamePanel.Size = new System.Drawing.Size(446, 31);
            this.userNamePanel.TabIndex = 16;
            // 
            // iCodeSerialNumberVerify
            // 
            this.iCodeSerialNumberVerify.Image = ((System.Drawing.Image)(resources.GetObject("iCodeSerialNumberVerify.Image")));
            this.iCodeSerialNumberVerify.InitialImage = ((System.Drawing.Image)(resources.GetObject("iCodeSerialNumberVerify.InitialImage")));
            this.iCodeSerialNumberVerify.Location = new System.Drawing.Point(411, 29);
            this.iCodeSerialNumberVerify.Name = "iCodeSerialNumberVerify";
            this.iCodeSerialNumberVerify.Size = new System.Drawing.Size(36, 24);
            this.iCodeSerialNumberVerify.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.iCodeSerialNumberVerify.TabIndex = 16;
            this.iCodeSerialNumberVerify.TabStop = false;
            this.iCodeSerialNumberVerify.Click += new System.EventHandler(this.iCodeSerialNumberVerify_Click);
            // 
            // SubscriptionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(464, 237);
            this.Controls.Add(this.iCodeSerialNumberVerify);
            this.Controls.Add(this.userNamePanel);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.dateTimePickerTo);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.dateTimePickerFrom);
            this.Controls.Add(this.textBoxSerialNumber);
            this.Controls.Add(this.label2);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SubscriptionForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Subscription";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormClosingEventHandler);
            this.Load += new System.EventHandler(this.SubscriptionDlg_Load);
            ((System.ComponentModel.ISupportInitialize)(this.userNameVerify)).EndInit();
            this.userNamePanel.ResumeLayout(false);
            this.userNamePanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.iCodeSerialNumberVerify)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxSerialNumber;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DateTimePicker dateTimePickerTo;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.DateTimePicker dateTimePickerFrom;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox textBoxUserName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox userNameVerify;
        private System.Windows.Forms.Panel userNamePanel;
        private System.Windows.Forms.PictureBox iCodeSerialNumberVerify;
    }
}