using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class DynamicHuffman : Form
    {
        private static Dictionary<char, string> map = new Dictionary<char, string>();
        public static Microsoft.Msagl.Drawing.Graph graph = new Microsoft.Msagl.Drawing.Graph("Huffman Tree");
        public static DataTable Dtable = new DataTable("Huffman Table");
        public static string mainInput = "";
        public static string encodedText = "";
        public static string decodedText = "";
        private static Microsoft.Msagl.GraphViewerGdi.GViewer viewer = new Microsoft.Msagl.GraphViewerGdi.GViewer();
        HuffmanNode nodeZero = new HuffmanNode();
        SortedDictionary<int, HuffmanNode> nodeList = new SortedDictionary<int, HuffmanNode>();
        List<char> firstReadOfChar = new List<char>();
        Dictionary<char, HuffmanNode> dynamicMap = new Dictionary<char, HuffmanNode>();
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
            dataGridView1.ReadOnly = true;
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

            
            nodeList.Add(nodeZero.Depth, nodeZero);
        }

        public void cleanInit()
        {
            graph = new Microsoft.Msagl.Drawing.Graph("Huffman Tree");
            viewer.Graph = graph;
            viewer.Update();
            Dtable.Clear();
            dataGridView1.Update();
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            mainInput = "";
            encodedText = "";
            decodedText = "";
            label1.Text = "Compression Ratio:";
            nodeZero = new HuffmanNode();
            nodeList = new SortedDictionary<int, HuffmanNode>();
            firstReadOfChar = new List<char>();
            dynamicMap = new Dictionary<char, HuffmanNode>();
        }

        private void DecodeEncodedData(object sender, EventArgs e)
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
                str = " '" + node.ToString() + "'(" + node.Depth.ToString() + ","+node.Frequency+ ")\n" + node.getBit();
            }else if(node.LeftChild == null && node.RightChild == null)
            {
                return "ZeroNode("+node.Depth+")";
            }
            else
                str = node.ToString()+"("+node.Depth+")" + "\n" + node.getBit();

            return str;
        }   
        private void createDynamicGraph(HuffmanNode node)
        {
            map = new Dictionary<char, string>();

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


        private void CreateDynamicHuffmanTree(string text)
        {
            graph = new Microsoft.Msagl.Drawing.Graph("Huffman Tree");

            foreach (char c in text)
            {
                if (!firstReadOfChar.Contains(c))
                {
                    //encodedText += nodeZero.getBit() + c;
                    encodedText += nodeZero.getBit();
                    firstReadOfChar.Add(c);
                    HuffmanNode leafNode = new HuffmanNode(1, c, 0);
                    dynamicMap[c] = leafNode;
                    HuffmanNode newNode = new HuffmanNode(nodeZero, leafNode ,nodeZero.Depth);
                    nodeList.Add(leafNode.Depth, leafNode);
                    nodeList[newNode.Depth] = newNode;
                    nodeList.Add(nodeZero.Depth, nodeZero);

                    updateGraph();
                }
                else
                {
                    encodedText += dynamicMap[c].getBit();
                    dynamicMap[c].Frequency++;
                    updateGraph();
                }
            }

            textBox1.Text = encodedText;
            double compressionRatio = 100.0-Math.Floor((double)encodedText.Length / (double)(nodeList[256].Frequency * 8) * 100 * 100) / 100;
            this.label1.Text = "Compression Ratio: " + compressionRatio.ToString() + "%";
            Dtable.Rows.Clear();

            createDynamicGraph(nodeList[256]);
            viewer.Graph = graph;
            dataGridView1.Sort(dataGridView1.Columns[3], ListSortDirection.Ascending);
            dataGridView1.Update();
            viewer.Update();
        }

        private void updateFreqs(HuffmanNode root)
        {
            if (root.LeftChild != null)
                updateFreqs(root.LeftChild);
            if (root.RightChild != null)
                updateFreqs(root.RightChild);

            if (root.LeftChild == null && root.RightChild == null)
                return;
            root.Frequency = root.LeftChild.Frequency + root.RightChild.Frequency;
        }
        private void updateDepths(HuffmanNode root)
        {
            Queue<HuffmanNode> queue = new Queue<HuffmanNode>();
            int depthIndex = 256;
            queue.Enqueue(root);
            HuffmanNode currNode;
            while (queue.Count >0)
            {
                currNode = queue.Dequeue();
                currNode.Depth = depthIndex;
                depthIndex--;
                if(currNode.LeftChild != null && currNode.RightChild != null)
                {
                    queue.Enqueue(currNode.RightChild);
                    queue.Enqueue(currNode.LeftChild);
                }
            }
            updateFreqs(root);
        }
        private void Swap(HuffmanNode node1, HuffmanNode node2, int i , int j) //parent(will traverse), givenNode
        {
            //Console.WriteLine("Swapped\n"+getNodeName(node1)+"\nwith\n"+getNodeName(node2)+"\n");
            HuffmanNode.swap(node1, node2);
            HuffmanNode temp = nodeList[i];
            nodeList[i] = nodeList[j];
            nodeList[j] = temp;
        }
        private void updateGraph()
        {
            for (int i = 256; i>256-nodeList.Count();i--)
            {
                HuffmanNode selectedNode = nodeList[i];

                for (int j = i - 1; j > 256 - nodeList.Count() && selectedNode.IsLeaf ; j--)
                {
                    if (nodeList[j].Frequency != selectedNode.Frequency)
                        continue;
                    if (!nodeList[j].IsLeaf)
                    {
                        int t = j;
                        while(t < i)
                        {
                            Swap(nodeList[t], nodeList[t+1], t, t+1);
                            t++;
                        }
                    }
                        
                }
                int k = i;
                while (k < 256 && nodeList[k].Frequency > nodeList[k + 1].Frequency)
                {
                    Swap(nodeList[k], nodeList[k + 1], k, k + 1);
                    k++;
                }
            }
            updateDepths(nodeList[256]);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            CreateDynamicHuffmanTree(textBox3.Text);
            mainInput += textBox3.Text;
            if (checkBox1.Checked)
                textBox3.Text = "";
        }

        private void checkBox1_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            graph = new Microsoft.Msagl.Drawing.Graph("Huffman Tree");
            viewer.Graph = graph;
            viewer.Update();
            Dtable.Clear();
            dataGridView1.Update();
            textBox1.Text = "";
            textBox2.Text = "";
            mainInput = "";
            encodedText = "";
            decodedText = "";
            label1.Text = "Compression Ratio:";
            nodeZero = new HuffmanNode();
            nodeList = new SortedDictionary<int, HuffmanNode>();
            firstReadOfChar = new List<char>();
            dynamicMap = new Dictionary<char, HuffmanNode>();
        }
    }
}
