using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace HedgeLib.Models
{
    public class NinjaInfo
    {
        public string NodeName;
    }

    public class NinjaTextureList
    {
        public string NodeName;
        public List<NinjaTextureFile> TextureFiles = new List<NinjaTextureFile>();
    }
    public class NinjaTextureFile
    {
        public string TextureName;
        public uint Filters;
    }

    public class NinjaEffectList
    {
        public string NodeName;
        public List<NinjaEffectFile> EffectFiles = new List<NinjaEffectFile>();
        public List<NinjaEffectTechnique> Techniques = new List<NinjaEffectTechnique>();
        public List<ushort> TechniqueIDX = new List<ushort>();
    }
    public class NinjaEffectFile
    {
        public uint Type;
        public string Filename;
    }
    public class NinjaEffectTechnique
    {
        public uint Type;
        public uint ID; //Unsure if making this part of the class is a good idea or not
        public string TechniqueName;
    }

    public class NinjaNodeNameList
    {
        public string NodeName;
        public List<NinjaNodeName> NinjaNodeNames = new List<NinjaNodeName>();
    }
    public class NinjaNodeName
    {
        public uint ID; //Unsure if making this part of the class is a good idea or not
        public string Name;
    }

    public class NinjaObject
    {
        public string NodeName;
        public Vector3 ObjectCenter;
        public float ObjectRadius;
        public List<NinjaObjectMaterial> ObjectMaterialList = new List<NinjaObjectMaterial>();
        public List<NinjaObjectVertex> ObjectVertexList = new List<NinjaObjectVertex>();
        public List<NinjaObjectPrimitive> ObjectPrimitiveList = new List<NinjaObjectPrimitive>();
        public uint ObjectMaxNodeDepth;
        public List<NinjaObjectNodeList> ObjectNodeList = new List<NinjaObjectNodeList>();
        public uint ObjectMTXPAL; //Not sure what MTXPAL means
        public List<NinjaObjectSubObject> ObjectSubObjectList = new List<NinjaObjectSubObject>();
        public uint ObjectTextureCount;
    }
    public class NinjaObjectMaterial
    {
        public uint MaterialType;

        //NNS_MATERIAL_DESC
        public uint MaterialFlags;
        public uint User; //Not sure what that means, entirely 0 in the XTO.

        //NNS_MATERIAL_COLOR
        public Vector4 MaterialDiffuse;
        public Vector4 MaterialAmbient;
        public Vector4 MaterialSpecular;
        public Vector4 MaterialEmissive;
        public float MaterialPower;

        //NNS_MATERIAL_LOGIC
        public uint MaterialBlendenable;
        public uint MaterialSRCBlend;
        public uint MaterialDSTBlend;
        public uint MaterialBlendFactor;
        public uint MaterialBlendOP;
        public uint MaterialLogicOP;
        public uint MaterialAlphaEnable;
        public uint MaterialAlphaFunction;
        public uint MaterialAlphaRef;
        public uint MaterialZCompenable;
        public uint MaterialZFunction;
        public uint MaterialZUpdateEnable;

        //NNS_MATERIAL_TEXMAP2_DESC
        public uint TextureTexMapType;
        public uint TextureID;
        public Vector2 TextureOffset;
        public float TextureBlend;
        public uint TextureInfoPTR;
        public uint TextureMinFilter;
        public uint TextureMagFilter;
        public float TextureMipMapBias;
        public uint TextureMaxMipLevel;
    }
    public class NinjaObjectVertex
    {
        public uint Type;
        public uint VTXFormat;
        public uint VTXFVF;
        public uint VTXSize;
        public List<NinjaObjectVertexList> Vertexes = new List<NinjaObjectVertexList>();
        public uint VTXBlendNum;
        public uint VTXHDRCommon;
        public uint VTXHDRData;
        public uint VTXHDRLock;
        public int VTXMTX;
    }
    public class NinjaObjectVertexList
    {
        public Vector3 Position;
        public Vector3 Weight3;
        public byte[] MTXIDX;
        public Vector3 Normals;
        public byte[] RGBA8888;
        public Vector2 ST0;
        public Vector3 Tan;
        public Vector3 BNormal;
        public Vector2 UnknownV2;
    }
    public class NinjaObjectPrimitive
    {
        public uint Type;
        public uint Format;
        public uint Index;
        public uint Strip;
        public uint IndexBuf;
        public ushort IndexLength;
        public List<ushort> VertexIndexList = new List<ushort>();
    }
    public class NinjaObjectNodeList
    {
        public uint Type;
        public ushort Matrix;
        public ushort Parent;
        public ushort Child;
        public ushort Sibling;
        public Vector3 Transform;
        public Vector3 Rotation;
        public Vector3 Scale;
        public List<float> Invinit = new List<float>();
        public Vector3 Center;
        public uint User;
        public uint RSV0;
        public uint RSV1;
        public uint RSV2;
    }
    public class NinjaObjectSubObject
    {
        public Vector3 Center;
        public float Radius;
        public uint Node;
        public uint Matrix;
        public uint Material;
        public uint VertexList;
        public uint PrimList;
        public uint ShaderList;
    }
}
