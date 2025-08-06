using System.Threading.Tasks;

namespace MZ.Xray.Engine
{
    /// <summary>
    /// XrayVision 엔진의 주요 처리 프로세서 및 제어 기능을 제공하는 서비스 인터페이스
    /// 하위 모듈(PDF, UI, 미디어, 캘리브레이션, 머티리얼, Zeffect, 소켓 AI 엔진 제어 수행)
    /// </summary>
    public interface IXrayService
    {
        /// <summary>
        /// PDF 처리 프로세서
        /// </summary>
        PDFProcesser PDF { get; set; }
        /// <summary>
        /// UI 처리 프로세서
        /// </summary>
        UIProcesser UI { get; set; }
        /// <summary>
        /// 미디어(이미지/동영상 등) 처리 프로세서
        /// </summary>
        MediaProcesser Media { get; set; }
        /// <summary>
        /// Calibration 처리 프로세서
        /// </summary>
        CalibrationProcesser Calibration { get; set; }
        /// <summary>
        /// Material 처리 프로세서
        /// </summary>
        MaterialProcesser Material { get; set; }
        /// <summary>
        /// Zeffect 처리 프로세서
        /// </summary>
        ZeffectProcesser Zeffect { get; set; }

        /// <summary>
        /// Curve 처리 프로세서
        /// </summary>
        CurveSplineProcesser CurveSpline { get; set; }

        /// <summary>
        /// 소켓 데이터 수신 프로세서
        /// </summary>
        SocketReceiveProcesser SocketReceive { get; set; }

        /// <summary>
        /// 소켓 연결/초기화 작업을 수행
        /// </summary>
        void InitializeSocket();
        /// <summary>
        /// AI(딥러닝 등) 엔진을 비동기로 초기화
        /// </summary>
        Task InitializeAI();
        /// <summary>
        /// Xray 영상 재생
        /// </summary>
        void Play();
        /// <summary>
        /// Xray 영상 정지
        /// </summary>
        void Stop();
        /// <summary>
        /// 영상 재생 중인지 여부
        /// </summary>
        bool IsPlaying();
        /// <summary>
        /// 재생/정지 상태를 토글
        /// </summary>
        void PlayStop();
        /// <summary>
        /// 데이터베이스에서 로드
        /// </summary>
        void LoadDatabase();
        /// <summary>
        /// 데이터베이스에서 저장
        /// </summary>
        void SaveDatabase();
        /// <summary>
        /// 슬라이더 제어
        /// </summary>
        void PrevNextSlider(int index);
    }
}
