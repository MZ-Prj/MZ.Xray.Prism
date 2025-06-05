using MZ.Infrastructure.Interfaces;
using System.Collections.Generic;

namespace MZ.Infrastructure.Sessions
{
    public class UserSession : IUserSession
    {
        public string CurrentUser { get; set; }
        /// <summary>
        /// 세션에 데이터를 추가 시킬때 해당 부분을 기준으로 확장
        /// </summary>
        public Dictionary<string, object> SessionData { get; } = [];

        public void ClearAll()
        {
            CurrentUser = null; 
            SessionData.Clear(); 
        }
    }
}
