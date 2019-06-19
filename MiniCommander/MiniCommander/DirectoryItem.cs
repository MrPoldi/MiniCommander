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
    }
}
