using Dicom.Imaging;
using System;
using Dicom;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;

namespace ImageViewCtrl.Util
{
    public class ConvertUtil
    {
        int frame = 0;
         
        private static byte[] raw8bitbuffer = null;
        public static WriteableBitmap GetWriteableBitmap(byte[] buffer, int width, int height, int bit)
        {
            var pixelFormat = bit > 8 ? PixelFormats.Gray16 : PixelFormats.Gray8;
            var size = bit > 8 ? width * height * 2 : width * height;
            WriteableBitmap writeableBitmap = new WriteableBitmap(width, height, 96.0, 96.0, pixelFormat, null); ;
            writeableBitmap.Lock();
            Marshal.Copy(buffer, 0, writeableBitmap.BackBuffer, size);
            Int32Rect int32Rect = new Int32Rect(0, 0, width, height);
            writeableBitmap.AddDirtyRect(int32Rect);
            writeableBitmap.Unlock();
            return writeableBitmap;
        }

        public static ImageSource GetImageSource(WriteableBitmap writeableBitmapss)
        {
            TransformedBitmap bitmap = new TransformedBitmap();
            bitmap.BeginInit();
            bitmap.Source = writeableBitmapss;
            bitmap.EndInit();
            return bitmap;
        }
        public static ImageSource imageSourceForImageControl(Bitmap yourBitmap)
        {
            ImageSourceConverter c = new ImageSourceConverter();
            return (ImageSource)c.ConvertFrom(yourBitmap);
        }

        public static ImageSource BitmapToImageSource(Bitmap bitmap)
        {
            using (var gfx = Graphics.FromImage(bitmap))
                gfx.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear;
             //ScaleImageKeepingAspectRatio(ref gfx, bmp, panWidth, panHeight);
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

        void ScaleImageKeepingAspectRatio(ref Graphics grfx, Image image, int panelWidth, int panelHeight)
        {
         //   hScrollBar.Visible = false;
          //  vScrollBar.Visible = false;
            SizeF sizef = new SizeF(image.Width / image.HorizontalResolution,
                                    image.Height / image.VerticalResolution);

            float fScale = Math.Min(panelWidth / sizef.Width,
                                    panelHeight / sizef.Height);

            sizef.Width *= fScale;
            sizef.Height *= fScale;

            grfx.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            grfx.DrawImage(image, (panelWidth - sizef.Width) / 2,
                                  (panelHeight - sizef.Height) / 2,
                                  sizef.Width, sizef.Height);
        }


        //private ImageSource GetImageSource(string fileName)
        //{
        //    var writeableBitmap = GetWriteableBitmap(fileName);
        //    TransformedBitmap bitmap = new TransformedBitmap();
        //    bitmap.BeginInit();
        //    bitmap.Source = writeableBitmap;
        //    bitmap.EndInit();
        //    return bitmap;
        //}
    }
}
