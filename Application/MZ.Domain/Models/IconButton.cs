using MZ.Domain.Interfaces;
using Prism.Mvvm;
using System.Windows.Input;
using System.Windows.Media;

namespace MZ.Domain.Models
{
    public class IconButtonModel : BindableBase, IUserButton
    {
        private int _id;
        public int Id { get => _id; set => SetProperty(ref _id, value); }

        private object _uid;
        public object UId { get => _uid; set => SetProperty(ref _uid, value); }

        private bool _isVisibility = true;
        public bool IsVisibility { get => _isVisibility; set => SetProperty(ref _isVisibility, value); }

        private string _iconKind;
        public string IconKind { get => _iconKind; set => SetProperty(ref _iconKind, value); }

        private string _name;
        public string Name { get => _name; set => SetProperty(ref _name, value); }

        private string _toolTip;
        public string ToolTip { get => _toolTip; set => SetProperty(ref _toolTip, value); }

        private ICommand _command;
        public ICommand Command { get => _command; set => SetProperty(ref _command, value); }

        private Brush _colorBrush;
        public Brush ColorBrush { get => _colorBrush; set => SetProperty(ref _colorBrush, value); }

        public IconButtonModel(string iconKind, ICommand command, Brush colorBrush = null, bool isVisibility = true, object uid = null, string name = null, string tooltip = "tooltip", int id = 0)
        {
            Id = id;
            IsVisibility = isVisibility;
            Command = command;
            IconKind = iconKind;
            ColorBrush = colorBrush;
            UId = uid ?? iconKind;
            ToolTip = tooltip;
            Name = name;
        }
    }
}
