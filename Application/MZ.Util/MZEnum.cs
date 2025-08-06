using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Reflection;

namespace MZ.Util
{
    /// <summary>
    /// Enum 관련 확장 기능을 제공
    /// </summary>
    public class MZEnum
    {
        /// <summary>
        /// 해당 Enum의 모든 값 리스트 반환
        /// </summary>
        public static List<T> GetList<T>()
        {
            return [.. Enum.GetValues(typeof(T)) as T[]];
        }

        /// <summary>
        /// Enum 값의 Name 값 반환
        /// </summary>
        public static string GetName<TEnum>(TEnum enumValue) where TEnum : Enum
        {
            Type enumType = enumValue.GetType();
            string name = Enum.GetName(enumType, enumValue);

            if (name != null)
            {
                FieldInfo field = enumType.GetField(name);
                if (field != null)
                {
                    if (Attribute.GetCustomAttribute(field, typeof(DisplayAttribute)) is DisplayAttribute attribute)
                    {
                        return attribute.Name;
                    }
                }
            }

            return enumValue.ToString();
        }

        /// <summary>
        /// Enum 값의 Description 값 반환
        /// </summary>
        public static string GetDescription<TEnum>(TEnum enumValue) where TEnum : Enum
        {
            Type enumType = enumValue.GetType();
            string name = Enum.GetName(enumType, enumValue);

            if (name != null)
            {
                FieldInfo field = enumType.GetField(name);
                if (field != null)
                {
                    if (Attribute.GetCustomAttribute(field, typeof(DisplayAttribute)) is DisplayAttribute attribute)
                    {
                        return attribute.Description;
                    }
                }
            }

            return enumValue.ToString();
        }

        /// <summary>
        /// Enum 값의 Order 값 반환
        /// </summary>
        public static int GetOrder<TEnum>(TEnum enumValue) where TEnum : Enum
        {
            Type enumType = enumValue.GetType();
            string name = Enum.GetName(enumType, enumValue);

            if (name != null)
            {
                FieldInfo field = enumType.GetField(name);
                if (field != null)
                {
                    if (Attribute.GetCustomAttribute(field, typeof(DisplayAttribute)) is DisplayAttribute attribute)
                    {
                        return attribute.Order;
                    }
                }
            }
            return 0;
        }

        /// <summary>
        /// 입력 문자열과 일치하는 Enum 값을 Name 으로 반환
        /// </summary>
        public static TEnum? Get<TEnum>(string value) where TEnum : struct, Enum
        {
            foreach (TEnum enumValue in Enum.GetValues(typeof(TEnum)))
            {
                string name = GetName(enumValue);
                if (name.Equals(value, StringComparison.OrdinalIgnoreCase))
                {
                    return enumValue;
                }
            }
            return null;
        }
    }
}
