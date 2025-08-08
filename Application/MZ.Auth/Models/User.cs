using Prism.Mvvm;

namespace MZ.Auth.Models
{
    public class UserModel : BindableBase
    {
        private string _username;
        public string Username { get => _username; set => SetProperty(ref _username, value); }

        private string _password;
        public string Password { get => _password; set => SetProperty(ref _password, value); }

        private string _message = string.Empty;
        public string Message { get => _message; set => SetProperty(ref _message, value); }

        private bool _messageVisibility = false;
        public bool MessageVisibility { get => _messageVisibility; set => SetProperty(ref _messageVisibility, value); }

        private bool _isUsernameSave = false;
        public bool IsUsernameSave { get => _isUsernameSave; set => SetProperty(ref _isUsernameSave, value); }
    }

    public class UserRegisterModel : UserModel
    {
        private string _repassword;
        public string Repassword { get => _repassword; set => SetProperty(ref _repassword, value); }

        private bool _isAdmin;
        public bool IsAdmin { get => _isAdmin; set => SetProperty(ref _isAdmin, value); }

    }
}
