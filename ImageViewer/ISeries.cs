using ImageViewer.Screens;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageViewer
{
    public interface ISeries
    {
        List<SeriesImagePanel> GetseriesImagePanelsBy(List<FileInfo> files);
    }
}






