using Steganography.Crypto;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Steganography.ImageManipulating
{
    /// <summary>
    /// Hide and retrieve secret data from image file.
    /// </summary>
    public class ImageManipulator : IManipulator
    {
        /// <summary>
        /// Helper object to generate encryption key based on password,
        /// generate random byte array and perform SHA-256 hash on byte array.
        /// </summary>
        private IMyHashable _hasher;
        /// <summary>
        /// Helper object to encrypt data.
        /// </summary>
        private IMyEncryptable _encryptor;
        /// <summary>
        /// Manipulate image data.
        /// </summary>
        private IPictureEditor _pictureEditor;

        public IMyEncryptable Encryptor
        {
            set => _encryptor = value;
        }

        public IPictureEditor PictureEditor
        {
            set => _pictureEditor = value;
        }

        public IMyHashable Hasher
        {
            set => _hasher = value;
        }

        /// <summary>
        /// Hide file in to medium.
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="secretPath"></param>
        /// <param name="password"></param>
        /// <param name="outputExt"></param>
        /// <returns>
        /// Byte array to represents medium file after secret file is added.
        /// </returns>
        public async Task<byte[]> HideFileIntoMediumAsync(string filePath, string secretPath, string password, string outputExt)
        {
            if (outputExt != Constants.BmpExtension && outputExt != Constants.PngExtension)
            {
                throw new FormatException("Output must be either in bmp or png format.");
            }

            // Update PictureEditor's image object using new path from "TextPicture" textbox
            _pictureEditor.Path = filePath;

            // Generate encryption key and IV for encryption process.
            var salt = _hasher.GenerateRandomByteArray(16);
            var key = _hasher.GenerateEncryptionKey(password, salt);
            var ivMeta = _hasher.GenerateRandomByteArray(16);
            var ivFile = _hasher.GenerateRandomByteArray(16);
            // Encrypt file content, extension and size.
            var encryptedExt = _encryptor.Encrypt(FileHelper.GetFileExtension(secretPath), key, ivMeta);
            var fileData = File.ReadAllBytes(secretPath);
            var encryptedFile = _encryptor.Encrypt(fileData, key, ivFile);
            var encryptedSize = _encryptor.Encrypt(encryptedFile.Length * 8, key, ivMeta);

            // Added encrypted file data into medium.
            await Task.Run(() =>
            {
                SetStenographyFlag(password);
                SetExtension(encryptedExt);
                SetSize(encryptedSize);
                SetIvFile(ivFile);
                SetIvMeta(ivMeta);
                SetSalt(salt);
                SetFile(encryptedFile);
            });

            // Get medium with secret file and return as byte array.
            return _pictureEditor.GetImageData(outputExt);
        }

        /// <summary>
        /// Retrieve and decrypt secret file from medium.
        /// </summary>
        /// <param name="mediumPath"></param>
        /// <param name="password"></param>
        /// <returns>
        /// Secret file extension and byte array represents secret file content.
        /// </returns>
        public async Task<SecretFileData> GetFileFromMediumAsync(string mediumPath, string password)
        {
            // Update PictureEditor's image object using new path from TextPicture textbox
            _pictureEditor.Path = mediumPath;

            // Check if given password is correct and medium file contents secret file.
            if (!GetStenographyFlag(password))
            {
                throw new UnauthorizedAccessException("Password is incorrect or Stenography is disabled");
            }

            byte[] fileData = null;
            var extension = string.Empty;

            await Task.Run(() =>
            {
                // Retrieve IV and salt from medium and use them with given password to generate decryption key.
                var ivMeta = GetIvMeta();
                var salt = GetSalt();
                var key = _hasher.GenerateEncryptionKey(password, salt);
                // Get encrypted file's extension, size and content then decrypt them.
                var encryptedEx = GetExtension();
                extension = _encryptor.DecryptString(encryptedEx, key, ivMeta);
                var encryptedSize = GetSize();
                var ivFile = GetIvFile();
                var size = _encryptor.DecryptLong(encryptedSize, key, ivMeta);            
                var encryptedByte = GetFile(size);
                fileData = _encryptor.DecryptBytes(encryptedByte, key, ivFile);
            });

            // Return file content and extension.
            return new SecretFileData
            {
                Data = fileData,
                Extension = extension,
            };
        }

        /// <summary>
        /// Set byte array represents file content into medium.
        /// Please refer to readme.txt for information regarding steganography format. 
        /// </summary>
        /// <param name="bytes"></param>
        private void SetFile(byte[] bytes)
        {
            // Secret file must be smaller than 1/8 the size of medium file.
            if (!_pictureEditor.CheckHiddenFileSize(bytes.Length))
            {
                throw new Exception("File is too big, please choose a bigger medium.");
            }

            var offset = Constants.FlagSize + Constants.ExtSize + Constants.SizeSize + Constants.IvSaltSize * 3;
            var padding = (3 - bytes.Length * 8 % 3) % 3;
            var pixelSize = (bytes.Length * 8 + padding) / 3;
            _pictureEditor.SetBytes(bytes, pixelSize, offset, padding);
        }

        /// <summary>
        /// Get byte array represent file content from medium.
        /// </summary>
        /// <param name="bitSize"></param>
        /// <returns></returns>
        private byte[] GetFile(long bitSize)
        {
            var offset = Constants.FlagSize + Constants.ExtSize + Constants.SizeSize + Constants.IvSaltSize * 3;
            var padding = (int)((3 - bitSize % 3) % 3);
            var pixelSize = (bitSize + padding) / 3;
            return _pictureEditor.GetBytes(pixelSize, offset, padding);
        }

        /// <summary>
        /// Set steganography flag into medium.
        /// Please refer to readme.txt for information regarding steganography format. 
        /// </summary>
        /// <param name="password"></param>
        private void SetStenographyFlag(string password)
        {
            var offset = 0;
            var bytes = _hasher.HashPassword(password);
            _pictureEditor.SetBytes(bytes, Constants.FlagSize, offset, 2);
        }

        /// <summary>
        /// Get steganography flag from medium.
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        private bool GetStenographyFlag(string password)
        {
            var offset = 0;
            var bytes = _pictureEditor.GetBytes(Constants.FlagSize, offset, 2);
            return bytes.SequenceEqual(_hasher.HashPassword(password));
        }

        /// <summary>
        /// Set file size into medium.
        /// Please refer to readme.txt for information regarding steganography format. 
        /// </summary>
        /// <param name="size"></param>
        private void SetSize(byte[] size)
        {
            var offset = Constants.FlagSize + Constants.ExtSize;
            _pictureEditor.SetBytes(size, Constants.SizeSize, offset, 1);
        }

        /// <summary>
        /// Get file size from medium.
        /// </summary>
        /// <returns></returns>
        private byte[] GetSize()
        {
            var offset = Constants.FlagSize + Constants.ExtSize;
            return _pictureEditor.GetBytes(Constants.SizeSize, offset, 1);
        }

        /// <summary>
        /// Set file extension into medium.
        /// Please refer to readme.txt for information regarding steganography format. 
        /// </summary>
        /// <param name="extension"></param>
        private void SetExtension(byte[] extension)
        {
            var offset = Constants.FlagSize;
            _pictureEditor.SetBytes(extension, Constants.ExtSize, offset, 1);
        }


        /// <summary>
        /// Get file extension from medium.
        /// </summary>
        /// <returns></returns>
        private byte[] GetExtension()
        {
            var offset = Constants.FlagSize;
            return _pictureEditor.GetBytes(Constants.ExtSize, offset, 1);
        }

        /// <summary>
        /// Set byte array represents IV to generate encryption/decryption key into medium.
        /// Please refer to readme.txt for information regarding steganography format. 
        /// </summary>
        /// <param name="ivMeta"></param>
        private void SetIvMeta(byte[] ivMeta)
        {
            var offset = Constants.FlagSize + Constants.ExtSize + Constants.SizeSize;
            _pictureEditor.SetBytes(ivMeta, Constants.IvSaltSize, offset, 1);
        }

        /// <summary>
        /// Get byte array represents IV to generate encryption/decryption key from medium.
        /// </summary>
        /// <returns></returns>
        private byte[] GetIvMeta()
        {
            var offset = Constants.FlagSize + Constants.ExtSize + Constants.SizeSize;
            return _pictureEditor.GetBytes(Constants.IvSaltSize, offset, 1);
        }

        /// <summary>
        /// Set byte array represents IV to encrypt/decrypt file into medium.
        /// Please refer to readme.txt for information regarding steganography format. 
        /// </summary>
        /// <param name="ivFile"></param>
        private void SetIvFile(byte[] ivFile)
        {
            var offset = Constants.FlagSize + Constants.ExtSize + Constants.SizeSize + Constants.IvSaltSize;
            _pictureEditor.SetBytes(ivFile, Constants.IvSaltSize, offset, 1);
        }

        /// <summary>
        /// Get byte array represents IV to encrypt/decrypt file from medium.
        /// </summary>
        /// <returns></returns>
        private byte[] GetIvFile()
        {
            var offset = Constants.FlagSize + Constants.ExtSize + Constants.SizeSize + Constants.IvSaltSize;
            return _pictureEditor.GetBytes(Constants.IvSaltSize, offset, 1);
        }

        /// <summary>
        /// Set byte array represents password salt into medium.
        /// Please refer to readme.txt for information regarding steganography format. 
        /// </summary>
        /// <param name="salt"></param>
        private void SetSalt(byte[] salt)
        {
            var offset = Constants.FlagSize + Constants.ExtSize + Constants.SizeSize + Constants.IvSaltSize * 2;
            _pictureEditor.SetBytes(salt, Constants.IvSaltSize, offset, 1);
        }

        /// <summary>
        /// Get byte array represents password salt from medium.
        /// </summary>
        /// <returns></returns>
        private byte[] GetSalt()
        {
            var offset = Constants.FlagSize + Constants.ExtSize + Constants.SizeSize + Constants.IvSaltSize * 2;
            return _pictureEditor.GetBytes(Constants.IvSaltSize, offset, 1);
        }
    }
}
