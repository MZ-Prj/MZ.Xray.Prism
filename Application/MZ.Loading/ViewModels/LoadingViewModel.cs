using MZ.Core;
using MZ.Domain.Models;
using Prism.Ioc;

namespace MZ.Loading.ViewModels
{
    public class LoadingViewModel : MZBindableBase
    {
        public LoadingViewModel(IContainerExtension container) : base(container)
        {
        }

    }
}
