using MZ.Domain.Enums;

namespace MZ.DTO
{
    /// <summary>
    /// 언어 변경 요청 DTO
    /// 
    /// - 클라이언트가 시스템 언어 변경을 서버에 요청할 때 사용.
    /// </summary>
    /// <param name="LanguageRole">변경할 언어</param>
    public record LanguageRequest(
        LanguageRole? LanguageRole
    );

}
