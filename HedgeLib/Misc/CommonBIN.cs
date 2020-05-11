using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HedgeLib.Headers;
using HedgeLib.IO;

namespace HedgeLib.Misc
{
    public class CommonBIN : FileBase
    {
        public BINAHeader Header = new BINAv1Header();
        public override void Load(Stream fileStream)
        {
            // Header
            var reader = new BINAReader(fileStream);
            Header = reader.ReadHeader();

            while (reader.BaseStream.Position != 42020)
            {
                uint value1 = reader.ReadUInt32();
                uint value2 = reader.ReadUInt32();
                uint value3 = reader.ReadUInt32();
                uint value4 = reader.ReadUInt32();
                uint value5 = reader.ReadUInt32();
                uint value6 = reader.ReadUInt32();
                uint value7 = reader.ReadUInt32();
                uint value8 = reader.ReadUInt32();
                uint value9 = reader.ReadUInt32();
                uint value10 = reader.ReadUInt32();
                uint value11 = reader.ReadUInt32();
                uint value12 = reader.ReadUInt32();
                uint value13 = reader.ReadUInt32();
                uint value14 = reader.ReadUInt32();
                uint value15 = reader.ReadUInt32();
                uint value16 = reader.ReadUInt32();
                uint value17 = reader.ReadUInt32();
                uint value18 = reader.ReadUInt32();
                uint value19 = reader.ReadUInt32();
                uint value20 = reader.ReadUInt32();
                uint value21 = reader.ReadUInt32();
                uint value22 = reader.ReadUInt32();
                uint value23 = reader.ReadUInt32();
                uint value24 = reader.ReadUInt32();
            }
        }
    }
}
