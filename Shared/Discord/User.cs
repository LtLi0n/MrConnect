using LionLibrary.SQL;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SharedDiscord
{
    [Table("user")]
    public class User : IEntity<User, string>
    {
        [Key]
        [Column("id")]
        public string Id { get; set; }

        [Required]
        [Column("username")]
        public string Username { get; set; }

        [Column("discriminator")]
        public string Discriminator { get; set; }

        [Column("avatar")]
        public string Avatar { get; set; }

        [Column("bot?")]
        public bool? IsBot { get; set; }

        [Column("mfa_enabled?")]
        public bool MfaEnabled { get; set; }

        [Column("locale?")]
        public bool Locale { get; set; }

        [Column("verified?")]
        public bool Verified { get; set; }

        [Column("email?")]
        public string Email { get; set; }

        [Column("flags?")]
        public UserFlags Flags { get; set; }

        [Column("premium_type?")]
        public int PremiumType { get; set; }

        public ICollection<Emoji> Emojis { get; set; }

        public static void CreateModel(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(x =>
            {
                x.HasMany(user => user.Emojis)
                .WithOne(emoji => emoji.User)
                .HasForeignKey(emoji => emoji.UserId);
            });
        }
    }
}
