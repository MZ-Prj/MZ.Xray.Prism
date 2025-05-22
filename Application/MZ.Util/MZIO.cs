using System.IO;
using System.Linq;

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
                    if (!extension.StartsWith("."))
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
                    if (!extension.StartsWith("."))
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
                string directoryPath = Path.GetDirectoryName(path);
                if (!string.IsNullOrEmpty(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
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
                string directoryPath = Path.GetDirectoryName(copy);
                if (!string.IsNullOrEmpty(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                    File.Copy(input, copy);
                }
            }
            catch
            {

            }
        }
    }
}
