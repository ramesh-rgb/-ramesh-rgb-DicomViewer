using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageViewer
{
    public interface IRecursive
    {
        List<FileInfo> RecursiveDirectory(DirectoryInfo directoryInfo);
        void HandleFiles(IEnumerable<FileInfo> files);
    }
}
