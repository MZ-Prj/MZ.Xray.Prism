using MahApps.Metro.IconPacks;
using MZ.Core;
using MZ.Domain.Entities;
using MZ.Model;
using MZ.DTO;
using MZ.Util;
using Prism.Commands;
using Prism.Ioc;
using Prism.Regions;
using System;
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

        private ObservableCollection<ObjectDetectionModel> _objectDetections = [];
        public ObservableCollection<ObjectDetectionModel> ObjectDetections { get => _objectDetections; set => SetProperty(ref _objectDetections, value); }

        private string _selectedPathName;
        public string SelectedPathName { get => _selectedPathName; set => SetProperty(ref _selectedPathName, value); }

        private bool _aiOnOff = false;
        public bool AIOnOff { get => _aiOnOff; set => SetProperty(ref _aiOnOff, value); }

        public ObservableCollection<IconButtonModel> ActionButtons { get; } = [];
        #endregion

        #region Event
        public event Action<int> UpdateZoom;
        public event Action Refresh;
        #endregion


        #region Commands
        private DelegateCommand _aiOnOffCommand;
        public ICommand AIOnOffCommand => _aiOnOffCommand ??= new(MZAction.Wrapper(AIOnOffButton));
        
        private DelegateCommand _zoomInCommand;
        public ICommand ZoomInCommand => _zoomInCommand ??= new(MZAction.Wrapper(ZoomInButton));

        private DelegateCommand _zoomOutCommand;
        public ICommand ZoomOutCommand => _zoomOutCommand ??= new(MZAction.Wrapper(ZoomOutButton));

        private DelegateCommand _refreshCommand;
        public ICommand RefreshCommand => _refreshCommand ??= new DelegateCommand(MZAction.Wrapper(RefreshButton));
        #endregion

        public ImageStorageDetailViewModel(IContainerExtension container) : base(container)
        {
            base.Initialize();
        }

        public override void InitializeModel()
        {
            ActionButtons.Add(new(nameof(PackIconMaterialKind.MagnifyMinus), ZoomOutCommand, name: UserSettingButtonKeys.ZoomOutButton, tooltipKey: MZRegionNames.AddLng(MZRegionNames.XrayRealtimeRegion_ZoomOut)));
            ActionButtons.Add(new(nameof(PackIconMaterialKind.MagnifyPlus), ZoomInCommand, name: UserSettingButtonKeys.ZoomInButton, tooltipKey: MZRegionNames.AddLng(MZRegionNames.XrayRealtimeRegion_ZoomIn)));
            ActionButtons.Add(new(nameof(PackIconMaterialKind.Refresh), RefreshCommand, tooltipKey: MZRegionNames.AddLng(MZRegionNames.CommonRefresh)));
            ActionButtons.Add(new(nameof(PackIconMaterialKind.HeadRemoveOutline), AIOnOffCommand, name: UserSettingButtonKeys.AIOnOffButton, tooltipKey: MZRegionNames.AddLng(MZRegionNames.XrayRealtimeRegion_AIOnOff)));

        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {   
            if (navigationContext.Parameters.ContainsKey("ImageEntity"))
            {
                Image = navigationContext.Parameters.GetValue<ImageEntity>("ImageEntity");
                ObjectDetections = [.. ObjectDetectionMapper.EntitiesToModels(Image.ObjectDetections)];

                SelectedPathName = Path.Combine(Image.Path, Image.Filename);
            }
        }

        private void AIOnOffButton()
        {
            // ui
            ToggleFooterButton(AIOnOffCommand, nameof(PackIconMaterialKind.HeadRemoveOutline), nameof(PackIconMaterialKind.HeadCheckOutline), ActionButtons);

            AIOnOff = !AIOnOff;
        }

        private void ZoomInButton()
        {
            UpdateZoom?.Invoke(+1);
        }

        private void ZoomOutButton()
        {
            UpdateZoom?.Invoke(-1);
        }

        private void RefreshButton()
        {
            Refresh.Invoke();
        }


        /// <summary>
        /// 버튼 아이콘 On/Off 토글
        /// </summary>
        /// <param name="targetCommand">ICommand</param>
        /// <param name="iconOn">string</param>
        /// <param name="iconOff">string</param>
        /// <param name="buttonCollections">ObservableCollection<IconButtonModel>[]</param>
        private void ToggleFooterButton(ICommand targetCommand, string iconOn, string iconOff, params ObservableCollection<IconButtonModel>[] buttonCollections)
        {
            foreach (var collection in buttonCollections)
            {
                foreach (var button in collection)
                {
                    if (button.Command == targetCommand)
                    {
                        button.IconKind = button.IconKind == iconOff ? iconOn : iconOff;
                    }
                }
            }
        }
    }
}
