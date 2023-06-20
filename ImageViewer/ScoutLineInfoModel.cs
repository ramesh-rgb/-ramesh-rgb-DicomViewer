using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageViewer
{
    public class ScoutLineInfoModel
    {
        private String scoutPosition;
        private String scoutOrientation;
        private String scoutPixelSpacing;
        private String scoutRow;
        private String scoutColumn;
        private String scoutFrameofReferenceUID;
        private String imagePosition;
        private String imageOrientation;
        private String imagePixelSpacing;
        private int imageRow;
        private int imageColumn;
        private String imageFrameofReferenceUID;
        private String imageReferenceSOPInstanceUID;

        public ScoutLineInfoModel()
        {
        }
        public String getImageFrameofReferenceUID()
        {
            return imageFrameofReferenceUID;
        }
        public void setImageFrameofReferenceUID(String imageFrameofReferenceUID)
        {
            this.imageFrameofReferenceUID = imageFrameofReferenceUID;
        }
        public String getImageOrientation()
        {
            return imageOrientation;
        }
        public void setImageOrientation(String imageOrientation)
        {
            this.imageOrientation = imageOrientation;
        }
        public String getImagePixelSpacing()
        {
            return imagePixelSpacing;
        }
        public void setImagePixelSpacing(String imagePixelSpacing)
        {
            this.imagePixelSpacing = imagePixelSpacing;
        }
        public String getImagePosition()
        {
            return imagePosition;
        }
        public void setImagePosition(String imagePosition)
        {
            this.imagePosition = imagePosition;
        }
        public String getImageReferenceSOPInstanceUID()
        {
            return imageReferenceSOPInstanceUID;
        }
        public void setImageReferenceSOPInstanceUID(String imageReferenceSOPInstanceUID)
        {
            this.imageReferenceSOPInstanceUID = imageReferenceSOPInstanceUID;
        }
        public String getScoutColumn()
        {
            return scoutColumn;
        }
        public void setScoutColumn(String scoutColumn)
        {
            this.scoutColumn = scoutColumn;
        }
        public String getScoutFrameofReferenceUID()
        {
            return scoutFrameofReferenceUID;
        }
        public void setScoutFrameofReferenceUID(String scoutFrameofReferenceUID)
        {           
            this.scoutFrameofReferenceUID = scoutFrameofReferenceUID;
        }

        public String getScoutOrientation()
        {
            return scoutOrientation;
        }

        public void setScoutOrientation(String scoutOrientation)
        {
            this.scoutOrientation = scoutOrientation;
        }

        public String getScoutPixelSpacing()
        {
            return scoutPixelSpacing;
        }

        public void setScoutPixelSpacing(String scoutPixelSpacing)
        {
            this.scoutPixelSpacing = scoutPixelSpacing;
        }

        public String getScoutPosition()
        {
            return scoutPosition;
        }

        public void setScoutPosition(String scoutPosition)
        {
            this.scoutPosition = scoutPosition;
        }

        public String getScoutRow()
        {
            return scoutRow;
        }

        public void setScoutRow(String scoutRow)
        {
            this.scoutRow = scoutRow;
        }

        public int getImageColumn()
        {
            return imageColumn;
        }

        public void setImageColumn(int imageColumn)
        {
            this.imageColumn = imageColumn;
        }

        public int getImageRow()
        {
            return imageRow;
        }

        public void setImageRow(int imageRow)
        {
            this.imageRow = imageRow;
        }
    }

}
