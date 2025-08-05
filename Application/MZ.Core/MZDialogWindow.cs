using Prism.Services.Dialogs;
using System.Windows.Data;
using System.Windows;
using MahApps.Metro.Controls;

namespace MZ.Core
{
    public class MZDialogMetroWindowChrome : MetroWindow, IDialogWindow
    {
        public IDialogResult Result { get; set; }

        public MZDialogMetroWindowChrome()
        {
            this.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            this.SizeToContent = SizeToContent.WidthAndHeight;
            this.DataContextChanged += DialogWindowDataContextChanged;
        }

        private void DialogWindowDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is IDialogAware dialogAware)
            {
                Binding titleBinding = new("Title")
                {
                    Source = dialogAware,
                    Mode = BindingMode.OneWay
                };
                this.SetBinding(Window.TitleProperty, titleBinding);
            }
        }
    }
}
