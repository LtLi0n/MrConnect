using System;

namespace WoT.Shared
{
    [Flags]
    public enum UserSettings : ulong
    {
        Default = 0,

        AlertsOn = 0b1 << 0,
        PhoneMode = 0b1 << 1,
        OldProfileMode = 0b1 << 2
    }
}
