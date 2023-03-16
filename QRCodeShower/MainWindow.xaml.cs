using Microsoft.Win32;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
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
        BitMapImageList images = new BitMapImageList();

        public MainWindow()
        {
            InitializeComponent();
            setDefaultImageSource();

            Uri iconUri = new Uri("pack://application:,,,/resources/qr-scanner_icon-icons.com_50056.ico",
                UriKind.RelativeOrAbsolute);
            this.Icon = BitmapFrame.Create(iconUri);

        }

        private void setDefaultImageSource()
        {
            ImageObject.Source = new BitmapImage(new Uri("pack://application:,,,/resources/EmptyImg.jpg"));
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
                item.IsFile = true;
                item.File = file;
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

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        private async void FilesTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                var obj = FilesTree.SelectedItem as FileTreeItem;
                if (obj != null && obj.IsFile && obj.File != null && obj.File.Exists)
                {
                    string fileText = await File.ReadAllTextAsync(obj.File.FullName);
                    using var qrGenerator = new QRCodeGenerator();

                    var fileTextBytes = System.Text.Encoding.UTF8.GetBytes(fileText).Chunk(2000);
                    images.Clear();
                    foreach (var chunck in fileTextBytes)
                    {
                        using var qrCodeData = qrGenerator.CreateQrCode(chunck, QRCodeGenerator.ECCLevel.M);
                        //using var qrCodeData = qrGenerator.CreateQrCode(Base64Encode(fileText), QRCodeGenerator.ECCLevel.M);
                        using QRCode qrCode = new QRCode(qrCodeData);
                        //Bitmap qrCodeImage = qrCode.GetGraphic(120);
                        Bitmap qrCodeImage = qrCode.GetGraphic(20);
                        images.Add(BitmapToImageSource(qrCodeImage));
                    }

                    ImageObject.Source = images.GetPrev(); //Get first element
                    if (images.Count > 1)
                    {
                        ButtonsWrap.Visibility = Visibility.Visible;
                        PrevButton.IsEnabled = false;
                        NextButton.IsEnabled = true;
                        ImagesPosition.Visibility = Visibility.Visible;
                        ImagesPosition.Text = $"{images.Position + 1}/{images.Count}";
                    }
                    else
                    {
                        ButtonsWrap.Visibility = Visibility.Collapsed;
                        PrevButton.IsEnabled = false;
                        NextButton.IsEnabled = false;
                        ImagesPosition.Visibility = Visibility.Collapsed;
                    }
                }
                else setDefaultImageSource();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"FilesTree_SelectedItemChanged: {ex.Message}");
                setDefaultImageSource();
            }
        }

        BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }

        private void PrevButton_Click(object sender, RoutedEventArgs e)
        {
            if (!images.IsFirst)
            {
                ImageObject.Source = images.GetPrev();
                ImagesPosition.Text = $"{images.Position + 1}/{images.Count}";
            }
            else
                PrevButton.IsEnabled = false;

            if (!images.IsLast)
                NextButton.IsEnabled = true;

            if (images.IsFirst)
                PrevButton.IsEnabled = false;
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (!images.IsLast)
            {
                ImageObject.Source = images.GetNext();
                ImagesPosition.Text = $"{images.Position + 1}/{images.Count}";
            }
            else
                NextButton.IsEnabled = false;

            if (!images.IsFirst)
                PrevButton.IsEnabled = true;

            if (images.IsLast)
                NextButton.IsEnabled = false;
        }
    }
}
