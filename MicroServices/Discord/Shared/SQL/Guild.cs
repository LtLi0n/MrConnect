using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;

namespace Discord.Shared
{
    [Table("guilds")]
    public class Guild : IDiscordEntity<Guild, ulong>
    {
        public static class Ref
        {
            public const string OwnerId = "OwnerId";
            public const string Name = "Name";
            public const string Prefix = "Prefix";
        }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public ulong Id { get; set; }

        [Required]
        public ulong OwnerId { get; set; }
        [JsonIgnore, IgnoreDataMember]
        public User Owner { get; set; }

        [Required, Column(TypeName = "nvarchar(100)")]
        public string Name { get; set; }

        [Column(TypeName = "nvarchar(16)")]
        public string? Prefix { get; set; }

        public DateTime LastUpdatedAt { get; set; } = DateTime.Now;
        public DateTime AddedAt { get; set; } = DateTime.Now;

        public static void CreateModel(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Guild>(x =>
            {

            });
        }
    }
}
