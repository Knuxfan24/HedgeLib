using HedgeLib.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HedgeLib.Models
{
    public class NNFooterList : FileBase
    {
        public List<uint> Offsets = new List<uint>();
        public override void Load(Stream fileStream)
        {
            ExtendedBinaryReader reader = new ExtendedBinaryReader(fileStream) { Offset = 0x20};
            reader.JumpAhead(16);
            reader.JumpTo(reader.ReadUInt32(), false);
            reader.JumpAhead(8);
            var offsetCount = reader.ReadUInt32();
            reader.JumpAhead(4);
            for(int i = 0; i < offsetCount; i++)
            {
                Offsets.Add(reader.ReadUInt32());
                Offsets[i] = Offsets[i] + 0x20;
            }
        }
    }
}
