using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace QRCodeShower
{
    class BitMapImageList
    {
        private List<BitmapImage> images = new List<BitmapImage>();
        private int position = 0;

        public bool IsLast
        {
            get
            {
                if (images.Count == 0) return true;

                if (position == images.Count - 1) return true;

                return false;
            }
        }

        public bool IsFirst
        {
            get
            {
                if (position == 0) return true;
                else
                    return false;
            }
        }

        public BitmapImage? GetNext()
        {
            if (images.Count != 0)
            {
                if (!IsLast) position++;

                return images[position];
            }
            else
            {
                return null;
            }
        }

        public BitmapImage? GetPrev()
        {
            if (images.Count != 0)
            {
                if (!IsFirst) position--;

                return images[position];
            }
            else
            {
                return null;
            }
        }

        public void Add(BitmapImage image)
        {
            images.Add(image);
        }

        public void Clear()
        {
            images.Clear();
            position = 0;
        }

        public int Count
        {
            get { return images.Count; }
        }
    }
}
