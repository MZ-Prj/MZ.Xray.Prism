using MZ.Sidebar.Models;
using Prism.Events;

namespace MZ.Sidebar
{
    public static partial class MZEvents
    {
        public class Add : PubSubEvent<MenuModel> { }
        public class Delete : PubSubEvent<int> { }
        public class Select : PubSubEvent<MenuModel> { }
    }

}
