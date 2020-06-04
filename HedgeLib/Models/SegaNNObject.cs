using HedgeLib.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HedgeLib.Models
{
    /*XNO Block Types:
    NXIF
    NXMO
    NXTL
    NXEF
    NXNN
    NXOB
    NXMA
    NXCA
    NXMC
    NXLI
    NXML
    NEND
    NXMM
    NXMT
    */

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

    public class NXOB
    {
        public string ObjectList;
        public uint NodeLength;
        public List<Node> Nodes = new List<Node>();
    }

    public class Node
    {
        public uint NODE_TYPE;
        public ushort NODE_MATRIX;
        public ushort NODE_PARENT;
        public ushort NODE_CHILD;
        public ushort NODE_SIBLING;
        public List<float> NODE_TRN = new List<float>();
        public List<float> NODE_ROT = new List<float>();
        public List<float> NODE_SCL = new List<float>();
        public List<float> NODE_INVINIT_MTX = new List<float>();
        public List<float> NODE_CENTER = new List<float>();
        public float NODE_RADIUS;
        public uint NODE_USER;
        public uint NODE_RSV0;
        public uint NODE_RSV1;
        public uint NODE_RSV2;
    }

    public class MaterialColour
    {
        public List<float> MAT_DIFFUSE = new List<float>();
        public List<float> MAT_AMBIENT = new List<float>();
        public List<float> MAT_SPECULAR = new List<float>();
        public List<float> MAT_EMISSIVE = new List<float>();
        public float MAT_POWER;
    }

    public class SegaNNObject : FileBase
    {
        public NXIF InfoList;
        public NXTL TextureList;
        public NXEF EffectList;
        public NXNN NodeTree;
        public NXOB ObjectList;
        public override void Load(Stream fileStream)
        {
            ExtendedBinaryReader reader = new ExtendedBinaryReader(fileStream) { Offset = 0x20 };
            long pos = 0;
            // NINJA XBOX INFO [NXIF]
            InfoList = new NXIF()
            {
                HeaderInfoNode = new string(reader.ReadChars(4)),
                NodeLength = reader.ReadUInt32(),
                NodeCount = reader.ReadUInt32()
            };

            reader.JumpTo(InfoList.NodeLength + 8);

            for(int i = 0; i < InfoList.NodeCount; i++)
            {
                string nodeName = new string(reader.ReadChars(4));
                uint nodeLength = reader.ReadUInt32();
                reader.JumpBehind(8);
                switch(nodeName)
                {
                    case "NXTL":
                    case "NGTL":
                    case "NZTL":
                        // NINJA XBOX TEXTURE LIST [NXTL]
                        TextureList = ReadTextureList(reader, pos);
                        break;
                    case "NXEF":
                        // NINJA XBOX EFFECTS [NXEF]
                        EffectList = ReadEffectList(reader, pos);
                        break;
                    case "NXNN":
                    case "NGNN":
                        // NINJA XBOX NODE NAMES [NXNN]
                        NodeTree = ReadNodeNames(reader, pos);
                        break;
                    case "NXOB":
                    case "NZOB":
                        ObjectList = ReadNodes(reader, pos);
                        break;
                    default:
                        reader.JumpAhead(8);
                        reader.JumpAhead(nodeLength);
                        Console.WriteLine($"Block {nodeName} Not Implemented!");
                        break;
                }
            }
        }

        public NXTL ReadTextureList(ExtendedBinaryReader reader, long pos)
        {
            TextureList = new NXTL()
            {
                TextureListNode = new string(reader.ReadChars(4)),
                NodeLength = reader.ReadUInt32()
            };

            pos = reader.BaseStream.Position; //Save Position
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
            return TextureList;
        }

        public NXEF ReadEffectList(ExtendedBinaryReader reader, long pos)
        {
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
            for (int i = 0; i < effectTypeCount; i++)
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
            return EffectList;
        }

        public NXNN ReadNodeNames(ExtendedBinaryReader reader, long pos)
        {
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
            return NodeTree;
        }

        public NXOB ReadNodes(ExtendedBinaryReader reader, long pos)
        {
            /*
            Sonic 4 Decompilation Ref
            public NNS_NODE( AppMain.NNS_NODE node )
            {
                this.fType = node.fType;
                this.iMatrix = node.iMatrix;
                this.iParent = node.iParent;
                this.iChild = node.iChild;
                this.iSibling = node.iSibling;
                this.Translation.Assign( node.Translation );
                this.Rotation = node.Rotation;
                this.Scaling.Assign( node.Scaling );
                this.InvInitMtx.Assign( node.InvInitMtx );
                this.Center.Assign( node.Center );
                this.Radius = node.Radius;
                this.User = node.User;
                this.SIIKBoneLength = node.SIIKBoneLength;
                this.BoundingBoxY = node.BoundingBoxY;
                this.BoundingBoxZ = node.BoundingBoxZ;
            }
            */
            ObjectList = new NXOB()
            {
                ObjectList = new string(reader.ReadChars(4)),
                NodeLength = reader.ReadUInt32()
            };

            pos = reader.BaseStream.Position; //Save Position
            reader.JumpTo(reader.ReadUInt32(), false);
            reader.JumpAhead(0x10);

            //Variable Names Copy & Pasted from MaxScript
            var TexElmTotal = reader.ReadUInt32();
            var TexElmOffset = reader.ReadUInt32();

            var VertGroupTotal = reader.ReadUInt32();
            var VertGroupOffset = reader.ReadUInt32();

            var PolyElmTotal = reader.ReadUInt32();
            var PolyElmOffset = reader.ReadUInt32();

            var NodeTotal = reader.ReadUInt32();
            var UnknownCount1 = reader.ReadUInt32(); //Count of SOMETHING?
            var NodeOffset = reader.ReadUInt32();

            var UnknownCount2 = reader.ReadUInt32(); //MaxScript calls this NodeTotal, but I'm sure what they call BoneTotal is actually NodeTotal? Seems to be a Count of something
            var LinkTotal = reader.ReadUInt32();
            var LinkOffset = reader.ReadUInt32();
            var UnknownCount3 = reader.ReadUInt32(); //Count of SOMETHING?

            //NNS_NODE
            reader.JumpTo(NodeOffset, false);
            for (int i = 0; i < NodeTotal; i++)
            {
                Node node = new Node();
                node.NODE_TYPE = reader.ReadUInt32();
                node.NODE_MATRIX = reader.ReadUInt16();
                node.NODE_PARENT = reader.ReadUInt16();
                node.NODE_CHILD = reader.ReadUInt16();
                node.NODE_SIBLING = reader.ReadUInt16();

                for(int n = 0; n < 3; n++)
                {
                    node.NODE_TRN.Add(reader.ReadSingle());
                }

                for (int n = 0; n < 3; n++)
                {
                    node.NODE_ROT.Add(reader.ReadSingle());
                }

                for (int n = 0; n < 3; n++)
                {
                    node.NODE_SCL.Add(reader.ReadSingle());
                }

                for (int n = 0; n < 16; n++)
                {
                    node.NODE_INVINIT_MTX.Add(reader.ReadSingle());
                }

                for (int n = 0; n < 3; n++)
                {
                    node.NODE_CENTER.Add(reader.ReadSingle());
                }

                node.NODE_RADIUS = reader.ReadSingle();
                node.NODE_USER = reader.ReadUInt32();
                node.NODE_RSV0 = reader.ReadUInt32();
                node.NODE_RSV1 = reader.ReadUInt32();
                node.NODE_RSV2 = reader.ReadUInt32();
                ObjectList.Nodes.Add(node);
            }

            return ObjectList;
        }
    }
}
