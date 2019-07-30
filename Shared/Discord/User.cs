using Discord.Commands;
using LionLibrary.SQL;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SharedDiscord
{
    [NamedArgumentType]
    [Table("user")]
    public class User : IEntity<User, string>
    {
        public static class Ref
        {
            public const string Id = "Id";
            public const string Username = "Username";
            public const string Discriminator = "Discriminator";
        }

        public string Id { get; set; }

        public string Username { get; set; }

        [Column(TypeName = "char(4)")]
        public string Discriminator { get; set; }

        public ICollection<GuildEmote> Emotes { get; set; }

        public static void CreateModel(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(x =>
            {
                x.HasIndex(user => user.Id).IsUnique();

                x.HasMany(user => user.Emotes)
                .WithOne(emote => emote.User)
                .HasForeignKey(emote => emote.UserId);
            });
        }
    }
}
