﻿using Microsoft.Extensions.Configuration;

namespace MrConnect.Boot
{
    public interface IAppConfig
    {
        IConfigurationRoot ConfigRoot { get; }

        public ulong OwnerId { get; }

        string this[string key] { get; set; }
        T GetValue<T>(string key);
    }
}