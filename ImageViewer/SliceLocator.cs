using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ImageViewer
{
    
public class SliceLocator
        {
            private float _scoutRowCosx = 0;
            private float _scoutRowCosy = 0;
            private float _scoutRowCosz = 0;
            private float _scoutColCosx = 0;
            private float _scoutColCosy = 0;
            private float _scoutColCosz = 0;
            private float _scoutx = 0;
            private float _scouty = 0;
            private float _scoutz = 0;
            private float _imgRowCosx = 0;
            private float _imgRowCosy = 0;
            private float _imgRowCosz = 0;
            private float _imgColCosx = 0;
            private float _imgColCosy = 0;
            private float _imgColCosz = 0;
            private float _imgx = 0;
            private float _imgy = 0;
            private float _imgz = 0;
            private float _nrmCosX = 0;
            private float _nrmCosY = 0;
            private float _nrmCosZ = 0;
            private float _scoutxSpacing = 0;
            private float _scoutySpacing = 0;
            private float _imgxSpacing = 0;
            private float _imgySpacing = 0;
            private bool _scoutValid = false;
            private bool _imgValid = false;
            private float _scoutPosX = 0;
            private float _scoutPosY = 0;
            private float _scoutPosZ = 0;
            private float _scoutRowLen = 0;
            private float _scoutColLen = 0;
            private float _scoutRows = 0;
            private float _scoutCols = 0;
            private float _imgRowLen = 0;
            private float _imgColLen = 0;
            private float _imgRows = 0;
            private float _imgCols = 0;
            private float _boxUlx;
            private float _boxUly;
            private float _boxUrx;
            private float _boxUry;
            private float _boxLrx;
            private float _boxLry;
            private float _boxLlx;
            private float _boxLly;
            private float _mAxisTopx;
            private float _mAxisTopy;
            private float _mAxisRightx;
            private float _mAxisRighty;
            private float _mAxisBottomx;
            private float _mAxisBottomy;
            private float _mAxisLeftx;
            private float _mAxisLefty;

            public SliceLocator()
            {
            }

            private bool setScoutPosition(String scoutPosition)
            {
                bool retVal = true;
                string [] _scoutPositionArray;
                // set the member variables
                // _mScoutx, _mScouty, _mScoutz to the actual position information
                // if the input pointer is null or the string is empty set all position to 0 and
                // set the position valid flag (_mScoutvalid) to false
                retVal = checkPosString(scoutPosition);
                if (retVal)
                {
                    _scoutPositionArray = scoutPosition.Split('\\');
                    _scoutx = float.Parse(_scoutPositionArray[0]);
                    _scouty = float.Parse(_scoutPositionArray[1]);
                    _scoutz = float.Parse(_scoutPositionArray[2]);
                    _scoutValid = true;
                }
                else
                {
                    // The Pos contains no valid information it is assumed that the sout position is to be clweared of all valid
                    // entries
                    clearScout();
                }
                return (retVal);
            }

            private bool setScoutOrientation(String scoutOrientation)
            {
                bool retVal;
                String [] scoutOrientationArray;
                // Scna the sout orientation vactor into the local variables and chack it for validity
                retVal = checkVectorString(scoutOrientation);
                if (retVal)
                {
                    scoutOrientationArray = scoutOrientation.Split('\\');
                _scoutRowCosx = float.Parse(scoutOrientationArray[0]);
                    _scoutRowCosy = float.Parse(scoutOrientationArray[1]);
                    _scoutRowCosz = float.Parse(scoutOrientationArray[2]);
                    _scoutColCosx = float.Parse(scoutOrientationArray[3]);
                    _scoutColCosy = float.Parse(scoutOrientationArray[4]);
                    _scoutColCosz = float.Parse(scoutOrientationArray[5]);
                    _scoutValid = checkScoutVector();
                    if (!_scoutValid)
                    {
                        clearScout();
                    }
                }
                return (retVal);
            }

            private void clearScout()
            {
                // clear all the scout parameters and set the scout valid flag to false.
                _scoutRowCosx = 0;
                _scoutRowCosy = 0;
                _scoutRowCosz = 0;
                _scoutColCosx = 0;
                _scoutColCosy = 0;
                _scoutColCosz = 0;
                _scoutx = 0;
                _scouty = 0;
                _scoutz = 0;
                _scoutValid = false;
            }

            private void clearImg()
            {
                // clear all the image prameters and set the image valid flag to false.
                _imgRowCosx = 0;
                _imgRowCosy = 0;
                _imgRowCosz = 0;
                _imgColCosx = 0;
                _imgColCosy = 0;
                _imgColCosz = 0;
                _imgx = 0;
                _imgy = 0;
                _imgz = 0;
                _imgValid = false;
            }

            private bool setImgPosition(String imagePosition)
            {
                bool retVal = true;
                string [] _imgPositionArray;
                // set the member variables
                // _mImgx, _mImgy, _mImgz to the actual position information
                // if the input pointer is null or the string is empty set all position to 0 and
                // set the position valid flag (_mImgvalid) to false
                retVal = checkPosString(imagePosition);
                if (retVal)
                {
                    // the position information contains valid data.  It has been checked prior to
                    // the activation of theis member function
                    _imgPositionArray = imagePosition.Split('\\');
                    _imgx = float.Parse(_imgPositionArray[0]);
                    _imgy = float.Parse(_imgPositionArray[1]);
                    _imgz = float.Parse(_imgPositionArray[2]);

                    _imgValid = true;
                }
                else
                {
                    // The Pos contains no valid information it is assumed that the sout position is to be clweared of all valid
                    // entries
                    clearImg();
                }
                return (retVal);
            }

            private bool setImgOrientation(String imageOrientation)
            {
                bool retVal;
                string [] imageOrientationArray;
                // Scna the sout orientation vactor into the local variables and chack it for validity
                retVal = checkVectorString(imageOrientation);
                if (retVal)
                {
                    imageOrientationArray = imageOrientation.Split('\\');
                   _imgRowCosx = float.Parse(imageOrientationArray[0]);
                    _imgRowCosy = float.Parse(imageOrientationArray[1]);
                    _imgRowCosz = float.Parse(imageOrientationArray[2]);
                    _imgColCosx = float.Parse(imageOrientationArray[3]);
                    _imgColCosy = float.Parse(imageOrientationArray[4]);
                    _imgColCosz = float.Parse(imageOrientationArray[5]);
                    _imgValid = checkImgVector();
                    if (!_imgValid)
                    {
                        clearImg();
                    }
                }
                return (retVal);
            }

            private bool checkScoutVector()
            {
                bool retVal;
                // check the row vector and check the column vector
                retVal = checkVector(_scoutRowCosx, _scoutRowCosy, _scoutRowCosz);
                retVal = checkVector(_scoutColCosx, _scoutColCosy, _scoutColCosz);
                return (retVal);
            }
            private bool checkImgVector()
            {
                bool retVal;
                // check the row vector and check the column vector
                retVal = checkVector(_imgRowCosx, _imgRowCosy, _imgRowCosz);
                retVal = checkVector(_imgColCosx, _imgColCosy, _imgColCosz);
                return (retVal);
            }
            private bool checkPosString(String position)              
                   {
                bool retVal = true;
                String[] positionArray;
                positionArray = position.Split('\\');
            Regex expression = new Regex(@"((-|\\+)?[0-9]+(\\.[0-9]+)?)+");
         
            for (int i = 0; i < positionArray.Length; i++)
                {
                var resultdf = expression.Matches(positionArray[i]);

                //if (positionArray[i].SequenceEqual("((-|\\+)?[0-9]+(\\.[0-9]+)?)+"))
                //{
                //    retVal = true;
                //}
                //if(expression.Matches()
                //else
                //{
                //    retVal = false;
                //}

                if (resultdf != null)
                {
                    retVal = true;
                }
                else
                {
                    retVal = false;
                }


            }
                //I just want to check the existance of the numeric values in the position.
                return retVal;
            }

            private bool checkVectorString(String vector)
            {
                bool retVal = true;
                String[] vectorArray;
                vectorArray = vector.Split('\\');
            Regex expression = new Regex(@"((-|\\+)?[0-9]+(\\.[0-9]+)?)+");
            for (int i = 0; i < vectorArray.Length; i++)
                {
                //if (vectorArray[i].SequenceEqual("((-|\\+)?[0-9]+(\\.[0-9]+)?)+"))
                //{
                //    retVal = true;
                //}
                //else
                //{
                //    retVal = false;
                //}

                var resultdfs = expression.Matches(vectorArray[i]);

               
                if (resultdfs != null)
                {
                    retVal = true;
                }
                else
                {
                    retVal = false;
                }
            }
                //I just want to check the existance of the vector string.
                return retVal;
            }

            private bool checkVector(float CosX, float CosY, float CosZ)
            {
                // Check if the vector passed is a unit vector
                if (Math.Abs(CosX * CosX + CosY * CosY + CosZ * CosZ - 1) < 0)
                {
                    return (false);
                }
                else
                {
                    return (true);
                }
            }

            private bool normalizeScout()
            {
                // first create the scout normal vector
                _nrmCosX = _scoutRowCosy * _scoutColCosz - _scoutRowCosz * _scoutColCosy;
                _nrmCosY = _scoutRowCosz * _scoutColCosx - _scoutRowCosx * _scoutColCosz;
                _nrmCosZ = _scoutRowCosx * _scoutColCosy - _scoutRowCosy * _scoutColCosx;
                return (checkVector(_nrmCosX, _nrmCosY, _nrmCosZ));
            }

            private bool setScoutSpacing(String scoutPixelSpacing)
            {
                //  Convert the pixelspacing for the scout image and return true if both values are > 0
                // the pixel spacing is specified in adjacent row/adjacent column spacing
                // in this code ..xSpacing refers to column spacing
                String[] scoutPixelSpacingArray;
                scoutPixelSpacingArray = scoutPixelSpacing.Split('\\');
            _scoutxSpacing = float.Parse(scoutPixelSpacingArray[1]);
                _scoutySpacing = float.Parse(scoutPixelSpacingArray[0]);
                if (_scoutxSpacing == 0 || _scoutySpacing == 0)
                {
                    return (false);
                }
                else
                {
                    return (true);
                }
            }
            private bool setImgSpacing(String imgPixelSpacing)
            {
                //  Convert the pixelspacing for the Img image and return true if both values are > 0
                // the pixel spacing is specified in adjacent row/adjacent column spacing
                // in this code ..xSpacing ref
                //
                //
                // ers to column spacing
                String[] imgPixelSpacingArray;
                imgPixelSpacingArray = imgPixelSpacing.Split('\\');

            _imgxSpacing = float.Parse(imgPixelSpacingArray[1]);
                _imgySpacing = float.Parse(imgPixelSpacingArray[0]);
                if (_imgxSpacing == 0 || _imgySpacing == 0)
                {
                    return (false);
                }
                else
                {
                    return (true);
                }
            }

            private bool rotateImage(float imgPosx, float imgPosy, float imgPosz)
            {
                // projet the pioints passed into the space of the normalized scout image

                _scoutPosX = _scoutRowCosx * imgPosx + _scoutRowCosy * imgPosy + _scoutRowCosz * imgPosz;
                _scoutPosY = _scoutColCosx * imgPosx + _scoutColCosy * imgPosy + _scoutColCosz * imgPosz;
                _scoutPosZ = _nrmCosX * imgPosx + _nrmCosY * imgPosy + _nrmCosZ * imgPosz;
                return (true);
            }

            private bool setScoutDimensions()
            {
                _scoutRowLen = _scoutRows * _scoutxSpacing;
                _scoutColLen = _scoutCols * _scoutySpacing;
                return (true);
            }

            private bool setImgDimensions()
            {
                _imgRowLen = _imgRows * _imgxSpacing;
                _imgColLen = _imgCols * _imgySpacing;
                return (true);
            }

            private void calculateBoundingBox()
            {
                // the four points in 3d space that defines the corners of the bounding box
                float[] posX = new float[4];
                float [] posY = new float[4];
                float []posZ = new float[4];
                int []rowPixel = new int[4];
                int []colPixel = new int[4];
                int i;

                // upper left hand Corner
                posX[0] = _imgx;
                posY[0] = _imgy;
                posZ[0] = _imgz;

                // upper right hand corner

                posX[1] = posX[0] + _imgRowCosx * _imgRowLen;
                posY[1] = posY[0] + _imgRowCosy * _imgRowLen;
                posZ[1] = posZ[0] + _imgRowCosz * _imgRowLen;

                // Buttom right hand corner

                posX[2] = posX[1] + _imgColCosx * _imgColLen;
                posY[2] = posY[1] + _imgColCosy * _imgColLen;
                posZ[2] = posZ[1] + _imgColCosz * _imgColLen;

                // bottom left hand corner

                posX[3] = posX[0] + _imgColCosx * _imgColLen;
                posY[3] = posY[0] + _imgColCosy * _imgColLen;
                posZ[3] = posZ[0] + _imgColCosz * _imgColLen;

                // Go through all four corners

                for (i = 0; i < 4; i++)
                {
                    // we want to view the source slice from the "point of view" of
                    // the target localizer, i.e. a parallel projection of the source
                    // onto the target

                    // do this by imaging that the target localizer is a view port
                    // into a relocated and rotated co-ordinate space, where the
                    // viewport has a row vector of +X, col vector of +Y and normal +Z,
                    // then the X and Y values of the projected target correspond to
                    // row and col offsets in mm from the TLHC of the localizer image !

                    // move everything to origin of target
                    posX[i] -= _scoutx;
                    posY[i] -= _scouty;
                    posZ[i] -= _scoutz;

                    rotateImage(posX[i], posY[i], posZ[i]);
                    // at this point the position contains the location on the scout image. calculate the pixel position
                    // dicom coordinates are center of pixel 1\1
                    colPixel[i] = (int)(_scoutPosX / _scoutySpacing + 0.5);
                    rowPixel[i] = (int)(_scoutPosY / _scoutxSpacing + 0.5);
                }
                //  sort out the column and row pixel coordinates into the bounding box named coordinates
                //  same order as the position ULC -> URC -> BRC -> BLC
                _boxUlx = colPixel[0];
                _boxUly = rowPixel[0];
                _boxUrx = colPixel[1];
                _boxUry = rowPixel[1];
                _boxLrx = colPixel[2];
                _boxLry = rowPixel[2];
                _boxLlx = colPixel[3];
                _boxLly = rowPixel[3];
            }

            public bool projectSlice(String scoutPos, String scoutOrient, String scoutPixSpace,
                    int scoutRows,
                    int scoutCols,
                    String imgPos,
                    String imgOrient,
                    String imgPixSpace,
                    int imgRows,
                    int imgCols)
            {
                bool retVal = true;

                // Fisrs step check if either the scout or the image has to be updated.

                if (scoutPos != null && scoutOrient != null && scoutPixSpace != null && scoutRows != -1 && scoutCols != -1)
                {
                    // scout parameters appear to be semi-valid try to update the scout information
                    if (setScoutPosition(scoutPos) && setScoutOrientation(scoutOrient) && setScoutSpacing(scoutPixSpace))
                    {
                        _scoutRows = scoutRows;
                        _scoutCols = scoutCols;
                        setScoutDimensions();
                        retVal = normalizeScout();
                    }
                }
                //  Image and scout information is independent of one and other
                if (imgPos != null && imgOrient != null && imgPixSpace != null && imgRows != -1 && imgCols != -1)
                {
                    // Img parameters appear to be semi-valid try to update the Img information
                    if (setImgPosition(imgPos) && setImgOrientation(imgOrient) && setImgSpacing(imgPixSpace))
                    {
                        _imgRows = imgRows;
                        _imgCols = imgCols;
                        setImgDimensions();
                    }
                }
                if (retVal)
                {
                    // start the calculation of the projected bounding box and the ends of the axes along the sides.
                    calculateBoundingBox();
                    calculateAxisPoints();
                }
                return (retVal);
            }
            private void calculateAxisPoints()
            {
                // the four points in 3d space that defines the corners of the bounding box
                float []posX = new float[4];
                float []posY = new float[4];
                float[] posZ = new float[4];
                int []rowPixel = new int[4];
                int []colPixel = new int[4];
                int i;


                // upper center
                posX[0] = _imgx + _imgRowCosx * _imgRowLen / 2;
                posY[0] = _imgy + _imgRowCosy * _imgRowLen / 2;
                posZ[0] = _imgz + _imgRowCosz * _imgRowLen / 2;

                // right hand center
                posX[1] = _imgx + _imgRowCosx * _imgRowLen + _imgColCosx * _imgColLen / 2;              
                posY[1] = _imgy + _imgRowCosy * _imgRowLen + _imgColCosy * _imgColLen / 2;
                posZ[1] = _imgz + _imgRowCosz * _imgRowLen + _imgColCosz * _imgColLen / 2;
                // Buttom center
                posX[2] = posX[0] + _imgColCosx * _imgColLen;
                posY[2] = posY[0] + _imgColCosy * _imgColLen;
                posZ[2] = posZ[0] + _imgColCosz * _imgColLen;

                // left hand center

                posX[3] = _imgx + _imgColCosx * _imgColLen / 2;
                posY[3] = _imgy + _imgColCosy * _imgColLen / 2;
                posZ[3] = _imgz + _imgColCosz * _imgColLen / 2;

                // Go through all four corners

                for (i = 0; i < 4; i++)
                {
                    // we want to view the source slice from the "point of view" of
                    // the target localizer, i.e. a parallel projection of the source
                    // onto the target
                    // do this by imaging that the target localizer is a view port
                    // into a relocated and rotated co-ordinate space, where the
                    // viewport has a row vector of +X, col vector of +Y and normal +Z,
                    // then the X and Y values of the projected target correspond to
                    // row and col offsets in mm from the TLHC of the localizer image !
                    // move everything to origin of target
                    posX[i] -= _scoutx;
                    posY[i] -= _scouty;
                    posZ[i] -= _scoutz;
                    rotateImage(posX[i], posY[i], posZ[i]);
                    // at this point the position contains the location on the scout image. calculate the pixel position
                    // dicom coordinates are center of pixel 1\1
                    colPixel[i] = (int)(_scoutPosX / _scoutySpacing + 0.5);
                    rowPixel[i] = (int)(_scoutPosY / _scoutxSpacing + 0.5);
                }
                // sort out the column and row pixel coordinates into the bounding box axis named coordinates
                // same order as the position top -> right -> bottom -> left
                _mAxisTopx = colPixel[0];
                _mAxisTopy = rowPixel[0];
                _mAxisRightx = colPixel[1];
                _mAxisRighty = rowPixel[1];
                _mAxisBottomx = colPixel[2];
                _mAxisBottomy = rowPixel[2];
                _mAxisLeftx = colPixel[3];
                _mAxisLefty = rowPixel[3];
            }

            public float getBoxLlx()
            {
                return _boxLlx;
            }

            public float getBoxLly()
            {
                return _boxLly;
            }

            public float getBoxLrx()
            {
                return _boxLrx;
            }

            public float getBoxLry()
            {
                return _boxLry;
            }

            public float getBoxUlx()
            {
                return _boxUlx;
            }

            public float getBoxUly()
            {
                return _boxUly;
            }
            public float getBoxUrx()
            {
                return _boxUrx;
            }
            public float getBoxUry()
            {
                return _boxUry;
            }
            public float getmAxisBottomx()
            {
                return _mAxisBottomx;
            }

            public float getmAxisBottomy()
            {
                return _mAxisBottomy;
            }

            public float getmAxisLeftx()
            {
                return _mAxisLeftx;
            }

            public float getmAxisLefty()
            {
                return _mAxisLefty;
            }

            public float getmAxisRightx()
            {
                return _mAxisRightx;
            }

            public float getmAxisRighty()
            {
                return _mAxisRighty;
            }

            public float getmAxisTopx()
            {
                return _mAxisTopx;
            }

            public float getmAxisTopy()
            {
                return _mAxisTopy;
            }
        }

    
}














