using MZ.Auth.Models;
using MZ.Core;
using MZ.Infrastructure;
using MZ.Util;
using Prism.Ioc;
using Prism.Commands;
using System;
using MZ.Logger;

namespace MZ.Auth.ViewModels
{
    public class UserLoginViewModel : MZBindableBase
    {
        #region Params
        private UserModel _user = new();
        public UserModel User { get => _user; set => SetProperty(ref _user, value); }
        #endregion

        #region Command
        public DelegateCommand LoginCommand => new (MZAction.Wrapper(LoginButton));
        public DelegateCommand RegisterCommand => new (MZAction.Wrapper(RegisterButton));
        #endregion

        private readonly DatabaseService _databaseService;
        public UserLoginViewModel(IContainerExtension container) : base(container)
        {
            _databaseService = container.Resolve<DatabaseService>();
        }

        public override async void InitializeCore()
        {
            try
            {
                //var response = await _databaseService.AppSetting.GetAppSetting();
                //var data = response.Data;

                //User.Username = data.IsUsernameSave ? data.LastestUsername : string.Empty;
                //User.IsUsernameSave = data.IsUsernameSave;
            }
            catch (Exception ex)
            {
                MZLogger.Error(ex.ToString());
            }
            
        }

        private async void LoginButton()
        {
            //var response = await _databaseService.User.Login(new(User.Username, User.Password));

            //User.MessageVisibility = !response.Success;
            //User.Message = MZEnum.GetDescription(response.Code);

            //if (response.Success)
            //{
            //}
        }

        private void RegisterButton()
        {
            //_eventAggregator.GetEvent<UserRegisterEvent>().Publish();
        }
    }
}
