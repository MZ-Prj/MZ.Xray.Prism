using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace MZ.Util
{
    public class MZIO
    {
        public static string GetSystemTempPath()
        {
            return Path.GetTempPath();
        }

        public static string GetSystemTempPath(string path)
        {
            var fileName = Path.GetFileName(path);
            return Path.Combine(Path.GetTempPath(), fileName);
        }

        public static string[] GetFileNamesWithExtension(string path, string extension)
        {
            try
            {
                string pattern = "*";

                if (!Directory.Exists(path))
                {
                    return [];
                }

                if (!string.IsNullOrWhiteSpace(extension))
                {
                    if (!extension.StartsWith('.'))
                    {
                        extension = "." + extension;
                    }
                    pattern = "*" + extension;
                }

                var fullPaths = Directory.GetFiles(path, pattern, SearchOption.AllDirectories);
                return [.. fullPaths.Select(Path.GetFileName)];
            }
            catch
            {
                return [];
            }
        }

        public static string[] GetFilesWithExtension(string path, string extension)
        {
            try
            {
                string pattern = "*";

                if (!Directory.Exists(path))
                {
                    return [];
                }

                if (!string.IsNullOrWhiteSpace(extension))
                {
                    if (!extension.StartsWith('.'))
                    {
                        extension = "." + extension;
                    }
                    pattern = "*" + extension;
                }

                return Directory.GetFiles(path, pattern, SearchOption.AllDirectories);
            }
            catch
            {
                return [];
            }

        }

        public static void TryDeleteFile(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        public static void TryMakeDirectory(string path)
        {
            try
            {
                if (!string.IsNullOrEmpty(path))
                {
                    Directory.CreateDirectory(path);
                }
            }
            catch
            {

            }
        }

        public static void TryMakeDirectoryAndCopy(string input, string copy)
        {
            try
            {
                if (!string.IsNullOrEmpty(copy))
                {
                    Directory.CreateDirectory(copy);
                    File.Copy(input, copy);
                }
            }
            catch
            {

            }
        }
    }

    public class MZSecurityIO
    {
        public static void Encrypt(string inputFilePath, string outputFilePath, string fileKey)
        {
            byte[] fileBytes = File.ReadAllBytes(inputFilePath);
            byte[] keyBytes;
            keyBytes = SHA256.HashData(Encoding.UTF8.GetBytes(fileKey));

            using Aes aes = Aes.Create();
            aes.Key = keyBytes;
            aes.GenerateIV();
            byte[] iv = aes.IV;

            using MemoryStream ms = new();
            ms.Write(iv, 0, iv.Length);

            using (CryptoStream cryptoStream = new(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
            {
                cryptoStream.WriteAsync(fileBytes, 0, fileBytes.Length);
                cryptoStream.FlushFinalBlock();
            }

            byte[] encryptedData = ms.ToArray();
            File.WriteAllBytesAsync(outputFilePath, encryptedData);
        }

        public static void Decrypt(string inputFilePath, string outputFilePath, string fileKey)
        {
            byte[] encryptedFileBytes = File.ReadAllBytes(inputFilePath);
            byte[] keyBytes;
            keyBytes = SHA256.HashData(Encoding.UTF8.GetBytes(fileKey));

            using Aes aes = Aes.Create();
            aes.Key = keyBytes;
            int ivLength = aes.BlockSize / 8;

            byte[] iv = new byte[ivLength];
            Array.Copy(encryptedFileBytes, 0, iv, 0, iv.Length);
            aes.IV = iv;

            byte[] cipherText = new byte[encryptedFileBytes.Length - ivLength];
            Array.Copy(encryptedFileBytes, ivLength, cipherText, 0, cipherText.Length);

            using MemoryStream ms = new();

            using (CryptoStream cryptoStream = new(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
            {
                cryptoStream.WriteAsync(cipherText, 0, cipherText.Length);
                cryptoStream.FlushFinalBlock();
            }

            byte[] decryptedData = ms.ToArray();
            File.WriteAllBytesAsync(outputFilePath, decryptedData);
        }

        public static void EncryptAndRemove(string filePath, string fileKey, string extension = ".mz")
        {
            string newFilePath = Path.ChangeExtension(filePath, extension);

            Encrypt(filePath, newFilePath, fileKey);

            File.Delete(filePath);
        }

    }
}
