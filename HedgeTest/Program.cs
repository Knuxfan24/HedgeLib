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
            xno.Load(@"C:\Users\gabe1\AppData\Local\Hyper_Development_Team\Sonic '06 Toolkit\Archives\78988\1z2ovl5b.2zp\player_sonic\win32\player\sonic_new\so_itm_sbungle_L.xno");
        }
    }
}
