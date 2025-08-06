using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Prism.Mvvm;

namespace MZ.Core
{
    /// <summary>
    /// 런타임에 객체의 특정 프로퍼티를 동적으로 바인딩/편집할 수 있게 하는 ViewModel 역할의 클래스  
    /// - 인스턴스와 PropertyInfo 참조를 보관 및 값 변경 시 양방향 동기화됨
    /// </summary>
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

    /// <summary>
    /// 객체의 PropertyInfo들을 자동으로 PropertyItemModel 컬렉션에 바인딩/등록하는 헬퍼 클래스  
    /// - 타입 T의 모든 public settable property 중, allowedTypes에 포함된 타입만 필터링해서 추가  
    /// </summary>
    [Obsolete("Reflection에 전적으로 의존하여, 런타임 성능 저하 ")]
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
