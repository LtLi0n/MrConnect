using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using DataServerHelpers;
using LionLibrary.SQL;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace WoT.Shared
{
    [Table("users")]
    public class User : IEntity<User, uint>
    {
        public static class Ref
        {
            public const string DiscordId = "discord_id";
            public const string IsPremium = "premium?";
            public const string Settings = "settings";
        }

        [Key, Column(SharedRef.Id)]
        public uint Id { get; set; }

        [Required, Column(Ref.DiscordId)]
        public ulong DiscordId { get; set; }

        [Column(Ref.IsPremium)]
        public bool IsPremium { get; set; } = false;

        [Column(Ref.Settings)]
        public UserSettings Settings { get; set; } = UserSettings.Default;

        [JsonIgnore, IgnoreDataMember]
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
