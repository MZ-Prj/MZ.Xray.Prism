using System;
using System.Linq;
using System.Reflection;

namespace MZ.Util
{
    /// <summary>
    /// 객체 복사 및 매핑 관련 확장 메서드를 제공하는 유틸리티 클래스
    /// </summary>
    [Obsolete("TODO : Deep Clone 검증 수행 ")]
    public static class MZObject
    {
        /// <summary>
        /// 같은 타입의 두 객체 간 public 속성 값 전체 복사
        /// </summary>
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

        /// <summary>
        /// 지정한 속성명을 제외하고 복사(새 객체 반환)
        /// </summary>
        public static T CopyToWithoutProperties<T>(this T source, params string[] propertiesToExclude) where T : new()
        {
            T target = new();
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

        /// <summary>
        /// "Id" 속성을 제외하고 복사(새 객체 반환)
        /// </summary>
        public static T CopyToWithoutId<T>(this T source) where T : new()
        {
            return source.CopyToWithoutProperties("Id");
        }

        /// <summary>
        /// 서로 다른 타입의 객체 간 동일한 이름/타입의 속성 매핑 (새 객체 반환)
        /// </summary>
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

        /// <summary>
        /// 값이 null이 아니면 action을 수행
        /// </summary>
        public static void Let<T>(this T value, Action<T> action)
        {
            if (value != null)
            {
                action(value);
            }
        }
    }
}
