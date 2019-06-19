using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniCommander
{
    class DirectoryItem : DiscItem
    {
        public DirectoryItem(string path) : base(path) { }

        public List<DirectoryItem> GetDirectories()
        {
            List<DirectoryItem> directories = new List<DirectoryItem>();
            try
            {
                string[] dirPaths = Directory.EnumerateDirectories(Path).ToArray();
                foreach (string dirPath in dirPaths)
                {
                    directories.Add(new DirectoryItem(dirPath));
                }
            }
            catch (Exception)
            {
                System.Windows.MessageBox.Show("Unauthorized access!");
                
            }

            return directories;
        }

        public List<FileItem> GetFiles()
        {
            List<FileItem> files = new List<FileItem>();

            try
            {
                string[] filePaths = Directory.EnumerateFiles(Path).ToArray();

                foreach (string filePath in filePaths)
                {
                    files.Add(new FileItem(filePath));
                }
            }
            catch (Exception)
            {
                
            }            

            return files;
        }

        public UpperDirectory GetUpperDirectory()
        {
            string path = this.Path;
            int count = 0;
            foreach (char c in path)
                if (c == '\\') count++;

            if (count == 1)
            {

                path = path.Substring(0, path.LastIndexOf(@"\") + 1);

            }
            else if (count > 1)
            {
                path = path.Substring(0, path.LastIndexOf(@"\"));
            }

            UpperDirectory upDir = new UpperDirectory(path);

            return upDir;
        }
    }
}
