using System.Reflection;
using System.Runtime.InteropServices;
using System;

namespace MZ.Resource
{
    /// <summary>
    /// 실행 파일의 빌드 버전, 빌드 구성, 아키텍처 등 정보를 제공
    /// </summary>
    public static class BuildVersionService
    {
        /// <summary>
        /// 빌드 버전 문자열 저장 프로퍼티
        /// </summary>
        public static string BuildVersion { get; set; }

        public static void Load(this Assembly assembly)
        {
            BuildVersion = GetBuildInformation(assembly);
        }

        /// <summary>
        /// 어셈블리에서 버전 정보 추출
        /// </summary>
        public static string GetBuildVersion(this Assembly assembly)
        {
            ArgumentNullException.ThrowIfNull(assembly);
            Version version = assembly.GetName().Version;
            return version != null ? version.ToString() : "Unknown Version";
        }

        /// <summary>
        /// 어셈블리의 버전 정보 반환
        /// </summary>
        public static string GetBuildVersion()
        {
            return Assembly.GetExecutingAssembly().GetBuildVersion();
        }

        /// <summary>
        /// 어셈블리의 빌드 구성
        /// </summary>
        public static string GetBuildConfiguration()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var configAttribute = assembly.GetCustomAttribute<AssemblyConfigurationAttribute>();
            return configAttribute?.Configuration ?? "Unknown";
        }

        /// <summary>
        /// 프로세스의 아키텍처 정보 반환
        /// </summary>
        public static string GetProcessArchitecture()
        {
            return RuntimeInformation.ProcessArchitecture.ToString();
        }

        /// <summary>
        /// 어셈블리의 버전, 빌드 구성, 아키텍처 정보 포맷
        /// </summary>
        public static string GetBuildInformation(Assembly assembly)
        {
            string version = GetBuildVersion(assembly);
            string config = GetBuildConfiguration();
            string architecture = GetProcessArchitecture();
            return $"Version : {version}-{config}-{architecture}";
        }
    }
}
