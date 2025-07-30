namespace MZ.DTO
{
#nullable enable

    #region Request
    public record AppSettingRegisterRequest(
        string LastestUsername,
        bool IsUsernameSave
    );

    #endregion
}
