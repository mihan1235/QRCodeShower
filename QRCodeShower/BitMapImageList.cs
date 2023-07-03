using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using QRCoder;

namespace QRCodeShower
{
    class BitMapImageList
    {
        private List<byte[]> chanksRaw = new List<byte[]>();
        private int position = 0;

        public int Position
        {
            get { return position; }
        }

        public bool IsLast
        {
            get
            {
                if (chanksRaw.Count == 0) return true;

                if (position == chanksRaw.Count - 1) return true;

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

        public static string Base64Encode(byte[] plainText)
        {
            //var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainText);
        }

        public BitmapImage? GetNext()
        {
            if (chanksRaw.Count != 0)
            {
                if (!IsLast) position++;

                return toQrQodeBitmap(chanksRaw[position]);
            }
            else
            {
                return null;
            }
        }

        public BitmapImage? GetPrev()
        {
            if (chanksRaw.Count != 0)
            {
                if (!IsFirst) position--;

                return toQrQodeBitmap(chanksRaw[position]);
            }
            else
            {
                return null;
            }
        }

        private BitmapImage? toQrQodeBitmap(byte[] chunck)
        {
            using var qrGenerator = new QRCodeGenerator();

            using var qrCodeData = qrGenerator.CreateQrCode(Base64Encode(chunck), QRCodeGenerator.ECCLevel.M);
            //using var qrCodeData = qrGenerator.CreateQrCode(Base64Encode(fileText), QRCodeGenerator.ECCLevel.M);
            using QRCode qrCode = new QRCode(qrCodeData);
            //Bitmap qrCodeImage = qrCode.GetGraphic(120);
            Bitmap qrCodeImage = qrCode.GetGraphic(20);
            return BitmapToImageSource(qrCodeImage);

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

        public void Clear()
        {
            chanksRaw.Clear();
            position = 0;
        }

        internal void Add(IEnumerable<byte[]> fileTextBytes)
        {
            foreach (var chunck in fileTextBytes)
            {
                chanksRaw.Add(chunck);
            }
        }

        public int Count
        {
            get { return chanksRaw.Count; }
        }
    }
}
