
using System.ComponentModel.DataAnnotations;

namespace MZ.DTO.Enums
{
    public enum BaseRole
    {
        [Display(Name = "Warning", Description = "Warning")]
        Warning = 199,
        [Display(Name = "Success", Description = "Success")]
        Success = 200,
        [Display(Name = "Fail", Description = "Fail")]
        Fail = 400,
        [Display(Name = "Valid", Description = "Valid")]
        Valid = 500,
    }
}
