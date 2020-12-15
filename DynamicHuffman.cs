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
    public partial class DynamicHuffman : Form
    {

        public static HuffmanNode topNode = new HuffmanNode();
        private static Dictionary<char, string> map = new Dictionary<char, string>();
        public static Microsoft.Msagl.Drawing.Graph graph;
        public static DataTable Dtable = new DataTable("Huffman Table");
        public static string encodedText;
        public static string decodedText;
        private static Microsoft.Msagl.GraphViewerGdi.GViewer viewer = new Microsoft.Msagl.GraphViewerGdi.GViewer();
        public DynamicHuffman()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(0, 0);

            viewer.Dock = System.Windows.Forms.DockStyle.None;
            viewer.Location = new Point(972, 12);
            viewer.Size = new Size(948, 503);
            viewer.EdgeInsertButtonVisible = false;
            viewer.NavigationVisible = false;
            viewer.LayoutAlgorithmSettingsButtonVisible = false;
            viewer.SaveButtonVisible = false;
            viewer.SaveGraphButtonVisible = false;
            viewer.UndoRedoButtonsVisible = false;
            viewer.ToolBarIsVisible = true;
            viewer.PanButtonPressed = true;
            viewer.LayoutEditingEnabled = false;
            this.SuspendLayout();
            this.Controls.Add(viewer);
            viewer.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dynamicHuffman_KeyPress);
            this.ResumeLayout();

            Dtable.Columns.Add(new DataColumn("Char"));
            Dtable.Columns.Add(new DataColumn("Freq"));
            Dtable.Columns.Add(new DataColumn("Code"));
            Dtable.Columns.Add(new DataColumn("Bits"));

            dataGridView1.RowHeadersVisible = false;
            dataGridView1.ReadOnly = false;
            dataGridView1.AllowUserToResizeRows = false;
            dataGridView1.AutoGenerateColumns = true;
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.LightYellow;
            dataGridView1.EnableHeadersVisualStyles = false;
            dataGridView1.DataSource = Dtable;
            foreach (DataGridViewColumn col in dataGridView1.Columns)
            {
                col.ReadOnly = true;
                col.SortMode = DataGridViewColumnSortMode.NotSortable;
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }
        }

        public void cleanInit()
        {

        }

        private void DecodeEncodedData(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            int currPos = textBox3.SelectionStart;
            textBox3.Text = textBox3.Text.ToLower();
            textBox3.SelectionStart = currPos;
        }

        private void dynamicHuffman_KeyPress(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Hide();
            }
        }


        private string getNodeName(HuffmanNode node)
        {
            string str;
            if (node.IsLeaf)
            {
                str = " '" + node.ToString() + "'(" + node.Frequency.ToString() + ")\n" + node.getBit();
            }else if(node.LeftChild == null && node.RightChild == null)
            {
                return "ZeroNode";
            }
            else
                str = node.ToString() + "\n" + node.getBit();

            return str;
        }   
        private void createDynamicGraph(HuffmanNode node)
        {
            map = new Dictionary<char, string>();
            Dtable.Clear();

            graph.AddNode(getNodeName(node));
            if (node.IsLeaf)
            {
                graph.FindNode(getNodeName(node)).Attr.Color = new Microsoft.Msagl.Drawing.Color(255, 0, 0);
                DataRow row = Dtable.NewRow();
                row[0] = "'" + node.Char.ToString() + "'";
                row[1] = node.Frequency;
                row[2] = node.getBit();
                row[3] = node.getBit().Length.ToString();
                Dtable.Rows.Add(row);
                map.Add(node.Char, node.getBit());
            }
            if (node.RightChild != null)
            {
                graph.AddEdge(getNodeName(node), getNodeName(node.RightChild));
                createDynamicGraph(node.RightChild);
            }
            if (node.LeftChild != null)
            {
                graph.AddEdge(getNodeName(node), getNodeName(node.LeftChild));
                createDynamicGraph(node.LeftChild);
            }

        }

        private static List<HuffmanNode> leafNodeList;
        private void CreateDynamicHuffmanTree(object sender, EventArgs e)
        {
            graph = new Microsoft.Msagl.Drawing.Graph("Huffman Tree");
            List<char> firstReadOfChar = new List<char>();
            leafNodeList = new List<HuffmanNode>();
            encodedText = "";
            HuffmanNode nodeZero = new HuffmanNode();

            foreach(char c in textBox3.Text)
            {
                if (!firstReadOfChar.Contains(c))
                {
                    firstReadOfChar.Add(c);
                    HuffmanNode leafNode = new HuffmanNode(1, c, 0);
                    HuffmanNode newNode = new HuffmanNode(nodeZero, leafNode ,nodeZero.Depth);
                    leafNodeList.Add(leafNode);
                    updateGraph(newNode);
                }
                else
                {
                    foreach(HuffmanNode currNode in leafNodeList)
                    {
                        if(c == currNode.Char)
                        {
                            currNode.Frequency++;
                            updateGraph(currNode);
                            break;
                        }
                    }
                }
            }


            while (nodeZero.Parent != null)
                nodeZero = nodeZero.Parent;
            createDynamicGraph(nodeZero);
            viewer.Graph = graph;
            viewer.Update();
        }


        private bool nodeFound;
        private bool ReLocate(HuffmanNode currNode, HuffmanNode node) //parent(will traverse), givenNode
        {
            if(node.Frequency > currNode.Frequency && currNode.parentNode != null) { 
                HuffmanNode.swap(node,currNode);
                Console.WriteLine("swap occurred");
                while(node.parentNode != null)
                {
                    node = node.parentNode;
                    node.Frequency = node.LeftChild.Frequency + node.RightChild.Frequency;
                }
                while(currNode.parentNode != null)
                {
                    currNode = currNode.parentNode;
                    currNode.Frequency = currNode.LeftChild.Frequency + currNode.RightChild.Frequency;
                }
                return true;
            }
            if (currNode.RightChild != null && currNode.Depth + 1 <= node.Depth && !nodeFound)
            {
                if (currNode.RightChild.Char == node.Char && currNode.RightChild.Frequency == node.Frequency) { 
                    nodeFound = true;
                }else
                ReLocate(currNode.RightChild, node);
            }
            if (currNode.LeftChild != null && currNode.Depth+1 <= node.Depth && !nodeFound) {
                if (currNode.LeftChild.Equals(node)) { 
                    nodeFound = true;
                }
                else { 
                ReLocate(currNode.LeftChild, node);
                }
            }

            return false;

        }
        private void updateGraph(HuffmanNode node)
        {
            HuffmanNode currNode = node;

            while (currNode.Parent != null)
            {
                currNode = currNode.Parent;
            }

            bool isSwapped;
            do
            {
                isSwapped = false;
                for(int i = leafNodeList.Count-1;i>=0;i--)
                {
                    HuffmanNode nodeIx = leafNodeList[i];
                    while (currNode != nodeIx)
                    {
                        nodeFound = false;
                        if (ReLocate(currNode, nodeIx))//parent,newNode
                            isSwapped = true;
                        nodeIx = nodeIx.parentNode;
                        nodeIx.Frequency = nodeIx.LeftChild.Frequency + nodeIx.RightChild.Frequency;
                    }
                }
            } while (isSwapped);
        }
    }
}
