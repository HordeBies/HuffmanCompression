using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class MainMenu : Form
    {
        public MainMenu()
        {
            InitializeComponent();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            linkLabel1.LinkVisited = true;
            System.Diagnostics.Process.Start("https://github.com/HordeBies/HuffmanCompression");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
                Program.staticHuffman.cleanInit();
            Program.staticHuffman.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
                Program.dynamicHuffman.cleanInit();
            Program.dynamicHuffman.Show();
        }
    }
}
