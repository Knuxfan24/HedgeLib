using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HedgeLib.Sets;
using System.IO;
using HedgeLib.Text;
using HedgeLib.Misc;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            CommonBIN common = new CommonBIN();
            common.Load(@"C:\Users\Knuxf\AppData\Local\Hyper_Development_Team\Sonic '06 Toolkit\Archives\75981\c0m0521e.y10\object\xenon\object\Common - copy.bin");

            //S06Props prop = new S06Props();
            //prop.Load(@"C:\Users\Knuxf\AppData\Local\Hyper_Development_Team\Sonic '06 Toolkit\Archives\59666\pwtrmqr5.dco\game\xenon\actor_aquaticbase.prop");
            /*var props = Directory.GetFiles(@"C:\Users\Knuxf\AppData\Local\Hyper_Development_Team\Sonic '06 Toolkit\Archives\59666\pwtrmqr5.dco\game\xenon\", "*.prop", SearchOption.TopDirectoryOnly);

            foreach (var propFile in props)
            {
                S06Props prop = new S06Props();
                prop.Load(propFile);
                Console.WriteLine($"{Path.GetDirectoryName(propFile)}\\{Path.GetFileNameWithoutExtension(propFile)}.xml");
                prop.ExportXML($"{Path.GetDirectoryName(propFile)}\\{Path.GetFileNameWithoutExtension(propFile)}.xml");
            }*/
        }
    }
}
