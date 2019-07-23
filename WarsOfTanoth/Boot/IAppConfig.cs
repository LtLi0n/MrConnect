using Microsoft.Extensions.Configuration;

namespace WarsOfTanoth.Boot
{
    public interface IAppConfig
    {
        IConfigurationRoot ConfigRoot { get; }

        string this[string key] { get; set; }
        T GetValue<T>(string key);
    }
}