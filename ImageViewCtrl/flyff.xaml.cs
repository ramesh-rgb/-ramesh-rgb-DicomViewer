using Dicom;
using Dicom.Imaging;
using Dicom.Imaging.Render;
using FellowOakDicom.Imaging;
using ImageViewCtrl.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
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
using Point = System.Drawing.Point;

namespace ImageViewCtrl
{
    public enum ImageBitsPerPixel { Eight, Twelve,Sixteen, TwentyFour };
    public enum ViewSettings { Zoom1_1, ZoomToFit };

    public partial class flyff : UserControl
    {
        private const float MIN_ZOOMRATIO = 0.5f;
        private const float MAX_ZOOMRATIO = 2.0f;
        private const float ZOOM_STEP = 0.1f;
        private byte[] raw8BitBuffer;
        private byte[] raw16BitBuffer;
        private int width;
        private int height;
        private int bits;
        private double ww;
        private double wl;
        double rescaleSlope;
        double rescaleIntercept;
        Dicom.Imaging.PhotometricInterpretation photometricinterpretation;
        int pixelRepresentation;
        private float currentRatio = 1.0f;
        private int samplesPerPixel;
        private object file;

        public bool HasImage { get; set; } = false;
        private System.Drawing.Point LastZoomPoint { get; set; }
        List<byte> pix8;
        List<ushort> pix12;
        List<ushort> pix16;
        List<byte> pix24;
        Bitmap bmp;
        int hOffset;
        int vOffset;
        int hMax;
        int vMax;
        int imgWidth;
        int imgHeight;
        int panWidth;
        int panHeight;
        bool newImage;

        // For Window Level
        int winMin;
        int winMax;
        int winCentre;
        int winWidth;
        int winWidthBy2;
        int deltaX;
        int deltaY;

        Point ptWLDown;
        double changeValWidth;
        double changeValCentre;
        bool rightMouseDown;
        bool imageAvailable;
        bool signed16Image;

        byte[] lut8;
        byte[] lut12;
        byte[] lut16;
        byte[] imagePixels8;
        byte[] imagePixels12;
        byte[] imagePixels16;
        byte[] imagePixels24;
        int sizeImg;
        int sizeImg3;
      //Mainwindow mf;

        public ViewSettings viewSettings;
        public bool viewSettingsChanged;
        int maxPixelValue;    // Updated July 2012
        int minPixelValue;
        ImageBitsPerPixel bpp;
        DicomImage DicomImageSOURCE;
        DicomDetailsnew DicomDetails;
        public flyff()
        {
            InitializeComponent();
      
            pix8 = new List<byte>();
            pix16 = new List<ushort>();
            pix24 = new List<byte>();
            pix12 = new List<ushort>();       
            winMin = 0;
            winMax = 4096;

            ptWLDown = new Point();
            changeValWidth = 0.5;
            changeValCentre = 20.0;
            rightMouseDown = false;
            imageAvailable = false;
            signed16Image = false;

            lut8 = new byte[256];
            lut16 = new byte[65536];
            lut12 = new byte[4096];
            viewSettings = ViewSettings.ZoomToFit;
            viewSettingsChanged = false;

            PerformResize();
        }

        /// <summary>
        /// open and show dicom file
        /// </summary>
        /// <param name="dicmFile"></param>
        /// <returns></returns>
        /// 
        private void PerformResize()
        {
            //panel.Location = new Point(3, 3);
            //panel.Width =
            //ClientRectangle.Width - 24;
            //panel.Height = ClientRectangle.Height - 24;

            //vScrollBar.Location = new Point(ClientRectangle.Width - 19, 3);
            //vScrollBar.Height = panel.Height;

            //hScrollBar.Location = new Point(3, ClientRectangle.Height - 19);
            //hScrollBar.Width = panel.Width;

            //hMax = hScrollBar.Maximum - hScrollBar.LargeChange + hScrollBar.SmallChange;
            //vMax = vScrollBar.Maximum - vScrollBar.LargeChange + vScrollBar.SmallChange;
        }
        public bool Signed16Image
        {
            set { signed16Image = value; }
        }
        public void SetParameters(ref List<byte> arr, int wid, int hei, double windowWidth,
                     double windowCentre, int samplesPerPixel, bool resetScroll,Bitmap bitmap,DicomImage dicomImage)
        {
            SetWindowInfo(windowWidth, windowCentre);

            //if (samplesPerPixel == 1)
            //{
            //    bpp = ImageBitsPerPixel.Eight;
            //    imgWidth = wid;
            //    imgHeight = hei;
            //    winWidth = Convert.ToInt32(windowWidth);
            //    winCentre = Convert.ToInt32(windowCentre);
            //    changeValWidth = 0.1;
            //    changeValCentre = 20.0;
            //    sizeImg = imgWidth * imgHeight;
            //    sizeImg3 = sizeImg * 3;

            //    pix8 = arr;
            //    imagePixels8 = new byte[sizeImg3];

            //   // mf = mainFrm;
            //    imageAvailable = true;
            //    if (bmp != null)
            //        bmp.Dispose();
            //    ResetValues();
            //    ComputeLookUpTable8();
            //    bmp = new Bitmap(imgWidth, imgHeight, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            //    CreateImage8();
            //    var imageSource = ConvertUtil.BitmapToImageSource(bmp);
            //    this.image.Source = imageSource;
            //}
            if (samplesPerPixel == 1)
            {
                DicomImageSOURCE = dicomImage;
                //DicomDetails = new DicomDetailsnew();
                //DicomDetails.patientname = dicomDetailsnew.patientname;
                //DicomDetails.patientid = dicomDetailsnew.patientid;
                //DicomDetails.instance = dicomDetailsnew.instance;
                //DicomDetails.gender = dicomDetailsnew.gender;
                //DicomDetails.age = dicomDetailsnew.age;
                //DicomDetails.studydescription = dicomDetailsnew.studydescription;

                bpp = ImageBitsPerPixel.Eight;
                imgWidth = wid;
                imgHeight = hei;
                winWidth = Convert.ToInt32(windowWidth);
                winCentre = Convert.ToInt32(windowCentre);
                changeValWidth = 0.1;
                changeValCentre = 20.0;
                sizeImg = imgWidth * imgHeight;
                sizeImg3 = sizeImg * 3;
                pix8 = arr;
                imagePixels8 = new byte[sizeImg3];

                // mf = mainFrm;
                imageAvailable = true;
                if (bmp != null)
                    bmp.Dispose();
                ResetValues();
                //  ComputeLookUpTable8();
                // bmp = new Bitmap(imgWidth, imgHeight, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                //  CreateImage8();
                bmp = bitmap;
                var imageSource = ConvertUtil.BitmapToImageSource(bitmap);
                this.image.Source = imageSource;
            }
            if (samplesPerPixel == 3)
            {
                bpp = ImageBitsPerPixel.TwentyFour;
                imgWidth = wid;
                imgHeight = hei;
                winWidth = Convert.ToInt32(windowWidth);
                winCentre = Convert.ToInt32(windowCentre);
                changeValWidth = 0.1;
                changeValCentre = 0.1;
                sizeImg = imgWidth * imgHeight;
                sizeImg3 = sizeImg * 3;
                pix24 = arr;
                imagePixels24 = new byte[sizeImg3];
               // mf = mainFrm;
                imageAvailable = true;
                if (bmp != null)
                    bmp.Dispose();
                ResetValues();
                ComputeLookUpTable8();
                bmp = new Bitmap(imgWidth, imgHeight, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
              //  CreateImage24();
            }
            if (resetScroll == true) ComputeScrollBarParameters();
           // Invalidate();
        }
        public void SetParameters12(ref List<ushort> arr, int wid, int hei, double windowWidth,
        double windowCentre, bool resetScroll)
        {
            SetWindowInfo(windowWidth, windowCentre);
            bpp = ImageBitsPerPixel.Twelve;
            imgWidth = wid;
            imgHeight = hei;
            winWidth = Convert.ToInt32(windowWidth);
            winCentre = Convert.ToInt32(windowCentre);
            Console.WriteLine(windowWidth + "--" + windowCentre + "--");
            sizeImg = imgWidth * imgHeight;
            sizeImg3 = sizeImg * 3;
            double sizeImg3By4 = sizeImg3 / 4.0;
            DetermineMouseSensitivity();
            pix12 = arr;
            Console.WriteLine("pixel16 is " + pix12.Count);
            // imagePixels16 = new byte[sizeImg3];
            // mf = mainFrm;
            imageAvailable = true;
            if (bmp != null)
                bmp.Dispose();
            ResetValues();
            ComputeLookUpTable12();
            bmp = new Bitmap(imgWidth, imgHeight, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            CreateImage12();
            if (resetScroll == true) ComputeScrollBarParameters();
            var imageSource = ConvertUtil.BitmapToImageSource(bmp);
            this.image.Source = imageSource;
            //   Invalidate();
        }
        public void SetParameters(ref List<ushort> arr, int wid, int hei, double windowWidth,double windowCentre, bool resetScroll,Bitmap bitmap,DicomImage dicomImage)
        {
            DicomImageSOURCE = null;
            DicomImageSOURCE = dicomImage;
            SetWindowInfo(windowWidth, windowCentre);
            bpp = ImageBitsPerPixel.Sixteen;
            imgWidth = wid;
            imgHeight = hei;
            winWidth = Convert.ToInt32(windowWidth);
            winCentre = Convert.ToInt32(windowCentre);
            Console.WriteLine(windowWidth + "--" + windowCentre + "--");
            sizeImg = imgWidth * imgHeight;
            sizeImg3 = sizeImg * 3;
            double sizeImg3By4 = sizeImg3 / 4.0;
            DetermineMouseSensitivity();
            pix16 = arr;
            Console.WriteLine("pixel16 is " + pix16.Count);
            // imagePixels16 = new byte[sizeImg3];
            // mf = mainFrm;
            imageAvailable = true;
            if (bmp != null)
                bmp.Dispose();
            ResetValues();
            //  ComputeLookUpTable16();
            // bmp = new Bitmap(imgWidth, imgHeight, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            // CreateImage16();
            // if (resetScroll == true) ComputeScrollBarParameters();
            bmp = bitmap;
            var imageSource = ConvertUtil.BitmapToImageSource(bitmap);
            this.image.Source = imageSource;
            //Invalidate();
        }

        // Create a bitmap on the fly, using 8-bit grayscale pixel data
        private void CreateImage8()
        {
            BitmapData bmd = bmp.LockBits(new System.Drawing.Rectangle(0, 0, imgWidth, imgHeight),
                System.Drawing.Imaging.ImageLockMode.ReadOnly, bmp.PixelFormat);

            unsafe
            {
                int pixelSize = 3;
                int i, j, j1, i1;
                byte b;

                for (i = 0; i < bmd.Height; ++i)
                {
                    byte* row = (byte*)bmd.Scan0 + (i * bmd.Stride);
                    i1 = i * bmd.Width;

                    for (j = 0; j < bmd.Width; ++j)
                    {
                        b = lut8[pix8[i * bmd.Width + j]];
                        j1 = j * pixelSize;
                        row[j1] = b;            // Red
                        row[j1 + 1] = b;        // Green
                        row[j1 + 2] = b;        // Blue
                    }
                }
            }
            bmp.UnlockBits(bmd);
        }

        // Create a bitmap on the fly, using 24-bit RGB pixel data
        private void CreateImage24()
        {
            {
                int numBytes = imgWidth * imgHeight * 3;
                int j;
                int i, i1;

                BitmapData bmd = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width,
                    bmp.Height), ImageLockMode.WriteOnly, bmp.PixelFormat);

                int width3 = bmd.Width * 3;

                unsafe
                {
                    for (i = 0; i < bmd.Height; ++i)
                    {
                        byte* row = (byte*)bmd.Scan0 + (i * bmd.Stride);
                        i1 = i * bmd.Width * 3;

                        for (j = 0; j < width3; j += 3)
                        {
                            // Windows uses little-endian, so the RGB data is 
                            //  actually stored as BGR
                            row[j + 2] = lut8[pix24[i1 + j]];     // Blue
                            row[j + 1] = lut8[pix24[i1 + j + 1]]; // Green
                            row[j] = lut8[pix24[i1 + j + 2]];     // Red
                        }
                    }
                }
                bmp.UnlockBits(bmd);
            }
        }

        // Create a bitmap on the fly, using 16-bit grayscale pixel data
        private void CreateImage16()
        {
            BitmapData bmd = bmp.LockBits(new System.Drawing.Rectangle(0, 0, imgWidth, imgHeight),
               System.Drawing.Imaging.ImageLockMode.ReadOnly, bmp.PixelFormat);
            Console.WriteLine("Width is:" + imgWidth + "Height is " + imgHeight);
            unsafe
            {
                int pixelSize = 3;
                int i, j, j1, i1;
                byte b;

                for (i = 0; i < bmd.Height; ++i)
                {
                    byte* row = (byte*)bmd.Scan0 + (i * bmd.Stride);
                    i1 = i * bmd.Width;

                    for (j = 0; j < bmd.Width; ++j)
                    {
                        b = lut16[pix16[i * bmd.Width + j]];
                        j1 = j * pixelSize;
                        row[j1] = b;            // Red
                        row[j1 + 1] = b;        // Green
                        row[j1 + 2] = b;        // Blue
                    }
                }
            }
            bmp.UnlockBits(bmd);
        }

        private void CreateImage12()
        {
            BitmapData bmd = bmp.LockBits(new System.Drawing.Rectangle(0, 0, imgWidth, imgHeight),
               System.Drawing.Imaging.ImageLockMode.ReadOnly, bmp.PixelFormat);
            Console.WriteLine("Width is:" + imgWidth + "Height is " + imgHeight);
            unsafe
            {
                int pixelSize = 3;
                int i, j, j1, i1;
                byte b;

                for (i = 0; i < bmd.Height; ++i)
                {
                    byte* row = (byte*)bmd.Scan0 + (i * bmd.Stride);
                    i1 = i * bmd.Width;

                    for (j = 0; j < bmd.Width; ++j)
                    {
                        b = lut12[pix12[i * bmd.Width + j]];
                        j1 = j * pixelSize;
                        row[j1] = b;            // Red
                        row[j1 + 1] = b;        // Green
                        row[j1 + 2] = b;        // Blue
                    }
                }
            }
            bmp.UnlockBits(bmd);
        }

        public void ResetValues()
        {
            winMax = Convert.ToInt32(winCentre + 0.5 * winWidth);
            winMin = winMax - winWidth;
            //UpdateMainForm();
        }
        //ramesh
       
        private void ComputeScrollBarParameters()
        {
            //panWidth = panel.Width;
            //panHeight = panel.Height;

            //hOffset = (panWidth - imgWidth) / 2;
            //vOffset = (panHeight - imgHeight) / 2;

            //if (imgWidth < panWidth)
            //{
            //    hScrollBar.Visible = false;
            //}
            //else
            //{
            //    hScrollBar.Visible = true;
            //    hScrollBar.Value = (hScrollBar.Maximum + 1 -
            //        hScrollBar.LargeChange - hScrollBar.Minimum) / 2;
            //}

            //if (imgHeight < panHeight)
            //{
            //    vScrollBar.Visible = false;
            //}
            //else
            //{
            //    vScrollBar.Visible = true;
            //    vScrollBar.Value = (vScrollBar.Maximum + 1 -
            //        vScrollBar.LargeChange - vScrollBar.Minimum) / 2;
            //}
        }
        private void ComputeLookUpTable16()
        {
            int range = winMax - winMin;
            Console.WriteLine("winMaxxxxxxxxxx" + winMax);
            Console.WriteLine("winMaxxxxxxxxxx" + winMin + "range" + range);
            if (range < 1) range = 1;
            double factor = 255.0 / range;
            int i;

            for (i = 0; i <65536; ++i)
            {
                if (i <= winMin)
                    lut16[i] = 0;
                else if (i >= winMax)
                    lut16[i] = 255;
                else
                {
                    lut16[i] = (byte)((i - winMin) * factor);
                }
            }
        }

        private void ComputeLookUpTable12()
        {
                      
            // ushort min = level - window / 2;
            //  ushort max = level + window / 2;
            //int i;
            //for (i = 0; i < 4096; i++)
            //{
            //    float theVal = (i - winMin) / (winMax - winMin) * 255;//can go negative, so use floats
            //                                                 //  to make sure we avoid wraparounds
            //    if (theVal < 0) theVal = 0;
            //    if (theVal > 255) theVal = 255;
            //    lut12[i] = (byte)(theVal + 0.5f); //add half to round during casting
            //}

            int range = winMax - winMin;
            Console.WriteLine("winMaxxxxxxxxxx" + winMax);
            Console.WriteLine("winMaxxxxxxxxxx" + winMin + "range" + range);
            if (range < 1) range = 1;
            double factor = 255.0 / range;
            int i;

            for (i = 0; i < 4096; ++i)
            {
                if (i <= winMin)
                    lut12[i] = 0;
                else if (i >= winMax)
                    lut12[i] = 255;
                else
                {
                    lut12[i] = (byte)((i - winMin) * factor);
                }
            }
        }
        private void ComputeLookUpTable8()
        {
            if (winMax == 0)
                winMax = 255;

            int range = winMax - winMin;
            if (range < 1) range = 1;
            double factor = 255.0 / range;

            for (int i = 0; i < 256; ++i)
            {
                if (i <= winMin)
                    lut8[i] = 0;
                else if (i >= winMax)
                    lut8[i] = 255;
                else
                {
                    lut8[i] = (byte)((i - winMin) * factor);
                }
            }
        }



        //rameshend

        


        public bool CloseImage()
        {
            try
            {
                this.image.Source = null;
                HasImage = false;
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void SetWindowInfo(double ww,double wl)
        {
            this.lbl_WL.Content = $"WL:{wl}";
            this.lbl_WW.Content = $"WW:{ww}";
        }
        
        public void ShowWindowInfo()
        {
            stackpanel_Window.Visibility = Visibility.Visible;
        }

        public void HideWindowInfo()
        {
            stackpanel_Window.Visibility = Visibility.Hidden;
        }

        private void ZoomImage(Point point, int delta)
        {
            if (delta == 0)
                return;

            if (delta < 0 && scaleTransform.ScaleX < MIN_ZOOMRATIO)
                return;

            if (delta > 0 && scaleTransform.ScaleX > MAX_ZOOMRATIO)
                return;
                   
            var ratio = 0.0;
            if (delta > 0)
            {
                ratio = scaleTransform.ScaleX * ZOOM_STEP;
            }
            else
            {
                ratio = scaleTransform.ScaleX * -ZOOM_STEP;
   
            }
            scaleTransform.CenterX = this.image.ActualWidth / 2.0;
            scaleTransform.CenterY = this.image.ActualHeight / 2.0;

            //TODO use animation
            scaleTransform.ScaleX += ratio;
            scaleTransform.ScaleY = Math.Abs(scaleTransform.ScaleX);                
        }

        private void Border_MouseWheel(object sender, MouseWheelEventArgs e)
        {   var element = sender as UIElement;
                var positionmove = e.GetPosition(element);
            Point p = new Point();
            p.X = (int)positionmove.X;
            p.Y = (int)positionmove.Y;

            ZoomImage(p, e.Delta);
        }
        void DetermineMouseSensitivity()
        {
            // Modify the 'sensitivity' of the mouse based on the current window width
            if (winWidth < 10)
            {
                changeValWidth = 0.1;
            }
            else if (winWidth >= 20000)
            {
                changeValWidth = 40;
            }
            else
            {
                changeValWidth = 0.1 + (winWidth - 10) / 500.0;
            }

            changeValCentre = changeValWidth;
        }
        private void Border_MouseMove(object sender, MouseEventArgs e)
        {
            DetermineMouseSensitivity();
            if (rightMouseDown == true)
            {
                var element = sender as UIElement;
                var positionmove = e.GetPosition(element);
                
                winWidthBy2 = winWidth / 2;
                winWidth = winMax - winMin;
                winCentre = winMin + winWidthBy2;

                deltaX = (int)((ptWLDown.X - (int)positionmove.X) * changeValWidth);
                deltaY = (int)((ptWLDown.Y - (int)positionmove.Y) * changeValCentre);

                winCentre -= deltaY;
                winWidth -= deltaX;

                if (winWidth < 2) winWidth = 2;
                winWidthBy2 = winWidth / 2;

                winMax = winCentre + winWidthBy2;
                winMin = winCentre - winWidthBy2;

                if (winMin >= winMax) winMin = winMax - 1;
                if (winMax <= winMin) winMax = winMin + 1;


                ptWLDown.X = (int)positionmove.X;
                ptWLDown.Y = (int)positionmove.Y;

                SetWindowInfo(winWidth, winCentre);
                //if (bpp == ImageBitsPerPixel.Eight)
                //{

                //    ComputeLookUpTable8();
                //    CreateImage8();
                //}
                //else if (bpp == ImageBitsPerPixel.Sixteen)
                //{
                //    ComputeLookUpTable16();
                //    CreateImage16();
                //}
                //else if (bpp == ImageBitsPerPixel.Twelve)
                //{
                //    ComputeLookUpTable12();
                //    CreateImage12();
                //}
                //else // (bpp == ImageBitsPerPixel.TwentyFour)
                //{
                //    ComputeLookUpTable8();
                //    //CreateImage24();
                //}
                var imageSource = ConvertUtil.BitmapToImageSource(bmp);
                this.image.Source = imageSource;
            }
            // Invalidate();
        
        //old existing code

        //if (e.RightButton == MouseButtonState.Pressed)
        //{
        //    var point = e.GetPosition(this.image);

        //    if (LastZoomPoint.X == 0 && LastZoomPoint.Y == 0)
        //    {
        //        // r   LastZoomPoint = point;
        //        return;
        //    }

        //    var xPos = point.X - LastZoomPoint.X;
        //    var yPos = point.Y - LastZoomPoint.Y;

        //    if (Math.Abs(xPos) < 10 && Math.Abs(yPos) < 10)
        //        return;

        //    //Hit test

        //    var ratio = currentRatio;
        //    if (xPos < 0)
        //        ratio *= 1.1f;
        //    else
        //        ratio *= 0.9f;

        //    LimitRatio(ref ratio);

        //    scaleTransform.CenterX = this.image.ActualWidth / 2.0;
        //    scaleTransform.CenterY = this.image.ActualHeight / 2.0;

        //    scaleTransform.ScaleX *= ratio / currentRatio;
        //    scaleTransform.ScaleY *= ratio / currentRatio;

        //    currentRatio = ratio;
        //    //r  LastZoomPoint = point;
        //}
    }

        private void LimitRatio(ref float ratio)
        {
            if (ratio > MAX_ZOOMRATIO)
                ratio = MAX_ZOOMRATIO;
            else if(ratio < MIN_ZOOMRATIO)
                ratio = MIN_ZOOMRATIO;
        }

        private void Border_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ResetZoomPoint();
            if (rightMouseDown == true)
            {
                rightMouseDown = false;
              //  Cursor = Cursors.Default;
                ResetZoomPoint();
            }
        }

        private void ResetZoomPoint()
        {
            LastZoomPoint = new Point();
        }

        private void Border_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //if (imageAvailable == true)
            //{
            //    var element = sender as UIElement;
            //    var position = e.GetPosition(element);

            //    ptWLDown.X = (int)position.X;
            //    ptWLDown.Y = (int)position.Y;
            //    rightMouseDown = false;
            //    dicomimage.Dataset.Get<int>(DicomTag.SamplesPerPixel);
            //    var D = DicomImageSOURCE._pixels
            //     var pixeldatatest = DicomImageSOURCE.PixelData; // returns DicomPixelData type
            //    var pixelData = PixelDataFactory.Create(DicomImageSOURCE.PixelData, 0); // returns IPixelData type
            //    if (pixelData is Dicom.Imaging.Render.GrayscalePixelDataU8)
            //    {
            //        for (int i = 0; i < pixelData.Width; i++)
            //        {
            //            for (int j = 0; j < pixelData.Height; j++)
            //            {
            //                Console.WriteLine("{0}", Convert.ToSingle(pixelData.GetPixel(ptWLDown.X, ptWLDown.Y)));
            //                lbl_Hu.Content = "X value: " + ptWLDown.X + " Y value: " + ptWLDown.Y + "  " + Convert.ToSingle(pixelData.GetPixel(91, 40)).ToString();

            //            }
            //        }
            //    }
            //    Cursor = Cursors.Hand;

            //}
           
            if (imageAvailable == true)
            {
               var element = sender as UIElement;
                var position = e.GetPosition(element);

                ptWLDown.X = (int)position.X;
                ptWLDown.Y = (int)position.Y;
                rightMouseDown = false;

                ImageSource imageSource = image.Source;
                BitmapSource bitmapImage = (BitmapSource)imageSource;
               // var d = bmp.GetPixel(ptWLDown.X, ptWLDown.Y);
              //  Console.WriteLine("Pixel value is : *****" + d);
               double x= (e.GetPosition(image).X * bitmapImage.PixelWidth / image.ActualWidth);
                double y = (e.GetPosition(image).Y * bitmapImage.PixelHeight / image.ActualHeight);
              
     
           // var pixeldatatest = DicomImageSOURCE.PixelData; // returns DicomPixelData type
                IPixelData pixelData = PixelDataFactory.Create(DicomImageSOURCE.PixelData, 0); // returns IPixelData type
               
                if (pixelData is Dicom.Imaging.Render.GrayscalePixelDataU16)
                {
                //    // for (int i = 0; i < pixelData.Width; i++)
                    // {
                    //  for (int j = 0; j < pixelData.Height; j++)
                    //  {
                    int rounded_2 = (int)Math.Round(x);
                    int rounded_2y = (int)Math.Round(y);
                    if (rounded_2 >= 0 && rounded_2y >= 0)
                    {
                        //Console.WriteLine("{0}", Convert.ToSingle(pixelData.GetPixel(ptWLDown.X, ptWLDown.Y)));
                        lbl_Hu.Content = "X value: " + rounded_2 + " Y value: " + rounded_2y + "  " + Convert.ToSingle(pixelData.GetPixel((int)rounded_2, (int)rounded_2y)).ToString();
                    }
                    else
                    {
                        lbl_Hu.Content = "X value: " + rounded_2 + " Y value: " + rounded_2y + "  " + "Out of boundary";
                    }
                       // }
                 //   }
              }
                Cursor = Cursors.Hand;

            }


           
        }
    }
}
