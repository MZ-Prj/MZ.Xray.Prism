using System.ComponentModel.DataAnnotations;

namespace MZ.Domain.Enums
{
    public enum ThemeRole
    {
        [Display(Name = "Dark.Steel", Description = "Dark")]
        DarkSteel,
        [Display(Name = "Light.Steel", Description = "Light")]
        LightSteel,
    }
}
