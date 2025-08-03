using MZ.Core;
using MZ.Domain.Models;
using MZ.DTO;
using MZ.Infrastructure;
using MZ.Loading;
using MZ.Util;
using Prism.Commands;
using Prism.Ioc;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace MZ.Dashboard.ViewModels
{
    public class ImageStorageControlViewModel : MZBindableBase
    {
        #region Services
        private readonly IDatabaseService _databaseService;
        private readonly ILoadingService _loadingService;
        #endregion

        #region Params
        private LoadingModel _loadingModel;
        public LoadingModel LoadingModel { get => _loadingModel ??= _loadingService[MZRegionNames.ImageStorageControl]; set => SetProperty(ref _loadingModel, value); }

        private ObservableCollection<ImageLoadResponse> _images = [];
        public ObservableCollection<ImageLoadResponse> Images { get => _images; set => SetProperty(ref _images, value); }

        private ICollectionView _filteredImages;
        public ICollectionView FilteredImages { get => _filteredImages; set => SetProperty(ref _filteredImages, value); }

        private string _searchImageText;
        public string SearchImageText
        {
            get => _searchImageText;
            set
            {
                if (SetProperty(ref _searchImageText, value))
                {
                    FilteredImages.Refresh();
                }
            }
        }

        private DateTime? _startSelectedDate = DateTime.Today;
        public DateTime? StartSelectedDate { get => _startSelectedDate; set => SetProperty(ref _startSelectedDate, value); }

        private DateTime? _endSelectedDate = DateTime.Today.AddDays(1).AddSeconds(-1);
        public DateTime? EndSelectedDate
        {
            get => _endSelectedDate;
            set
            {
                if (value.HasValue)
                {
                    var endOfDay = value.Value.Date.AddDays(1).AddSeconds(-1);
                    SetProperty(ref _endSelectedDate, endOfDay);
                }
                else
                {
                    SetProperty(ref _endSelectedDate, null);
                }
            }
        }

        private string _selectedPathName;
        public string SelectedPathName { get => _selectedPathName; set => SetProperty(ref _selectedPathName, value); }

        private bool _selectedImageVisibility = false;
        public bool SelectedImageVisibility { get => _selectedImageVisibility; set => SetProperty(ref _selectedImageVisibility, value); }

        public int CurrentPage { get; set; } = 0;
        public int PageSize { get; set; } = 50;
        #endregion

        #region Commands
        private DelegateCommand _searchCommand;
        public ICommand SearchCommand => _searchCommand ??= new DelegateCommand(MZAction.Wrapper(SearchButton));

        private DelegateCommand _refreshCommand;
        public ICommand RefreshCommand => _refreshCommand ??= new DelegateCommand(MZAction.Wrapper(RefreshButton));

        private DelegateCommand _closeCommand;
        public ICommand CloseCommand => _closeCommand ??= new DelegateCommand(MZAction.Wrapper(CloseButton));


        private DelegateCommand<ScrollChangedEventArgs> _scrollChangedCommand;
        public ICommand ScrollChangedCommand => _scrollChangedCommand ??= new DelegateCommand<ScrollChangedEventArgs>(ScrollChanged);

        public DelegateCommand<ImageLoadResponse> _selectedImageCommand;
        public ICommand SelectedImageCommand => _selectedImageCommand ??= new DelegateCommand<ImageLoadResponse>(MZAction.Wrapper<ImageLoadResponse>(SelectedImageButton));


        #endregion


        public ImageStorageControlViewModel(IContainerExtension container, IDatabaseService databaseService, ILoadingService loadingService) : base(container)
        {
            _databaseService = databaseService;
            _loadingService = loadingService;

            base.Initialize();

        }

        public override async void InitializeModel()
        {
            using (_loadingService[MZRegionNames.ImageStorageControl].Show())
            {
                Clear();
                LoadImages();
                UpdateSearchFilter();
                await Task.Delay(100);
            }

        }

        private async void SearchButton()
        {
            using (_loadingService[MZRegionNames.ImageStorageControl].Show())
            {
                Clear();
                LoadImages();
                UpdateSearchFilter();
                await Task.Delay(100);
            }
        }

        private void RefreshButton()
        {
            Clear();
            LoadImages();
            UpdateSearchFilter();
        }
        private void CloseButton()
        {
            SelectedImageVisibility = false;
        }

        private void SelectedImageButton(ImageLoadResponse response)
        {
            SelectedPathName = response.PathName;
            SelectedImageVisibility = true;
        }

        private void ScrollChanged(ScrollChangedEventArgs args)
        {
            if (args.ExtentHeight > args.ViewportHeight)
            {
                if (args.VerticalOffset + args.ViewportHeight >= args.ExtentHeight - 20)
                {
                    LoadImages();
                }
            }
        }

        private void UpdateSearchFilter()
        {
            FilteredImages = CollectionViewSource.GetDefaultView(Images);
            FilteredImages.Filter = FilterImages;
        }

        private bool FilterImages(object item)
        {
            if (item is ImageLoadResponse images)
            {
                if (string.IsNullOrWhiteSpace(SearchImageText))
                {
                    return true;
                }
                return images.Filename?.IndexOf(SearchImageText, StringComparison.OrdinalIgnoreCase) >= 0;
            }
            return false;
        }

        private async void LoadImages()
        {

            if (StartSelectedDate.HasValue && EndSelectedDate.HasValue)
            {
                var images = await _databaseService.Image.Load(new ImageLoadRequest(StartSelectedDate.Value, EndSelectedDate.Value, CurrentPage, PageSize));
                if (images != null && images.Data.Count != 0)
                {
                    foreach (var image in images.Data)
                    {
                        Images.Add(image);
                    }
                    CurrentPage++;
                }
            }
        }

        private void Clear()
        {
            Images.Clear();
            CurrentPage = 0;
        }

    }
}
