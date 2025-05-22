using System;
using MZ.Core;
using Prism.Ioc;
using Prism.Services.Dialogs;

namespace MZ.Dialog.ViewModels
{
    public class DialogViewModel : MZDialogBindableBase, IDialogAware
    {
        private string _title;
        public string Title { get => _title; set => SetProperty(ref _title, value); }

        private string _regionName;
        public string RegionName { get => _regionName; set => SetProperty(ref _regionName, value); }

        public event Action<IDialogResult> RequestClose;

        public DialogViewModel(IContainerExtension container) : base(container)
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
            if (parameters.ContainsKey("RegionName"))
            {
                RegionName = parameters.GetValue<string>("RegionName");
            }
            if (parameters.ContainsKey("Title"))
            {
                Title = parameters.GetValue<string>("Title");
            }
        }
    }
}
