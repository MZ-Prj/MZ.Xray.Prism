using Prism.Mvvm;
using System;
using System.Threading;

namespace MZ.Sidebar.Models
{
    public class MenuModel : BindableBase
    {
        private static int _lastIndex = -1; 

        private int _index;
        public int Index { get => _index; set => SetProperty(ref _index, value); }

        private string _iconKind = string.Empty;
        public string IconKind { get => _iconKind; set => SetProperty(ref _iconKind, value); }

        private string _title = string.Empty;
        public string Title { get => _title; set => SetProperty(ref _title, value); }

        public MenuModel()
        {
            _index = Interlocked.Increment(ref _lastIndex);
        }
    }
}
