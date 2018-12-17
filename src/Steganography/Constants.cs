namespace Steganography
{
    /// <summary>
    /// Constants value for Steganography library.
    /// </summary>
    internal class Constants
    {
        public const int DefaultEncryptKeyIteration = 10000;
        public const int DefaultPasswordIteration = 20000;
        public const string PngExtension = "png";
        public const string BmpExtension = "bmp";
        /// <summary>
        /// Size of isStenography flag (256bit round up 258bit == 86 * 3) padding 2 last 0 bit.
        /// </summary>
        public const int FlagSize = 86;
        /// <summary>
        /// Size of extension (128 bit round up 129 == 43 * 3) padding 1 last 0 bit.
        /// </summary>
        public const int ExtSize = 43;
        /// <summary>
        /// Size of hidden data (128 bit round up 129 == 43 * 3) padding 1 last 0 bit.
        /// </summary>
        public const int SizeSize = 43;
        /// <summary>
        /// Size of IV and salt (128 bit round up 129 == 43 * 3) padding 1 last 0 bit.
        /// </summary>
        public const int IvSaltSize = 43;
        /// <summary>
        /// Character use to pad string to multiple of 16, used in NoEncrypt.DecryptString/DecryptString.
        /// </summary>
        public const string ExtPadding = "?";

        public class Pixel
        {
            /// <summary>
            /// Position of red color in pixel array.
            /// </summary>
            public const int BitR = 0;
            /// <summary>
            /// Position of green color in pixel array.
            /// </summary>
            public const int BitG = 1;
            /// <summary>
            /// Position of blue color in pixel array.
            /// </summary>
            public const int BitB = 2;
        }

        public class AppSettingKeys
        {
            /// <summary>
            /// Encryption key iteration setting key.
            /// </summary>
            public const string EncryptKeyIteration = "encryptkeyiteration";
            /// <summary>
            /// Password iteration setting key.
            /// </summary>
            public const string PasswordIteration = "passworditeration";
        }
    }
}
