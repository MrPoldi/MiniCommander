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

    }
}
