using LionLibrary.Framework;
using MrConnect.Boot;
using System;
using System.Collections.Generic;
using System.Text;

namespace MrConnect.Services
{
    public class WoTService : ServerCommunicator, IWoTService
    {
        public WoTService(IServiceProvider services, IAppConfig config) : base(services, config, null)
        {
        }
    }
}
