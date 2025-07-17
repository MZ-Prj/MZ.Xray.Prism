using MZ.Core;
using MZ.Domain.Entities;
using MZ.Infrastructure;
using Prism.Ioc;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace MZ.Dashboard.ViewModels
{
    public class ImageStorageControlViewModel : MZBindableBase
    {
        #region Services
        private readonly IDatabaseService _databaseService;
        #endregion


        #region Params
        private ObservableCollection<ImageEntity> _imageFiles = [];
        public ObservableCollection<ImageEntity> ImageFiles { get => _imageFiles; set => SetProperty(ref _imageFiles, value); }

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

        public ImageStorageControlViewModel(IContainerExtension container, IDatabaseService databaseService) : base(container)
        {
            _databaseService = databaseService;

            base.Initialize();
        }

        public override void InitializeModel()
        {
            LoadImages();
        }

        private async void LoadImages()
        {
            if (StartSelectedDate.HasValue && EndSelectedDate.HasValue)
            {
                var images = await _databaseService.Image.Load(new(StartSelectedDate.Value, EndSelectedDate.Value, CurrentPage, PageSize));
                if (images != null && images.Data.Count != 0)
                {
                    foreach (var image in images.Data)
                    {
                        ImageFiles.Add(image);
                    }
                    CurrentPage++;
                }
            }
        }

    }
}
