namespace SolidConvert_Pro
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.label1 = new System.Windows.Forms.Label();
            this.checkBoxPDF = new System.Windows.Forms.CheckBox();
            this.checkBoxDWG = new System.Windows.Forms.CheckBox();
            this.lblinfoSelected = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnAddfiles = new System.Windows.Forms.Button();
            this.btnClearList = new System.Windows.Forms.Button();
            this.btnRemoveSelected = new System.Windows.Forms.Button();
            this.lstSelectedFiles = new System.Windows.Forms.ListBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnSelectOutFolder = new System.Windows.Forms.Button();
            this.txtboxOutFolder = new System.Windows.Forms.TextBox();
            this.btnExport = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.lblUpdate = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtboxStatus = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // checkBoxPDF
            // 
            resources.ApplyResources(this.checkBoxPDF, "checkBoxPDF");
            this.checkBoxPDF.Name = "checkBoxPDF";
            this.checkBoxPDF.UseVisualStyleBackColor = true;
            // 
            // checkBoxDWG
            // 
            resources.ApplyResources(this.checkBoxDWG, "checkBoxDWG");
            this.checkBoxDWG.Name = "checkBoxDWG";
            this.checkBoxDWG.UseVisualStyleBackColor = true;
            // 
            // lblinfoSelected
            // 
            resources.ApplyResources(this.lblinfoSelected, "lblinfoSelected");
            this.lblinfoSelected.Name = "lblinfoSelected";
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.btnAddfiles);
            this.groupBox1.Controls.Add(this.btnClearList);
            this.groupBox1.Controls.Add(this.btnRemoveSelected);
            this.groupBox1.Controls.Add(this.lstSelectedFiles);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // btnAddfiles
            // 
            resources.ApplyResources(this.btnAddfiles, "btnAddfiles");
            this.btnAddfiles.Name = "btnAddfiles";
            this.btnAddfiles.UseVisualStyleBackColor = true;
            this.btnAddfiles.Click += new System.EventHandler(this.btnAddfiles_Click);
            // 
            // btnClearList
            // 
            resources.ApplyResources(this.btnClearList, "btnClearList");
            this.btnClearList.Name = "btnClearList";
            this.btnClearList.UseVisualStyleBackColor = true;
            this.btnClearList.Click += new System.EventHandler(this.btnClearList_Click);
            // 
            // btnRemoveSelected
            // 
            resources.ApplyResources(this.btnRemoveSelected, "btnRemoveSelected");
            this.btnRemoveSelected.Name = "btnRemoveSelected";
            this.btnRemoveSelected.UseVisualStyleBackColor = true;
            this.btnRemoveSelected.Click += new System.EventHandler(this.btnRemoveSelected_Click);
            // 
            // lstSelectedFiles
            // 
            this.lstSelectedFiles.FormattingEnabled = true;
            resources.ApplyResources(this.lstSelectedFiles, "lstSelectedFiles");
            this.lstSelectedFiles.Name = "lstSelectedFiles";
            this.lstSelectedFiles.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnSelectOutFolder);
            this.groupBox2.Controls.Add(this.txtboxOutFolder);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // btnSelectOutFolder
            // 
            resources.ApplyResources(this.btnSelectOutFolder, "btnSelectOutFolder");
            this.btnSelectOutFolder.Name = "btnSelectOutFolder";
            this.btnSelectOutFolder.UseVisualStyleBackColor = true;
            this.btnSelectOutFolder.Click += new System.EventHandler(this.btnSelectOutFolder_Click);
            // 
            // txtboxOutFolder
            // 
            resources.ApplyResources(this.txtboxOutFolder, "txtboxOutFolder");
            this.txtboxOutFolder.Name = "txtboxOutFolder";
            // 
            // btnExport
            // 
            resources.ApplyResources(this.btnExport, "btnExport");
            this.btnExport.ForeColor = System.Drawing.Color.Green;
            this.btnExport.Name = "btnExport";
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // lblStatus
            // 
            resources.ApplyResources(this.lblStatus, "lblStatus");
            this.lblStatus.Name = "lblStatus";
            // 
            // lblUpdate
            // 
            resources.ApplyResources(this.lblUpdate, "lblUpdate");
            this.lblUpdate.Name = "lblUpdate";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // txtboxStatus
            // 
            resources.ApplyResources(this.txtboxStatus, "txtboxStatus");
            this.txtboxStatus.Name = "txtboxStatus";
            this.txtboxStatus.ReadOnly = true;
            // 
            // Form1
            // 
            resources.ApplyResources(this, "$this");
            this.AutoValidate = System.Windows.Forms.AutoValidate.Disable;
            this.Controls.Add(this.txtboxStatus);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lblUpdate);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.btnExport);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.lblinfoSelected);
            this.Controls.Add(this.checkBoxDWG);
            this.Controls.Add(this.checkBoxPDF);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox checkBoxPDF;
        private System.Windows.Forms.CheckBox checkBoxDWG;
        private System.Windows.Forms.Label lblinfoSelected;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ListBox lstSelectedFiles;
        private System.Windows.Forms.Button btnClearList;
        private System.Windows.Forms.Button btnRemoveSelected;
        private System.Windows.Forms.Button btnAddfiles;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnSelectOutFolder;
        private System.Windows.Forms.TextBox txtboxOutFolder;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label lblUpdate;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtboxStatus;
    }
}

