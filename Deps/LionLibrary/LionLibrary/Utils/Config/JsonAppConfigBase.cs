using Microsoft.Extensions.Configuration;

namespace LionLibrary.Utils
{
    public abstract class JsonAppConfigBase : AppConfigBase
    {
        public JsonAppConfigBase(string path)
        {
            _configRoot = new ConfigurationBuilder()
                .AddJsonFile(path)
                .Build();
        }
    }
}
