using Steganography.Crypto;
using System.Threading.Tasks;

namespace Steganography.ImageManipulating
{
    /// <summary>
    /// Method to insert and retrieve data from medium.
    /// </summary>
    public interface IManipulator
    {
        IMyEncryptable Encryptor { set; }
        /// <summary>
        /// Hide file in to medium.
        /// </summary>
        /// <param name="fileData"></param>
        /// <param name="secretData"></param>
        /// <param name="secretExt"></param>
        /// <param name="password"></param>
        /// <param name="outputExt"></param>
        /// <returns>
        /// Byte array to represents medium file after secret file is added.
        /// </returns>
        Task<byte[]> HideFileIntoMediumAsync(byte[] fileData, byte[] secretData,
            string secretExt, string password, string outputExt);
        /// <summary>
        /// Retrieve and decrypt secret file from medium.
        /// </summary>
        /// <param name="mediumData"></param>
        /// <param name="password"></param>
        /// <returns>
        /// Secret file extension and byte array represents secret file content.
        /// </returns>
        Task<SecretFileData> GetFileFromMediumAsync(byte[] mediumData, string password);
    }
}
