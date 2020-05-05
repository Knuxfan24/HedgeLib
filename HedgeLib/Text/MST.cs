using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HedgeLib.IO;
using HedgeLib.Exceptions;
using HedgeLib.Headers;
using System.IO;
using System.Xml.Linq;

namespace HedgeLib.Text
{
    public class MST : FileBase
    {
        public BINAHeader Header = new BINAv1Header();
        public List<MSTEntries> entries = new List<MSTEntries>();

        public const string Signature = "WTXT", Extension = ".mst";

        // Methods
        public override void Load(Stream fileStream)
        {
            // Header
            var reader = new BINAReader(fileStream);
            Header = reader.ReadHeader();

            string sig = reader.ReadSignature(4);
            if (sig != Signature)
                throw new InvalidSignatureException(Signature, sig);

            uint messageTableOffset = reader.ReadUInt32();
            uint messageCount = reader.ReadUInt32();

            for(uint i = 0; i < messageCount; i++)
            {
                string name = string.Empty;
                string text = string.Empty;
                string placeholder = string.Empty;

                uint nameOffset = reader.ReadUInt32();
                uint textOffset = reader.ReadUInt32();
                uint placeholderOffset = reader.ReadUInt32();

                long pos = reader.BaseStream.Position;

                reader.JumpTo(nameOffset, false);
                name = reader.ReadNullTerminatedString();
                reader.JumpTo(textOffset, false);
                text = reader.ReadNullTerminatedStringUTF16();
                if (placeholderOffset != 0)
                {
                    reader.JumpTo(placeholderOffset, false);
                    placeholder = reader.ReadNullTerminatedString();
                }

                MSTEntries entry = new MSTEntries(name, text, placeholder);
                entries.Add(entry);

                reader.JumpTo(pos, true);
            }
        }

        public void ExportXML()
        {
            var rootElem = new XElement("MST");
            int index = 0;
            foreach (var entry in entries)
            {
                var message = new XElement("Message", entry.Text);
                var indexAttr = new XAttribute("Index", index);
                var nameAttr = new XAttribute("Name", entry.Name);
                var placeholderAttr = new XAttribute("Placeholder", entry.Placeholder);
                message.Add(indexAttr, nameAttr, placeholderAttr);
                rootElem.Add(message);
                index++;
            }

            var xml = new XDocument(rootElem);
            xml.Save(@"C:\Users\Knuxf\AppData\Local\Hyper_Development_Team\Sonic '06 Toolkit\Archives\72300\4au4zd2f.ih2\text\xenon\text\english\test.xml");
        }
    }
}
