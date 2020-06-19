using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HedgeLib.Terrain;

namespace xCol_Test
{
    class Program
    {
        static void Main(string[] args)
        {
            //svcol col = new svcol();
            //col.Load(@"D:\Steam\steamapps\common\SonicForces\build\main\projects\exec\mods\Stupid Test Mod\disk\wars_patch\w5a01\w5a01_trr_cmn\w5a01_svcol.svcol.bin");
            //col.ExportXML(@"D:\Steam\steamapps\common\SonicForces\build\main\projects\exec\mods\Stupid Test Mod\disk\wars_patch\w5a01\w5a01_trr_cmn\w5a01_svcol.svcol.xml");
            /*
            //var files = Directory.GetFiles(@"G:\Sonic Forces\Game Dump", "*.svcol.bin", SearchOption.AllDirectories);
            //foreach(var file in files)
            //{
            //    Console.WriteLine(file);
            //    svcol col = new svcol();
            //    col.Load(file);
            //    Console.WriteLine();
            //}*/
            svcol col = new svcol();
            SvShape shape = new SvShape();
            shape.Name = "svShapeCube18";
            shape.Unknown1 = 513u;
            shape.Size.X = 4000f;
            shape.Size.Y = 4000f;
            shape.Size.Z = 2660f;
            shape.Position.X = 0f;
            shape.Position.Y = 0f;
            shape.Position.Z = 335f;
            shape.Rotation.X = 0f;
            shape.Rotation.Y = 0f;
            shape.Rotation.Z = 0f;
            shape.Rotation.W = 1f;
            shape.BoundingBox.Minimum.X = -2000f;
            shape.BoundingBox.Minimum.Y = -2000f;
            shape.BoundingBox.Minimum.Z = -660f;
            shape.BoundingBox.Maximum.X = 2000f;
            shape.BoundingBox.Maximum.Y = 2000f;
            shape.BoundingBox.Maximum.Z = 2000f;
            SvSector sector = new SvSector();
            sector.SectorIndex = 16;
            sector.Visible = true;
            shape.Sectors.Add(sector);
            col.SvShapes.Add(shape);
            col.Save(@"D:\Steam\steamapps\common\SonicForces\build\main\projects\exec\mods\Stupid Test Mod\disk\wars_patch\w5a01\w5a01_trr_cmn\w5a01_svcol.svcol.bin");
        }
    }
}
