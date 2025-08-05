#nullable enable
namespace MZ.DTO
{

    #region Request
    /// <summary>
    /// 애플리케이션 환경설정 등록 요청 DTO
    /// </summary>
    /// <param name="LastestUsername">최근 로그인한 사용자 아이디<</param>
    /// <param name="IsUsernameSave">아이디 저장 여부</param>
    public record AppSettingRegisterRequest(
        string LastestUsername,
        bool IsUsernameSave
    );

    #endregion
}
