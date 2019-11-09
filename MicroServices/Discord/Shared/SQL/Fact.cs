using LionLibrary.SQL;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text;

namespace Discord.Shared
{
    [Table("facts")]
    public class Fact : IEntity<Fact, uint>
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

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime AddedAt { get; set; }

        public static void CreateModel(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Fact>(x =>
            {
                x.HasIndex(fact => fact.Content).IsUnique();
            });
        }
    }
}
