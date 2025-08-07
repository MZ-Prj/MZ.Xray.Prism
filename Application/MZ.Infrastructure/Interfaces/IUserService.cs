using MZ.DTO;
using MZ.DTO.Enums;
using MZ.Domain.Enums;
using MZ.Domain.Entities;
using System.Threading.Tasks;

namespace MZ.Infrastructure.Interfaces
{
    /// <summary>
    /// 사용자 관련 서비스 인터페이스
    /// </summary>
    public interface IUserService 
    {
        /// <summary>
        /// 사용자 로그인 (비동기)
        /// </summary>
        Task<BaseResponse<UserLoginRole, UserEntity>> Login(UserLoginRequest request);

        /// <summary>
        /// 사용자 로그아웃
        /// </summary>
        Task<BaseResponse<BaseRole, string>> Logout();

        /// <summary>
        /// 현재 로그인된 사용자 조회
        /// </summary>
        BaseResponse<BaseRole, string> CurrentUser();
        /// <summary>
        /// 현재 로그인된 사용자 조회(전체정보)
        /// </summary>
        Task<BaseResponse<BaseRole, UserEntity>> GetUser();
        /// <summary>
        /// 사용자 및 사용자 설정 정보 조회 (비동기)
        /// </summary>
        Task<BaseResponse<BaseRole, UserEntity>> GetUserWithUserSetting();
        /// <summary>
        /// 사용자 설정 정보 조회 (비동기)
        /// </summary>
        Task<BaseResponse<BaseRole, UserSettingEntity>> GetUserSetting();
        /// <summary>
        /// 사용자 회원가입 (비동기)
        /// </summary>
        Task<BaseResponse<UserRegisterRole, UserEntity>> Register(UserRegisterRequest request);
        /// <summary>
        /// 언어 설정 변경 (비동기)
        /// </summary>
        Task<BaseResponse<BaseRole, LanguageRole>> ChangeLanguage(LanguageRequest language);
        /// <summary>
        /// 테마 설정 변경 (비동기)
        /// </summary>
        Task<BaseResponse<BaseRole, ThemeRole>> ChangeTheme(ThemeRequest theme);
        /// <summary>
        /// 사용자 설정 저장 (비동기)
        /// </summary>
        Task<BaseResponse<BaseRole, UserSettingEntity>> SaveUserSetting(UserSettingSaveRequest request);

        /// <summary>
        /// 관리자 유무
        /// </summary>
        Task<BaseResponse<BaseRole, bool>> IsAdmin();

        /// <summary>
        /// 로그인 상태 여부 반환
        /// </summary>
        bool IsLoggedIn();

    }
}
