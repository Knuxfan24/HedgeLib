using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HedgeLib.Models;
using System.IO;

namespace HedgeTest
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> nodeNames = new List<string>();
            //nodeNames.Add("GN* Entries:");
            //var files = Directory.GetFiles(@"G:\Sonic Unleashed\Wii\Game Dump", "*.gn*", SearchOption.AllDirectories);
            //foreach (var file in files)
            //{
            //    if (!file.Contains("ncp"))
            //    {
            //        Console.WriteLine(file);
            //        SegaNNLister xno = new SegaNNLister();
            //        xno.Load(file);
            //        foreach(var entry in xno.nodeNames)
            //        {
            //            if (!nodeNames.Contains(entry))
            //            {
            //                nodeNames.Add(entry);
            //            }
            //        }
            //    }
            //}

            //nodeNames.Add("\nZN* Entries:");
            //files = Directory.GetFiles(@"G:\Sonic 4\Episode 1\AMBs", "*.zn*", SearchOption.AllDirectories);
            //foreach (var file in files)
            //{
            //    if (!file.Contains("ncp"))
            //    {
            //        Console.WriteLine(file);
            //        SegaNNLister xno = new SegaNNLister();
            //        xno.Load(file);
            //        foreach (var entry in xno.nodeNames)
            //        {
            //            if (!nodeNames.Contains(entry))
            //            {
            //                nodeNames.Add(entry);
            //            }
            //        }
            //    }
            //}

            //nodeNames.Add("\nXN* Entries:");
            //files = Directory.GetFiles(@"G:\Sonic '06\Extracted Files", "*.xn*", SearchOption.AllDirectories);
            //foreach (var file in files)
            //{
            //    if (!file.Contains("ncp"))
            //    {
            //        Console.WriteLine(file);
            //        SegaNNLister xno = new SegaNNLister();
            //        xno.Load(file);
            //        foreach (var entry in xno.nodeNames)
            //        {
            //            if (!nodeNames.Contains(entry))
            //            {
            //                nodeNames.Add(entry);
            //            }
            //        }
            //    }
            //}

            //var files = Directory.GetFiles(@"G:\Sonic '06\Extracted Files", "*.xn*", SearchOption.AllDirectories);
            //foreach (var file in files)
            //{
            //    if (!file.Contains("ncp"))
            //    {
            //        Console.WriteLine(file);
            //        SegaNNObject xno = new SegaNNObject();
            //        xno.Load(file);
            //        if (xno.EffectList != null)
            //        {
            //            foreach (var entry in xno.EffectList.Techs)
            //            {
            //                if (!nodeNames.Contains(entry.Filename))
            //                {
            //                    nodeNames.Add(entry.Filename);
            //                }
            //            }
            //        }
            //    }
            //}


            //Console.Clear();
            //nodeNames.Sort();
            //foreach (var entry in nodeNames)
            //{
            //    Console.WriteLine(entry);
            //}
            SegaNNObject xno = new SegaNNObject();
            //xno.Load(@"G:\Sonic '06\Extracted Files\win32\enemy_data\win32\enemy\secondmefiress\en_Kyozoress.xno");
            //xno.Load(@"G:\Sonic '06\Extracted Files\win32\player_sonic\win32\player\sonic_new\so_itm_sbungle_L.xno");
            xno.Load(@"D:\Steam\steamapps\common\Sonic the Hedgehog 4 EP 1\G_COM\PLY\SON_MDL\SON_MODEL.ZNO");
        }
    }
}
