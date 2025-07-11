using MZ.Domain.Models;
using Prism.Events;
using static MZ.Core.MZModel;

namespace MZ.Event
{
    /// <summary>
    /// Navigate Event
    /// </summary>
    public static partial class MZEvent
    {
        public class SplashCloseEvent : PubSubEvent { }
        public class DashboardNavigationEvent : PubSubEvent<NavigationModel> { }
        public class AnalysisNavigationEvent : PubSubEvent<NavigationModel> { }
        
        public class WindowOpenEvent : PubSubEvent<string> { }
        public class WindowCloseEvent : PubSubEvent<string> { }
        public class WindowHideEvent : PubSubEvent<string> { }
        
    }
   
    /// <summary>
    /// XrayService Event
    /// </summary>
    public static partial class MZEvent
    {
        public class FileReceiveEvent : PubSubEvent<FileModel> { }
    }

}
