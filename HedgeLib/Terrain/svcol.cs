using HedgeLib.Exceptions;
using HedgeLib.Headers;
using HedgeLib.IO;
using HedgeLib.Math;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace HedgeLib.Terrain
{
    //Based on TGE's 010 Editor template created from Skyth's spec.
    public class SvShape
    {
        public string Name;
        public uint Unknown1;
        public Vector3 Size;
        public Vector3 Position;
        public Quaternion Rotation = new Quaternion();
        public AABB BoundingBox = new AABB();
        public uint Unknown2;
        public List<SvSector> Sectors = new List<SvSector>();
    }

    public class SvSector
    {
        public int SectorIndex;
        public bool Visible;
    }
    public class svcol : FileBase
    {
        // Variables/Constants
        public BINAHeader Header = new BINAv2Header(210);
        public const string Signature = "OCVS";
        public List<SvShape> SvShapes = new List<SvShape>();

        // Methods
        public override void Load(Stream fileStream)
        {
            // BINA Header
            var reader = new BINAReader(fileStream);
            Header = reader.ReadHeader();

            string sig = reader.ReadSignature(4);
            if (sig != Signature)
                throw new InvalidSignatureException(Signature, sig);
            var unknown1 = reader.ReadUInt32(); //Might be part of the Header according to Skyth's spec?
            if (unknown1 != 1) { Console.WriteLine($"Unknown1 does not equal 1 in this file! It's actually set to {unknown1}"); }
            var shapeCount = reader.ReadInt64();
            var unknown2 = reader.ReadUInt64();
            if (unknown2 != 24) { Console.WriteLine($"Unknown1 does not equal 24 in this file! It's actually set to {unknown1}"); }
            Console.WriteLine(unknown2);

            for (int i = 0; i < shapeCount; i++)
            {
                SvShape shape = new SvShape();
                var shapeNameOffset = reader.ReadInt64();
                long pos = reader.BaseStream.Position;
                reader.JumpTo(shapeNameOffset, false);
                shape.Name = reader.ReadNullTerminatedString();
                reader.JumpTo(pos, true);

                shape.Unknown1 = reader.ReadUInt32();
                shape.Size = reader.ReadVector3();
                shape.Position = reader.ReadVector3();
                shape.Rotation = reader.ReadQuaternion();
                shape.BoundingBox.Minimum = reader.ReadVector3();
                shape.BoundingBox.Maximum = reader.ReadVector3();
                shape.Unknown2 = reader.ReadUInt32();

                var sectorCount = reader.ReadInt64();
                var sectorListOffset = reader.ReadInt64();
                pos = reader.BaseStream.Position;
                reader.JumpTo(sectorListOffset, false);
                for(int s = 0; s < sectorCount; s++ )
                {
                    SvSector sector = new SvSector();
                    sector.SectorIndex = reader.Read();
                    sector.Visible = reader.ReadBoolean();
                    shape.Sectors.Add(sector);
                }
                reader.JumpTo(pos, true);
                SvShapes.Add(shape);
            }
        }
        public override void Save(Stream fileStream)
        {
            //The file this produces needs the footer hex edited in some way to correct it (usually changing it to DBVBVB etc etc)
            var writer = new BINAWriter(fileStream, Header);
            writer.WriteSignature(Signature);
            writer.Write(1); //Unknown 1
            writer.Write(Convert.ToInt64(SvShapes.Count));
            writer.Write(24L); //Unknown 2

            for(int i = 0; i < SvShapes.Count; i++)
            {
                writer.AddString($"ShapeName{i}", SvShapes[i].Name, 8u);
                writer.Write(SvShapes[i].Unknown1);
                writer.Write(SvShapes[i].Size);
                writer.Write(SvShapes[i].Position);
                writer.Write(SvShapes[i].Rotation);
                writer.Write(SvShapes[i].BoundingBox.Minimum);
                writer.Write(SvShapes[i].BoundingBox.Maximum);
                writer.Write(SvShapes[i].Unknown2);
                writer.Write(Convert.ToInt64(SvShapes[i].Sectors.Count));
                writer.AddOffset($"SectorsOffset{i}", 8u);
            }

            for(int i = 0; i < SvShapes.Count; i++)
            {
                writer.FillInOffset($"SectorsOffset{i}", false);
                for (int s = 0; s < SvShapes[i].Sectors.Count; s++)
                {
                    writer.Write(Convert.ToByte(SvShapes[i].Sectors[s].SectorIndex));
                    writer.Write(SvShapes[i].Sectors[s].Visible);
                }
            }

            writer.WriteNulls(0x30);
            writer.FinishWrite(Header);
        }
        public void ExportXML(string filePath)
        {
            var rootElem = new XElement("svcol");

            foreach(var shape in SvShapes)
            {
                var shapeElem = new XElement("Shape");
                var shapeNameElem = new XElement("Name", shape.Name);
                var unknown1Elem = new XElement("Unknown1", shape.Unknown1);

                var sizeElem = new XElement("Size");
                var sizeXElem = new XElement("X", shape.Size.X);
                var sizeYElem = new XElement("Y", shape.Size.Y);
                var sizeZElem = new XElement("Z", shape.Size.Z);
                sizeElem.Add(sizeXElem, sizeYElem, sizeZElem);

                var positionElem = new XElement("Position");
                var positionXElem = new XElement("X", shape.Position.X);
                var positionYElem = new XElement("Y", shape.Position.Y);
                var positionZElem = new XElement("Z", shape.Position.Z);
                positionElem.Add(positionXElem, positionYElem, positionZElem);

                var rotationElem = new XElement("Rotation");
                var rotationXElem = new XElement("X", shape.Rotation.X);
                var rotationYElem = new XElement("Y", shape.Rotation.Y);
                var rotationZElem = new XElement("Z", shape.Rotation.Z);
                var rotationWElem = new XElement("W", shape.Rotation.W);
                rotationElem.Add(rotationXElem, rotationYElem, rotationZElem, rotationWElem);

                var AABBElem = new XElement("AABB");
                var AABBMinElem = new XElement("Minimum");
                var AABBMinXElem = new XElement("X", shape.BoundingBox.Minimum.X);
                var AABBMinYElem = new XElement("Y", shape.BoundingBox.Minimum.Y);
                var AABBMinZElem = new XElement("Z", shape.BoundingBox.Minimum.Z);
                AABBMinElem.Add(AABBMinXElem, AABBMinYElem, AABBMinZElem);
                var AABBMaxElem = new XElement("Maximum");
                var AABBMaxXElem = new XElement("X", shape.BoundingBox.Maximum.X);
                var AABBMaxYElem = new XElement("Y", shape.BoundingBox.Maximum.Y);
                var AABBMaxZElem = new XElement("Z", shape.BoundingBox.Maximum.Z);
                AABBMaxElem.Add(AABBMaxXElem, AABBMaxYElem, AABBMaxZElem);
                AABBElem.Add(AABBMinElem, AABBMaxElem);

                var unknown2Elem = new XElement("Unknown2", shape.Unknown2);
                shapeElem.Add(shapeNameElem, unknown1Elem, sizeElem, positionElem, rotationElem, AABBElem, unknown2Elem);

                foreach(var sector in shape.Sectors)
                {
                    var sectorElem = new XElement("Sector");
                    var sectorIndexElem = new XElement("Index", sector.SectorIndex);
                    var sectorVisibleElem = new XElement("IsVisible", sector.Visible);
                    sectorElem.Add(sectorIndexElem, sectorVisibleElem);
                    shapeElem.Add(sectorElem);
                }

                rootElem.Add(shapeElem);
            }

            var xml = new XDocument(rootElem);
            xml.Save(filePath);
        }
    }
}
