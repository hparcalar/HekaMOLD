
namespace HekaMOLD.DataImporter
{
    partial class frmMain
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
            this.txtFileName = new System.Windows.Forms.TextBox();
            this.btnSelectFile = new System.Windows.Forms.Button();
            this.ofDialog = new System.Windows.Forms.OpenFileDialog();
            this.lblSuccess = new System.Windows.Forms.Label();
            this.lblError = new System.Windows.Forms.Label();
            this.lblErrMsg = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // txtFileName
            // 
            this.txtFileName.Location = new System.Drawing.Point(12, 18);
            this.txtFileName.Name = "txtFileName";
            this.txtFileName.Size = new System.Drawing.Size(158, 22);
            this.txtFileName.TabIndex = 0;
            // 
            // btnSelectFile
            // 
            this.btnSelectFile.Location = new System.Drawing.Point(176, 12);
            this.btnSelectFile.Name = "btnSelectFile";
            this.btnSelectFile.Size = new System.Drawing.Size(117, 34);
            this.btnSelectFile.TabIndex = 1;
            this.btnSelectFile.Text = "Dosya Seç";
            this.btnSelectFile.UseVisualStyleBackColor = true;
            this.btnSelectFile.Click += new System.EventHandler(this.btnSelectFile_Click);
            // 
            // ofDialog
            // 
            this.ofDialog.Filter = "*.xls|*.xlsx";
            // 
            // lblSuccess
            // 
            this.lblSuccess.AutoSize = true;
            this.lblSuccess.Location = new System.Drawing.Point(12, 72);
            this.lblSuccess.Name = "lblSuccess";
            this.lblSuccess.Size = new System.Drawing.Size(62, 17);
            this.lblSuccess.TabIndex = 2;
            this.lblSuccess.Text = "Başarılı: ";
            // 
            // lblError
            // 
            this.lblError.AutoSize = true;
            this.lblError.Location = new System.Drawing.Point(12, 100);
            this.lblError.Name = "lblError";
            this.lblError.Size = new System.Drawing.Size(73, 17);
            this.lblError.TabIndex = 3;
            this.lblError.Text = "Başarısız: ";
            // 
            // lblErrMsg
            // 
            this.lblErrMsg.AutoSize = true;
            this.lblErrMsg.Location = new System.Drawing.Point(12, 138);
            this.lblErrMsg.Name = "lblErrMsg";
            this.lblErrMsg.Size = new System.Drawing.Size(0, 17);
            this.lblErrMsg.TabIndex = 4;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(519, 322);
            this.Controls.Add(this.lblErrMsg);
            this.Controls.Add(this.lblError);
            this.Controls.Add(this.lblSuccess);
            this.Controls.Add(this.btnSelectFile);
            this.Controls.Add(this.txtFileName);
            this.Name = "frmMain";
            this.Text = "Heka MOLD - Data Importer";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtFileName;
        private System.Windows.Forms.Button btnSelectFile;
        private System.Windows.Forms.OpenFileDialog ofDialog;
        private System.Windows.Forms.Label lblSuccess;
        private System.Windows.Forms.Label lblError;
        private System.Windows.Forms.Label lblErrMsg;
    }
}

