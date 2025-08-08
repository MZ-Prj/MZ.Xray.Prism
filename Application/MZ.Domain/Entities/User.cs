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
    /// 시스템 사용자 정보를 나타내는 도메인 Entity
    /// 
    /// - DB 테이블명: User
    /// - 인증, 권한, 설정 등 모든 사용자 기반 로직의 중심
    /// </summary>
    [Table("User")]
    public class UserEntity : IUser
    {
        /// <summary>
        /// PK. 사용자 고유 식별자 (Auto-increment)
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// 사용자명 (로그인 ID)
        /// - 4글자 고정
        /// - Unique, Not Null
        /// </summary>
        [Required]
        [StringLength(4, MinimumLength = 4, ErrorMessage = "Username must be 4 characters long")]
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// 평문 비밀번호 (인증 과정에서만 사용, DB에는 저장 X)
        /// </summary>
        [NotMapped]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// 암호화(해시)된 비밀번호
        /// </summary>
        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        /// <summary>
        /// 사용자 역할(권한) 정보
        /// </summary>
        public UserRole Role { get; set; } = UserRole.User;

        /// <summary>
        /// 계정 생성 일시
        /// </summary>
        public TimeSpan UsingDate { get; set; } = TimeSpan.Zero;

        /// <summary>
        /// 계정 생성 일시
        /// </summary>
        public DateTime CreateDate { get; set; } = DateTime.Now;

        /// <summary>
        /// 마지막 로그인 일시
        /// </summary>
        public DateTime LastLoginDate { get; set; } = DateTime.Now;

        /// <summary>
        /// 비밀번호를 해시로 변환하여 PasswordHash에 저장
        /// </summary>
        /// <param name="password">string : 원본 비밀번호</param>
        /// <param name="encoder">IInformationEncoder : 해시 인코더</param>
        public void HashPassword(string password, IInformationEncoder encoder)
        {
            PasswordHash = encoder.Hash(password);
        }

        /// <summary>
        /// 평문 비밀번호와 해시값의 일치 여부 검증
        /// </summary>
        /// <param name="password">string : 입력 비밀번호</param>
        /// <param name="encoder">IInformationEncoder : 해시 인코더</param>
        /// <returns>bool : 검증 결과</returns>
        public bool VerifyPassword(string password, IInformationEncoder encoder)
        {
            return encoder.Verify(PasswordHash, password);
        }

        /// <summary>
        /// User Setting (1:1)
        /// </summary>
        public UserSettingEntity UserSetting { get; set; }
        /// <summary>
        /// Calibration (1:1)
        /// </summary>
        public CalibrationEntity Calibration { get; set; }
        /// <summary>
        /// Filter (1:1)
        /// </summary>
        public FilterEntity Filter { get; set; }
        /// <summary>
        /// Material (1:1)
        /// </summary>
        public MaterialEntity Material { get; set; }
        /// <summary>
        /// ZeffectControls  (1:N)
        /// </summary>
        public ICollection<ZeffectControlEntity> Zeffect { get; set; }
        /// <summary>
        /// CurveControls  (1:N)
        /// </summary>
        public ICollection<CurveControlEntity> Curve { get; set; }
    }

    /// <summary>
    /// 사용자별 환경설정 정보 Entity
    /// 
    /// - 테마, 언어, 커스텀 버튼 등 UI/UX 관련 설정값 저장
    /// - DB 테이블명: UserSetting
    /// </summary>
    [Table("UserSetting")]
    public class UserSettingEntity : IUserSetting
    {
        /// <summary>
        /// PK. 환경설정 고유 식별자 (Auto-increment)
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        /// <summary>
        /// 사용자 테마
        /// </summary>
        public ThemeRole Theme { get; set; }
        /// <summary>
        /// 사용자 언어
        /// </summary>
        public LanguageRole Language { get; set; }
        /// <summary>
        /// 사용자 커스텀 버튼 (1:N)
        /// </summary>
        public ICollection<UserButtonEntity> Buttons { get; set; }

        /// <summary>
        /// FK. 연결된 사용자 Id
        /// </summary>
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public UserEntity User { get; set; }
    }

    /// <summary>
    /// 사용자 커스텀 버튼 Entity
    /// 
    /// - 대시보드, 화면 등에서 노출 여부/이름 등 커스텀 버튼 관리
    /// - DB 테이블명: UserButton
    /// </summary>
    [Table("UserButton")]
    public class UserButtonEntity : IUserButton
    {
        /// <summary>
        /// PK. 커스텀 버튼 고유 식별자 (Auto-increment)
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// 버튼 명칭
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// 버튼 노출 여부
        /// </summary>
        [Required]
        public bool IsVisibility { get; set; }

        /// <summary>
        /// FK. 환경설정 Id
        /// </summary>
        public int UserSettingId { get; set; }

        [ForeignKey("UserSettingId")]
        public UserSettingEntity UserSetting { get; set; }
    }

}
