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

        public List<string> Effects = new List<string>();
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

            reader.JumpTo(reader.ReadUInt32(), false);
            textureList.TextureCount = reader.ReadUInt32();
            uint textureListOffset = reader.ReadUInt32();
            reader.JumpTo(textureListOffset, false);

            for (int i = 0; i < textureList.TextureCount; i++)
            {
                reader.JumpAhead(0x4);
                uint textureNameOffset = reader.ReadUInt32();
                long pos = reader.BaseStream.Position; //Save Position
                reader.JumpTo(textureNameOffset, false);
                textureList.Textures.Add(reader.ReadNullTerminatedString());
                reader.JumpTo(pos);
                reader.JumpAhead(0xC);
            }

            // NINJA XBOX EFFECTS [NXEF]
            NXEF effectList = new NXEF()
            {
                EffectNode = new string(reader.ReadChars(4)),
                NodeLength = reader.ReadUInt32()
            };



            // NINJA XBOX NODE NAMES [NXNN]

            // NINJA XBOX OBJECTS [NXOB]

            // NINJA XBOX MATERIALS [NXMT]
        }
    }
}
