using MZ.Util;
using Prism.Mvvm;
using System;

namespace MZ.Domain.Models
{
    public class LoadingModel : BindableBase
    {
        private bool _isLoading = false;
        public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }

        private string _message = "Loading...";
        public string Message { get => _message; set => SetProperty(ref _message, value); }

        public IDisposable Show(string message = null)
        {
            if (!string.IsNullOrEmpty(message))
            {
                Message = message;
            }

            return MZDisposable.LoadingWrapper((isLoading) =>
            {
                IsLoading = isLoading;
            });
        }
    }
}
