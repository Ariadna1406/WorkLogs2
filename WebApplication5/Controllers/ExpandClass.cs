using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApplication5.Controllers
{
    public static class ExpandClass
    {

        public static List<DirectoryEntry> ChildrenToList(this DirectoryEntry directoryEntry)
        {
            List<DirectoryEntry> childrenList = new List<DirectoryEntry>();
            foreach (var entry in directoryEntry.Children)
            {
                var ent = entry as DirectoryEntry;
                childrenList.Add(ent);
            }
            return childrenList;

        }


    }
}
