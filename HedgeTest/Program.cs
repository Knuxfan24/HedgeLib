using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HedgeLib.Models;

namespace HedgeTest
{
    class Program
    {
        static void Main(string[] args)
        {
            SegaNNObject xno = new SegaNNObject();
            xno.Load(@"G:\Sonic '06\Extracted Files\win32\enemy_data\win32\enemy\secondmefiress\en_Kyozoress.xno");
            foreach(var entry in xno.NodeTree.Nodes)
            {
                Console.WriteLine(entry.NodeName);
            }
            //xno.Load(@"G:\Sonic '06\Extracted Files\win32\player_sonic\win32\player\sonic_new\so_itm_sbungle_L.xno");
        }
    }
}
