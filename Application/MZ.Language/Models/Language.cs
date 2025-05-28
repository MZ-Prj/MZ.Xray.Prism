using Prism.Mvvm;

namespace MZ.Language.Models
{
    public class LanguageModel : BindableBase
    {
        private string _displayName;
        public string DisplayName { get => _displayName; set => SetProperty(ref _displayName, value); }

        private string _cultureCode;
        public string CultureCode { get => _cultureCode; set => SetProperty(ref _cultureCode, value); }
    }
}
