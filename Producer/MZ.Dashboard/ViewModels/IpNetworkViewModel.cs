using MZ.Core;
using MZ.Producer.Engine;
using Prism.Ioc;
using System.Collections.ObjectModel;

namespace MZ.Dashboard.ViewModels
{
    public class IpNetworkViewModel : MZBindableBase
    {
        #region Services
        private readonly IProducerService _producerService;
        #endregion

        private ObservableCollection<PropertyItemModel> _propertyItems = [];
        public ObservableCollection<PropertyItemModel> PropertyItems { get => _propertyItems; set => SetProperty(ref _propertyItems, value); }

        public IpNetworkViewModel(IContainerExtension container, IProducerService producerService) : base(container)
        {
            _producerService = producerService;

            base.Initialize();
        }

        public override void InitializeModel()
        {
            MZProperty.Set(_producerService.Socket.Model, PropertyItems, typeof(double), typeof(int), typeof(string), typeof(bool));
        }
    }
}
