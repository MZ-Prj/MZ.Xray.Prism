using MZ.Core;
using MZ.Sidebar.Models;
using MZ.Util;
using Prism.Commands;
using Prism.Ioc;
using Prism.Regions;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using static MZ.Sidebar.MZEvents;

namespace MZ.Sidebar.ViewModels
{
    public class SidebarViewModel : MZBindableBase
    {
        #region Params

        private ObservableCollection<MenuModel> _menus = [];
        public ObservableCollection<MenuModel> Menus { get => _menus; set => SetProperty(ref _menus, value); }

        private MenuModel selectedMenu;
        public MenuModel SelectedMenu 
        {
            get => selectedMenu;
            set 
            {
                if(SetProperty(ref selectedMenu, value))
                {
                    OnSelectedMenu();
                }
            } 
        }

        private bool _isExpand = true;
        public bool IsExpand { get => _isExpand; set => SetProperty(ref _isExpand, value); }
        #endregion

        #region Command
        private DelegateCommand _toggleExpandCommand;
        public ICommand ToggleExpandCommand => _toggleExpandCommand ??= new(MZAction.Wrapper(ToggleExapndButton));
        #endregion

        public SidebarViewModel(IContainerExtension container) : base(container)
        {
            base.Initialize();
        }

        public override void InitializeEvent()
        {
            SubscribeEvent<Add, MenuModel>(OnEventAddMenu);
            SubscribeEvent<Delete, int>(OnEventDeleteMenu);
        }

        private void OnEventDeleteMenu(int index)
        {
            Menus.RemoveAt(index);
        }

        private void OnEventAddMenu(MenuModel menu)
        {
            Menus.Add(menu);
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            if (navigationContext.Parameters.TryGetValue("Menus", out ObservableCollection<MenuModel> menus))
            {
                Menus = menus;
                SelectedMenu = Menus.FirstOrDefault();
            }
        }

        private void ToggleExapndButton()
        {
            IsExpand = !IsExpand;
        }

        private void OnSelectedMenu()
        {
            _eventAggregator.GetEvent<Select>().Publish(SelectedMenu);
        }

    }
}
