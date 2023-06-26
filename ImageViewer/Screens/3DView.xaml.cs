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
using System.Windows.Shapes;
using System.Windows.Forms;
using Kitware.VTK;
using System.Drawing;

namespace ImageViewer.Screens
{
    /// <summary>
    /// Interaction logic for _3DView.xaml
    /// </summary>
    public partial class _3DView : Window
    {
     //   public RenderWindowControl renderWindowControl = new RenderWindowControl();
        vtkRenderWindow renderWindow3D;
      //  vtkRenderer renderer3D;
     //   vtkDICOMImageReader reader3D;
        System.Windows.Forms.Panel panel1 = new System.Windows.Forms.Panel();
        vtkImageViewer2 _ImageViewer;
        vtkTextMapper _SliceStatusMapper;
        int _Slice;
        int _MinSlice;
        int _MaxSlice;
        //3Dview
        public RenderWindowControl renderWindowControl = new RenderWindowControl();
        vtkRenderWindow renWin;
        vtkRenderer renderer3D;
        vtkDICOMImageReader reader3D;
        double[] xmins = { 0, .51, 0, .51 };
        double[] xmaxs = { 0.49, 1, 0.49, 1 };
        double[] ymins = { 0, 0, .51, .51 };
        double[] ymaxs = { 0.49, 0.49, 1, 1 };
        double[,] CameraViewUp = new double[,] { { 0, 0, -1 }, { 0, 0, 1 }, { 0, 1, 0 }, };

        double[] CameraFocalPoint = new double[] { 0, 0, 0 };
        double[] CameraPoint = new double[] { 0, 1, 0 };

        vtkRenderWindowInteractor iren = vtkRenderWindowInteractor.New();
        List<vtkRenderer> rens = new List<vtkRenderer>();
        List<vtkImagePlaneWidget> planeWidget = new List<vtkImagePlaneWidget>();
        List<vtkResliceCursorWidget> resliceCursorWidget = new List<vtkResliceCursorWidget>();
        List<vtkResliceCursorLineRepresentation> resliceCursorRep = new List<vtkResliceCursorLineRepresentation>();
        double[] scalarRange = null;
        vtkImageData vtkimagedata;


        public _3DView()
        {
            InitializeComponent();


            renderWindowControl.Parent = panel1;
            renderWindowControl.AddTestActors = true;
            renderWindowControl.Load += renderWindowControl1_Load;
            host1.Child=panel1 ;
           // panel1.MouseWheel += Panel1_MouseWheel;
         
        }
        private void renderWindowControl1_Load(object sender, EventArgs e)
        {          
                try
                {
                // ReadDICOMSeriess(@"D:\DicomImages\images\JAYAPRIYA 26F--2.16.356.330.169832745262664717560289635220234881954\00004_1.3.12.2.1107.5.1.4.64225.30000019082223000443300001088");
                // ReadDICOMSeries(@"D:\DicomImages\images\JAYAPRIYA 26F--2.16.356.330.169832745262664717560289635220234881954\00004_1.3.12.2.1107.5.1.4.64225.30000019082223000443300001088");
                Render3(@"D:\DicomImages\images\JAYAPRIYA 26F--2.16.356.330.169832745262664717560289635220234881954\00004_1.3.12.2.1107.5.1.4.64225.30000019082223000443300001088");
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show(ex.Message, "Exception", MessageBoxButtons.OK);
                }
     
        }
        private void ReadDicom(string folder)
        {

        }
        private void ReadDICOMSeries(string folder)
        {
            // Path to vtk data must be set as an environment variable
            // VTK_DATA_ROOT = "C:\VTK\vtkdata-5.8.0"
            //vtkTesting test = vtkTesting.New();
           // string root = test.GetDataRoot();
            // Read all the DICOM files in the specified directory.
            // Caution: folder "DicomTestImages" don't exists by default in the standard vtk data folder
            // sample data are available at http://www.vtk.org/Wiki/images/1/12/VTK_Examples_StandardFormats_Input_DicomTestImages.zip
           // string folder = Path.co(root, @"Data\DicomTestImages");
            vtkDICOMImageReader reader = vtkDICOMImageReader.New();
            reader.SetDirectoryName(folder);
            reader.Update();
            // Visualize
            _ImageViewer = vtkImageViewer2.New();
            _ImageViewer.SetInputConnection(reader.GetOutputPort());
            // get range of slices (min is the first index, max is the last index)
            _ImageViewer.GetSliceRange(ref _MinSlice, ref _MaxSlice);
            //Debug.WriteLine("slices range from : " + _MinSlice.ToString() + " to " + _MaxSlice.ToString());

            // slice status message
            vtkTextProperty sliceTextProp = vtkTextProperty.New();
            sliceTextProp.SetFontFamilyToCourier();
            sliceTextProp.SetFontSize(20);
            sliceTextProp.SetVerticalJustificationToBottom();
            sliceTextProp.SetJustificationToLeft();

            _SliceStatusMapper = vtkTextMapper.New();
            _SliceStatusMapper.SetInput("Slice No " + (_Slice + 1).ToString() + "/" + (_MaxSlice + 1).ToString());
            _SliceStatusMapper.SetTextProperty(sliceTextProp);

            vtkActor2D sliceStatusActor = vtkActor2D.New();
            sliceStatusActor.SetMapper(_SliceStatusMapper);
            sliceStatusActor.SetPosition(15, 10);

            // usage hint message
            vtkTextProperty usageTextProp = vtkTextProperty.New();
            usageTextProp.SetFontFamilyToCourier();
            usageTextProp.SetFontSize(14);
            usageTextProp.SetVerticalJustificationToTop();
            usageTextProp.SetJustificationToLeft();

            vtkTextMapper usageTextMapper = vtkTextMapper.New();
            usageTextMapper.SetInput("Slice with mouse wheel\nor Up/Down-Key");
            usageTextMapper.SetTextProperty(usageTextProp);

            vtkActor2D usageTextActor = vtkActor2D.New();
            usageTextActor.SetMapper(usageTextMapper);
            usageTextActor.GetPositionCoordinate().SetCoordinateSystemToNormalizedDisplay();
            usageTextActor.GetPositionCoordinate().SetValue(0.05, 0.95);

            vtkRenderWindow renderWindow = renderWindowControl.RenderWindow;

            vtkInteractorStyleImage interactorStyle = vtkInteractorStyleImage.New();
            interactorStyle.MouseWheelForwardEvt += new vtkObject.vtkObjectEventHandler(interactor_MouseWheelForwardEvt);
            interactorStyle.MouseWheelBackwardEvt += new vtkObject.vtkObjectEventHandler(interactor_MouseWheelBackwardEvt);

            renderWindow.GetInteractor().SetInteractorStyle(interactorStyle);
            renderWindow.GetRenderers().InitTraversal();
            vtkRenderer ren;
            while ((ren = renderWindow.GetRenderers().GetNextItem()) != null)
                ren.SetBackground(0.0, 0.0, 0.0);

            _ImageViewer.SetRenderWindow(renderWindow);
            _ImageViewer.GetRenderer().AddActor2D(sliceStatusActor);
            _ImageViewer.GetRenderer().AddActor2D(usageTextActor);
            _ImageViewer.SetSlice(_MinSlice);
            _ImageViewer.Render();
        }

        private void MoveForwardSlice()
        {
          //  Debug.WriteLine(_Slice.ToString());
            if (_Slice < _MaxSlice)
            {
                _Slice += 1;
                _ImageViewer.SetSlice(_Slice);
                _SliceStatusMapper.SetInput("Slice No " + (_Slice + 1).ToString() + "/" + (_MaxSlice + 1).ToString());
                _ImageViewer.Render();
            }
        }

        /// <summary>
        /// move backward to next slice
        /// </summary>
        private void MoveBackwardSlice()
        {
           // Debug.WriteLine(_Slice.ToString());
            if (_Slice > _MinSlice)
            {
                _Slice -= 1;
                _ImageViewer.SetSlice(_Slice);
                _SliceStatusMapper.SetInput("Slice No " + (_Slice + 1).ToString() + "/" + (_MaxSlice + 1).ToString());
                _ImageViewer.Render();
            }
        }
        void interactor_MouseWheelForwardEvt(vtkObject sender, vtkObjectEventArgs e)
        {
            MoveForwardSlice();
        }
        /// <summary>
        /// event handler for mousewheel backward event
        /// </su
        /// mmary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void interactor_MouseWheelBackwardEvt(vtkObject sender, vtkObjectEventArgs e)
        {
            MoveBackwardSlice();
        }
        private void Panel1_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Coloring(e.Delta);
        }
        public void Render3D(string folder)
        {
            double thresh1 = 150.0;
            double thresh2 = 320.0;
            double thresh3 = 440.0;
            int[] nbDim = { 0, 1, 2, 3 };
            reader3D = vtkDICOMImageReader.New();
            reader3D.SetDirectoryName(folder);
            reader3D.SetDataSpacing(1, 1, 1);
            reader3D.SetDataOrigin(0, 0, 0);
            reader3D.Update();        
            renderWindow3D = vtkRenderWindow.New();
            vtkRenderWindowInteractor iren = vtkRenderWindowInteractor.New();
            //# irenStyle = vtk.vtkInteractorStyleImage()
            vtkInteractorStyleTrackballCamera irenStyle = vtkInteractorStyleTrackballCamera.New();
            iren.SetInteractorStyle(irenStyle);
            vtkCellPicker picker = vtkCellPicker.New();
            picker.SetTolerance(0.005);
            iren.SetRenderWindow(renderWindow3D);
            iren.SetPicker(picker);
          //  iren.seten(1)
            //iren.GetPickingManager().AddPicker(picker)
            iren.Initialize();

            // Define viewport ranges in renderwindow
            double[] xmins = { 0, .51, 0, .51 };
            double[] xmaxs = { 0.49, 1, 0.49, 1 };
            double[] ymins = { 0, 0, .51, .51 };
            double[] ymaxs = {0.49, 0.49, 1, 1};

            //# TODO : confirm if this camera orientation is good for 3D echo
            //viewUp = [[0, 0, -1],[0, 0, -1],[0, 1, 0]]

            // # Define 3 MPR views using image plane widgets and reslice cursor
               double[] ipws = new double[1];
              vtkRenderer[] rens = new vtkRenderer[1];
            vtkImageCursor3D[] rcws = new vtkImageCursor3D[1];
               double[] rcwReps = new double[1];
         
            renderWindowControl.Dock = DockStyle.Fill;
            panel1.Controls.Add(renderWindowControl);
            renderer3D = vtkRenderer.New();
            renderWindow3D = renderWindowControl.RenderWindow;
            renderWindow3D.AddRenderer(renderer3D);
            renderer3D.SetViewport(xmins[3], ymins[3], xmaxs[3], ymaxs[3]);
            
            foreach(int i in nbDim )
            {
                vtkRenderer ren = vtkRenderer.New();
                rens[0]=ren;
                renderWindow3D.AddRenderer(ren);
                ren.SetViewport(xmins[i], ymins[i], xmaxs[i], ymaxs[i]);
                vtkImageCursor3D rcw = vtkImageCursor3D.New();
                rcws[0] = rcw;
              //  rcw.AddInput(iren);
            };
             
            Coloring();
        }
            vtkVolume vol;
        public void MPR()
        {
            vtkTransform vtkTransform = new vtkTransform();
            vtkTransform.RotateZ(45);
            vtkTransform.Scale(1.414, 1.414 ,1.414);
            vtkImageReslice _imageReslice = new vtkImageReslice();
            _imageReslice.SetInputConnection(reader3D.GetOutputPort());
            _imageReslice.SetResliceTransform(vtkTransform);
            _imageReslice.SetInterpolationModeToCubic();
            _imageReslice.WrapOn();
            _imageReslice.AutoCropOutputOn();
            vtkImageData vtkImageActor = new vtkImageData();
          //  vtkImageActor.SetInput(_imageReslice); 
            double thresh1 = 150.0;
            double thresh2 = 320.0;
            double thresh3 = 440.0;
            double nbDim = 3;

            //Reading dicom
            reader3D = vtkDICOMImageReader.New();
         // reader3D.SetDirectoryName(folder);
            reader3D.SetDataSpacing(1, 1, 1);
            reader3D.SetDataOrigin(0, 0, 0);
            reader3D.Update();
        }
        public void Coloring(int shft = 0)
        {
            double thresh1 = 150.0;
            double thresh2 = 320.0;
            double thresh3 = 440.0;
            vtkGPUVolumeRayCastMapper texMapper = vtkGPUVolumeRayCastMapper.New();
          //vtkSmartVolumeMapper texMapper = vtkSmartVolumeMapper.New();
            vol = vtkVolume.New();
            vtkColorTransferFunction colorTransferFunction = vtkColorTransferFunction.New();
            vtkPiecewiseFunction funcOpacityScalar = vtkPiecewiseFunction.New();
            vtkPiecewiseFunction gpwf = vtkPiecewiseFunction.New();
            texMapper.SetInputConnection(reader3D.GetOutputPort());
            texMapper.SetBlendModeToComposite();
            texMapper.AutoAdjustSampleDistancesOn();
            //Set the color curve for the volume
            //ctf.AddHSVPoint(0, .67, .07, 1);
            //ctf.AddHSVPoint(94, .67, .0
            //
            //7, 1);
            //ctf.AddHSVPoint(139, 0, 0, 0);
            //ctf.AddHSVPoint(160, .28, .047, 1);
            //ctf.AddHSVPoint(254, .38, .013, 1);

            colorTransferFunction.AddRGBPoint(0.0, 0.0, 0.0, 0.0);
            colorTransferFunction.AddRGBPoint(thresh1, 140 / 255, 64 / 255, 38 / 255);
            colorTransferFunction.AddRGBPoint(thresh2, 225 / 255, 154 / 255, 74 / 255);
            colorTransferFunction.AddRGBPoint(thresh3, 255 / 255, 239 / 255, 243 / 255);
            colorTransferFunction.AddRGBPoint(211, 211 / 255, 168 / 255, 255 / 255);

            funcOpacityScalar.AddPoint(0, 0);
            funcOpacityScalar.AddPoint(thresh1, 0);
            funcOpacityScalar.AddPoint(thresh2, 0.45);
            funcOpacityScalar.AddPoint(thresh3, 0.63);
            funcOpacityScalar.AddPoint(255, 0.63);
            //ctf.AddRGBPoint(0.0, 0.0, 0.0, 0.0);
            //ctf.AddRGBPoint(64.0, 1.0, 0.0, 0.0);
            //ctf.AddRGBPoint(128.0, 0.0, 0.0, 1.0);
            //ctf.AddRGBPoint(192.0, 0.0, 1.0, 0.0);
            //ctf.AddRGBPoint(255.0, 0.0, 0.2, 0.0);

            //Set the opacity curve for the volume
            //spwf.AddPoint(584 + shft, 0);
            //spwf.AddPoint(651 + shft, .1);
            //spwf.AddPoint(255, 1);

            //spwf.AddPoint(4, 0);
            //spwf.AddPoint(51, .7);
            //spwf.AddPoint(155, 0.5);
            //spwf.AddPoint(255, 0.2);
            //spwf.AddPoint(1055, 0);
           // spwf.AddPoint(200.0, 0.0);
            //spwf.AddPoint(1200.0, 0.2);
            //spwf.AddPoint(4000.0, 0.4);

            //Set the gradient curve for the volume
            gpwf.AddPoint(0, .2);
            gpwf.AddPoint(10, 1);
            gpwf.AddPoint(225, 0.5);
            gpwf.AddPoint(1235, 0.2);
            gpwf.AddPoint(3235, 0);




            vtkVolumeProperty volumeProperty = new vtkVolumeProperty();
            volumeProperty.ShadeOn();
            volumeProperty.SetScalarOpacity(funcOpacityScalar);
            volumeProperty.SetInterpolationTypeToLinear();
            volumeProperty.SetColor(colorTransferFunction);
            volumeProperty.SetAmbient(0.20);
            volumeProperty.SetDiffuse(1.00);
            volumeProperty.SetSpecular(0.00);
            volumeProperty.SetSpecularPower(0.00);

    
   
            //vol.GetProperty().SetColor(ctf);
            // vol.GetProperty().SetScalarOpacity(spwf);
            //vol.GetProperty().SetGradientOpacity(gpwf);
            //vol.GetProperty().ShadeOn();
            // vol.GetProperty().SetInterpolationTypeToLinear();
            vol.SetMapper(texMapper);
            vol.SetProperty(volumeProperty);
            
            //green background
            renderer3D.SetBackground(0.3, 0.6, 0.3);
            //Go through the Graphics Pipeline
            renderer3D.AddVolume(vol);
            renderWindow3D.Render();
            
        }
        //private void DrawMode_Normal()
        //{
        //    #region 体属性 vtkVolumeProperty
        //    //设定体数据的属性:不透明性和颜色值映射标量值 
        //    vtkVolumeProperty volumeProperty = vtkVolumeProperty.New();
        //    volumeProperty.SetColor(m_ColorTransferFunction);
        //    volumeProperty.SetScalarOpacity(m_PiecewiseFunction);
        //    //设置插值类型
        //    volumeProperty.SetInterpolationTypeToNearest();
        //    volumeProperty.SetDiffuse(0.7);
        //    volumeProperty.SetAmbient(0.01);
        //    volumeProperty.SetSpecular(0.5);
        //    volumeProperty.SetSpecularPower(100);
        //    #endregion

        //    //绘制方法:体射线投射
        //    vtkVolumeRayCastCompositeFunction compositeFunction = vtkVolumeRayCastCompositeFunction.New();

        //    #region 体数据映射器 vtkVolumeRayCastMapper
        //    //体数据映射器  
        //    vtkVolumeRayCastMapper volumeMapper = vtkVolumeRayCastMapper.New();
        //    volumeMapper.SetInput(m_ImageData);
        //    volumeMapper.SetVolumeRayCastFunction(compositeFunction);
        //    #endregion

        //    #region 体 vtkVolume
        //    //体
        //    vtkVolume volume = vtkVolume.New();
        //    volume.SetMapper(volumeMapper);
        //    volume.SetProperty(volumeProperty);
        //    #endregion

        //    //模型体放入Renerer
        //    m_Renderer.AddVolume(volume);
        //}
        private void ReadDICOMSeriess(string folder)
        {
            //vtkDICOMImageReader reader3D = vtkDICOMImageReader.New();
            //reader3D.SetDirectoryName(folder);
            //reader3D.Update();

            //vtkVolumeRayCastCompositeFunction compositeFunction = vtkVolumeRayCastCompositeFunction.New();

            //vtkVolumeRayCastMapper volumeMapper = vtkVolumeRayCastMapper.New();
            //volumeMapper.SetInputConnection(reader3D.GetOutputPort());
            //volumeMapper.SetVolumeRayCastFunction(compositeFunction);
            //var colorFunction = vtkColorTransferFunction.New();
            //colorFunction.AddHSVPoint(0, 0.67, 0.07, 1);
            //colorFunction.ClampingOn();

            //var opacityFunction = vtkPiecewiseFunction.New();
            //opacityFunction.AddPoint(84, 0);
            ////opacityFunction.AddPoint(70, 0.00);
            ////opacityFunction.AddPoint(90, 0.00);
            ////opacityFunction.AddPoint(180, 0.00);
            //opacityFunction.Update();

            //var volumeProperty = vtkVolumeProperty.New();
            //volumeProperty.SetColor(colorFunction);
            //volumeProperty.SetScalarOpacity(opacityFunction);
            //volumeProperty.SetInterpolationTypeToNearest();
            //volumeProperty.SetDiffuse(0.7);
            //volumeProperty.SetAmbient(0.01);
            //volumeProperty.SetSpecular(0.5);
            //volumeProperty.SetSpecularPower(100);


            //vol = vtkVolume.New();
            //vol.SetMapper(volumeMapper);
            //vol.SetProperty(volumeProperty);
            //vol.SetOrigin(0, 0, 0);
            //vol.Update();


            reader3D = vtkDICOMImageReader.New();
            reader3D.SetDirectoryName(folder);
            reader3D.SetDataSpacing(1, 1, 1);
            reader3D.SetDataOrigin(0, 0, 0);
            reader3D.Update();

            // vtkVolumeRayCastMapper texMapper = vtkVolumeRayCastMapper.New();
            vtkSmartVolumeMapper texMapper = vtkSmartVolumeMapper.New();
            vol = vtkVolume.New();
            vtkColorTransferFunction ctf = vtkColorTransferFunction.New();
            vtkPiecewiseFunction spwf = vtkPiecewiseFunction.New();
            vtkPiecewiseFunction gpwf = vtkPiecewiseFunction.New();

            texMapper.SetInputConnection(reader3D.GetOutputPort());

            //Set the color curve for the volume
            //ctf.AddHSVPoint(0, .67, .07, 1);
            //ctf.AddHSVPoint(94, .67, .07, 1);
            //ctf.AddHSVPoint(139, 0, 0, 0);
            //ctf.AddHSVPoint(160, .28, .047, 1);
            //ctf.AddHSVPoint(254, .38, .013, 1);

            ctf.AddRGBPoint(0.0, 0.0, 0.0, 0.0);
            ctf.AddRGBPoint(64.0, 1.0, 0.0, 0.0);
            ctf.AddRGBPoint(128.0, 0.0, 0.0, 1.0);
            ctf.AddRGBPoint(192.0, 0.0, 1.0, 0.0);
            ctf.AddRGBPoint(255.0, 0.0, 0.2, 0.0);

            //Set the opacity curve for the volume
            spwf.AddPoint(584 + 0, 0);
            spwf.AddPoint(651 + 0, .1);
            //spwf.AddPoint(255, 1);


            //spwf.AddPoint(4, 0);
            //spwf.AddPoint(51, .7);
            //spwf.AddPoint(155, 0.5);
            //spwf.AddPoint(255, 0.2);
            //spwf.AddPoint(1055, 0);


            //Set the gradient curve for the volume
            //gpwf.AddPoint(0, .2);
            gpwf.AddPoint(10, 1);
            gpwf.AddPoint(225, 0.5);
            gpwf.AddPoint(1235, 0.2);
            gpwf.AddPoint(3235, 0);

            vol.GetProperty().SetColor(ctf);
            vol.GetProperty().SetScalarOpacity(spwf);
            //vol.GetProperty().SetGradientOpacity(gpwf);

            vol.GetProperty().ShadeOn();
            vol.GetProperty().SetInterpolationTypeToLinear();

            vol.SetMapper(texMapper);

            //green background
            renderer3D.SetBackground(0.3, 0.6, 0.3);
            //Go through the Graphics Pipeline
            renderer3D.AddVolume(vol);

            renderWindow3D.Render();
        }
        private vtkActor BuildOutlineActor(vtkImageData ImageData)
        {

            vtkOutlineFilter OutlineFilter = vtkOutlineFilter.New();
            OutlineFilter.SetInputData(ImageData);
            vtkPolyDataMapper OutlineMapper = vtkPolyDataMapper.New();
            OutlineMapper.SetInputConnection(OutlineFilter.GetOutputPort());
            vtkActor outlineActor = vtkActor.New();
            outlineActor.SetMapper(OutlineMapper);
            outlineActor.GetProperty().SetColor(0.5, 0.5, 0.5);

            return outlineActor;
        }
        public void Render3(string folder)
        {
            reader3D = vtkDICOMImageReader.New();
            reader3D.SetDirectoryName(folder);
            reader3D.Update();
            vtkimagedata = reader3D.GetOutput();
            scalarRange = vtkimagedata.GetScalarRange();
            // vtkImageViewer resliceImageViewerXY = new vtkImageViewer();
            // resliceImageViewerXY.SetInputConnection(reader3D.GetOutput());          

            vtkActor OutlineActor = BuildOutlineActor(vtkimagedata);
            //renderer3D.AddActor(OutlineActor);

            vtkRenderer renderer30 = vtkRenderer.New();

            rens.Add(renderer30);
            vtkRenderer renderer31 = vtkRenderer.New();
            rens.Add(renderer31);
            vtkRenderer renderer32 = vtkRenderer.New();
            rens.Add(renderer32);
            vtkRenderer renderer33 = vtkRenderer.New();
            rens.Add(renderer33);
            renWin = vtkRenderWindow.New();
            //renWin.SetSize(1000,1000);
            renWin.BordersOn();
            renWin.SetMultiSamples(0);
            for (int i = 0; i < 4; i++)
            {
                // rens[i] = vtkRenderer.New();
                renWin.AddRenderer(rens[i]);
            }

            //# irenStyle = vtk.vtkInteractorStyleImage()
            //vtkInteractorStyleTrackballCamera irenStyle = vtkInteractorStyleTrackballCamera.New();
            //iren.SetInteractorStyle(irenStyle);


            vtkCellPicker picker = vtkCellPicker.New();
            picker.SetTolerance(0.005);

            iren.SetRenderWindow(renWin);
            vtkProperty ipwProp = vtkProperty.New();
            int[] imageDims = new int[3];
            imageDims = vtkimagedata.GetDimensions();

            for (int i = 0; i < 3; i++)
            {
                vtkImagePlaneWidget vtkImagePlaneWidget = vtkImagePlaneWidget.New();
                planeWidget.Add(vtkImagePlaneWidget);

                planeWidget[i].SetInteractor(iren);
                planeWidget[i].SetPicker(picker);
                planeWidget[i].RestrictPlaneToVolumeOn();
                double[] color = { 0, 0, 0 };
                color[i] = 1;
                planeWidget[i].GetPlaneProperty().SetColor(color[0], color[1], color[2]);
                planeWidget[i].SetTexturePlaneProperty(ipwProp);
                planeWidget[i].TextureInterpolateOff();
                planeWidget[i].SetResliceInterpolateToLinear();
                planeWidget[i].SetInputData(vtkimagedata);
                planeWidget[i].SetPlaneOrientation(i);
                planeWidget[i].SetSliceIndex(imageDims[i] / 2);
                planeWidget[i].DisplayTextOn();
                planeWidget[i].SetDefaultRenderer(rens[3]);
                planeWidget[i].SetWindowLevel(1358, -27, 0);
                planeWidget[i].On();
                planeWidget[i].InteractionOn();
                planeWidget[i].SetEnabled(1);

            }

            planeWidget[1].SetLookupTable(planeWidget[0].GetLookupTable());
            planeWidget[2].SetLookupTable(planeWidget[0].GetLookupTable());
            vtkResliceCursorCallback cbk = new vtkResliceCursorCallback();
            vtkResliceCursor resliceCursor = vtkResliceCursor.New();
            double[] reslicenterobj = reader3D.GetOutput().GetCenter();
            resliceCursor.SetCenter(reslicenterobj[0], reslicenterobj[1], reslicenterobj[2]);
            resliceCursor.SetThickMode(0);
            resliceCursor.SetThickness(10, 10, 10);
            resliceCursor.SetImage(vtkimagedata);

            double[,] viewUp = new double[,] { { 0, 0, -1 }, { 0, 0, 1 }, { 0, 1, 0 } };

            //List<vtkImagePlaneWidget> vtkPlaneWidgetlist=new List<vtkImagePlaneWidget>();

            for (int i = 0; i < 3; i++)
            {
                vtkResliceCursorWidget vtkResliceCursorWidgetseparate = vtkResliceCursorWidget.New();
                resliceCursorWidget.Add(vtkResliceCursorWidgetseparate);
                vtkResliceCursorLineRepresentation vtkResliceCursorLineRepresentation = vtkResliceCursorLineRepresentation.New();
                resliceCursorRep.Add(vtkResliceCursorLineRepresentation);
                resliceCursorWidget[i].SetInteractor(iren);
                resliceCursorWidget[i].SetRepresentation(resliceCursorRep[i]);
                resliceCursorRep[i].GetResliceCursorActor().GetCursorAlgorithm().SetResliceCursor(resliceCursor);
                resliceCursorRep[i].GetResliceCursorActor().GetCursorAlgorithm().SetReslicePlaneNormal(i);
                resliceCursorRep[i].SetManipulationMode(2);
                double[] minVal = new double[4];
                minVal = vtkimagedata.GetScalarRange();
                vtkImageReslice reslice = (vtkImageReslice)resliceCursorRep[i].GetReslice();

                reslice.SetInputConnection(reader3D.GetOutputPort());
                reslice.SetBackgroundColor(minVal[0], minVal[0], minVal[0], minVal[0]);
                // reslice.AutoCropOutputOn();
                reslice.Update();

                resliceCursorWidget[i].SetDefaultRenderer(rens[i]);
                resliceCursorWidget[i].SetEnabled(1);


                rens[i].GetActiveCamera().SetFocalPoint(0, 0, 0);
                double[] camPos = { 0, 0, 0 };
                camPos[i] = 1;
                rens[i].GetActiveCamera().SetPosition(camPos[0], camPos[1], camPos[2]);
                rens[i].GetActiveCamera().ParallelProjectionOn();
                rens[i].GetActiveCamera().SetViewUp(CameraViewUp[i, 0], CameraViewUp[i, 1], CameraViewUp[i, 2]);
                rens[i].ResetCamera();

                cbk.IPW[i] = planeWidget[i];
                cbk.RCW[i] = resliceCursorWidget[i];
                resliceCursorWidget[i].AnyEvt += cbk.MyCallback;
                // resliceCursorWidget[i].AddObserver((uint)vtkResliceCursorWidget.ResetCursorEvent_WrapperEnum.ResliceAxesChangedEvent, cbk, 1.0f);
                resliceCursorRep[i].SetWindowLevel(minVal[1] - minVal[0], (minVal[0] + minVal[1]) / 2.0, 0);
                planeWidget[i].SetWindowLevel(minVal[1] - minVal[0], (minVal[0] + minVal[1]) / 2.0, 0);
                resliceCursorRep[i].SetLookupTable(resliceCursorRep[0].GetLookupTable());
                planeWidget[i].GetColorMap().SetLookupTable(resliceCursorRep[0].GetLookupTable());
            }


            rens[0].SetBackground(0, 0, 0);
            rens[1].SetBackground(0, 0, 0);
            rens[2].SetBackground(0, 0, 0);
            rens[3].AddActor(OutlineActor);
            rens[3].SetBackground(0.0, 0.0, 0.0);
            renWin.SetSize(600, 600);
            // renWin->SetFullScreen(1);

            rens[0].SetViewport(0, 0, 0.5, 0.5);
            rens[1].SetViewport(0.5, 0, 1, 0.5);
            rens[2].SetViewport(0, 0.5, 0.5, 1);
            rens[3].SetViewport(0.5, 0.5, 1, 1);

            // Set the actors' positions

            renWin.Render();


            rens[3].GetActiveCamera().Elevation(110);
            rens[3].GetActiveCamera().SetViewUp(0, 0, -1);
            rens[3].GetActiveCamera().Azimuth(45);
            rens[3].GetActiveCamera().Dolly(1.15);
            rens[3].ResetCameraClippingRange();

            vtkInteractorStyleImage style = vtkInteractorStyleImage.New();
            iren.SetInteractorStyle(style);
            iren.Initialize();
            iren.Start();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //vol.UpdateScalarOpacityforSampleSize(renderer3D, 100);
            //renderer3D.Render();       
           // Coloring(100);
        }
    }
}
