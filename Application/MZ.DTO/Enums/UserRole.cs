using System.ComponentModel.DataAnnotations;

namespace MZ.DTO.Enums
{
    public enum UserLoginRole
    {
        [Display(Name = "Success", Description = "User Login Success")]
        Success = 200,
        [Display(Name = "Fail", Description = "User Login Fail")]
        Fail = 400,
        [Display(Name = "NotFound", Description = "User Information NotFound")]
        NotFound = 401,
        [Display(Name = "Valid", Description = "User Login Valid")]
        Valid = 500,
    }

    public enum UserRegisterRole
    {
        [Display(Name = "Success", Description = "User Register Success")]
        Success = 200,
        [Display(Name = "Fail", Description = "User Register Fail")]
        Fail = 400,
        [Display(Name = "NotMatch", Description = "Check Password & Repassword")]
        NotMatchPassword = 401,
        [Display(Name = "AleadyExist", Description = "User Aleady Exist")]
        AleadyExist = 402,
        [Display(Name = "Valid", Description = "User Login Valid")]
        Valid = 500,
    }
}
