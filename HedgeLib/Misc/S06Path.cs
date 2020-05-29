using HedgeLib.Headers;
using HedgeLib.IO;
using System;
using System.Collections.Generic;
using System.IO;

namespace HedgeLib.Misc
{
    public class S06PathEntry
    {
        public uint SplineInfoOffset;
        public uint SplineCount;
        public uint Unknown1;
        public uint VertexDataOffset;
        public uint VertexCount;
        public uint Unknown2;
    }
    public class S06Path : FileBase
    {
        public BINAHeader Header = new BINAv1Header();
        public List<S06PathEntry> Paths = new List<S06PathEntry>();
        public override void Load(Stream fileStream)
        {
            // Header
            var reader = new BINAReader(fileStream);
            Header = reader.ReadHeader();

            var pathTableOffset = reader.ReadUInt32();
            var pathCount = reader.ReadUInt32();
            var nodeTableOffset = reader.ReadUInt32();
            var nodeCount = reader.ReadUInt32();

            for(int i = 0; i < pathCount; i++)
            {
                S06PathEntry pathEntry = new S06PathEntry();
                pathEntry.SplineInfoOffset = reader.ReadUInt32();
                pathEntry.SplineCount = reader.ReadUInt32();
                pathEntry.Unknown1 = reader.ReadUInt32();
                pathEntry.VertexDataOffset = reader.ReadUInt32();
                pathEntry.VertexCount = reader.ReadUInt32();
                pathEntry.Unknown2 = reader.ReadUInt32();
                Paths.Add(pathEntry);
            }

            for(int i = 0; i < Paths.Count; i++)
            {
                reader.JumpTo(Paths[i].VertexDataOffset, false);
                for(int v = 0; v < Paths[i].VertexCount; v++)
                {
                    Console.WriteLine($"Flag: {reader.ReadSingle()}");
                    Console.WriteLine($"xPos: {reader.ReadSingle()}");
                    Console.WriteLine($"yPos: {reader.ReadSingle()}");
                    Console.WriteLine($"zPos: {reader.ReadSingle()}");
                    Console.WriteLine($"invec_xPos: {reader.ReadSingle()}");
                    Console.WriteLine($"invec_yPos: {reader.ReadSingle()}");
                    Console.WriteLine($"invec_zPos: {reader.ReadSingle()}");
                    Console.WriteLine($"outvec_xPos: {reader.ReadSingle()}");
                    Console.WriteLine($"outvec_yPos: {reader.ReadSingle()}");
                    Console.WriteLine($"outvec_zPos: {reader.ReadSingle()}");
                }
            }
        }
    }
}
