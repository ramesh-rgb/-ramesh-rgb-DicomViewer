using Kitware.VTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ImageViewer.Screens
{
    class vtkResliceCursorCallback
    {


        public vtkImagePlaneWidget[] IPW;
        public vtkResliceCursorWidget[] RCW;
        public vtkResliceCursorCallback()
        {
            IPW = new vtkImagePlaneWidget[3];
            RCW = new vtkResliceCursorWidget[3];
        }


        public void MyCallback(vtkObject sender, vtkObjectEventArgs e)
        {

            vtkImagePlaneWidget ipw = vtkImagePlaneWidget.SafeDownCast(e.Caller);
            if (ipw != null)
            {
                double[] wl = new double[2];
                Marshal.Copy(e.CallData, wl, 0, 2);

                if (ipw == this.IPW[0])
                {
                    this.IPW[1].SetWindowLevel(wl[0], wl[1], 1);
                    this.IPW[2].SetWindowLevel(wl[0], wl[1], 1);
                }
                else if (ipw == this.IPW[1])
                {
                    this.IPW[0].SetWindowLevel(wl[0], wl[1], 1);
                    this.IPW[2].SetWindowLevel(wl[0], wl[1], 1);
                }
                else if (ipw == this.IPW[2])
                {
                    this.IPW[0].SetWindowLevel(wl[0], wl[1], 1);
                    this.IPW[1].SetWindowLevel(wl[0], wl[1], 1);
                }
            }
            vtkResliceCursorWidget rcw = vtkResliceCursorWidget.SafeDownCast(e.Caller);
            if (rcw != null)
            {
                vtkResliceCursorLineRepresentation rep = vtkResliceCursorLineRepresentation.SafeDownCast(rcw.GetRepresentation());
                vtkResliceCursor rc = rep.GetResliceCursorActor().GetCursorAlgorithm().GetResliceCursor();
                double[] origin = rc.GetCenter();

                for (int i = 0; i < 3; i++)
                {
                    double[] normal = rc.GetPlane(i).GetNormal();
                    vtkPlaneSource ps = (vtkPlaneSource)IPW[i].GetPolyDataAlgorithm();
                    ps.SetNormal(normal[0], normal[1], normal[2]);
                    double[] centerorigin = rc.GetPlane(i).GetOrigin();
                    ps.SetCenter(centerorigin[0], centerorigin[1], centerorigin[2]);
                    this.IPW[i].UpdatePlacement();
                }
                this.RCW[0].Render();
            }

        }

        //public new static vtkResliceCursorCallback New()
        //{
        //    return new vtkResliceCursorCallback();
        //}

        //public override void Execute(vtkObject caller, uint eventId, IntPtr callData)
        //{
        //    vtkImagePlaneWidget ipw = vtkImagePlaneWidget.SafeDownCast(caller);
        //    if (ipw != null)
        //    {
        //        double[] wl = new double[2];
        //        Marshal.Copy(callData, wl, 0, 2);

        //        if (ipw == this.IPW[0])
        //        {
        //            this.IPW[1].SetWindowLevel(wl[0], wl[1], 1);
        //            this.IPW[2].SetWindowLevel(wl[0], wl[1], 1);
        //        }
        //        else if (ipw == this.IPW[1])
        //        {
        //            this.IPW[0].SetWindowLevel(wl[0], wl[1], 1);
        //            this.IPW[2].SetWindowLevel(wl[0], wl[1], 1);
        //        }
        //        else if (ipw == this.IPW[2])
        //        {
        //            this.IPW[0].SetWindowLevel(wl[0], wl[1], 1);
        //            this.IPW[1].SetWindowLevel(wl[0], wl[1], 1);
        //        }
        //    }
        //    vtkResliceCursorWidget rcw = vtkResliceCursorWidget.SafeDownCast(caller);
        //    if (rcw != null)
        //    {
        //        vtkResliceCursorLineRepresentation rep = vtkResliceCursorLineRepresentation.SafeDownCast(rcw.GetRepresentation());
        //        vtkResliceCursor rc = rep.GetResliceCursorActor().GetCursorAlgorithm().GetResliceCursor();
        //        double[] origin = rc.GetCenter();

        //        for (int i = 0; i < 3; i++)
        //        {
        //            double[] normal = rc.GetPlane(i).GetNormal();
        //            vtkPlaneSource ps = (vtkPlaneSource)IPW[i].GetPolyDataAlgorithm();
        //            ps.SetNormal(normal[0], normal[1], normal[2]);
        //            double[] centerorigin = rc.GetPlane(i).GetOrigin();
        //            ps.SetCenter(centerorigin[0], centerorigin[1], centerorigin[2]);
        //            this.IPW[i].UpdatePlacement();
        //        }
        //        this.RCW[0].Render();
        //    }
        //}
    }
}
