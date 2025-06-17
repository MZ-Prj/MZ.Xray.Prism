using MZ.Core;
using MZ.Domain.Models;
using MZ.Loading;
using MZ.Sidebar.Models;
using MZ.Sidebar.Views;
using Prism.Ioc;
using Prism.Regions;
using System.Collections.ObjectModel;

namespace MZ.Dashboard.ViewModels
{
    public class DashboardControlViewModel : MZBindableBase
    {
        public DashboardControlViewModel(IContainerExtension container) : base(container)
        {
        }

    }
}