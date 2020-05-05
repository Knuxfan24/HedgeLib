using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HedgeLib.Sets
{
    public class SetGroup
    {
        public string GroupName;
        public string GroupType;
        public uint GroupObjectCount;
        public List<uint> ObjectIDs = new List<uint>();
        public SetGroup() { }
    }
}
