using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MZ.Domain.Enums;
using MZ.Domain.Interfaces;

namespace MZ.Domain.Entities
{
    /// <summary>
    /// 
    /// </summary>
    [Table("User")]
    public class UserEntity : IUser
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(4, MinimumLength = 4, ErrorMessage = "Username must be 4 characters long")]
        public string Username { get; set; } = string.Empty;

        [NotMapped]
        public string Password { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        public UserRole Role { get; set; } = UserRole.User;
        public DateTime CreateDate { get; set; } = DateTime.Now;
        public DateTime LastLoginDate { get; set; } = DateTime.Now;
        public void HashPassword(string password, IInformationEncoder encoder)
        {
            PasswordHash = encoder.Hash(password);
        }

        public bool VerifyPassword(string password, IInformationEncoder encoder)
        {
            return encoder.Verify(PasswordHash, password);
        }

        public UserSettingEntity UserSetting { get; set; }
        public CalibrationEntity Calibration { get; set; }
        public FilterEntity Filter { get; set; }
        public MaterialEntity Material { get; set; }
        public ICollection<ZeffectControlEntity> Zeffect { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    [Table("UserSetting")]
    public class UserSettingEntity : IUserSetting
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public ThemeRole Theme { get; set; }
        public LanguageRole Language { get; set; }
        public ICollection<UserButtonEntity> Buttons { get; set; }

        // Foreign key
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public UserEntity User { get; set; }
    }

    [Table("UserButton")]
    public class UserButtonEntity : IUserButton
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        [Required]
        public bool IsVisibility { get; set; }

        // Foreign key
        public int UserSettingId { get; set; }

        [ForeignKey("UserSettingId")]
        public UserSettingEntity UserSetting { get; set; }
    }

}
