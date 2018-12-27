using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Steganography
{
    /// <summary>
    /// Small helper method to retrieve information from file.
    /// </summary>
    public class FileHelper
    {
        /// <summary>
        /// Get extension of file from its full path.
        /// </summary>
        /// <param name="input"></param>
        /// <returns>
        /// Extension of file at given full path, converted to lower case.
        /// </returns>
        public static string GetFileExtension(string input)
        {
            var rs = string.Empty;
            var regex = new Regex(@"^.+\.(.+)$");
            var match = regex.Match(input);
            if (match.Success)
            {
                rs = match.Groups[1].Value.ToLower();
            }
            return rs;
        }

        /// <summary>
        /// Read binary data and get extension of file from disk using file path.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Tuple<byte[], string> ReadFileFromDisk(string path)
        {
            var extension = GetFileExtension(path);
            var fileData = File.ReadAllBytes(path);
            return new Tuple<byte[], string>(fileData, extension);
        }
    }
}
