using System.Reflection;
using System.Runtime.InteropServices;
using System;

namespace MZ.Resource
{
    public static class BuildVersionService
    {
        public static string BuildVersion { get; set; }

        public static void Load(this Assembly assembly)
        {
            BuildVersion = GetBuildVersion(assembly);
        }

        public static string GetBuildVersion(this Assembly assembly)
        {
            ArgumentNullException.ThrowIfNull(assembly);
            Version version = assembly.GetName().Version;
            return version != null ? version.ToString() : "Unknown Version";
        }

        public static string GetBuildVersion()
        {
            return Assembly.GetExecutingAssembly().GetBuildVersion();
        }

        public static string GetBuildConfiguration()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var configAttribute = assembly.GetCustomAttribute<AssemblyConfigurationAttribute>();
            return configAttribute?.Configuration ?? "Unknown";
        }

        public static string GetProcessArchitecture()
        {
            return RuntimeInformation.ProcessArchitecture.ToString();
        }

        public static string GetBuildInformation(Assembly assembly)
        {
            string version = GetBuildVersion(assembly);
            string config = GetBuildConfiguration();
            string architecture = GetProcessArchitecture();
            return $"Version : {version}-{config}-{architecture}";
        }
    }
}
