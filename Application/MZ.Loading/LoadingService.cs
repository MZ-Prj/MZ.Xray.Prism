using MZ.Loading.Models;
using Prism.Mvvm;
using System.Collections.Generic;

namespace MZ.Loading
{
    public class LoadingService : BindableBase
    {
        private readonly Dictionary<string, LoadingModel> _matchers = [];

        public LoadingModel this[string regionName]
        {
            get
            {
                if (!_matchers.TryGetValue(regionName, out var model))
                {
                    model = new LoadingModel();
                    _matchers[regionName] = model;
                }
                return model;
            }
        }
    }
}
