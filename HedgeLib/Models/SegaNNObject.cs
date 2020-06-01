using HedgeLib.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HedgeLib.Models
{
    public class NXIF
    {
        public string HeaderInfoNode;
        public uint NodeLength;
        public uint NodeCount;
    }

    public class NXTL
    {
        public string TextureListNode;
        public uint NodeLength;
        public uint TextureCount;

        public List<string> Textures = new List<string>();
    }

    public class NXEF
    {
        public string EffectNode;
        public uint NodeLength;
        public uint EffectCount;

        public List<EFFFILE> Effects = new List<EFFFILE>();
        public List<TECHNAME> Techs = new List<TECHNAME>();
    }

    public class EFFFILE
    {
        public uint Type;
        public string Filename;
    }

    public class TECHNAME
    {
        public uint Type;
        public uint IDX; //???
        public string Filename;
    }

    public class SegaNNObject : FileBase
    {
        public override void Load(Stream fileStream)
        {
            ExtendedBinaryReader reader = new ExtendedBinaryReader(fileStream) { Offset = 0x20 };

            // NINJA XBOX INFO [NXIF]
            NXIF infoList = new NXIF()
            {
                HeaderInfoNode = new string(reader.ReadChars(4)),
                NodeLength = reader.ReadUInt32(),
                NodeCount = reader.ReadUInt32()
            };

            reader.JumpTo(infoList.NodeLength + 8);

            // NINJA XBOX TEXTURE LIST [NXTL]
            NXTL textureList = new NXTL()
            {
                TextureListNode = new string(reader.ReadChars(4)),
                NodeLength = reader.ReadUInt32()
            };

            long pos = reader.BaseStream.Position; //Save Position
            reader.JumpTo(reader.ReadUInt32(), false);
            textureList.TextureCount = reader.ReadUInt32();
            uint textureListOffset = reader.ReadUInt32();
            reader.JumpTo(textureListOffset, false);

            for (int i = 0; i < textureList.TextureCount; i++)
            {
                reader.JumpAhead(0x4);
                uint textureNameOffset = reader.ReadUInt32();
                long texturePos = reader.BaseStream.Position; //Save Position
                reader.JumpTo(textureNameOffset, false);
                textureList.Textures.Add(reader.ReadNullTerminatedString());
                reader.JumpTo(texturePos);
                reader.JumpAhead(0xC);
            }
            reader.JumpTo(pos);
            reader.JumpAhead(textureList.NodeLength);

            // NINJA XBOX EFFECTS [NXEF]
            NXEF effectList = new NXEF()
            {
                EffectNode = new string(reader.ReadChars(4)),
                NodeLength = reader.ReadUInt32()
            };

            pos = reader.BaseStream.Position; //Save Position
            reader.JumpTo(reader.ReadUInt32() + 4, false);
            var effectTypeCount = reader.ReadUInt32();
            var effectTypeOffset = reader.ReadUInt32();
            var techiqueCount = reader.ReadUInt32();
            var techniqueOffset = reader.ReadUInt32();
            reader.JumpTo(effectTypeOffset, false);
            for(int i = 0; i < effectTypeCount; i++)
            {
                EFFFILE effFile = new EFFFILE();
                effFile.Type = reader.ReadUInt32();
                var jumpPoint = reader.ReadUInt32();
                long effectPos = reader.BaseStream.Position; //Save Position
                reader.JumpTo(jumpPoint, false);
                effFile.Filename = reader.ReadNullTerminatedString();
                reader.JumpTo(effectPos);
                effectList.Effects.Add(effFile);
            }
            reader.JumpTo(techniqueOffset, false);
            for (int i = 0; i < techiqueCount; i++)
            {
                TECHNAME tech = new TECHNAME();
                tech.Type = reader.ReadUInt32();
                tech.IDX = reader.ReadUInt32();
                var jumpPoint = reader.ReadUInt32();
                long techPos = reader.BaseStream.Position; //Save Position
                reader.JumpTo(jumpPoint, false);
                tech.Filename = reader.ReadNullTerminatedString();
                reader.JumpTo(techPos);
                effectList.Techs.Add(tech);
            }
            reader.JumpTo(pos);


            // NINJA XBOX NODE NAMES [NXNN]

            // NINJA XBOX OBJECTS [NXOB]

            // NINJA XBOX MATERIALS [NXMT]
        }
    }
}
