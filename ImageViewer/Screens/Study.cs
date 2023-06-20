using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageViewer.Screens
{
    internal class Study
    {
        private string StudyName;
        private string StudyDescription;
        private string StudyInstanceUID;
        private string StudyInstanceName;
        
        public void SetStudyUID(string sUid)
        {
            StudyInstanceUID = sUid;
        }

        public string GetstudyUID()
        {
            if(StudyInstanceUID != null)
            {

            }
            return StudyInstanceUID;
        }

    }
}
