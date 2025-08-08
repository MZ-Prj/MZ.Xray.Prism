using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace MZ.Util
{
    public class MZZip
    {
        public static async Task<bool> UnzipAsync(string zipPath, string extractPath)
        {
            if (!File.Exists(zipPath))
            {
                return false;
            }

            try
            {
                await Task.Run(() =>
                {
                    MZIO.TryDeleteFile(extractPath);
                    MZIO.TryMakeDirectoryRemoveFile(extractPath);

                    ZipFile.ExtractToDirectory(zipPath, extractPath, overwriteFiles: true);
                });

                return true;
            }
            catch
            {
                return false; 
            }
        }
    }
}
