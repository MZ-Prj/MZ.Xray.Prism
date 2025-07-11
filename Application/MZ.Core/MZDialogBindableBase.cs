using Prism.Ioc;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using System.Windows.Threading;

namespace MZ.Core
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class MZDialogBindableBase : BindableBase
    {
        protected readonly Dispatcher _dispatcher = Dispatcher.CurrentDispatcher;

        protected IRegionManager _regionManager;
        protected IDialogService _dialogService;

        protected MZDialogBindableBase(IContainerExtension container)
        {
            InitializeServices(container);
        }

        protected void InitializeServices(IContainerExtension container)
        {
            _dialogService = container.Resolve<IDialogService>();
            _regionManager = container.Resolve<IRegionManager>();
        }
    }
}
