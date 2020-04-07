using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HedgeLib.Sets
{
    public class S06Group
    {
        public string GroupName;
        public string GroupType;
        public int ObjectCount;
        public List<uint> ObjectList;

        public S06Group() { }

        public S06Group(string groupName, string groupType, int objectCount, List<uint> objectList)
        {
            GroupName = groupName;
            GroupType = groupType;
            ObjectCount = objectCount;
            ObjectList = objectList;
            Console.WriteLine(groupName);
            Console.WriteLine(groupType);
            Console.WriteLine(objectCount);
        }
    }
}
