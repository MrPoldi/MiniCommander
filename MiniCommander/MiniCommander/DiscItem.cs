using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniCommander
{
    abstract class DiscItem
    {
        private string path;
        protected string name;
        protected DateTime creationDate;  

        public DateTime CreationDate
        {
            get { return creationDate; }            
        }
        public string Name
        {
            get { return name; }            
        }
        public string Path
        {
            get { return path; }            
        }

        public DiscItem(string path)
        {
            this.path = path;            
            creationDate = File.GetCreationTime(path);
            name = path.Substring(path.LastIndexOf(@"\") + 1);
        }

        public class SortByNameAscending : IComparer<DiscItem>
        {
            public int Compare(DiscItem x, DiscItem y)
            {
                return String.Compare(x.name, y.name);
            }
        }
        public class SortByNameDescending : IComparer<DiscItem>
        {
            public int Compare(DiscItem x, DiscItem y)
            {
                return String.Compare(y.name, x.name);
            }

        }
        public class SortByDateAscending : IComparer<DiscItem>
        {
            public int Compare(DiscItem x, DiscItem y)
            {
                return DateTime.Compare(x.creationDate, y.creationDate);
            }
        }
        public class SortByDateDescending : IComparer<DiscItem>
        {
            public int Compare(DiscItem x, DiscItem y)
            {
                return DateTime.Compare(y.creationDate, x.creationDate);
            }
        }

    }
}
