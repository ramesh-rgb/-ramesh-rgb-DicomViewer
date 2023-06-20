using Dicom;
using Dicom.Imaging;
using Dicom.Imaging.LUT;
using Dicom.Imaging.Mathematics;
using Dicom.Imaging.Render;
using Dicom.Media;
using FellowOakDicom;
using ImageViewCtrl.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using Brushes = System.Windows.Media.Brushes;
using Matrix = System.Windows.Media.Matrix;
using Point = System.Drawing.Point;
using Rectangle = System.Windows.Shapes.Rectangle;

//18.08.2022

namespace ImageViewer.Screens
{
    /// <summary>
    /// Interaction logic for SeriesImagePanel.xaml
    /// </summary>
    public partial class SeriesImagePanel: UserControl, IPersonManage
    {
        private int pixelRepresentation;
        public bool signedImage;
        private PhotometricInterpretation photometricinterpretation;
        private List<byte> pix8;
        private List<ushort> pix16;
        private List<byte> pix24;
        private BinaryReader file;
        private Bitmap bmp1;
        private int offset;
        private int min8 = Byte.MinValue;
        private int max8 = Byte.MaxValue;
        private Person _person;
        private List<string> dicomlist = new List<string>();
        private List<DicomImage> dicomImageList = new List<DicomImage>();
        private string name;
        private double zoomin = 1;
        private double rotateangle;
        private double Swindowwidth = 0.0;
        private double SwindowCenter = 0.0;
        private double currentScaleFactor = 1;
        public int indexr;
        public int j;
        public int Lastr;
        private const float MIN_ZOOMRATIO = 0.5f;
        private const float MAX_ZOOMRATIO = 2.0f;
        private const float ZOOM_STEP = 0.1f;
        //System.Drawing.Point ptWLDown;
        private IPixelData pixel;
        //public int width;
        //public int height;  
        double rescaleSlope;
        double rescaleIntercept;
        int winMin;
        int winMax;
        byte[] lut8;
        byte[] lut16;
        int min16 = short.MinValue;
        int max16 = ushort.MaxValue;
        bool rightMouseDown;
        bool imageAvailable;
        private byte[] pixeldata;
        //bool signed16Image;
        private DispatcherTimer dispatcherTimer = new DispatcherTimer();
        public int frameTime;
        public bool threemanipulate = false;
        public bool twomanipulate = false;
        public bool fivemanipulate = false;
        private System.Windows.Point startpoint;
        private System.Windows.Point dicomSelectionPoint;
        private System.Windows.Point dicomEndePoint;
        private Rectangle rectSelectArea;
        private Line linetool;
        private TextBlock textBlock1;
        private bool mousedowncheck;
        private bool scoutlineToolEnabled = false;
        private bool lineToolEnabled = false;
        private bool RectToolEnabled = false;
        private bool circleToolEnabled = false;
        private bool selectToolEnabled = false;
        private bool WindowingToolEnabled = false;
        private Ellipse ellipseArea;
        private int pixeldatWidth;
        private int pixeldatHeight;
        private double pixelSpacingValue;
        private string seriesRecordPathFileName;
        private DicomDirectoryRecord dicomDirectoryRecordobj;
        DicomDec dicomDecoder;
        DicomDecoder dicomDecoderPixel;
        public bool isDicomDirFolderPath;
        private bool looping = true;
        //18.08.2022
        private string frameOfReferenceUID;
        private string imagePosition;
        private string imageOrientation;
        private string[] imageType;
        private string referencedSOPInstanceUID = "";
        private string pixelSpacing;
        private int row;
        private int column;
        private bool isLocalizer = false;
        private static bool displayScout = false;
        private Line ScoutLine1;
        //private int scoutLine1X1;
        //private int scoutLine1Y1;
        //private int scoutLine1X2;
        //private int scoutLine1Y2;
        //private int scoutLine2X1;
        //private int scoutLine2Y1;
        //private int scoutLine2X2;
        //private int scoutLine2Y2;
        private int axisLeftX;
        private int axisLeftY;
        private int axisRightX;
        private int axisRightY;
        private int axisBottomX;
        private int axisBottomY;
        private int axisTopX;
        private int axisTopY;
        private LocalizerDelegate localizer = new LocalizerDelegate();
        public MainWindow mv;
        private byte[] raw8BitBuffer;
        private byte[] raw16BitBuffer;
        private int bits;
        private int x;
        private int y;
        List<ushort> pixels16;
        List<ushort> pixels12;
        List<int> pixels16Int;
        public SeriesImagePanel()
        {
            InitializeComponent();
            Imageborder.Width = 1520;
            Imageborder.Height = 1070;
            DicomInstanceImage.Width = 512;
            DicomInstanceImage.Height = 512;
            DicomInstanceImage.MaxWidth = 512;
            DicomInstanceImage.MaxHeight = 512;
            ImageCanvas.Width = 512;
            ImageCanvas.Height = 512;
            winMin = 0;
            winMax = 65536;
            pix8 = new List<byte>();
            pix16 = new List<ushort>();
            pix24 = new List<byte>();
            pixels16 = new List<ushort>();
            lut8 = new byte[256];
            lut16 = new byte[65536];
            mousedowncheck = false;
            dicomDecoder = new DicomDec();
            dicomDirectoryRecordobj = new DicomDirectoryRecord();
            pixeldatWidth = 512;
            pixeldatHeight = 512;
            frameTime = 0;
            dicomDecoderPixel = new DicomDecoder();
            offset = 1;
            rescaleIntercept = 0.0; // Default value
            rescaleSlope = 1.0;
            // retrieveScoutParam();
        }

        public bool isLocalizerDicom()
        {
            return isLocalizer;


        }
        public double BorderWidth
        {
            get { return this.Imageborder.Width; }
            set
            {
                this.Imageborder.Width = value;
            }
        }

        public bool EnableLooping
        {
            get { return this.looping; }
            set { this.looping = value; }
        }

        public double BorderHeight
        {
            get { return Imageborder.Height; }                           
            set
            {
                Imageborder.Height = value;
            }
        }

        public void showoverlay(bool val)
        {
            if (val)
            {
                stackpanel_Windowtop.Visibility = Visibility.Visible;
                stackpanel_Window.Visibility = Visibility.Visible;
                stackpanel_Windowbottomright.Visibility = Visibility.Visible;
                stackpanel_Windowrighttop.Visibility = Visibility.Visible;
            }
            else
            {
                stackpanel_Windowtop.Visibility = Visibility.Collapsed;
                stackpanel_Window.Visibility = Visibility.Collapsed;
                stackpanel_Windowbottomright.Visibility = Visibility.Collapsed;
                stackpanel_Windowrighttop.Visibility = Visibility.Collapsed;
            }
        }
        public void AddFile(string fileName)
        {
            this.dicomlist.Add(fileName);
        }
        public string SeriesRecordPathFileName
        { get { return seriesRecordPathFileName; } set { seriesRecordPathFileName = value; } }

        public IPixelData GetPixels
        {
            get
            { return pixel; }
        }
        public bool Recttoolenable
        {
            get { return RectToolEnabled; }
            set { RectToolEnabled = value; }
        }
        public bool linetoolenable
        {
            get { return lineToolEnabled; }
            set { lineToolEnabled = value; }
        }
        public bool circletoolenable
        {
            get { return circleToolEnabled; }
            set { circleToolEnabled = value; }
        }
        public bool windowingtoolenable
        {
            get { return WindowingToolEnabled; }
            set { WindowingToolEnabled = value; }
        }
        public bool Scoutlinetoolenable
        {
            get { return scoutlineToolEnabled; }
            set { scoutlineToolEnabled = value; }
        }

        public bool Selectiontoolenable
        {
            get { return selectToolEnabled; }
            set { selectToolEnabled = value; }
        }
        public string Name
        {
            get
            { return name; }
            set { name = value; }
        }
        public double RotateTransforms
        {
            set
            {   rotateangle = value;
                if (DicomInstanceImage != null)
                    DicomInstanceImage.RenderTransform = new MatrixTransform(matrixtransformm(rotateangle));
            }
            get
            {
                return rotateangle;
            }
        }
        private Matrix matrixtransformm(double rotateang)
        {
            Matrix matrix = new Matrix();
            System.Windows.Point center = new System.Windows.Point(ImageCanvas.ActualWidth / 2.0, ImageCanvas.ActualHeight / 2.0);
            center = matrix.Transform(center);
            matrix.RotateAt(rotateang, center.X, center.Y);
            return matrix;                     
        }
        public void reset()
        {
            if (DicomInstanceImage != null)
                DicomInstanceImage.RenderTransform = new MatrixTransform();
        }
        public int Indexr
        {
            get { return indexr; }
            set { indexr = value; }
        }

        public double Winowwidth
        {
            get { return Swindowwidth; }
            set
            {
                Swindowwidth = value;
            }
        }
        public double Winowcenter
        {
            get { return SwindowCenter; }
            set
            {
                SwindowCenter = value;
            }
        }
        //18.08.22
        public bool isLocalizerInstance()
        {
            return isLocalizer;
        }

        public List<string> DicomList
        {
            get { return dicomlist; }
        }
        public void MoveForward()
        {
            Indexr++;
            DicomImage image = GetDicomImage(dicomlist[Indexr]);
            if (image != null)
            {
                image.WindowCenter = SwindowCenter;
                image.WindowWidth = Swindowwidth;
                if (pixeldatWidth == 0)
                {
                    pixeldatWidth = image.Width;
                    pixeldatHeight = image.Height;
                }
                Bitmap bmp = new Bitmap(image.RenderImage(0).As<Bitmap>());
                DicomInstanceImage.Source = BitmapToImageSource(bmp);
                this.lbl_Instance.Content = "Images:" + Indexr.ToString();
            }
        }
        public void OnetimeLoop()
        {
            while (Indexr < dicomlist.Count)
            {
                DicomImage image = GetDicomImage(dicomlist[Indexr]);
                if (image != null)
                {
                    image.WindowCenter = SwindowCenter;
                    image.WindowWidth = Swindowwidth;
                    if (pixeldatWidth == 0)
                    {
                        pixeldatWidth = image.Width;
                        pixeldatHeight = image.Height;
                    }
                    Bitmap bmp = new Bitmap(image.RenderImage(0).As<Bitmap>());
                    DicomInstanceImage.Source = BitmapToImageSource(bmp);
                    this.lbl_Instance.Content = "Images:" + Indexr.ToString();
                }
                Indexr++;
            }
        }
        public void MoveBackward()
        {
            if (Indexr != 0)
            {
                Indexr--;
                DicomImage image = GetDicomImage(dicomlist[Indexr]);
                if (image != null)
                {
                    image.WindowCenter = SwindowCenter;
                    image.WindowWidth = Swindowwidth;
                    if (pixeldatWidth == 0)
                    {
                        pixeldatWidth = image.Width;
                        pixeldatHeight = image.Height;
                    }
                    Bitmap bmp = new Bitmap(image.RenderImage(0).As<Bitmap>());
                    DicomInstanceImage.Source = BitmapToImageSource(bmp);
                    this.lbl_Instance.Content = "Images:" + Indexr.ToString();
                }
            }
        }
      
        public void Refresh()
        {
            if (dicomlist.Count() > 0)
            {
                DicomImage image = GetDicomImage(dicomlist[Indexr]);
                if (image != null)
                {
                    image.WindowCenter = SwindowCenter;
                    image.WindowWidth = Swindowwidth;
                    if (pixeldatWidth == 0)
                    {
                        // pixeldatwidth = image.Width;
                        // pixeldatheight = image.Height;
                    }
                    if (image.PixelData.NumberOfFrames>0)
                    {
                        var pixelData = image.PixelData.GetFrame(0).Data;
                    }

                    this.bits = image.Dataset.Get<int>(DicomTag.BitsStored);

                    //if (bits > 8)
                    //{
                    //    raw16BitBuffer = new byte[pixeldatwidth * pixeldatheight * 2];
                    //    Array.Copy(pixelData, raw16BitBuffer, pixelData.Length);
                    //}
                    //else
                    //{
                    //    raw8BitBuffer = new byte[pixeldatwidth * pixeldatwidth];
                    //    Array.Copy(pixelData, raw8BitBuffer, pixelData.Length);
                    //}
                    //var writeableBitmap = ConvertUtil.GetWriteableBitmap(pixelData, pixeldatwidth, pixeldatheight, this.bits);
                    //var imageSource = ConvertUtil.GetImageSource(writeableBitmap);
                   

                    Bitmap bmp = new Bitmap(image.RenderImage(0).As<Bitmap>());
                  //  var ratio = PanelRatio(image.Width, image.Height);
                    DicomInstanceImage.RenderTransform = new MatrixTransform();
                   // ImageCanvas.RenderTransform = new MatrixTransform(scalematrixtransformm(1));
                    //DicomInstanceImage.RenderTransform = new MatrixTransform();
                    DicomInstanceImage.Source = (BitmapToImageSource(bmp));
                    //var D = ImageCanvas.Width;
                    Imageborder.Focus();

                    //Bitmap bmp = new Bitmap(image.RenderImage(0).As<Bitmap>());
                    //DicomInstanceImage.Source = BitmapToImageSource(bmp);
                    //Imageborder.Focus();
                }
                SetWindowInfo(image);
            }
            //else if (isDicomDirFolderPath)
            //{
            //    DicomImage image;
            //    image = GetDicomImage(dicomDecoder.GetFilePath(dicomDirectoryRecordobj.LowerLevelDirectoryRecordCollection.ElementAt(indexr), seriesRecordPathFileName));
            //    if (image != null)
            //    {
            //        image.WindowCenter = SwindowCenter;
            //        image.WindowWidth = Swindowwidth;
            //        if (pixeldatwidth == 0)
            //        {
            //            pixeldatwidth = image.Width;
            //            pixeldatheight = image.Height;
            //        }
            //        var pixelData = image.PixelData.GetFrame(0).Data;
            //        this.bits = image.Dataset.Get<int>(DicomTag.BitsStored);

            //        if (bits > 8)
            //        {
            //            raw16BitBuffer = new byte[pixeldatwidth * pixeldatheight * 2];
            //            Array.Copy(pixelData, raw16BitBuffer, pixelData.Length);
            //        }
            //        else
            //        {
            //            raw8BitBuffer = new byte[pixeldatwidth * pixeldatheight];
            //            Array.Copy(pixelData, raw8BitBuffer, pixelData.Length);
            //        }

            //        var writeableBitmap = ConvertUtil.GetWriteableBitmap(pixelData, pixeldatwidth, pixeldatheight, this.bits);
            //        var imageSource = ConvertUtil.GetImageSource(writeableBitmap);
            //        DicomInstanceImage.RenderTransform = new MatrixTransform();
            //        this.DicomInstanceImage.Source = imageSource;
            //        Bitmap bmp = new Bitmap(image.RenderImage(0).As<Bitmap>());
            //        //DicomInstanceImage.RenderTransform = new MatrixTransform();
            //        DicomInstanceImage.Source = (BitmapToImageSource(bmp));
            //        var d = ImageCanvas.Width;
            //        Imageborder.Focus();

            //    }
            //    SetWindowInfo(image);
            //}

        }
        public double Zoom
        {
            set
            {
                if (value >= 0)
                    zoomin = value;
                else
                    zoomin = 0;
               // var wid = ImageCanvas.Width;
                ImageCanvas.RenderTransform = new MatrixTransform(scalematrixtransformm(zoomin));
                //if (DicomInstanceImage != null)
                //    DicomInstanceImage.RenderTransform = new MatrixTransform(scalematrixtransformm(zoomin));
                //if(ImageCanvas.Children.Count>1)
                //{
                //    var g = ImageCanvas.Children;
                //  g[1].RenderTransform = new ScaleTransform(zoomin,zoomin);
                //}                
                // ImageCanvas = new Canvas();               
            }
            get
            {
                return zoomin;
            }
        }

        public Person Person
        {
            get
            {
                _person.Id = 1;
                _person.Id = 2;
                return _person;
            }
            set
            {
                _person = value;
            }
        }

        public void FlipVTransform(double flipVetl)
        {
            if (DicomInstanceImage != null)
            {
                Matrix matrix = new Matrix();
                System.Windows.Point center = new System.Windows.Point(ImageCanvas.ActualWidth / 2.0, ImageCanvas.ActualHeight / 2.0);
                center = matrix.Transform(center);
                matrix.ScaleAt(Math.Abs(flipVetl), flipVetl, center.X, center.Y);
                ImageCanvas.RenderTransform = new MatrixTransform(matrix);
            }
               
            //ScaleTransform scaleTransform = new ScaleTransform();
            //scaleTransform.ScaleY = flipVetl;
            //scaleTransform.ScaleX = Math.Abs(flipVetl);
            //scaleTransform.CenterX = 256;
            //scaleTransform.CenterY = 256;
            //ImageCanvas.RenderTransform = scaleTransform;
        }

        public void FlipHTransform(double flipHztl)
        {
            if (DicomInstanceImage != null)
            {
                Matrix matrix = new Matrix();
                System.Windows.Point center = new System.Windows.Point(ImageCanvas.ActualWidth / 2.0, ImageCanvas.ActualHeight / 2.0);
                center = matrix.Transform(center);
                matrix.ScaleAt(flipHztl, Math.Abs(flipHztl), center.X, center.Y);
                ImageCanvas.RenderTransform = new MatrixTransform(matrix);            
            }

            //ScaleTransform scaleTransform = new ScaleTransform();
            //scaleTransform.CenterX = 256;
            //scaleTransform.CenterY = 256;
            //scaleTransform.ScaleX = flipHztl;
            //scaleTransform.ScaleY = Math.Abs(flipHztl);
            //ImageCanvas.RenderTransform = scaleTransform;
        }
        private Matrix scalematrixtransformm(double zoomvalue)
        {
            Matrix matrix = new Matrix();
            System.Windows.Point center = new System.Windows.Point(ImageCanvas.ActualWidth / 2.0, ImageCanvas.ActualHeight / 2.0);
            center = matrix.Transform(center);
            matrix.ScaleAt(zoomvalue, zoomvalue, center.X, center.Y);
            return matrix;
        }

        public void AddImageList(List<string> dicomlistnew)
        {
            dicomlist = dicomlistnew;
        }
        public void AddNewImage(DicomImage dicomImage)
        {
            dicomImageList.Add(dicomImage);
            // dicomlist.Add(dicomImage);
            // dicomImage1 = dicomImage;
            // if (dicomImage1 != null)
            // {
            //     Swindowwidth = dicomImage1.WindowWidth;
            //     SwindowCenter = dicomImage1.WindowCenter;
            ////     bitsstored = dicomImage1.Dataset.Get<int>(DicomTag.BitsStored);
            // //    samplesPerPixel = dicomImage1.Dataset.Get<int>(DicomTag.SamplesPerPixel);
            // //    PhotometricInterpretation = dicomImage1.PhotometricInterpretation;
            //     Bitmap bmp = new Bitmap(dicomImage1.RenderImage(0).As<Bitmap>());
            //     DicomInstanceImage.RenderTransform = new MatrixTransform();
            //     DicomInstanceImage.Source = (BitmapToImageSource(bmp));
            //     SetWindowInfo(dicomImage1);
            //     Zoom = 1.5;
            // }
        }



        public void AddSeriesRecord(DicomDirectoryRecord dicomDirectoryRecord)
        {

            dicomDirectoryRecordobj = dicomDirectoryRecord;
            // dicomlist = 
            //dicomlist =dicomDirectoryRecord.LowerLevelDirectoryRecordCollection.ToList();
        }
        //public int Width
        //{
        //       get { return width; }
        //       set { this.width = value; }
        //}
        //public int Height
        //{ 
        //    get { return height; }  
        //    set { this.height = value; }
        //} 


        //26.08

        public String getImageOrientation()
        {
            return imageOrientation;
        }

        public String getImagePosition()
        {
            return imagePosition;
        }

        public String getPixelSpacing()
        {
            return pixelSpacing;
        }
        public int getColumn()
        {
            return column;
        }

        public int getRow()
        {
            return row;
        }
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            //DicomImage dImage = GetDicomImage(dicomlist[0]);
            //DicomImage dImage = GetDicomImage(dicomDecoder.GetFilePath(dicomDirectoryRecordobj.LowerLevelDirectoryRecordCollection.First(), seriesRecordPathFileName));
            //Indexr++;
            //Indexr = Indexr >= dImage.NumberOfFrames ? Lastr : Indexr;
            //dImage.WindowWidth = Swindowwidth;
            //dImage.WindowCenter = SwindowCenter;
            //if (dImage.NumberOfFrames>1)
            //{
            //    Bitmap bmp = new Bitmap(dImage.RenderImage(Indexr).As<Bitmap>());
            //    DicomInstanceImage.Source = BitmapToImageSource(bmp);
            //    //  IPixelData pixelData = PixelDataFactory.Create(dImage.PixelData, 0);
            //    //  DicomRange<double> dicomRange = pixelData.GetMinMax();
            //    //  this.lbl_min.Content = "Min:" + dicomRange.Minimum.ToString();
            //    //  this.lbl_max.Content = "Max:" + dicomRange.Maximum.ToString();
            //    imagecount = Indexr;
            //    this.lbl_Instance.Content = "Images:" + imagecount++.ToString();
            //}
            if (isDicomDirFolderPath)
            {
                DicomImage dImage = GetDicomImage(dicomDecoder.GetFilePath(dicomDirectoryRecordobj.LowerLevelDirectoryRecordCollection.ElementAt(Indexr), seriesRecordPathFileName));
                j++;
                if (j < dImage.NumberOfFrames)
                {
                    //  Indexr++;
                    //  Indexr = Indexr >= dImage.NumberOfFrames ? Lastr : Indexr;                                  
                    {
                        if (dImage != null)
                        {
                            dImage.WindowWidth = Swindowwidth;
                            dImage.WindowCenter = SwindowCenter;
                            Bitmap bmp = new Bitmap(dImage.RenderImage(j).As<Bitmap>());
                            DicomInstanceImage.Source = BitmapToImageSource(bmp);
                            //IPixelData pixelData = PixelDataFactory.Create(dImage.PixelData, 0);
                            //DicomRange<double> dicomRange = pixelData.GetMinMax();
                            // this.lbl_min.Content = "Min:" + dicomRange.Minimum.ToString();
                            // this.lbl_max.Content = "Max:" + dicomRange.Maximum.ToString();
                            imagecount = j;
                            this.lbl_Instance.Content = "Images:" + imagecount++.ToString();
                        }
                    }
                }
                else
                {
                    Indexr++;
                    // Indexr = Indexr >= dicomlist.Count ? Lastr : Indexr;
                    Indexr = Indexr >= dicomDirectoryRecordobj.LowerLevelDirectoryRecordCollection.Count() ? Lastr : Indexr;
                    if (dImage != null)
                    {
                        dImage.WindowWidth = Swindowwidth;
                        dImage.WindowCenter = SwindowCenter;
                        Bitmap bmp = new Bitmap(dImage.RenderImage(0).As<Bitmap>());
                        DicomInstanceImage.Source = BitmapToImageSource(bmp);
                        //IPixelData pixelData = PixelDataFactory.Create(dImage.PixelData, 0);
                        //DicomRange<double> dicomRange = pixelData.GetMinMax();
                        //this.lbl_min.Content = "Min:" + dicomRange.Minimum.ToString();
                        //this.lbl_max.Content = "Max:" + dicomRange.Maximum.ToString();
                        imagecount = Indexr;
                        this.lbl_Instance.Content = "Images:" + imagecount++.ToString();
                    }
                }
            }
            else
            {
                DicomImage dImage = GetDicomImage(dicomlist[Indexr]);
                j++;
                if (j < dImage.NumberOfFrames)
                {
                    //  Indexr++;
                    //  Indexr = Indexr >= dImage.NumberOfFrames ? Lastr : Indexr;                                  
                    {
                        if (dImage != null)
                        {
                            dImage.WindowWidth = Swindowwidth;
                            dImage.WindowCenter = SwindowCenter;
                            Bitmap bmp = new Bitmap(dImage.RenderImage(j).As<Bitmap>());
                            DicomInstanceImage.Source = BitmapToImageSource(bmp);
                            //IPixelData pixelData = PixelDataFactory.Create(dImage.PixelData, 0);
                            //DicomRange<double> dicomRange = pixelData.GetMinMax();
                            // this.lbl_min.Content = "Min:" + dicomRange.Minimum.ToString();
                            // this.lbl_max.Content = "Max:" + dicomRange.Maximum.ToString();
                            imagecount = j;
                            this.lbl_Instance.Content = "Images:" + imagecount++.ToString();
                        }
                    }
                }
                else
                {
                    Indexr++;
                    Indexr = Indexr >= dicomlist.Count ? Lastr : Indexr;
                    if (looping != true && Indexr == 0)
                    {
                        dispatcherTimer.Stop();
                    }
                    if (dImage != null)
                    {
                        dImage.WindowWidth = Swindowwidth;
                        dImage.WindowCenter = SwindowCenter;
                        Bitmap bmp = new Bitmap(dImage.RenderImage(0).As<Bitmap>());
                        DicomInstanceImage.Source = BitmapToImageSource(bmp);
                        //IPixelData pixelData = PixelDataFactory.Create(dImage.PixelData, 0);
                        //DicomRange<double> dicomRange = pixelData.GetMinMax();
                        //this.lbl_min.Content = "M
                        //in:" + dicomRange.Minimum.ToString();
                        //this.lbl_max.Content = "Max:" + dicomRange.Maximum.ToString();
                        imagecount = Indexr;
                        this.lbl_Instance.Content = "Images:" + imagecount++.ToString();
                    }
                }
            }
        }
        public bool Sniploop()
        {
            if (frameTime <= 0.0)
            {
                frameTime = 500;
            }
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, frameTime);
            dispatcherTimer.Start();
            return true;
        }
        public bool stopsniploop()
        {
            dispatcherTimer.Stop();
            return true;
        }
        public bool SetInterval()
        {
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, frameTime);
            return true;
        }
        public DicomImage GetDicomImage(string dicomFile)
        {
            try
            {             
                    var dicomFile1 = DicomFile.Open(dicomFile);
                    DicomImage dicomimage = new DicomImage(dicomFile1.Dataset, 0);
                    return dicomimage;                           
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
                    
        }
               
        // Create a bitmap on the fly, using 8-bit grayscale pixel data
        private void CreateImage8(int imgWidth,int imgHeight)
        {
            BitmapData bmd = bmp1.LockBits(new System.Drawing.Rectangle(0, 0, imgWidth, imgHeight),
                System.Drawing.Imaging.ImageLockMode.ReadOnly, bmp1.PixelFormat);

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
            bmp1.UnlockBits(bmd);
        }

        public void ResetValues()
        {
            winMax = Convert.ToInt32(SwindowCenter + 0.5 * Swindowwidth);
            winMin = winMax - (int)Swindowwidth;
            //UpdateMainForm();
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
        private void ComputeLookUpTable16()
        {
            int range = winMax - winMin;
            if (range < 1) range = 1;
            double factor = 255.0 / range;
            int i;
            for (i = 0; i < 65536; ++i)
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
        private void CreateImage16(int imgWidth, int imgHeight)
        {
            BitmapData bmd = bmp1.LockBits(new System.Drawing.Rectangle(0, 0, imgWidth, imgHeight),
               System.Drawing.Imaging.ImageLockMode.ReadOnly, bmp1.PixelFormat);
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
            bmp1.UnlockBits(bmd);
        }

        private void ReadPixels(DicomImage dImagePixelCre)
        {
            this.bits = dImagePixelCre.Dataset.Get<int>(DicomTag.BitsAllocated);
                var samplesPerPixel= dImagePixelCre.Dataset.Get<int>(DicomTag.SamplesPerPixel);
                List<byte> pixels88=new List<byte>();
            //     if (samplesPerPixel == 1 && bits == 8)
            if (samplesPerPixel == 1 && bits == 16)
            {
                if (pixels16 != null)
                    pixels16.Clear();
                if (pixels16Int != null)
                    pixels16Int.Clear();

                pixels16 = new List<ushort>();
                pixels16Int = new List<int>();
                int numPixels = dImagePixelCre.Width * dImagePixelCre.Height;
                // byte[] bufByte = new byte[numPixels * 2];
                byte[] signedData = new byte[2];
                //   file.BaseStream.Position = offset;
                //   file.Read(bufByte, 0, numPixels * 2);
                ushort unsignedS;
                int i, i1, pixVal;
                byte b0, b1;
                Console.WriteLine("Sixteen bit Image ");

                for (i = 0; i < numPixels; ++i)
                {
                    i1 = i * 2;
                    b0 = pixeldata[i1];
                    b1 = pixeldata[i1 + 1];
                    unsignedS = Convert.ToUInt16((b1 << 8) + b0);
                    if (pixelRepresentation == 0) // Unsigned
                    {
                        pixVal = (int)(unsignedS * rescaleSlope + rescaleIntercept);
                        if (photometricinterpretation.Value == "MONOCHROME1")
                            pixVal = max16 - pixVal;
                    }
                    else  // Pixel representation is 1, indicating a 2s complement image
                    {
                        signedData[0] = b0;
                        signedData[1] = b1;
                        short sVal = System.BitConverter.ToInt16(signedData, 0);

                        // Need to consider rescale slope and intercepts to compute the final pixel value
                        pixVal = (int)(sVal * rescaleSlope + rescaleIntercept);
                        if (photometricinterpretation.Value == "MONOCHROME1")
                            pixVal = max16 - pixVal;
                    }
                    pixels16Int.Add(pixVal);
                }

                int minPixVal = pixels16Int.Min();
                signedImage = false;
                if (minPixVal < 0) signedImage = true;

                // Use the above pixel data to populate the list pixels16 
                foreach (int pixel in pixels16Int)
                {
                    // We internally convert all 16-bit images to the range 0 - 65535
                    //if (signedImage)
                    //    pixels16.Add((ushort)(pixel - min16));
                    //else
                    //    pixels16.Add((ushort)(pixel));
                    pixels16.Add((ushort)(pixel - (signedImage ? min16 : 0)));
                }

                pixels16Int.Clear();
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            Indexr = 0;
            j = 0;
            DicomImage dImage;
            if (dicomlist.Count > 0)
            {
                dImage = GetDicomImage(dicomlist[Indexr]);
               
                if (dImage != null)
                {
                    photometricinterpretation = dImage.PhotometricInterpretation;
                    Swindowwidth = dImage.WindowWidth;
                    SwindowCenter = dImage.WindowCenter;
                    // pixeldatwidth
                    //
                    // 0= dImage.Width;                  
                    // pixeldatheight = dImage.Height;                   
                    DicomPixelData pixelDatas = DicomPixelData.Create(dImage.Dataset);
                    // Get Raw Data
                    pixeldata = pixelDatas.GetFrame(0).Data;
                    ReadPixels(dImage);
                    // pixel = PixelDataFactory.Create(dImage.PixelData, 0);
                    // pixel.GetPixel()
                    // int[] output = {};
                    //  ILUT lut83=
                    //        pixel.Render(lut8, output)
                    this.bits = dImage.Dataset.Get<int>(DicomTag.BitsAllocated);
                    var samplesPerPixel = dImage.Dataset.Get<int>(DicomTag.SamplesPerPixel);
                    //if (samplesPerPixel==1&& bits==8)
                    //{
                    //    byte[] rawPixelData = dImage.PixelData.GetFrame(0).Data;
                    //    pix8 = new List<byte>(rawPixelData);
                    //    //pix8 = ReadPixels(dImage);
                    //    imageAvailable = true;
                    //    if (bmp1 != null)
                    //        bmp1.Dispose();
                    //    ResetValues();
                    //    ComputeLookUpTable8();
                    //    bmp1 = new Bitmap(dImage.Width, dImage.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                    //    CreateImage8(dImage.Width, dImage.Height);
                    //    PanelRatio(bmp1.Width, bmp1.Height);
                    //    DicomInstanceImage.RenderTransform = new MatrixTransform(scalematrixtransformm(1));
                    //    //ImageCanvas.RenderTransform = new ScaleTransform(1, 1, x, y);
                    //    DicomInstanceImage.Source = (BitmapToImageSource(bmp1));
                    //}                    
                    //else if(samplesPerPixel == 1 && bits == 16)
                    //{

                    //    if (pixels16 != null)
                    //    {
                    //        pix16 = pixels16;
                    //        int maxpixvalue=pixels16.Max();


                    //        // int max = pix16.Max(r => ((short)r));
                    //        imageAvailable = true;
                    //        if (bmp1 != null)
                    //            bmp1.Dispose();
                    //        ResetValues();
                    //        ComputeLookUpTable16();
                    //        bmp1 = new Bitmap(dImage.Width, dImage.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                    //        CreateImage16(dImage.Width, dImage.Height);
                    //        PanelRatio(bmp1.Width, bmp1.Height);
                    //        DicomInstanceImage.RenderTransform = new MatrixTransform(scalematrixtransformm(1));
                    //        //ImageCanvas.RenderTransform = new ScaleTransform(1, 1, x, y);
                    //        DicomInstanceImage.Source = BitmapToImageSource(bmp1);
                    //        // var imageSource = ConvertUtil.BitmapTo
                    //        //
                    //        //
                    //        // ImageSource(bmp1);
                    //        // DicomInstanceImage.Source = imageSource;
                    //      //  Imageborder.Focus();
                    //    }
                    //}
                    //  this.bits = dImage.Dataset.Get<int>(DicomTag.BitsStored);
                    //if (bits > 8)
                    //{
                    //    raw16BitBuffer = new byte[pixeldatwidth * pixeldatheight * 2];
                    //    Array.Copy(pixeldata, raw16BitBuffer, pixeldata.Length);
                    //}
                    //else
                    //{
                    //    raw8BitBuffer = new byte[pixeldatwidth * pixeldatwidth];
                    //    Array.Copy(pixeldata, raw8BitBuffer, pixeldata.Length);
                    //}

                  //  var writeableBitmap = ConvertUtil.GetWriteableBitmap(pixeldata, pixeldatwidth, pixeldatheight, this.bits);
                    PanelRatio(dImage.Width, dImage.Height);
                    dImage.UseVOILUT = false;
                    Bitmap bmp = new Bitmap(dImage.RenderImage(0).As<Bitmap>());
                    //  var ratio = PanelRatio(image.Width, image.Height);
                    DicomInstanceImage.RenderTransform = new MatrixTransform();
                
                    DicomInstanceImage.Source = (BitmapToImageSource(bmp));


                    //DicomInstanceImage.Source = ConvertUtil.GetImageSource(writeableBitmap);
                }
                else
                {
                    Console.WriteLine("dicomlist count is @ user control * ******" + dicomlist.Count);
                    DicomInstanceImage.Source = null;
                }
                SetWindowInfo(dImage);
               //  Zoom = 2;
            }
            else if (isDicomDirFolderPath)
            {
                dImage = GetDicomImage(dicomDecoder.GetFilePath(dicomDirectoryRecordobj.LowerLevelDirectoryRecordCollection.ElementAt(indexr), seriesRecordPathFileName));
                if (dImage != null)
                {
                    Swindowwidth = dImage.WindowWidth;
                    SwindowCenter = dImage.WindowCenter;
                    //  pixeldatwidth = dImage.Width;
                    //  pixeldatheight = dImage.Height;
                    Bitmap bmp = new Bitmap(dImage.RenderImage(0).As<Bitmap>());
                    PanelRatio(dImage.Width, dImage.Height);
                    DicomInstanceImage.RenderTransform = new MatrixTransform(scalematrixtransformm(1));
                    DicomInstanceImage.Source = (BitmapToImageSource(bmp));
                }
            }
       retrieveScoutParam();
            //ImageCanvas.RenderTransform = new ScaleTransform(2.0, 2.0, 512 / 2, 512 / 2);
        }
        public BitmapImage BitmapToImageSource(Bitmap bitmap)
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
        int imagecount = 0;

        public void setScoutCoordinates(int line1X1, int line1Y1, int line1X2, int line1Y2, int line2X1, int line2Y1, int line2X2, int line2Y2)
        {
            displayScout = true;
            if (ScoutLine1 != null)
            {
                ScoutLine1.Stroke = Brushes.Green;
                ScoutLine1.StrokeThickness = 2;
                ScoutLine1.X1 = line1X1;
                ScoutLine1.X2 = line1Y1;
                ScoutLine1.Y1 = line1X2;
                ScoutLine1.Y2 = line1Y2;
            }
            else
            {
                ScoutLine1 = new Line()
                {
                    Stroke = Brushes.Green,
                    StrokeThickness = 2,
                    X1 = line1X1,
                    X2 = line1Y1,
                    Y1 = line1X2,
                    Y2 = line1Y2

                };
                ImageCanvas.Children.Add(ScoutLine1);
            }
            //ImageCanvas.Children.Remove(ScoutLine1);

            Refresh();
        }

        public void setAxisCoordinates(int leftx, int lefty, int rightx, int righty, int topx, int topy, int bottomx, int bottomy)
        {
            axisLeftX = leftx;
            axisLeftY = lefty;
            axisRightX = rightx;
            axisRightY = righty;
            axisTopX = topx;
            axisTopY = topy;
            axisBottomX = bottomx;
            axisBottomY = bottomy;

            displayScout = true;
            if (ScoutLine1 != null)
            {
                ScoutLine1.Stroke = Brushes.Green;
                ScoutLine1.StrokeThickness = 2;
                ScoutLine1.X1 = axisLeftX;
                ScoutLine1.Y1 = axisLeftY;
                ScoutLine1.X2 = axisRightX;
                ScoutLine1.Y2 = axisRightY;
            }
            else
            {
                ScoutLine1 = new Line()
                {
                    Stroke = Brushes.Green,
                    StrokeThickness = 2,
                    X1 = axisLeftX,
                    Y1 = axisLeftY,
                    X2 = axisRightX,
                    Y2 = axisRightY

                };
                ImageCanvas.Children.Add(ScoutLine1);
            }

            Refresh();
        }

        public void retrieveScoutParam()
        {
            try
            {
                DicomImage dImage = GetDicomImage(dicomlist[Indexr]);
                frameOfReferenceUID = dImage.Dataset.Get<string>(DicomTag.FrameOfReferenceUID) != null ? dImage.Dataset.Get<string>(DicomTag.FrameOfReferenceUID) : "";
                imagePosition = dImage.Dataset.Get<string>(DicomTag.ImagePositionPatient, 0) != null ? dImage.Dataset.Get<string>(DicomTag.ImagePositionPatient, 0) + "\\" + dImage.Dataset.Get<string>(DicomTag.ImagePositionPatient, 1) + "\\" + dImage.Dataset.Get<string>(DicomTag.ImagePositionPatient, 2) : null;
                imageOrientation = dImage.Dataset.Get<string>(DicomTag.ImageOrientationPatient) != null ? dImage.Dataset.Get<string>(DicomTag.ImageOrientationPatient, 0) + "\\" + dImage.Dataset.Get<string>(DicomTag.ImageOrientationPatient, 1) + "\\" + dImage.Dataset.Get<string>(DicomTag.ImageOrientationPatient, 2) + "\\" + dImage.Dataset.Get<string>(DicomTag.ImageOrientationPatient, 3) + "\\" + dImage.Dataset.Get<string>(DicomTag.ImageOrientationPatient, 4) + "\\" + dImage.Dataset.Get<string>(DicomTag.ImageOrientationPatient, 5) : null;
                //imageType = dImage.Dataset.Get(DicomTag.ImageType) != null ? dImage.Dataset.Get<string>(DicomTag.ImageType) : null;
                pixelSpacing = dImage.Dataset.Get<string>(DicomTag.PixelSpacing) != null ? dImage.Dataset.Get<string>(DicomTag.PixelSpacing, 0) + "\\" + dImage.Dataset.Get<string>(DicomTag.PixelSpacing, 1) : null;
                row = dImage.Dataset.Get<int>(DicomTag.Rows) != null ? dImage.Dataset.Get<int>(DicomTag.Rows) : 0;
                column = dImage.Dataset.Get<int>(DicomTag.Columns) != null ? dImage.Dataset.Get<int>(DicomTag.Columns) : 0;
                //Dataset referencedImageSequence = dataset.getItem(Tag.ReferencedImageSequence) != null ? dataset.getItem(Tag.ReferencedImageSequence) : null;
                imageType = dImage.Dataset.Get<string[]>(DicomTag.ImageType);
                if (imageType != null)
                {
                    if (imageType.Length >= 3 && imageType[2].SequenceEqual("LOCALIZER"))
                    {
                        isLocalizer = true;
                    }
                    else
                    {
                        //if (referencedImageSequence != null)
                        //{
                        //    referencedSOPInstanceUID = referencedImageSequence.getString(Tag.ReferencedSOPInstanceUID);
                        //}
                        isLocalizer = false;
                    }
                }

                //   findOrientation();
            }
            catch (Exception e)
            {
                // e.printStackTrace();
            }
        }
        //private void findOrientation()
        //{

        //    string [] imageOrientationArray;
        //    if (imageOrientation != null)
        //    {
        //        imageOrientationArray = imageOrientation.Split('\\');
        //        float _imgRowCosx = float.Parse(imageOrientationArray[0]);
        //        float _imgRowCosy = float.Parse(imageOrientationArray[1]);
        //        float _imgRowCosz = float.Parse(imageOrientationArray[2]);
        //        float _imgColCosx = float.Parse(imageOrientationArray[3]);
        //        float _imgColCosy = float.Parse(imageOrientationArray[4]);
        //        float _imgColCosz = float.Parse(imageOrientationArray[5]);
        //        orientationLabel = getOrientationLabelFromImageOrientation(_imgRowCosx, _imgRowCosy, _imgRowCosz, _imgColCosx, _imgColCosy, _imgColCosz);
        //        if (orientationLabel.equalsIgnoreCase("CORONAL") || orientationLabel.equalsIgnoreCase("SAGITTAL"))
        //        {
        //            isLocalizer = true;
        //        }
        //    }
        //}

        private void DoActionUp()
        {
            if (isDicomDirFolderPath)
            {
                DicomImage dImage = GetDicomImage(dicomDecoder.GetFilePath(dicomDirectoryRecordobj.LowerLevelDirectoryRecordCollection.ElementAt(Indexr), seriesRecordPathFileName));
                j++;
                if (j < dImage.NumberOfFrames)
                {
                    //  Indexr++;
                    //  Indexr = Indexr >= dImage.NumberOfFrames ? Lastr : Indexr;                                  
                    {
                        if (dImage != null)
                        {
                            dImage.WindowWidth = Swindowwidth;
                            dImage.WindowCenter = SwindowCenter;
                            Bitmap bmp = new Bitmap(dImage.RenderImage(j).As<Bitmap>());
                            DicomInstanceImage.Source = BitmapToImageSource(bmp);
                            //IPixelData pixelData = PixelDataFactory.Create(dImage.PixelData, 0);
                            //DicomRange<double> dicomRange = pixelData.GetMinMax();
                            // this.lbl_min.Content = "Min:" + dicomRange.Minimum.ToString();
                            // this.lbl_max.Content = "Max:" + dicomRange.Maximum.ToString();
                            imagecount = j;
                            this.lbl_Instance.Content = "Images:" + imagecount++.ToString();
                        }
                    }
                }
                else
                {
                    Indexr++;
                    // Indexr = Indexr >= dicomlist.Count ? Lastr : Indexr;
                    Indexr = Indexr >= dicomDirectoryRecordobj.LowerLevelDirectoryRecordCollection.Count() ? Lastr : Indexr;
                    if (dImage != null)
                    {
                        dImage.WindowWidth = Swindowwidth;
                        dImage.WindowCenter = SwindowCenter;

                        Bitmap bmp = new Bitmap(dImage.RenderImage(0).As<Bitmap>());
                        DicomInstanceImage.Source = BitmapToImageSource(bmp);
                        //IPixelData pixelData = PixelDataFactory.Create(dImage.PixelData, 0);
                        //DicomRange<double> dicomRange = pixelData.GetMinMax();
                        //this.lbl_min.Content = "Min:" + dicomRange.Minimum.ToString();
                        //this.lbl_max.Content = "Max:" + dicomRange.Maximum.ToString();
                        imagecount = Indexr;
                        this.lbl_Instance.Content = "Images:" + imagecount++.ToString();
                    }
                }

                //imagePosition = dImage.Dataset.Get<string>(DicomTag.ImagePositionPatient, 0) != null ? dImage.Dataset.Get<string>(DicomTag.ImagePositionPatient, 0) + "\\" + dImage.Dataset.Get<string>(DicomTag.ImagePositionPatient, 1) + "\\" + dImage.Dataset.Get<string>(DicomTag.ImagePositionPatient, 2) : null;
            }
            else
            {
                DicomImage dImage = GetDicomImage(dicomlist[Indexr]);
                j++;
                if (j < dImage.NumberOfFrames)
                {
                    //  Indexr++;
                    //  Indexr = Indexr >= dImage.NumberOfFrames ? Lastr : Indexr;                                  
                    {
                        if (dImage != null)
                        {
                            dImage.WindowWidth = Swindowwidth;
                            dImage.WindowCenter = SwindowCenter;


                            Bitmap bmp = new Bitmap(dImage.RenderImage(j).As<Bitmap>());
                            DicomInstanceImage.Source = BitmapToImageSource(bmp);
                            //IPixelData pixelData = PixelDataFactory.Create(dImage.PixelData, 0);
                            //DicomRange<double> dicomRange = pixelData.GetMinMax();
                            // this.lbl_min.Content = "Min:" + dicomRange.Minimum.ToString();
                            // this.lbl_max.Content = "Max:" + dicomRange.Maximum.ToString();
                            imagecount = j;
                            this.lbl_Instance.Content = "Images:" + imagecount++.ToString();
                        }
                    }
                }
                else
                {
                    Indexr++;

                    Indexr = Indexr >= dicomlist.Count ? Lastr : Indexr;
                    if (dImage != null)
                    {
                        dImage.WindowWidth = Swindowwidth;
                        dImage.WindowCenter = SwindowCenter;
                        Bitmap bmp = new Bitmap(dImage.RenderImage(0).As<Bitmap>());
                        DicomInstanceImage.Source = BitmapToImageSource(bmp);
                        //   IPixelData pixelData = PixelDataFactory.Create(dImage.PixelData, 0);
                        //DicomRange<double> dicomRange = pixelData.GetMinMax();
                        // this.lbl_min.Content = "Min:" + dicomRange.Minimum.ToString();
                        // this.lbl_max.Content = "Max:" + dicomRange.Maximum.ToString();
                        imagecount = Indexr;
                        this.lbl_Instance.Content = "Images:" + imagecount++.ToString();
                        if (mv != null)
                        {
                            retrieveScoutParam();
                            localizer.projectSlice(mv);
                        }
                        //else
                        //{
                        //    MessageBox.Show("MainWindow is null");
                        //}
                    }
                }
                // imagePosition = dImage.Dataset.Get<string>(DicomTag.ImagePositionPatient, 0) != null ? dImage.Dataset.Get<string>(DicomTag.ImagePositionPatient, 0) + "\\" + dImage.Dataset.Get<string>(DicomTag.ImagePositionPatient, 1) + "\\" + dImage.Dataset.Get<string>(DicomTag.ImagePositionPatient, 2) : null;
            }
        }

        private void Border_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            //Imageborder.Background = Brushes.Black;
            //  DicomImage dImage = GetDicomImage(dicomlist[Indexr]);
            if (e.Delta > 0)
                DoActionUp();

            else if (e.Delta < 0)
                DoActionDown();   
        }

        private void DoActionDown()
        {
            if (isDicomDirFolderPath)
            {
                DicomImage dImage = GetDicomImage(dicomDecoder.GetFilePath(dicomDirectoryRecordobj.LowerLevelDirectoryRecordCollection.ElementAt(Indexr), seriesRecordPathFileName));
                j--;
                if (j < dImage.NumberOfFrames)
                {
                    //  Indexr++;
                    //  Indexr = Indexr >= dImage.NumberOfFrames ? Lastr : Indexr;                                  
                    {
                        if (dImage != null)
                        {
                            dImage.WindowWidth = Swindowwidth;
                            dImage.WindowCenter = SwindowCenter;
                            Bitmap bmp = new Bitmap(dImage.RenderImage(j).As<Bitmap>());
                            DicomInstanceImage.Source = BitmapToImageSource(bmp);
                            //IPixelData pixelData = PixelDataFactory.Create(dImage.PixelData, 0);
                            //DicomRange<double> dicomRange = pixelData.GetMinMax();
                            // this.lbl_min.Content = "Min:" +
                            //
                            // dicomRange.Minimum.ToString();
                            // this.lbl_max.Content = "Max:" + dicomRange.Maximum.ToString();
                            imagecount = j;
                            this.lbl_Instance.Content = "Images:" + imagecount++.ToString();
                        }
                    }
                }
                else
                {
                    Indexr--;
                    // Indexr = Indexr >= dicomlist.Count ? Lastr : Indexr;
                    Indexr = Indexr >= dicomDirectoryRecordobj.LowerLevelDirectoryRecordCollection.Count() ? Lastr : Indexr;
                    if (dImage != null)
                    {
                        
                        
                        dImage.WindowWidth = Swindowwidth;
                        dImage.WindowCenter = SwindowCenter;

                        Bitmap bmp = new Bitmap(dImage.RenderImage(0).As<Bitmap>());
                        DicomInstanceImage.Source = BitmapToImageSource(bmp);
                        //IPixelData pixelData = PixelDataFactory.Create(dImage.PixelData, 0);
                        //DicomRange<double> dicomRange = pixelData.GetMinMax();



                        //this.lbl_min.Content = "Min:" + dicomRange.Minimum.ToString();
                        //this.lbl_max.Content = "Max:" + dicomRange.Maximum.ToString();
                        imagecount = Indexr;
                        this.lbl_Instance.Content = "Images:" + imagecount++.ToString();
                    }
                }

                //imagePosition = dImage.Dataset.Get<string>(DicomTag.ImagePositionPatient, 0) != null ? dImage.Dataset.Get<string>(DicomTag.ImagePositionPatient, 0) + "\\" + dImage.Dataset.Get<string>(DicomTag.ImagePositionPatient, 1) + "\\" + dImage.Dataset.Get<string>(DicomTag.ImagePositionPatient, 2) : null;
            }
            else
            {
                DicomImage dImage = GetDicomImage(dicomlist[Indexr]);
                j--;
                if (j < dImage.NumberOfFrames)
                {
                    //  Indexr++;
                    //  Indexr = Indexr >= dImage.NumberOfFrames ? Lastr : Indexr;                                  
                    {
                        if (dImage != null)
                        {
                            dImage.WindowWidth = Swindowwidth;
                            dImage.WindowCenter = SwindowCenter;
                            Bitmap bmp = new Bitmap(dImage.RenderImage(j).As<Bitmap>());
                            DicomInstanceImage.Source = BitmapToImageSource(bmp);
                            //IPixelData pixelData = PixelDataFactory.Create(dImage.PixelData, 0);
                            //DicomRange<double> dicomRange = pixelData.GetMinMax();
                            // this.lbl_min.Content = "Min:" + dicomRange.Minimum.ToString();
                            // this.lbl_max.Content = "Max:" + dicomRange.Maximum.ToString();
                            imagecount = j;
                            this.lbl_Instance.Content = "Images:" + imagecount++.ToString();
                        }
                    }
                }
                else
                {
                    Indexr--;
                    Indexr = Indexr >= dicomlist.Count ? Lastr : Indexr;

                    if (dImage != null)
                    {
                        dImage.WindowWidth = Swindowwidth;
                        dImage.WindowCenter = SwindowCenter;
                        Bitmap bmp = new Bitmap(dImage.RenderImage(0).As<Bitmap>());
                        DicomInstanceImage.Source = BitmapToImageSource(bmp);
                        //   IPixelData pixelData = PixelDataFactory.Create(dImage.PixelData, 0);
                        //DicomRange<double> dicomRange = pixelData.GetMinMax();
                        // this.lbl_min.Content = "Min:" + dicomRange.Minimum.ToString();
                        // this.lbl_max.Content = "Max:" + dicomRange.Maximum.ToString();
                        imagecount = Indexr;
                        this.lbl_Instance.Content = "Images:" + imagecount++.ToString();
                        if (mv != null)
                        {
                            retrieveScoutParam();
                            localizer.projectSlice(mv);
                        }
                        //else
                        //{
                        //    MessageBox.Show("MainWindow is null");
                        //}
                    }
                }
                // imagePosition = dImage.Dataset.Get<string>(DicomTag.ImagePositionPatient, 0) != null ? dImage.Dataset.Get<string>(DicomTag.ImagePositionPatient, 0) + "\\" + dImage.Dataset.Get<string>(DicomTag.ImagePositionPatient, 1) + "\\" + dImage.Dataset.Get<string>(DicomTag.ImagePositionPatient, 2) : null;
            }
        }



        // [Obsolete]
        private void SetWindowInfo(DicomImage dicomImage)
        {
            try
            {
                this.lbl_WL.Content = $"WL:{dicomImage.WindowCenter}";
                this.lbl_WW.Content = $"WW:{dicomImage.WindowWidth}";
                this.lbl_Name.Content = dicomImage.Dataset.Get<string>(DicomTag.PatientName).ToString();
                this.lbl_Modality.Content = dicomImage.Dataset.Get<string>(DicomTag.Modality).ToString();
                this.lbl_PatientId.Content = "Patient ID:" + dicomImage.Dataset.Get<string>(DicomTag.PatientID).ToString();
                if (dicomImage.Dataset.Contains(DicomTag.SeriesDescription))
                    this.lbl_SeriesDescription.Content = dicomImage.Dataset.Get<string>(DicomTag.SeriesDescription).ToString();
                //if (dicomImage.Dataset.Contains(DicomTag.PatientBirthDate))
                //    this.lbl_PatientBirthdate.Content = dicomImage.Dataset.Get<DateTime>(DicomTag.PatientBirthDate);
                if (dicomImage.Dataset.Contains(DicomTag.StudyDescription))
                {
                    //this.lbl_StudyDescription.Content = dicomImage.Dataset.GetSequence(DicomTag.StudyDescription).ToString();
                    this.lbl_StudyDescription.Content = dicomImage.Dataset.Get<string>(DicomTag.StudyDescription, "StudyDescription").ToString();
                }
                this.lbl_Instance.Content = "Images:" + dicomImage.NumberOfFrames.ToString();
                if (dicomImage.Dataset.Contains(DicomTag.StudyDate))
                    this.lbl_StudyDate.Content = dicomImage.Dataset.Get<DateTime>(DicomTag.StudyDate).ToString();
                if (dicomImage.Dataset.Contains(DicomTag.SeriesNumber))
                    this.lbl_Series.Content = "Series No:" + dicomImage.Dataset.Get<int>(DicomTag.SeriesNumber).ToString();
                IPixelData pixelData = PixelDataFactory.Create(dicomImage.PixelData, 0);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            // DicomRange<double> dicomRange = pixelData.GetMinMax();
            // this.lbl_min.Content = "Min:" + dicomRange.Minimum.ToString();
            // this.lbl_max.Content = "Max:" + dicomRange.Maximum.ToString();
        }
        //public void zoom(double value)
        //{

        //   
        
        // //if (delta == 0)
        //   // //    return;

        //   // //if (delta < 0 && scaleTransform.ScaleX < MIN_ZOOMRATIO)
        //   // //    return;

        //   // //if (delta > 0 && scaleTransform.ScaleX > MAX_ZOOMRATIO)
        //   // //    return;

        //   // var ratio = 0.0;
        //   // //if (delta > 0)
        //   //// {
        //   //     ratio = scaleTransform.ScaleX * ZOOM_STEP;
        //   //// }
        //   // //else
        //   // //{
        //   // //    ratio = scaleTransform.ScaleX * -ZOOM_STEP;
        //   // //}
        //   // scaleTransform.CenterX = this.DicomInstanceImage.ActualWidth / 2.0;
        //   // scaleTransform.CenterY = this.DicomInstanceImage.ActualHeight / 2.0;

        //   // //TODO use animation
        //   // scaleTransform.ScaleX += ratio;
        //   // scaleTransform.ScaleY = Math.Abs(scaleTransform.ScaleX);

        //}
        //public void pixelvalue()
        //{
        //    //if (imageAvailable == true)
        //    //{
        //    //    var element = sender as UIElement;
        //    //    var position = e.GetPosition(element);

        //    //    ptWLDown.X = (int)position.X;
        //    //    ptWLDown.Y = (int)position.Y;
        //    //    rightMouseDown = false;

        //    //    ImageSource imageSource = DicomInstanceImage.Source;
        //    //    BitmapSource bitmapImage = (BitmapSource)imageSource;
        //    //    // var d = bmp.GetPixel(ptWLDown.X, ptWLDown.Y);
        //    //    //  Console.WriteLine("Pixel value is : *****" + d);
        //    //    double x = (e.GetPosition(image).X * bitmapImage.PixelWidth / image.ActualWidth);
        //    //    double y = (e.GetPosition(image).Y * bitmapImage.PixelHeight / image.ActualHeight);


        //    //    // var pixeldatatest = DicomImageSOURCE.PixelData; // returns DicomPixelData type
        //    //    IPixelData pixelData = PixelDataFactory.Create(DicomImageSOURCE.PixelData, 0); // returns IPixelData type



        //    //    if (pixelData is Dicom.Imaging.Render.GrayscalePixelDataU16)
        //    //    {
        //    //        //    // for (int i = 0; i < pixelData.Width; i++)
        //    //        // {
        //    //        //  for (int j = 0; j < pixelData.Height; j++)
        //    //        //  {
        //    //        int rounded_2 = (int)Math.Round(x);
        //    //        int rounded_2y = (int)Math.Round(y);
        //    //        if (rounded_2 >= 0 && rounded_2y >= 0)
        //    //        {
        //    //            //Console.WriteLine("{0}", Convert.ToSingle(pixelData.GetPixel(ptWLDown.X, ptWLDown.Y)));
        //    //            lbl_Hu.Content = "X value: " + rounded_2 + " Y value: " + rounded_2y + "  " + Convert.ToSingle(pixelData.GetPixel((int)rounded_2, (int)rounded_2y)).ToString();
        //    //        }
        //    //        else
        //    //        {
        //    //            lbl_Hu.Content = "X value: " + rounded_2 + " Y value: " + rounded_2y + "  " + "Out of boundary";
        //    //        }
        //    //        // }
        //    //        //   }
        //    //    }
        //    //    Cursor = Cursors.Hand;
        //    //}
        //}

        void OnEllipseMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            Ellipse ellipse = (Ellipse)sender;
            ellipse.Stroke = Brushes.Green;
        }
       // [Obsolete]
        private void Imageborder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // if (imageAvailable == true)
            //{
            var element = sender as UIElement;
            startpoint = e.GetPosition(DicomInstanceImage);
            ImageSource imageSource = DicomInstanceImage.Source;
            BitmapSource bitmapImage = (BitmapSource)imageSource;
            // var d = bmp.GetPixel(ptWLDown.X, ptWLDown.Y);
            //  Console.WriteLine("Pixel value is : *****" + d);
            dicomSelectionPoint = new System.Windows.Point();
            dicomSelectionPoint.X = (e.GetPosition(DicomInstanceImage).X * bitmapImage.PixelWidth / DicomInstanceImage.ActualWidth);
            dicomSelectionPoint.Y = (e.GetPosition(DicomInstanceImage).Y * bitmapImage.PixelHeight / DicomInstanceImage.ActualHeight);
            //startpoint.X = dicomSelectionPoint.X;
            //startpoint.Y = dicomSelectionPoint.Y;
            DicomImage dImage;
            if (isDicomDirFolderPath)
            {
                dImage = GetDicomImage(dicomDecoder.GetFilePath(dicomDirectoryRecordobj.LowerLevelDirectoryRecordCollection.ElementAt(Indexr), seriesRecordPathFileName));
            }
            else
            {
                dImage = GetDicomImage(dicomlist[Indexr]);
            }
            IPixelData pixelData = null;
            if (dImage.PhotometricInterpretation != PhotometricInterpretation.Rgb)
            {
                if(dImage.PixelData!=null)
                   pixelData = PixelDataFactory.Create(dImage.PixelData, 0);//04.02.2023
            }
            // returns IPixelData type
            if (dImage.Dataset.Contains(DicomTag.RescaleIntercept))
                rescaleIntercept = dImage.Dataset.Get<int>(DicomTag.RescaleIntercept);
            if (dImage.Dataset.Contains(DicomTag.RescaleSlope))
                rescaleSlope = dImage.Dataset.Get<int>(DicomTag.RescaleSlope);
            if (dImage.Dataset.Contains(DicomTag.PixelSpacing))
                pixelSpacingValue = dImage.Dataset.Get<double>(DicomTag.PixelSpacing);          
            if (pixelData!=null)
            {               
                int pX = (int)Math.Round(dicomSelectionPoint.X);
                int pY = (int)Math.Round(dicomSelectionPoint.Y);
                if (pX >= 0 && pY >= 0 && pX < dImage.Width && pY < dImage.Height)
                {
                    double pixelValue = (pixelData.GetPixel((int)pX, (int)pY));
                    lbl_PX.Content = "X: " + pX + " Y: " + pY + " PX: " + Convert.ToSingle(pixelValue) + "  HU: " + (pixelValue * rescaleSlope + rescaleIntercept); ;
                }
                else
                {
                    lbl_PX.Content = "X value: " + " Y value: " + " PX: ";
                }              
            }
            if (WindowingToolEnabled)
            {
                mousedowncheck = true;
            }
            if (RectToolEnabled)
            {
                if (rectSelectArea != null)
                    ImageCanvas.Children.Remove(rectSelectArea);

                // Initialize the rectangle.
                // Set border color and width
                rectSelectArea = new Rectangle
                {
                    Stroke = Brushes.DarkGreen,
                    StrokeThickness = 2,
                    Tag = "s"
                };
                textBlock1 = new TextBlock
                {
                    Foreground = Brushes.Yellow,
                    Width = 70,
                    Height = 70,
                    Background = Brushes.Brown,
                };
                Canvas.SetLeft(rectSelectArea, startpoint.X);
                Canvas.SetTop(rectSelectArea, startpoint.X);
                ImageCanvas.Children.Add(rectSelectArea);
                mousedowncheck = true;
            }
            else if (lineToolEnabled)
            {
                if (linetool != null)
                    ImageCanvas.Children.Remove(linetool);
                linetool = new Line
                {
                    Stroke = Brushes.IndianRed,
                    StrokeThickness = 2,
                    X1 = startpoint.X,
                    Y1 = startpoint.Y,
                    X2 = startpoint.X,
                    Y2 = startpoint.Y,
                    Tag = "s"
                };
                textBlock1 = new TextBlock
                {
                    Text = "H",
                    Foreground = Brushes.Yellow,
                    Width = 50,
                    Height = 25,
                    Background = Brushes.Brown,
                    Tag = "s"
                };
                path1.Visibility = Visibility.Visible;
                Canvas.SetLeft(path1, startpoint.X);
                Canvas.SetTop(path1, startpoint.Y);
                ImageCanvas.Children.Add(linetool);
                mousedowncheck = true;
                rightMouseDown = true;
            }
            else if (circleToolEnabled)
            {
                ellipseArea = new Ellipse
                {
                    Stroke = Brushes.Red,
                    StrokeThickness = 2,
                    Tag = "s"
                };
                ellipseArea.MouseLeftButtonDown += OnEllipseMouseLeftButtonDown;
                textBlock1 = new TextBlock
                {
                    Foreground = Brushes.Yellow,
                    Width = 70,
                    Height = 70,
                    Background = Brushes.Brown,
                };
                ImageCanvas.Children.Add(ellipseArea);
                mousedowncheck = true;
                rightMouseDown = true;
            }
            else if (selectToolEnabled)
            {
            }
            rightMouseDown = true;
            Cursor = Cursors.Hand;

            

        }

        private string calculateDiff(int mouseLocX1, int mouseLocY1, int mouseLocX2, int mouseLocY2)
        {
            double diff;
            string returnString = "";                               
            diff = (double)Math.Sqrt(Math.Pow(((mouseLocY2 - mouseLocY1) / this.currentScaleFactor), 2) + Math.Pow(((mouseLocX2 - mouseLocX1) / this.currentScaleFactor) *(pixelSpacingValue), 2));             
            //returnValue = diff / 10;
            var distanceBP = (int)Math.Round(diff);
           return returnString = distanceBP + "mm";
                     
        }

        public void delete()
        {          
            for (int i = ImageCanvas.Children.Count - 1; i >= 0; i += -1)
            {
                UIElement Child = ImageCanvas.Children[i];
                if (Child is Line)
                    ImageCanvas.Children.Remove(Child);
                else if (Child is Rectangle)
                    ImageCanvas.Children.Remove(Child);
                else if (Child is Ellipse)
                    ImageCanvas.Children.Remove(Child);
                else if (Child is TextBlock)
                    ImageCanvas.Children.Remove(Child);
                path1.Visibility = Visibility.Collapsed;
                path2.Visibility = Visibility.Collapsed;
            }
        }
       // [Obsolete]
        private void Imageborder_MouseMove(object sender, MouseEventArgs e)
        {
            // DetermineMouseSensitivity();
            if (rightMouseDown == true)
            {

                //         //  Invalidate();
                if (mousedowncheck == true)
                {
                    //if (e.LeftButton == MouseButtonState.Released || rectSelectArea == null)
                    //    return;
                    //  var element = sender as UIElement;

                    var pos = e.GetPosition(DicomInstanceImage);

                    ImageSource imageSource = DicomInstanceImage.Source;
                    BitmapSource bitmapImage = (BitmapSource)imageSource;
                    // var d = bmp.GetPixel(ptWLDown.X, ptWLDown.Y);
                    //  Console.WriteLine("Pixel value is : *****" + d);
                    dicomEndePoint = new System.Windows.Point();
                    dicomEndePoint.X = (e.GetPosition(DicomInstanceImage).X * bitmapImage.PixelWidth / DicomInstanceImage.ActualWidth);
                    dicomEndePoint.Y = (e.GetPosition(DicomInstanceImage).Y * bitmapImage.PixelHeight / DicomInstanceImage.ActualHeight);               
                    if (lineToolEnabled)
                    {                  
                        linetool.X1 = startpoint.X;
                        linetool.Y1 = startpoint.Y;
                        linetool.X2 = Mouse.GetPosition(DicomInstanceImage).X;
                        linetool.Y2 = Mouse.GetPosition(DicomInstanceImage).Y;
                        linetool.Stroke = Brushes.Black;

                        // DrawLine(startpoint, pos);
                      //  var x = Math.Min(dicomendepoint.X, startpoint.X);
                      //  var y = Math.Min(dicomendepoint.Y, startpoint.Y);

                     //   double dx = startpoint.X - dicomendepoint.X;
                     //   double dy = startpoint.Y - dicomendepoint.Y;
                        DicomImage dImages;
                        if (isDicomDirFolderPath)
                        {
                            dImages = GetDicomImage(dicomDecoder.GetFilePath(dicomDirectoryRecordobj.LowerLevelDirectoryRecordCollection.ElementAt(Indexr), seriesRecordPathFileName));
                        }
                        else
                        {
                            dImages = GetDicomImage(dicomlist[Indexr]);
                        }                      
                        var geometry = new FrameGeometry(dImages.Dataset);
                        //// lets say click1 and click2 are two Point instances and give you the pixels where you want to do the measurement. 
                        //// for examle the whole diagonale of a 512x512 ct image: click1 = (0/0) and click2 = (511/511)
                        //// Point2 point2;
                        var patientCoord1 = geometry.TransformImagePointToPatient(new Point2((int)dicomSelectionPoint.X, (int)dicomSelectionPoint.Y));
                        var patientCoord2 = geometry.TransformImagePointToPatient(new Point2((int)dicomEndePoint.X, (int)dicomEndePoint.Y));
                        double distanceInMM = patientCoord1.Distance(patientCoord2);
                     //     double distanceMeasure = ((double)startpoint.X * (double)Mouse.GetPosition(ImageCanvas).X + (double)startpoint.Y * (double)Mouse.GetPosition(ImageCanvas).Y);
                        if (distanceInMM > 0)
                        {
                       // double distancecalculates = (distanceMeasure * pixelspacingvalue);
          //              textBlock1.Text = calculateDiff((int)dicomSelectionPoint.X, (int)dicomSelectionPoint.Y, (int)dicomEndePoint.X, (int)dicomEndePoint.Y);
                       textBlock1.Text = (distanceInMM).ToString();

                        lbl_PX.Content = "distanceMeasure: " + textBlock1.Text;
                        }
                        // Canvas.SetLeft(linetool, startpoint.X);
                        // Canvas.SetTop(linetool, startpoint.Y);
                        Cursor = Cursors.Hand;
                        //  e.Handled = true;
                      //  double distanceMeasure = (double)Math.Sqrt(Math.Pow(((mouseLocY2 - mouseLocY1) / this.layeredCanvas.imgpanel.getCurrentScaleFactor()), 2) + Math.pow(((mouseLocX2 - mouseLocX1) / this.layeredCanvas.imgpanel.getCurrentScaleFactor()), 2));
                    }
                    if (RectToolEnabled)
                    {
                        //  Set the position of rectangle
                        var x = Math.Min(pos.X, startpoint.X);
                        var y = Math.Min(pos.Y, startpoint.Y);

                        // Set the dimenssion of the rectangle
                        var w = Math.Max(pos.X, startpoint.X) - x;
                        var h = Math.Max(pos.Y, startpoint.Y) - y;

                        rectSelectArea.Width = w;
                        rectSelectArea.Height = h;
                        Canvas.SetLeft(rectSelectArea, x);
                        Canvas.SetTop(rectSelectArea, y);
                        textBlock1.Text = "width:" + w + "\n" + "height:" + h + "\n" + "Area:" + w * h;
                        Cursor = Cursors.Hand;
                    }
                    if (circleToolEnabled)
                    {
                        double minX = Math.Min(pos.X, startpoint.X);
                        double minY = Math.Min(pos.Y, startpoint.Y);
                        double maxX = Math.Max(pos.X, startpoint.X);
                        double maxY = Math.Max(pos.Y, startpoint.Y);

                        Canvas.SetTop(ellipseArea, minY);
                        Canvas.SetLeft(ellipseArea, minX);

                        double height = maxY - minY;
                        double width = maxX - minX;

                        ellipseArea.Height = Math.Abs(height);
                        ellipseArea.Width = Math.Abs(width);
                        textBlock1.Text = "width:" + ellipseArea.Width + "\n" + "height:" + ellipseArea.Height + "\n" + "Area:" + (3.14 * ellipseArea.Width * ellipseArea.Height);

                    }

                    if (WindowingToolEnabled)
                    {

                        Swindowwidth -= (int)((startpoint.X - (int)pos.X));
                        SwindowCenter -= (int)((startpoint.Y - (int)pos.Y));
                        startpoint.X = (int)pos.X;
                        startpoint.Y = (int)pos.Y;
                        this.Refresh();
                    }
                }
            }
        }

        void DrawLine(System.Windows.Point spt, System.Windows.Point ept)
        {
            Line link = new Line();
            link.X1 = spt.X;
            link.Y1 = spt.Y;
            link.X2 = ept.X;
            link.Y2 = ept.Y;
            link.Stroke = Brushes.Black;
            if (ImageCanvas.Children.Count > 2)
            {
                ImageCanvas.Children.RemoveAt(ImageCanvas.Children.Count - 1);
            }
            ImageCanvas.Children.Add(link);
        }

        private void Imageborder_MouseUp(object sender, MouseButtonEventArgs e)
        {

            if (mousedowncheck == true && rightMouseDown == true)
            {
                if (lineToolEnabled)
                {
                    Canvas.SetLeft(textBlock1, Mouse.GetPosition(ImageCanvas).X);
                    Canvas.SetTop(textBlock1, Mouse.GetPosition(ImageCanvas).Y);
                    Canvas.SetLeft(path2, Mouse.GetPosition(ImageCanvas).X);
                    Canvas.SetTop(path2, Mouse.GetPosition(ImageCanvas).Y);
                    ImageCanvas.Children.Add(textBlock1);
                }
                if (RectToolEnabled)
                {
                    Canvas.SetLeft(textBlock1, Mouse.GetPosition(ImageCanvas).X);
                    Canvas.SetTop(textBlock1, Mouse.GetPosition(ImageCanvas).Y);
                    ImageCanvas.Children.Add(textBlock1);
                }
                if (circleToolEnabled)
                {
                    Canvas.SetLeft(textBlock1, Mouse.GetPosition(ImageCanvas).X);
                    Canvas.SetTop(textBlock1, Mouse.GetPosition(ImageCanvas).Y);
                    ImageCanvas.Children.Add(textBlock1);
                }
                rectSelectArea = null;
                ellipseArea = null;
                mousedowncheck = false;
                linetool = null;
                rightMouseDown = false;
            }

        }

        private void Imageborder_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            if (e.Manipulators.Count() == 1 && !twomanipulate && !threemanipulate && !fivemanipulate)
            {
                Swindowwidth -= e.DeltaManipulation.Translation.X;
                SwindowCenter -= e.DeltaManipulation.Translation.Y;
               // Refresh();
            }
            else if (e.Manipulators.Count() == 2 && !fivemanipulate && !threemanipulate)
            {
                twomanipulate = true;
                Console.WriteLine("2 manipulators entered");
                var transform = ImageCanvas.RenderTransform as MatrixTransform;
                if (transform != null)
                {
                    var matrix = transform.Matrix;
                    // System.Windows.Point center = new System.Windows.Point(centerwith, centerheight);
                    // center = matrix.Transform(center);

                    matrix.ScaleAt(e.DeltaManipulation.Scale.X, e.DeltaManipulation.Scale.X, e.ManipulationOrigin.X, e.ManipulationOrigin.Y);
                    //matrix.RotateAt(e.DeltaManipulation.Rotation, center.X, center.Y);
                    //matrix.Translate(e.DeltaManipulation.Translation.X, e.DeltaManipulation.Translation.Y);
                    // DicomInstanceImage.RenderTransform = new MatrixTransform(matrix);
                    ImageCanvas.RenderTransform = new MatrixTransform(matrix);
                }
            }
            else if (e.Manipulators.Count() == 3 && !fivemanipulate)
            {
                threemanipulate = true;
                var transform = ImageCanvas.RenderTransform as MatrixTransform;
                if (transform != null)
                {
                    var matrix = transform.Matrix;
                    System.Windows.Point center = new System.Windows.Point(ImageCanvas.ActualWidth / 2.0, ImageCanvas.ActualHeight / 2.0);
                    center = matrix.Transform(center);
                    //matrix.ScaleAt(e.DeltaManipulation.Scale.X, e.DeltaManipulation.Scale.X, center.X, center.Y);
                    //matrix.RotateAt(e.DeltaManipulation.Rotation, center.X, center.Y);
                    matrix.Translate(e.DeltaManipulation.Translation.X, e.DeltaManipulation.Translation.Y);
                    ImageCanvas.RenderTransform = new MatrixTransform(matrix);
                }

            }
            else if (e.Manipulators.Count() == 5)
            {
                fivemanipulate = true;

                Matrix matrix = new Matrix();
                System.Windows.Point center = new System.Windows.Point(ImageCanvas.ActualWidth / 2.0, ImageCanvas.ActualHeight / 2.0);
                    center = matrix.Transform(center);
                    Console.WriteLine("degree *************" + e.DeltaManipulation.Rotation);
                    //matrix.ScaleAt(e.DeltaManipulation.Scale.X, e.DeltaManipulation.Scale.X, center.X, center.Y);
                    matrix.RotateAt(e.DeltaManipulation.Rotation, center.X, center.Y);
                //matrix.Translate(e.DeltaManipulation.Translation.X, e.DeltaManipulation.Translation.Y);
                ImageCanvas.RenderTransform = new MatrixTransform(matrix);
                
            }
            else
            {
                Console.WriteLine("No manipulators:::::::::");
            }
        }

        private void Imageborder_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            // e.ManipulationContainer
            twomanipulate = false;
            threemanipulate = false;
            fivemanipulate = false;
        }

        private void canvas1_ManipulationInertiaStarting(object sender, ManipulationInertiaStartingEventArgs e)
        {
            twomanipulate = false;
            threemanipulate = false;
            fivemanipulate = false;
        }

        //Developed by Ramesh.B
        //03.11.2022
        private double PanelRatio(double imageWidthParam, double imageHeightParam)
        {
            var panel_ratio = DicomInstanceImage.Width / DicomInstanceImage.Height;
            var image_ratio = imageWidthParam / imageHeightParam;
            //return panel_ratio > image_ratio?
            //    DicomInstanceImage.Height / imageHeightParam
            //    : DicomInstanceImage.Width / imageWidthParam;

            if (panel_ratio < image_ratio)
            {
                // double rathei=DicomInstanceImage.Height / imageHeightParam;
                DicomInstanceImage.Height = (DicomInstanceImage.Width / image_ratio);
                ImageCanvas.Height = (DicomInstanceImage.Height / image_ratio);
            }
            else
            {
                //double ratewid= DicomInstanceImage.Width / imageWidthParam;
                DicomInstanceImage.Width = (DicomInstanceImage.Height * image_ratio);
                ImageCanvas.Width = (DicomInstanceImage.Height * image_ratio);
            }
          
            x = (int)(DicomInstanceImage.MaxWidth - DicomInstanceImage.Width) / 2;
            y = (int)(DicomInstanceImage.MaxHeight - DicomInstanceImage.Height) / 2;

            return panel_ratio;
        }
    }
}




