using MZ.Core;
using MZ.Toolbar.Models;
using Prism.Ioc;
using Prism.Regions;
using System.Collections.ObjectModel;

namespace MZ.Toolbar.ViewModels
{
    public class ToolbarViewModel : MZBindableBase
    {
        private ObservableCollection<MenuModel> _menus = [];
        public ObservableCollection<MenuModel> Menus { get => _menus; set => SetProperty(ref _menus, value); }

        public ToolbarViewModel(IContainerExtension container) : base(container)
        {

        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            if (navigationContext.Parameters.TryGetValue("Menus", out ObservableCollection<MenuModel> menus))
            {
                Menus = menus;
            }
        }

    }
}
