using Dicom;
using ImageViewer.Screens;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageViewer
{
   
    public class RecurseFileStructure
    {

        List<FileInfo>  fileinfoList = new List<FileInfo>();
        List<FileInfo> uniqueList=new List<FileInfo>();
        ObservableCollection<DirectoryInfo> ts = new ObservableCollection<DirectoryInfo>();
        List<Series> serieslistobj = new List<Series>();
        string studyUId = "1";
        int seriesindex = -1;

        public List<FileInfo> TraverseDirectory(DirectoryInfo directoryInfo)
        {
            var subdirectories = directoryInfo.EnumerateDirectories();
            var files = directoryInfo.EnumerateFiles();

            foreach (var subdirectory in subdirectories)
            {
                TraverseDirectory(subdirectory);
            }


            HandleFiles(files);
            Console.WriteLine(directoryInfo.EnumerateFiles().Count());
            // List<FileInfo> uniqueList = fileinfoList.Distinct().ToList();

            return uniqueList;


        }

       public void HandleFiles(IEnumerable<FileInfo> files)
        {
            fileinfoList.AddRange(files);
            uniqueList = fileinfoList.Distinct().ToList();
           // uniqueList.AddRange(files);
           
        }
        //public List<SeriesImagePanel> TraverseDirectory(DirectoryInfo directoryInfo)
        //{
        //    var subdirectories = directoryInfo.EnumerateDirectories();
        //    var files = directoryInfo.EnumerateFiles();

        //    foreach (var subdirectory in subdirectories)
        //    {
        //        TraverseDirectory(subdirectory);
        //    }


        //    // HandleFiles(files);
        //    Console.WriteLine(directoryInfo.EnumerateFiles().Count());
        //    //foreach (var file in files.Where(x => x.Extension == ".dcm"))
        //    //{

        //    //    var dicomFile = Dicom.DicomFile.Open(file.FullName);
        //    //    var sUId = dicomFile.Dataset.GetSingleValueOrDefault(DicomTag.SeriesInstanceUID, string.Empty);
        //    //    if (studyUId == sUId)
        //    //    {
        //    //        var t = SeriesList[seriesindex];
        //    //        if (t.Name == dicomFile.Dataset.GetSingleValueOrDefault(DicomTag.SeriesNumber, string.Empty))
        //    //            t.AddFile(file.FullName);
        //    //      //  var d  = ts[seriesindex];
        //    //        var d = serieslistobj[seriesindex];
        //    //        d.files.Add(file.FullName);
        //    //        Console.WriteLine("dicomfile added ******************");
        //    //    }
        //    //    else
        //    //    {
        //    //        seriesindex++;
        //    //        var sNum = dicomFile.Dataset.GetSingleValueOrDefault(DicomTag.SeriesNumber, string.Empty);
        //    //        var sIP = new SeriesImagePanel();
        //    //        sIP.Name = sNum;
        //    //        sIP.AddFile(file.FullName);
        //    //       var fList= new Series();
        //    //        fList.files.Add(file.FullName);
        //    //        SeriesList.Add(sIP);
        //    //        serieslistobj.Add(fList);

        //    //        studyUId = sUId;

        //    //        Console.WriteLine("series obj created******************");

        //    //    }

        //    //    HandleFile(file);
        //    //}
        //    return SeriesList;
        //}

        //void HandleFile(FileInfo file)
        //{
        //    var dicomFile = Dicom.DicomFile.Open(file.FullName);
        //    dicomDir.AddFile(dicomFile, String.Format(@"000001\{0}", file.Name));
        //    //DicomFile file1= DicomFile.Open(file.Name);           
        //    //DicomDataset dicoms = file1.Dataset;
        //    //    string suid= dicoms.Get<string>(DicomTag.StudyInstanceUID).ToString();
        //    Console.WriteLine("{0}", file.Name);
        //}

        //void HandleFiles(IEnumerable<FileInfo> files)
        //{

        //}


    }
}
