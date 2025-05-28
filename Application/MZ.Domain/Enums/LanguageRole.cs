using System.ComponentModel.DataAnnotations;

namespace MZ.Domain.Enums
{
    public enum LanguageRole
    {
        [Display(Name = "ko-KR", Description = "한국어")]
        KoKR,
        [Display(Name = "en-US", Description = "English")]
        EnUS,
    }
}
