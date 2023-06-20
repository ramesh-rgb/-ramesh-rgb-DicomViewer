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
    public  class SeriesSegregation
    {
        private List<SeriesImagePanel> seriesImagePanelsList=new List<SeriesImagePanel>();
        private string studyUId = "1";
        private int seriesIndex = -1;
        private IRecursive recursive;
        private DirectoryInfo directoryInfo;
     
        public SeriesSegregation(IRecursive recursive)
        {
            this.recursive = recursive;
        }


    public List<SeriesImagePanel> GetseriesImagePanels(DirectoryInfo directoryInfo)
        {
            try
            {
                this.directoryInfo = directoryInfo;
                if (directoryInfo.Exists)
                {
                    List<FileInfo> files = this.recursive.RecursiveDirectory(this.directoryInfo);
                    //Console.WriteLine(DirectoryInfo.EnumerateFiles().Count());
                    foreach (var file in files.Where(x => x.Extension == ".dcm"))
                    //foreach (var file in files)
                    {
                        var dicomFile = Dicom.DicomFile.Open(file.FullName);
                        if (dicomFile.Dataset.Contains(DicomTag.PatientName))
                              MainWindow.PatientName = dicomFile.Dataset.GetSingleValueOrDefault(DicomTag.PatientName
                                  
                                  , string.Empty);
                              MainWindow.StudyDescription= dicomFile.Dataset.GetSingleValueOrDefault(DicomTag.StudyDescription, string.Empty);
                              MainWindow.Studydate = dicomFile.Dataset.GetSingleValueOrDefault(DicomTag.StudyDate, string.Empty);

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
                            SeriesImagePanel sIP = new SeriesImagePanel();
                            sIP.Name = sNum;
                            sIP.retrieveScoutParam();
                            seriesImagePanelsList.Add(sIP);
                            studyUId = sUId;
                            Console.WriteLine("series obj created******************");
                        }
                    }                  
                }
                return seriesImagePanelsList;

            }
            catch (Exception ex)
            {
                return null;
            }           
        }
    }

}


