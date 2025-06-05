namespace MZ.Infrastructure.Interfaces
{
    public interface IUserSession
    {
        string CurrentUser { get; set; }
        void ClearAll();
    }
}
