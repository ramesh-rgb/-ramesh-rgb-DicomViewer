using Dicom;
using Dicom.Imaging;
using Dicom.Imaging.Mathematics;
using Dicom.Imaging.Render;
using Dicom.Media;
using System;
using System.Collections.Generic;
using System.Drawing;
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
    public partial class SeriesImagePanel : UserControl
    {

        //List<DicomFile> dicomFiles = new List<DicomFile>();
        List<string> dicomlist = new List<string>();
        List<DicomImage> dicomImageList = new List<DicomImage>();
        private string name;
        private double zoomin = 0.0;
        private double rotateangle;
        private double Swindowwidth = 0.0;
        private double SwindowCenter = 0.0;

        //private double angle;
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
        //int winMin;
        //int winMax;
        //int winCentre;
        //int winWidth;
        //int winWidthBy2;
        //int deltaX;
        //int deltaY; 
        //double changeValWidth;
        //double changeValCentre;
        bool rightMouseDown;
        //bool imageAvailable;
        //bool signed16Image;
        private DispatcherTimer dispatcherTimer = new DispatcherTimer();
        public int frameTime;
        public bool threemanipulate = false;
        public bool twomanipulate = false;
        public bool fivemanipulate = false;
        private System.Windows.Point startpoint;
        private System.Windows.Point dicominstancepoint;
        private System.Windows.Point dicomendepoint;
        private Rectangle rectSelectArea;
        private Line linetool;
        private TextBlock textBlock1;
        private bool mousedowncheck;
        private bool linetoolenabled = false;
        private bool Recttoolenabled = false;
        private bool circletoolenabled = false;
        private bool selecttoolenabled = false;
        private bool WindowingToolEnabled = false;
        private Ellipse ellipsearea;
        private int pixeldatwidth;
        private int pixeldatheight;
        private double pixelspacingvalue;
      //private DicomImage dicomImage1;
        private string seriesRecordPathFileName;
        private DicomDirectoryRecord dicomDirectoryRecordobj;
        DicomDec dicomDecoder;
        private bool isDicomDirFolderPath;
        private bool looping=true;
        //18.08.2022

        private String frameOfReferenceUID;
        private String imagePosition;
        private String imageOrientation;
        private String[] imageType;
        private String referencedSOPInstanceUID = "";
        private String pixelSpacing;
        private int row;
        private int column;
        private bool isLocalizer = false;
        private static bool displayScout = false;
        private int scoutLine1X1;
        private int scoutLine1Y1;
        private int scoutLine1X2;
        private int scoutLine1Y2;
        private int scoutLine2X1;
        private int scoutLine2Y1;
        private int scoutLine2X2;
        private int scoutLine2Y2;

        public SeriesImagePanel()
        {
            InitializeComponent();
            //ptWLDown=new System.Drawing.Point();
            //winMin = 0;
            //winMax = 65535;
            //changeValWidth = 0.5;
            //changeValCentre = 20.0;
            mousedowncheck = false;
            dicomDecoder = new DicomDec();
            dicomDirectoryRecordobj = new DicomDirectoryRecord();
            BorderWidth = 1520;
            BorderHeight = 1070;
            frameTime = 0;
            retrieveScoutParam();
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

        public void  showoverlay(bool val)
        {
           if(val)
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
        {   get
            { return pixel; }
        }
        public bool Recttoolenable
        {
            get { return Recttoolenabled; }
            set { Recttoolenabled = value; }
        }
        public bool linetoolenable
        {
            get { return linetoolenabled; }
            set { linetoolenabled = value; }
        }
        public bool circletoolenable
        {
            get { return circletoolenabled; }
            set { circletoolenabled = value; }
        }
        public bool windowingtoolenable
        {
            get { return WindowingToolEnabled; }
            set { WindowingToolEnabled = value; }
        }
        public bool Selectiontoolenable
        {
            get { return selecttoolenabled; }
             set { selecttoolenabled = value; }
        }
        public string Name
        {
            get
            { return name; }
            set { name = value; }
        }
        public double RotateTransforms
        {
            set {
                rotateangle = value;
                if(DicomInstance!=null)                   
                DicomInstance.RenderTransform= new MatrixTransform(matrixtransformm(rotateangle));
                }            
            get
                {
                return rotateangle;
                }
        }
        private Matrix matrixtransformm(double rotateang)
        {
            Matrix matrix = new Matrix();
            matrix.RotateAt(rotateang, 256, 256);
            return matrix;
        }

        public void reset()      
        {         
                if (DicomInstance != null)
                    DicomInstance.RenderTransform = new MatrixTransform();  
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

        public List<string>DicomList
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
                if (pixeldatwidth == 0)
                {
                    pixeldatwidth = image.Width;
                    pixeldatheight = image.Height;
                }
                Bitmap bmp = new Bitmap(image.RenderImage(0).As<Bitmap>());
                DicomInstance.Source = BitmapToImageSource(bmp);
                this.lbl_Instance.Content = "Images:" + Indexr.ToString();
            }
            
        }
        public void OnetimeLoop()
        {
            while (Indexr<dicomlist.Count)
            {              
                DicomImage image = GetDicomImage(dicomlist[Indexr]);
                if (image != null)
                {
                    image.WindowCenter = SwindowCenter;
                    image.WindowWidth = Swindowwidth;
                    if (pixeldatwidth == 0)
                    {
                        pixeldatwidth = image.Width;
                        pixeldatheight = image.Height;
                    }
                    Bitmap bmp = new Bitmap(image.RenderImage(0).As<Bitmap>());
                    DicomInstance.Source = BitmapToImageSource(bmp);
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
                    if (pixeldatwidth == 0)
                    {
                        pixeldatwidth = image.Width;
                        pixeldatheight = image.Height;
                    }
                    Bitmap bmp = new Bitmap(image.RenderImage(0).As<Bitmap>());
                    DicomInstance.Source = BitmapToImageSource(bmp);
                    this.lbl_Instance.Content = "Images:" + Indexr.ToString();
                }
                
            }
        }
        [Obsolete]
        public void Refresh()
        {
            if (dicomlist.Count() > 0)
            {
                DicomImage image = GetDicomImage(dicomlist[Indexr]);
                if (image != null)
                {
                    image.WindowCenter = SwindowCenter;
                    image.WindowWidth = Swindowwidth;
                    if (pixeldatwidth == 0)
                    {
                        pixeldatwidth = image.Width;
                        pixeldatheight = image.Height;
                    }

                    Bitmap bmp = new Bitmap(image.RenderImage(0).As<Bitmap>());
                    DicomInstance.Source = BitmapToImageSource(bmp);
                    Imageborder.Focus();
                }
                SetWindowInfo(image);
            }
            else if(isDicomDirFolderPath)
            {
                DicomImage image;
                image = GetDicomImage(dicomDecoder.GetFilePath(dicomDirectoryRecordobj.LowerLevelDirectoryRecordCollection.ElementAt(indexr), seriesRecordPathFileName));
                if (image != null)
                {
                    image.WindowCenter = SwindowCenter;
                    image.WindowWidth = Swindowwidth;
                    if (pixeldatwidth == 0)
                    {
                        pixeldatwidth = image.Width;
                        pixeldatheight = image.Height;
                    }
                    Bitmap bmp = new Bitmap(image.RenderImage(0).As<Bitmap>());
                    //DicomInstance.RenderTransform = new MatrixTransform();
                    DicomInstance.Source = (BitmapToImageSource(bmp));
                    Imageborder.Focus();

                }
                SetWindowInfo(image);
            }
          
        }
        public double Zoom
        {
            set
            {
                if(value>=0)
                zoomin = value;
                else
                zoomin = 0;
                canvas1.RenderTransform = new ScaleTransform(zoomin, zoomin, pixeldatwidth/2,pixeldatheight/2);
                //if (DicomInstance != null)
                //   DicomInstance.RenderTransform = new MatrixTransform(scalematrixtransformm(zoomin));
                //if(canvas1.Children.Count>1)
                //{
                //    var g = canvas1.Children;
                //  g[1].RenderTransform = new ScaleTransform(zoomin,zoomin);
                //}                
                // canvas1 = new Canvas();               
            }    
            get { 
                return zoomin;
            } 
        }

        public void FlipVTransform(double flipVetl)
        {
            ScaleTransform scaleTransform = new ScaleTransform();
            scaleTransform.ScaleY = flipVetl;
            scaleTransform.ScaleX = Math.Abs(flipVetl);
            scaleTransform.CenterX = 256;
            scaleTransform.CenterY = 256;
            canvas1.RenderTransform = scaleTransform;
        }
        public void FlipHTransform(double flipHztl)
        {
            ScaleTransform scaleTransform = new ScaleTransform();
            scaleTransform.CenterX = 256;
            scaleTransform.CenterY = 256;
            scaleTransform.ScaleX = flipHztl;
            scaleTransform.ScaleY = Math.Abs(flipHztl);
            canvas1.RenderTransform = scaleTransform;
        }
        private Matrix scalematrixtransformm(double zoomvalue)
        {
            Matrix matrix = new Matrix();
            matrix.ScaleAt(zoomvalue, zoomvalue,256,256);
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
         //     DicomInstance.RenderTransform = new MatrixTransform();
         //     DicomInstance.Source = (BitmapToImageSource(bmp));
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
            //    DicomInstance.Source = BitmapToImageSource(bmp);
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
                            DicomInstance.Source = BitmapToImageSource(bmp);
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
                        DicomInstance.Source = BitmapToImageSource(bmp);
                        //   IPixelData pixelData = PixelDataFactory.Create(dImage.PixelData, 0);
                        //DicomRange<double> dicomRange = pixelData.GetMinMax();
                        // this.lbl_min.Content = "Min:" + dicomRange.Minimum.ToString();
                        // this.lbl_max.Content = "Max:" + dicomRange.Maximum.ToString();
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
                            DicomInstance.Source = BitmapToImageSource(bmp);
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
                        if(looping!=true&&Indexr==0)
                        {
                           dispatcherTimer.Stop();                      
                        }                 

                    if (dImage != null)
                    {
                        dImage.WindowWidth = Swindowwidth;
                        dImage.WindowCenter = SwindowCenter;
                        Bitmap bmp = new Bitmap(dImage.RenderImage(0).As<Bitmap>());
                        DicomInstance.Source = BitmapToImageSource(bmp);
                        //   IPixelData pixelData = PixelDataFactory.Create(dImage.PixelData, 0);
                        //DicomRange<double> dicomRange = pixelData.GetMinMax();
                        // this.lbl_min.Content = "Min:" + dicomRange.Minimum.ToString();
                        // this.lbl_max.Content = "Max:" + dicomRange.Maximum.ToString();
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
                DicomImage dicomimage = new DicomImage(dicomFile, 0);
          
                return dicomimage;
            }
            catch (Exception)
            {
                return null;
               
            }        
           // DicomImage dicomimage = new DicomImage(dicomFile.Dataset, 0);
           
        }

        [Obsolete]
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);            
               Indexr = 0;
               j = 0;
            DicomImage dImage;    
             if (dicomlist.Count>0)
              {
                dImage = GetDicomImage(dicomlist[Indexr]);
                if (dImage != null)
                {
                    Swindowwidth = dImage.WindowWidth;
                    SwindowCenter = dImage.WindowCenter;
                    pixeldatwidth = dImage.Width;
                    pixeldatheight = dImage.Height;
                    Bitmap bmp = new Bitmap(dImage.RenderImage(0).As<Bitmap>());
                    DicomInstance.RenderTransform = new MatrixTransform();
                    DicomInstance.Source = (BitmapToImageSource(bmp));
                }
                else
                {
                    Console.WriteLine("dicomlist count is @ user control * ******" + dicomlist.Count);
                    DicomInstance.Source = null;
                }
                SetWindowInfo(dImage);
              //  Zoom = 0.5;
            }
            else if(isDicomDirFolderPath)
            {
                dImage = GetDicomImage(dicomDecoder.GetFilePath(dicomDirectoryRecordobj.LowerLevelDirectoryRecordCollection.ElementAt(indexr), seriesRecordPathFileName));
                if (dImage != null)
                {
                    Swindowwidth = dImage.WindowWidth;
                    SwindowCenter = dImage.WindowCenter;
                    pixeldatwidth = dImage.Width;
                    pixeldatheight = dImage.Height;
                    Bitmap bmp = new Bitmap(dImage.RenderImage(0).As<Bitmap>());
                    DicomInstance.RenderTransform = new MatrixTransform();
                    DicomInstance.Source = (BitmapToImageSource(bmp));
                }

            }
            retrieveScoutParam();
            //canvas1.RenderTransform = new ScaleTransform(2.0, 2.0, 512 / 2, 512 / 2);
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
            scoutLine1X1 = line1X1;
            scoutLine1X2 = line1X2;
            scoutLine1Y1 = line1Y1;
            scoutLine1Y2 = line1Y2;
            scoutLine2X1 = line2X1;
            scoutLine2X2 = line2X2;
            scoutLine2Y1 = line2Y1;
            scoutLine2Y2 = line2Y2;
        }

        //public void setAxisCoordinates(int leftx, int lefty, int rightx, int righty, int topx, int topy, int bottomx, int bottomy)
        //{
        //    axisLeftX = leftx;
        //    axisLeftY = lefty;
        //    axisRightX = rightx;
        //    axisRightY = righty;
        //    axisTopX = topx;
        //    axisTopY = topy;
        //    axisBottomX = bottomx;
        //    axisBottomY = bottomy;
        //}

        private void retrieveScoutParam()
        {
            try
            {
                DicomImage dImage = GetDicomImage(dicomlist[Indexr]);
                frameOfReferenceUID = dImage.Dataset.Get<string>(DicomTag.FrameOfReferenceUID) != null ? dImage.Dataset.Get<string>(DicomTag.FrameOfReferenceUID) : "";
                imagePosition = dImage.Dataset.Get<string>(DicomTag.ImagePositionPatient, 0) != null ? dImage.Dataset.Get<string>(DicomTag.ImagePositionPatient, 0) + "\\" + dImage.Dataset.Get<string>(DicomTag.ImagePositionPatient, 1) + "\\" + dImage.Dataset.Get<string>(DicomTag.ImagePositionPatient, 2) : null;
                imageOrientation = dImage.Dataset.Get<string>(DicomTag.ImageOrientationPatient) != null ? dImage.Dataset.Get<string>(DicomTag.ImageOrientationPatient, 0) + "\\" + dImage.Dataset.Get<string>(DicomTag.ImageOrientationPatient, 1) + "\\" + dImage.Dataset.Get<string>(DicomTag.ImageOrientationPatient, 2) + "\\" + dImage.Dataset.Get<string>(DicomTag.ImageOrientationPatient, 3) + "\\" + dImage.Dataset.Get<string>(DicomTag.ImageOrientationPatient, 4) + "\\" + dImage.Dataset.Get<string>(DicomTag.ImageOrientationPatient, 5) : null;
               // imageType = dImage.Dataset.Get(DicomTag.ImageType) != null ? dImage.Dataset.Get<string>(DicomTag.ImageType) : null;
                pixelSpacing = dImage.Dataset.Get<string>(DicomTag.PixelSpacing) != null ? dImage.Dataset.Get<string>(DicomTag.PixelSpacing, 0) + "\\" + dImage.Dataset.Get<string>(DicomTag.PixelSpacing, 1) : null;
                //row = dImage.Dataset.Get<string>(DicomTag.Rows) != null ? Integer.parseInt(dataset.getString(Tags.Rows)) : 0;
                //column = dImage.Dataset.Get<string>(DicomTag.Columns) != null ? Integer.parseInt(dataset.getString(Tags.Columns)) : 0;
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

        private void Border_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            //Imageborder.Background = Brushes.Black;
            //  DicomImage dImage = GetDicomImage(dicomlist[Indexr]);
            if(isDicomDirFolderPath)
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
                            DicomInstance.Source = BitmapToImageSource(bmp);
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
                        DicomInstance.Source = BitmapToImageSource(bmp);
                        //   IPixelData pixelData = PixelDataFactory.Create(dImage.PixelData, 0);
                        //DicomRange<double> dicomRange = pixelData.GetMinMax();
                        // this.lbl_min.Content = "Min:" + dicomRange.Minimum.ToString();
                        // this.lbl_max.Content = "Max:" + dicomRange.Maximum.ToString();
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
                            DicomInstance.Source = BitmapToImageSource(bmp);
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
                        DicomInstance.Source = BitmapToImageSource(bmp);
                        //   IPixelData pixelData = PixelDataFactory.Create(dImage.PixelData, 0);
                        //DicomRange<double> dicomRange = pixelData.GetMinMax();
                        // this.lbl_min.Content = "Min:" + dicomRange.Minimum.ToString();
                        // this.lbl_max.Content = "Max:" + dicomRange.Maximum.ToString();
                        imagecount = Indexr;
                        this.lbl_Instance.Content = "Images:" + imagecount++.ToString();
                    }
                }
            }
                       
        }

        [Obsolete]
        private void SetWindowInfo(DicomImage dicomImage)
        {
            try
            {
                this.lbl_WL.Content = $"WL:{dicomImage.WindowCenter}";
                this.lbl_WW.Content = $"WW:{dicomImage.WindowWidth}";
                this.lbl_Name.Content = dicomImage.Dataset.Get<string>(DicomTag.PatientName).ToString();
                this.lbl_Modality.Content = dicomImage.Dataset.Get<string>(DicomTag.Modality).ToString();
                this.lbl_PatientId.Content = "Patient ID:"+dicomImage.Dataset.Get<string>(DicomTag.PatientID).ToString();
                if (dicomImage.Dataset.Contains(DicomTag.SeriesDescription))
                    this.lbl_SeriesDescription.Content = dicomImage.Dataset.Get<string>(DicomTag.SeriesDescription).ToString();
                //if (dicomImage.Dataset.Contains(DicomTag.PatientBirthDate))
                //    this.lbl_PatientBirthdate.Content = dicomImage.Dataset.Get<DateTime>(DicomTag.PatientBirthDate);
                if (dicomImage.Dataset.Contains(DicomTag.StudyDescription))
                {
                    //this.lbl_StudyDescription.Content = dicomImage.Dataset.GetSequence(DicomTag.StudyDescription).ToString();
                    this.lbl_StudyDescription.Content = dicomImage.Dataset.Get<string>(DicomTag.StudyDescription,"StudyDescription").ToString();
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
           
         //   DicomRange<double> dicomRange = pixelData.GetMinMax();
           // this.lbl_min.Content = "Min:" + dicomRange.Minimum.ToString();
           // this.lbl_max.Content = "Max:" + dicomRange.Maximum.ToString();
        }
        //public void zoom(double value)
        //{

        //   // //if (delta == 0)
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
        //   // scaleTransform.CenterX = this.DicomInstance.ActualWidth / 2.0;
        //   // scaleTransform.CenterY = this.DicomInstance.ActualHeight / 2.0;

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

        //    //    ImageSource imageSource = DicomInstance.Source;
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
        [Obsolete]
        private void Imageborder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // if (imageAvailable == true)
            //{
            var element = sender as UIElement;
            startpoint = e.GetPosition(canvas1);
       //     string name = ((Shape)sender).Name;
            //ptWLDown.X = (int)startpoint.X;
            //ptWLDown.Y = (int)startpoint.Y;
            // rightMouseDown = false;
            ImageSource imageSource = DicomInstance.Source;
            BitmapSource bitmapImage = (BitmapSource)imageSource;
            // var d = bmp.GetPixel(ptWLDown.X, ptWLDown.Y);
            //  Console.WriteLine("Pixel value is : *****" + d);
            dicominstancepoint = new System.Windows.Point();
            dicominstancepoint.X = (e.GetPosition(DicomInstance).X * bitmapImage.PixelWidth / DicomInstance.ActualWidth);
            dicominstancepoint.Y = (e.GetPosition(DicomInstance).Y * bitmapImage.PixelHeight / DicomInstance.ActualHeight);
            // var pixeldatatest = DicomImageSOURCE.PixelData; // returns DicomPixelData type
            //
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
            if (dImage.PhotometricInterpretation != PhotometricInterpretation.Monochrome2 && dImage.PhotometricInterpretation!= PhotometricInterpretation.Rgb)
            {
                pixelData = PixelDataFactory.Create(dImage.PixelData, 0);
            }        
            // returns IPixelData type
            if (dImage.Dataset.Contains(DicomTag.RescaleIntercept))
            rescaleIntercept = dImage.Dataset.Get<int>(DicomTag.RescaleIntercept);
            if (dImage.Dataset.Contains(DicomTag.RescaleSlope))
                rescaleSlope = dImage.Dataset.Get<int>(DicomTag.RescaleSlope);
            if (dImage.Dataset.Contains(DicomTag.PixelSpacing))
            pixelspacingvalue = dImage.Dataset.Get<Double>(DicomTag.SamplesPerPixel);
            if (WindowingToolEnabled)
            {
                mousedowncheck = true;

            }
            if (Recttoolenabled)
            {
                if (rectSelectArea != null)
                    canvas1.Children.Remove(rectSelectArea);

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

                canvas1.Children.Add(rectSelectArea);
                mousedowncheck = true;
            }
            else if (linetoolenabled)
            {
                if (linetool != null)
                    canvas1.Children.Remove(linetool);
                linetool = new Line
                {
                    Stroke = Brushes.IndianRed,
                    StrokeThickness = 2,
                    X1 = startpoint.X,
                    Y1 = startpoint.Y,
                    X2 = Mouse.GetPosition(canvas1).X,
                    Y2 = Mouse.GetPosition(canvas1).Y,
                    Tag="s"
                    
                };
                textBlock1 = new TextBlock
                {
                    Text = "H",
                    Foreground = Brushes.Yellow,
                    Width=50,
                    Height=25,  
                    Background = Brushes.Brown,
                    Tag="s"
                };
               
                path1.Visibility = Visibility.Visible;
                Canvas.SetLeft(path1, startpoint.X);
                Canvas.SetTop(path1, startpoint.Y);

                canvas1.Children.Add(linetool);
              
                mousedowncheck = true;
                rightMouseDown = true;
            }
            else if(circletoolenabled)
            {
                ellipsearea = new Ellipse
                { 
                    Stroke = Brushes.Red,
                    StrokeThickness= 2,
                    Tag="s"
                };
                ellipsearea.MouseLeftButtonDown += OnEllipseMouseLeftButtonDown;
                textBlock1 = new TextBlock
                {

                    Foreground = Brushes.Yellow,
                    Width = 70,
                    Height = 70,
                    Background = Brushes.Brown,
                };
                canvas1.Children.Add(ellipsearea);
                mousedowncheck = true;
                rightMouseDown = true;
            }
            else if(selecttoolenabled)
            {

            }
            if (pixelData is Dicom.Imaging.Render.GrayscalePixelDataU16)
            {
                //    // for (int i = 0; i < pixelData.Width; i++)
                // {
                //  for (int j = 0; j < pixelData.Height; j++)
                //  {
                int rounded_2 = (int)Math.Round(dicominstancepoint.X);
                int rounded_2y = (int)Math.Round(dicominstancepoint.Y);
                if (rounded_2 >= 0 && rounded_2y >= 0 && rounded_2 < 512 && rounded_2y < 512)
                {
                    
                    //Console.WriteLine("{0}", Convert.ToSingle(pixelData.GetPixel(ptWLDown.X, ptWLDown.Y)));
                    double pixelvalueis = (pixelData.GetPixel((int)rounded_2, (int)rounded_2y));
                    lbl_PX.Content = "X: " + rounded_2 + " Y: " + rounded_2y + " PX: " + Convert.ToSingle(pixelvalueis) + "  HU: " + (pixelvalueis * rescaleSlope + rescaleIntercept); ;
                }
                else
                {
                    lbl_PX.Content = "X value: " + " Y value: " + " PX: ";
                }
                // }
                //   }
            }
            rightMouseDown = true;
            Cursor = Cursors.Hand;
       
        }
        public void delete()
        {
            //  canvas1.Children.Clear();
            //var child = (from c in canvas1.Children
            //             where "s".Equals(c.Tag)
            //             select c).First();

            for (int i = canvas1.Children.Count - 1; i >= 0; i += -1)
            {
                UIElement Child = canvas1.Children[i];
                if (Child is Line)
                    canvas1.Children.Remove(Child);
                else if(Child is Rectangle)
                    canvas1.Children.Remove(Child);
                else if(Child is Ellipse)
                    canvas1.Children.Remove(Child);
                else if (Child is TextBlock)
                    canvas1.Children.Remove(Child);
                path1.Visibility = Visibility.Collapsed;
                path2.Visibility = Visibility.Collapsed;
            }
           
          
        }

        [Obsolete]
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
                    var pos = e.GetPosition(canvas1);
                
                    ImageSource imageSource = DicomInstance.Source;
                    BitmapSource bitmapImage = (BitmapSource)imageSource;
                    // var d = bmp.GetPixel(ptWLDown.X, ptWLDown.Y);
                    //  Console.WriteLine("Pixel value is : *****" + d);
                    dicomendepoint = new System.Windows.Point();
                    dicomendepoint.X = (e.GetPosition(DicomInstance).X * bitmapImage.PixelWidth / DicomInstance.ActualWidth);
                    dicomendepoint.Y = (e.GetPosition(DicomInstance).Y * bitmapImage.PixelHeight / DicomInstance.ActualHeight);

                  //  var pos = e.GetPosition(element);

                    if (linetoolenabled)
                    {
                            //var child = (from c in canvas1.Children.OfType<FrameworkElement>()
                            //             where "tempLine".Equals(c.Tag)
                            //             select c).First();
                            linetool.X1 = startpoint.X;
                            linetool.Y1 = startpoint.Y;
                            linetool.X2 = Mouse.GetPosition(canvas1).X;
                            linetool.Y2 = Mouse.GetPosition(canvas1).Y;
                            linetool.Stroke = Brushes.Black;
                            // DrawLine(startpoint, pos);
                            var x = Math.Min(pos.X, startpoint.X);
                            var y = Math.Min(pos.Y, startpoint.Y);
                                                                    
                           double dx = startpoint.X - pos.X;
                           double dy = startpoint.Y - pos.Y;
                           DicomImage dImages;
                            if (isDicomDirFolderPath)
                            {
                                dImages = GetDicomImage(dicomDecoder.GetFilePath(dicomDirectoryRecordobj.LowerLevelDirectoryRecordCollection.ElementAt(Indexr), seriesRecordPathFileName));
                            }
                            else
                            {
                                dImages = GetDicomImage(dicomlist[Indexr]);
                            }
                    
                            //  var geometry = new FrameGeometry(dImages.Dataset);

                            // lets say click1 and click2 are two Point instances and give you the pixels where you want to do the measurement. 
                            // for examle the whole diagonale of a 512x512 ct image: click1 = (0/0) and click2 = (511/511)
                            // Point2 point2;
                
                            //    var patientCoord1 = geometry.TransformImagePointToPatient(new Point2((int)startpoint.X, (int)startpoint.Y));
                            //  var patientCoord2 = geometry.TransformImagePointToPatient(new Point2((int)Mouse.GetPosition(canvas1).X, (int)Mouse.GetPosition(canvas1).Y));
                            // double distanceInMM = patientCoord1.Distance(patientCoord2);
                           double distanceMeasure = ((double)startpoint.X * (double)Mouse.GetPosition(canvas1).X + (double)startpoint.Y * (double)Mouse.GetPosition(canvas1).Y);
                            //if (distanceInMM > 0)
                            //{
                            double distancecalculates = (distanceMeasure * pixelspacingvalue);                                            
                            textBlock1.Text = distancecalculates.ToString();                          
                             
                                lbl_PX.Content = "distanceMeasure: " + distancecalculates;
                           //}
                           // Canvas.SetLeft(linetool, startpoint.X);
                           // Canvas.SetTop(linetool, startpoint.Y);
                            Cursor = Cursors.Hand;
                          //  e.Handled = true;
                    }
                    if (Recttoolenabled)
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
                        textBlock1.Text = "width:" + w + "\n" + "height:" + h + "\n"+"Area:"+w*h;
                        Cursor = Cursors.Hand;
                    }
                    if(circletoolenabled)
                    {
                        double minX = Math.Min(pos.X, startpoint.X);
                        double minY = Math.Min(pos.Y, startpoint.Y);
                        double maxX = Math.Max(pos.X, startpoint.X);
                        double maxY = Math.Max(pos.Y, startpoint.Y);

                        Canvas.SetTop(ellipsearea, minY);
                        Canvas.SetLeft(ellipsearea, minX);

                        double height = maxY - minY;
                        double width = maxX - minX;

                        ellipsearea.Height = Math.Abs(height);
                        ellipsearea.Width = Math.Abs(width);
                        textBlock1.Text = "width:" + ellipsearea.Width + "\n" + "height:" + ellipsearea.Height + "\n" + "Area:" +(3.14* ellipsearea.Width * ellipsearea.Height);
                    
                    }

                    if(WindowingToolEnabled)
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
            if (canvas1.Children.Count > 2)
            {
                canvas1.Children.RemoveAt(canvas1.Children.Count - 1);
            }
            canvas1.Children.Add(link);
        }

        private void Imageborder_MouseUp(object sender, MouseButtonEventArgs e)
        {
        
            if(mousedowncheck == true && rightMouseDown==true)
            {              
               if(linetoolenabled)
                {
                    Canvas.SetLeft(textBlock1, Mouse.GetPosition(canvas1).X);
                    Canvas.SetTop(textBlock1, Mouse.GetPosition(canvas1).Y);
                    Canvas.SetLeft(path2, Mouse.GetPosition(canvas1).X);
                    Canvas.SetTop(path2, Mouse.GetPosition(canvas1).Y);
                    canvas1.Children.Add(textBlock1);
                }
                if(Recttoolenabled)
                {
                    Canvas.SetLeft(textBlock1, Mouse.GetPosition(canvas1).X);
                    Canvas.SetTop(textBlock1, Mouse.GetPosition(canvas1).Y);
                    canvas1.Children.Add(textBlock1);
                }
                if (circletoolenabled)
                {
                    Canvas.SetLeft(textBlock1, Mouse.GetPosition(canvas1).X);
                    Canvas.SetTop(textBlock1, Mouse.GetPosition(canvas1).Y);
                    canvas1.Children.Add(textBlock1);
                }             
                rectSelectArea = null;
                ellipsearea = null; 
                mousedowncheck = false;
                linetool = null;
                rightMouseDown = false;
            }
            
        }

        private void Imageborder_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {             
            if (e.Manipulators.Count() == 1 && !twomanipulate && !threemanipulate &&!fivemanipulate)
            {
                Swindowwidth -= e.DeltaManipulation.Translation.X;
                SwindowCenter -= e.DeltaManipulation.Translation.Y;
                Refresh();
            }
            else if (e.Manipulators.Count()== 2 && !fivemanipulate && !threemanipulate)
            {
                twomanipulate = true;
                Console.WriteLine("2 manipulators entered");
                var transform = DicomInstance.RenderTransform as MatrixTransform;
                if (transform != null)
                {               
                    var matrix = transform.Matrix;
                    // System.Windows.Point center = new System.Windows.Point(centerwith, centerheight);
                    // center = matrix.Transform(center);
                   
                    matrix.ScaleAt(e.DeltaManipulation.Scale.X, e.DeltaManipulation.Scale.X, e.ManipulationOrigin.X, e.ManipulationOrigin.Y);
                    //matrix.RotateAt(e.DeltaManipulation.Rotation, center.X, center.Y);
                    //matrix.Translate(e.DeltaManipulation.Translation.X, e.DeltaManipulation.Translation.Y);
                    DicomInstance.RenderTransform = new MatrixTransform(matrix);
                }

            }
            else if (e.Manipulators.Count() == 3 && !fivemanipulate)
            {
                threemanipulate = true;
                var transform = DicomInstance.RenderTransform as MatrixTransform;
                if (transform != null)
                {
                    var matrix = transform.Matrix;
                    System.Windows.Point center = new System.Windows.Point(DicomInstance.ActualWidth / 2.0, DicomInstance.ActualHeight / 2.0);
                    center = matrix.Transform(center);

                    //matrix.ScaleAt(e.DeltaManipulation.Scale.X, e.DeltaManipulation.Scale.X, center.X, center.Y);
                    //matrix.RotateAt(e.DeltaManipulation.Rotation, center.X, center.Y);
                    matrix.Translate(e.DeltaManipulation.Translation.X, e.DeltaManipulation.Translation.Y);

                    DicomInstance.RenderTransform = new MatrixTransform(matrix);
                }
            }
            else if (e.Manipulators.Count() == 5)
            {
                fivemanipulate = true;
                var transform = DicomInstance.RenderTransform as MatrixTransform;
                if (transform != null)
                {
                    var matrix = transform.Matrix;
                    System.Windows.Point center = new System.Windows.Point(DicomInstance.ActualWidth / 2.0, DicomInstance.ActualHeight / 2.0);
                    center = matrix.Transform(center);
                    Console.WriteLine("degree *************"+e.DeltaManipulation.Rotation);
                    //matrix.ScaleAt(e.DeltaManipulation.Scale.X, e.DeltaManipulation.Scale.X, center.X, center.Y);
                     matrix.RotateAt(e.DeltaManipulation.Rotation, center.X, center.Y);                
                    //matrix.Translate(e.DeltaManipulation.Translation.X, e.DeltaManipulation.Translation.Y);
                    DicomInstance.RenderTransform = new MatrixTransform(matrix);
                }
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
   
    }
}




