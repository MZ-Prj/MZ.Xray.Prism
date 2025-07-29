using System.Threading.Tasks;
using System.Threading;
using System;
using System.Net.Http;
using System.IO;
using MZ.Logger;

namespace MZ.Util
{
    public interface IFileDownloader
    {
        Task<bool> RunAsync(string source, string destination, IProgress<double> progress = null, CancellationToken cancellationToken = default);
    }

    public class MZWebDownload : IFileDownloader, IDisposable
    {
        private readonly HttpClient _httpClient;

        public MZWebDownload()
        {
            _httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromMinutes(10)
            };
        }

        public async Task<bool> RunAsync(string source, string destination, IProgress<double> progress = null, CancellationToken cancellationToken = default)
        {
            try
            {
                MZLogger.Information($"Executing : Source={source}, Destination={destination}");

                using var response = await _httpClient.GetAsync(source, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

                response.EnsureSuccessStatusCode();

                MZLogger.Information($"Executing : Starting Download [Status: {response.StatusCode}]");
                
                await Download(response, destination, progress, cancellationToken);
                
                MZLogger.Information($"Executing : Download Completed");

                return true;
            }
            catch (Exception ex)
            {
                MZLogger.Error(ex.ToString());
                MZIO.TryDeleteFile(destination);
                return false;
            }
        }

        private async Task Download(HttpResponseMessage response, string filePath, IProgress<double> progress, CancellationToken cancellationToken)
        {
            var totalBytes = response.Content.Headers.ContentLength ?? -1L;
            int bufferSize = 8192;

            using var contentStream = await response.Content.ReadAsStreamAsync(cancellationToken);
            using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: bufferSize, useAsync: true);
            var buffer = new byte[bufferSize];
            var totalBytesRead = 0L;
            var bytesRead = 0;

            while ((bytesRead = await contentStream.ReadAsync(buffer, cancellationToken)) > 0)
            {
                await fileStream.WriteAsync(buffer.AsMemory(0, bytesRead), cancellationToken);

                totalBytesRead += bytesRead;

                if (totalBytes > 0 && progress != null)
                {
                    progress.Report((double)totalBytesRead / totalBytes * 100);
                }
            }
        }
        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}
