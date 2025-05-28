using MZ.Infrastructure.Interfaces;

namespace MZ.Infrastructure.Sessions
{
    public class UserSession : IUserSession
    {
        public string CurrentUser { get; set; }
    }
}
