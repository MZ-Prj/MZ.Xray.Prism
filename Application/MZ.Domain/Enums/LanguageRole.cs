using System.ComponentModel.DataAnnotations;

namespace MZ.Domain.Enums
{
    /// <summary>
    /// 지원되는 언어(로케일) Enum
    /// 
    /// - 각 항목은 시스템 다국어, UI 언어 전환 등에 사용
    /// - Display Name: 시스템 언어 코드(ISO)
    /// - Description: 실제 언어명
    /// </summary>
    public enum LanguageRole
    {
        /// <summary>
        /// 한국어 (대한민국, ko-KR)
        /// </summary>
        [Display(Name = "ko-KR", Description = "한국어")]
        KoKR,

        /// <summary>
        /// 영어 (미국, en-US)
        /// </summary>
        [Display(Name = "en-US", Description = "English")]
        EnUS,
    }
}
