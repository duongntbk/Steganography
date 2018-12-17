namespace Steganography.Crypto
{
    /// <summary>
    /// Method to encrypt and decrypt data with a given encryption.
    /// </summary>
    public interface IMyEncryptable
    {
        /// <summary>
        /// Encrypt text with a given encryption.
        /// </summary>
        /// <param name="plainText"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        byte[] Encrypt(string plainText, byte[] key, byte[] iv);
        /// <summary>
        /// Encrypt number with a given encryption.
        /// </summary>
        /// <param name="plainLong"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        byte[] Encrypt(long plainLong, byte[] key, byte[] iv);
        /// <summary>
        /// Encryption binary data with a given encryption.
        /// </summary>
        /// <param name="plainData"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        byte[] Encrypt(byte[] plainData, byte[] key, byte[] iv);
        /// <summary>
        /// Decrypt text with a given encryption, key and iv. 
        /// </summary>
        /// <param name="cipherText"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        string DecryptString(byte[] cipherText, byte[] key, byte[] iv);
        /// <summary>
        /// Decrypt number with a given encryption, key and iv. 
        /// </summary>
        /// <param name="cipherLong"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        long DecryptLong(byte[] cipherLong, byte[] key, byte[] iv);
        /// <summary>
        /// Decrypt binary data with a given encryption, key and iv. 
        /// </summary>
        /// <param name="cipher"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        byte[] DecryptBytes(byte[] cipher, byte[] key, byte[] iv);
    }
}
