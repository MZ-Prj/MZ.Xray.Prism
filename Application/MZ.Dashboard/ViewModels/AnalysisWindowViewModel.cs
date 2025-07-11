using System.Windows.Input;
using MZ.Core;
using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using static MZ.Core.MZModel;
using static MZ.Event.MZEvent;

namespace MZ.Dashboard.ViewModels
{
    public class AnalysisWindowViewModel : MZBindableBase
    {

        private DelegateCommand windowClosingCommand;
        public ICommand WindowClosingCommand => windowClosingCommand ??= new DelegateCommand(WindowClosing);


        public AnalysisWindowViewModel(IContainerExtension container) : base(container)
        {
            base.Initialize();
        }

        public override void InitializeEvent()
        {
            _eventAggregator.GetEvent<AnalysisNavigationEvent>().Subscribe((NavigationModel model) =>
            {
                _regionManager.RequestNavigate(model.Region, model.View);
            }, ThreadOption.UIThread, true);
        }

        private void WindowClosing()
        {
            _eventAggregator.GetEvent<WindowCloseEvent>().Publish(MZWindowNames.AnalysisWindow);

        }
    }
}
