using System.Collections;
using System.Drawing;

namespace Steganography.ImageManipulating
{
    /// <summary>
    /// Method to insert and retrieve data from least significant bit of pixel.
    /// </summary>
    public interface IPixel
    {
        /// <summary>
        /// Retrieve data from least significant bit of pixel.
        /// </summary>
        /// <param name="color"></param>
        /// <returns>
        /// A bit array with length of 3 storing data,
        /// retrieve from least significant bit of color R, G and B.
        /// </returns>
        BitArray GetHiddenData(Color color);
        /// <summary>
        /// Insert data into least significant bit of pixel.
        /// </summary>
        /// <param name="color"></param>
        /// <param name="array"></param>
        /// <returns>
        /// Color with new R, G and B value after inserting 3 bit.
        /// </returns>
        Color SetHiddenData(Color color, BitArray array);
    }
}
