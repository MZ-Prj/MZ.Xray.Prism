using MahApps.Metro.Controls;
using MZ.WindowDialog.ViewModels;
using MZ.WindowDialog.Views;
using Prism.Ioc;
using Prism.Regions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;

namespace MZ.WindowDialog
{
    public class WindowDialogService : IWindowDialogService
    {
        private readonly IContainerProvider _containerProvider;
        private readonly IRegionManager _regionManager;

        private readonly Dictionary<string, List<MetroWindow>> _windows = [];

        public WindowDialogService(IContainerProvider containerProvider, IRegionManager regionManager)
        {
            _containerProvider = containerProvider;
            _regionManager = regionManager;
        }

        public Task ShowWindow(string title, string regionName, NavigationParameters parameters = null)
        {
            return ShowWindow(title, regionName, isMultiple: true, parameters: parameters);
        }

        public Task ShowWindow(string title, string regionName, bool isMultiple, ResizeMode resizeMode = ResizeMode.CanMinimize, int width = 1024, int height = 768, NavigationParameters parameters = null)
        {

            // isMultiple == false & Opened Window
            if (!isMultiple
                && _windows.TryGetValue(regionName, out var list)
                && list.FirstOrDefault(w => w.IsVisible) is MetroWindow existing)
            {
                existing.Activate();
                return Task.CompletedTask;
            }

            var window = _containerProvider.Resolve<WindowDialogView>();
            var regionManager = (_regionManager as RegionManager).CreateRegionManager();
            RegionManager.SetRegionManager(window, regionManager);

            if (window.DataContext is WindowDialogViewModel vm)
            {
                vm.Title = title;
                vm.RegionName = regionName;
            }

            regionManager.RequestNavigate(regionName, regionName, parameters);

            if (!_windows.ContainsKey(regionName))
            {
                _windows[regionName] = [];
            }

            window.ResizeMode = resizeMode;
            window.Width = width;
            window.Height = height;
            
            _windows[regionName].Add(window);

            window.Closed += (s, e) =>
            {
                _windows[regionName].Remove(window);
                if (_windows[regionName].Count == 0)
                {
                    _windows.Remove(regionName);
                }
            };

            window.Show();
            return Task.CompletedTask;
        }
    }
}
