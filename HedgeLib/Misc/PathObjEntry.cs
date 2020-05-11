using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HedgeLib.Misc
{
    public class PathObjEntry
    {
        public List<PathObjEntryStrings> Objects = new List<PathObjEntryStrings>();

        public PathObjEntry() { }

        public PathObjEntry(List<PathObjEntryStrings> objects)
        {
            Objects = objects;
        }
    }
    public class PathObjEntryStrings
    {
        // Variables/Constants
        public List<string> ObjectValues = new List<string>();

        // Constructors
        public PathObjEntryStrings() { }
        public PathObjEntryStrings(List<string> objectValues)
        {
            ObjectValues = objectValues;
        }
    }
}
