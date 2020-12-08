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
    public partial class Form2 : Form
    {
        private static Microsoft.Msagl.GraphViewerGdi.GViewer viewer = new Microsoft.Msagl.GraphViewerGdi.GViewer();

        public Form2()
        {
            InitializeComponent();
            
            viewer.Dock = System.Windows.Forms.DockStyle.None;
            viewer.EdgeInsertButtonVisible = false;
            viewer.NavigationVisible = false;
            viewer.LayoutAlgorithmSettingsButtonVisible = false;
            viewer.SaveButtonVisible = false;
            viewer.SaveGraphButtonVisible = false;
            viewer.UndoRedoButtonsVisible = false;
            viewer.ToolBarIsVisible = true;
            this.SuspendLayout();
            this.Controls.Add(viewer);
            viewer.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form2_KeyPress);
            this.ResumeLayout();
            viewer.PanButtonPressed = true;
            viewer.LayoutEditingEnabled = false;
            viewer.Width = this.Width;
            viewer.Height = this.Height;
        }
        private void Form2_KeyPress(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Escape)
            {
                this.Hide();
            }
        }

        public void UpdateGraph(Microsoft.Msagl.Drawing.Graph graph)
        {

            viewer.Graph = graph;
            viewer.Update();
        }

        private void Form2_SizeChanged(object sender, EventArgs e)
        {
            viewer.Width = this.Width;
            viewer.Height = this.Height;
        }
    }
}
