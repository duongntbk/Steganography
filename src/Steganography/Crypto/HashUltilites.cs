using System.Text;
using System.Security.Cryptography;
using System.Configuration;

namespace Steganography.Crypto
{
    /// <summary>
    /// Helper class to generate password hash, key and random data to be used in encryption.
    /// </summary>
    public class HashUtilities : IMyHashable
    {
        private readonly int _encryptKeyIteration;
        private readonly int _passwordIteration;

        /// <summary>
        /// If encryption key iteration and password iteration does not exists in app config,
        /// use default value.
        /// </summary>
        public HashUtilities()
        {
            if (!int.TryParse(ConfigurationManager.AppSettings[Constants.AppSettingKeys.EncryptKeyIteration], 
                out _encryptKeyIteration))
            {
                _encryptKeyIteration = Constants.DefaultEncryptKeyIteration;
            }

            if (!int.TryParse(ConfigurationManager.AppSettings[Constants.AppSettingKeys.PasswordIteration], 
                out _passwordIteration))
            {
                _passwordIteration = Constants.DefaultPasswordIteration;
            }
        }

        /// <summary>
        /// Compute the SHA256 hash of a string.
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public byte[] HashPassword(string password)
        {
            var rs = Encoding.UTF8.GetBytes(password);
            var hashManaged = new SHA256Managed();
            for (var i = 0; i < _passwordIteration; i++)
            {
                rs = hashManaged.ComputeHash(rs);
            }
            return rs;
        }

        /// <summary>
        /// Use PBKDF2 to generate an encryption key from given password and salt.
        /// </summary>
        /// <param name="password"></param>
        /// <param name="salt"></param>
        /// <returns></returns>
        public byte[] GenerateEncryptionKey(string password ,byte[] salt)
        {
            // Setup the password generator
            var pwdGen = new Rfc2898DeriveBytes(password, salt, _encryptKeyIteration);

            // generate an key
            return pwdGen.GetBytes(32);
        }

        /// <summary>
        /// Generate cryptographically secure random binary data with given size.
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public byte[] GenerateRandomByteArray(int size)
        {
            var rs = new byte[size];
            var myRng = new RNGCryptoServiceProvider();
            myRng.GetBytes(rs);
            return rs;
        }
    }
}
