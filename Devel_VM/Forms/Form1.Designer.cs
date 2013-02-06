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
            this.zasobnik = new System.Windows.Forms.NotifyIcon(this.components);
            this.menuZasobnik = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.showToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem7 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem8 = new System.Windows.Forms.ToolStripMenuItem();
            this.hTTPDToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.restartToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.reloadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem6 = new System.Windows.Forms.ToolStripSeparator();
            this.startToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.stopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sprawdźAktulizacjeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pobierzNaNowoObrazToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.usuńObrazToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resetSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.DeamonsStripMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.bETAToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.zasobyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aLFAToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bETA100ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripSeparator();
            this.informacjeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.testToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bHide = new System.Windows.Forms.Button();
            this.tState = new System.Windows.Forms.Timer(this.components);
            this.tUpdateState = new System.Windows.Forms.Timer(this.components);
            this.tAutoStart = new System.Windows.Forms.Timer(this.components);
            this.button2 = new System.Windows.Forms.Button();
            this.tUpdateAutocheck = new System.Windows.Forms.Timer(this.components);
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
            this.toolStripMenuItem7,
            this.toolStripMenuItem8,
            this.toolStripSeparator2,
            this.DeamonsStripMenu,
            this.bETAToolStripMenuItem,
            this.zasobyToolStripMenuItem,
            this.toolStripMenuItem5,
            this.informacjeToolStripMenuItem,
            this.toolStripMenuItem4,
            this.closeToolStripMenuItem,
            this.testToolStripMenuItem});
            this.menuZasobnik.Name = "menuZasobnik";
            this.menuZasobnik.Size = new System.Drawing.Size(166, 292);
            // 
            // showToolStripMenuItem
            // 
            this.showToolStripMenuItem.Name = "showToolStripMenuItem";
            this.showToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
            this.showToolStripMenuItem.Text = "Show";
            this.showToolStripMenuItem.Visible = false;
            this.showToolStripMenuItem.Click += new System.EventHandler(this.showToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(162, 6);
            this.toolStripSeparator1.Visible = false;
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Enabled = false;
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(165, 22);
            this.toolStripMenuItem2.Text = "Wait";
            this.toolStripMenuItem2.Click += new System.EventHandler(this.toolStripMenuItem2_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Enabled = false;
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(165, 22);
            this.toolStripMenuItem3.Text = "Wait";
            this.toolStripMenuItem3.Visible = false;
            this.toolStripMenuItem3.Click += new System.EventHandler(this.toolStripMenuItem3_Click);
            // 
            // toolStripMenuItem7
            // 
            this.toolStripMenuItem7.Name = "toolStripMenuItem7";
            this.toolStripMenuItem7.Size = new System.Drawing.Size(165, 22);
            this.toolStripMenuItem7.Text = "Aktualizuj obraz";
            this.toolStripMenuItem7.Visible = false;
            this.toolStripMenuItem7.Click += new System.EventHandler(this.toolStripMenuItem7_Click);
            // 
            // toolStripMenuItem8
            // 
            this.toolStripMenuItem8.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.hTTPDToolStripMenuItem,
            this.sprawdźAktulizacjeToolStripMenuItem,
            this.pobierzNaNowoObrazToolStripMenuItem,
            this.usuńObrazToolStripMenuItem,
            this.resetSettingsToolStripMenuItem});
            this.toolStripMenuItem8.Name = "toolStripMenuItem8";
            this.toolStripMenuItem8.Size = new System.Drawing.Size(165, 22);
            this.toolStripMenuItem8.Text = "Zaawansowane";
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
            this.hTTPDToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
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
            // sprawdźAktulizacjeToolStripMenuItem
            // 
            this.sprawdźAktulizacjeToolStripMenuItem.Name = "sprawdźAktulizacjeToolStripMenuItem";
            this.sprawdźAktulizacjeToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
            this.sprawdźAktulizacjeToolStripMenuItem.Text = "Sprawdź aktulizacje";
            this.sprawdźAktulizacjeToolStripMenuItem.Click += new System.EventHandler(this.sprawdzAktulizacjeToolStripMenuItem_Click);
            // 
            // pobierzNaNowoObrazToolStripMenuItem
            // 
            this.pobierzNaNowoObrazToolStripMenuItem.Name = "pobierzNaNowoObrazToolStripMenuItem";
            this.pobierzNaNowoObrazToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
            this.pobierzNaNowoObrazToolStripMenuItem.Text = "Pobierz na nowo obraz";
            this.pobierzNaNowoObrazToolStripMenuItem.Click += new System.EventHandler(this.toolStripMenuItem7_Click);
            // 
            // usuńObrazToolStripMenuItem
            // 
            this.usuńObrazToolStripMenuItem.Name = "usuńObrazToolStripMenuItem";
            this.usuńObrazToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
            this.usuńObrazToolStripMenuItem.Text = "Usuń obraz";
            this.usuńObrazToolStripMenuItem.Click += new System.EventHandler(this.usunObrazToolStripMenuItem_Click);
            // 
            // resetSettingsToolStripMenuItem
            // 
            this.resetSettingsToolStripMenuItem.Name = "resetSettingsToolStripMenuItem";
            this.resetSettingsToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
            this.resetSettingsToolStripMenuItem.Text = "Reset ustawień";
            this.resetSettingsToolStripMenuItem.Click += new System.EventHandler(this.resetSettingsToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(162, 6);
            // 
            // DeamonsStripMenu
            // 
            this.DeamonsStripMenu.Name = "DeamonsStripMenu";
            this.DeamonsStripMenu.Size = new System.Drawing.Size(165, 22);
            this.DeamonsStripMenu.Text = "Demony";
            this.DeamonsStripMenu.Visible = false;
            // 
            // bETAToolStripMenuItem
            // 
            this.bETAToolStripMenuItem.Name = "bETAToolStripMenuItem";
            this.bETAToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
            this.bETAToolStripMenuItem.Text = "Otwórz stronę";
            this.bETAToolStripMenuItem.Visible = false;
            // 
            // zasobyToolStripMenuItem
            // 
            this.zasobyToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aLFAToolStripMenuItem,
            this.bETA100ToolStripMenuItem});
            this.zasobyToolStripMenuItem.Name = "zasobyToolStripMenuItem";
            this.zasobyToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
            this.zasobyToolStripMenuItem.Text = "Katalogi sieciowe";
            // 
            // aLFAToolStripMenuItem
            // 
            this.aLFAToolStripMenuItem.Name = "aLFAToolStripMenuItem";
            this.aLFAToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.aLFAToolStripMenuItem.Text = "alpha";
            this.aLFAToolStripMenuItem.Click += new System.EventHandler(this.aLFAToolStripMenuItem_Click);
            // 
            // bETA100ToolStripMenuItem
            // 
            this.bETA100ToolStripMenuItem.Name = "bETA100ToolStripMenuItem";
            this.bETA100ToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.bETA100ToolStripMenuItem.Text = "beta";
            this.bETA100ToolStripMenuItem.Click += new System.EventHandler(this.bETA100ToolStripMenuItem_Click);
            // 
            // toolStripMenuItem5
            // 
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            this.toolStripMenuItem5.Size = new System.Drawing.Size(162, 6);
            // 
            // informacjeToolStripMenuItem
            // 
            this.informacjeToolStripMenuItem.Name = "informacjeToolStripMenuItem";
            this.informacjeToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
            this.informacjeToolStripMenuItem.Text = "Informacje";
            this.informacjeToolStripMenuItem.Click += new System.EventHandler(this.informacjeToolStripMenuItem_Click);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(162, 6);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
            this.closeToolStripMenuItem.Text = "Zamknij";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
            // 
            // testToolStripMenuItem
            // 
            this.testToolStripMenuItem.Name = "testToolStripMenuItem";
            this.testToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
            this.testToolStripMenuItem.Text = "Test";
            this.testToolStripMenuItem.Visible = false;
            this.testToolStripMenuItem.Click += new System.EventHandler(this.testToolStripMenuItem_Click);
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
            this.tAutoStart.Interval = 15000;
            this.tAutoStart.Tick += new System.EventHandler(this.tAutoStart_Tick);
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
            // tUpdateAutocheck
            // 
            this.tUpdateAutocheck.Enabled = true;
            this.tUpdateAutocheck.Interval = 3000;
            this.tUpdateAutocheck.Tick += new System.EventHandler(this.tUpdateAutocheck_Tick);
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
            this.menuZasobnik.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button bExit;
        private System.Windows.Forms.NotifyIcon zasobnik;
        private System.Windows.Forms.ContextMenuStrip menuZasobnik;
        private System.Windows.Forms.ToolStripMenuItem showToolStripMenuItem;
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
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem informacjeToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem7;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem8;
        private System.Windows.Forms.ToolStripMenuItem hTTPDToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem restartToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem reloadToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem6;
        private System.Windows.Forms.ToolStripMenuItem startToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem stopToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sprawdźAktulizacjeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pobierzNaNowoObrazToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem usuńObrazToolStripMenuItem;
        private System.Windows.Forms.Timer tUpdateAutocheck;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem DeamonsStripMenu;
        private System.Windows.Forms.ToolStripMenuItem testToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resetSettingsToolStripMenuItem;
    }
}

