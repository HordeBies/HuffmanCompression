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
    public partial class StaticHuffman : Form
    {
        public static HuffmanNode topNode;
        private static Dictionary<char, string> map = new Dictionary<char, string>();
        public static Microsoft.Msagl.Drawing.Graph graph;
        public static HuffmanListSorter sorter = new HuffmanListSorter();
        public static DataTable Dtable = new DataTable("Huffman Table");
        public static string encodedText;
        public static string decodedText;
        private static Microsoft.Msagl.GraphViewerGdi.GViewer viewer = new Microsoft.Msagl.GraphViewerGdi.GViewer();
        public StaticHuffman()
        {

            InitializeComponent();
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(0, 0);
            //this.WindowState = FormWindowState.Maximized;

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
            viewer.KeyDown += new System.Windows.Forms.KeyEventHandler(this.staticHuffman_KeyPress);
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
            graph = new Microsoft.Msagl.Drawing.Graph("Huffman Tree");
            viewer.Graph = graph;
            viewer.Update();
            Dtable.Clear();
            dataGridView1.Update();
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            label1.Text = "Compression Ratio:";

        }

        private string getNodeName(HuffmanNode node)
        {
            string str;
            if (node.IsLeaf) {
                str = " '" + node.ToString() + "'(" + node.Frequency.ToString() + ")\n" + node.getBit();
            }
            else
                str = node.ToString() + "\n" + node.getBit();

            return str;
        }
        private void createStaticGraph(HuffmanNode node)
        {
            graph.AddNode(getNodeName(node));
            if (node.IsLeaf) {
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
                createStaticGraph(node.RightChild);
            }
            if (node.LeftChild != null)
            {
                graph.AddEdge(getNodeName(node), getNodeName(node.LeftChild));
                createStaticGraph(node.LeftChild);
            }

        }

        private string Encode()
        {
            string encoded = "";
            foreach (char c in textBox3.Text.ToLower())
            {
                encoded += map[c];
            }
            return encoded;
        }

        private string Decode()
        {
            if (topNode == null || encodedText.Length <1)
                return "";
            HuffmanNode currKey;

            decodedText = "";
            currKey = topNode;
            foreach (char c in encodedText)
            {
                if (c == '1')
                    currKey = currKey.RightChild;
                else if (c == '0')
                    currKey = currKey.LeftChild;
                if (currKey.IsLeaf)
                {
                    decodedText += currKey.Char;
                    currKey = topNode;
                }
            }
            return decodedText;
        }
        private void CreateStaticHuffmanTree(object sender, EventArgs e)
        {
            graph = new Microsoft.Msagl.Drawing.Graph("Huffman Tree");
            Dtable.Clear();
            map = new Dictionary<char, string>();
            String text = textBox3.Text;
            textBox2.Text = "";
            if (text.Length < 1)
            {
                UpdateGraph(graph);
                textBox1.Text = "";
                encodedText = "";
                label1.Text = "Compression Ratio:";
                MessageBox.Show("Can't encode empty input ", "Encoding Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            var count2 = text.ToLower().Select(x => x).GroupBy(c => c).Select(chunk => new { Letter = chunk.Key, Count = chunk.Count() }).OrderByDescending(item => item.Count).ThenBy(item => item.Letter); // Language Integrated Query

            List<HuffmanNode> list = new List<HuffmanNode>();
            foreach (var c1 in count2)
            {
                //Console.WriteLine("Alphabet :" + c1.Letter + ", Count :" + c1.Count);
                list.Add(new HuffmanNode(c1.Count, c1.Letter));
            }
            list.Sort(sorter);

            while (list.Count > 1)
            {
                list.Add(new HuffmanNode(list[0], list[1]));
                list.RemoveAt(0);
                list.RemoveAt(0);
                list.Sort(sorter);
            }
            topNode = list[0];
            createStaticGraph(topNode);
            encodedText = Encode();
            double compressionRatio = 100.0- Math.Floor((double)encodedText.Length/ (double)(topNode.Frequency * 8)*100*100)/100;
            this.label1.Text = "Compression Ratio: "+compressionRatio.ToString()+"%";
            updateTextBox(encodedText);
            dataGridView1.Sort(dataGridView1.Columns[3], ListSortDirection.Ascending);
            UpdateGraph(graph);
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            int currPos = textBox3.SelectionStart;
            textBox3.Text = textBox3.Text.ToLower();
            textBox3.SelectionStart = currPos;
        }

        public void updateTextBox(string str)
        {
            this.textBox1.Text = str;
        }

        public void UpdateGraph(Microsoft.Msagl.Drawing.Graph graph)
        {

            viewer.Graph = graph;
            viewer.Update();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
        private void staticHuffman_KeyPress(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Hide();
            }
        }

        private void DecodeEncodedData(object sender, EventArgs e)
        {
            if (topNode == null || encodedText.Length < 1)
            {
                MessageBox.Show("Can't decode empty input or not encoded text", "Decoding Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            textBox2.Text = Decode();

        }
    }
}
