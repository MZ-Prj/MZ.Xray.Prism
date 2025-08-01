using MZ.Domain.Entities;
using MZ.Domain.Models;
using Prism.Mvvm;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MZ.Xray.Engine
{
    public class UIProcesser : BindableBase
    {
        public ObservableCollection<IconButtonModel> ActionButtons { set; get; } = [];

        public void LoadActionButton(IEnumerable<UserButtonEntity> buttonSettings)
        {
            if (buttonSettings == null)
            {
                return;
            }

            ActionButtons.ToList().ForEach(button =>
            {
                var setting = buttonSettings.FirstOrDefault(b => b.Name == button.Name);
                button.Id = setting.Id;
                button.IsVisibility = setting?.IsVisibility ?? true;
            });
        }

    }
}
