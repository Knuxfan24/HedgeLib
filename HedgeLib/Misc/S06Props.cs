using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HedgeLib.IO;
using HedgeLib.Exceptions;
using HedgeLib.Headers;
using System.IO;
using System.Xml.Linq;

namespace HedgeLib.Misc
{
    public class S06Props : FileBase
    {
        public BINAHeader Header = new BINAv1Header();
        public S06Prop prop = new S06Prop();
        public override void Load(Stream fileStream)
        {
            // Header
            var reader = new BINAReader(fileStream);
            Header = reader.ReadHeader();
            long namePositionHack = reader.BaseStream.Position;
            reader.JumpAhead(0xC);
            prop.Name = reader.ReadNullTerminatedString();
            reader.JumpTo(namePositionHack, true);
            reader.JumpTo(0x4c, true);
            prop.ObjectCount = reader.ReadUInt32();
            reader.JumpAhead(0x4);
            for (uint i = 0; i < prop.ObjectCount; i++)
            {
                S06PropObject obj = new S06PropObject();

                var objectNameOffset = reader.ReadUInt32();
                obj.ObjectParameterCount = reader.ReadUInt32();
                var paramOffset = reader.ReadUInt32();
                obj.ObjectUnknown1 = reader.ReadUInt32();
                obj.ObjectUnknown2 = reader.ReadUInt32();

                long position = reader.BaseStream.Position;

                reader.JumpTo(objectNameOffset, false);
                obj.ObjectName = reader.ReadNullTerminatedString();

                for (uint c = 0; c < obj.ObjectParameterCount; c++)
                {
                    reader.JumpTo(paramOffset + (c * 0x18), false);

                    S06PropParameter parameter = new S06PropParameter();
                    parameter.ParameterName = reader.ReadNullTerminatedString();
                    reader.JumpTo(paramOffset + (c * 0x18) + 0x10, false);

                    parameter.ParameterType = reader.ReadUInt32();
                    parameter.ParameterID = reader.ReadUInt32();

                    obj.Parameters.Add(parameter);
                }

                prop.Objects.Add(obj);

                reader.JumpTo(position, true);
            }

        }

        public void ExportXML(string filepath)
        {
            var rootElem = new XElement("Prop");
            var propNameAttr = new XAttribute("name", prop.Name);
            rootElem.Add(propNameAttr);
            int index = 0;
            foreach (var obj in prop.Objects)
            {
                var objectElem = new XElement("Object");
                var objectNameAttr = new XAttribute("name", obj.ObjectName);
                var objectParamCountAttr = new XAttribute("paramcount", obj.ObjectParameterCount);
                var objectUnknown1Attr = new XAttribute("unknown1", obj.ObjectUnknown1);
                var objectUnknown2Attr = new XAttribute("unknown2", obj.ObjectUnknown2);
                objectElem.Add(objectNameAttr, objectParamCountAttr, objectUnknown1Attr, objectUnknown2Attr);

                foreach(var param in obj.Parameters)
                {
                    var paramElem = new XElement("Parameter");
                    var paramName = new XElement("Name", param.ParameterName);
                    var paramType = new XElement("Type", param.ParameterType);
                    var paramID = new XElement("ID", param.ParameterID);

                    paramElem.Add(paramName, paramType, paramID);
                    objectElem.Add(paramElem);
                }

                rootElem.Add(objectElem);
                index++;
            }

            var xml = new XDocument(rootElem);
            xml.Save(filepath);
        }
    
    }
}
