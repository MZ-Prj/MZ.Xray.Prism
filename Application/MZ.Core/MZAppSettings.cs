using Microsoft.Extensions.Configuration;

namespace MZ.Core
{
    public interface IMZAppSetting
    {
        string GetValue(string key);
    }

    public class MZAppSettings
    {
        private readonly IConfiguration _configuration;
        
        public MZAppSettings(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetValue(string key)
        {
            return _configuration[key];
        }
    }
}
