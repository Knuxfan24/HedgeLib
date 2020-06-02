using HedgeLib.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HedgeLib.Models
{
    public class SegaNNLister : FileBase
    {
        public List<string> nodeNames = new List<string>();
        public override void Load(Stream fileStream)
        {
            ExtendedBinaryReader reader = new ExtendedBinaryReader(fileStream) { Offset = 0x20};
            var nodeName = new string(reader.ReadChars(4));
            nodeNames.Add(nodeName);
            var nodeLength = reader.ReadUInt32();
            if (nodeName == "NGIF")
            {
                reader.IsBigEndian = true;
            }
            var nodeCount = reader.ReadUInt32();
            reader.JumpAhead(4);
            var footerOffset = reader.ReadUInt32();
            if (nodeName == "NGIF")
            {
                reader.IsBigEndian = false;
            }
            reader.JumpTo(nodeLength + 8);

            for (int i = 0; i < nodeCount; i++)
            {
                nodeNames.Add(new string(reader.ReadChars(4)));
                nodeLength = reader.ReadUInt32();
                reader.JumpAhead(nodeLength);
            }

            reader.JumpTo(footerOffset, false);

            for(int i = 0; i < 2; i++)
            {
                nodeNames.Add(new string(reader.ReadChars(4)));
                nodeLength = reader.ReadUInt32();
                reader.JumpAhead(nodeLength);
            }
        }
    }
}
