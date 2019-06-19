using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using Path = System.IO.Path;

namespace MiniCommander
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window    {

        ListView currFilesView;

        DriveInfo[] drives;
        List<DiscItem> filesToCopy;

        List <DirectoryItem> directoriesL;
        List<FileItem> filesL;
        UpperDirectory upDirL;
        DirectoryItem dirL;
        bool ascendingSortNameL = true;
        bool ascendingSortDateL = false;

        List<DirectoryItem> directoriesR;
        List<FileItem> filesR;
        UpperDirectory upDirR;
        DirectoryItem dirR;
        bool ascendingSortNameR = true;
        bool ascendingSortDateR = false;        
        

        public MainWindow()
        {
            InitializeComponent();
            this.KeyDown += new KeyEventHandler(MainWindow_KeyDown);                
            currFilesView = FilesViewL;
        }        

        private void ChangeDirectory(object sender)
        {
            if (currFilesView == FilesViewL)
            {
                dirL = new DirectoryItem((sender as DirectoryItem).Path);
            }
            else
            {
                dirR = new DirectoryItem((sender as DirectoryItem).Path);
            }
            
            GetItems();
            RefreshFilesView();
        }

        private void GetItems()
        {
            if(currFilesView == FilesViewL)
            {
                directoriesL = dirL.GetDirectories();
                filesL = dirL.GetFiles();
                upDirL = GetUpperDirectory(dirL);
            }
            else
            {
                directoriesR = dirR.GetDirectories();
                filesR = dirR.GetFiles();
                upDirR = GetUpperDirectory(dirR);
            }
        }

        private void RefreshFilesView()
        {            
            if((dirL != null && dirR != null) && dirL.Path == dirR.Path)
            {
                ListView tempFV = currFilesView;

                FilesViewL.Items.Clear();
                FilesViewR.Items.Clear();
                FilesViewL.Items.Add(upDirL);
                FilesViewR.Items.Add(upDirR);

                currFilesView = FilesViewL;
                GetItems();
                foreach (DirectoryItem directory in directoriesL)
                {
                    FilesViewL.Items.Add(directory);                    
                }
                foreach (FileItem file in filesL)
                {
                    FilesViewL.Items.Add(file);                    
                }

                currFilesView = FilesViewR;
                GetItems();
                foreach (DirectoryItem directory in directoriesR)
                {                    
                    FilesViewR.Items.Add(directory);
                }
                foreach (FileItem file in filesR)
                {                   
                    FilesViewR.Items.Add(file);
                }

                currFilesView = tempFV;
            }
            else
            {
                currFilesView.Items.Clear();
                List<DirectoryItem> tempDirectories;
                List<FileItem> tempFiles;
                if (currFilesView == FilesViewL)
                {
                    currFilesView.Items.Add(upDirL);
                    tempDirectories = directoriesL;
                    tempFiles = filesL;
                }
                else
                {
                    currFilesView.Items.Add(upDirR);
                    tempDirectories = directoriesR;
                    tempFiles = filesR;
                }

                foreach (DirectoryItem directory in tempDirectories)
                {
                    currFilesView.Items.Add(directory);
                }

                foreach (FileItem file in tempFiles)
                {
                    currFilesView.Items.Add(file);
                }
            }
        }

        private UpperDirectory GetUpperDirectory(DirectoryItem dir)
        {
            string path = dir.Path;            
            int count = 0;
            foreach (char c in path)
                if (c == '\\') count++;

            if(count == 1)
            {
                
                path = path.Substring(0, path.LastIndexOf(@"\") + 1);

            }
            else if(count > 1)
            {
                path = path.Substring(0, path.LastIndexOf(@"\"));                
            }
            UpperDirectory upDir = new UpperDirectory(path);

            return upDir;
        }

        private void FilesView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DiscItem item = currFilesView.SelectedItem as DiscItem;
            if (item is FileItem)
            {
                System.Diagnostics.Process.Start(item.Path);
            }
            else if (item == null)
            {
                
            }
            else if(item is DirectoryItem)
            {
                ChangeDirectory(item);
            }
            
            
        }

        private void GridViewColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            var headerClicked = e.OriginalSource as GridViewColumnHeader;
            string header = headerClicked.Content as string;
            if (dirL != null && currFilesView == FilesViewL)
            {
                Sort(header, directoriesL, filesL);
            }
            else if (dirR != null && currFilesView == FilesViewR)
            {
                Sort(header, directoriesR, filesR);
            }
        }

        
        private void Sort(string sortBy, List<DirectoryItem> directories, List<FileItem> files)
        {
            bool ascendingSortName = true;
            bool ascendingSortDate = true;


            if (currFilesView == FilesViewL)
            {
                ascendingSortDate = ascendingSortDateL;
                ascendingSortName = ascendingSortNameL;
            }
            else if(currFilesView == FilesViewR)
            {
                ascendingSortDate = ascendingSortDateR;
                ascendingSortName = ascendingSortNameR;
            }            
            
            
            if(sortBy == "Name")
            {
                if (ascendingSortName)
                {
                    
                    directories.Sort(delegate (DirectoryItem x, DirectoryItem y)
                    {
                        return y.Name.CompareTo(x.Name);
                    });

                    files.Sort(delegate (FileItem x, FileItem y)
                    {
                        return y.Name.CompareTo(x.Name);
                    });
                    
                    ascendingSortName = false;
                }
                else
                {
                    directories.Sort(delegate (DirectoryItem x, DirectoryItem y)
                    {
                        return x.Name.CompareTo(y.Name);
                    });

                    files.Sort(delegate (FileItem x, FileItem y)
                    {
                        return x.Name.CompareTo(y.Name);
                    });
                    
                    ascendingSortName = true;                    
                }
            }

            if (sortBy == "Date")
            {
                if (ascendingSortDate)
                {
                    directories.Sort(delegate (DirectoryItem x, DirectoryItem y)
                    {
                        return y.CreationDate.CompareTo(x.CreationDate);
                    });

                    files.Sort(delegate (FileItem x, FileItem y)
                    {
                        return y.CreationDate.CompareTo(x.CreationDate);
                    });

                    ascendingSortDate = false;
                }
                else
                {
                    directories.Sort(delegate (DirectoryItem x, DirectoryItem y)
                    {
                        return x.CreationDate.CompareTo(y.CreationDate);
                    });

                    files.Sort(delegate (FileItem x, FileItem y)
                    {
                        return x.CreationDate.CompareTo(y.CreationDate);
                    });

                    ascendingSortDate = true;
                }
            }

            if (currFilesView == FilesViewL)
            {
                ascendingSortDateL = ascendingSortDate;
                ascendingSortNameL = ascendingSortName;
            }
            else if (currFilesView == FilesViewR)
            {
                ascendingSortDateR = ascendingSortDate;
                ascendingSortNameR = ascendingSortName;
            }

            RefreshFilesView();
        }

        private void ComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            drives = DriveInfo.GetDrives();
            foreach (DriveInfo drive in drives)
            {
                if (drive.IsReady)
                {
                    (sender as ComboBox).Items.Add(drive.Name);
                }
            }
        }

        private void DrivesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //change current files view
            if((sender as ComboBox).Name == "drivesComboBoxL")
            {
                currFilesView = FilesViewL;
                CopyL_Button.IsEnabled = true;                
            }
            else
            {
                currFilesView = FilesViewR;
                CopyR_Button.IsEnabled = true;                
            }

            CreateDirectoryButton.IsEnabled = true;
            DeleteDirectory_Button.IsEnabled = true;
            currFilesView.IsEnabled = true;
            string path = (sender as ComboBox).SelectedItem.ToString();
            DirectoryItem dir = new DirectoryItem(path);
            ChangeDirectory(dir);
        }
        

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F7)
            {
                CreateDirectory();
            }
            else if (e.Key == Key.F8 || e.Key == Key.Delete)
            {
                DeleteDiscItem();                
            }
            //else if(e.Key == Key.LeftCtrl && e.IsDown)
            //{
                                              
            //}
        }

        private void DeleteDiscItem()
        {
            List<DiscItem> deletePaths = new List<DiscItem>();
            foreach (DiscItem item in currFilesView.SelectedItems)
            {
                deletePaths.Add(item);
            }            

            foreach (DiscItem item in deletePaths)
            {
                if (item is DirectoryItem) Directory.Delete(item.Path, true);
                if (item is FileItem) File.Delete(item.Path);
            }
            
            GetItems();            
            RefreshFilesView();
        }

        private void CreateDirectory()
        {
            CreateDirectoryDialog dialog = new CreateDirectoryDialog();
            dialog.ShowDialog();
            string result = dialog.newDir;
            string newDirPath;
            if(currFilesView == FilesViewL)
            {
                newDirPath = dirL.Path + @"\" + result;
            }
            else
            {
                newDirPath = dirR.Path + @"\" + result;
            }
            

            Directory.CreateDirectory(newDirPath);
            //ListView tempFV = currFilesView;
            //currFilesView = FilesViewL;
            GetItems();
            //currFilesView = tempFV;
            RefreshFilesView();
        }        

        private void CreateDirectoryButton_Click(object sender, RoutedEventArgs e)
        {
            CreateDirectory();
        }

        private void ContextMenuDelete_Click(object sender, RoutedEventArgs e)
        {
            DeleteDiscItem();
        }

        private void ContextMenuNewDir_Click(object sender, RoutedEventArgs e)
        {
            CreateDirectory();
        }

        private void FilesViewL_GotFocus(object sender, RoutedEventArgs e)
        {
            currFilesView = FilesViewL;
        }

        private void FilesViewR_GotFocus(object sender, RoutedEventArgs e)
        {
            currFilesView = FilesViewR;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DeleteDiscItem();
        }

        private void Copy_Button_Click(object sender, RoutedEventArgs e)
        {
            CopyDiscItem();
        }

        private void CopyDiscItem()
        {
            filesToCopy = new List<DiscItem>();
            foreach(DiscItem item in currFilesView.SelectedItems)
            {
                filesToCopy.Add(item);
            }
            if (FilesViewR.IsEnabled)
            {
                PasteR_Button.IsEnabled = true;
            }
            if (FilesViewL.IsEnabled)
            {
                PasteR_Button_Copy.IsEnabled = true;
            }
        }

        private void PasteDiscItem()
        {
            string destDirPath;
            if (currFilesView == FilesViewL)
            {
                destDirPath = dirL.Path;
            }
            else
            {
                destDirPath = dirR.Path;
            }
            foreach (DiscItem item in filesToCopy)
            {
                if(item is FileItem)
                {
                    if (!File.Exists(Path.Combine(destDirPath, item.Name)))
                    {
                        File.Copy(item.Path, Path.Combine(destDirPath, item.Name));
                    }                    
                }
                else if(item is DirectoryItem)
                {   
                    if(!Directory.Exists(Path.Combine(destDirPath, item.Name)))
                    {
                        Directory.CreateDirectory(Path.Combine(destDirPath, item.Name));
                    }                    
                    PasteDiscItem(Path.Combine(destDirPath, item.Name), item as DirectoryItem);
                }
            }                
        }

        private void PasteDiscItem(string destDirPath, DirectoryItem dir)
        {
            List<DirectoryItem> dirs = dir.GetDirectories();
            List<FileItem> files = dir.GetFiles();
            foreach (FileItem file in files)
            {
                if (!File.Exists(Path.Combine(destDirPath, file.Name)))
                {
                    File.Copy(file.Path, Path.Combine(destDirPath, file.Name));
                }                
            }
            foreach (DirectoryItem directory in dirs)
            {
                if (!Directory.Exists(Path.Combine(destDirPath, directory.Name)))
                {
                    Directory.CreateDirectory(Path.Combine(destDirPath, directory.Name));
                }
                PasteDiscItem(Path.Combine(destDirPath, directory.Name), directory as DirectoryItem);
            }
        }

        private void PasteR_Button_Click(object sender, RoutedEventArgs e)
        {
            currFilesView = FilesViewR;
            PasteDiscItem();

            GetItems();
            RefreshFilesView();
        }

        private void PasteL_Button_Click(object sender, RoutedEventArgs e)
        {
            currFilesView = FilesViewL;
            PasteDiscItem();

            GetItems();
            RefreshFilesView();
        }

        
        

        private void FilesView_MouseMove(object sender, MouseEventArgs e)
        {            
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                CopyDiscItem();
                DragDrop.DoDragDrop(currFilesView,
                             currFilesView.SelectedItems,
                             DragDropEffects.Copy);
            }
        }

        private void FilesView_Drop(object sender, DragEventArgs e)
        {
            if(currFilesView != sender as ListView)
            {
                currFilesView = sender as ListView;
                PasteDiscItem();
                GetItems();
                RefreshFilesView();
            }            
        }
    }
}
