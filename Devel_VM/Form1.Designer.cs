namespace Devel_VM
{
    partial class fMain
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(fMain));
            this.bExit = new System.Windows.Forms.Button();
            this.processVM = new System.Diagnostics.Process();
            this.zasobnik = new System.Windows.Forms.NotifyIcon(this.components);
            this.menuZasobnik = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.showToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.vMToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.softstopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stoppoweroffToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.restartToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
            this.installToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.runInstallerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.updateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.zasobyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aLFAToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bETA100ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bETAToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripSeparator();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.folderBrowse = new System.Windows.Forms.FolderBrowserDialog();
            this.tPath = new System.Windows.Forms.TextBox();
            this.bBrowse = new System.Windows.Forms.Button();
            this.processMGMT = new System.Diagnostics.Process();
            this.bStart = new System.Windows.Forms.Button();
            this.bSoftStop = new System.Windows.Forms.Button();
            this.bStopPower = new System.Windows.Forms.Button();
            this.bHide = new System.Windows.Forms.Button();
            this.menuZasobnik.SuspendLayout();
            this.SuspendLayout();
            // 
            // bExit
            // 
            this.bExit.Location = new System.Drawing.Point(422, 132);
            this.bExit.Name = "bExit";
            this.bExit.Size = new System.Drawing.Size(102, 25);
            this.bExit.TabIndex = 0;
            this.bExit.Text = "Exit";
            this.bExit.UseVisualStyleBackColor = true;
            this.bExit.Click += new System.EventHandler(this.bExit_Click);
            // 
            // processVM
            // 
            this.processVM.StartInfo.CreateNoWindow = true;
            this.processVM.StartInfo.Domain = "";
            this.processVM.StartInfo.LoadUserProfile = false;
            this.processVM.StartInfo.Password = null;
            this.processVM.StartInfo.RedirectStandardError = true;
            this.processVM.StartInfo.RedirectStandardInput = true;
            this.processVM.StartInfo.RedirectStandardOutput = true;
            this.processVM.StartInfo.StandardErrorEncoding = null;
            this.processVM.StartInfo.StandardOutputEncoding = null;
            this.processVM.StartInfo.UserName = "";
            this.processVM.StartInfo.UseShellExecute = false;
            this.processVM.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            this.processVM.SynchronizingObject = this;
            this.processVM.Exited += new System.EventHandler(this.processVM_Exited);
            // 
            // zasobnik
            // 
            this.zasobnik.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.zasobnik.ContextMenuStrip = this.menuZasobnik;
            this.zasobnik.Icon = ((System.Drawing.Icon)(resources.GetObject("zasobnik.Icon")));
            this.zasobnik.Text = "Devel Control";
            this.zasobnik.Visible = true;
            this.zasobnik.DoubleClick += new System.EventHandler(this.zasobnik_DoubleClick);
            // 
            // menuZasobnik
            // 
            this.menuZasobnik.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showToolStripMenuItem,
            this.toolStripMenuItem1,
            this.vMToolStripMenuItem,
            this.zasobyToolStripMenuItem,
            this.bETAToolStripMenuItem,
            this.toolStripMenuItem5,
            this.closeToolStripMenuItem});
            this.menuZasobnik.Name = "menuZasobnik";
            this.menuZasobnik.Size = new System.Drawing.Size(129, 126);
            // 
            // showToolStripMenuItem
            // 
            this.showToolStripMenuItem.Name = "showToolStripMenuItem";
            this.showToolStripMenuItem.Size = new System.Drawing.Size(128, 22);
            this.showToolStripMenuItem.Text = "Show";
            this.showToolStripMenuItem.Click += new System.EventHandler(this.showToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(125, 6);
            // 
            // vMToolStripMenuItem
            // 
            this.vMToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.startToolStripMenuItem,
            this.toolStripMenuItem2,
            this.softstopToolStripMenuItem,
            this.stoppoweroffToolStripMenuItem,
            this.toolStripMenuItem3,
            this.restartToolStripMenuItem,
            this.toolStripMenuItem4,
            this.installToolStripMenuItem});
            this.vMToolStripMenuItem.Name = "vMToolStripMenuItem";
            this.vMToolStripMenuItem.Size = new System.Drawing.Size(128, 22);
            this.vMToolStripMenuItem.Text = "VM";
            // 
            // startToolStripMenuItem
            // 
            this.startToolStripMenuItem.Name = "startToolStripMenuItem";
            this.startToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.startToolStripMenuItem.Text = "Start";
            this.startToolStripMenuItem.Click += new System.EventHandler(this.startToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(159, 6);
            // 
            // softstopToolStripMenuItem
            // 
            this.softstopToolStripMenuItem.Name = "softstopToolStripMenuItem";
            this.softstopToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.softstopToolStripMenuItem.Text = "Stop (soft)";
            this.softstopToolStripMenuItem.Click += new System.EventHandler(this.softstopToolStripMenuItem_Click);
            // 
            // stoppoweroffToolStripMenuItem
            // 
            this.stoppoweroffToolStripMenuItem.Name = "stoppoweroffToolStripMenuItem";
            this.stoppoweroffToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.stoppoweroffToolStripMenuItem.Text = "Stop (power-off)";
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(159, 6);
            // 
            // restartToolStripMenuItem
            // 
            this.restartToolStripMenuItem.Name = "restartToolStripMenuItem";
            this.restartToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.restartToolStripMenuItem.Text = "Restart";
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(159, 6);
            // 
            // installToolStripMenuItem
            // 
            this.installToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.runInstallerToolStripMenuItem,
            this.updateToolStripMenuItem});
            this.installToolStripMenuItem.Name = "installToolStripMenuItem";
            this.installToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.installToolStripMenuItem.Text = "Install";
            // 
            // runInstallerToolStripMenuItem
            // 
            this.runInstallerToolStripMenuItem.Name = "runInstallerToolStripMenuItem";
            this.runInstallerToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.runInstallerToolStripMenuItem.Text = "Run installer";
            // 
            // updateToolStripMenuItem
            // 
            this.updateToolStripMenuItem.Name = "updateToolStripMenuItem";
            this.updateToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.updateToolStripMenuItem.Text = "Update";
            // 
            // zasobyToolStripMenuItem
            // 
            this.zasobyToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aLFAToolStripMenuItem,
            this.bETA100ToolStripMenuItem});
            this.zasobyToolStripMenuItem.Name = "zasobyToolStripMenuItem";
            this.zasobyToolStripMenuItem.Size = new System.Drawing.Size(128, 22);
            this.zasobyToolStripMenuItem.Text = "Przejdź do";
            // 
            // aLFAToolStripMenuItem
            // 
            this.aLFAToolStripMenuItem.Name = "aLFAToolStripMenuItem";
            this.aLFAToolStripMenuItem.Size = new System.Drawing.Size(131, 22);
            this.aLFAToolStripMenuItem.Text = "ALFA (200)";
            // 
            // bETA100ToolStripMenuItem
            // 
            this.bETA100ToolStripMenuItem.Name = "bETA100ToolStripMenuItem";
            this.bETA100ToolStripMenuItem.Size = new System.Drawing.Size(131, 22);
            this.bETA100ToolStripMenuItem.Text = "BETA (100)";
            // 
            // bETAToolStripMenuItem
            // 
            this.bETAToolStripMenuItem.Name = "bETAToolStripMenuItem";
            this.bETAToolStripMenuItem.Size = new System.Drawing.Size(128, 22);
            this.bETAToolStripMenuItem.Text = "BETA";
            // 
            // toolStripMenuItem5
            // 
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            this.toolStripMenuItem5.Size = new System.Drawing.Size(125, 6);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(128, 22);
            this.closeToolStripMenuItem.Text = "Close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.bExit_Click);
            // 
            // tPath
            // 
            this.tPath.Location = new System.Drawing.Point(12, 12);
            this.tPath.Name = "tPath";
            this.tPath.Size = new System.Drawing.Size(451, 20);
            this.tPath.TabIndex = 1;
            this.tPath.Text = "C:\\Program Files\\Oracle\\VirtualBox";
            // 
            // bBrowse
            // 
            this.bBrowse.Location = new System.Drawing.Point(469, 12);
            this.bBrowse.Name = "bBrowse";
            this.bBrowse.Size = new System.Drawing.Size(55, 20);
            this.bBrowse.TabIndex = 2;
            this.bBrowse.Text = "Browse";
            this.bBrowse.UseVisualStyleBackColor = true;
            this.bBrowse.Click += new System.EventHandler(this.button2_Click_1);
            // 
            // processMGMT
            // 
            this.processMGMT.StartInfo.CreateNoWindow = true;
            this.processMGMT.StartInfo.Domain = "";
            this.processMGMT.StartInfo.LoadUserProfile = false;
            this.processMGMT.StartInfo.Password = null;
            this.processMGMT.StartInfo.RedirectStandardError = true;
            this.processMGMT.StartInfo.RedirectStandardInput = true;
            this.processMGMT.StartInfo.RedirectStandardOutput = true;
            this.processMGMT.StartInfo.StandardErrorEncoding = null;
            this.processMGMT.StartInfo.StandardOutputEncoding = null;
            this.processMGMT.StartInfo.UserName = "";
            this.processMGMT.StartInfo.UseShellExecute = false;
            this.processMGMT.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            this.processMGMT.SynchronizingObject = this;
            // 
            // bStart
            // 
            this.bStart.Location = new System.Drawing.Point(12, 132);
            this.bStart.Name = "bStart";
            this.bStart.Size = new System.Drawing.Size(75, 23);
            this.bStart.TabIndex = 4;
            this.bStart.Text = "Start VM";
            this.bStart.UseVisualStyleBackColor = true;
            this.bStart.Click += new System.EventHandler(this.startToolStripMenuItem_Click);
            // 
            // bSoftStop
            // 
            this.bSoftStop.Location = new System.Drawing.Point(93, 132);
            this.bSoftStop.Name = "bSoftStop";
            this.bSoftStop.Size = new System.Drawing.Size(75, 23);
            this.bSoftStop.TabIndex = 5;
            this.bSoftStop.Text = "Stop (soft)";
            this.bSoftStop.UseVisualStyleBackColor = true;
            this.bSoftStop.Click += new System.EventHandler(this.softstopToolStripMenuItem_Click);
            // 
            // bStopPower
            // 
            this.bStopPower.Location = new System.Drawing.Point(174, 132);
            this.bStopPower.Name = "bStopPower";
            this.bStopPower.Size = new System.Drawing.Size(90, 23);
            this.bStopPower.TabIndex = 6;
            this.bStopPower.Text = "Stop (poweroff)";
            this.bStopPower.UseVisualStyleBackColor = true;
            this.bStopPower.Click += new System.EventHandler(this.bStopPower_Click);
            // 
            // bHide
            // 
            this.bHide.Location = new System.Drawing.Point(449, 103);
            this.bHide.Name = "bHide";
            this.bHide.Size = new System.Drawing.Size(75, 23);
            this.bHide.TabIndex = 7;
            this.bHide.Text = "Hide";
            this.bHide.UseVisualStyleBackColor = true;
            this.bHide.Click += new System.EventHandler(this.bHide_Click);
            // 
            // fMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(536, 169);
            this.Controls.Add(this.bHide);
            this.Controls.Add(this.bBrowse);
            this.Controls.Add(this.bStopPower);
            this.Controls.Add(this.bSoftStop);
            this.Controls.Add(this.bStart);
            this.Controls.Add(this.tPath);
            this.Controls.Add(this.bExit);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "fMain";
            this.Text = "Devel VM Controller";
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            this.Load += new System.EventHandler(this.fMain_Load);
            this.menuZasobnik.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button bExit;
        private System.Diagnostics.Process processVM;
        private System.Windows.Forms.NotifyIcon zasobnik;
        private System.Windows.Forms.Button bBrowse;
        private System.Windows.Forms.TextBox tPath;
        private System.Windows.Forms.FolderBrowserDialog folderBrowse;
        private System.Windows.Forms.ContextMenuStrip menuZasobnik;
        private System.Windows.Forms.ToolStripMenuItem showToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem vMToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem startToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem softstopToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stoppoweroffToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem restartToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem installToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem runInstallerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem updateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem zasobyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem bETAToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem5;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aLFAToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem bETA100ToolStripMenuItem;
        private System.Diagnostics.Process processMGMT;
        private System.Windows.Forms.Button bStart;
        private System.Windows.Forms.Button bSoftStop;
        private System.Windows.Forms.Button bStopPower;
        private System.Windows.Forms.Button bHide;
    }
}

