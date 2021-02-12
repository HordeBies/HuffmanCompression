using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
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
        SortedDictionary<int, HuffmanNode> EncodeNodeList = new SortedDictionary<int, HuffmanNode>();
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
            dataGridView1.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;


            EncodeNodeList.Add(nodeZero.Depth, nodeZero);
        }

        public void ResizeGUI()
        {
            textBox3.Width = (this.Width - 30) / 2;
            textBox3.Height = ((this.Height-36)/2) -25;

            button1.Location =new Point(12, 18+textBox3.Height);
            button3.Location = new Point(18 + button1.Width, button1.Location.Y);

            textBox1.Location = new Point(12, button1.Location.Y + button1.Height + 6);
            textBox1.Height = ((this.Height - 36) / 2) - 25;
            textBox1.Width = ((this.Width - 36) / 2) - this.Width / 20;

            checkBox1.Location = new Point(6 + button3.Location.X + button3.Width, 24 + textBox3.Height);
            checkBox2.Location = new Point(6 + button3.Location.X + button3.Width, textBox1.Location.Y - checkBox2.Height -12);
            checkBox3.Location = new Point(6 + checkBox2.Location.X + checkBox2.Width, 15+textBox3.Height);
            label2.Location = new Point(6 + checkBox2.Location.X + checkBox2.Width, textBox1.Location.Y - label2.Height - 3);
            label1.Location = new Point(6 + label2.Location.X + label2.Width, button1.Location.Y);
            
            dataGridView1.Location = new Point(18 + textBox1.Width, textBox1.Location.Y);
            dataGridView1.Height = textBox1.Height;
            dataGridView1.Width = this.Width / 10;

            textBox2.Location = new Point(6+dataGridView1.Location.X + dataGridView1.Width, dataGridView1.Location.Y);
            textBox2.Height = textBox1.Height;
            textBox2.Width = textBox1.Width;

            button2.Location = new Point((6 + label1.Location.X + label1.Width < textBox2.Location.X ? textBox2.Location.X + 6 : 6 + label1.Location.X + label1.Width), label1.Location.Y);

            viewer.Location = new Point(textBox3.Width + 18, 12);
            viewer.Size = textBox3.Size;
           
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
            EncodeNodeList = new SortedDictionary<int, HuffmanNode>();
            EncodeNodeList.Add(nodeZero.Depth, nodeZero);
            firstReadOfChar = new List<char>();
            dynamicMap = new Dictionary<char, HuffmanNode>();
        }

        HuffmanNode DecodingRootNode;
        private void DecodeEncodedData()
        {
            if(firstReadOfChar.Count < 1)
            {
                MessageBox.Show("Can't decode empty input or not encoded data", "Decoding Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            decodedText = "";
            int cfri = 0; //charfirstreadindex
            SortedDictionary<int, HuffmanNode> DecodeNodeList = new SortedDictionary<int, HuffmanNode>();
            HuffmanNode decodingNodeZero = new HuffmanNode();
            HuffmanNode leafNode = new HuffmanNode(1, firstReadOfChar[cfri], 0);
            DecodingRootNode = new HuffmanNode(decodingNodeZero, leafNode, decodingNodeZero.Depth);
            DecodeNodeList.Add(decodingNodeZero.Depth, decodingNodeZero);
            DecodeNodeList.Add(leafNode.Depth, leafNode);
            DecodeNodeList.Add(DecodingRootNode.Depth, DecodingRootNode);
            decodedText += firstReadOfChar[cfri];
            HuffmanNode traverse = DecodingRootNode;
            cfri++;
            foreach (char c in encodedText)
            {
                if(c == '1')
                    traverse = traverse.RightChild;
                else if(c == '0')
                    traverse = traverse.LeftChild;
                
                if(traverse == decodingNodeZero)
                {
                    decodedText += firstReadOfChar[cfri];
                    leafNode = new HuffmanNode(1, firstReadOfChar[cfri], 0);
                    HuffmanNode newNode = new HuffmanNode(decodingNodeZero, leafNode, decodingNodeZero.Depth);
                    DecodeNodeList.Add(leafNode.Depth, leafNode);
                    DecodeNodeList[newNode.Depth] = newNode;
                    DecodeNodeList.Add(decodingNodeZero.Depth, decodingNodeZero);
                    cfri++;
                    traverse = DecodingRootNode;
                    updateGraph(DecodeNodeList);
                }
                if (traverse.IsLeaf)
                {
                    decodedText += traverse.Char;
                    traverse.Frequency++;
                    traverse = DecodingRootNode;
                    updateGraph(DecodeNodeList);
                }
            }

            textBox2.Text = decodedText;
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            int currPos = textBox3.SelectionStart;
            textBox3.Text = textBox3.Text.ToLower();
            textBox3.SelectionStart = currPos;
            if (checkBox2.Checked)
            {
                if (textBox3.Text.StartsWith(mainInput))
                    CreateDynamicHuffmanTree(textBox3.Text.Substring(mainInput.Length));
                else if (textBox3.Text == "")
                {
                    cleanInit();
                }
                else
                {
                    MessageBox.Show("Program currently doesnt allow correction.\nPlease use clear tree", "Real Time Encoding Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBox3.Text = mainInput;
                    textBox3.SelectionStart = mainInput.Length;
                }
                mainInput = textBox3.Text;
            }
        }

        public bool ALT_F4 = false;
        private void dynamicHuffman_KeyPress(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.F4) && e.Alt == true)
                ALT_F4 = true;

            if (e.KeyCode == Keys.Escape)
            {
                this.Hide();
                Program.mainMEnu.BringToFront();
            }
        }


        private string getNodeName(HuffmanNode node)
        {
            string str;
            if (node.IsLeaf)
            {
                str = " '" + node.ToString() + "'(" + node.Depth.ToString() + "," + node.Frequency + ")\n" + node.getBit();
            }
            else if (node.LeftChild == null && node.RightChild == null)
            {
                return "ZeroNode(" + node.Depth + ")";
            }
            else
                str = node.ToString() + "(" + node.Depth + ")" + "\n" + node.getBit();

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
                    HuffmanNode newNode = new HuffmanNode(nodeZero, leafNode, nodeZero.Depth);
                    EncodeNodeList.Add(leafNode.Depth, leafNode);
                    EncodeNodeList[newNode.Depth] = newNode;
                    EncodeNodeList.Add(nodeZero.Depth, nodeZero);

                    updateGraph(EncodeNodeList);
                }
                else
                {
                    encodedText += dynamicMap[c].getBit();
                    dynamicMap[c].Frequency++;
                    updateGraph(EncodeNodeList);
                }
            }

            textBox1.Text = encodedText;
            double compressionRatio = 100.0 - Math.Floor(((double)encodedText.Length) / (double)(EncodeNodeList[256].Frequency * 8) * 100 * 100) / 100;
            this.label1.Text = "Compression Ratio: " + compressionRatio.ToString() + "%";
            

            if (!checkBox3.Checked) {
                Dtable.Rows.Clear();
                createDynamicGraph(EncodeNodeList[256]);
                viewer.Graph = graph;
                dataGridView1.Sort(dataGridView1.Columns[3], ListSortDirection.Ascending);
                dataGridView1.Update();
                viewer.Update();
            }
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
            while (queue.Count > 0)
            {
                currNode = queue.Dequeue();
                currNode.Depth = depthIndex;
                depthIndex--;
                if (currNode.LeftChild != null && currNode.RightChild != null)
                {
                    queue.Enqueue(currNode.RightChild);
                    queue.Enqueue(currNode.LeftChild);
                }
            }
            updateFreqs(root);
        }
        private void Swap(HuffmanNode node1, HuffmanNode node2, int i, int j , SortedDictionary<int, HuffmanNode> nodeList) //parent(will traverse), givenNode
        {
            //Console.WriteLine("Swapped\n"+getNodeName(node1)+"\nwith\n"+getNodeName(node2)+"\n");
            if(HuffmanNode.swap(node1, node2)) { 
            HuffmanNode temp = nodeList[i];
            nodeList[i] = nodeList[j];
            nodeList[j] = temp;
            }
        }
        private void updateGraph(SortedDictionary<int,HuffmanNode> nodeList)
        {
            for (int i = 256; i > 256 - nodeList.Count(); i--)
            {
                HuffmanNode selectedNode = nodeList[i];

                for (int j = i - 1; j > 256 - nodeList.Count() && selectedNode.IsLeaf; j--)
                {
                    if (nodeList[j].Frequency != selectedNode.Frequency)
                        continue;
                    if (!nodeList[j].IsLeaf)
                    {
                        int t = j;
                        while (t < i)
                        {
                            Swap(nodeList[t], nodeList[t + 1], t, t + 1 , nodeList);
                            t++;
                        }
                    }

                }
                int k = i;
                while (k < 256 && nodeList[k].Frequency > nodeList[k + 1].Frequency)
                {
                    Swap(nodeList[k], nodeList[k + 1], k, k + 1 , nodeList);
                    k++;
                }
            }
            updateDepths(nodeList[256]);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                MessageBox.Show("Cannot manually update tree in real time encoding", "Manual Encoding Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            CreateDynamicHuffmanTree(textBox3.Text);
            mainInput += textBox3.Text;
            if (checkBox1.Checked)
                textBox3.Text = "";
        }

        private void checkBox1_Click(object sender, EventArgs e)
        {
            if (checkBox2.Checked && checkBox1.Checked)
            {
                checkBox2.Checked = false;
                checkBox2.CheckState = CheckState.Unchecked;
                MessageBox.Show("Create Tree in Real Time unchecked", "Input Updating Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            //cleanInit();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            cleanInit();
        }

        private void checkBox2_Click(object sender, EventArgs e)
        {
            if (checkBox1.Checked && checkBox2.Checked)
            {
                checkBox1.Checked = false;
                checkBox1.CheckState = CheckState.Unchecked;
                MessageBox.Show("Clear input after updating box unchecked", "Real Time Encoding Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            cleanInit();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DecodeEncodedData();
        }

        private void checkBox3_Click(object sender, EventArgs e)
        {
            if (checkBox3.Checked)
            {
                Dtable.Rows.Clear();
                graph = new Microsoft.Msagl.Drawing.Graph("Dynamic Huffman Tree");
                viewer.Graph = graph;
                viewer.Update();
                dataGridView1.Update();
            }
            else
            {
                Dtable.Rows.Clear();
                createDynamicGraph(EncodeNodeList[256]);
                viewer.Graph = graph;
                dataGridView1.Sort(dataGridView1.Columns[3], ListSortDirection.Ascending);
                dataGridView1.Update();
                viewer.Update();
            }
        }

        private void DynamicHuffman_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (ALT_F4)
            {
                e.Cancel = true;
                this.Hide();
                return;
            }
        }
    }
}
