using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using LionLibrary.SQL;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace SharedWoT.SQL
{
    [Table("user")]
    public class User : IEntity<User, uint>
    {
        [Key] [Column("id")]
        public uint Id { get; set; }

        [Column("discord_id")]
        public ulong DiscordId { get; set; }

        [JsonIgnore] [IgnoreDataMember]
        public ICollection<Character> Characters { get; set; }

        public static void CreateModel(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(x =>
            {
                x.HasMany(user => user.Characters)
                .WithOne(character => character.User)
                .HasForeignKey(character => character.UserId);

                x.HasIndex(x => x.DiscordId).IsUnique();
            });
        }
    }
}
