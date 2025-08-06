using MahApps.Metro.IconPacks;
using MZ.Core;
using MZ.Domain.Entities;
using MZ.Domain.Models;
using MZ.DTO;
using MZ.Util;
using Prism.Commands;
using Prism.Ioc;
using Prism.Regions;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;

namespace MZ.Dashboard.ViewModels
{
    public class ImageStorageDetailViewModel : MZBindableBase
    {
        #region Params
        private ImageEntity _image = new();
        public ImageEntity Image { get => _image; set => SetProperty(ref _image, value); }

        private string _selectedPathName;
        public string SelectedPathName { get => _selectedPathName; set => SetProperty(ref _selectedPathName, value); }

        public ObservableCollection<IconButtonModel> WindowCommandButtons { get; } = [];
        #endregion

        #region Commands
        private DelegateCommand _aiOnOffCommand;
        public ICommand AIOnOffCommand => _aiOnOffCommand ??= new(MZAction.Wrapper(AIOnOffButton));
        
        private DelegateCommand _zoomInCommand;
        public ICommand ZoomInCommand => _zoomInCommand ??= new(MZAction.Wrapper(ZoomInButton));

        private DelegateCommand _zoomOutCommand;
        public ICommand ZoomOutCommand => _zoomOutCommand ??= new(MZAction.Wrapper(ZoomOutButton));

        #endregion

        public ImageStorageDetailViewModel(IContainerExtension container) : base(container)
        {
            base.Initialize();
        }

        public override void InitializeModel()
        {
            WindowCommandButtons.Add(new(nameof(PackIconMaterialKind.HeadRemoveOutline), AIOnOffCommand, name: UserSettingButtonKeys.AIOnOffButton));
            WindowCommandButtons.Add(new(nameof(PackIconMaterialKind.MagnifyMinus), ZoomOutCommand, name: UserSettingButtonKeys.ZoomOutButton));
            WindowCommandButtons.Add(new(nameof(PackIconMaterialKind.MagnifyPlus), ZoomInCommand, name: UserSettingButtonKeys.ZoomInButton));
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {   
            if (navigationContext.Parameters.ContainsKey("ImageEntity"))
            {
                Image = navigationContext.Parameters.GetValue<ImageEntity>("ImageEntity");
                SelectedPathName = Path.Combine(Image.Path, Image.Filename);
            }
        }

        private void AIOnOffButton()
        {
        }

        private void ZoomInButton()
        {
        }

        private void ZoomOutButton()
        {
        }

    }
}
