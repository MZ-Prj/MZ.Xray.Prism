using MZ.AI.Engine;
using MZ.Core;
using MZ.Domain.Models;
using Prism.Ioc;
using System;
using System.ComponentModel;
using System.Windows.Data;

namespace MZ.Dashboard.ViewModels
{
    /// <summary>
    /// AI Control ViewModel : 인공지능 카테고리 및 식별율 제어 담당
    /// </summary>
    public class AIControlViewModel : MZBindableBase
    {
        #region Service
        private readonly IAIService _aiService;

        public YoloProcessor Yolo
        {
            get => _aiService.Yolo;
            set => _aiService.Yolo = value;
        }
        #endregion

        #region Params
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
        #endregion

        public AIControlViewModel(IContainerExtension container, IAIService aiService) : base(container)
        {
            _aiService = aiService;

            base.Initialize();
        }

        public override void InitializeModel()
        {
            InitializeFilter();
        }


        /// <summary>
        /// Yolo 카테고리를 필터링 뷰로 래핑 & 필터 조건 설정
        /// </summary>
        private void InitializeFilter()
        {
            FilteredTexts = CollectionViewSource.GetDefaultView(Yolo.Categories);
            FilteredTexts.Filter = FilterTexts;
        }

        /// <summary>
        /// 검색 필터 : 검색 항목만 표기 수행 
        /// </summary>
        /// <param name="item">CategoryModel</param>
        /// <returns></returns>
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
