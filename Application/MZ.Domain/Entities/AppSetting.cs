using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MZ.Domain.Entities
{
    /// <summary>
    /// 애플리케이션 환경설정 정보 Entity
    /// 
    /// - 사용자 환경설정, 로그인 관련 설정 등 클라이언트 로컬 환경정보 저장용
    /// - DB 테이블명 : AppSetting
    /// </summary>
    [Table("AppSetting")]
    public class AppSettingEntity
    {
        /// <summary>
        /// PK. 고유 식별자 (Auto-Increment)
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        /// <summary>
        /// 마지막 로그인 사용자 저장 여부
        /// </summary>
        public bool IsUsernameSave { get; set; }
        /// <summary>
        /// 마지막 로그인 시도한 사용자명
        /// </summary>
        public string LastestUsername { get; set; }
    }
}
