using System.ComponentModel.DataAnnotations;

namespace MZ.Domain.Enums
{
    /// <summary>
    /// 지원하는 UI 테마 종류 Enum
    /// 
    /// - 각 테마는 앱/대시보드/뷰어의 전체 색상 스타일에 적용
    /// - Name : 내부 ThemeService 코드/식별자
    /// - Description : 사용자에게 보여줄 테마 명칭
    /// </summary>
    public enum ThemeRole
    {
        /// <summary>
        /// 다크모드(어두운 배경), Steel 컬러 계열  
        /// - 내부 명칭: "Dark.Steel"
        /// - 사용자: Dark
        /// </summary>
        [Display(Name = "Dark.Steel", Description = "Dark")]
        DarkSteel,

        /// <summary>
        /// 라이트모드(밝은 배경), Steel 컬러 계열  
        /// - 내부 명칭: "Light.Steel"
        /// - 사용자: Light
        /// </summary>
        [Display(Name = "Light.Steel", Description = "Light")]
        LightSteel,
    }
}
