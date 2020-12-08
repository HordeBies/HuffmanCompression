using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    public class HuffmanListSorter : IComparer<HuffmanNode>
    {
        public int Compare(HuffmanNode x, HuffmanNode y)
        {
            if (x.Frequency > y.Frequency)
                return 1;
            else if (x.Frequency < y.Frequency)
                return -1;
            else
                return 0;
        }
    }
}
