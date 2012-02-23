using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Devel_VM;

namespace InfoSender
{
    public partial class Form1 : Form
    {
        Network_listener nl;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            nl = new Network_listener();
            nl.OnAny += new Network_listener.AnyEvent(Any);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Packet p = new Packet();
            p.message = textBox1.Text;
            p.auth = "TestApp";
            switch (comboBox1.Text)
            {
                case "Info":
                    p.dataIdentifier = Packet.DataIdentifier.Info;
                    break;
                case "Error":
                    p.dataIdentifier = Packet.DataIdentifier.Error;
                    break;
                case "Kill":
                    p.dataIdentifier = Packet.DataIdentifier.Kill;
                    break;
                case "Ping":
                    p.dataIdentifier = Packet.DataIdentifier.Ping;
                    break;
                case "Update":
                    p.dataIdentifier = Packet.DataIdentifier.Update;
                    break;
                case "Execute":
                    p.dataIdentifier = Packet.DataIdentifier.Execute;
                    break;
                case "Chat":
                    p.dataIdentifier = Packet.DataIdentifier.Chat;
                    break;
                default:
                    p.dataIdentifier = Packet.DataIdentifier.Null;
                    break;
            }

            Network_Broadcast.send(p);
        }

        void Any(String t, string a, string m)
        {
            richTextBox1.Text += "\r\n [" + t + "] " + a + ": " + m;
        }
    }
}
