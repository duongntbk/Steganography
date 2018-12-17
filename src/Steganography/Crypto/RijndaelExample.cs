/*
 * Code based on Microsoft Developer Network
 * https://msdn.microsoft.com/en-us/library/system.security.cryptography.rijndael(v=vs.110).aspx
 */

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Steganography.Crypto
{
    /// <summary>
    /// Implementation of IMyEncryptable using Rijndael symmetric encryption algorithm.
    /// </summary>
    public class RijndaelExample : IMyEncryptable
    {
        /// <summary>
        /// Encrypt text using AES with given key and IV.
        /// </summary>
        /// <param name="plainText"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        public byte[] Encrypt(string plainText, byte[] key, byte[] iv)
        {
            var bytes = Encoding.UTF8.GetBytes(plainText);
            return Encrypt(bytes, key, iv);
        }       

        /// <summary>
        /// Decrypt text using AES with given key and IV.
        /// </summary>
        /// <param name="cipherText"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        public string DecryptString(byte[] cipherText, byte[] key, byte[] iv)
        {
            var bytes = DecryptBytes(cipherText, key, iv);
            return Encoding.UTF8.GetString(bytes);
        }

        /// <summary>
        /// Encrypt number using AES with given key and IV.
        /// </summary>
        /// <param name="plainLong"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        public byte[] Encrypt(long plainLong, byte[] key, byte[] iv)
        {
            var bytes = BitConverter.GetBytes(plainLong);
            return Encrypt(bytes, key, iv);
        }

        /// <summary>
        /// Decrypt number using AES with given key and IV.
        /// </summary>
        /// <param name="cipherLong"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        public long DecryptLong(byte[] cipherLong, byte[] key, byte[] iv)
        {
            var bytes = DecryptBytes(cipherLong, key, iv);
            return BitConverter.ToInt64(bytes, 0);
        }

        /// <summary>
        /// Encrypt binary data using AES with given key and IV.
        /// </summary>
        /// <param name="plainData"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        public byte[] Encrypt(byte[] plainData, byte[] key, byte[] iv)
        {            
            // Check arguments. 
            if (plainData == null || plainData.Length <= 0)
            {
                throw new ArgumentNullException(nameof(plainData));
            }
            if (key == null || key.Length <= 0)
            {
                throw new ArgumentNullException(nameof(key));
            } 
            if (iv == null || iv.Length <= 0)
            {
                throw new ArgumentNullException(nameof(iv));
            }

            byte[] encrypted;
            // Create an RijndaelManaged object 
            // with the specified key and IV. 
            using (var rijAlg = new RijndaelManaged())
            {
                rijAlg.Key = key;
                rijAlg.IV = iv;

                // Create a decryptor to perform the stream transform.
                var encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);

                // Create the streams used for encryption. 
                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        //Write all data to the stream.
                        csEncrypt.Write(plainData, 0, plainData.Length);
                        csEncrypt.FlushFinalBlock();
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            // Return the encrypted bytes from the memory stream. 
            return encrypted;
        }

        /// <summary>
        /// Decrypt binary data using AES with given key and IV.
        /// </summary>
        /// <param name="cipher"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        public byte[] DecryptBytes(byte[] cipher, byte[] key, byte[] iv)
        {
            // Check arguments. 
            if (cipher == null || cipher.Length <= 0)
            {
                throw new ArgumentNullException(nameof(cipher));
            }
            if (key == null || key.Length <= 0)
            {
                throw new ArgumentNullException(nameof(key));
            }
            if (iv == null || iv.Length <= 0)
            {
                throw new ArgumentNullException(nameof(iv));
            }

            try
            {
                // Create an RijndaelManaged object 
                // with the specified key and IV. 
                using (var rijAlg = new RijndaelManaged())
                {
                    rijAlg.Key = key;
                    rijAlg.IV = iv;

                    // Create a decryptor to perform the stream transform.
                    var decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

                    // Create the streams used for decryption. 
                    using (var msDecrypt = new MemoryStream(cipher))
                    {
                        using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (var ms = new MemoryStream())
                            {
                                csDecrypt.CopyTo(ms);
                                return ms.ToArray();
                            }
                        }
                    }
                }
            }
            catch (CryptographicException ex)
            {
                throw new Exception("Cannot decrypt file using AES, please try no encryption option", ex);
            }
        }
    }
}