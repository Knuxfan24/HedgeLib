using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HedgeLib.Misc
{
    public class S06Prop
    {
        public string Name;
        public uint ObjectCount;
        public List<S06PropObject> Objects = new List<S06PropObject>();

        public S06Prop() { }

        public S06Prop(uint objectCount, string name)
        {
            Name = name;
            ObjectCount = objectCount;
        }
    }
    public class S06PropObject
    {
        // Variables/Constants
        public string ObjectName;
        public uint ObjectParameterCount;
        public uint ObjectUnknown1;
        public uint ObjectUnknown2;
        public List<S06PropParameter> Parameters = new List<S06PropParameter>();

        // Constructors
        public S06PropObject() { }
        public S06PropObject(string objectName, uint objectParameterCount, uint objectUnknown1, uint objectUnknown2)
        {
            ObjectName = objectName;
            ObjectParameterCount = objectParameterCount;
            ObjectUnknown1 = objectUnknown1;
            ObjectUnknown2 = objectUnknown2;
        }
    }
    public class S06PropParameter
    {
        // Variables/Constants
        public string ParameterName;
        public uint ParameterType;
        public uint ParameterID;

        // Constructors
        public S06PropParameter() { }
        public S06PropParameter(string parameterName, uint parameterType, uint parameterID)
        {
            ParameterName = parameterName;
            ParameterType = parameterType;
            ParameterID = parameterID;
        }
    }
}
