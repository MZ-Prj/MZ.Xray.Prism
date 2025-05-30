using MZ.Auth.Models;
using MZ.Auth.Views;
using MZ.Core;
using MZ.Domain.Enums;
using MZ.Infrastructure;
using MZ.Util;
using Prism.Commands;
using Prism.Ioc;
using static MZ.Core.MZEvent;
using static MZ.Core.MZModel;

namespace MZ.Auth.ViewModels
{
    public class UserRegisterViewModel : MZBindableBase
    {
        #region Params
        private UserRegisterModel _user = new();
        public UserRegisterModel User { get => _user; set => SetProperty(ref _user, value); }
        #endregion

        #region Command
        public DelegateCommand RegisterCommand => new(MZAction.Wrapper(RegisterButton));
        public DelegateCommand BackCommand => new(MZAction.Wrapper(BackButton));
        #endregion

        #region Service
        private readonly DatabaseService _databaseService;

        #endregion

        public UserRegisterViewModel(IContainerExtension container) : base(container)
        {
            _databaseService = container.Resolve<DatabaseService>();
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
