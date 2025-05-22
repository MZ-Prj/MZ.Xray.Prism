using System;
using System.Linq;
using System.Reflection;

namespace MZ.Util
{
    public static class MZObject
    {
        public static void CopyTo<T>(this T source, T target) where T : class
        {
            if (source == null || target == null)
            {
                return;
            }

            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var prop in properties)
            {
                if (prop.CanRead && prop.CanWrite)
                {
                    var value = prop.GetValue(source);
                    prop.SetValue(target, value);
                }
            }
        }

        public static T CopyToWithoutProperties<T>(this T source, params string[] propertiesToExclude) where T : new()
        {
            T target = new T();
            var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var prop in props)
            {
                if (propertiesToExclude.Contains(prop.Name))
                {
                    continue;
                }

                if (!prop.CanWrite)
                {
                    continue;
                }

                var value = prop.GetValue(source);
                prop.SetValue(target, value);
            }
            return target;
        }

        public static T CopyToWithoutId<T>(this T source) where T : new()
        {
            return source.CopyToWithoutProperties("Id");
        }

        public static TDestination MapTo<TSource, TDestination>(this TSource source) where TDestination : new()
        {
            TDestination destination = new();

            var sourceProperties = typeof(TSource)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var destinationProperties = typeof(TDestination)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var sourceProp in sourceProperties)
            {
                if (!sourceProp.CanRead)
                    continue;

                var destProp = destinationProperties
                    .FirstOrDefault(p => p.Name == sourceProp.Name &&
                                         p.CanWrite &&
                                         p.PropertyType.IsAssignableFrom(sourceProp.PropertyType));
                if (destProp != null)
                {
                    var value = sourceProp.GetValue(source);
                    destProp.SetValue(destination, value);
                }
            }

            return destination;
        }

        public static void Let<T>(this T value, Action<T> action)
        {
            if (value != null)
            {
                action(value);
            }
        }
    }
}
