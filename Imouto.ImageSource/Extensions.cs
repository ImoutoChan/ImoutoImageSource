using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Imouto.ImageSource
{
    static class Extensions
    {
        public static string CalculateMd5HashForString(this string inputString) 
            => CalculateMd5HashForBytes(Encoding.ASCII.GetBytes(inputString));

        public static string CalculateMd5HashForFile(this string filePath) 
            => CalculateMd5HashForFile(new FileInfo(filePath));

        public static string CalculateMd5HashForFile(this FileInfo fileInfo)
        {
            if (!fileInfo.Exists)
            {
                throw new ArgumentException($"{nameof(fileInfo)} doesn't exist.");
            }

            return CalculateMd5HashForBytes(File.ReadAllBytes(fileInfo.FullName));
        }

        public static string CalculateMd5HashForBytes(this byte[] bytes)
        {
            var md5 = MD5.Create();

            byte[] hash = md5.ComputeHash(bytes);

            var sb = new StringBuilder();

            foreach (byte t in hash)
            {
                sb.Append(t.ToString("X2"));
            }

            return sb.ToString();
        }
    }
}
