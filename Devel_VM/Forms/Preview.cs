using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Devel_VM.Forms
{
    public partial class Preview : Form
    {
        public Preview()
        {
            InitializeComponent();
        }

        public byte[] data;

        private void Preview_Load(object sender, EventArgs e)
        {

        }

        private void Preview_MouseClick(object sender, MouseEventArgs e)
        {
            byte[] buff = new byte[data.Length];
 
            for(int i = 0; i<data.Length; i++) {
                if (data[i] > 255) throw null;
                buff[i] = (byte) data[i];
            }

            pictureBox1.Image = Image.FromStream(new MemoryStream(buff));
            
        }
    }
}
