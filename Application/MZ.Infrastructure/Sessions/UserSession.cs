using MZ.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;

namespace MZ.Infrastructure.Sessions
{
    /// <summary>
    /// 사용자 세션 관리
    /// </summary>
    public class UserSession : IUserSession
    {
        /// <summary>
        /// 현재 로그인한 사용자명
        /// </summary>
        public string CurrentUser { get; set; }
        /// <summary>
        /// 세션에 데이터를 추가 시킬때 해당 부분을 기준으로 확장
        /// </summary>
        public Dictionary<string, object> SessionData { get; } = [];

        /// <summary>
        /// 로그인할 때 시간
        /// </summary>
        public DateTime LoginTime { get; set; }

        /// <summary>
        /// 모든 세션 정보 초기화
        /// </summary>
        public void ClearAll()
        {
            CurrentUser = null;
            LoginTime = DateTime.MinValue;
            SessionData.Clear(); 
        }
    }
}
