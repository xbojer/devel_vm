using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Devel_VM
{
    public partial class fMain : Form
    {
        bool VMstarted = false;
        bool VMclosing = false;

        public fMain()
        {
            InitializeComponent();
            zasobnik.Visible = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            zasobnik.ShowBalloonTip(5000, "title", "text", ToolTipIcon.Info);
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            folderBrowse.SelectedPath = tPath.Text;
            if (folderBrowse.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                tPath.Text = folderBrowse.SelectedPath;
            }
        }

        private void softstopToolStripMenuItem_Click(object sender, EventArgs e)
        {
                vmACPI();
                bool t = processVM.WaitForExit(10000);
                if (t == true)
                {
                    vmPowerOff();
                }
        }

        private void bExit_Click(object sender, EventArgs e)
        {
            if (VMstarted)
            {
                vmACPI();
                bool t =processVM.WaitForExit(10000);
                if (t == true)
                {
                    vmPowerOff();
                }
            }
            Close();
            Application.Exit();
        }

        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            vmStart();
        }
        private void vmStart()
        {
            processVM.Refresh();
            processVM.StartInfo.Arguments = "-s Devel";
            processVM.StartInfo.FileName = tPath.Text + "\\VBoxHeadless.exe";
            processVM.StartInfo.WorkingDirectory = tPath.Text;
            if (processVM.Start())
            {
                VMstarted = true;
            }
        }
        private void vmACPI()
        {
            processMGMT.Refresh();
            processMGMT.StartInfo.Arguments = "controlvm Devel acpipowerbutton";
            processMGMT.StartInfo.FileName = tPath.Text + "\\VBoxManage.exe";
            processMGMT.StartInfo.WorkingDirectory = tPath.Text;
            processMGMT.Start();
        }
        private void vmPowerOff()
        {
            processMGMT.Refresh();
            processMGMT.StartInfo.Arguments = "controlvm Devel poweroff";
            processMGMT.StartInfo.FileName = tPath.Text + "\\VBoxManage.exe";
            processMGMT.StartInfo.WorkingDirectory = tPath.Text;
            processMGMT.Start();
        }
        private void vmReset()
        {
            vmPowerOff();
            vmStart();
        }

        private void zasobnik_DoubleClick(object sender, EventArgs e)
        {
            Show();
        }

        private void processVM_Exited(object sender, EventArgs e)
        {
            VMstarted = false;
        }

        private void bStopPower_Click(object sender, EventArgs e)
        {
            vmPowerOff();
        }

        private void showToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Show();
        }

        private void fMain_Load(object sender, EventArgs e)
        {
            Hide();
            Visible = false;
            
        }

        private void bHide_Click(object sender, EventArgs e)
        {
            Hide();
        }
    }
}
