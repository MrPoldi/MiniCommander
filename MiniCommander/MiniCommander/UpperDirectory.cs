using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniCommander
{
    class UpperDirectory : DirectoryItem
    {
        public UpperDirectory(string path) : base(path)
        {
            this.creationDate = new DateTime();            
            this.name = "[..] " + this.Path;
        }
    }
}
