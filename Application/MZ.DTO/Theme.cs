using MZ.Domain.Enums;

namespace MZ.DTO
{
    /// <summary>
    /// 테마 변경 요청 DTO
    /// - 사용자의 테마(예: 다크모드, 라이트모드 등) 변경 
    /// </summary>
    /// <param name="ThemeRole">변경할 테마</param>
    public record ThemeRequest(
        ThemeRole? ThemeRole
    );
}
