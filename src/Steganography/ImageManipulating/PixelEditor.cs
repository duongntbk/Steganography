using System.Drawing;
using System.Collections;

namespace Steganography.ImageManipulating
{
    /// <summary>
    /// Hide and retrieve data from pixel.
    /// </summary>
    public class PixelEditor : IPixel
    {
        private const int LastBit = 0;

        /// <summary>
        /// Retrieve data from least significant bit of pixel.
        /// </summary>
        /// <param name="color"></param>
        /// <returns>
        /// A bit array with length of 3 storing data,
        /// retrieve from least significant bit of color R, G and B.
        /// </returns>
        public BitArray GetHiddenData(Color color)
        {
            return new BitArray(3)
            {
                [0] = (color.R & (1 << LastBit)) != 0,
                [1] = (color.G & (1 << LastBit)) != 0,
                [2] = (color.B & (1 << LastBit)) != 0
            };
        }

        /// <summary>
        /// Insert data into least significant bit of pixel.
        /// </summary>
        /// <param name="color"></param>
        /// <param name="array"></param>
        /// <returns>
        /// Color with new R, G and B value after inserting 3 bit.
        /// </returns>
        public Color SetHiddenData(Color color, BitArray array)
        {
            var r = color.R;
            var g = color.G;
            var b = color.B;

            r = SetBit(r, LastBit, array[Constants.Pixel.BitR]);
            g = SetBit(g, LastBit, array[Constants.Pixel.BitG]);
            b = SetBit(b, LastBit, array[Constants.Pixel.BitB]);

            return Color.FromArgb(r, g, b);
        }

        /// <summary>
        /// Set the bit at "pos" position of input byte with given value.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="pos"></param>
        /// <param name="value"></param>
        /// <returns>
        /// Input byte after modification.
        /// </returns>
        private byte SetBit(byte input, int pos, bool value)
        {
            if (value)
            {
                // Left-shift 1, then bitwise OR
                return (byte)(input | (1 << pos));
            }
            // Left-shift 1, then take complement, then bitwise AND
            return (byte)(input & ~(1 << pos));
        }
    }
}
