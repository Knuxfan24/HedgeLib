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

namespace HedgeLib.Sound
{
    public class SBKCue
    {
        public char[] Name;
        public uint SoundType; //?
        public uint Index;
        public uint Category; //?
        public float Unknown1;
        public float Unknown2;

        public SBKCue() { }

        public SBKCue(char[] name, uint soundType, uint index, uint category, float unknown1, float unknown2)
        {
            Name = name;
            SoundType = soundType;
            Index = index;
            Category = category;
            Unknown1 = unknown1;
            Unknown2 = unknown2;
        }
    }

    public class S06SBK : FileBase
    {
        public BINAHeader Header = new BINAv1Header();
        public const string Signature = "SBNK", Extension = ".sbk";
        uint Unknown1;
        public char[] Name;
        public uint CueCount;
        public uint NormalCueCount;
        public uint StreamCount;
        public List<SBKCue> Cues = new List<SBKCue>();
        public List<string> SoundNames = new List<string>();

        public override void Load(Stream fileStream)
        {
            // Header
            var reader = new BINAReader(fileStream);
            Header = reader.ReadHeader();

            string sig = reader.ReadSignature(4);
            if (sig != Signature)
                throw new InvalidSignatureException(Signature, sig);

            Unknown1 = reader.ReadUInt32(); //Probably Flags according to Rad's Spec
            if (Unknown1 != 537265920)
            {
                Console.WriteLine($"Unknown 1 does not equal 537265920! Actually equals {Unknown1}!");
            }
            uint bankNameOffset = reader.ReadUInt32();
            uint cueNameOffset = reader.ReadUInt32();
            uint cueIndiciesOffset = reader.ReadUInt32();
            uint streamOffset = reader.ReadUInt32();
            Name = reader.ReadChars(64);
            CueCount = reader.ReadUInt32();
            NormalCueCount = reader.ReadUInt32();
            StreamCount = reader.ReadUInt32();

            for(uint i = 0; i < CueCount; i++)
            {
                SBKCue cue = new SBKCue();
                cue.Name = reader.ReadChars(32);
                cue.SoundType = reader.ReadUInt32();
                cue.Index = reader.ReadUInt32();
                cue.Category = reader.ReadUInt32();
                cue.Unknown1 = reader.ReadSingle();
                cue.Unknown2 = reader.ReadSingle();
                Cues.Add(cue);
            }

            for(uint i = 0; i < StreamCount; i++)
            {
                long pos = reader.BaseStream.Position;
                reader.JumpTo(streamOffset, false);
                reader.JumpAhead(4 * i);
                reader.JumpTo(reader.ReadUInt32(), false);
                SoundNames.Add(reader.ReadNullTerminatedString());
                reader.JumpTo(pos, true);
            }
        }

        public override void Save(Stream fileStream)
        {
            // Header
            var writer = new BINAWriter(fileStream, Header);
            writer.WriteSignature(Signature);
            writer.Write(Unknown1);
            writer.AddOffset("banksOffset");
            writer.AddOffset("cueNamesOffset");
            writer.AddOffset("cueIndiciesOffset");
            writer.AddOffset("streamsOffset");
            writer.FillInOffset("banksOffset", false);
            writer.Write(Name);
            writer.Write(CueCount);
            writer.Write(NormalCueCount);
            writer.Write(StreamCount);
            
            for(uint i = 0; i < CueCount; i++)
            {
                if (i == 0)
                {
                    writer.FillInOffset("cueNamesOffset", false);
                }
                writer.Write(Cues[(int)i].Name);
                writer.Write(Cues[(int)i].SoundType);
                writer.Write(Cues[(int)i].Index);
                writer.Write(Cues[(int)i].Category);
                writer.Write(Cues[(int)i].Unknown1);
                writer.Write(Cues[(int)i].Unknown2);
            }

            //Write number table for sounds without a stream
            bool filledInCueIndicies = false;
            for (uint i = 0; i < CueCount; i++)
            {
                if (Cues[(int)i].SoundType == 0)
                {
                    if (!filledInCueIndicies)
                    {
                        writer.FillInOffset("cueIndiciesOffset", false);
                        filledInCueIndicies = true;
                    }
                    writer.Write(Cues[(int)i].Index);
                }
            }

            bool filledInStreams = false;
            int soundNameIndex = 0;
            for (uint i = 0; i < CueCount; i++)
            {
                if (Cues[(int)i].SoundType == 1)
                {
                    if (!filledInStreams)
                    {
                        writer.FillInOffset("streamsOffset", false);
                        filledInStreams = true;
                    }
                    writer.AddString($"streamOffset{i}", $"{SoundNames[soundNameIndex]}");
                    soundNameIndex++;
                }
            }


            writer.FinishWrite(Header);
        }

        public void ExportXML(string filepath)
        {
            var rootElem = new XElement("SBK");
            var sbkUnknown1Attr = new XAttribute("unknown1", Unknown1);
            var name = new string(Name);
            name = name.Replace("\0", "");
            var sbkNameAttr = new XAttribute("name", name);
            var cueCountAttr = new XAttribute("cueCount", CueCount);
            var normalCueCountAttr = new XAttribute("normalCueCount", NormalCueCount);
            var streamCountAttr = new XAttribute("streamCount", StreamCount);
            rootElem.Add(sbkUnknown1Attr, sbkNameAttr, cueCountAttr, normalCueCountAttr, streamCountAttr);

            foreach(var cue in Cues)
            {
                var cueElem = new XElement("Cue");
                name = new string(cue.Name);
                name = name.Replace("\0", "");
                var cueNameElm = new XElement("Name", name);
                var cueSoundTypeElm = new XElement("SoundType", cue.SoundType);
                var cueIndexElem = new XElement("Index", cue.Index);
                var cueCategoryElem = new XElement("Category", cue.Category);
                var cueUnknown1Elem = new XElement("Unknown1", cue.Unknown1);
                var cueUnknown2Elem = new XElement("Unknown2", cue.Unknown2);

                cueElem.Add(cueNameElm, cueSoundTypeElm, cueIndexElem, cueCategoryElem, cueUnknown1Elem, cueUnknown2Elem);

                if (cue.SoundType == 1)
                {
                    var cueStreamElem = new XElement("Stream", SoundNames[(int)cue.Index]);
                    cueElem.Add(cueStreamElem);
                }
                rootElem.Add(cueElem);
            }

            var xml = new XDocument(rootElem);
            xml.Save(filepath);
        }

        public void ImportXML(string filepath)
        {
            var xml = XDocument.Load(filepath);
            Unknown1 = uint.Parse(xml.Root.Attribute("unknown1").Value);
            char[] name = xml.Root.Attribute("name").Value.PadRight(64, '\0').ToCharArray();
            Name = name;
            CueCount = uint.Parse(xml.Root.Attribute("cueCount").Value);
            NormalCueCount = uint.Parse(xml.Root.Attribute("normalCueCount").Value);
            StreamCount = uint.Parse(xml.Root.Attribute("streamCount").Value);
            foreach (var cueElem in xml.Root.Elements("Cue"))
            {
                SBKCue cue = new SBKCue();
                cue.Name = cueElem.Element("Name").Value.PadRight(32, '\0').ToCharArray();
                cue.SoundType = uint.Parse(cueElem.Element("SoundType").Value);
                cue.Index = uint.Parse(cueElem.Element("Index").Value);
                cue.Category = uint.Parse(cueElem.Element("Category").Value);
                cue.Unknown1 = float.Parse(cueElem.Element("Unknown1").Value);
                cue.Unknown2 = float.Parse(cueElem.Element("Unknown2").Value);
                Cues.Add(cue);

                if (cue.SoundType == 1)
                {
                    SoundNames.Add(cueElem.Element("Stream").Value);
                }
            }
        }
    }
}
