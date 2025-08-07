using System.Threading.Tasks;
using System.Threading;
using System;
using System.Net.Http;
using System.IO;
using MZ.Logger;

namespace MZ.Util
{
    /// <summary>
    /// 파일 다운로드 비동기 인터페이스
    /// </summary>
    public interface IFileDownloader
    {
        Task<bool> RunAsync(string source, string destination, IProgress<double> progress = null, CancellationToken cancellationToken = default);
    }


    /// <summary>
    /// HttpClient 기반 파일 다운로드 클래스
    /// </summary>
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

        /// <summary>
        /// 파일 다운로드 실행, 예외시 파일 삭제 및 로깅
        /// </summary>
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
                MZLogger.Error(ex.Message);
                MZIO.TryDeleteFile(destination);
                return false;
            }
        }

        /// <summary>
        /// 실제 파일 다운로드, 진행률 리포트 및 취소 지원
        /// </summary>
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
        /// <summary>
        /// HttpClient 리소스 해제
        /// </summary>
        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}
