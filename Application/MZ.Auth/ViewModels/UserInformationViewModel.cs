using MZ.Core;
using MZ.Domain.Entities;
using MZ.Infrastructure;
using MZ.Infrastructure.Interfaces;
using MZ.Resource;
using MZ.Xray.Engine;
using Prism.Ioc;

namespace MZ.Auth.ViewModels
{
    public class UserInformationViewModel : MZBindableBase
    {

        #region Services
        private readonly IXrayService _xrayService;
        private readonly IDatabaseService _databaseService;
        private readonly IUserSession _userSession;
        #endregion

        #region Params
        private UserEntity _user = new();
        public UserEntity User { get => _user; set => SetProperty(ref _user, value); }

        private string buildVersion;
        public string BuildVersion { get => buildVersion; set => SetProperty(ref buildVersion, value); }

        public UIProcesser UI
        {
            get => _xrayService.UI;
            set => _xrayService.UI = value;
        }

        #endregion
        public UserInformationViewModel(IContainerExtension container, IDatabaseService databaseService, IUserSession userSession, IXrayService xrayService) : base(container)
        {
            _databaseService = databaseService;
            _userSession = userSession;
            _xrayService = xrayService;

            base.Initialize();
        }

        public override async void InitializeModel()
        {
            BuildVersion = BuildVersionService.BuildVersion;

            var user = await _databaseService.User.GetUser();
            if (user.Success)
            {
                User = user.Data;
                _xrayService.UI.StartUsingDate(User.UsingDate, _userSession.LoginTime);
            }
        }
    }
}
