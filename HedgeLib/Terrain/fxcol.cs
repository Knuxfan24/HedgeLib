using HedgeLib.Exceptions;
using HedgeLib.Headers;
using HedgeLib.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HedgeLib.Terrain
{
    public class fxcol : FileBase
    {
        // Variables/Constants
        public BINAHeader Header = new BINAv2Header(210);
        public const string Signature = "OCXF";
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
            var unknown1 = reader.ReadUInt32(); //Might be part of the Header if Skyth's svcol spec is anything to go on?
        }
    }
}
