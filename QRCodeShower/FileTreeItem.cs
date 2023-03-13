using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows.Forms.Design;

namespace QRCodeShower
{
    class FileTreeItem
    {
        public FileTreeItem()
        {
            this.Items = new ObservableCollection<FileTreeItem>();
        }

        public string FileName { get; set; }

        public ObservableCollection<FileTreeItem> Items { get; set;}
    }
}
