using MZ.AI.Engine;
using MZ.Core;
using Prism.Ioc;

namespace MZ.Dashboard.ViewModels
{
    public class AIControlViewModel : MZBindableBase
    {
        #region Service
        private readonly IAIService _aiService;
        #endregion

        private string _searchLabelText;
        public string SearchLabelText { get => _searchLabelText; set => SetProperty(ref _searchLabelText, value); }

        public AIControlViewModel(IContainerExtension container, IAIService aiService) : base(container)
        {
            _aiService = aiService; 
        }

    }
}
