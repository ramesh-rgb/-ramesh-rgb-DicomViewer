using Dicom;
using Dicom.Imaging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageViewer
{
    class DicomDecoder
    {
        public int bitsAllocated;
        public int bitsStored;
        public int width;
        public int height;
        public int offset;
        public int nImages;
        public int samplesPerPixel;
        public double pixelDepth = 1.0;
        public double pixelWidth = 1.0;
        public double pixelHeight = 1.0;
        public string unit;
        public double windowCentre, windowWidth;
        public bool signedImage;
     //   public TypeOfDicomFile typeofDicomFile;
        public List<string> dicomInfo;
        public bool dicmFound; // "DICM" found at offset 128
        public DicomImage dicomimage;
   //   DicomDictionary dic;
        BinaryReader file;
        String dicomFileName;
        String photoInterpretation;
        bool littleEndian = true;
        bool oddLocations;  // one or more tags at odd locations
        bool bigEndianTransferSyntax = false;
        bool inSequence;
        bool widthTagFound;
        bool heightTagFound;
        bool pixelDataTagFound;
        int location = 0;
        int elementLength;
        int vr;  // Value Representation
        int min8 = Byte.MinValue;
        int max8 = Byte.MaxValue;
        int min16 = short.MinValue;
        int max16 = ushort.MaxValue;
        int min12 = short.MinValue;
        int max12 = ushort.MaxValue;
        int pixelRepresentation;
        Dicom.Imaging.PhotometricInterpretation photometricinterpretation;
        double rescaleIntercept;
        double rescaleSlope;
        byte[] reds;
        byte[] greens;
        byte[] blues;
        byte[] vrLetters = new byte[2];
        List<byte> pixels8;
        List<byte> pixels24; // 8 bits bit depth, 3 samples per pixel
        List<ushort> pixels16;
        List<ushort> pixels12;
        List<int> pixels16Int;
        List<int> pixels12Int;
        private byte[] pixeldata;
        public string patientname;
        public string age;
        public string modality;
        public string patientid;
        public int instance;
        public string studydescription;
        public string gender;
        public Bitmap bmp;
        public DicomDecoder()
        {
          InitializeDicom();
        }
        void InitializeDicom()
        {
            bitsAllocated = 0;
            bitsStored = 0;
            width = 1;
            height = 1;
            offset = 1;
            nImages = 1;
            samplesPerPixel = 1;
            photoInterpretation = "";
            unit = "mm";
            windowCentre = 0;
            windowWidth = 0;
            signedImage = false;
            widthTagFound = false;
            heightTagFound = false;
            pixelDataTagFound = false;
            littleEndian = true;
            rescaleIntercept = 0.0; // Default value
            rescaleSlope = 1.0; // Default value
            //  typeofDicomFile = TypeOfDicomFile.NotDicom;
            patientname = null;
            modality = null;
            patientid = null;
            studydescription = null;
            instance = 0;
            age = null;
            gender=null;
            dicomimage = null;
        }
        public string DicomFileName
        {
            set
            {
                dicomFileName = value;
                InitializeDicom();                          
                try
                {
                    bool readResult = ReadFileInfo(dicomFileName);
                    //if (readResult && widthTagFound && heightTagFound && pixelDataTagFound)
                    if(readResult)
                    {
                        ReadPixels();
                       // if (dicmFound == true)
                           // typeofDicomFile = TypeOfDicomFile.Dicom3File;
                       // else
                           // typeofDicomFile = TypeOfDicomFile.DicomOldTypeFile;
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message.ToString());
                }
                finally
                {                   
                }
            }
        }

        [Obsolete]
        public bool ReadFileInfo(string file)
        {
            dicomimage = new DicomImage(file, 0);
            pixeldata = dicomimage.PixelData.GetFrame(0).Data;
            width = dicomimage.Width;
            height = dicomimage.Height;
            bitsStored = dicomimage.Dataset.Get<int>(DicomTag.BitsAllocated);
            windowCentre = dicomimage.WindowCenter;
            windowWidth = dicomimage.WindowWidth;
            samplesPerPixel = dicomimage.Dataset.Get<int>(DicomTag.SamplesPerPixel);
            photometricinterpretation = dicomimage.PhotometricInterpretation;
            signedImage = false;
            patientname = dicomimage.Dataset.Get<string>(DicomTag.PatientName);
            modality = dicomimage.Dataset.Get<string>(DicomTag.Modality);
            bmp = new Bitmap(dicomimage.RenderImage(0).As<Bitmap>());
            //instance= dicomimage.Dataset.Get<int>(DicomTag.InstanceNumber);
            //studydescription = dicomimage.Dataset.Get<string>(DicomTag.StudyDescription);
            //patientid = dicomimage.Dataset.Get<string>(DicomTag.PatientID);
            //age = dicomimage.Dataset.Get<string>(DicomTag.PatientAge);
            //gender = dicomimage.Dataset.Get<string>(DicomTag.PatientSex);
            return true;
        }
        public void GetPixels8(ref List<byte> pixels)
        {
            pixels = pixels8;
        }
        public void GetPixels16(ref List<ushort> pixels)
        {
            pixels = pixels16;
        }
        public void GetPixels24(ref List<byte> pixels)
        {
            pixels = pixels24;
        }
        public void GetPixels12(ref List<ushort> pixels)
        {
            pixels = pixels12;
        }
        void ReadPixels()
        {
            if (samplesPerPixel == 1 && bitsStored == 8)
            {
                if (pixels8 != null)
                    pixels8.Clear();
                pixels8 = new List<byte>();
                int numPixels = width * height;
                byte[] buf = new byte[numPixels];        
                for (int i = 0; i < numPixels; ++i)
                {
                    int pixVal = (int)(pixeldata[i] * rescaleSlope + rescaleIntercept);
                    // We internally convert all 8-bit images to the range 0 - 255
                    //if (photoInterpretation.Equals("MONOCHROME1", StringComparison.OrdinalIgnoreCase))
                    //    pixVal = 65535 - pixVal;
                    Console.WriteLine("Decoderrrrr");
                    if (photometricinterpretation.ToString() == "MONOCHROME1")
                        pixVal = max8 - pixVal;
                    pixels8.Add((byte)(pixelRepresentation == 1 ? pixVal : (pixVal - min8)));
                }
            }
            if (samplesPerPixel == 1 && bitsStored == 16)
            {
                if (pixels16 != null)
                    pixels16.Clear();
                if (pixels16Int != null)
                    pixels16Int.Clear();
                pixels16 = new List<ushort>();
                pixels16Int = new List<int>();
                int numPixels = width * height;
               // byte[] bufByte = new byte[numPixels * 2];
                byte[] signedData = new byte[2];
             //   file.BaseStream.Position = offset;
             //   file.Read(bufByte, 0, numPixels * 2);
                ushort unsignedS;
                int i, i1, pixVal;
                byte b0, b1;
                Console.WriteLine("Sixteen bit Image ");
                for (i = 0; i < numPixels; ++i)
                {
                    i1 = i * 2;
                    b0 = pixeldata[i1];
                    b1 = pixeldata[i1 + 1];
                    unsignedS = Convert.ToUInt16((b1 << 8) + b0);
                    if (pixelRepresentation == 0) // Unsigned
                    {
                        pixVal = (int)(unsignedS * rescaleSlope + rescaleIntercept);
                        if (photometricinterpretation.Value == "MONOCHROME1")
                            pixVal = max16 - pixVal;
                    }
                    else  // Pixel representation is 1, indicating a 2s complement image
                    {
                        signedData[0] = b0;
                        signedData[1] = b1;
                        short sVal = System.BitConverter.ToInt16(signedData, 0);
                        // Need to consider rescale slope and intercepts to compute the final pixel value
                        pixVal = (int)(sVal * rescaleSlope + rescaleIntercept);
                        if (photometricinterpretation.Value == "MONOCHROME1")
                            pixVal = max16 - pixVal;
                    }
                    pixels16Int.Add(pixVal);
                }
                int minPixVal = pixels16Int.Min();
                signedImage = false;
                if (minPixVal < 0) signedImage = true;
                // Use the above pixel data to populate the list pixels16 
                foreach (int pixel in pixels16Int)
                {
                    // We internally convert all 16-bit images to the range 0 - 65535
                    //if (signedImage)
                    //    pixels16.Add((ushort)(pixel - min16));
                    //else
                    //    pixels16.Add((ushort)(pixel));
                    pixels16.Add((ushort)(pixel - (signedImage ? min16 : 0)));
                }
                pixels16Int.Clear();
            }
            if (samplesPerPixel == 1 && bitsStored == 12)
            {
                if (pixels12 != null)
                    pixels12.Clear();
                if (pixels12Int != null)
                    pixels12Int.Clear();

                pixels12 = new List<ushort>();
                pixels12Int = new List<int>();
                int numPixels = width * height;
               //  byte[] bufByte = new byte[numPixels * 2];
                byte[] signedData = new byte[2];
                 // file.BaseStream.Position = offset;
                //   file.Read(bufByte, 0, numPixels * 2);
                ushort unsignedS;
                int i, i1, pixVal;
                byte b0, b1;
                Console.WriteLine("Sixteen bit Image ");
                for (i = 0; i < numPixels; ++i)
                {
                    i1 = i * 2;
                    b0 = pixeldata[i1];
                    b1 = pixeldata[i1 + 1];
                    unsignedS = Convert.ToUInt16((b1 << 8) + b0);
                    if (pixelRepresentation == 0) // Unsigned
                    {
                        pixVal = (int)(unsignedS * rescaleSlope + rescaleIntercept);
                        if (photometricinterpretation.Value == "MONOCHROME1")
                            pixVal = max12 - pixVal;
                    }
                    else  // Pixel representation is 1, indicating a 2s complement image
                    {
                        signedData[0] = b0;
                        signedData[1] = b1;
                        short sVal = System.BitConverter.ToInt16(signedData, 0);

                        // Need to consider rescale slope and intercepts to compute the final pixel value
                        pixVal = (int)(sVal * rescaleSlope + rescaleIntercept);
                        if (photometricinterpretation.Value == "MONOCHROME1")
                            pixVal = max12 - pixVal;
                    }
                    pixels12Int.Add(pixVal);
                }
                int minPixVal = pixels12Int.Min();
                signedImage = false;
                if (minPixVal < 0) signedImage = true;
                // Use the above pixel data to populate the list pixels16 
                foreach (int pixel in pixels12Int)
                {
                  //  We internally convert all 16 - bit images to the range 0 - 65535
                    //if (signedImage)
                    //    pixels16.Add((ushort)(pixel - min16));
                    //else
                    //    pixels16.Add((ushort)(pixel));
                    pixels12.Add((ushort)(pixel - (signedImage ? min12 : 0)));
                }

                pixels12Int.Clear();
            }
            //- to account for Ultrasound images
            if (samplesPerPixel == 3 && bitsStored == 8)
            {
                signedImage = false;
                if (pixels24 != null)
                    pixels24.Clear();
                pixels24 = new List<byte>();
                int numPixels = width * height;

                int numBytes = numPixels * samplesPerPixel;
                byte[] buf = new byte[numBytes];
                file.BaseStream.Position = offset;
                file.Read(buf, 0, numBytes);
                           
                
                for (int i = 0; i < numBytes; ++i)
                {
                    pixels24.Add(buf[i]);
                }
            }
        }
    }
}
