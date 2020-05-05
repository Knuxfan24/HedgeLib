using HedgeLib.Headers;
using HedgeLib.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace HedgeLib.Sets
{
    public class S06SetData : SetData
    {
        // Variables/Constants
        public BINAHeader Header = new BINAv1Header();
        public const string Extension = ".set";

        // Methods
        public override void Load(Stream fileStream,
            Dictionary<string, SetObjectType> objectTemplates)
        {
            // Header
            var reader = new BINAReader(fileStream);
            Header = reader.ReadHeader();

            //Set Name (hardcoded to ASSUME it's four characters long)
            long namePosition = reader.BaseStream.Position; //Save position so we can jump back after reading name, type and parameters
            reader.JumpAhead(0xC);
            Name = reader.ReadNullTerminatedString();
            reader.JumpTo(namePosition, true);
            reader.JumpAhead(0x2C);

            uint objectCount = reader.ReadUInt32();
            uint objectTableOffset = reader.ReadUInt32();
            uint groupCount = reader.ReadUInt32();
            uint groupTableOffset = reader.ReadUInt32();

            //Objects
            reader.JumpTo(objectTableOffset, false);
            for(uint i = 0; i < objectCount; i++)
            {
                var obj = new SetObject();
                obj.ObjectID = i;
                uint objectNameOffset = reader.ReadUInt32();
                uint objectTypeOffset = reader.ReadUInt32();
                obj.UnknownBytes = reader.ReadBytes(16); //parameter.Unknown 16 bytes (pattern tends to be 40 00 00 00/01 (depending on whether object is activated by a group) 00 00 00 00 00 00 00 00 00 00 00 00)
                obj.Transform.Position = reader.ReadVector3();
                obj.DrawDistance = reader.ReadSingle();
                obj.Transform.Rotation = reader.ReadQuaternion();
                uint parameterCount = reader.ReadUInt32();
                uint parameterOffset = reader.ReadUInt32();
                
                long position = reader.BaseStream.Position; //Save position so we can jump back after reading name, type and parameters

                //Object Name and Type
                reader.JumpTo(objectNameOffset, false);
                obj.ObjectName = reader.ReadNullTerminatedString();
                reader.JumpTo(objectTypeOffset, false);
                obj.ObjectType = reader.ReadNullTerminatedString();

                reader.JumpTo(parameterOffset, false);
                //Object Parameters
                for (uint c = 0; c < parameterCount; c++)
                {
                    var parameter = new SetObjectParam();
                    uint parameterType = reader.ReadUInt32();
                    switch (parameterType)
                    {
                        case 0: //boolean
                            parameter.DataType = typeof(bool);
                            parameter.Data = reader.ReadUInt32() == 1;
                            parameter.Unknown1 = reader.ReadUInt32();
                            parameter.Unknown2 = reader.ReadUInt32();
                            parameter.Unknown3 = reader.ReadUInt32();
                            break;
                        case 1: //int
                            parameter.DataType = typeof(int);
                            parameter.Data = reader.ReadInt32();
                            parameter.Unknown1 = reader.ReadUInt32();
                            parameter.Unknown2 = reader.ReadUInt32();
                            parameter.Unknown3 = reader.ReadUInt32();
                            break;
                        case 2: //single
                            parameter.DataType = typeof(float);
                            parameter.Data = reader.ReadSingle();
                            parameter.Unknown1 = reader.ReadUInt32();
                            parameter.Unknown2 = reader.ReadUInt32();
                            parameter.Unknown3 = reader.ReadUInt32();
                            break;
                        case 3: //string
                            uint offset = reader.ReadUInt32();
                            parameter.Unknown1 = reader.ReadUInt32();
                            parameter.Unknown2 = reader.ReadUInt32();
                            parameter.Unknown3 = reader.ReadUInt32();

                            long stringParameterPosition = reader.BaseStream.Position; //Save position so we can jump back after reading name, type and parameters
                            reader.JumpTo(offset, false);

                            parameter.DataType = typeof(string);
                            parameter.Data = reader.ReadNullTerminatedString();

                            reader.JumpTo(stringParameterPosition, true);
                            break;
                        case 4: //Vector3
                            parameter.DataType = typeof(Vector3);
                            parameter.Data = reader.ReadVector3();
                            parameter.Unknown3 = reader.ReadUInt32();
                            break;
                        case 6: //uint
                            parameter.DataType = typeof(uint);
                            parameter.Data = reader.ReadUInt32();
                            parameter.Unknown1 = reader.ReadUInt32();
                            parameter.Unknown2 = reader.ReadUInt32();
                            parameter.Unknown3 = reader.ReadUInt32();
                            break;
                        default:
                            Console.WriteLine("Unhandled Data Type!");
                            break;
                    }
                    obj.Parameters.Add(parameter);
                }

                //Save Object and jump back for the next one
                Objects.Add(obj);
                reader.JumpTo(position, true);
            }

            //Groups
            reader.JumpTo(groupTableOffset, false);
            for (uint i = 0; i < groupCount; i++)
            {
                var group = new SetGroup();
                uint groupNameOffset = reader.ReadUInt32();
                uint groupTypeOffset = reader.ReadUInt32();
                group.GroupObjectCount = reader.ReadUInt32();
                uint groupObjectListOffset = reader.ReadUInt32();

                long position = reader.BaseStream.Position; //Save position so we can jump back after reading name, type and object list

                //Group Name and Type
                reader.JumpTo(groupNameOffset, false);
                group.GroupName = reader.ReadNullTerminatedString();
                reader.JumpTo(groupTypeOffset, false);
                group.GroupType = reader.ReadNullTerminatedString();

                //Group Object List
                reader.JumpTo(groupObjectListOffset, false);
                for (uint c = 0; c < group.GroupObjectCount; c++)
                {
                    reader.JumpAhead(4);
                    group.ObjectIDs.Add(reader.ReadUInt32());
                }

                //Save Group and jump back for the next one
                Groups.Add(group);
                reader.JumpTo(position, true);
            }
        }

        public override void Save(Stream fileStream)
        {
            var writer = new BINAWriter(fileStream, Header);
            uint stringParamCount = 0;

            // Header
            writer.WriteNulls(0xC);
            writer.WriteNullTerminatedString(Name);
            writer.WriteNulls(0x1B);
            writer.Write(Objects.Count);
            writer.AddOffset("objectTableOffset");
            writer.Write(Groups.Count);
            if (Groups.Count != 0)
            {
                writer.AddOffset("groupTableOffset");
            }
            else
            {
                writer.Write(0);
            }

            //Objects
            writer.FillInOffset("objectTableOffset", false);
            for (int i = 0; i < Objects.Count; i++)
            {
                //Object Values
                writer.AddString($"objectNameOffset{i}", $"{Objects[i].ObjectName}");
                writer.AddString($"objectTypeOffset{i}", $"{Objects[i].ObjectType}");
                writer.Write(Objects[i].UnknownBytes);
                writer.Write(Objects[i].Transform.Position);
                writer.Write(Objects[i].DrawDistance);
                writer.Write(Objects[i].Transform.Rotation);
                writer.Write(Objects[i].Parameters.Count);
                if (Objects[i].Parameters.Count != 0)
                {
                    writer.AddOffset($"parameterOffset{i}");
                }
                else
                {
                    writer.Write(0);
                }
            }

            for (int i = 0; i < Objects.Count; i++)
            {
                if (Objects[i].Parameters.Count != 0)
                {
                    writer.FillInOffset($"parameterOffset{i}", false);
                    foreach (var parameter in Objects[i].Parameters)
                    {
                        switch(parameter.DataType.ToString())
                        {
                            case "System.Boolean":
                                writer.Write(0);
                                writer.Write(((bool)parameter.Data) ? 1 : 0);
                                writer.Write(parameter.Unknown1);
                                writer.Write(parameter.Unknown2);
                                break;
                            case "System.Int32":
                                writer.Write(1);
                                writer.Write((int)parameter.Data);
                                writer.Write(parameter.Unknown1);
                                writer.Write(parameter.Unknown2);
                                break;
                            case "System.Single":
                                writer.Write(2);
                                writer.Write((float)parameter.Data);
                                writer.Write(parameter.Unknown1);
                                writer.Write(parameter.Unknown2);
                                break;
                            case "System.String":
                                writer.Write(3);
                                writer.AddString($"stringParamOffset{stringParamCount}", (string)parameter.Data);
                                writer.Write(parameter.Unknown1);
                                writer.Write(parameter.Unknown2);
                                stringParamCount++;
                                break;
                            case "HedgeLib.Vector3":
                                writer.Write(4);
                                writer.Write((Vector3)parameter.Data);
                                break;
                            case "System.UInt32":
                                writer.Write(6);
                                writer.Write((uint)parameter.Data);
                                writer.Write(parameter.Unknown1);
                                writer.Write(parameter.Unknown2);
                                break;
                            default:
                                Console.WriteLine(parameter.DataType.ToString());
                                writer.Write(0);
                                writer.Write(0);
                                writer.Write(0);
                                writer.Write(0);
                                break;
                        }
                        writer.Write(parameter.Unknown3);
                    }
                }
                /*
                //Object Parameters
                if (Objects[i].Parameters.Count != 0)
                {
                    Console.WriteLine($"Object {i}");
                    writer.FillInOffset($"parameterOffset{i}", false);
                    foreach (var parameter in Objects[i].Parameters)
                    {
                        if (parameter.DataType.ToString() == "System.Boolean")
                        {
                            Console.WriteLine("Boolean");
                            writer.Write(66);
                            writer.Write(((bool)parameter.Data) ? 1 : 0);
                            writer.Write(parameter.Unknown1);
                            writer.Write(parameter.Unknown2);
                            writer.Write(parameter.Unknown3);
                        }
                        else if (parameter.DataType.ToString() == "System.Int32")
                        {
                            Console.WriteLine("Int32");
                            writer.Write(77);
                            writer.Write((int)parameter.Data);
                            writer.Write(parameter.Unknown1);
                            writer.Write(parameter.Unknown2);
                            writer.Write(parameter.Unknown3);
                        }
                        else if (parameter.DataType.ToString() == "System.Single")
                        {
                            Console.WriteLine("Single");
                            writer.Write(88);
                            writer.Write((float)parameter.Data);
                            writer.Write(parameter.Unknown1);
                            writer.Write(parameter.Unknown2);
                            writer.Write(parameter.Unknown3);
                        }
                        else if (parameter.DataType.ToString() == "System.String")
                        {
                            Console.WriteLine("String");
                            writer.Write(99);
                            writer.AddString($"offset{stringParamCount}", (string)parameter.Data);
                            writer.Write(parameter.Unknown1);
                            writer.Write(parameter.Unknown2);
                            writer.Write(parameter.Unknown3);

                            ++stringParamCount;
                        }
                        else if (parameter.DataType.ToString() == "HedgeLib.Vector3")
                        {
                            Console.WriteLine("Vector3");
                            writer.Write(55);
                            writer.Write((Vector3)parameter.Data);
                            writer.Write(parameter.Unknown3);
                        }
                        else if (parameter.DataType.ToString() == "System.UInt32")
                        {
                            Console.WriteLine("UInt32");
                            writer.Write(44);
                            writer.Write((uint)parameter.Data);
                            writer.Write(parameter.Unknown1);
                            writer.Write(parameter.Unknown2);
                            writer.Write(parameter.Unknown3);
                        }
                        else
                        {
                            Console.WriteLine("Unhandled Data Type!");
                            writer.Write(0L);
                        }
                        writer.FixPadding(0x14);
                    }
                }*/
            }

            //Groups
            if (Groups.Count != 0)
            {
                writer.FillInOffset("groupTableOffset", false);
                for (int i = 0; i < Groups.Count; i++)
                {
                    writer.AddString($"groupNameOffset{i}", $"{Groups[i].GroupName}");
                    writer.AddString($"groupTypeOffset{i}", $"{Groups[i].GroupType}");
                    writer.Write(Groups[i].GroupObjectCount);
                    writer.AddOffset($"groupObjectList{i}");
                }


                for (int i = 0; i < Groups.Count; i++)
                {
                    writer.FillInOffset($"groupObjectList{i}", false);
                    for (int c = 0; c < Groups[i].ObjectIDs.Count; c++)
                    {
                        writer.Write(0);
                        writer.Write(Groups[i].ObjectIDs[c]);
                    }
                }
            }
            //writer.WriteNulls(572);
            // Write Footer
            writer.FinishWrite(Header);
        }
    }
}
