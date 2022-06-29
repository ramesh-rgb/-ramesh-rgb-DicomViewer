using Dicom;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageViewer
{
    public  class DicomDec
    {
        public string GetFilePath(DicomDataset imageRecord, string fileName)
        {
            var values1 = imageRecord.Get<string[]>(DicomTag.ReferencedFileID);
            var filepath = System.IO.Path.Combine(values1[0], values1[1]);
            DirectoryInfo parentDir = Directory.GetParent(fileName);
            string filepath2 = System.IO.Path.Combine(parentDir.FullName, filepath);
            return filepath2;
        }
    }
}
