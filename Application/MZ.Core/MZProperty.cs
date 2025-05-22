using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Prism.Mvvm;

namespace MZ.Core
{
    public class PropertyItemModel : BindableBase
    {
        private readonly object _Instance;
        private readonly PropertyInfo _propertyInfo;
        private object _value;
        public object Value
        {
            get => _value;
            set
            {
                if (SetProperty(ref _value, value))
                {
                    _propertyInfo.SetValue(_Instance, value);
                }
            }
        }
        public string Name => _propertyInfo.Name;
        public Type PropertyType => _propertyInfo.PropertyType;

        public PropertyItemModel(object instance, PropertyInfo propertyInfo)
        {
            _Instance = instance;
            _propertyInfo = propertyInfo;
            _value = propertyInfo.GetValue(instance);

            if (_Instance is INotifyPropertyChanged notifyPropertyChanged)
            {
                notifyPropertyChanged.PropertyChanged += OnInstancePropertyChanged;
            }
        }
        private void OnInstancePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == _propertyInfo.Name)
            {
                _value = _propertyInfo.GetValue(_Instance);
                RaisePropertyChanged(nameof(Value));
            }
        }
    }

    public class MZProperty
    {
        public static void Set<T>(T target, ICollection<PropertyItemModel> propertyItems, params Type[] allowedTypes)
        {
            var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var prop in props)
            {
                if (prop.CanRead && prop.CanWrite)
                {
                    if (allowedTypes.Contains(prop.PropertyType))
                    {
                        propertyItems.Add(new PropertyItemModel(target, prop));
                    }
                }
            }
        }
    }
}
