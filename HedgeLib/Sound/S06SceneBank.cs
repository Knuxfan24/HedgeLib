using HedgeLib.Exceptions;
using HedgeLib.Headers;
using HedgeLib.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace HedgeLib.Sound
{
    public class Cue
    {
        public char[] Name; //Name of this Cue in the Scene Bank
        public uint Category; //Uncertain what exactly this affects
        public float Unknown1; //Flag?
        public float Unknown2; //Flag?
        public string Stream; //XMA this Cue uses, if null, assume it uses a CSB instead
    }
    public class S06SceneBank : FileBase
    {
        public BINAHeader Header = new BINAv1Header();
        public const string Signature = "SBNK", Extension = ".sbk";
        public char[] Name;
        public List<Cue> Cues = new List<Cue>();
        public override void Load(Stream fileStream)
        {
            // Header
            var reader = new BINAReader(fileStream);
            Header = reader.ReadHeader();

            string sig = reader.ReadSignature(4);
            if (sig != Signature)
                throw new InvalidSignatureException(Signature, sig);

            uint unknown1 = reader.ReadUInt32(); //Probably Flags according to Rad's Spec
            if (unknown1 != 537265920) { Console.WriteLine($"unknown1 does not equal 537265920! Actually equals {unknown1}!"); }

            uint bankNameOffset = reader.ReadUInt32(); //Offset to the Scene Bank name
            uint cueNameOffset = reader.ReadUInt32(); //Offset of the first entry in the Scene Bank
            uint cueIndiciesOffset = reader.ReadUInt32(); //Offset to the number list for non stream indexs
            uint streamOffset = reader.ReadUInt32(); //Offset to the table for xma names

            Name = reader.ReadChars(64); //Scene Bank's name
            uint cueCount = reader.ReadUInt32(); //Total Number of Cues in this Scene Bank
            uint csbCueCount = reader.ReadUInt32(); //Amount of Cues in this Scene Bank which pull their data from a corrosponding CSB file
            uint streamCueCount = reader.ReadUInt32(); //Amount of Cues in this Scene Bank which use XMA files

            int streams = 0;

            for (uint i = 0; i < cueCount; i++)
            {
                Cue cue = new Cue()
                {
                    Name = reader.ReadChars(32)
                };
                uint cueType = reader.ReadUInt32();
                uint cueIndex = reader.ReadUInt32();
                cue.Category = reader.ReadUInt32();
                cue.Unknown1 = reader.ReadSingle();
                cue.Unknown2 = reader.ReadSingle();

                if (cueType == 1)
                {
                    long pos = reader.BaseStream.Position; //Save position
                    reader.JumpTo(streamOffset, false);
                    reader.JumpAhead(4 * streams); //Jump ahead to the right offset for our Cue's XMA
                    reader.JumpTo(reader.ReadUInt32(), false);
                    cue.Stream = reader.ReadNullTerminatedString(); //Read the XMA's name for this Cue
                    reader.JumpTo(pos, true); //Jump back to where we were
                    streams++;
                }
                Cues.Add(cue); //Save Cue to list
            }
        }

        public override void Save(Stream fileStream)
        {
            //Determine amount of Cues that use a CSB and amount that use an XMA
            int csbCueCount = 0;
            int streamCueCount = 0;
            for (int i = 0; i < Cues.Count; i++)
            {
                if(Cues[i].Stream == null) { csbCueCount++; }
                else { streamCueCount++; }
            }

            // Header
            var writer = new BINAWriter(fileStream, Header);
            writer.WriteSignature(Signature);
            writer.Write(537265920); //Hardcoded as all Scene Banks seem to have this number in this position.
            writer.AddOffset("banksOffset");
            writer.AddOffset("cueNamesOffset");
            writer.AddOffset("cueIndiciesOffset");
            writer.AddOffset("streamsOffset");
            writer.FillInOffset("banksOffset", false);
            writer.Write(Name);
            writer.Write(Cues.Count);
            writer.Write(csbCueCount);
            writer.Write(streamCueCount);

            //Cue Information
            writer.FillInOffset("cueNamesOffset", false);
            int csbCueID = 0;
            int streamCueID = 0;
            for(int i = 0; i < Cues.Count; i++)
            {
                writer.Write(Cues[i].Name);
                if (Cues[i].Stream == null)
                {
                    writer.Write(0);
                    writer.Write(csbCueID);
                    csbCueID++;
                }
                else
                {
                    writer.Write(1);
                    writer.Write(streamCueID);
                    streamCueID++;
                }
                writer.Write(Cues[i].Category);
                writer.Write(Cues[i].Unknown1);
                writer.Write(Cues[i].Unknown2);
            }

            //CSB Cue ID List
            writer.FillInOffset("cueIndiciesOffset", false);
            for (int i = 0; i < csbCueCount; i++) { writer.Write(i); }

            //Stream Names
            if(streamCueCount != 0)
            {
                writer.FillInOffset("streamsOffset", false);
                for (int i = 0; i < Cues.Count; i++)
                {
                    if (Cues[i].Stream != null) { writer.AddString($"streamOffset{i}", $"{Cues[i].Stream}"); }
                }
            }

            writer.FinishWrite(Header);
        }

        public void ExportXML(string filepath)
        {
            var rootElem = new XElement("SBK");
            var name = new string(Name); //Convert Char Array to String
            name = name.Replace("\0", ""); //Replace Empty Chars with nothing
            var sbkNameAttr = new XAttribute("name", name);
            rootElem.Add(sbkNameAttr);

            foreach (var cue in Cues)
            {
                var cueElem = new XElement("Cue");
                name = new string(cue.Name); //Convert Char Array to String
                name = name.Replace("\0", ""); //Replace Empty Chars with nothing
                var cueNameElm = new XElement("Name", name);
                var cueCategoryElem = new XElement("Category", cue.Category);
                var cueUnknown1Elem = new XElement("Unknown1", cue.Unknown1);
                var cueUnknown2Elem = new XElement("Unknown2", cue.Unknown2);
                var cueStreamElem = new XElement("Stream", cue.Stream);

                cueElem.Add(cueNameElm, cueCategoryElem, cueUnknown1Elem, cueUnknown2Elem, cueStreamElem);
                rootElem.Add(cueElem);
            }

            var xml = new XDocument(rootElem);
            xml.Save(filepath);
        }

        public void ImportXML(string filepath)
        {
            var xml = XDocument.Load(filepath);
            char[] name = xml.Root.Attribute("name").Value.PadRight(64, '\0').ToCharArray(); //Convert String to Char Array
            Name = name;
            foreach (var cueElem in xml.Root.Elements("Cue"))
            {
                Cue cue = new Cue()
                {
                    Name = cueElem.Element("Name").Value.PadRight(32, '\0').ToCharArray(), //Convert String to Char Array
                    Category = uint.Parse(cueElem.Element("Category").Value),
                    Unknown1 = float.Parse(cueElem.Element("Unknown1").Value),
                    Unknown2 = float.Parse(cueElem.Element("Unknown2").Value),
                };
                if (cueElem.Element("Stream").Value != "") { cue.Stream = cueElem.Element("Stream").Value; } //Check if Stream actually has a value before setting it
                Cues.Add(cue);
            }
        }
    }
}
