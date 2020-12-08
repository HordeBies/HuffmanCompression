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
        public static HuffmanNode topNode;
        private static StaticTree visualization = new StaticTree();
        private static StaticData convertedData = new StaticData();
        private static Dictionary<char,string> map = new Dictionary<char, string>();
        public static Microsoft.Msagl.Drawing.Graph graph;
        public static HuffmanListSorter sorter = new HuffmanListSorter();
        public static DataTable Dtable = new DataTable("Huffman Table");
        public static string encodedText;
        public MainMenu()
        {
            InitializeComponent();
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

        private string getNodeName(HuffmanNode node)
        {
            string str;
            if (node.IsLeaf) { 
                str = " '"+node.ToString() + "'(" + node.Frequency.ToString() + ")\n" + node.getBit();
            }
            else
                str = node.ToString() + "\n" + node.getBit();

            return str;
        }
        private void createGraph(HuffmanNode node)
        {
            graph.AddNode(getNodeName(node));
            if (node.IsLeaf) { 
                graph.FindNode(getNodeName(node)).Attr.Color = new Microsoft.Msagl.Drawing.Color(255, 0, 0);
                DataRow row = Dtable.NewRow();
                row[0] = "'"+node.Char.ToString()+"'";
                row[1] = node.Frequency;
                row[2] = node.getBit();
                row[3] = node.getBit().Length.ToString();
                Dtable.Rows.Add(row);
                map.Add(node.Char, node.getBit());
            }
            if (node.RightChild != null)
            {
                graph.AddEdge(getNodeName(node), getNodeName(node.RightChild));
                createGraph(node.RightChild);
            }
            if (node.LeftChild != null)
            {
                graph.AddEdge(getNodeName(node), getNodeName(node.LeftChild));
                createGraph(node.LeftChild);
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

        private void OpenForm(object sender, EventArgs e)
        {
           
            visualization.StartPosition = FormStartPosition.Manual;
            visualization.SetDesktopLocation(this.DesktopLocation.X+this.Width-14, this.DesktopLocation.Y);
            visualization.Show();
            convertedData.StartPosition = FormStartPosition.Manual;
            convertedData.SetDesktopLocation(visualization.DesktopLocation.X + visualization.Width - 14, visualization.DesktopLocation.Y);
            convertedData.Height = visualization.Height;
            convertedData.Width = visualization.Width;
            convertedData.Show();
        }

        private void CreateHuffmanTree(object sender, EventArgs e)
        {
            graph = new Microsoft.Msagl.Drawing.Graph("Huffman Tree");
            Dtable.Clear();
            map = new Dictionary<char, string>();
            String text = textBox3.Text;
            if (text.Length < 1)
            {
                visualization.UpdateGraph(graph);
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
            createGraph(topNode);
            encodedText = Encode();
            double compressionRatio = Math.Floor((double)encodedText.Length/ (double)(topNode.Frequency * 8)*100*100)/100;
            this.label1.Text = "Compression Ratio: "+compressionRatio.ToString()+"%";
            convertedData.updateTextBox(encodedText);
            dataGridView1.Sort(dataGridView1.Columns[3], ListSortDirection.Ascending);
            visualization.UpdateGraph(graph);
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
