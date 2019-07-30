using System;

namespace SharedDiscord
{
    [Flags]
    public enum UserFlags : int
    {
        None = 0b0,
        Discord_Employee = 0b1 << 0,
        Discord_Partner = 0b1 << 1,
        HypeSquad_Events = 0b1 << 2,
        Bug_Hunter = 0b1 << 3,
        House_Bravery = 0b1 << 6,
        House_Brilliance = 0b1 << 7,
        House_Balance = 0b1 << 8,
        Early_Supporter = 0b1 << 9,
        Team_User = 0b1 << 10,
    }
}
