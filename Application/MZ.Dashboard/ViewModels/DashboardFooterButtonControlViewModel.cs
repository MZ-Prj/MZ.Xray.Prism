using MZ.Core;
using MZ.Model;
using Prism.Ioc;
using Prism.Regions;
using System.Collections.ObjectModel;

namespace MZ.Dashboard.ViewModels
{
    /// <summary>
    /// Dashboard FooterButton Control ViewModel : Xray RealTime View의 Action 버튼 제어
    /// </summary>
    public class DashboardFooterButtonControlViewModel : MZBindableBase
    {

        #region Params
        private ObservableCollection<IconButtonModel> _actionButtons;
        public ObservableCollection<IconButtonModel> ActionButtons { get => _actionButtons; set => SetProperty(ref _actionButtons, value); }
        #endregion

        public DashboardFooterButtonControlViewModel(IContainerExtension container) : base(container)
        {
        }

        /// <summary>
        /// Navigation 진입시 Parameter목록을 받음
        /// </summary>
        /// <param name="navigationContext"></param>
        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            if (navigationContext.Parameters.ContainsKey("ActionButtons"))
            {
                ActionButtons = navigationContext.Parameters["ActionButtons"] as ObservableCollection<IconButtonModel>;
            }
        }

    }
}
