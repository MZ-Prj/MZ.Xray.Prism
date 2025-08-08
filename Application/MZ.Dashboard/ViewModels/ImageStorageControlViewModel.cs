using MZ.Core;
using MZ.Dashboard.Views;
using MZ.Domain.Entities;
using MZ.Model;
using MZ.DTO;
using MZ.Infrastructure;
using MZ.Loading;
using MZ.Util;
using MZ.WindowDialog;
using Prism.Commands;
using Prism.Ioc;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace MZ.Dashboard.ViewModels
{
    /// <summary>
    /// Image Storage Control ViewModel : 저장된 이미지 표기 및 검색
    /// </summary>
    public class ImageStorageControlViewModel : MZBindableBase
    {
        #region Services
        private readonly IDatabaseService _databaseService;
        private readonly ILoadingService _loadingService;
        private readonly IWindowDialogService _windowDialogService;
        #endregion

        #region Params
        private LoadingModel _loadingModel;
        public LoadingModel LoadingModel { get => _loadingModel ??= _loadingService[MZRegionNames.ImageStorageControl]; set => SetProperty(ref _loadingModel, value); }

        private ObservableCollection<ImageEntity> _images = [];
        public ObservableCollection<ImageEntity> Images { get => _images; set => SetProperty(ref _images, value); }

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

        public int CurrentPage { get; set; } = 0;
        public int PageSize { get; set; } = 50;
        #endregion

        #region Commands
        private DelegateCommand _searchCommand;
        public ICommand SearchCommand => _searchCommand ??= new DelegateCommand(MZAction.Wrapper(SearchButton));

        private DelegateCommand _refreshCommand;
        public ICommand RefreshCommand => _refreshCommand ??= new DelegateCommand(MZAction.Wrapper(RefreshButton));

        private DelegateCommand<ScrollChangedEventArgs> _scrollChangedCommand;
        public ICommand ScrollChangedCommand => _scrollChangedCommand ??= new DelegateCommand<ScrollChangedEventArgs>(ScrollChanged);

        public DelegateCommand<ImageEntity> _selectedImageCommand;
        public ICommand SelectedImageCommand => _selectedImageCommand ??= new DelegateCommand<ImageEntity>(MZAction.Wrapper<ImageEntity>(SelectedImageButton));


        #endregion

        public ImageStorageControlViewModel(IContainerExtension container, IDatabaseService databaseService, ILoadingService loadingService, IWindowDialogService windowDialogService) : base(container)
        {
            _databaseService = databaseService;
            _loadingService = loadingService;
            _windowDialogService = windowDialogService;

            base.Initialize();

        }

        public override async void InitializeModel()
        {
            await LoadingWithImage();
        }

        /// <summary>
        /// 검색 실행
        /// </summary>
        private async void SearchButton()
        {
            await LoadingWithImage();
        }

        /// <summary>
        /// 새로고침
        /// </summary>
        private async void RefreshButton()
        {
            await LoadingWithImage();
        }

        /// <summary>
        /// 이미지 선택시 호출
        /// </summary>
        /// <param name="response">ImageEntity : 선택 이미지</param>
        private async void SelectedImageButton(ImageEntity response)
        {
            await _windowDialogService.ShowWindow(
                title: response.Filename,
                regionName: nameof(ImageStorageDetailView),
                isMultiple: true,
                parameters: new()
                {
                    { "ImageEntity" , response }
                });
        }

        /// <summary>
        /// 스크롤 하단 도달시 추가 로딩
        /// </summary>
        /// <param name="args">ScrollChangedEventArgs : 스크롤 변경 이벤트</param>
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

        /// <summary>
        /// 이미지 필터 갱신
        /// </summary>
        private void UpdateSearchFilter()
        {
            FilteredImages = CollectionViewSource.GetDefaultView(Images);
            FilteredImages.Filter = FilterImages;
        }

        /// <summary>
        /// 이미지 필터(이미지 파일명 기준) 조건
        /// </summary>
        /// <param name="item">object : ImageEntity</param>
        /// <returns>조건 충족 여부</returns>
        private bool FilterImages(object item)
        {
            if (item is ImageEntity images)
            {
                if (string.IsNullOrWhiteSpace(SearchImageText))
                {
                    return true;
                }
                return images.Filename?.IndexOf(SearchImageText, StringComparison.OrdinalIgnoreCase) >= 0;
            }
            return false;
        }

        /// <summary>
        /// 이미지 로딩
        /// </summary>
        private async void LoadImages()
        {

            if (StartSelectedDate.HasValue && EndSelectedDate.HasValue)
            {
                var images = await _databaseService.Image.Load(new ImageLoadRequest(StartSelectedDate.Value, EndSelectedDate.Value, CurrentPage, PageSize));
                if (images.Success)
                {
                    Images.AddRange(images.Data);
                    CurrentPage++;
                }
            }
        }

        /// <summary>
        /// 이미지 초기화
        /// </summary>
        private void Clear()
        {
            Images.Clear();
            CurrentPage = 0;
        }

        /// <summary>
        /// 로딩 중 이미지 처리 
        /// </summary>
        /// <returns></returns>
        private async Task LoadingWithImage()
        {
            using (_loadingService[MZRegionNames.ImageStorageControl].Show())
            {
                Clear();
                LoadImages();
                UpdateSearchFilter();
                await Task.Delay(100);
            }
        }
    }
}
