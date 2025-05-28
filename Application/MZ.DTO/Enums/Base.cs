
using System.ComponentModel.DataAnnotations;

namespace MZ.DTO.Enums
{
    public enum BaseRole
    {
        [Display(Name = "Success", Description = "Success")]
        Success = 200,
        [Display(Name = "Fail", Description = "Fail")]
        Fail = 400,
        [Display(Name = "Valid", Description = "Valid")]
        Valid = 500,
    }
}
