using Discord.Commands;
using LionLibrary.SQL;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SharedDiscord
{
    [Table("user")]
    public class User : IEntity<User, ulong>
    {
        public static class Ref
        {
            public const string id = "id";
            public const string username = "username";
            public const string discriminator = "discriminator";
        }

        [Key, Column(Ref.id)]
        public ulong Id { get; set; }

        [Column(Ref.username, TypeName = "nvarchar(255)")]
        public string Username { get; set; }

        [Column(Ref.discriminator, TypeName = "nvarchar(255)")]
        public string Discriminator { get; set; }
    }
}
