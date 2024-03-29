﻿using LionLibrary.Commands;
using Microsoft.EntityFrameworkCore;

namespace WoT.Server.Network.Commands
{
    ///<summary>Use it to not forget mandatory methods for back-end data filling.</summary>
    ///<typeparam name="T">Data object.</typeparam>
    public interface IWoTDbModule<T> : IDataModule<T> where T : class
    {
        WoTDbContext SQL { get; set; }
        DbContext DbContext { get; }
    }
}
