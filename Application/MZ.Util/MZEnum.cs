using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Reflection;

namespace MZ.Util
{
    public class MZEnum
    {
        public static List<T> GetList<T>()
        {
            return [.. Enum.GetValues(typeof(T)) as T[]];
        }

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
    }
}
