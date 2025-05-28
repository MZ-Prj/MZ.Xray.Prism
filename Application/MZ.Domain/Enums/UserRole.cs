using System.ComponentModel.DataAnnotations;

namespace MZ.Domain.Enums
{
    public enum UserRole
    {
        [Display(Name = "User", Description = "User")]
        User,
        [Display(Name = "Admin", Description = "Admin")]
        Admin
    }
}
