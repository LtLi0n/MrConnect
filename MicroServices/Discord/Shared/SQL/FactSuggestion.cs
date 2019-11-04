using LionLibrary.SQL;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace Discord.Shared
{
    [Table("facts_suggestions")]
    public class FactSuggestion : IDiscordEntity<FactSuggestion, uint>
    {
        public static class Ref
        {
            public const string UserId = "UserId";
            public const string Content = "Content";
        }

        [Key]
        public uint Id { get; set; }

        [Required]
        public ulong UserId { get; set; }
        [JsonIgnore, IgnoreDataMember]
        public User User { get; set; }

        [Required]
        public string Content { get; set; }

        public DateTime LastUpdatedAt { get; set; }
        public DateTime AddedAt { get; set; } = DateTime.Now;

        public static void CreateModel(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FactSuggestion>(x =>
            {

            });
        }
    }
}
