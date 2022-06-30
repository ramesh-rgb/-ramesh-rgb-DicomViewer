using System;
using System.Collections.Generic;
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
using Microsoft.Win32;
using System.Runtime.InteropServices;
using Dicom.Imaging;
using Dicom;
using System.IO;
using System.Drawing;
using ImageViewer.Screens;
using Image = System.Windows.Controls.Image;
using Color = System.Windows.Media.Color;
using System.Windows.Forms;
using Dicom.Media;
using System.Diagnostics;
using System.Linq;
namespace ImageViewer
{
    /// <summary>
    /// MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DicomDecoder dd;
        List<byte> pixels8;
        List<ushort> pixels16;
        List<ushort> pixels12;
        List<byte> pixels24; // 30 July 2022
        int imageWidth;
        int imageHeight;
        int bitDepth;
        int samplesPerPixel;  // Updated 30 July 2022
        bool imageOpened;
        double winCentre;
        double winWidth;
        bool signedImage;
        int maxPixelValue;    // Updated July 2022
        int minPixelValue;
        List<SeriesImagePanel> seriesImagePanelslist;
        int layout = 1;
        private SeriesImagePanel selectedImageControl;
        bool flipVertl;
        bool flipHztl;
        public System.ComponentModel.BackgroundWorker backgroundWorker1;
        public LogWriter logWriter;

        public MainWindow()
        {
            InitializeComponent();
            dd = new DicomDecoder();
            pixels8 = new List<byte>();
            pixels16 = new List<ushort>();
            pixels12 = new List<ushort>();
            pixels24 = new List<byte>();
            imageOpened = false;
            signedImage = false;
            maxPixelValue = 0;
            minPixelValue = 65536;
            seriesImagePanelslist = new List<SeriesImagePanel>();
            selectedImageControl = new SeriesImagePanel();
            flipVertl = false;
            flipHztl = false;
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            // backgroundWorker1.DoWork += BackgroundWorker1_DoWork; ;
            logWriter = new LogWriter("Log Created");
        }

        //private void BackgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        //{
        //    var seriesrecord = sender as DicomDataset;
        //    foreach (var imageRecord in seriesrecord.LowerLevelDirectoryRecordCollection)
        //    {
        //        rectanglepanel.Children.Add(DrawRectangle());
        //        Border border = new Border();
        //        border.Width = 70;
        //        border.Height = 70;
        //        border.Margin = new Thickness(5, 5, 5, 5);
        //        border.VerticalAlignment = VerticalAlignment.Stretch;
        //        Image imageThumb = new Image();
        //        imageThumb.Width = 70;
        //        imageThumb.Height = 70;
        //        imageThumb.MouseDown += Image1_MouseDown;
        //        imageThumb.Stretch = Stretch.Uniform;
        //        imageThumb.Tag = seriesImagePanel;
        //        DicomImage dicomimage = new DicomImage(GetFilePath(imageRecord));
        //        //  DicomImage dicomimage = new DicomImage(imageRecord, 0);
        //        if (dicomimage != null)
        //        {
        //            Bitmap bmp = new Bitmap(dicomimage.RenderImage(0).As<Bitmap>());
        //            imageThumb.Source = BitmapToImageSource(bmp);
        //            border.Child = imageThumb;
        //        }
        //        dicomDirectoryHelper.Display(imageRecord);
        //        borderList.Add(border);
        //        seriesImagePanel.AddNewImage(dicomimage);
        //    }
        //    wrapPanel.Children.Add(rectanglepanel);
        //    wrapPanel.Children.Add(borderList[0]);
        //    Imagethumbnailpanel.Children.Add(wrapPanel);
        //}


        private void Exit(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Open(object sender, RoutedEventArgs e)
        {

        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
          
        }
        private void Image1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Image pb = (Image)sender;
            //int indexvalue;
            //int indexrvalue;
            //if (pb != null)
            //{
            //    var contentPresenter = VisualTreeHelper.GetParent(pb) as ContentPresenter;
            //    if (contentPresenter != null)
            //    {
            //        var stackPanel = VisualTreeHelper.GetParent(contentPresenter) as StackPanel;
            //        foreach (var element in stackPanel.Children.OfType<WrapPanel>())
            //        {
            //            var name = element.Name;
            //        }
            //        if (stackPanel != null)
            //            indexvalue = stackPanel.Children.IndexOf(contentPresenter);
            //        indexrvalue = Imagethumbnailpanel.Children.IndexOf(stackPanel);
            //        System.Windows.Controls.ListBox panel = ittem.Header as System.Windows.Controls.ListBox;                  
            //        rec.Fill = new SolidColorBrush(Colors.Red);                 
            //        if (indexr >= 1)
            //        {
            //            var temp = indexr;
            //            temp = temp - 1;
            //            System.Windows.Shapes.Rectangle recs = panel.Items.GetItemAt(temp) as System.Windows.Shapes.Rectangle;
            //            recs.Fill = new SolidColorBrush(Colors.DarkBlue);
            //        }
            //        //Imagethumbnailpanel.Children.GetType().Name
            //    }
            //}

            layout = 1;
            string imagetag = (string)pb.Tag;
            string[] authorsList = imagetag.Split('-');
            int seriesCount = int.Parse(authorsList[0]);
           // seriesCount--;
            int imageNo=int.Parse(authorsList[1]);
            imageNo--;
            SeriesImagePanel seriesImagePanel = new SeriesImagePanel();

            seriesImagePanel= seriesImagePanelslist[seriesCount];
          
            if(seriesImagePanel != null)
            {
                seriesImagePanel.Height = 1070;
                seriesImagePanel.Width = 1520;
                seriesImagePanel.Indexr = imageNo;
                seriesImagePanel.RotateTransforms += 0;
                seriesImagePanel.Refresh();
            }
            //   SS.Zoom = 1;
            MainDicomPanel.Children.Clear();          
            MainDicomPanel.Children.Add(seriesImagePanel);
            selectedImageControl = null;
            selectedImageControl = seriesImagePanel;
        }

        //custom windowing ok button
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            windowingcall(Windowleveltxt.Text.ToString(), windowwidthtxt.Text.ToString());
            MyPopup.IsOpen = false;
        }
        private void windowingcall(string Windowleveltxt, string windowwidthtxt)
        {
            if (selectedImageControl != null && Windowleveltxt != String.Empty && windowwidthtxt != string.Empty)
            {
                selectedImageControl.Winowwidth = double.Parse(windowwidthtxt);
                selectedImageControl.Winowcenter = double.Parse(Windowleveltxt);
                selectedImageControl.Refresh();
            }
        }
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           
        }

        private void layout1_Click(object sender, RoutedEventArgs e)
        {
            if (seriesImagePanelslist.Count > 0)
            {
                layout = 1;
                SeriesImagePanel seriesImagePanel = seriesImagePanelslist[0];
                seriesImagePanel.Height = 960;
                seriesImagePanel.Width = 1600;
                // SS.zoom(2.0);
                MainDicomPanel.Children.Clear();
                MainDicomPanel.Children.Add(seriesImagePanel);
            }
        }


        private void layout12_Click_1(object sender, RoutedEventArgs e)
        {
            if (seriesImagePanelslist.Count > 0 && seriesImagePanelslist.Count != 2)
            {
                layout = 2;
                SeriesImagePanel seriesImagePanel = seriesImagePanelslist[0];
                MainDicomPanel.Children.Clear();
                seriesImagePanel.Height = 960;
                seriesImagePanel.Width = 1600;
                MainDicomPanel.Children.Add(seriesImagePanel);
            }
            else if (seriesImagePanelslist.Count == 2)
            {
                SeriesImagePanel seriesImagePanel = seriesImagePanelslist[0];
                MainDicomPanel.Children.Clear();
                seriesImagePanel.Height = 960;
                seriesImagePanel.Width = 800;
                MainDicomPanel.Children.Add(seriesImagePanel);
                SeriesImagePanel seriesImagePanelOne = seriesImagePanelslist[1];
                seriesImagePanelOne.Height = 960;
                seriesImagePanelOne.Width = 800;
                MainDicomPanel.Children.Add(seriesImagePanelOne);
            }
        }

        private void layout13_Click(object sender, RoutedEventArgs e)
        {
            if (seriesImagePanelslist.Count > 0)
            {
                layout = 3;
                SeriesImagePanel seriesImagePanel = seriesImagePanelslist[0];
                seriesImagePanel.Height = 960;
                seriesImagePanel.Width = 533;
                MainDicomPanel.Children.Clear();
                MainDicomPanel.Children.Add(seriesImagePanel);
                // SS.zoom(0.5);
                if (seriesImagePanelslist.Count == 1)
                {
                    SeriesImagePanel seriesImagePanel1 = seriesImagePanelslist[1];
                    seriesImagePanel1.Height = 960;
                    seriesImagePanel1.Width = 533;
                    MainDicomPanel.Children.Add(seriesImagePanel1);
                }
                if (seriesImagePanelslist.Count == 2)
                {
                    // SS1.zoom(0.5);
                    SeriesImagePanel seriesImagePanel2 = seriesImagePanelslist[2];
                    seriesImagePanel2.Height = 960;
                    seriesImagePanel2.Width = 533;
                    MainDicomPanel.Children.Add(seriesImagePanel2);
                }
                //  SS2.zoom(0.5);
                //    SS.scaleTransform.CenterX = SS.DicomInstance.ActualWidth / 2.0;
                //    SS.scaleTransform.CenterY = SS.DicomInstance.ActualHeight / 2.0;
                //    //TODO use animation
                //    SS.scaleTransform.ScaleX += SS.scaleTransform.ScaleX * 0.6;
                //  SS.scaleTransform.ScaleY = Math.Abs(SS.scaleTransform.ScaleX);

            }
        }

        void mousedownclick(object sender, MouseButtonEventArgs e)
        {
            //SeriesImagePanel ctrl = sender as SeriesImagePanel;
            //if (ctrl != null)
            //    selectedImagecontrol = ctrl;
            //base.OnMouseDown(e);
            //e.Handled = false;
        }

        private void RotateLeft_Click(object sender, RoutedEventArgs e)
        {
            if (selectedImageControl != null)
                selectedImageControl.RotateTransforms -= 90;
        }
        private void RotateRight_Click(object sender, RoutedEventArgs e)
        {
            if (selectedImageControl != null)
                selectedImageControl.RotateTransforms += 90;
        }
        private void Play_but_Click(object sender, RoutedEventArgs e)
        {
            if (selectedImageControl != null)
                selectedImageControl.Sniploop();
        }
        private void Stop_but_Click(object sender, RoutedEventArgs e)
        {
            if (selectedImageControl != null)
                selectedImageControl.stopsniploop();
        }

        private void Zoom_Pan_but_Click(object sender, RoutedEventArgs e)
        {
            if (selectedImageControl != null)
                selectedImageControl.Zoom += 0.5;
        }

        private void ZoomOut(object sender, RoutedEventArgs e)
        {
            if (selectedImageControl != null)
                selectedImageControl.Zoom -= 0.5;
        }

        private void RotateLeftsclick(object sender, RoutedEventArgs e)
        {

        }

        private void CustomWindowingButton_Click(object sender, RoutedEventArgs e)
        {
            if (Presetpopup.IsOpen)
                Presetpopup.IsOpen = false;
            else
            {
                Presetpopup.IsOpen = true;
            }


            if (selectedImageControl.linetoolenable)
                selectedImageControl.linetoolenable = false;
            if (selectedImageControl.circletoolenable)
                selectedImageControl.circletoolenable = false;
            if (selectedImageControl.Recttoolenable)
            {
                selectedImageControl.Recttoolenable = false;
            }
            if(selectedImageControl.windowingtoolenable)
            {
                selectedImageControl.windowingtoolenable = false;
            }
            else
            {
                selectedImageControl.windowingtoolenable = true;
            }

        }

        private void layout14_Click(object sender, RoutedEventArgs e)
        {
            //if(layoutpopup.IsOpen)
            //    layoutpopup.IsOpen = false;

            //layoutpopup.IsOpen = true;
        }

        private void Grid_MouseLeftButtonDown(object sender, RoutedEventArgs e)
        {


        }
        private int gridStartingPoint;
        private int gridEndingPoint;
        private bool gridMouseDown;
        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender != null)
            {
                TextBlock _grid = sender as TextBlock;
                int _row = (int)_grid.GetValue(Grid.RowProperty);
                int _column = (int)_grid.GetValue(Grid.ColumnProperty);
                // MessageBox.Show(string.Format("Grid clicked at column {0}, row {1}", _column, _row));
                gridStartingPoint = _row + _column;
                gridMouseDown = true;
            }
        }

        private void TextBlock_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (sender != null && gridMouseDown)
            {
                TextBlock _grid = sender as TextBlock;
                int _row = (int)_grid.GetValue(Grid.RowProperty);
                int _column = (int)_grid.GetValue(Grid.ColumnProperty);
                gridEndingPoint = _row + _column;
                System.Windows.MessageBox.Show(string.Format("Grid clicked at column {0}, row {1}", gridStartingPoint, gridEndingPoint));
                gridStartingPoint = 0;
                gridEndingPoint = 0;
                gridMouseDown = false;
            }
        }

        private void TextBlock_PreviewMouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (sender != null && gridMouseDown)
            {
                TextBlock _grid = sender as TextBlock;
                _grid.Background = new SolidColorBrush(Colors.Red);
                //int _row = (int)_grid.GetValue(Grid.RowProperty);
                //int _column = (int)_grid.GetValue(Grid.ColumnProperty);
                // MessageBox.Show(string.Format("Grid clicked at column {0}, row {1}", _column, _row));
            }
        }

        private void Grid_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender != null)
            {
                TextBlock _grid = sender as TextBlock;
                int _row = (int)_grid.GetValue(Grid.RowProperty);
                int _column = (int)_grid.GetValue(Grid.ColumnProperty);
                //MessageBox.Show(string.Format("Grid clicked at column {0}, row {1}", _column, _row));
                //gridstartingpoint = _row + _column;
                gridMouseDown = true;
                //layoutpopup.IsOpen = false;
            }
        }

        private void ResetDeltamanipulation_Click(object sender, RoutedEventArgs e)
        {
            if (selectedImageControl != null)
                selectedImageControl.reset();
        }

        private void Rectangle_Click(object sender, RoutedEventArgs e)
        {
            if (selectedImageControl.linetoolenable)
                selectedImageControl.linetoolenable = false;
            if (selectedImageControl.circletoolenable)
                selectedImageControl.circletoolenable = false;
            if (selectedImageControl.Recttoolenable == true)
            {
                selectedImageControl.Recttoolenable = false;
            }
            else
            {
                selectedImageControl.Recttoolenable = true;
            }
        }

        private void Line_Click(object sender, RoutedEventArgs e)
        {
            if (selectedImageControl.Recttoolenable)
                selectedImageControl.Recttoolenable = false;

            if (selectedImageControl.circletoolenable)
                selectedImageControl.circletoolenable = false;

            if (selectedImageControl.linetoolenable == true)
                selectedImageControl.linetoolenable = false;
            else
            {
                selectedImageControl.linetoolenable = true;
            }

        }


        private void ElipseButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedImageControl.Recttoolenable)
                selectedImageControl.Recttoolenable = false;

            if (selectedImageControl.linetoolenable)
                selectedImageControl.linetoolenable = false;

            if (selectedImageControl.circletoolenable == true)
                selectedImageControl.circletoolenable = false;
            else
            {
                selectedImageControl.circletoolenable = true;
            }
        }

        private void selectBut_Click(object sender, RoutedEventArgs e)
        {
            if (selectedImageControl.Recttoolenable)
                selectedImageControl.Recttoolenable = false;

            if (selectedImageControl.linetoolenable)
                selectedImageControl.linetoolenable = false;

            if (selectedImageControl.circletoolenable)
                selectedImageControl.circletoolenable = false;

            selectedImageControl.Selectiontoolenable = true;
        }

        public DicomImage GetDicomImage(string dicomfolder)
        {
            try
            {
                var files=System.IO.Directory.GetFiles(dicomfolder);
                DicomImage dicomimage = new DicomImage(files[0], 0);
                return dicomimage;
            }
            catch (Exception)
            {
                return null;

            }
            // DicomImage dicomimage = new DicomImage(dicomFile.Dataset, 0);

        }
        public DicomImage GetDicomImagefile(string fPath)
        {
            try
            {
            
                DicomImage dicomimage = new DicomImage(fPath, 0);
                return dicomimage;
            }
            catch (Exception)
            {
                return null;

            }
            // DicomImage dicomimage = new DicomImage(dicomFile.Dataset, 0);

        }

   

        [Obsolete]
        private void OpenFolder_Click(object sender, RoutedEventArgs e)
        {
            MainDicomPanel.Children.Clear();
            seriesImagePanelslist.Clear();
           
            Imagethumbnailpanel.Children.Clear();
            logWriter.LogWrite("OpenFolder CLicked");
            try
            {
                using (var fbd = new FolderBrowserDialog())
                {
                    DialogResult result = fbd.ShowDialog();
                    if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                    {
                        DirectoryInfo startDir = new DirectoryInfo(fbd.SelectedPath);
                        RecurseFileStructure recurseFileStructure = new RecurseFileStructure();
                             // var fo=startDir.EnumerateFiles(".dcm",SearchOption.AllDirectories);
                        List<FileInfo> files = recurseFileStructure.TraverseDirectory(startDir);
                        Seriessegregation seriessegregation = new Seriessegregation();
                       seriesImagePanelslist=seriessegregation.GetseriesImagePanels(files);
                        //  string[] files = System.IO.Directory.GetDirectories(fbd.SelectedPath);
                        if (files.Count > 0)
                        {
                            logWriter.LogWrite("Number of Series folder contains:" + files.Count);
                            var dicomFile = Dicom.DicomFile.Open(files.ElementAt(0).FullName);

                            //  DicomImage dicomImageobj = GetDicomImage(files.ch);
                            if (dicomFile != null)
                            {
                                if (dicomFile.Dataset.Contains(DicomTag.PatientName))
                                    this.Patientnamelbl.Content = dicomFile.Dataset.GetSingleValueOrDefault(DicomTag.PatientName, string.Empty);

                                logWriter.LogWrite("Patient Name :" + this.Patientnamelbl.Content);
                                if (dicomFile.Dataset.Contains(DicomTag.StudyDescription))
                                    this.StudyDescriptionlbl.Content = dicomFile.Dataset.GetSingleValueOrDefault(DicomTag.StudyDescription, string.Empty);

                                logWriter.LogWrite("StudyDescription :" + this.StudyDescriptionlbl.Content);

                                if (dicomFile.Dataset.Contains(DicomTag.StudyDate))
                                    this.datelbl.Content = dicomFile.Dataset.GetSingleValueOrDefault(DicomTag.StudyDate, string.Empty);

                                logWriter.LogWrite("StudyDate :" + this.datelbl.Content);

                            }
                            totalserieslbl.Content = seriesImagePanelslist.Count + " series";
                            for (int i = 0; i < seriesImagePanelslist.Count; i++)
                            {
                                // SeriesImagePanel seriesImagePanel = new SeriesImagePanel();
                                // seriesImagePanel.Name = "Series" + i.ToString();
                                logWriter.LogWrite("Series Number :" + seriesImagePanelslist[i].Name.ToString());

                                WrapPanel wrapPanel = new WrapPanel();
                                //  seriesImagePanel.MouseDown += mousedownclick;
                                //  List<string> filesobj = new List<string>();
                                //   var dirss = System.IO.Directory.GetFiles(item.]);//System.IO.Directory.GetDirectories(foldername);
                                //  logWriter.LogWrite("Instance count :" + dirss.Length);
                                if (seriesImagePanelslist[i].DicomList.Count > 0)
                                {
                                    int seriesNum = int.Parse(seriesImagePanelslist[i].Name);
                                    //     seriesImagePanel.AddImageList(filesobj);
                                    //   seriesImagePanelslist.Add(seriesImagePanel);
                                    WrapPanel stackPanel = new WrapPanel();
                                    stackPanel.Margin = new Thickness(0, 10, 0, 20);
                                    System.Windows.Controls.Label seriesdes = new System.Windows.Controls.Label();
                                    seriesdes.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                                    seriesdes.Foreground = System.Windows.Media.Brushes.DeepSkyBlue;
                                    seriesdes.FontFamily = new System.Windows.Media.FontFamily("SF UI Display");
                                    seriesdes.FontSize = 12;
                                    seriesdes.Margin = new System.Windows.Thickness(0, 0, 0, 0);
                                    seriesdes.Content = "Series Description";
                                    seriesdes.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                                    stackPanel.Children.Add(seriesdes);
                                    System.Windows.Controls.Label seriesdes1 = new System.Windows.Controls.Label();
                                    seriesdes1.Margin = new System.Windows.Thickness(76, 1, 10, 0);
                                    seriesdes1.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                                    seriesdes1.Foreground = System.Windows.Media.Brushes.DeepSkyBlue;
                                    seriesdes1.FontFamily = new System.Windows.Media.FontFamily("SF UI Display");
                                    seriesdes1.FontSize = 12;
                                    seriesdes1.Content = seriesImagePanelslist[i].DicomList.Count + "imgs";

                                    seriesdes1.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                                    stackPanel.Children.Add(seriesdes1);
                                    WrapPanel rectanglepanel = new WrapPanel();
                                    rectanglepanel.Margin = new Thickness(0, 0, 1, 5);
                                    foreach (var items in seriesImagePanelslist[i].DicomList)
                                    {
                                        rectanglepanel.Orientation = System.Windows.Controls.Orientation.Horizontal;
                                        rectanglepanel.Margin = new Thickness(5, 0, 1, 5);
                                        rectanglepanel.Width = 280;
                                        rectanglepanel.Height = Double.NaN;
                                        rectanglepanel.Children.Add(DrawRectangle());
                                    }
                                    stackPanel.Children.Add(rectanglepanel);

                                    if (seriesImagePanelslist[i].DicomList.Count > 3)
                                    {
                                        int middle = seriesImagePanelslist[i].DicomList.Count / 2;
                                        //  stackPanel = new WrapPanel();
                                        stackPanel.Orientation = System.Windows.Controls.Orientation.Horizontal;
                                        stackPanel.Children.Add(GetImage(seriesImagePanelslist[i].DicomList.First(), i, 1));
                                        System.Windows.Shapes.Rectangle recs = rectanglepanel.Children[0] as System.Windows.Shapes.Rectangle;
                                        recs.Fill = new SolidColorBrush(Colors.DarkBlue);
                                        stackPanel.Children.Add(GetImage(seriesImagePanelslist[i].DicomList.ElementAt(middle), i, middle));
                                        System.Windows.Shapes.Rectangle recs1 = rectanglepanel.Children[middle] as System.Windows.Shapes.Rectangle;
                                        recs1.Fill = new SolidColorBrush(Colors.DarkBlue);
                                        stackPanel.Children.Add(GetImage(seriesImagePanelslist[i].DicomList.Last(), i, seriesImagePanelslist[i].DicomList.Count));
                                        int reducedcount = seriesImagePanelslist[i].DicomList.Count;
                                        reducedcount--;
                                        System.Windows.Shapes.Rectangle recs2 = rectanglepanel.Children[reducedcount] as System.Windows.Shapes.Rectangle;
                                        recs2.Fill = new SolidColorBrush(Colors.DarkBlue);
                                        DicomImage dicomImage = seriesImagePanelslist[i].GetDicomImage(seriesImagePanelslist[i].DicomList.First());
                                        if (dicomImage != null)
                                        {
                                            seriesdes.Content = dicomImage.Dataset.Get<string>(DicomTag.SeriesDescription).ToString();
                                        }
                                    }
                                    else
                                    {
                                        if (seriesImagePanelslist[i].DicomList.Count == 3)
                                        {
                                            stackPanel.Orientation = System.Windows.Controls.Orientation.Horizontal;
                                            stackPanel.Children.Add(GetImage(seriesImagePanelslist[i].DicomList.First(), i, 1));
                                            System.Windows.Shapes.Rectangle recs = rectanglepanel.Children[0] as System.Windows.Shapes.Rectangle;
                                            recs.Fill = new SolidColorBrush(Colors.DarkBlue);
                                            stackPanel.Children.Add(GetImage(seriesImagePanelslist[i].DicomList.ElementAt(1), i, 2));
                                            System.Windows.Shapes.Rectangle recs1 = rectanglepanel.Children[1] as System.Windows.Shapes.Rectangle;
                                            recs1.Fill = new SolidColorBrush(Colors.DarkBlue);
                                            stackPanel.Children.Add(GetImage(seriesImagePanelslist[i].DicomList.First(), i, 3));
                                            System.Windows.Shapes.Rectangle recs2 = rectanglepanel.Children[2] as System.Windows.Shapes.Rectangle;
                                            recs2.Fill = new SolidColorBrush(Colors.DarkBlue);
                                            DicomImage dicomImage = seriesImagePanelslist[i].GetDicomImage(seriesImagePanelslist[i].DicomList.First());
                                            if (dicomImage != null)
                                            {
                                                seriesdes.Content = dicomImage.Dataset.Get<string>(DicomTag.SeriesDescription).ToString();
                                            }
                                        }
                                        else if (seriesImagePanelslist[i].DicomList.Count == 2)
                                        {
                                            // stackPanel = new WrapPanel();
                                            stackPanel.Orientation = System.Windows.Controls.Orientation.Horizontal;
                                            stackPanel.Children.Add(GetImage(seriesImagePanelslist[i].DicomList.First(), i, 1));
                                            System.Windows.Shapes.Rectangle recs1 = rectanglepanel.Children[0] as System.Windows.Shapes.Rectangle;
                                            recs1.Fill = new SolidColorBrush(Colors.DarkBlue);
                                            stackPanel.Children.Add(GetImage(seriesImagePanelslist[i].DicomList.Last(), i, 2));
                                            System.Windows.Shapes.Rectangle recs2 = rectanglepanel.Children[1] as System.Windows.Shapes.Rectangle;
                                            recs2.Fill = new SolidColorBrush(Colors.DarkBlue);
                                            DicomImage dicomImage = seriesImagePanelslist[i].GetDicomImage(seriesImagePanelslist[i].DicomList.First());
                                            if (dicomImage != null)
                                            {
                                                seriesdes.Content = dicomImage.Dataset.Get<string>(DicomTag.SeriesDescription).ToString();
                                            }
                                        }
                                        else
                                        {   //stackPanel = new WrapPanel();
                                            stackPanel.Orientation = System.Windows.Controls.Orientation.Horizontal;
                                            stackPanel.Children.Add(GetImage(seriesImagePanelslist[i].DicomList.First(), i, 1));
                                            System.Windows.Shapes.Rectangle recs1 = rectanglepanel.Children[0] as System.Windows.Shapes.Rectangle;
                                            recs1.Fill = new SolidColorBrush(Colors.DarkBlue);
                                            DicomImage dicomImage = seriesImagePanelslist[i].GetDicomImage(seriesImagePanelslist[i].DicomList.First());
                                            if (dicomImage != null)
                                            {
                                                seriesdes.Content = dicomImage.Dataset.Get<string>(DicomTag.SeriesDescription).ToString();
                                            }



                                        }
                                    }

                                    //    if (layout == 1)
                                    //    {

                                    //        SS.scaleTransform.CenterX = SS.DicomInstance.ActualWidth / 2.0;
                                    //        SS.scaleTransform.CenterY = SS.DicomInstance.ActualHeight / 2.0;
                                    //        SS.zoom();
                                    //        //TODO use animation
                                    //        SS.scaleTransform.ScaleX += SS.scaleTransform.ScaleX * 0.6;
                                    //        SS.scaleTransform.ScaleY = Math.Abs(SS.scaleTransform.ScaleX);
                                    //    }
                                    //    else
                                    //    {

                                    //        var dirssobj = System.IO.Directory.GetFiles(fbd.SelectedPath);//System.IO.Directory.GetDirectories(foldername);
                                    //        if (dirssobj.Length > 0)
                                    //        {

                                    //            List<string> filesobj1 = new List<string>();
                                    //            filesobj1.AddRange(dirssobj);

                                    //            WrapPanel stackPanel = new WrapPanel();
                                    //            stackPanel.Margin = new Thickness(0, 10, 0, 20);
                                    //            System.Windows.Controls.Label seriesdes = new System.Windows.Controls.Label();
                                    //            seriesdes.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                                    //            seriesdes.Foreground = System.Windows.Media.Brushes.DeepSkyBlue;
                                    //            seriesdes.FontFamily = new System.Windows.Media.FontFamily("SF UI Display");
                                    //            seriesdes.FontSize = 12;
                                    //            seriesdes.Margin = new System.Windows.Thickness(0, 0, 0, 0);
                                    //            seriesdes.Content = "Series Description";
                                    //            seriesdes.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                                    //            stackPanel.Children.Add(seriesdes);
                                    //            System.Windows.Controls.Label seriesdes1 = new System.Windows.Controls.Label();
                                    //            seriesdes1.Margin = new System.Windows.Thickness(220, 1, 10, 0);
                                    //            seriesdes1.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                                    //            seriesdes1.Foreground = System.Windows.Media.Brushes.DeepSkyBlue;
                                    //            seriesdes1.FontFamily = new System.Windows.Media.FontFamily("SF UI Display");
                                    //            seriesdes1.FontSize = 12;
                                    //            seriesdes1.Content = filesobj1.Count.ToString() + "imgs";

                                    //            seriesdes1.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                                    //            stackPanel.Children.Add(seriesdes1);
                                    //            DicomImage dicomImageobj = GetDicomImagefile(filesobj1[0]);
                                    //            if (dicomImageobj != null)
                                    //            {
                                    //                this.Patientnamelbl.Content = dicomImageobj.Dataset.Get<string>(DicomTag.PatientName).ToString();
                                    //                this.StudyDescriptionlbl.Content = dicomImageobj.Dataset.Get<string>(DicomTag.StudyDescription).ToString();
                                    //                this.datelbl.Content = dicomImageobj.Dataset.Get<string>(DicomTag.StudyDate).ToString();
                                    //            }

                                    //            foreach (var item in filesobj1)
                                    //            {
                                    //                SeriesImagePanel seriesImagePanel = new SeriesImagePanel();
                                    //                seriesImagePanel.MouseDown += mousedownclick;
                                    //                List<string> list = new List<string>();
                                    //                list.Add(item);
                                    //                seriesImagePanel.AddImageList(list);
                                    //                seriesImagePanelslist.Add(seriesImagePanel);
                                    //                stackPanel.Orientation = System.Windows.Controls.Orientation.Horizontal;
                                    //                stackPanel.Children.Add(GetImage(item, 1, 1));
                                    //                DicomImage dicomImage = seriesImagePanel.GetDicomImage(item);
                                    //                if (dicomImage != null)
                                    //                {
                                    //                    seriesdes.Content = dicomImage.Dataset.Get<string>(DicomTag.StudyDescription).ToString();
                                    //                }
                                    //            }
                                    //            Imagethumbnailpanel.Children.Add(stackPanel);
                                    //        }
                                    //    }

                                    Imagethumbnailpanel.Children.Add(stackPanel);
                                }

                                //if (seriesImagePanelslist.Count > 0)
                                //{
                                //    SeriesImagePanel SS = new SeriesImagePanel();
                                //    SS = seriesImagePanelslist[0];
                                //    SS.Height = 1070;
                                //    SS.Width = 1520;
                                //    SS.Zoom = 1;
                                //    MainDicomPanel.Children.Add(SS);
                                //    selectedImageControl = SS;
                                //}
                            }
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                logWriter.LogWrite("Exceptio On OpenFolder_Click:" + ex.Message);
            }         
        }

        public Border GetBorder(DicomImage dicomimage, SeriesImagePanel seriesImagePanel)
        {
            Border border = new Border();
            border.Width = 80;
            border.Height = 80;
            border.Margin = new Thickness(5, 5, 5, 5);
            border.VerticalAlignment = VerticalAlignment.Stretch;
            Image image1 = new Image();
            image1.Width = 80;
            image1.Height = 80;
            image1.MouseDown += Image1_MouseDown;
            image1.Stretch = Stretch.Uniform;
            image1.Tag = seriesImagePanel;
            try
            {
               
                  
                    if (dicomimage != null)
                    {
                        //  image1.Tag = dicomimage;
                        Bitmap bmp = new Bitmap(dicomimage.RenderImage(0).As<Bitmap>());
                        image1.Source = BitmapToImageSource(bmp);
                        border.Child = image1;
                    }
                    return border;
  

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

            }

            return border;


        }
        public Border GetImage(string filename,int seriescount,int imageindex)
        {
            Border border = new Border();
            border.Width = 80;
            border.Height = 80;
            border.Margin = new Thickness(5, 5, 5, 5);
            border.VerticalAlignment = VerticalAlignment.Stretch;
            Image image1 = new Image();
            image1.Width = 80;
            image1.Height = 80;
            image1.MouseDown += Image1_MouseDown;
            image1.Stretch = Stretch.Uniform;
            int indexReduced = imageindex--;
            image1.Tag = seriescount+"-"+ indexReduced;
            try
            {
                if (filename.EndsWith("dcm"))
                {
                    DicomImage dicomimage = new DicomImage(filename, 0);
                    if (dicomimage != null)
                    {
                        //  image1.Tag = dicomimage;
                        Bitmap bmp = new Bitmap(dicomimage.RenderImage(0).As<Bitmap>());
                        image1.Source = BitmapToImageSource(bmp);
                        border.Child = image1;
                    }
                    return border;
                }
                else
                {
                    DicomImage dicomimage = new DicomImage(filename, 0);
                    if (dicomimage != null)
                    {
                        //  image1.Tag = dicomimage;
                        Bitmap bmp = new Bitmap(dicomimage.RenderImage(0).As<Bitmap>());
                        image1.Source = BitmapToImageSource(bmp);
                        border.Child = image1;
                    }
                    return border;
                }



            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

            }

            return border;


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
        private System.Windows.Shapes.Rectangle DrawRectangle()
        {
        //    string values1 = dicomDataSet.Get<string>(DicomTag.ReferencedFileID);
            System.Windows.Shapes.Rectangle exampleRectangle = new System.Windows.Shapes.Rectangle();
            exampleRectangle.Width = 10;
            exampleRectangle.Height = 10;
        //    exampleRectangle.Tag = values1;    
            // Create a SolidColorBrush and use it to
            // paint the rectangle.
            //   exampleRectangle.Stroke = System.Windows.Media.Brushes.Black;
            exampleRectangle.Stroke = (SolidColorBrush)new BrushConverter().ConvertFrom("#FF292525");
            exampleRectangle.StrokeThickness = 1;
            SolidColorBrush myBrush =(SolidColorBrush)new BrushConverter().ConvertFrom("#a6a6a6");
           // SolidColorBrush myBrush = new SolidColorBrush(Colors.AntiqueWhite);
            exampleRectangle.Fill = myBrush;
            return exampleRectangle;
        }

        //public void AddImagetoThumbnail(string fileName)
        //    {
        //    DicomImage image = new DicomImage(fileName);
        //    if (image != null)
        //    {

        //        Image image1 = new Image();
        //        image1.Width = 70;
        //        image1.Height = 70;
        //        image1.MouseDown += Image1_MouseDown;
        //        image1.Stretch = Stretch.Uniform;
        //        image1.Tag = seriesImagePanel;
        //        Bitmap bmp = new Bitmap(image.RenderImage(0).As<Bitmap>());
        //        image1.Source = BitmapToImageSource(bmp);
        //        Imagethumbnailpanel.Children.Add(image1);
        //    }

        //}
        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            MainDicomPanel.Children.Clear();
            seriesImagePanelslist.Clear();
            totalserieslbl.Content = "";
            Imagethumbnailpanel.Children.Clear();
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "Dicom Files|*.dcm";
            openFileDialog.InitialDirectory = Environment.CurrentDirectory;
            if (openFileDialog.ShowDialog() == true)
            {            
                    SeriesImagePanel seriesImagePanel = new SeriesImagePanel();
                    seriesImagePanel.Name = "Image";
                    seriesImagePanel.MouseDown += mousedownclick;
                    List<string> filesobj = new List<string>();
                //  var dirss = System.IO.Directory.GetFiles();//System.IO.Directory.GetDirectories(foldername);
                    filesobj.Add(openFileDialog.FileName);
                //  DicomImage dicomImage1= seriesImagePanel.GetDicomImage(openFileDialog.FileName);
                // seriesImagePanel.AddNewImage(dicomImage1);
                  seriesImagePanel.AddImageList(filesobj);
                    seriesImagePanelslist.Add(seriesImagePanel);
                    WrapPanel stackPanel = new WrapPanel();
                    stackPanel.Margin = new Thickness(0, 10, 0, 20);                             
                    
                    WrapPanel rectanglepanel = new WrapPanel();
                //  rectanglepanel.Margin = new Thickness(0, 0, 1, 5);
                    rectanglepanel.Orientation = System.Windows.Controls.Orientation.Horizontal;
                    rectanglepanel.Margin = new Thickness(5, 0, 1, 5);
                    rectanglepanel.Width = 280;
                    rectanglepanel.Height = Double.NaN;
                    rectanglepanel.Children.Add(DrawRectangle());
                
                   stackPanel.Children.Add(rectanglepanel);

                   Border border = new Border();
                    border.BorderBrush = System.Windows.Media.Brushes.WhiteSmoke;
                    border.BorderThickness = new Thickness(1);
                    border.Margin = new Thickness(0, 20, 0, 3);
                    Image image1 = new Image();
                    image1.Width = 70;
                    image1.Height = 70;
                    image1.VerticalAlignment = VerticalAlignment.Top;
                    image1.MouseDown += Image1_MouseDown;
                    image1.Tag = 0 + "-" + 1;
                DicomImage dicomImage = seriesImagePanel.GetDicomImage(filesobj[0]);
                    if (dicomImage != null)
                    {
                    this.Patientnamelbl.Content = dicomImage.Dataset.Get<string>(DicomTag.PatientName).ToString();
                    if (dicomImage.Dataset.Contains(DicomTag.StudyDate))
                        this.datelbl.Content = dicomImage.Dataset.Get<string>(DicomTag.StudyDate).ToString();
                    if (dicomImage.Dataset.Contains(DicomTag.StudyDescription))
                         this.StudyDescriptionlbl.Content= dicomImage.Dataset.Get<string>(DicomTag.StudyDescription).ToString();
                      Bitmap bmp = new Bitmap(dicomImage.RenderImage(0).As<Bitmap>());
                        image1.Source = seriesImagePanel.BitmapToImageSource(bmp);
                        border.Child = image1;
                        stackPanel.Children.Add(border);                      
                    }   
                    Imagethumbnailpanel.Children.Add(stackPanel);
            }
            //if (layout == 1)
            //{
            //if (seriesImagePanelslist.Count > 0)
            //{
            //    SeriesImagePanel seriesImagePanel = new SeriesImagePanel();
            //    seriesImagePanel = seriesImagePanelslist[0];
            //    seriesImagePanel.Height = 1070;
            //    seriesImagePanel.Width = 1520;
            //    // SS.Zoom = 1;                                         
            //    MainDicomPanel.Children.Add(seriesImagePanel);
            //    selectedImageControl = seriesImagePanel;
            //}
            //    SS.scaleTransform.CenterX = SS.DicomInstance.ActualWidth / 2.0;
            //    SS.scaleTransform.CenterY = SS.DicomInstance.ActualHeight
            //
            //    / 2.0;
            //    SS.zoom();
            //    //TODO use animation
            //    SS.scaleTransform.ScaleX += SS.scaleTransform.ScaleX * 0.6;
            //    SS.scaleTransform.ScaleY = Math.Abs(SS.scaleTransform.ScaleX);           
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            selectedImageControl.delete();
        }

        private void Layout_Click(object sender, RoutedEventArgs e)
        {

        }

        private void FlipVertl_Click(object sender, RoutedEventArgs e)
        {

            if (selectedImageControl != null)         
            {
                var val = selectedImageControl.Zoom;
                if (flipVertl==false)
                {
                  
                    selectedImageControl.FlipVTransform(-val);
                    flipVertl = true;
                }
                else
                {

                    selectedImageControl.FlipVTransform(val);
                    flipVertl = false;
                }               
            }            
        }

        private void FlipHztl_Click(object sender, RoutedEventArgs e)
        {
            if (selectedImageControl != null)
            {
                var val = selectedImageControl.Zoom;
                if (flipHztl==false)
                {
                    selectedImageControl.FlipHTransform(-val);
                    flipHztl = true;
                }
                 else
                {
                    selectedImageControl.FlipHTransform(val);
                    flipHztl = false;
                }           
            }
               
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }
        private static void LogToDebugConsole(string informationToLog)
        {
            Debug.WriteLine(informationToLog);
        }
        public string filename = "";
        private void OpenDicomDir_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MainDicomPanel.Children.Clear();
                seriesImagePanelslist.Clear();
                totalserieslbl.Content = "";
                Imagethumbnailpanel.Children.Clear();

                Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
                openFileDialog.FileName = "DICOMDIR";
                openFileDialog.Filter = "Dicom Dir|*.*";
                openFileDialog.InitialDirectory = Environment.CurrentDirectory;
                if (openFileDialog.ShowDialog() == true)
                {
                    filename=openFileDialog.FileName;   
                     var dicomDirectory = DicomDirectory.Open(openFileDialog.FileName);

                    var dicomDirectoryHelper = new DicomDirectoryHelper(LogToDebugConsole);

                    dicomDirectoryHelper.ShowDicomDirectoryMetaInformation(dicomDirectory);

                    foreach (var patientRecord in dicomDirectory.RootDirectoryRecordCollection)
                    {
                      
                        this.Patientnamelbl.Content = patientRecord.Get<string>(DicomTag.PatientName).ToString();                       
                        foreach (var studyRecord in patientRecord.LowerLevelDirectoryRecordCollection)
                        {
                            this.StudyDescriptionlbl.Content = "Series Description";
                            this.datelbl.Content = studyRecord.Get<string>(DicomTag.StudyDate).ToString();
                           
                            int seriescount = 0;
                            foreach (var seriesRecord in studyRecord.LowerLevelDirectoryRecordCollection)
                            {
                                seriescount++;
                                totalserieslbl.Content = "series" + seriescount;
                           
                                SeriesImagePanel seriesImagePanel = new SeriesImagePanel();                                              
                                seriesImagePanel.MouseDown += mousedownclick;
                                seriesImagePanel.SeriesRecordPathFileName = filename;
                                seriesImagePanel.AddSeriesRecord(seriesRecord);
                                seriesImagePanelslist.Add(seriesImagePanel);
                                var imagecnt = seriesRecord.LowerLevelDirectoryRecordCollection.Count();
                                WrapPanel wrapPanel = new WrapPanel();
                                wrapPanel.Margin = new Thickness(0, 10, 0, 20);
                                System.Windows.Controls.Label seriesdes = new System.Windows.Controls.Label();
                                seriesdes.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                                seriesdes.Foreground = System.Windows.Media.Brushes.DeepSkyBlue;
                                seriesdes.FontFamily = new System.Windows.Media.FontFamily("SF UI Display");
                                seriesdes.FontSize = 12;
                                seriesdes.Margin = new System.Windows.Thickness(0, 0, 0, 0);
                                DicomDataset dicomDataSet = seriesRecord as DicomDataset;
                                seriesdes.Content = "Series Number:"+seriescount;
                                seriesdes.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                                wrapPanel.Children.Add(seriesdes);
                                System.Windows.Controls.Label instancecount = new System.Windows.Controls.Label();
                                instancecount.Margin = new System.Windows.Thickness(86, 1, 10, 0);
                                instancecount.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                                instancecount.Foreground = System.Windows.Media.Brushes.DeepSkyBlue;
                                instancecount.FontFamily = new System.Windows.Media.FontFamily("SF UI Display");
                                instancecount.FontSize = 12;
                                instancecount.Content = imagecnt+"Imgs";
                                instancecount.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                                wrapPanel.Children.Add(instancecount);
                                wrapPanel.Tag = seriescount;    
                                WrapPanel rectanglepanel = new WrapPanel();
                                rectanglepanel.Orientation = System.Windows.Controls.Orientation.Horizontal;
                                rectanglepanel.Margin = new Thickness(5, 0, 1, 5);
                                rectanglepanel.Width = 280;
                                rectanglepanel.Height = Double.NaN;
                                rectanglepanel.Tag = seriescount;
                                List<Border> borderList = new List<Border>();
                               
                                foreach (var item in seriesRecord.LowerLevelDirectoryRecordCollection)
                                {
                                    rectanglepanel.Orientation = System.Windows.Controls.Orientation.Horizontal;
                                    rectanglepanel.Margin = new Thickness(5, 0, 1, 5);
                                    rectanglepanel.Width = 280;
                                    rectanglepanel.Height = Double.NaN;
                                    rectanglepanel.Children.Add(DrawRectangle());
                                }

                                wrapPanel.Children.Add(rectanglepanel);
                                //for (int i = 0; i < imagecnt; i++)
                                //{
                                //    rectanglepanel.Orientation = System.Windows.Controls.Orientation.Horizontal;
                                //    rectanglepanel.Margin = new Thickness(5, 0, 1, 5);
                                //    rectanglepanel.Width = 280;
                                //    rectanglepanel.Height = Double.NaN;
                                //    rectanglepanel.Children.Add(DrawRectangle());
                                //}
                                if (imagecnt>3)
                                {
                                    int middle = imagecnt / 2;
                                    wrapPanel.Orientation = System.Windows.Controls.Orientation.Horizontal;
                                    wrapPanel.Children.Add(GetImage(dicomDirectoryHelper.GetFilePath(seriesRecord.LowerLevelDirectoryRecordCollection.First(), filename),seriescount,1));
                                    System.Windows.Shapes.Rectangle recs = rectanglepanel.Children[0] as System.Windows.Shapes.Rectangle;
                                    recs.Fill = new SolidColorBrush(Colors.DarkBlue);
                                    wrapPanel.Children.Add(GetImage(dicomDirectoryHelper.GetFilePath(seriesRecord.LowerLevelDirectoryRecordCollection.ElementAt(middle), filename), seriescount, middle));
                                    System.Windows.Shapes.Rectangle recs2 = rectanglepanel.Children[middle] as System.Windows.Shapes.Rectangle;
                                    recs2.Fill = new SolidColorBrush(Colors.DarkBlue);
                                    wrapPanel.Children.Add(GetImage(dicomDirectoryHelper.GetFilePath(seriesRecord.LowerLevelDirectoryRecordCollection.Last(), filename), seriescount,imagecnt));
                                    int reducedcount = imagecnt;
                                    reducedcount--;
                                    System.Windows.Shapes.Rectangle recs3 = rectanglepanel.Children[reducedcount] as System.Windows.Shapes.Rectangle;
                                    recs3.Fill = new SolidColorBrush(Colors.DarkBlue);
                                    DicomImage dicomImage = seriesImagePanel.GetDicomImage(dicomDirectoryHelper.GetFilePath(seriesRecord.LowerLevelDirectoryRecordCollection.Last(), filename));
                                    if (dicomImage != null)
                                    {
                                        // seriesdes.Content = dicomImage.Dataset.Get<string>(DicomTag.SeriesDescription).ToString();
                                    }
                                }
                                else
                                {
                                    if (imagecnt== 3)
                                    {
                                        wrapPanel.Orientation = System.Windows.Controls.Orientation.Horizontal;
                                        wrapPanel.Children.Add(GetImage(dicomDirectoryHelper.GetFilePath(seriesRecord.LowerLevelDirectoryRecordCollection.First(),filename),seriescount,1));
                                        System.Windows.Shapes.Rectangle recs = rectanglepanel.Children[0] as System.Windows.Shapes.Rectangle;
                                        recs.Fill = new SolidColorBrush(Colors.DarkBlue);
                                        wrapPanel.Children.Add(GetImage(dicomDirectoryHelper.GetFilePath(seriesRecord.LowerLevelDirectoryRecordCollection.ElementAt(2), filename), seriescount,2));
                                        System.Windows.Shapes.Rectangle recs2 = rectanglepanel.Children[1] as System.Windows.Shapes.Rectangle;
                                        recs2.Fill = new SolidColorBrush(Colors.DarkBlue);
                                        wrapPanel.Children.Add(GetImage(dicomDirectoryHelper.GetFilePath(seriesRecord.LowerLevelDirectoryRecordCollection.Last(),filename ), seriescount,imagecnt));
                                        System.Windows.Shapes.Rectangle recs3 = rectanglepanel.Children[2] as System.Windows.Shapes.Rectangle;
                                        recs3.Fill = new SolidColorBrush(Colors.DarkBlue);
                                        DicomImage dicomImage = seriesImagePanel.GetDicomImage(dicomDirectoryHelper.GetFilePath(seriesRecord.LowerLevelDirectoryRecordCollection.Last(), filename));
                                        if (dicomImage != null)
                                        {
                                            //seriesdes.Content = dicomImage.Dataset.Get<string>(DicomTag.SeriesDescription).ToString();
                                        }
                                    }
                                    else if (imagecnt == 2)
                                    {
                                        wrapPanel.Orientation = System.Windows.Controls.Orientation.Horizontal;                                
                                        wrapPanel.Children.Add(GetImage(dicomDirectoryHelper.GetFilePath(seriesRecord.LowerLevelDirectoryRecordCollection.First(), filename), seriescount,1));
                                        System.Windows.Shapes.Rectangle recs = rectanglepanel.Children[0] as System.Windows.Shapes.Rectangle;
                                        recs.Fill = new SolidColorBrush(Colors.DarkBlue);
                                        wrapPanel.Children.Add(GetImage(dicomDirectoryHelper.GetFilePath(seriesRecord.LowerLevelDirectoryRecordCollection.Last(), filename), seriescount,2));                                      
                                        DicomImage dicomImage = seriesImagePanel.GetDicomImage(dicomDirectoryHelper.GetFilePath(seriesRecord.LowerLevelDirectoryRecordCollection.Last(), filename));
                                        if (dicomImage != null)
                                        {
                                           // seriesdes.Content = dicomImage.Dataset.Get<string>(DicomTag.SeriesDescription).ToString();
                                        }
                                    }
                                    else
                                    {   // stackPanel = new StackPanel();
                                        wrapPanel.Orientation = System.Windows.Controls.Orientation.Horizontal;
                                        wrapPanel.Children.Add(GetImage(dicomDirectoryHelper.GetFilePath(seriesRecord.LowerLevelDirectoryRecordCollection.First(), filename), seriescount,1));
                                        System.Windows.Shapes.Rectangle recs = rectanglepanel.Children[0] as System.Windows.Shapes.Rectangle;
                                        recs.Fill = new SolidColorBrush(Colors.DarkBlue);
                                        DicomImage dicomImage = seriesImagePanel.GetDicomImage(dicomDirectoryHelper.GetFilePath(seriesRecord.LowerLevelDirectoryRecordCollection.Last(), filename));
                                        if (dicomImage != null)
                                        {
                                            //seriesdes.Content = dicomImage.Dataset.Get<string>(DicomTag.SeriesDescription).ToString();
                                        }
                                    }
                                }
                                Imagethumbnailpanel.Children.Add(wrapPanel);
                          
                            }

                        }
                    }

                }
                LogToDebugConsole("Dicom directory dump operation was successful");
            }
            catch (Exception ex)
            {
                LogToDebugConsole($"Error occured during Dicom directory dump. Error:{ex.Message}");
            }

        }

     

        private WrapPanel ShowSeriesLevelInfo(DicomDataset dataset)
        {

            WrapPanel stackPanel = new WrapPanel();
            stackPanel.Margin = new Thickness(0, 10, 0, 20);
            System.Windows.Controls.Label seriesdes = new System.Windows.Controls.Label();
            seriesdes.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            seriesdes.Foreground = System.Windows.Media.Brushes.DeepSkyBlue;
            seriesdes.FontFamily = new System.Windows.Media.FontFamily("SF UI Display");
            seriesdes.FontSize = 12;
            seriesdes.Margin = new System.Windows.Thickness(0, 0, 0, 0);
            seriesdes.Content  = dataset.Get<int>(DicomTag.SeriesNumber);
            seriesdes.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            stackPanel.Children.Add(seriesdes);
            System.Windows.Controls.Label seriesdes1 = new System.Windows.Controls.Label();
            seriesdes1.Margin = new System.Windows.Thickness(76, 1, 10, 0);
            seriesdes1.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            seriesdes1.Foreground = System.Windows.Media.Brushes.DeepSkyBlue;
            seriesdes1.FontFamily = new System.Windows.Media.FontFamily("SF UI Display");
            seriesdes1.FontSize = 12;
       //     seriesdes1.Content = dataset.Get<string>(DicomTag.NumberOfFrames);
            seriesdes1.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
            stackPanel.Children.Add(seriesdes1);
          return stackPanel;    
        }

        private void BoneButton_Click(object sender, RoutedEventArgs e)
        {
            windowingcall("300", "2500");
        }

        private void LungButton_Click(object sender, RoutedEventArgs e)
        {
            windowingcall("-400", "1600");
        }

        private void SpineButton_Click(object sender, RoutedEventArgs e)
        {
            windowingcall("20", "300");
        }

        private void AbdomenButton_Click(object sender, RoutedEventArgs e)
        {
            windowingcall("10", "400");
        }
    }
}
