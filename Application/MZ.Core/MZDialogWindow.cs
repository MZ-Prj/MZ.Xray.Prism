using Prism.Services.Dialogs;
using System.Windows.Data;
using System.Windows;
using MahApps.Metro.Controls;

namespace MZ.Core
{
    /// <summary>
    /// 대화 상자용 MetroWindow 커스텀 클래스
    /// </summary>
    public class MZDialogMetroWindowChrome : MetroWindow, IDialogWindow
    {
        public IDialogResult Result { get; set; }

        public MZDialogMetroWindowChrome()
        {
            this.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            this.SizeToContent = SizeToContent.WidthAndHeight;
            this.DataContextChanged += DialogWindowDataContextChanged;
        }

        /// <summary>
        /// DataContext가 변경될 때 호출되는 이벤트
        /// </summary>
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
