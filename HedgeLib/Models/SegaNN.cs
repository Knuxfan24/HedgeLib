using HedgeLib.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HedgeLib.Models
{
    public class SegaNN : FileBase
    {
        public NinjaInfo NinjaInfo;
        public NinjaTextureList NinjaTextureList;
        public NinjaEffectList NinjaEffectList;
        public NinjaNodeNameList NinjaNodeNameList;
        public NinjaObject NinjaObject;
        public override void Load(Stream fileStream)
        {
            ExtendedBinaryReader reader = new ExtendedBinaryReader(fileStream) { Offset = 0x20 };
            long pos = 0;
            NinjaInfo = new NinjaInfo() { NodeName = new string(reader.ReadChars(4)) };
            uint NodeLength = reader.ReadUInt32();
            uint NodeCount = reader.ReadUInt32();
            uint Unknown1 = reader.ReadUInt32(); //Seems to always be 0x20 (at least in XN*s)?
            uint Unknown2 = reader.ReadUInt32(); //Footer Offset Table Start?
            uint Unknown3 = reader.ReadUInt32(); //Footer Offset Table Data Start?
            uint Unknown4 = reader.ReadUInt32(); //Footer Offset Table Length?
            uint Unknown5 = reader.ReadUInt32(); //Seems to always be 1 (at least in XN*s)?

            for (int i = 0; i < NodeCount; i++)
            {
                //Determine type of node to read
                string NextNodeName = new string(reader.ReadChars(4));
                uint NextNodeLength = reader.ReadUInt32();
                reader.JumpBehind(8);
                switch (NextNodeName)
                {
                    case "NXTL":
                    case "NZTL":
                        NinjaTextureList = ReadNinjaTextureList(reader, pos);
                        break;
                    case "NXEF":
                        NinjaEffectList = ReadNinjaEffectList(reader, pos);
                        break;
                    case "NXNN":
                        NinjaNodeNameList = ReadNinjaNodeNameList(reader, pos);
                        break;
                    case "NXOB":
                    case "NZOB":
                        NinjaObject = ReadNinjaObject(reader, pos);
                        break;
                    default:
                        reader.JumpAhead(8);
                        reader.JumpAhead(NextNodeLength);
                        //Console.WriteLine($"Block {NextNodeName} Not Implemented!");
                        break;
                }
            }
        }

        public NinjaTextureList ReadNinjaTextureList(ExtendedBinaryReader reader, long pos)
        {
            NinjaTextureList = new NinjaTextureList() { NodeName = new string(reader.ReadChars(4)) };
            uint NodeLength = reader.ReadUInt32();
            pos = reader.BaseStream.Position; //Save Position
            uint NodeOffset = reader.ReadUInt32();
            reader.JumpTo(NodeOffset, false);

            uint TextureCount = reader.ReadUInt32();
            uint TextureListOffset = reader.ReadUInt32();
            reader.JumpTo(TextureListOffset, false);

            for (int i = 0; i < TextureCount; i++)
            {
                uint Unknown1 = reader.ReadUInt32(); //Seems to always be 0 (at least in XN*s)? (Padding?)
                uint TextureNameOffset = reader.ReadUInt32();
                long currentPos = reader.BaseStream.Position; //Save Position
                reader.JumpTo(TextureNameOffset, false);
                NinjaTextureFile TextureFile = new NinjaTextureFile() { TextureName = reader.ReadNullTerminatedString() };
                reader.JumpTo(currentPos, true);
                TextureFile.Filters = reader.ReadUInt32();
                uint Unknown2 = reader.ReadUInt32(); //Seems to always be 0 (at least in XN*s)? (Padding?)
                uint Unknown3 = reader.ReadUInt32(); //Seems to always be 0 (at least in XN*s)? (Padding?)
                NinjaTextureList.TextureFiles.Add(TextureFile);
            }

            reader.JumpTo(pos, true);
            reader.JumpAhead(NodeLength);
            return NinjaTextureList;
        }

        public NinjaEffectList ReadNinjaEffectList(ExtendedBinaryReader reader, long pos)
        {
            NinjaEffectList = new NinjaEffectList() { NodeName = new string(reader.ReadChars(4)) };
            uint NodeLength = reader.ReadUInt32();
            pos = reader.BaseStream.Position; //Save Position
            uint NodeOffset = reader.ReadUInt32();
            reader.JumpTo(NodeOffset, false);

            uint Unknown1 = reader.ReadUInt32(); //Seems to always be 0 (at least in XN*s)? (Padding?)
            uint EffectTypeCount = reader.ReadUInt32();
            uint EffectTypeOffset = reader.ReadUInt32();
            uint TechniqueCount = reader.ReadUInt32();
            uint TechniqueOffset = reader.ReadUInt32();
            uint TechniqueIDXCount = reader.ReadUInt32();
            uint TechniqueIDXOffset = reader.ReadUInt32();
            
            //Effect Files
            reader.JumpTo(EffectTypeOffset, false);
            for (int i = 0; i <EffectTypeCount; i++)
            {
                NinjaEffectFile EffectFile = new NinjaEffectFile() { Type = reader.ReadUInt32() };
                uint EffectFilenameOffset = reader.ReadUInt32();
                long currentPos = reader.BaseStream.Position; //Save Position
                reader.JumpTo(EffectFilenameOffset, false);
                EffectFile.Filename = reader.ReadNullTerminatedString();
                reader.JumpTo(currentPos, true);
                NinjaEffectList.EffectFiles.Add(EffectFile);
            }

            //Effect List
            reader.JumpTo(TechniqueOffset, false);
            for (int i = 0; i < TechniqueCount; i++)
            {
                NinjaEffectTechnique Technique = new NinjaEffectTechnique()
                {
                    Type = reader.ReadUInt32(),
                    ID = reader.ReadUInt32()
                };
                uint TechniqueNameOffset = reader.ReadUInt32();
                long currentPos = reader.BaseStream.Position; //Save Position
                reader.JumpTo(TechniqueNameOffset, false);
                Technique.TechniqueName = reader.ReadNullTerminatedString();
                reader.JumpTo(currentPos, true);
                NinjaEffectList.Techniques.Add(Technique);
            }

            //Technique IDX (not sure what this is for)
            reader.JumpTo(TechniqueIDXOffset, false);
            for (int i = 0; i < TechniqueIDXCount; i++)
            {
                NinjaEffectList.TechniqueIDX.Add(reader.ReadUInt16());
            }

            reader.JumpTo(pos, true);
            reader.JumpAhead(NodeLength);
            return NinjaEffectList;
        }
    
        public NinjaNodeNameList ReadNinjaNodeNameList(ExtendedBinaryReader reader, long pos)
        {
            NinjaNodeNameList = new NinjaNodeNameList() { NodeName = new string(reader.ReadChars(4)) };
            uint NodeLength = reader.ReadUInt32();
            pos = reader.BaseStream.Position; //Save Position
            uint NodeOffset = reader.ReadUInt32();
            reader.JumpTo(NodeOffset, false);

            uint Unknown1 = reader.ReadUInt32(); //Padding?
            uint NodeTableCount = reader.ReadUInt32();
            uint NodeTableOffset = reader.ReadUInt32();
            reader.JumpTo(NodeTableOffset, false);

            for (int i = 0; i < NodeTableCount; i++)
            {
                NinjaNodeName NodeName = new NinjaNodeName() { ID = reader.ReadUInt32() };
                uint NodeNameIndex = reader.ReadUInt32();
                long currentPos = reader.BaseStream.Position; //Save Position
                reader.JumpTo(NodeNameIndex, false);
                NodeName.Name = reader.ReadNullTerminatedString();
                reader.JumpTo(currentPos, true);
                NinjaNodeNameList.NinjaNodeNames.Add(NodeName);
            }

            reader.JumpTo(pos, true);
            reader.JumpAhead(NodeLength);
            return NinjaNodeNameList;
        }
        public NinjaObject ReadNinjaObject(ExtendedBinaryReader reader, long pos)
        {
            NinjaObject = new NinjaObject() { NodeName = new string(reader.ReadChars(4)) };
            uint NodeLength = reader.ReadUInt32();
            pos = reader.BaseStream.Position; //Save Position
            uint NodeOffset = reader.ReadUInt32();
            reader.JumpTo(NodeOffset, false);

            NinjaObject.ObjectCenter = reader.ReadVector3();
            NinjaObject.ObjectRadius = reader.ReadSingle();
            uint ObjectMaterialCount = reader.ReadUInt32();
            uint ObjectMaterialOffset = reader.ReadUInt32();
            uint ObjectVTXCount = reader.ReadUInt32();
            uint ObjectVTXOffset = reader.ReadUInt32();
            uint ObjectPrimitiveCount = reader.ReadUInt32();
            uint ObjectPrimitiveOffset = reader.ReadUInt32();
            uint ObjectNodeCount = reader.ReadUInt32();
            NinjaObject.ObjectMaxNodeDepth = reader.ReadUInt32();
            uint ObjectNodeOffset = reader.ReadUInt32();
            NinjaObject.ObjectMTXPAL = reader.ReadUInt32();
            uint ObjectSubObjectCount = reader.ReadUInt32();
            uint ObjectSubObjectOffset = reader.ReadUInt32();
            NinjaObject.ObjectTextureCount = reader.ReadUInt32();

            //Materials
            reader.JumpTo(ObjectMaterialOffset, false);
            for (int i = 0; i < ObjectMaterialCount; i++)
            {
                //TO-DO: Figure out how all this fits together
                NinjaObjectMaterial ObjectMaterial = new NinjaObjectMaterial() { MaterialType = reader.ReadUInt32() };
                uint MaterialOffset = reader.ReadUInt32();
                long currentPos = reader.BaseStream.Position; //Save Position
                reader.JumpTo(MaterialOffset, false);

                ObjectMaterial.MaterialFlags = reader.ReadUInt32();
                ObjectMaterial.User = reader.ReadUInt32();
                uint MaterialColourOffset = reader.ReadUInt32();
                uint MaterialLogicOffset = reader.ReadUInt32();
                uint MaterialTexDescOffset = reader.ReadUInt32();

                reader.JumpTo(MaterialColourOffset, false);
                ObjectMaterial.MaterialDiffuse = reader.ReadVector4();
                ObjectMaterial.MaterialAmbient = reader.ReadVector4();
                ObjectMaterial.MaterialSpecular = reader.ReadVector4();
                ObjectMaterial.MaterialEmissive = reader.ReadVector4();
                ObjectMaterial.MaterialPower = reader.ReadSingle();

                reader.JumpTo(MaterialLogicOffset, false);
                ObjectMaterial.MaterialBlendenable = reader.ReadUInt32();
                ObjectMaterial.MaterialSRCBlend = reader.ReadUInt32();
                ObjectMaterial.MaterialDSTBlend = reader.ReadUInt32();
                ObjectMaterial.MaterialBlendFactor = reader.ReadUInt32();
                ObjectMaterial.MaterialBlendOP = reader.ReadUInt32();
                ObjectMaterial.MaterialLogicOP = reader.ReadUInt32();
                ObjectMaterial.MaterialAlphaEnable = reader.ReadUInt32();
                ObjectMaterial.MaterialAlphaFunction = reader.ReadUInt32();
                ObjectMaterial.MaterialAlphaRef = reader.ReadUInt32();
                ObjectMaterial.MaterialZCompenable = reader.ReadUInt32();
                ObjectMaterial.MaterialZFunction = reader.ReadUInt32();
                ObjectMaterial.MaterialZUpdateEnable = reader.ReadUInt32();

                reader.JumpTo(MaterialTexDescOffset, false);
                ObjectMaterial.TextureTexMapType = reader.ReadUInt32();
                ObjectMaterial.TextureID = reader.ReadUInt32();
                ObjectMaterial.TextureOffset = reader.ReadVector2();
                ObjectMaterial.TextureBlend = reader.ReadSingle();
                ObjectMaterial.TextureInfoPTR = reader.ReadUInt32();
                ObjectMaterial.TextureMinFilter = reader.ReadUInt32();
                ObjectMaterial.TextureMagFilter = reader.ReadUInt32();
                ObjectMaterial.TextureMipMapBias = reader.ReadSingle();
                ObjectMaterial.TextureMaxMipLevel = reader.ReadUInt32();

                reader.JumpTo(currentPos, true);
                NinjaObject.ObjectMaterialList.Add(ObjectMaterial);
            }

            //Vertexes
            reader.JumpTo(ObjectVTXOffset, false);
            for (int i = 0; i < ObjectVTXCount; i++)
            {
                NinjaObjectVertex ObjectVertex = new NinjaObjectVertex() { Type = reader.ReadUInt32() };
                uint VTXOffset = reader.ReadUInt32();
                long currentPos = reader.BaseStream.Position; //Save Position

                reader.JumpTo(VTXOffset, false);
                ObjectVertex.VTXFormat = reader.ReadUInt32();
                ObjectVertex.VTXFVF = reader.ReadUInt32();
                ObjectVertex.VTXSize = reader.ReadUInt32();
                uint VTXNumber = reader.ReadUInt32();
                uint VTXListOffset = reader.ReadUInt32();
                ObjectVertex.VTXBlendNum = reader.ReadUInt32();
                uint VTXMTXOffset = reader.ReadUInt32();
                ObjectVertex.VTXHDRCommon = reader.ReadUInt32();
                ObjectVertex.VTXHDRData = reader.ReadUInt32();
                ObjectVertex.VTXHDRLock = reader.ReadUInt32();

                reader.JumpTo(VTXListOffset, false);
                for (int v = 0; v < VTXNumber; v++)
                {
                    NinjaObjectVertexList Vertex = new NinjaObjectVertexList();
                    switch (ObjectVertex.VTXSize)
                    {
                        //Any ones other than 52 & 76 are probably wrong here, as they're not in the XTO and are instead based on the MaxScript
                        //This is such a mess
                        case 20:
                            Vertex.Position = reader.ReadVector3();
                            Vertex.ST0 = reader.ReadVector2();
                            ObjectVertex.Vertexes.Add(Vertex);
                            break;
                        case 24:
                            Vertex.Position = reader.ReadVector3();
                            Vertex.RGBA8888 = reader.ReadBytes(4);
                            Vertex.ST0 = reader.ReadVector2();
                            ObjectVertex.Vertexes.Add(Vertex);
                            break;
                        case 28:
                            Vertex.Position = reader.ReadVector3();
                            Vertex.Normals = reader.ReadVector3();
                            Vertex.RGBA8888 = reader.ReadBytes(4);
                            ObjectVertex.Vertexes.Add(Vertex);
                            break;
                        case 32:
                            Vertex.Position = reader.ReadVector3();
                            Vertex.Normals = reader.ReadVector3();
                            Vertex.ST0 = reader.ReadVector2();
                            ObjectVertex.Vertexes.Add(Vertex);
                            break;
                        case 36:
                            Vertex.Position = reader.ReadVector3();
                            Vertex.Normals = reader.ReadVector3();
                            Vertex.RGBA8888 = reader.ReadBytes(4);
                            Vertex.ST0 = reader.ReadVector2();
                            ObjectVertex.Vertexes.Add(Vertex);
                            break;
                        case 44:
                            Vertex.Position = reader.ReadVector3();
                            Vertex.Normals = reader.ReadVector3();
                            Vertex.RGBA8888 = reader.ReadBytes(4);
                            Vertex.ST0 = reader.ReadVector2();
                            Vertex.UnknownV2 = reader.ReadVector2();
                            ObjectVertex.Vertexes.Add(Vertex);
                            break;
                        case 48:
                            Vertex.Position = reader.ReadVector3();
                            Vertex.Weight3 = reader.ReadVector3();
                            Vertex.Normals = reader.ReadVector3();
                            Vertex.RGBA8888 = reader.ReadBytes(4);
                            Vertex.ST0 = reader.ReadVector2();
                            ObjectVertex.Vertexes.Add(Vertex);
                            break;
                        case 52:
                            Vertex.Position = reader.ReadVector3();
                            Vertex.Weight3 = reader.ReadVector3();
                            Vertex.MTXIDX = reader.ReadBytes(4);
                            Vertex.Normals = reader.ReadVector3();
                            Vertex.RGBA8888 = reader.ReadBytes(4);
                            Vertex.ST0 = reader.ReadVector2();
                            ObjectVertex.Vertexes.Add(Vertex);
                            break;
                        case 60:
                            Vertex.Position = reader.ReadVector3();
                            Vertex.Weight3 = reader.ReadVector3();
                            Vertex.Normals = reader.ReadVector3();
                            Vertex.Tan = reader.ReadVector3();
                            Vertex.BNormal = reader.ReadVector3();
                            ObjectVertex.Vertexes.Add(Vertex);
                            break;
                        case 72:
                            Vertex.Position = reader.ReadVector3();
                            Vertex.Weight3 = reader.ReadVector3();
                            Vertex.MTXIDX = reader.ReadBytes(4);
                            Vertex.Normals = reader.ReadVector3();
                            Vertex.ST0 = reader.ReadVector2();
                            Vertex.Tan = reader.ReadVector3();
                            Vertex.BNormal = reader.ReadVector3();
                            ObjectVertex.Vertexes.Add(Vertex);
                            break;
                        case 76:
                            Vertex.Position = reader.ReadVector3();
                            Vertex.Weight3 = reader.ReadVector3();
                            Vertex.MTXIDX = reader.ReadBytes(4);
                            Vertex.Normals = reader.ReadVector3();
                            Vertex.RGBA8888 = reader.ReadBytes(4);
                            Vertex.ST0 = reader.ReadVector2();
                            Vertex.Tan = reader.ReadVector3();
                            Vertex.BNormal = reader.ReadVector3();
                            ObjectVertex.Vertexes.Add(Vertex);
                            break;
                        default:
                            Console.WriteLine($"Vertex Size of {ObjectVertex.VTXSize} not handled!");
                            continue;
                    }
                }

                if (VTXMTXOffset != 0)
                {
                    reader.JumpTo(VTXMTXOffset, false);
                    ObjectVertex.VTXMTX = reader.ReadInt32();
                }

                reader.JumpTo(currentPos, true);
                NinjaObject.ObjectVertexList.Add(ObjectVertex);
            }

            reader.JumpTo(ObjectPrimitiveOffset, false);
            for (int i = 0; i < ObjectPrimitiveCount; i++)
            {
                NinjaObjectPrimitive ObjectPrimitive = new NinjaObjectPrimitive() { Type = reader.ReadUInt32() };
                uint PrimitiveListOffset = reader.ReadUInt32();
                long currentPos = reader.BaseStream.Position; //Save Position

                reader.JumpTo(PrimitiveListOffset, false);
                ObjectPrimitive.Format = reader.ReadUInt32();
                ObjectPrimitive.Index = reader.ReadUInt32();
                ObjectPrimitive.Strip = reader.ReadUInt32();
                uint LengthOffset = reader.ReadUInt32();
                uint IndexOffset = reader.ReadUInt32();
                ObjectPrimitive.IndexBuf = reader.ReadUInt32();

                reader.JumpTo(LengthOffset, false);
                ObjectPrimitive.IndexLength = reader.ReadUInt16(); //May be the same as Index all the time???

                reader.JumpTo(IndexOffset, false);
                for (int v = 0; v < ObjectPrimitive.Index; v++)
                {
                    ObjectPrimitive.VertexIndexList.Add(reader.ReadUInt16());
                }
                
                reader.JumpTo(currentPos, true);
                NinjaObject.ObjectPrimitiveList.Add(ObjectPrimitive);
            }

            reader.JumpTo(ObjectNodeOffset, false);
            for (int i = 0; i < ObjectNodeCount; i++)
            {
                NinjaObjectNodeList Node = new NinjaObjectNodeList()
                {
                    Type = reader.ReadUInt32(),
                    Matrix = reader.ReadUInt16(),
                    Parent = reader.ReadUInt16(),
                    Child = reader.ReadUInt16(),
                    Sibling = reader.ReadUInt16(),
                    Transform = reader.ReadVector3(),
                    Rotation = reader.ReadVector3(),
                    Scale = reader.ReadVector3()
                };
                for (int v = 0; v < 16; v++) { Node.Invinit.Add(reader.ReadSingle()); }
                Node.Center = reader.ReadVector3();
                Node.User = reader.ReadUInt32();
                Node.RSV0 = reader.ReadUInt32();
                Node.RSV1 = reader.ReadUInt32();
                Node.RSV2 = reader.ReadUInt32();
                reader.JumpAhead(0x4);
                NinjaObject.ObjectNodeList.Add(Node);
            }

            reader.JumpTo(ObjectSubObjectOffset, false);
            for (int i = 0; i < ObjectSubObjectCount; i++)
            {
                uint Type = reader.ReadUInt32();
                uint NMSST = reader.ReadUInt32();
                uint MSSTOffset = reader.ReadUInt32();
                uint Textures = reader.ReadUInt32();
                uint TexturesOffset = reader.ReadUInt32();
                long currentPos = reader.BaseStream.Position; //Save Position

                reader.JumpTo(MSSTOffset, false);
                for(int v = 0; v < NMSST; v++)
                {
                    NinjaObjectSubObject SubObject = new NinjaObjectSubObject()
                    {
                        Center = reader.ReadVector3(),
                        Radius = reader.ReadSingle(),
                        Node = reader.ReadUInt32(),
                        Matrix = reader.ReadUInt32(),
                        Material = reader.ReadUInt32(),
                        VertexList = reader.ReadUInt32(),
                        PrimList = reader.ReadUInt32(),
                        ShaderList = reader.ReadUInt32()
                    };
                    NinjaObject.ObjectSubObjectList.Add(SubObject);
                }

                reader.JumpTo(currentPos, true);
            }

            reader.JumpTo(pos, true);
            reader.JumpAhead(NodeLength);
            return NinjaObject;
        }
    }
}
