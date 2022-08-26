using Dicom;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageViewer.Screens
{
    public  class Seriessegregation
    {
        List<SeriesImagePanel> seriesImagePanelsList=new List<SeriesImagePanel>();
        string studyUId = "1";
        int seriesIndex = -1;
        public List<SeriesImagePanel> GetseriesImagePanels(List<FileInfo> files)
        {
            //Console.WriteLine(DirectoryInfo.EnumerateFiles().Count());
            foreach (var file in files.Where(x => x.Extension == ".dcm"))
            {
                var dicomFile = Dicom.DicomFile.Open(file.FullName);
                var sUId = dicomFile.Dataset.GetSingleValueOrDefault(DicomTag.SeriesInstanceUID, string.Empty);
                if (studyUId == sUId)
                {
                    var sip = seriesImagePanelsList[seriesIndex];
                    if (sip.Name == dicomFile.Dataset.GetSingleValueOrDefault(DicomTag.SeriesNumber, string.Empty))
                        sip.AddFile(file.FullName);
                        Console.WriteLine("dicomfile added ******************");
                }
                else
                {
                    seriesIndex++;
                    var sNum = dicomFile.Dataset.GetSingleValueOrDefault(DicomTag.SeriesNumber, string.Empty);
                    var sIP = new SeriesImagePanel();
                    sIP.Name = sNum;
                    sIP.AddFile(file.FullName);
                    // var fList = new Series();
                    // fList.files.Add(file.FullName);
                    seriesImagePanelsList.Add(sIP);
                    // serieslistobj.Add(fList);
                    studyUId = sUId;
                    Console.WriteLine("series obj created******************");
                }
            }
            return seriesImagePanelsList;       

        }
    }

}


