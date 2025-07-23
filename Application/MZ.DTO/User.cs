using MZ.Domain.Enums;
using System;

namespace MZ.DTO
{
#nullable enable

    #region Request
    public record UserLoginRequest(
        string Username,
        string Password
    );

    public record UserRegisterRequest(
        string Username,
        string Password,
        string RePassword,
        string Email,
        UserRole UserRole
    );

    #endregion

    #region Response
    //public record UserResponse(
    //    bool Success,
    //    string? Message,
    //    UserDto? User
    //) : BaseResponse(Success, Message);
    #endregion

    #region DTO
    public record UserDto(
        int Id,
        string Username,
        string Email,
        string Role,
        DateTime CreateDate
    );
    #endregion
}
