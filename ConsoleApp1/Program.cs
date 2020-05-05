using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HedgeLib.Sets;
using System.IO;
using HedgeLib.Text;

namespace ConsoleApp1
{
    /*class Program
    {
        static void Main(string[] args)
        {
            MST mst = new MST();
            mst.Load(@"C:\Users\Knuxf\AppData\Local\Hyper_Development_Team\Sonic '06 Toolkit\Archives\72300\4au4zd2f.ih2\text\xenon\text\english\msg_hint_xenon.e.mst");
            foreach(var entry in mst.entries)
            {
                Console.WriteLine(entry.Text);
            }
            mst.ExportXML();
        }
    }*/
    class Program
    {
        static void Main(string[] args)
        {
            S06SetData controlset = new S06SetData();
            controlset.Load(@"C:\Users\Knuxf\AppData\Local\Hyper_Development_Team\Sonic '06 Toolkit\Archives\95334\uogwixin.l3e\scripts\xenon\placement\wvo\set_wvoA_sonic.set");
            controlset.ExportXML(@"Z:\test.xml");
            controlset.Save(@"Z:\control.set", true);

            S06SetData set = new S06SetData();
            set.Load(@"Z:\\control.set");
            set.ExportXML(@"Z:\test2.xml");
            //set.Save(@"Z:\test_xml.set", true);
        }
    }
}
