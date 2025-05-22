using System.ComponentModel.DataAnnotations;

namespace MZ.Domain.Enums
{
    public enum LanguageRole
    {
        [Display(Name = "ko-KR", Description = "한국어")]
        koKR,
        [Display(Name = "en-US", Description = "English")]
        enUS,
    }
}
