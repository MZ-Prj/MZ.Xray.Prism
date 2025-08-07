using System;

namespace MZ.Infrastructure.Interfaces
{
    /// <summary>
    /// 사용자 세션 관리 인터페이스
    /// </summary>
    public interface IUserSession
    {
        /// <summary>
        /// 현재 로그인한 사용자명
        /// </summary>
        string CurrentUser { get; set; }

        /// <summary>
        /// 로그인할 때 시간
        /// </summary>
        DateTime LoginTime { get; set; }

        /// <summary>
        /// 모든 세션 정보 초기화
        /// </summary>
        void ClearAll();
    }
}
