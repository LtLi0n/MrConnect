using Microsoft.Extensions.Configuration;

namespace LionLibrary.Utils
{
    ///<summary>JSON config file helper. Implement your own class that inherits from this.</summary>
    public abstract class AppConfigBase
    {
        internal IConfigurationRoot _configRoot;
        protected IConfiguration Config => _configRoot;

        public AppConfigBase() 
        {
            _configRoot = new ConfigurationBuilder()
                .Build();
        }

        public string this[string key]
        {
            get => Config[key];
            set => Config[key] = value;
        }

        public T Value<T>(string key) => Config.GetValue<T>(key);

        /*
         * Examples of how to create your own config implementation:
         * 
            public string AppTitle
            {
                get => this["info:title"];
                set => this["info:title"] = value;
            }
        
            public double AppVersion
            {
                get => Value<double>("info:version");
                set => Config["info:version"] = value.ToString();
            }

        */
    }
}
