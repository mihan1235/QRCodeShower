using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using forms = System.Windows.Forms;

namespace QRCodeShower
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ObservableCollection<FileTreeItem> files = new ObservableCollection<FileTreeItem>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void OpenFolder(object sender, ExecutedRoutedEventArgs e)
        {
            var myDialog = new forms.FolderBrowserDialog();
            forms.DialogResult result = myDialog.ShowDialog();

            if (result == forms.DialogResult.OK && !string.IsNullOrWhiteSpace(myDialog.SelectedPath))
            {
                DirectoryInfo dirInfo = new DirectoryInfo(myDialog.SelectedPath);
                files = createFileTreeItem(dirInfo);
                FilesTree.ItemsSource = files;

                //MessageBox.Show($"Got files: {myDialog.SelectedPath}");
            }
        }

        ObservableCollection<FileTreeItem> createFileTreeItem(DirectoryInfo dirInfo)
        {
            var items = new ObservableCollection<FileTreeItem>();
            FileTreeItem item = new FileTreeItem();
            item.FileName = dirInfo.Name;
            foreach (var it in _addDirsItems(dirInfo).Concat(_addFilesItems(dirInfo)))
            {
                item.Items.Add(it);
            }

            items.Add(item);
            //item.Items = 
            return items;
        }

        private List<FileTreeItem> _addDirsItems(DirectoryInfo dirInfo)
        {
            var items = new List<FileTreeItem>();
            foreach (var dir in dirInfo.GetDirectories())
            {
                var item = new FileTreeItem();
                item.FileName = dir.Name;
                foreach (var it in _addDirsItems(dir).Concat(_addFilesItems(dir)))
                {
                    item.Items.Add(it);
                }
                items.Add(item);
            }

            //item.Items = 
            return items;
        }
        List<FileTreeItem> _addFilesItems(DirectoryInfo dirInfo)
        {
            var items = new List<FileTreeItem>();
            foreach (var file in dirInfo.GetFiles())
            {
                var item = new FileTreeItem();
                item.FileName = file.Name;
                items.Add(item);
            }

            //item.Items = 
            return items;
        }

        private void CanOpenFolder(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CanPauseSlideShow(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
        }

        private void PlaySlideShow(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void CanPlaySlideShow(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
        }

        private void PauseSlideShow(object sender, ExecutedRoutedEventArgs e)
        {
        }

        private void CanStopSlideShow(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
        }

        private void StopSlideShow(object sender, ExecutedRoutedEventArgs e)
        {
        }
    }
}
