using Prism.Ioc;
using Prism.Commands;
using Prism.Regions;
using System;
using System.Windows.Input;
using MZ.DTO;
using MZ.Core;
using MZ.Auth.Views;
using MZ.Auth.Models;
using MZ.Infrastructure;
using MZ.Util;
using MZ.Logger;
using static MZ.Core.MZModel;
using static MZ.Event.MZEvent;

namespace MZ.Auth.ViewModels
{
    public class UserLoginViewModel : MZBindableBase
    {

        #region Services
        private readonly IDatabaseService _databaseService;
        #endregion


        #region Params
        private UserModel _user = new();
        public UserModel User { get => _user; set => SetProperty(ref _user, value); }
        #endregion

        #region Command

        private DelegateCommand _loginCommand;
        public ICommand LoginCommand => _loginCommand ??= new (MZAction.Wrapper(LoginButton));

        private DelegateCommand _registerCommand;
        public ICommand RegisterCommand => _registerCommand ??= new(MZAction.Wrapper(RegisterButton));
        #endregion

        public UserLoginViewModel(IContainerExtension container, IDatabaseService databaseService) : base(container)
        {
            _databaseService = databaseService;

            base.Initialize();  
        }

        public override async void OnNavigatedTo(NavigationContext context)
        {
            try
            {
                var response = await _databaseService.AppSetting.GetAppSetting();
                if (response.Success)
                {
                    User.Username = response.Data.IsUsernameSave ? response.Data.LastestUsername : string.Empty;
                    User.Password = string.Empty;
                    User.IsUsernameSave = response.Data.IsUsernameSave;
                }
            }
            catch (Exception ex)
            {
                MZLogger.Error(ex.ToString());
            }
        }

        private async void LoginButton()
        {
            var response = await _databaseService.User.Login(new(User.Username, User.Password));

            User.MessageVisibility = !response.Success;
            User.Message = MZEnum.GetDescription(response.Code);

            if (response.Success)
            {
                await _databaseService.AppSetting.Register(
                    new AppSettingRegisterRequest(User.Username, User.IsUsernameSave)
                );
                _eventAggregator.GetEvent<DashboardNavigationEvent>().Publish(
                            new NavigationModel(
                                MZRegionNames.DashboardRegion,
                                MZViewNames.DashboardControlView));
            }
        }

        private void RegisterButton()
        {
            _eventAggregator.GetEvent<DashboardNavigationEvent>().Publish(
                            new NavigationModel(
                                MZRegionNames.DashboardRegion,
                                nameof(UserRegisterView)));
        }
    }
}
