using ImageViewer.Screens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageViewer
{
    public class LocalizerDelegate
    {
        private SliceLocator locator = new SliceLocator();
      
        public LocalizerDelegate()
        {
            
        }
        public bool projectSlice(MainWindow mv)
        {         
            String scoutPos = mv.isLocalizerPanel.getImagePosition();
            String scoutOrientation = mv.isLocalizerPanel.getImageOrientation();
            String scoutPixelSpacing = mv.isLocalizerPanel.getPixelSpacing();
            int scoutRow = mv.isLocalizerPanel.getRow();
            int scoutColumn = mv.isLocalizerPanel.getColumn();
            String imgPos = mv.selectedImageControl.getImagePosition();
            String imgOrientation = mv.selectedImageControl.getImageOrientation();
            String imgPixelSpacing = mv.selectedImageControl.getPixelSpacing();
            int imgRow = mv.selectedImageControl.getRow();
            int imgColumn = mv.selectedImageControl.getColumn();
            //ScoutLineInfoModel[] borderLineArray = ApplicationContext.imgPanel.getScoutBorder();
            //locator.projectSlice(scoutPos, scoutOrientation, scoutPixelSpacing, scoutRow, scoutColumn, borderLineArray[0].getImagePosition(), borderLineArray[0].getImageOrientation(), borderLineArray[0].getImagePixelSpacing(), borderLineArray[0].getImageRow(), borderLineArray[0].getImageColumn());
            //temp.imgpanel.setScoutBorder1Coordinates((int) locator.getBoxUlx(), (int) locator.getBoxUly(), (int) locator.getBoxLlx(), (int) locator.getBoxLly());
            //temp.imgpanel.setAxis1Coordinates((int) locator.getmAxisLeftx(), (int) locator.getmAxisLefty(), (int) locator.getmAxisRightx(), (int) locator.getmAxisRighty(), (int) locator.getmAxisTopx(), (int) locator.getmAxisTopy(), (int) locator.getmAxisBottomx(), (int) locator.getmAxisBottomy());
            //locator.projectSlice(scoutPos, scoutOrientation, scoutPixelSpacing, scoutRow, scoutColumn, borderLineArray[1].getImagePosition(), borderLineArray[1].getImageOrientation(), borderLineArray[1].getImagePixelSpacing(), borderLineArray[1].getImageRow(), borderLineArray[1].getImageColumn());
            //temp.imgpanel.setScoutBorder2Coordinates((int) locator.getBoxUlx(), (int) locator.getBoxUly(), (int) locator.getBoxLlx(), (int) locator.getBoxLly());
            //temp.imgpanel.setAxis2Coordinates((int) locator.getmAxisLeftx(), (int) locator.getmAxisLefty(), (int) locator.getmAxisRightx(), (int) locator.getmAxisRighty(), (int) locator.getmAxisTopx(), (int) locator.getmAxisTopy(), (int) locator.getmAxisBottomx(), (int) locator.getmAxisBottomy());
            locator.projectSlice(scoutPos, scoutOrientation, scoutPixelSpacing, scoutRow, scoutColumn, imgPos, imgOrientation, imgPixelSpacing, imgRow, imgColumn);
            // tempPanel.setScoutCoordinates((int)locator.getBoxUlx(), (int)locator.getBoxUly(), (int)locator.getBoxLlx(), (int)locator.getBoxLly(), (int)locator.getBoxUrx(), (int)locator.getBoxUry(), (int)locator.getBoxLrx(), (int)locator.getBoxLry());
            mv.isLocalizerPanel.setAxisCoordinates((int)locator.getmAxisLeftx(), (int)locator.getmAxisLefty(), (int)locator.getmAxisRightx(), (int)locator.getmAxisRighty(), (int)locator.getmAxisTopx(), (int)locator.getmAxisTopy(), (int)locator.getmAxisBottomx(), (int)locator.getmAxisBottomy());
            return true;
        }
    }
}
