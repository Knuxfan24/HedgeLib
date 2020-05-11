using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using HedgeLib.Headers;
using HedgeLib.IO;

namespace HedgeLib.Misc
{
    public class PathObj : FileBase
    {
        public BINAHeader Header = new BINAv1Header();
        public PathObjEntry extractedData = new PathObjEntry();
        public override void Load(Stream fileStream)
        {
            // Header
            var reader = new BINAReader(fileStream);
            Header = reader.ReadHeader();

            int entriesInObj = 0;
            long nullPos = 0;
            List<string> objects = new List<string> { };
            //while (reader.ReadUInt32() != 0)
            while (reader.BaseStream.Position != 624)
            {
                //reader.JumpBehind(0x4);
                var valueOffset = reader.ReadUInt32();
                if (valueOffset != 0)
                {
                    long position = reader.BaseStream.Position;
                    reader.JumpTo(valueOffset, false);
                    objects.Add(reader.ReadNullTerminatedString());
                    reader.JumpTo(position, true);
                }
                else
                {
                    PathObjEntryStrings entry = new PathObjEntryStrings();
                    foreach(var value in objects)
                    {
                        entry.ObjectValues.Add(value);
                    }
                    objects.Clear();
                    extractedData.Objects.Add(entry);
                    nullPos = reader.BaseStream.Position;
                }
                entriesInObj++;
            }
        }

        public void ExportXML(string filepath)
        {
            var rootElem = new XElement("PathObj");
            foreach(var obj in extractedData.Objects)
            {
                var objectElem = new XElement("Object");
                int value = 0;

                foreach(var entry in obj.ObjectValues)
                {
                    var entryName = new XElement($"Value{value}", entry);
                    value++;
                    objectElem.Add(entryName);
                }

                rootElem.Add(objectElem);
            }

            var xml = new XDocument(rootElem);
            xml.Save(filepath);
        }
    }
}
