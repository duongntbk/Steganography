namespace Steganography.Crypto
{
    /// <summary>
    /// Hash and data generation related method.
    /// </summary>
    public interface IMyHashable
    {
        /// <summary>
        /// Compute the SHA256 hash of a string.
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        byte[] HashPassword(string password);
        /// <summary>
        /// Use PBKDF2 to generate an encryption key from given password and salt.
        /// </summary>
        /// <param name="password"></param>
        /// <param name="salt"></param>
        /// <returns></returns>
        byte[] GenerateEncryptionKey(string password, byte[] salt);
        /// <summary>
        /// Generate cryptographically secure random binary data with given size.
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        byte[] GenerateRandomByteArray(int size);
    }
}
