using Prism.Events;
using static MZ.Core.MZModel;

namespace MZ.Core
{
    public static partial class MZEvent
    {
        public class SplashCloseEvent : PubSubEvent { }
        public class NavigationEvent : PubSubEvent<NavigationModel> { }
    }

}
