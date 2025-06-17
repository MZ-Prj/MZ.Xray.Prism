using System.Collections.ObjectModel;
using System;
using Prism.Commands;
using Prism.Ioc;
using MZ.Core;
using MZ.Util;
using MZ.Resource;
using MZ.Domain.Enums;
using MZ.Language.Models;
using System.Windows.Input;

namespace MZ.Language.ViewModels
{
    public class LanguageViewModel : MZBindableBase
    {
        public ObservableCollection<LanguageModel> Languages { get; set; } = [];

        private DelegateCommand<string> _langueCommand;
        public ICommand LanguageCommand => _langueCommand ??= new DelegateCommand<string>(MZAction.Wrapper<string>(LanguageButton));

        public LanguageViewModel(IContainerExtension container) : base(container)
        {
            base.Initialize();
        }

        public override void InitializeModel()
        {
            foreach (LanguageRole language in Enum.GetValues(typeof(LanguageRole)))
            {
                string displayName = MZEnum.GetDescription(language);
                string cultureCode = MZEnum.GetName(language);

                Languages.Add(new LanguageModel() { DisplayName = displayName, CultureCode = cultureCode });
            }
        }

        private void LanguageButton(string cultureCode)
        {
            LanguageService.Load(cultureCode);
        }
    }
}
