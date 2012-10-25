namespace DVMinstaller
{
    partial class f
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(f));
            this.txt = new System.Windows.Forms.Label();
            this.pb = new System.Windows.Forms.ProgressBar();
            this.SuspendLayout();
            // 
            // txt
            // 
            this.txt.AutoSize = true;
            this.txt.Location = new System.Drawing.Point(12, 8);
            this.txt.Name = "txt";
            this.txt.Size = new System.Drawing.Size(20, 13);
            this.txt.TabIndex = 0;
            this.txt.Text = "init";
            // 
            // pb
            // 
            this.pb.Location = new System.Drawing.Point(12, 25);
            this.pb.MarqueeAnimationSpeed = 10;
            this.pb.Name = "pb";
            this.pb.Size = new System.Drawing.Size(296, 23);
            this.pb.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.pb.TabIndex = 1;
            // 
            // f
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(320, 68);
            this.Controls.Add(this.pb);
            this.Controls.Add(this.txt);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "f";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Devel VM Installer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.f_FormClosing);
            this.Shown += new System.EventHandler(this.f_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label txt;
        private System.Windows.Forms.ProgressBar pb;
    }
}

