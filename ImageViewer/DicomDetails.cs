﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageViewer
{
    public  class DicomDetails
    {
        public string patientname { get; set; }
        public string age { get; set; }
        public string modality { get; set; }
        public string patientid { get; set; }
        public int instance { get; set; }
        public string studydescription { get; set; }
        public string gender { get;set;}
    }

    public class Patient
    {
        public string patientName { get; set; }
        public string patientID { get; set; }
    }
}
