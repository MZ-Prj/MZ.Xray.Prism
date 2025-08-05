using MZ.Domain.Entities;
using MZ.Domain.Enums;
using MZ.Domain.Models;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

#nullable enable
namespace MZ.DTO
{

    #region Request
    /// <summary>
    /// 사용자 로그인 요청 DTO
    /// </summary>
    /// <param name="Username">로그인할 사용자 ID(필수, 4자 고정)</param>
    /// <param name="Password">비밀번호(PlainText, 필수)</param>
    public record UserLoginRequest(
        string Username,
        string Password
    );

    /// <summary>
    /// 사용자 회원가입 요청 DTO
    /// </summary>
    /// <param name="Username">회원가입할 사용자 ID(필수, 4자 고정)</param>
    /// <param name="Password">비밀번호(PlainText, 필수)</param>
    /// <param name="RePassword">비밀번호 확인(PlainText, 필수)</param>
    /// <param name="Email">이메일 주소(필수)</param>
    /// <param name="UserRole">사용자 권한(일반/관리자)</param>
    public record UserRegisterRequest(
        string Username,
        string Password,
        string RePassword,
        string Email,
        UserRole UserRole
    );
    /// <summary>
    /// 사용자 환경설정 저장 요청 DTO
    /// </summary>
    /// <param name="Theme">사용자 선택 테마</param>
    /// <param name="Language">사용자 선택 언어</param>
    /// <param name="Buttons">사용자 지정 액션버튼 목록</param>
    public record UserSettingSaveRequest
    (
        ThemeRole Theme,
        LanguageRole Language,
        ICollection<UserButtonEntity> Buttons
    );

    #endregion

    #region Mapper
    /// <summary>
    /// UserSetting Mapper 유틸리티 클래스
    /// </summary>
    public static class UserSettingMapper
    {
        /// <summary>
        /// IconButtonModel 리스트를 UserButtonEntity로 변환하여 UserSettingSaveRequest 생성
        /// </summary>
        /// <param name="theme">테마</param>
        /// <param name="language">언어</param>
        /// <param name="buttons">UI 상의 아이콘버튼 정보</param>
        /// <returns>UserSettingSaveRequest</returns>
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

    #endregion
    /// <summary>
    /// UserSetting에서 사용하는 사용자 버튼 Key 값 정의 상수 클래스
    /// </summary>
    public static class UserSettingButtonKeys
    {
        /// <summary>화면 축소 버튼</summary>
        public const string ZoomOutButton = "ZoomOutButton";
        /// <summary>화면 확대 버튼</summary>
        public const string ZoomInButton = "ZoomInButton";
        /// <summary>Grayscale 버튼</summary>
        public const string GrayButton = "GrayButton";
        /// <summary>Color 버튼</summary>
        public const string ColorButton = "ColorButton";
        /// <summary>Organic 버튼</summary>
        public const string OrganicButton = "OrganicButton";
        /// <summary>Inorganic 버튼</summary>
        public const string InorganicButton = "InorganicButton";
        /// <summary>Metal 버튼</summary>
        public const string MetalButton = "MetalButton";
        /// <summary>밝기 - 버튼</summary>
        public const string BrightDownButton = "BrightDownButton";
        /// <summary>밝기 + 버튼</summary>
        public const string BrightUpButton = "BrightUpButton";
        /// <summary>명암 - 버튼</summary>
        public const string ContrastDownButton = "ContrastDownButton";
        /// <summary>명암 + 버튼</summary>
        public const string ContrastUpButton = "ContrastUpButton";
        /// <summary>필터 초기화 버튼</summary>
        public const string FilterClearButton = "FilterClearButton";
        /// <summary>Z-effect 버튼</summary>
        public const string ZeffectButton = "ZeffectButton";
        /// <summary>AI On/Off 버튼</summary>
        public const string AIOnOffButton = "AIOnOffButton";
        /// <summary>이미지 저장 버튼</summary>
        public const string SaveImageButton = "SaveImageButton";

        /// <summary>
        /// 모든 버튼 키 문자열 배열 반환
        /// </summary>
        public static string[] GetAllKeys()
        {
            return [.. typeof(UserSettingButtonKeys)
                .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                .Where(fi => fi.IsLiteral && !fi.IsInitOnly && fi.FieldType == typeof(string))
                .Select(fi => (string)fi.GetRawConstantValue()!)];
        }
    }
}
