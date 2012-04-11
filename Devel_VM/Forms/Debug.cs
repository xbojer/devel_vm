using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Devel_VM.Forms
{
    public partial class Debug : Form
    {
        public Debug()
        {
            if (Program.VM == null) throw new Exception("Nie można załadować ");
            InitializeComponent();
        }

        private void debugLog(String msg, String title, int priority)
        {
            MethodInvoker method = delegate
            {
                textBox1.Text += "["+title+"] ("+priority.ToString()+"): "+msg+"\n";
            };
            if (InvokeRequired) BeginInvoke(method);
            else method.Invoke();
        }
        private void Debug_Load(object sender, EventArgs e)
        {
            Program.VM.OnVmEvent += new VirtualMachine.VmEvent(this.debugLog);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            contextMenuStrip1.Show();
        }
    }
}
