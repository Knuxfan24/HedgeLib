using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HedgeLib.Models;
using System.IO;
using HedgeLib.Havok;

namespace HedgeTest
{
    class Program
    {
        static void Main(string[] args)
        {
            HavokFile test = new HavokFile();
            test.Load(@"G:\Sonic '06\Extracted Files\xenon\object\xenon\object\wvo\parasol\wvo_obj_parasolA.hkx");
            //List<string> nodeNames = new List<string>();
            //nodeNames.Add("GN * Entries:");
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
            //SegaNNObject xno = new SegaNNObject();

            //Sonic '06
            //xno.Load(@"G:\Sonic '06\Extracted Files\win32\enemy_data\win32\enemy\secondmefiress\en_Kyozoress.xno");
            //xno.Load(@"G:\Sonic '06\Extracted Files\win32\player_sonic\win32\player\sonic_new\so_itm_sbungle_L.xno");
            //xno.Load(@"C:\Users\Knuxf\AppData\Local\Hyper_Development_Team\Sonic '06 Toolkit\Archives\17553\wvjktqer.j0d\player_sonic\win32\player\sonic_new\sonic_Root.xno");

            //Sonic 4: Episode 1
            //xno.Load(@"D:\Steam\steamapps\common\Sonic the Hedgehog 4 EP 1\G_COM\PLY\SON_MDL\SON_MODEL.ZNO");

            //Sonic Riders
            //xno.Load(@"G:\v.xno");

            //Sonic Unwiished (GNO doesn't work right now)
            //xno.Load(@"G:\Sonic Unleashed\Wii\Game Dump\one_wii_lUS\load_main_game\common\Extracted Files\cmn_sv_super\Extracted Files\ssonic_always\Extracted Files\ssonic_model\Extracted Files\sonic_model_default\sonic.gno");
            //NNFooterList list = new NNFooterList();
            //list.Load(@"G:\Sonic '06\Extracted Files\win32\player_sonic\win32\player\sonic_new\so_itm_sbungle_L.xno");
            //Console.WriteLine($"{list.Offsets.Count} Offsets in Footer, pointing to:");
            //foreach(var entry in list.Offsets)
            //{
            //    Console.WriteLine("{0:X}", entry);
            //}
            //Console.WriteLine("\n(0x20 offset already accounted for!");
            //Console.ReadKey();
        }
    }
}
