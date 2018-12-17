namespace Steganography
{
    /// <summary>
    /// Secret file modal.
    /// </summary>
    public class SecretFileData
    {
        /// <summary>
        /// Byte array to store contain of secret file.
        /// </summary>
        public byte[] Data { get; set; }
        /// <summary>
        /// Extension of secret file.
        /// </summary>
        public string Extension { get; set; }
    }
}
