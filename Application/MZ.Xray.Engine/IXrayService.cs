
using System.Threading.Tasks;

namespace MZ.Xray.Engine
{
    public interface IXrayService
    {
        PDFProcesser PDF { get; set; }
        UIProcesser UI { get; set; }
        MediaProcesser Media { get; set; }
        CalibrationProcesser Calibration { get; set; }
        MaterialProcesser Material { get; set; }
        ZeffectProcesser Zeffect { get; set; }
        SocketReceiveProcesser SocketReceive { get; set; }
        void InitializeSocket();
        Task InitializeAI();
        void Play();
        void Stop();
        bool IsPlaying();
        void PlayStop();
        void LoadDatabase();
        void SaveDatabase();
        void PrevNextSlider(int index);
    }
}
