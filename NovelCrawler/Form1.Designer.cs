namespace NovelCrawler
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            txtUrl = new TextBox();
            txtSavePath = new TextBox();
            btnSelectFolder = new Button();
            progressBar = new ProgressBar();
            btnStart = new Button();
            rtbLog = new RichTextBox();
            SuspendLayout();
            // 
            // txtUrl
            // 
            txtUrl.Location = new Point(37, 22);
            txtUrl.Name = "txtUrl";
            txtUrl.Size = new Size(327, 27);
            txtUrl.TabIndex = 0;
            // 
            // txtSavePath
            // 
            txtSavePath.Location = new Point(35, 102);
            txtSavePath.Name = "txtSavePath";
            txtSavePath.Size = new Size(329, 27);
            txtSavePath.TabIndex = 1;
            // 
            // btnSelectFolder
            // 
            btnSelectFolder.Location = new Point(370, 102);
            btnSelectFolder.Name = "btnSelectFolder";
            btnSelectFolder.Size = new Size(94, 29);
            btnSelectFolder.TabIndex = 2;
            btnSelectFolder.Text = "Browse";
            btnSelectFolder.UseVisualStyleBackColor = true;
            btnSelectFolder.Click += btnSelectFolder_Click;
            // 
            // progressBar
            // 
            progressBar.Location = new Point(33, 165);
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(723, 29);
            progressBar.TabIndex = 3;
            // 
            // btnStart
            // 
            btnStart.BackColor = SystemColors.ActiveCaption;
            btnStart.Location = new Point(585, 22);
            btnStart.Name = "btnStart";
            btnStart.Size = new Size(171, 98);
            btnStart.TabIndex = 5;
            btnStart.Text = "Start Crawl";
            btnStart.UseVisualStyleBackColor = false;
            btnStart.Click += btnStart_Click;
            // 
            // rtbLog
            // 
            rtbLog.Location = new Point(32, 227);
            rtbLog.Name = "rtbLog";
            rtbLog.Size = new Size(394, 195);
            rtbLog.TabIndex = 6;
            rtbLog.Text = "";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(rtbLog);
            Controls.Add(btnStart);
            Controls.Add(progressBar);
            Controls.Add(btnSelectFolder);
            Controls.Add(txtSavePath);
            Controls.Add(txtUrl);
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox txtUrl;
        private TextBox txtSavePath;
        private Button btnSelectFolder;
        private ProgressBar progressBar;
        private Button btnStart;
        private RichTextBox rtbLog;
    }
}
