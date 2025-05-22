using System;

namespace MZ.DTO
{
#nullable enable

    #region Request
    public record UserRegistrationRequest(
        string Username,
        string Email,
        string Password
    );

    public record LoginRequest(
        string Username,
        string Password
    );

    public record UserUpdateRequest(
        string? NewUsername,
        string? NewEmail
    );

    public record ChangePasswordRequest(
        string CurrentPassword,
        string NewPassword
    );
    #endregion

    #region Response
    public record UserResponse(
        bool Success,
        string? Message,
        UserDto? User
    ) : BaseResponse(Success, Message);
    #endregion

    #region User DTO
    public record UserDto(
        int Id,
        string Username,
        string Email,
        string Role,
        DateTime CreateDate
    );
    #endregion
}
