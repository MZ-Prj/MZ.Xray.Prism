using MZ.Infrastructure.Interfaces;
using System.Threading.Tasks;

namespace MZ.Infrastructure
{
    /// <summary>
    /// 데이터베이스 서비스의 엔트리 포인트 인터페이스
    /// 
    /// - 각종 비즈니스 서비스(User, AppSetting, Image 등)에 대한 접근 포인트 역할을 수행
    /// - 시스템 초기 데이터 생성(Admin, User, AppSetting 등) 유틸리티 메서드 제공
    /// - DI(의존성 주입) 컨테이너에서 하나의 서비스로 등록해 사용 가능
    /// </summary>
    public interface IDatabaseService
    {
        /// <summary>
        /// 사용자 관련 비즈니스 로직 서비스
        /// </summary>
        IUserService User { get; }

        /// <summary>
        /// 앱 환경설정 관련 비즈니스 로직 서비스
        /// </summary>
        IAppSettingService AppSetting { get; }

        /// <summary>
        /// 이미지 데이터 관련 비즈니스 로직 서비스
        /// </summary>
        IXrayVisionImageService Image { get; }

        /// <summary>
        /// Filter 관련 비즈니스 로직 서비스
        /// </summary>
        IXrayVisionFilterService Filter { get; }

        /// <summary>
        /// Calibration 관련 비즈니스 로직 서비스
        /// </summary>
        IXrayVisionCalibrationService Calibration { get; }

        /// <summary>
        /// Material 관련 비즈니스 로직 서비스
        /// </summary>
        IXrayVisionMaterialService Material { get; }

        /// <summary>
        /// ZeffectControl 관련 비즈니스 로직 서비스
        /// </summary>
        IXrayVisionZeffectControlService ZeffectControl { get; }

        /// <summary>
        /// CurveControl 관련 비즈니스 로직 서비스
        /// </summary>
        IXrayVisionCurveControlService CurveControl { get; }

        /// <summary>
        /// AI 모델 옵션 관련 비즈니스 로직 서비스
        /// </summary>
        IXrayAIOptionService AIOption { get; }


        Task MakeAdmin();
        Task<bool> MakeUserAsync(string username, string password);
        Task MakeAppSettingAsync(string username);
    }
}
