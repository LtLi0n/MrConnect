using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using LionLibrary.SQL;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace WoT.Shared.SQL
{
    [Table("user")]
    public class User : IEntity<User, uint>
    {
        [Key] [Column("id")]
        public uint Id { get; set; }

        [Required, Column("discord_id")]
        public ulong DiscordId { get; set; }

        [Column("premium?")]
        public bool IsPremium { get; set; } = false;

        public UserSettings Settings { get; set; }

        [JsonIgnore] [IgnoreDataMember]
        public ICollection<Character> Characters { get; set; }

        public static void CreateModel(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(x =>
            {
                x.HasIndex(x => x.DiscordId).IsUnique();

                x.HasMany(user => user.Characters)
                .WithOne(character => character.User)
                .HasForeignKey(character => character.UserId);
            });
        }
    }
}
