
namespace Configurator.Application.Dialogs
{
    partial class DeviceTestForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DeviceTestForm));
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.textBoxSerialNumberRes = new System.Windows.Forms.TextBox();
            this.textBoxUuid = new System.Windows.Forms.TextBox();
            this.textBoxSerialNumber = new System.Windows.Forms.TextBox();
            this.textBoxUuidRes = new System.Windows.Forms.TextBox();
            this.textEraseFlash = new System.Windows.Forms.TextBox();
            this.textBoxLoadStm32 = new System.Windows.Forms.TextBox();
            this.buttonLoadUcb = new System.Windows.Forms.Button();
            this.targetMarketCombo = new System.Windows.Forms.ComboBox();
            this.targetMarketLabel = new System.Windows.Forms.Label();
            this.loadBootloaderBackgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButtonCreateApplication = new System.Windows.Forms.RadioButton();
            this.btniCodeOptionsOK = new System.Windows.Forms.Button();
            this.radioButtonApplication = new System.Windows.Forms.RadioButton();
            this.radioButtonBootloader = new System.Windows.Forms.RadioButton();
            this.loadApplicationBackgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.btnUsers = new System.Windows.Forms.Button();
            this.buttonLookupData = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(297, 123);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(500, 32);
            this.progressBar1.TabIndex = 71;
            this.progressBar1.Visible = false;
            // 
            // textBoxSerialNumberRes
            // 
            this.textBoxSerialNumberRes.Location = new System.Drawing.Point(498, 50);
            this.textBoxSerialNumberRes.Name = "textBoxSerialNumberRes";
            this.textBoxSerialNumberRes.Size = new System.Drawing.Size(298, 22);
            this.textBoxSerialNumberRes.TabIndex = 38;
            this.textBoxSerialNumberRes.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBoxUuid
            // 
            this.textBoxUuid.Location = new System.Drawing.Point(366, 88);
            this.textBoxUuid.Name = "textBoxUuid";
            this.textBoxUuid.Size = new System.Drawing.Size(126, 22);
            this.textBoxUuid.TabIndex = 39;
            this.textBoxUuid.Text = "iCode UUID";
            // 
            // textBoxSerialNumber
            // 
            this.textBoxSerialNumber.Location = new System.Drawing.Point(366, 50);
            this.textBoxSerialNumber.Name = "textBoxSerialNumber";
            this.textBoxSerialNumber.Size = new System.Drawing.Size(126, 22);
            this.textBoxSerialNumber.TabIndex = 69;
            this.textBoxSerialNumber.Text = "iCode Serial Number";
            // 
            // textBoxUuidRes
            // 
            this.textBoxUuidRes.Location = new System.Drawing.Point(498, 88);
            this.textBoxUuidRes.Name = "textBoxUuidRes";
            this.textBoxUuidRes.Size = new System.Drawing.Size(298, 22);
            this.textBoxUuidRes.TabIndex = 70;
            this.textBoxUuidRes.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textEraseFlash
            // 
            this.textEraseFlash.BackColor = System.Drawing.SystemColors.Window;
            this.textEraseFlash.Location = new System.Drawing.Point(361, 128);
            this.textEraseFlash.Name = "textEraseFlash";
            this.textEraseFlash.Size = new System.Drawing.Size(326, 22);
            this.textEraseFlash.TabIndex = 72;
            this.textEraseFlash.Text = "Erasing STM32 Flash....";
            this.textEraseFlash.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textEraseFlash.Visible = false;
            // 
            // textBoxLoadStm32
            // 
            this.textBoxLoadStm32.Location = new System.Drawing.Point(441, 128);
            this.textBoxLoadStm32.Name = "textBoxLoadStm32";
            this.textBoxLoadStm32.Size = new System.Drawing.Size(166, 22);
            this.textBoxLoadStm32.TabIndex = 73;
            this.textBoxLoadStm32.Text = "Loading Bootloader";
            this.textBoxLoadStm32.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBoxLoadStm32.Visible = false;
            // 
            // buttonLoadUcb
            // 
            this.buttonLoadUcb.Location = new System.Drawing.Point(667, 156);
            this.buttonLoadUcb.Name = "buttonLoadUcb";
            this.buttonLoadUcb.Size = new System.Drawing.Size(129, 16);
            this.buttonLoadUcb.TabIndex = 76;
            this.buttonLoadUcb.Text = "Load iCode UCB";
            this.buttonLoadUcb.UseVisualStyleBackColor = true;
            this.buttonLoadUcb.Visible = false;
            // 
            // targetMarketCombo
            // 
            this.targetMarketCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.targetMarketCombo.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.targetMarketCombo.FormattingEnabled = true;
            this.targetMarketCombo.Location = new System.Drawing.Point(498, 16);
            this.targetMarketCombo.Name = "targetMarketCombo";
            this.targetMarketCombo.Size = new System.Drawing.Size(151, 25);
            this.targetMarketCombo.TabIndex = 77;
            // 
            // targetMarketLabel
            // 
            this.targetMarketLabel.AutoSize = true;
            this.targetMarketLabel.Location = new System.Drawing.Point(363, 16);
            this.targetMarketLabel.Name = "targetMarketLabel";
            this.targetMarketLabel.Size = new System.Drawing.Size(81, 13);
            this.targetMarketLabel.TabIndex = 78;
            this.targetMarketLabel.Text = "Target Market:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioButtonCreateApplication);
            this.groupBox1.Controls.Add(this.btniCodeOptionsOK);
            this.groupBox1.Controls.Add(this.radioButtonApplication);
            this.groupBox1.Controls.Add(this.radioButtonBootloader);
            this.groupBox1.Location = new System.Drawing.Point(12, 16);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(268, 120);
            this.groupBox1.TabIndex = 79;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "iCode Options";
            // 
            // radioButtonCreateApplication
            // 
            this.radioButtonCreateApplication.AutoSize = true;
            this.radioButtonCreateApplication.Location = new System.Drawing.Point(17, 47);
            this.radioButtonCreateApplication.Name = "radioButtonCreateApplication";
            this.radioButtonCreateApplication.Size = new System.Drawing.Size(120, 17);
            this.radioButtonCreateApplication.TabIndex = 83;
            this.radioButtonCreateApplication.Text = "Create Application";
            this.radioButtonCreateApplication.UseVisualStyleBackColor = true;
            // 
            // btniCodeOptionsOK
            // 
            this.btniCodeOptionsOK.Location = new System.Drawing.Point(171, 83);
            this.btniCodeOptionsOK.Name = "btniCodeOptionsOK";
            this.btniCodeOptionsOK.Size = new System.Drawing.Size(81, 26);
            this.btniCodeOptionsOK.TabIndex = 82;
            this.btniCodeOptionsOK.Text = "GO";
            this.btniCodeOptionsOK.UseVisualStyleBackColor = true;
            this.btniCodeOptionsOK.Click += new System.EventHandler(this.btniCodeOptionsOK_Click);
            // 
            // radioButtonApplication
            // 
            this.radioButtonApplication.AutoSize = true;
            this.radioButtonApplication.Location = new System.Drawing.Point(17, 72);
            this.radioButtonApplication.Name = "radioButtonApplication";
            this.radioButtonApplication.Size = new System.Drawing.Size(112, 17);
            this.radioButtonApplication.TabIndex = 81;
            this.radioButtonApplication.Text = "Load Application";
            this.radioButtonApplication.UseVisualStyleBackColor = true;
            // 
            // radioButtonBootloader
            // 
            this.radioButtonBootloader.AutoSize = true;
            this.radioButtonBootloader.Checked = true;
            this.radioButtonBootloader.Location = new System.Drawing.Point(19, 21);
            this.radioButtonBootloader.Name = "radioButtonBootloader";
            this.radioButtonBootloader.Size = new System.Drawing.Size(110, 17);
            this.radioButtonBootloader.TabIndex = 80;
            this.radioButtonBootloader.TabStop = true;
            this.radioButtonBootloader.Text = "Load Bootloader";
            this.radioButtonBootloader.UseVisualStyleBackColor = true;
            // 
            // loadApplicationBackgroundWorker
            // 
            this.loadApplicationBackgroundWorker.WorkerReportsProgress = true;
            // 
            // btnUsers
            // 
            this.btnUsers.Location = new System.Drawing.Point(721, 15);
            this.btnUsers.Name = "btnUsers";
            this.btnUsers.Size = new System.Drawing.Size(75, 23);
            this.btnUsers.TabIndex = 81;
            this.btnUsers.Text = "Users";
            this.btnUsers.UseVisualStyleBackColor = true;
            this.btnUsers.Visible = false;
            this.btnUsers.Click += new System.EventHandler(this.btnUsers_Click);
            // 
            // buttonLookupData
            // 
            this.buttonLookupData.Location = new System.Drawing.Point(82, 162);
            this.buttonLookupData.Name = "buttonLookupData";
            this.buttonLookupData.Size = new System.Drawing.Size(155, 23);
            this.buttonLookupData.TabIndex = 82;
            this.buttonLookupData.Text = "Look up data";
            this.buttonLookupData.UseVisualStyleBackColor = true;
            this.buttonLookupData.Visible = false;
            this.buttonLookupData.Click += new System.EventHandler(this.button1_Click);
            // 
            // DeviceTestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(828, 197);
            this.Controls.Add(this.buttonLookupData);
            this.Controls.Add(this.btnUsers);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.targetMarketLabel);
            this.Controls.Add(this.targetMarketCombo);
            this.Controls.Add(this.buttonLoadUcb);
            this.Controls.Add(this.textBoxLoadStm32);
            this.Controls.Add(this.textEraseFlash);
            this.Controls.Add(this.textBoxUuidRes);
            this.Controls.Add(this.textBoxSerialNumber);
            this.Controls.Add(this.textBoxUuid);
            this.Controls.Add(this.textBoxSerialNumberRes);
            this.Controls.Add(this.progressBar1);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(790, 236);
            this.Name = "DeviceTestForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "iCode Configurator & Test";
            this.Load += new System.EventHandler(this.DeviceTestDlg_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.TextBox textBoxSerialNumberRes;
        private System.Windows.Forms.TextBox textBoxUuid;
        private System.Windows.Forms.TextBox textBoxSerialNumber;
        private System.Windows.Forms.TextBox textBoxUuidRes;
        private System.Windows.Forms.TextBox textEraseFlash;
        private System.Windows.Forms.TextBox textBoxLoadStm32;
        private System.Windows.Forms.Button buttonLoadUcb;
        private System.Windows.Forms.ComboBox targetMarketCombo;
        private System.Windows.Forms.Label targetMarketLabel;
        private System.ComponentModel.BackgroundWorker loadBootloaderBackgroundWorker;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btniCodeOptionsOK;
        private System.Windows.Forms.RadioButton radioButtonApplication;
        private System.Windows.Forms.RadioButton radioButtonBootloader;
        private System.ComponentModel.BackgroundWorker loadApplicationBackgroundWorker;
        private System.Windows.Forms.Button btnUsers;
        private System.Windows.Forms.RadioButton radioButtonCreateApplication;
        private System.Windows.Forms.Button buttonLookupData;
        //private System.Windows.Forms.OpenFileDialog comPortBinFileDialog;
    }
}

