using System.Threading.Tasks;

namespace MZ.Xray.Engine
{
    public interface IXrayService
    {
        MediaProcesser Media { get; set; }
        CalibrationProcesser Calibration { get; set; }
        MaterialProcesser Material { get; set; }
        ZeffectProcesser Zeffect { get; set; }
        XrayDataSaveManager SaveManager { get; set; }
        SocketReceiveProcesser SocketReceive { get; set; }
        void InitializeSocket();
        void Play();
        void Stop();
        bool IsPlaying();
    }
}
