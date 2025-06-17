using Prism.Mvvm;
using System.Windows.Input;

namespace MZ.Toolbar.Models
{
    public class MenuModel : BindableBase
    {
        private string _iconKind;
        public string IconKind { get => _iconKind; set => SetProperty(ref _iconKind, value); }

        private ICommand _command;
        public ICommand Command { get => _command; set => SetProperty(ref _command, value); }

    }
}
