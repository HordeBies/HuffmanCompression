using System;
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
            if (Program.dynamicHuffman.IsDisposed) { 
                MessageBox.Show("This Form forcefully terminated.\nIn order to prevent corruption, application needs to be restarted", "Disposed Form Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Program.staticHuffman.Size = new System.Drawing.Size(Screen.FromControl(this).Bounds.Width, Screen.FromControl(this).Bounds.Height);
            Program.staticHuffman.Location = new System.Drawing.Point(Screen.FromControl(this).Bounds.X, Screen.FromControl(this).Bounds.Y);
            Program.staticHuffman.ResizeGUI();

            if (checkBox1.Checked)
                Program.staticHuffman.cleanInit();
            Program.staticHuffman.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (Program.dynamicHuffman.IsDisposed) { 
                MessageBox.Show("This Form forcefully terminated.\nIn order to prevent corruption, application needs to be restarted", "Disposed Form Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Program.dynamicHuffman.Size = new System.Drawing.Size(Screen.FromControl(this).Bounds.Width, Screen.FromControl(this).Bounds.Height);
            Program.dynamicHuffman.Location = new System.Drawing.Point(Screen.FromControl(this).Bounds.X, Screen.FromControl(this).Bounds.Y);
            Program.dynamicHuffman.ResizeGUI();

            if (checkBox2.Checked)
                Program.dynamicHuffman.cleanInit();

            Program.dynamicHuffman.ALT_F4 = false;
            Program.dynamicHuffman.Show();
        }
    }
}
