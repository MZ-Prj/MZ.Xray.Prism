using System;

#nullable enable
namespace MZ.Util
{
    public class MZComponentInfo
    {
        public string Message { get; set; } = string.Empty;
        public double Version { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Summary { get; set; } = string.Empty;
    }

    public class MZComponentAttribute : Attribute
    {
        public string Message { get; }
        public double Version { get; set; }
        public DateTime CreatedAt { get; set; }

        public MZComponentAttribute(string message, double version)
        {
            Message = message;
            Version = version;
            CreatedAt = DateTime.Now;
        }

        public string GetSummary()
        {
            return $"[{Version}-{CreatedAt}] : {Message} ";
        }
    }

    public static class MZComponentHelper
    {
        public static MZComponentInfo? Summary(Type targetType)
        {
            var attribute = Attribute.GetCustomAttribute(targetType, typeof(MZComponentAttribute)) as MZComponentAttribute;
            
            if (attribute == null)
            {
                return null;
            }

            return new MZComponentInfo
            {
                Message = attribute.Message,
                Version = attribute.Version,
                CreatedAt = attribute.CreatedAt,
                Summary = attribute.GetSummary()
            };
        }

    }
}
