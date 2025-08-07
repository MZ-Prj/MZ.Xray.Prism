using MZ.Core;
using MZ.Domain.Entities;
using MZ.Infrastructure;
using MZ.Infrastructure.Interfaces;
using MZ.Infrastructure.Sessions;
using MZ.Resource;
using Prism.Ioc;
using System;
using System.Timers;

namespace MZ.Auth.ViewModels
{
    public class UserInformationViewModel : MZBindableBase
    {

        #region Services
        private readonly IDatabaseService _databaseService;
        private readonly IUserSession _userSession;
        #endregion

        #region Params
        private UserEntity _user = new();
        public UserEntity User { get => _user; set => SetProperty(ref _user, value); }

        private string buildVersion;
        public string BuildVersion { get => buildVersion; set => SetProperty(ref buildVersion, value); }

        private TimeSpan _usingDate;
        public TimeSpan UsingDate { get => _usingDate; set => SetProperty(ref _usingDate, value); }

        private Timer _usingDateTimer;
        #endregion
        public UserInformationViewModel(IContainerExtension container, IDatabaseService databaseService, IUserSession userSession) : base(container)
        {
            _databaseService = databaseService;
            _userSession = userSession;

            base.Initialize();
        }

        public override async void InitializeModel()
        {
            BuildVersion = BuildVersionService.BuildVersion;

            var user = await _databaseService.User.GetUser();
            if (user.Success)
            {
                User = user.Data;
                UsingDate = User.UsingDate;

                StartUsingDateTimer();
            }
        }

        /// <summary>
        /// 실제 사용시간 누계 표시를 위함
        /// </summary>
        private void StartUsingDateTimer()
        {
            UsingDate += (DateTime.Now - _userSession.LoginTime);

            _usingDateTimer = new Timer(1000);
            _usingDateTimer.Elapsed += (s, e) =>
            {
                UsingDate += TimeSpan.FromSeconds(1);
            };
            _usingDateTimer.AutoReset = true;
            _usingDateTimer.Start();
        }
    }
}
