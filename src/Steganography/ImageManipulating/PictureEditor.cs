using System.Collections.Generic;
using System.Drawing;
using System.Collections;
using System.IO;
using System.Drawing.Imaging;
using System;

namespace Steganography.ImageManipulating
{
    /// <summary>
    /// Provides means to hide and retrieve data from image file.
    /// </summary>
    public class PictureEditor : IPictureEditor
    {
        private long _size;
        private int _width, _height;
        private Bitmap _img;
        private IPixel _pixelEditor;

        public IPixel PixelEditor
        {
            set => _pixelEditor = value;
        }

        /// <summary>
        /// Read image file from binary and get image's data, size, width and height.
        /// </summary>
        public void LoadMedium(byte[] fileData)
        {
            _size = fileData.Length;
            _img = new Bitmap(new MemoryStream(fileData));
            var format = _img.RawFormat.Guid;

            if (format != ImageFormat.Png.Guid && format != ImageFormat.Bmp.Guid)
            {
                throw new FormatException("Image must be either in bmp or png format.");
            }

            _width = _img.Width;
            _height = _img.Height;
            if (format == ImageFormat.Png.Guid)
            {
                _img = DeleteTransparent();
            }
        }

        /// <summary>
        /// Hide data into image, starting from pixel at offset position.
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="size"></param>
        /// <param name="offset"></param>
        /// <param name="padding"></param>
        public void SetBytes(byte[] bytes, int size, int offset, int padding)
        {
            var bit = new BitArray(bytes);
            for (var i = 0; i < size; i++)
            {
                var array = new BitArray(3);
                // If this is not the last pixel, or if we don't need padding.
                if ((i < size - 1) || (padding == 0))
                {
                    // Hide data into least significant bit of R, G and B.
                    array[0] = bit[i * 3 + 0];
                    array[1] = bit[i * 3 + 1];
                    array[2] = bit[i * 3 + 2];
                }
                // Pad 1 last bit of last pixel.
                else if (padding == 1)
                {
                    array[0] = bit[i * 3 + 0];
                    array[1] = bit[i * 3 + 1];
                    array[2] = false; // Least significant bit of B color is padded to 0.
                }
                // Pad 2 last bits of last pixel.
                else
                {
                    array[0] = bit[i * 3 + 0];
                    // Least significant bit of G and B color is padded to 0.
                    array[1] = false;
                    array[2] = false;
                }
                
                var x = ConvertIndexToCoordinate(i + offset).Item1;
                var y = ConvertIndexToCoordinate(i + offset).Item2;
                var color = _img.GetPixel(x, y);
                _img.SetPixel(x, y, _pixelEditor.SetHiddenData(color, array));
            }
        }

        /// <summary>
        /// Get data from image, starting from given index position with given size.
        /// </summary>
        /// <param name="size"></param>
        /// <param name="offset"></param>
        /// <param name="padding"></param>
        /// <returns></returns>
        public byte[] GetBytes(long size, int offset, int padding)
        {
            var trioList = new List<bool>();
            for (var i = 0; i < size; i++)
            {
                var x = ConvertIndexToCoordinate(i + offset).Item1;
                var y = ConvertIndexToCoordinate(i + offset).Item2;

                var color = _img.GetPixel(x, y);
                // If this is not the last pixel, or if we don't need padding.
                if ((i < size - 1) || (padding == 0))
                {
                    // Get least significant bit of all 3 colors.
                    trioList.Add(_pixelEditor.GetHiddenData(color)[0]);
                    trioList.Add(_pixelEditor.GetHiddenData(color)[1]);
                    trioList.Add(_pixelEditor.GetHiddenData(color)[2]);
                }
                // Pad 1 last bit of last pixel.
                else if (padding == 1)
                {
                    // Only retrieve least significant bit of R and B.
                    trioList.Add(_pixelEditor.GetHiddenData(color)[0]);
                    trioList.Add(_pixelEditor.GetHiddenData(color)[1]);
                }
                // Pad 1 last bits of last pixel.
                else
                {
                    // Only retrieve least significant bit of R.
                    trioList.Add(_pixelEditor.GetHiddenData(color)[0]);
                }
            }
            var arr = new BitArray(trioList.ToArray());
            return ConvertBitArrayToByte(arr);
        }

        /// <summary>
        /// Convert Bitmap object into byte array to be saved to disk.
        /// </summary>
        /// <param name="extension"></param>
        /// <returns></returns>
        public byte[] GetImageData(string extension)
        {
            extension = extension.ToLower();
            // Right now only BMP and PNG extension is supported.
            if (extension != Constants.PngExtension && extension != Constants.BmpExtension)
            {
                throw new ArgumentException("Please save image with steganography data in PNG or BMP format.");
            }

            using (var stream = new MemoryStream())
            {
                var imageFormat = extension == Constants.PngExtension ? ImageFormat.Png : ImageFormat.Bmp;
                _img.Save(stream, imageFormat);
                return stream.ToArray();
            }
        }

        /// <summary>
        /// Verify if medium is big enough to hide secret file.
        /// </summary>
        /// <param name="fileSize"></param>
        /// <returns></returns>
        public bool CheckHiddenFileSize(int fileSize)
        {
            // For each pixel (8 bit), 
            // the least significant bit in each color is used to stored hidden data,
            return fileSize <= _size / 8;
        }

        /// <summary>
        /// Delete transparent layer from png file,
        /// or else it will mess with result image.
        /// </summary>
        /// <returns></returns>
        private Bitmap DeleteTransparent()
        {
            var temp = new Bitmap(_width, _height, PixelFormat.Format24bppRgb);
            var g = Graphics.FromImage(temp);
            g.Clear(Color.Transparent);
            g.DrawImage(_img, Point.Empty);
            return temp;
        }

        /// <summary>
        /// Convert array of bit to byte array.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private byte[] ConvertBitArrayToByte(BitArray input)
        {
            var ret = new byte[(input.Length - 1) / 8 + 1];
            input.CopyTo(ret, 0);
            return ret;
        }

        /// <summary>
        /// Convert index of pixel (base 0) into horizontal and vertical resolution value.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private Tuple<int, int> ConvertIndexToCoordinate(int index)
        {
            if (index > _height * _width)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Pixel index is invalid!");
            }
            var x = index % _width;
            var y = (index - x) / _width;
            return new Tuple<int, int>(x, y);
        }
    }
}
