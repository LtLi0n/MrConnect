﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SharedWoT.SQL
{
    [Owned]
    public class Character_Skills
    {
        [Column("mining_xp")]
        public ulong MiningXp { get; set; } = 0;

        [Column("woodcutting_xp")]
        public ulong WoodcuttingXp { get; set; } = 0;
    }
}