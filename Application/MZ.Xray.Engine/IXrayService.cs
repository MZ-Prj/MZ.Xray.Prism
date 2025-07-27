
namespace MZ.Xray.Engine
{
    public interface IXrayService
    {
        MediaProcesser Media { get; set; }
        CalibrationProcesser Calibration { get; set; }
        MaterialProcesser Material { get; set; }
        ZeffectProcesser Zeffect { get; set; }
        SocketReceiveProcesser SocketReceive { get; set; }
        void InitializeSocket();
        void InitializeAI();
        void Play();
        void Stop();
        bool IsPlaying();
        void LoadDatabase();
        void SaveDatabase();
    }
}
