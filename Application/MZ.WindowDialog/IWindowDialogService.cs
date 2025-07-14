using Prism.Regions;
using System.Threading.Tasks;

namespace MZ.WindowDialog
{
    public interface IWindowDialogService
    {
        public Task ShowWindow(string title, string regionName, NavigationParameters parameters = null);
        public Task ShowWindow(string title, string regionName, bool isMultiple, NavigationParameters parameters = null);
    }
}
