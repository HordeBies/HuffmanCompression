using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    static class Program
    {
        public static MainMenu mainMEnu;
        public static StaticHuffman staticHuffman;
        public static DynamicHuffman dynamicHuffman;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            mainMEnu = new MainMenu();
            staticHuffman = new StaticHuffman();
            dynamicHuffman = new DynamicHuffman();
            Application.Run(mainMEnu);
        }
    }
}
