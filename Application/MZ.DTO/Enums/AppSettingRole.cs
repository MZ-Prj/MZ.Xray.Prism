using System.ComponentModel.DataAnnotations;

namespace MZ.DTO.Enums
{
    public enum AppSettingRole
    {
        [Display(Name = "Success", Description = "App Setting Success")]
        Success = 200,
        [Display(Name = "Fail", Description = "App Setting Fail")]
        Fail = 400,
        [Display(Name = "Valid", Description = "App Setting Valid")]
        Valid = 500,
    }
}
