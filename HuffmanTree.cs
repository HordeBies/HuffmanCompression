using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    public class HuffmanNode
    {
        private HuffmanNode parentNode;
        private HuffmanNode leftChild;
        private HuffmanNode rightChild;
        private bool isLeaf;
        private int freq;
        private char ch;
        public HuffmanNode(int freq, char ch)
        {
            this.parentNode = null;
            this.leftChild = null;
            this.rightChild = null;
            this.isLeaf = true;
            this.freq = freq;
            this.ch = ch;
        }
        public HuffmanNode(HuffmanNode leftChild, HuffmanNode rightChild)
        {
            this.parentNode = null;
            this.leftChild = leftChild;
            this.leftChild.parentNode = this;
            this.rightChild = rightChild;
            this.rightChild.parentNode = this;
            this.isLeaf = false;
            this.freq = this.leftChild.Frequency + this.rightChild.Frequency;

        }
        public int Frequency
        {
            get
            {
                return this.freq;
            }
        }
        public char Char
        {
            get
            {
                return this.ch;
            }
        }
        public Boolean IsLeaf
        {
            get
            {
                return this.isLeaf;
            }
        }
        public HuffmanNode LeftChild
        {
            get
            {
                return this.leftChild;
            }
        }
        public HuffmanNode RightChild
        {
            get
            {
                return this.rightChild;
            }
        }
        public string getBit()
        {
            return parentNode == null ? "" : (parentNode.leftChild == this ? parentNode.getBit() + "0" : parentNode.getBit() + "1");
        }
        public override string ToString()
        {
            return isLeaf ? ch.ToString() : freq.ToString();   
        }
    }
}
