using MZ.Domain.Enums;
using System;

namespace MZ.Domain.Interfaces
{
    /// <summary>
    /// 사용자(User) 정보 모델용 인터페이스  
    /// 
    /// - 회원가입, 로그인, 사용자 관리, 권한관리 등에서 공통으로 사용  
    /// - Entity, DTO, ViewModel 등에서 구현
    /// </summary>
    public interface IUser
    {
        /// <summary>
        /// 사용자 아이디(로그인용)  
        /// - 4자 영문/숫자 등 제한  
        /// - DB, 인증 등에서 PK 역할 가능
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// 사용자 입력 비밀번호  
        /// - 평문 상태로 일시 보유(전송, 검증 용도)  
        /// - 실제 DB에는 저장 X (보안상 해시값만 저장)
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 해시 처리된 비밀번호(암호화 값)  
        /// - DB에는 이 값만 저장  
        /// - 로그인 등 검증시 해시 후 비교
        /// </summary>
        public string PasswordHash { get; set; }
        /// <summary>
        /// 사용자 권한/역할  
        /// - User, Admin 등 Enum  
        /// </summary>
        public UserRole Role { get; set; }
        /// <summary>
        /// 가입 일시
        /// </summary>
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// 마지막 로그인 일시
        /// </summary>
        public DateTime LastLoginDate { get; set; }
    }

    /// <summary>
    /// 사용자별 환경설정(테마, 언어) 인터페이스  
    /// 
    /// - Theme, Language 개인별 커스터마이징 정보
    /// - User와 1:1 매핑
    /// </summary>
    public interface IUserSetting
    {
        /// <summary>
        /// UI 테마(다크/라이트)
        /// </summary>
        public ThemeRole Theme { get; set; }
        /// <summary>
        /// 언어/로케일(ko-KR, en-US)
        /// </summary>
        public LanguageRole Language { get; set; }
    }

    /// <summary>
    /// 사용자 지정 퀵버튼/액션 설정 인터페이스
    /// 
    /// - 대시보드 하단, 액션 버튼 표시/숨김 여부
    /// </summary>
    public interface IUserButton
    {
        /// <summary>
        /// 버튼 표시 여부
        /// </summary>
        bool IsVisibility { get; set; }
        /// <summary>
        /// 버튼 이름/키(예: ZoomIn, AIOnOff 등)
        /// </summary>
        string Name { get; set; }
    }
}
