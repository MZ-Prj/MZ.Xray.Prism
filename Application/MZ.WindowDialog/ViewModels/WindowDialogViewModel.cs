using MZ.Core;
using MZ.Domain.Models;
using MZ.Loading;
using Prism.Ioc;
using System;

namespace MZ.WindowDialog.ViewModels
{
    public class WindowDialogViewModel : MZBindableBase
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
        public LoadingModel LoadingModel { get => _loadingModel ??= _loadingService[MZRegionNames.WindowDialogRegion]; set => SetProperty(ref _loadingModel, value); }
        #endregion

        public WindowDialogViewModel(IContainerExtension container, ILoadingService loadingService) : base(container)
        {
            _loadingService = loadingService;
        }

        
    }
}
