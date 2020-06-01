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

        public List<TEXFILE> Textures = new List<TEXFILE>();
    }


    public class TEXFILE
    {
        public string Filename;
        public uint Filters;
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
        public uint NodeID; //??? Listed in the XTO for en_Kyozoress as
        //	     0,     0,     0,     1,     1,     1,     1,     1,     1,     1,     1,     1,     2,     2,
        public string Filename;
    }

    public class NXNN
    {
        public string TreeNode;
        public uint NodeLength;

        public List<NodeTree> Nodes = new List<NodeTree>();
    }
    public class NodeTree
    {
        public uint NodeIndex;
        public string NodeName;
    }

    public class SegaNNObject : FileBase
    {
        public NXIF InfoList;
        public NXTL TextureList;
        public NXEF EffectList;
        public NXNN NodeTree;
        public override void Load(Stream fileStream)
        {
            ExtendedBinaryReader reader = new ExtendedBinaryReader(fileStream) { Offset = 0x20 };

            // NINJA XBOX INFO [NXIF]
            InfoList = new NXIF()
            {
                HeaderInfoNode = new string(reader.ReadChars(4)),
                NodeLength = reader.ReadUInt32(),
                NodeCount = reader.ReadUInt32()
            };

            reader.JumpTo(InfoList.NodeLength + 8);

            // NINJA XBOX TEXTURE LIST [NXTL]
            TextureList = new NXTL()
            {
                TextureListNode = new string(reader.ReadChars(4)),
                NodeLength = reader.ReadUInt32()
            };

            long pos = reader.BaseStream.Position; //Save Position
            reader.JumpTo(reader.ReadUInt32(), false);
            TextureList.TextureCount = reader.ReadUInt32();
            uint textureListOffset = reader.ReadUInt32();
            reader.JumpTo(textureListOffset, false);

            for (int i = 0; i < TextureList.TextureCount; i++)
            {
                reader.JumpAhead(0x4);
                uint textureNameOffset = reader.ReadUInt32();
                long texturePos = reader.BaseStream.Position; //Save Position
                reader.JumpTo(textureNameOffset, false);
                TEXFILE tex = new TEXFILE();
                tex.Filename = reader.ReadNullTerminatedString();
                reader.JumpTo(texturePos);
                tex.Filters = reader.ReadUInt32();
                reader.JumpAhead(0x8);
                TextureList.Textures.Add(tex);
            }
            reader.JumpTo(pos);
            reader.JumpAhead(TextureList.NodeLength);

            // NINJA XBOX EFFECTS [NXEF]
            EffectList = new NXEF()
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
                EffectList.Effects.Add(effFile);
            }
            reader.JumpTo(techniqueOffset, false);
            for (int i = 0; i < techiqueCount; i++)
            {
                TECHNAME tech = new TECHNAME();
                tech.Type = reader.ReadUInt32();
                tech.NodeID = reader.ReadUInt32();
                var jumpPoint = reader.ReadUInt32();
                long techPos = reader.BaseStream.Position; //Save Position
                reader.JumpTo(jumpPoint, false);
                tech.Filename = reader.ReadNullTerminatedString();
                reader.JumpTo(techPos);
                EffectList.Techs.Add(tech);
            }
            reader.JumpTo(pos);
            reader.JumpAhead(EffectList.NodeLength);

            // NINJA XBOX NODE NAMES [NXNN]
            NodeTree = new NXNN()
            {
                TreeNode = new string(reader.ReadChars(4)),
                NodeLength = reader.ReadUInt32()
            };

            pos = reader.BaseStream.Position; //Save Position
            reader.JumpTo(reader.ReadUInt32() + 4, false);
            var nodeCount = reader.ReadUInt32();
            reader.JumpTo(reader.ReadUInt32(), false);
            for (int i = 0; i < nodeCount; i++)
            {
                NodeTree node = new NodeTree();
                node.NodeIndex = reader.ReadUInt32();
                var nameOffset = reader.ReadUInt32();
                long nodePos = reader.BaseStream.Position; //Save Position
                reader.JumpTo(nameOffset, false);
                node.NodeName = reader.ReadNullTerminatedString();
                reader.JumpTo(nodePos);
                NodeTree.Nodes.Add(node);
            }
            reader.JumpTo(pos);
            reader.JumpAhead(NodeTree.NodeLength);

            // NINJA XBOX OBJECTS [NXOB]

            // NINJA XBOX MATERIALS [NXMT]
        }
    }
}
