using MZ.Domain.Models;
using System.Threading.Tasks;

namespace MZ.Producer.Engine
{
    public interface IProducerService
    {
        SocketProcesser Socket { get; set; }
        XrayDataProcesser XrayData { get; set; }
        bool IsPaused { get; set; }
        Task LoadFilesAsync(string path);
        Task SendFileAsync(FileModel model);
        Task LoadAsync();
        Task RunAsync();
        void Stop();
        void Pause();
    }
}
