using System.Collections.ObjectModel;
using System;
using Prism.Commands;
using Prism.Ioc;
using MZ.Core;
using MZ.Util;
using MZ.Resource;
using MZ.Domain.Enums;
using MZ.Language.Models;

namespace MZ.Language.ViewModels
{
    public class LanguageViewModel : MZBindableBase
    {
        public ObservableCollection<LanguageModel> Languages { get; set; } = [];

        private DelegateCommand<string> _langueCommand;
        public DelegateCommand<string> LanguageCommand => _langueCommand ??= new DelegateCommand<string>(LanguageButton);

        public LanguageViewModel(IContainerExtension container) : base(container)
        {
        }

        public override void InitializeModel()
        {
            foreach (LanguageRole lang in Enum.GetValues(typeof(LanguageRole)))
            {
                string displayName = MZEnum.GetDescription(lang);
                string cultureCode = MZEnum.GetName(lang);

                Languages.Add(new LanguageModel() { DisplayName = displayName, CultureCode = cultureCode });
            }
            base.InitializeModel();
        }

        private void LanguageButton(string cultureCode)
        {
            LanguageService.Load(cultureCode);
        }
    }
}
