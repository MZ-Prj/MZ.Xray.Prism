using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace MZ.Util
{
    /// <summary>
    /// 파일 및 디렉토리 관련 기능을 제공하는 유틸리티 클래스
    /// </summary>
    public class MZIO
    {
        /// <summary>
        /// 시스템 임시폴더 경로 반환
        /// </summary>
        public static string GetSystemTempPath()
        {
            return Path.GetTempPath();
        }

        /// <summary>
        /// 전달받은 경로의 파일명만 시스템 임시폴더에 결합하여 반환
        /// </summary>
        public static string GetSystemTempPath(string path)
        {
            var fileName = Path.GetFileName(path);
            return Path.Combine(Path.GetTempPath(), fileName);
        }

        /// <summary>
        /// 지정한 경로에서 확장자에 해당하는 파일명 리스트 반환 (재귀)
        /// </summary>
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

        /// <summary>
        /// 지정한 경로에서 확장자에 해당하는 전체 파일 경로 리스트 반환 (재귀)
        /// </summary>
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

        /// <summary>
        /// 파일 존재 시 안전하게 삭제 시도
        /// </summary>
        public static void TryDeleteFile(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
            catch
            {

            }
        }

        /// <summary>
        /// 경로가 존재하지 않으면 디렉토리 생성 시도
        /// </summary>
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

        /// <summary>
        /// 파일 경로에서 디렉토리만 추출하여 존재하지 않으면 생성
        /// </summary>
        public static void TryMakeDirectoryRemoveFile(string path)
        {
            try
            {
                string directory = Path.GetDirectoryName(path);

                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
            }
            catch
            {

            }
        }

        /// <summary>
        /// 대상 경로에 디렉토리 생성 후 파일 복사
        /// </summary>
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
        /// <summary>
        /// 파일 존재 여부 반환
        /// </summary>
        public static bool IsFileExist(string input)
        {
            if (File.Exists(input))
            {
                return true;
            }
            return false;
        }
    }

    /// <summary>
    /// AES 암호화/복호화 및 파일 삭제 지원 유틸리티 클래스
    /// </summary>
    public class MZSecurityIO
    {
        /// <summary>
        /// 파일을 AES로 암호화해서 다른 경로에 저장
        /// </summary>
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

        /// <summary>
        /// AES로 암호화된 파일을 복호화해서 다른 경로에 저장
        /// </summary>
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

        /// <summary>
        /// 파일을 암호화하고 원본 삭제
        /// </summary>
        public static void EncryptAndRemove(string filePath, string fileKey, string extension = ".mz")
        {
            string newFilePath = Path.ChangeExtension(filePath, extension);

            Encrypt(filePath, newFilePath, fileKey);

            File.Delete(filePath);
        }

    }
}
