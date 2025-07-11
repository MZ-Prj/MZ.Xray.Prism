using Prism.Commands;
using System;
using MZ.Core;
using MZ.Util;
using MZ.Domain.Models;
using MZ.Loading;
using Prism.Ioc;
using Prism.Services.Dialogs;
using System.Windows.Input;
using Prism.Regions;

namespace MZ.Dialog.ViewModels
{
    public class DialogViewModel : MZDialogBindableBase, IDialogAware
    {

        #region Services
        private readonly ILoadingService _loadingService;
        #endregion

        #region Models
        private string _title;
        public string Title { get => _title; set => SetProperty(ref _title, value); }

        private string _regionName;
        public string RegionName { get => _regionName; set => SetProperty(ref _regionName, value); }

        private LoadingModel _loadingModel;
        public LoadingModel LoadingModel { get => _loadingModel ??= _loadingService[MZRegionNames.DialogRegion]; set => SetProperty(ref _loadingModel, value); }
        #endregion

        #region Command
        public event Action<IDialogResult> RequestClose;

        // 닫기 버튼을 별도로 만들경우 사용
        private DelegateCommand _closingCommand;
        public ICommand ClosingCommand => _closingCommand ??= new DelegateCommand(MZAction.Wrapper(ClosingButton));

        #endregion

        public DialogViewModel(IContainerExtension container, ILoadingService loadingService) : base(container)
        {
            _loadingService = loadingService;
        }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            if (parameters.ContainsKey("RegionName"))
            {
                RegionName = parameters.GetValue<string>("RegionName");
            }

            if (parameters.ContainsKey("Title"))
            {
                Title = parameters.GetValue<string>("Title");
            }
        }

        private void ClosingButton()
        {
            RequestClose?.Invoke(new DialogResult(ButtonResult.Cancel));
        }

        private NavigationParameters ToNavigationParameters(IDialogParameters parameters)
        {
            var param = new NavigationParameters();
            foreach (var key in parameters.Keys)
            {
                param.Add(key, parameters.GetValue<object>(key));
            }
            return param;
        }

        private string ChangedReigionToViewName(string region)
        {
            return region.Replace("Region", "View");
        }
    }
}
