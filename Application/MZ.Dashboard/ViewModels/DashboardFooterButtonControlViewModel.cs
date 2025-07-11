using MZ.Core;
using MZ.Domain.Models;
using Prism.Ioc;
using Prism.Services.Dialogs;
using System;
using System.Collections.ObjectModel;

namespace MZ.Dashboard.ViewModels
{
    public class DashboardFooterButtonControlViewModel : MZBindableBase, IDialogAware
    {

        #region Models
        private ObservableCollection<IconButtonModel> _actionButtons;
        public ObservableCollection<IconButtonModel> ActionButtons { get => _actionButtons; set => SetProperty(ref _actionButtons, value); }

        private string _title;
        public string Title { get => _title; set => SetProperty(ref _title, value); }

        public event Action<IDialogResult> RequestClose;
        #endregion

        public DashboardFooterButtonControlViewModel(IContainerExtension container) : base(container)
        {
        }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {

            if (parameters.ContainsKey("Title"))
            {
                Title = parameters.GetValue<string>("Title");
            }

            if (parameters.ContainsKey("ActionButtons"))
            {
                ActionButtons = parameters.GetValue<ObservableCollection<IconButtonModel>>("ActionButtons");
            }

        }
    }
}
