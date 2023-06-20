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
    public class RecurseFileReader :IRecursive 
    {
        private List<FileInfo>  fileinfoList = new List<FileInfo>();
        private List<FileInfo> uniqueList=new List<FileInfo>();      
  
        public List<FileInfo> RecursiveDirectory(DirectoryInfo directoryInfo)
        {
            var subdirectories = directoryInfo.EnumerateDirectories();
            var files = directoryInfo.EnumerateFiles();
            foreach (var subdirectory in subdirectories)
            {
                RecursiveDirectory(subdirectory);
            }
            HandleFiles(files);
            Console.WriteLine(directoryInfo.EnumerateFiles().Count());
            return uniqueList;
        }

        public void HandleFiles(IEnumerable<FileInfo> files)
        {
            fileinfoList.AddRange(files);
                 uniqueList = fileinfoList.Distinct().ToList();  
        }
    }
}
