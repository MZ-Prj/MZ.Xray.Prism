using MZ.Auth.Models;
using MZ.Auth.Views;
using MZ.Core;
using MZ.Domain.Enums;
using MZ.Infrastructure;
using MZ.Util;
using Prism.Commands;
using Prism.Ioc;
using System.Windows.Input;
using static MZ.Core.MZEvent;
using static MZ.Core.MZModel;

namespace MZ.Auth.ViewModels
{
    public class UserRegisterViewModel : MZBindableBase
    {

        #region Service
        private readonly IDatabaseService _databaseService;

        #endregion


        #region Params
        private UserRegisterModel _user = new();
        public UserRegisterModel User { get => _user; set => SetProperty(ref _user, value); }
        #endregion

        #region Command
        public DelegateCommand _registerCommand;
        public ICommand RegisterCommand => _registerCommand ??= new(MZAction.Wrapper(RegisterButton));
        public DelegateCommand _backCommand;
        public ICommand BackCommand => _backCommand ??= new(MZAction.Wrapper(BackButton));
        #endregion

        public UserRegisterViewModel(IContainerExtension container) : base(container)
        {
            _databaseService = container.Resolve<DatabaseService>();

            base.Initialize();
        }

        private async void RegisterButton()
        {
            var response = await _databaseService.User.Register(new(User.Username, User.Password, User.Repassword, User.Email,  User.IsAdmin ? UserRole.Admin : UserRole.User ));

            User.MessageVisibility = !response.Success;
            User.Message = MZEnum.GetDescription(response.Code);

            if (response.Success)
            {
                _eventAggregator.GetEvent<DashboardNavigationEvent>().Publish(
                            new NavigationModel(
                                MZRegionNames.DashboardRegion,
                                nameof(UserLoginView)));
            }
        }

        private void BackButton()
        {
            _eventAggregator.GetEvent<DashboardNavigationEvent>().Publish(
                new NavigationModel(
                    MZRegionNames.DashboardRegion,
                    nameof(UserLoginView)));
        }
    }
}
