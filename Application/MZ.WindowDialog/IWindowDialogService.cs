using Prism.Regions;
using System.Threading.Tasks;
using System.Windows;

namespace MZ.WindowDialog
{
    public interface IWindowDialogService
    {
        public Task ShowWindow(string title, string regionName, NavigationParameters parameters = null);

        public Task ShowWindow(string title, string regionName, bool isMultiple, ResizeMode resizeMode = ResizeMode.CanResize, int width = 1024, int height = 768, NavigationParameters parameters = null);
    }
}
