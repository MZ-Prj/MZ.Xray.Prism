using MZ.Domain.Entities;
using MZ.Domain.Enums;
using MZ.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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

    public record UserSettingSaveRequest
    (
        ThemeRole Theme,
        LanguageRole Language,
        ICollection<UserButtonEntity> Buttons
    );

    #endregion

    #region Response
    #endregion

    public static class UserSettingMapper
    {
        public static UserSettingSaveRequest ModelToRequest(ThemeRole theme, LanguageRole language, ICollection<IconButtonModel> buttons)
        {
            var userButtons = buttons.Select(b => new UserButtonEntity{
                Id = b.Id,
                Name = b.Name,
                IsVisibility = b.IsVisibility
            }).ToList();

            return new UserSettingSaveRequest(
                Theme:theme,
                Language:language,
                Buttons: userButtons
            );
        }
    }

    public static class UserSettingButtonKeys
    {
        public const string ZoomOutButton = "ZoomOutButton";
        public const string ZoomInButton = "ZoomInButton";
        public const string GrayButton = "GrayButton";
        public const string ColorButton = "ColorButton";
        public const string OrganicButton = "OrganicButton";
        public const string InorganicButton = "InorganicButton";
        public const string MetalButton = "MetalButton";
        public const string BrightDownButton = "BrightDownButton";
        public const string BrightUpButton = "BrightUpButton";
        public const string ContrastDownButton = "ContrastDownButton";
        public const string ContrastUpButton = "ContrastUpButton";
        public const string FilterClearButton = "FilterClearButton";
        public const string ZeffectButton = "ZeffectButton";
        public const string AIOnOffButton = "AIOnOffButton";
        public const string SaveImageButton = "SaveImageButton";

        public static string[] GetAllKeys()
        {
            return [.. typeof(UserSettingButtonKeys)
                .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                .Where(fi => fi.IsLiteral && !fi.IsInitOnly && fi.FieldType == typeof(string))
                .Select(fi => (string)fi.GetRawConstantValue()!)];
        }
    }
}
