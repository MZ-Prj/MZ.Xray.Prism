using System.ComponentModel.DataAnnotations;

namespace MZ.Domain.Enums
{
    /// <summary>
    /// 시스템 사용자 권한/역할 구분 Enum
    /// 
    /// - DB User 테이블, 인증/권한 분기, 메뉴/기능 제한 등에서 활용
    /// - Name: 시스템 코드/영문역할
    /// - Description: 상세 설명
    /// </summary>
    public enum UserRole
    {
        /// <summary>
        /// 일반 사용자  
        /// - 기능 제한 있음 (설정, 관리, 시스템 권한 X)
        /// - 주로 데이터 확인, 기본 기능 사용
        /// </summary>
        [Display(Name = "User", Description = "User")]
        User,

        /// <summary>
        /// 관리자  
        /// - 모든 메뉴/설정/사용자 관리 가능  
        /// - 시스템 유지보수, 고급 기능 접근 가능
        /// </summary>
        [Display(Name = "Admin", Description = "Admin")]
        Admin
    }
}
