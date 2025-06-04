using System.ComponentModel.DataAnnotations;

namespace MZ.Domain.Enums
{
    public enum ColorRole
    {
        [Display(Name = "Gray", Description = "Gray")]
        Gray,
        [Display(Name = "Color", Description = "Color")]
        Color,
        [Display(Name = "Organic", Description = "Organic")]
        Organic,
        [Display(Name = "Inorganic", Description = "Inorganic")]
        Inorganic,
        [Display(Name = "Metal", Description = "Metal")]
        Metal,
    }
}
