using Prism.Events;
using static MZ.Core.MZModel;

namespace MZ.Core
{
    public static partial class MZEvent
    {
        public class SplashCloseEvent : PubSubEvent { }
        public class DashboardNavigationEvent : PubSubEvent<NavigationModel> { }
        public class AnalysisNavigationEvent : PubSubEvent<NavigationModel> { }
    }

}
