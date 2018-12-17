using System;
using System.Text;

namespace Steganography.Crypto
{
    /// <summary>
    /// Implement of IMyEncryptable without using any encryption.
    /// </summary>
    public class NoEncrypt : IMyEncryptable
    {
        /// <summary>
        /// This method return the input binary data without modification.
        /// </summary>
        /// <param name="cipher"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        public byte[] DecryptBytes(byte[] cipher, byte[] key, byte[] iv)
        {
            return cipher;
        }

        /// <summary>
        /// This method return the input number without modification.
        /// </summary>
        /// <param name="cipherLong"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        public long DecryptLong(byte[] cipherLong, byte[] key, byte[] iv)
        {
            var cipherLength = cipherLong.Length;
            return BitConverter.ToInt64(cipherLong, cipherLength - 8);
        }

        /// <summary>
        /// This method remove padding character from input text.
        /// </summary>
        /// <param name="cipherText"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        public string DecryptString(byte[] cipherText, byte[] key, byte[] iv)
        {
            var textWithPadding = Encoding.UTF8.GetString(cipherText);
            return textWithPadding.Replace(Constants.ExtPadding, string.Empty);
        }

        /// <summary>
        /// This method pads string to multiple of 16 bytes.
        /// </summary>
        /// <param name="plainText"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        public byte[] Encrypt(string plainText, byte[] key, byte[] iv)
        {
            var originalLength = plainText.Length;
            for (var i = 0; i < 16 - originalLength; i++)
            {
                plainText += Constants.ExtPadding;
            }
            return Encoding.UTF8.GetBytes(plainText);
        }

        /// <summary>
        /// This method return the input number without modification.
        /// </summary>
        /// <param name="plainLong"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        public byte[] Encrypt(long plainLong, byte[] key, byte[] iv)
        {
            var longData = BitConverter.GetBytes(plainLong);
            var rs = new byte[16];
            longData.CopyTo(rs, 8);
            return rs;
        }

        /// <summary>
        /// This method return the input binary data without modification.
        /// </summary>
        /// <param name="plainData"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        public byte[] Encrypt(byte[] plainData, byte[] key, byte[] iv)
        {
            return plainData;
        }
    }
}
