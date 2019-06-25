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
        #region Fields
        
        ListView currFilesView;
        DriveInfo[] drives;
        List<DiscItem> filesToCopy;

        private bool canRefreshBoth = true;
        private bool isDragging = false;
        private bool isTrack = false;
        private Point clickPoint;

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

        #endregion

        #region Constructors
        public MainWindow()
        {
            InitializeComponent();
            this.KeyDown += new KeyEventHandler(MainWindow_KeyDown);                
            currFilesView = FilesViewL;
        }
        #endregion

        #region Operations on Files and Directories
        /// <summary>
        /// Get items in directory and it's upper directory
        /// </summary>
        private void GetItems()
        {
            if (currFilesView == FilesViewL)
            {
                directoriesL = dirL.GetDirectories();
                filesL = dirL.GetFiles();
                upDirL = dirL.GetUpperDirectory();
                ascendingSortNameL = true;
            }
            else
            {
                directoriesR = dirR.GetDirectories();
                filesR = dirR.GetFiles();
                upDirR = dirR.GetUpperDirectory();
                ascendingSortNameR = true;
            }
        }        

        /// <summary>
        /// Change directory in current FilesView and adequate field
        /// </summary>
        /// <param name="sender"></param>
        private void ChangeDirectory(object sender)
        {
            if (currFilesView == FilesViewL)
            {
                dirL = new DirectoryItem((sender as DirectoryItem).Path);
                ascendingSortNameL = true;
            }
            else
            {
                dirR = new DirectoryItem((sender as DirectoryItem).Path);
                ascendingSortNameR = true;
            }

            GetItems();
            RefreshFilesView();
        }

        /// <summary>
        /// Delete selected files and directories recursively
        /// </summary>
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

        /// <summary>
        /// Open a dialog to create a new dir and create a new directory
        /// </summary>
        private void CreateDirectory()
        {
            CreateDirectoryDialog dialog = new CreateDirectoryDialog();
            dialog.ShowDialog();
            string result = dialog.newDir;
            string newDirPath;
            if (currFilesView == FilesViewL)
            {
                newDirPath = dirL.Path + @"\" + result;
            }
            else
            {
                newDirPath = dirR.Path + @"\" + result;
            }


            Directory.CreateDirectory(newDirPath);
            GetItems();
            RefreshFilesView();
        }

        #region Copy and Paste

        /// <summary>
        /// Add selected files to a list and enable paste buttons
        /// </summary> 
        private void CopyDiscItem()
        {
            // TODO: Repair enabling buttons            
            filesToCopy = null;
            filesToCopy = new List<DiscItem>();
            foreach (DiscItem item in currFilesView.SelectedItems)
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

        /// <summary>
        /// Paste files from filesToCopy list to a destination directory
        /// <para>It uses an overloaded method for recursive copying of directories</para>
        /// </summary>
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
                if (item is FileItem)
                {
                    if (!File.Exists(Path.Combine(destDirPath, item.Name)))
                    {
                        File.Copy(item.Path, Path.Combine(destDirPath, item.Name));
                    }
                }
                else if (item is DirectoryItem)
                {
                    if (!Directory.Exists(Path.Combine(destDirPath, item.Name)))
                    {
                        Directory.CreateDirectory(Path.Combine(destDirPath, item.Name));
                    }
                    PasteDiscItem(Path.Combine(destDirPath, item.Name), item as DirectoryItem);
                }
            }
        }

        /// <summary>
        /// Copies directories recursively with their files
        /// </summary>
        /// <param name="destDirPath">Destination path</param>
        /// <param name="dir">Currently copied directory</param>
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
        #endregion

        #endregion

        #region FilesView Methods

        /// <summary>
        /// Refresh the contents of FilesViews
        /// </summary>
        private void RefreshFilesView()
        {
            if ((dirL != null && dirR != null) && dirL.Path == dirR.Path && canRefreshBoth)
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

        /// <summary>
        /// Sort file and directory lists
        /// </summary>
        /// <param name="sortBy">Name or Date</param>
        /// <param name="directories">List of directories to sort</param>
        /// <param name="files">List of files to sort</param>        
        private void Sort(string sortBy, List<DirectoryItem> directories, List<FileItem> files)
        {
            bool ascendingSortName = true;
            bool ascendingSortDate = true;


            if (currFilesView == FilesViewL)
            {
                ascendingSortDate = ascendingSortDateL;
                ascendingSortName = ascendingSortNameL;
            }
            else if (currFilesView == FilesViewR)
            {
                ascendingSortDate = ascendingSortDateR;
                ascendingSortName = ascendingSortNameR;
            }


            if (sortBy == "Name")
            {
                if (ascendingSortName)
                {
                    directories.Sort(new DiscItem.SortByNameDescending());
                    files.Sort(new DiscItem.SortByNameDescending());
                    
                    ascendingSortName = false;
                }
                else
                {
                    directories.Sort(new DiscItem.SortByNameAscending());
                    files.Sort(new DiscItem.SortByNameAscending());

                    ascendingSortName = true;
                }                
            }

            if (sortBy == "Date")
            {
                if (ascendingSortDate)
                {
                    directories.Sort(new DiscItem.SortByDateDescending());
                    files.Sort(new DiscItem.SortByDateDescending());

                    ascendingSortDate = false;
                }
                else
                {
                    directories.Sort(new DiscItem.SortByDateAscending());
                    files.Sort(new DiscItem.SortByDateAscending());

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

            canRefreshBoth = false;
            RefreshFilesView();
            canRefreshBoth = true;
        }

        #endregion

        #region Events

        #region FilesView

        /// <summary>
        /// Change directory or open a file in default app
        /// </summary>        
        private void FilesView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            currFilesView = sender as ListView;
            DiscItem item = currFilesView.SelectedItem as DiscItem;
            if (item is FileItem)
            {
                System.Diagnostics.Process.Start(item.Path);
            }
            else if (item == null)
            {

            }
            else if (item is DirectoryItem)
            {
                System.Diagnostics.Debug.WriteLine("a");
                ChangeDirectory(item);
            }
        }

        /// <summary>
        /// Sort FilesView by clicked column
        /// </summary>        
        private void GridViewColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            isDragging = false;            
            var headerClicked = e.OriginalSource as GridViewColumnHeader;
            if (headerClicked.Tag.ToString() == "L")
            {
                currFilesView = FilesViewL;
            }
            else if(headerClicked.Tag.ToString() == "R")
            {
                currFilesView = FilesViewR;
            }
            string header = headerClicked.Content as string;            
            if (dirL != null && currFilesView == FilesViewL)                
            {
                //MessageBox.Show("L");
                Sort(header, directoriesL, filesL);
            }
            else if (dirR != null && currFilesView == FilesViewR)
            {
                //MessageBox.Show("R");
                Sort(header, directoriesR, filesR);
            }            
        }

        /// <summary>
        /// Set current FilesView as left
        /// </summary>        
        private void FilesViewL_GotFocus(object sender, RoutedEventArgs e)
        {
            currFilesView = FilesViewL;
        }

        /// <summary>
        /// Set current FilesView as right
        /// </summary> 
        private void FilesViewR_GotFocus(object sender, RoutedEventArgs e)
        {
            currFilesView = FilesViewR;
        }

        /// <summary>
        /// Copy files and directories on drag
        /// </summary>        
        private void FilesView_MouseMove(object sender, MouseEventArgs e)
        {
            
            if (isDragging)
            {
                if (Mouse.LeftButton == MouseButtonState.Pressed)
                {                    
                    CopyDiscItem();
                    DragDrop.DoDragDrop(currFilesView,
                                 currFilesView.SelectedItems,
                                 DragDropEffects.Copy);
                }
            }
        }

        /// <summary>
        /// Paste files and directories on drop
        /// </summary>        
        private void FilesView_Drop(object sender, DragEventArgs e)
        {
            isDragging = false;
            if (currFilesView != sender as ListView)
            {
                currFilesView = sender as ListView;
                PasteDiscItem();
                GetItems();
                RefreshFilesView();
            }
        }

        /// <summary>
        /// Blocks dragging and gets cursor position
        /// </summary>        
        private void FilesView_MouseDown(object sender, MouseButtonEventArgs e)
        {
                       
            isDragging = false;            
            clickPoint = e.GetPosition(this);
        }

        /// <summary>
        /// Enables dragging after 10 pixels of mouse movement distance
        /// </summary>
        private void FilesView_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            DependencyObject src = VisualTreeHelper.GetParent((DependencyObject)e.OriginalSource);
            if (src is System.Windows.Controls.Primitives.Track)
            {
                isTrack = true;
            }
            else
            {
                isTrack = false;
            }
            if (e.LeftButton == MouseButtonState.Pressed && !isTrack)
            {
                //MessageBox.Show(sender.GetType().ToString());
                Point currentPosition = e.GetPosition(this);
                double distanceX = Math.Abs(clickPoint.X - currentPosition.X);
                double distanceY = Math.Abs(clickPoint.Y - currentPosition.Y);
                if (distanceX > 10 || distanceY > 10)
                {
                    isDragging = true;
                    //System.Diagnostics.Debug.WriteLine("MOVE");
                }
            }
        }
        
        #endregion

        #region Context Menu

        private void ContextMenuDelete_Click(object sender, RoutedEventArgs e)
        {
            DeleteDiscItem();
        }

        private void ContextMenuNewDir_Click(object sender, RoutedEventArgs e)
        {
            CreateDirectory();
        }

        private void ContextMenuCopy_Click(object sender, RoutedEventArgs e)
        {
            CopyDiscItem();
        }

        private void ContextMenuPaste_Click(object sender, RoutedEventArgs e)
        {
            PasteDiscItem();

            GetItems();
            RefreshFilesView();
        }
        #endregion

        #region ComboBox

        /// <summary>
        /// Add available drives to ComboBox
        /// </summary>        
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

        /// <summary>
        /// Change drive in adequate FilesView
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DrivesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //change current files view
            if ((sender as ComboBox).Name == "drivesComboBoxL")
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
        #endregion

        #region Buttons

        private void CreateDirectoryButton_Click(object sender, RoutedEventArgs e)
        {
            CreateDirectory();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            DeleteDiscItem();
        }

        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            CopyDiscItem();
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



        #endregion

        #region KeyEvents
        /// <summary>
        /// Keyboard shortcuts
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        }
        #endregion

        #endregion

        
    }
}
