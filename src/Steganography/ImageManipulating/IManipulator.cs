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
        /// <param name="filePath"></param>
        /// <param name="secretPath"></param>
        /// <param name="password"></param>
        /// <param name="mediumExt"></param>
        /// <returns>
        /// Byte array to represents medium file after secret file is added.
        /// </returns>
        Task<byte[]> HideFileIntoMediumAsync(string filePath, string secretPath, string password, string mediumExt);
        /// <summary>
        /// Retrieve and decrypt secret file from medium.
        /// </summary>
        /// <param name="mediumPath"></param>
        /// <param name="password"></param>
        /// <returns>
        /// Secret file extension and byte array represents secret file content.
        /// </returns>
        Task<SecretFileData> GetFileFromMediumAsync(string mediumPath, string password);
    }
}
