﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace Devel_VM
{
    public partial class fMain : Form
    {

        public fMain()
        {
            InitializeComponent();
            zasobnik.Visible = true;
        }
        /*
        private void button2_Click(object sender, EventArgs e)
        {
            zasobnik.ShowBalloonTip(5000, "title", "text", ToolTipIcon.Info);
        }
        */

        #region Tray options
        private void showToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
        }
        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.VM.Start();
        }
        private void restartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.VM.Restart();
        }
        private void softstopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.VM.PowerOff(false);
        }
        private void stoppoweroffToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.VM.PowerOff(true);
        }

        private void zasobnik_DoubleClick(object sender, EventArgs e)
        {
            Show();
        }
        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
            Application.Exit();
        }
        #endregion

        #region Form control
        private void fMain_Load(object sender, EventArgs e)
        {
            Hide();
            Visible = false;
        }
        private void bHide_Click(object sender, EventArgs e)
        {
            Hide();
        }
        #endregion

        #region Timers' Events
        private void tState_Tick(object sender, EventArgs e)
        {
            Program.VM.Tick();
            
        }
        private void tUpdateState_Tick(object sender, EventArgs e)
        {
            switch (Program.VM.Status)
            {
                case VirtualMachine.State.Off:
                    lState.Text = "Off";
                    break;
                case VirtualMachine.State.Busy:
                    lState.Text = "Busy";
                    break;
                case VirtualMachine.State.Operational:
                    lState.Text = "Operational";
                    break;
                case VirtualMachine.State.Error:
                    lState.Text = "Error";
                    break;
                default:
                    lState.Text = "Unknown";
                    break;
            }
        }
        #endregion

        private void button1_Click(object sender, EventArgs e)
        {
            Program.VM.unlock();
        }

        

    }
}
