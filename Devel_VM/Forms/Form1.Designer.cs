﻿namespace Devel_VM
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
            this.zasobnik = new System.Windows.Forms.NotifyIcon(this.components);
            this.menuZasobnik = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.showToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.hTTPDToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.restartToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.reloadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem6 = new System.Windows.Forms.ToolStripSeparator();
            this.startToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.stopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.zasobyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aLFAToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bETA100ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bETAToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripSeparator();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bHide = new System.Windows.Forms.Button();
            this.tState = new System.Windows.Forms.Timer(this.components);
            this.tUpdateState = new System.Windows.Forms.Timer(this.components);
            this.tAutoStart = new System.Windows.Forms.Timer(this.components);
            this.button2 = new System.Windows.Forms.Button();
            this.menuZasobnik.SuspendLayout();
            this.SuspendLayout();
            // 
            // bExit
            // 
            this.bExit.Location = new System.Drawing.Point(160, 3);
            this.bExit.Name = "bExit";
            this.bExit.Size = new System.Drawing.Size(79, 23);
            this.bExit.TabIndex = 0;
            this.bExit.Text = "Exit";
            this.bExit.UseVisualStyleBackColor = true;
            this.bExit.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
            // 
            // zasobnik
            // 
            this.zasobnik.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.zasobnik.ContextMenuStrip = this.menuZasobnik;
            this.zasobnik.Icon = ((System.Drawing.Icon)(resources.GetObject("zasobnik.Icon")));
            this.zasobnik.Text = "Beta Manager";
            this.zasobnik.Visible = true;
            this.zasobnik.DoubleClick += new System.EventHandler(this.zasobnik_DoubleClick);
            // 
            // menuZasobnik
            // 
            this.menuZasobnik.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showToolStripMenuItem,
            this.toolStripSeparator1,
            this.toolStripMenuItem2,
            this.toolStripMenuItem3,
            this.toolStripMenuItem1,
            this.hTTPDToolStripMenuItem,
            this.zasobyToolStripMenuItem,
            this.bETAToolStripMenuItem,
            this.toolStripMenuItem5,
            this.closeToolStripMenuItem});
            this.menuZasobnik.Name = "menuZasobnik";
            this.menuZasobnik.Size = new System.Drawing.Size(153, 198);
            // 
            // showToolStripMenuItem
            // 
            this.showToolStripMenuItem.Name = "showToolStripMenuItem";
            this.showToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.showToolStripMenuItem.Text = "Show";
            this.showToolStripMenuItem.Click += new System.EventHandler(this.showToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(149, 6);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Enabled = false;
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(152, 22);
            this.toolStripMenuItem2.Text = "Wait";
            this.toolStripMenuItem2.Click += new System.EventHandler(this.toolStripMenuItem2_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Enabled = false;
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(152, 22);
            this.toolStripMenuItem3.Text = "Wait";
            this.toolStripMenuItem3.Visible = false;
            this.toolStripMenuItem3.Click += new System.EventHandler(this.toolStripMenuItem3_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(149, 6);
            // 
            // hTTPDToolStripMenuItem
            // 
            this.hTTPDToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.restartToolStripMenuItem1,
            this.reloadToolStripMenuItem,
            this.toolStripMenuItem6,
            this.startToolStripMenuItem1,
            this.stopToolStripMenuItem});
            this.hTTPDToolStripMenuItem.Enabled = false;
            this.hTTPDToolStripMenuItem.Name = "hTTPDToolStripMenuItem";
            this.hTTPDToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.hTTPDToolStripMenuItem.Text = "HTTPD";
            // 
            // restartToolStripMenuItem1
            // 
            this.restartToolStripMenuItem1.Name = "restartToolStripMenuItem1";
            this.restartToolStripMenuItem1.Size = new System.Drawing.Size(110, 22);
            this.restartToolStripMenuItem1.Text = "Restart";
            this.restartToolStripMenuItem1.Click += new System.EventHandler(this.restartToolStripMenuItem1_Click);
            // 
            // reloadToolStripMenuItem
            // 
            this.reloadToolStripMenuItem.Name = "reloadToolStripMenuItem";
            this.reloadToolStripMenuItem.Size = new System.Drawing.Size(110, 22);
            this.reloadToolStripMenuItem.Text = "Reload";
            this.reloadToolStripMenuItem.Click += new System.EventHandler(this.reloadToolStripMenuItem_Click);
            // 
            // toolStripMenuItem6
            // 
            this.toolStripMenuItem6.Name = "toolStripMenuItem6";
            this.toolStripMenuItem6.Size = new System.Drawing.Size(107, 6);
            // 
            // startToolStripMenuItem1
            // 
            this.startToolStripMenuItem1.Name = "startToolStripMenuItem1";
            this.startToolStripMenuItem1.Size = new System.Drawing.Size(110, 22);
            this.startToolStripMenuItem1.Text = "Start";
            this.startToolStripMenuItem1.Click += new System.EventHandler(this.startToolStripMenuItem1_Click);
            // 
            // stopToolStripMenuItem
            // 
            this.stopToolStripMenuItem.Name = "stopToolStripMenuItem";
            this.stopToolStripMenuItem.Size = new System.Drawing.Size(110, 22);
            this.stopToolStripMenuItem.Text = "Stop";
            this.stopToolStripMenuItem.Click += new System.EventHandler(this.stopToolStripMenuItem_Click);
            // 
            // zasobyToolStripMenuItem
            // 
            this.zasobyToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aLFAToolStripMenuItem,
            this.bETA100ToolStripMenuItem});
            this.zasobyToolStripMenuItem.Enabled = false;
            this.zasobyToolStripMenuItem.Name = "zasobyToolStripMenuItem";
            this.zasobyToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.zasobyToolStripMenuItem.Text = "SAMBA";
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
            this.bETAToolStripMenuItem.Enabled = false;
            this.bETAToolStripMenuItem.Name = "bETAToolStripMenuItem";
            this.bETAToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.bETAToolStripMenuItem.Text = "WWW";
            // 
            // toolStripMenuItem5
            // 
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            this.toolStripMenuItem5.Size = new System.Drawing.Size(149, 6);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.closeToolStripMenuItem.Text = "Close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
            // 
            // bHide
            // 
            this.bHide.Location = new System.Drawing.Point(116, 3);
            this.bHide.Name = "bHide";
            this.bHide.Size = new System.Drawing.Size(38, 23);
            this.bHide.TabIndex = 7;
            this.bHide.Text = "Hide";
            this.bHide.UseVisualStyleBackColor = true;
            this.bHide.Click += new System.EventHandler(this.bHide_Click);
            // 
            // tState
            // 
            this.tState.Enabled = true;
            this.tState.Interval = 1000;
            this.tState.Tick += new System.EventHandler(this.tState_Tick);
            // 
            // tUpdateState
            // 
            this.tUpdateState.Enabled = true;
            this.tUpdateState.Interval = 2000;
            this.tUpdateState.Tick += new System.EventHandler(this.tUpdateState_Tick);
            // 
            // tAutoStart
            // 
            this.tAutoStart.Enabled = true;
            this.tAutoStart.Interval = 60000;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(1, 3);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(109, 23);
            this.button2.TabIndex = 12;
            this.button2.Text = "debug";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // fMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(246, 28);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.bHide);
            this.Controls.Add(this.bExit);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "fMain";
            this.Text = "Beta Manager";
            this.Load += new System.EventHandler(this.fMain_Load);
            this.menuZasobnik.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button bExit;
        private System.Windows.Forms.NotifyIcon zasobnik;
        private System.Windows.Forms.ContextMenuStrip menuZasobnik;
        private System.Windows.Forms.ToolStripMenuItem showToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem zasobyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem bETAToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem5;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aLFAToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem bETA100ToolStripMenuItem;
        private System.Windows.Forms.Button bHide;
        internal System.Windows.Forms.Timer tState;
        private System.Windows.Forms.Timer tUpdateState;
        private System.Windows.Forms.Timer tAutoStart;
        private System.Windows.Forms.ToolStripMenuItem hTTPDToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem restartToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem reloadToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem6;
        private System.Windows.Forms.ToolStripMenuItem startToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem stopToolStripMenuItem;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem3;
    }
}
