using LionLibrary.SQL;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WoT.Shared
{
    [Table("characters_work")]
    public class CharacterWork : ICharacterEntity<CharacterWork>
    {
        public static class Ref
        {
            public const string IsWorking = "IsWorking";
            public const string StartedWorkAt = "StartedWorkAt";
            public const string WorkFinishesAt = "WorkFinishesAt";
            public const string TotalHours = "TotalHours";
        }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public uint CharacterId { get; set; }
        public Character Character { get; set; }

        public bool IsWorking { get; set; }
        public DateTime StartedWorkAt { get; set; }
        public DateTime WorkFinishesAt { get; set; }
        public uint TotalHours { get; set; }

        public static void CreateModel(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CharacterWork>();
        }
    }
}
