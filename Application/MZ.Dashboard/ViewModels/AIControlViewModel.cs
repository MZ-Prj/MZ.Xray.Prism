using MZ.AI.Engine;
using MZ.Core;
using MZ.Domain.Models;
using Prism.Ioc;
using System;
using System.ComponentModel;
using System.Windows.Data;

namespace MZ.Dashboard.ViewModels
{
    public class AIControlViewModel : MZBindableBase
    {
        #region Service
        private readonly IAIService _aiService;
        #endregion

        public YoloProcessor Yolo
        {
            get => _aiService.Yolo;
            set => _aiService.Yolo = value;
        }

        private ICollectionView _filteredTexts;
        public ICollectionView FilteredTexts { get => _filteredTexts; set => SetProperty(ref _filteredTexts, value); }


        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                {
                    FilteredTexts.Refresh();
                }
            }
        }
        public AIControlViewModel(IContainerExtension container, IAIService aiService) : base(container)
        {
            _aiService = aiService;

            base.Initialize();
        }

        public override void InitializeModel()
        {
            InitializeFilter();
        }

        private void InitializeFilter()
        {
            FilteredTexts = CollectionViewSource.GetDefaultView(Yolo.Categories);
            FilteredTexts.Filter = FilterTexts;
        }

        private bool FilterTexts(object item)
        {
            if (item is CategoryModel categories)
            {
                if (string.IsNullOrWhiteSpace(SearchText))
                {
                    return true;
                }
                return categories.Name.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0;
            }
            return false;
        }
    }
}
