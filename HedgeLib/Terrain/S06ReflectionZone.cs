using HedgeLib.Headers;
using HedgeLib.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Policy;
using System.Xml.Linq;

namespace HedgeLib.Terrain
{
    public class S06RFZone
    {
        public float Unknown1;
        public float Unknown2;
        public float Unknown3;
        public float Unknown4;
    }
    public class S06RFEntry
    {
        public uint VertexCount;
        public List<Vector3> Verticies = new List<Vector3>();
    }
    public class S06ReflectionZone : FileBase
    {
        public BINAHeader Header = new BINAv1Header();
        public List<S06RFZone> Zones = new List<S06RFZone>();
        public List<S06RFEntry> Entries = new List<S06RFEntry>();
        public override void Load(Stream fileStream)
        {
            // Header
            var reader = new BINAReader(fileStream);
            Header = reader.ReadHeader();

            var rfzoneCount = reader.ReadUInt32();
            var rfzoneOffset = reader.ReadUInt32(); //Seemingly always 16? (0x10)
            var entriesCount = reader.ReadUInt32(); //Seemingly always the same as rfzoneCount?
            var entriesOffset = reader.ReadUInt32();

            for(int i = 0; i < rfzoneCount; i++)
            {
                S06RFZone rfZone = new S06RFZone();
                rfZone.Unknown1 = reader.ReadSingle();
                rfZone.Unknown2 = reader.ReadSingle();
                rfZone.Unknown3 = reader.ReadSingle();
                rfZone.Unknown4 = reader.ReadSingle();
                Zones.Add(rfZone);
            }

            for (int i = 0; i < entriesCount; i++)
            {
                S06RFEntry entry = new S06RFEntry();
                entry.VertexCount = reader.ReadUInt32();
                var vertexTableOffset = reader.ReadUInt32();
                var zoneIndex = reader.ReadUInt32();

                long pos = reader.BaseStream.Position;
                reader.JumpTo(vertexTableOffset, false);

                for (int v = 0; v < entry.VertexCount; v++)
                {
                    entry.Verticies.Add(reader.ReadVector3());
                }

                reader.JumpTo(pos, true);
                Entries.Add(entry);
            }
        }

        public void ExportXML(string filepath)
        {
            int entryIndex = 0;

            var rootElem = new XElement("RAB");
            foreach(var zone in Zones)
            {
                var zoneElem = new XElement("Zone");
                var zoneUnknown1Elem = new XElement("Unknown1", zone.Unknown1);
                var zoneUnknown2Elem = new XElement("Unknown2", zone.Unknown2);
                var zoneUnknown3Elem = new XElement("Unknown3", zone.Unknown3);
                var zoneUnknown4Elem = new XElement("Unknown4", zone.Unknown4);
                zoneElem.Add(zoneUnknown1Elem, zoneUnknown2Elem, zoneUnknown3Elem, zoneUnknown4Elem);

                var verticiesElem = new XElement("Verticies");
                for(int i = 0; i < Entries[entryIndex].VertexCount; i++)
                {
                    var vertexElem = new XElement($"Vertex{i}", Entries[entryIndex].Verticies[i]);
                    verticiesElem.Add(vertexElem);
                }
                entryIndex++;

                zoneElem.Add(verticiesElem);
                rootElem.Add(zoneElem);
            }

            var xml = new XDocument(rootElem);
            xml.Save(filepath);
        }
    }
}
