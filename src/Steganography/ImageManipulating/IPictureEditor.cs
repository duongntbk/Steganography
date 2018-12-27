namespace Steganography.ImageManipulating
{
    /// <summary>
    /// Method to insert and retrieve data from picture.
    /// </summary>
    public interface IPictureEditor
    {
        /// <summary>
        /// Read image file form disk and get image's data, size, width and height.
        /// </summary>
        /// <param name="mediumData"></param>
        /// <returns></returns>
        void LoadMedium(byte[] mediumData);
        /// <summary>
        /// Verify if medium is big enough to hide secret file.
        /// </summary>
        /// <param name="fileSize"></param>
        /// <returns></returns>
        bool CheckHiddenFileSize(int fileSize);
        /// <summary>
        /// Hide data into image, starting from pixel at offset position.
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="size"></param>
        /// <param name="offset"></param>
        /// <param name="padding"></param>
        void SetBytes(byte[] bytes, int size, int offset, int padding);
        /// <summary>
        /// Convert Bitmap object into byte array to be saved to disk.
        /// </summary>
        /// <param name="extension"></param>
        /// <returns></returns>
        byte[] GetImageData(string extension);
        /// <summary>
        /// Get data from image, starting from given index position with given size.
        /// </summary>
        /// <param name="size"></param>
        /// <param name="offset"></param>
        /// <param name="padding"></param>
        /// <returns></returns>
        byte[] GetBytes(long size, int offset, int padding);
    }
}
