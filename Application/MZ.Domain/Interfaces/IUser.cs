using MZ.Domain.Enums;
using System;

namespace MZ.Domain.Interfaces
{
    public interface IUser
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string PasswordHash { get; set; }
        public string Email { get; set; } 
        public UserRole Role { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime LastLoginDate { get; set; }
    }

    public interface IUserSetting
    {
        public ThemeRole Theme { get; set; }
        public LanguageRole Language { get; set; }
    }
}
