using HedgeLib.Headers;
using HedgeLib.IO;
using System;
using System.Collections.Generic;
using System.IO;

namespace HedgeLib.Models
{
    public class Face
    {
        public ushort Vertex1;
        public ushort Vertex2;
        public ushort Vertex3;
        public uint Flags;
    }
    public class S06Coli : FileBase
    {
        public BINAHeader Header = new BINAv1Header();

        uint VertexCountOffsetOffset;
        uint PostFaceOffset;
        uint VertexCountOffset;
        uint FaceCountOffset;
        uint VertexCount;
        uint FaceCount;

        public List<Vector3> Vertices = new List<Vector3>();
        public List<Face> Faces = new List<Face>();
        public override void Load(Stream fileStream)
        {
            // Header
            var reader = new BINAReader(fileStream);
            Header = reader.ReadHeader();

            VertexCountOffsetOffset = reader.ReadUInt32(); //Refered to in LibS06 as `geometry_address`
            PostFaceOffset = reader.ReadUInt32(); //Refered to in LibS06 as `mopp_code_address`
            VertexCountOffset = reader.ReadUInt32(); //Refered to in LibS06 as `vertex_section_address`
            FaceCountOffset = reader.ReadUInt32(); //Refered to in LibS06 as `face_section_address`
            VertexCount = reader.ReadUInt32();

            //Vertexes
            for(int i = 0; i < VertexCount; i++)
            {
                Vertices.Add(reader.ReadVector3());
            }

            //Faces
            FaceCount = reader.ReadUInt32();
            for (int i = 0; i < FaceCount; i++)
            {
                Face face = new Face();
                face.Vertex1 = reader.ReadUInt16();
                face.Vertex2 = reader.ReadUInt16();
                face.Vertex3 = reader.ReadUInt16();
                reader.JumpAhead(2);
                face.Flags = reader.ReadUInt32();
                Faces.Add(face);
            }
        }

        public override void Save(Stream fileStream)
        {
            var writer = new BINAWriter(fileStream, Header);
            writer.AddOffset("VertexCountOffsetOffset");
            writer.Write(0); //PostFaceOffset (Not needed???)
            writer.FillInOffset("VertexCountOffsetOffset", false);
            writer.AddOffset("VertexCountOffset");
            writer.AddOffset("FaceCountOffset");

            writer.FillInOffset("VertexCountOffset", false);
            writer.Write(Vertices.Count);
            for(int i = 0; i < Vertices.Count; i++)
            {
                writer.Write(Vertices[i]);
            }

            writer.FillInOffset("FaceCountOffset", false);
            writer.Write(Faces.Count);
            for (int i = 0; i < Faces.Count; i++)
            {
                writer.Write(Faces[i].Vertex1);
                writer.Write(Faces[i].Vertex2);
                writer.Write(Faces[i].Vertex3);
                writer.WriteNulls(2);
                writer.Write(Faces[i].Flags);
            }

            writer.FinishWrite(Header);
        }

        public void ExportOBJ(string filepath)
        {
            using (StreamWriter log = new StreamWriter(filepath))
            {
                foreach (var vertex in Vertices)
                {
                    log.WriteLine($"v {vertex.X} {vertex.Y} {vertex.Z}");
                }

                foreach (var face in Faces)
                {
                    log.WriteLine($"g {Path.GetFileNameWithoutExtension(filepath)}_{face.Flags}");
                    log.WriteLine($"f {face.Vertex1 + 1} {face.Vertex2 + 1} {face.Vertex3 + 1}");
                }
            }
        }
    }
}
